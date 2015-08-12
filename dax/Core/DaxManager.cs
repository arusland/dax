/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using dax.Core.Events;
using dax.Db;
using dax.Db.Exceptions;
using dax.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dax.Core
{
    public class DaxManager
    {
        private const String PROPERTY_QUERY_VERSION = "version.query";
        private DaxDocument _document;
        private String _scopeVersion;
        private readonly TaskScheduler _uiContext;
        private List<Group> _groups;
        public event EventHandler<QueryReloadedEventArgs> OnQueryReloaded;
        public event EventHandler<NewBlockAddedEventArgs> OnNewBlockAdded;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<QueryProviderEventArgs> OnQueryProvider;
        public event EventHandler<EventArgs> OnScopeVersionChanged;
        public event EventHandler<EventArgs> OnGroupsChanged;

        public DaxManager(String filePath, TaskScheduler uiContext)
        {
            _uiContext = uiContext;
            _document = DaxDocument.Load(filePath);
        }

        public IEnumerable<Input> Inputs
        {
            get
            {
                return _document.Inputs;
            }
        }

        public IEnumerable<Group> Groups
        {
            get
            {
                return _groups ?? new List<Group>();
            }
            private set
            {
                if (_groups == null || !_groups.SequenceEqual(value))
                {
                    _groups = (List<Group>)value;

                    if (OnGroupsChanged != null)
                    {
                        RunOnUIContext(() => OnGroupsChanged(this, EventArgs.Empty));
                    }
                }
            }
        }

        public String FilePath
        {
            get
            {
                return _document.FilePath;
            }
        }

        public String DocumentName
        {
            get
            {
                return _document.Name;
            }
        }

        public bool IsConnected
        {
            get;
            private set;
        }

        public int ExecutedCount
        {
            get;
            private set;
        }

        public String ScopeVersion
        {
            get
            {
                return _scopeVersion;
            }
            private set
            {
                if (_scopeVersion != value)
                {
                    _scopeVersion = value;

                    RunOnUIContext(() => OnScopeVersionChanged(this, EventArgs.Empty));
                }
            }
        }

        public void ReloadDocument()
        {
            try
            {
                _document = DaxDocument.Load(_document.FilePath);
            }
            catch (Exception ex)
            {
                DoErrorEvent(ex);
            }
        }

        public void Reload(Dictionary<String, String> inputValues, Func<Block, bool> blockPredicate)
        {
            try
            {
                ReloadInternal(inputValues, blockPredicate);
            }
            catch (Exception ex)
            {
                DoErrorEvent(ex);
            }
        }

        public void ReloadInternal(Dictionary<String, String> inputValues, Func<Block, bool> blockPredicate)
        {
            ExecutedCount = 0;
            DoQueryReloadedEvent();

            IDbProvider dbProvider = GetQueryProvider();

            if (dbProvider != null)
            {
                String version = GetVersion(dbProvider);
                Scope scope = GetScope(version);
                ScopeVersion = scope.Version;
                Groups = scope.Groups.ToList();
                Dictionary<Block, IQueryBlock> acceptedBlocks = new Dictionary<Block, IQueryBlock>();

                foreach (Block block in scope.Blocks)
                {
                    if (block.CanExecute(inputValues) && blockPredicate(block))
                    {
                        String query = block.BuildQuery(inputValues);
                        acceptedBlocks.Add(block, dbProvider.CreateBlock(query));
                    }
                }

                List<Task> currentTasks = new List<Task>();
                bool cancelOperation = false;
                ExecutedCount = acceptedBlocks.Count;

                foreach (var item in acceptedBlocks)
                {
                    Block block = item.Key;
                    IQueryBlock queryBlock = item.Value;

                    var task = queryBlock.UpdateAsync();
                    currentTasks.Add(task);

                    task.GetAwaiter().OnCompleted(() =>
                        {
                            if (!cancelOperation && (!queryBlock.IsEmpty || block.ShowOnEmpty))
                            {
                                bool canceled = DoNewBlockAddedEvent(block, queryBlock);

                                if (canceled)
                                {
                                    cancelOperation = true;
                                }
                            }
                        });
                }

                while (!Task.WaitAll(currentTasks.ToArray(), 100))
                {
                    if (cancelOperation)
                    {
                        break;
                    }
                }
            }
        }

        public async Task ReloadAsync(Dictionary<String, String> inputValues, Func<Block, bool> blockPredicate)
        {
            Task task = Task.Factory.StartNew(() => Reload(inputValues, blockPredicate));

            await task;
        }

        public Task Reconnect()
        {
            return Task.Factory.StartNew(() =>
            {
                GetQueryProvider(true);
            });
        }

        private void DoQueryReloadedEvent()
        {
            RunOnUIContext(() => OnQueryReloaded(this, new QueryReloadedEventArgs()));
        }

        private IDbProvider GetQueryProvider(bool reset = false)
        {
            var ea = new QueryProviderEventArgs(this, reset);
            RunOnUIContext(() => OnQueryProvider(this, ea));

            if (ea.Provider != null)
            {
                IsConnected = true;
            }

            return ea.Provider;
        }

        private bool DoNewBlockAddedEvent(Block block, IQueryBlock queryBlock)
        {
            var ea = new NewBlockAddedEventArgs(block, queryBlock);
            RunOnUIContext(() => OnNewBlockAdded(this, ea));
            return ea.Canceled;
        }

        private void DoErrorEvent(Exception error)
        {
            if (error is AggregateException)
            {
                error = ((AggregateException)error).InnerExceptions[0];
            }

            QueryExecuteException queryException = error as QueryExecuteException;

            if (queryException != null)
            {
                RunOnUIContext(() => OnError(this, new ErrorEventArgs(queryException.InnerException.Message, queryException.Query)), true);
            }
            else
            {
                RunOnUIContext(() => OnError(this, new ErrorEventArgs(error.Message)), true);
            }
        }

        private Scope GetScope(String version)
        {
            if (String.IsNullOrEmpty(version))
            {
                Scope scope = _document.Scopes.FirstOrDefault(p => String.IsNullOrEmpty(p.Version));

                if (scope == null)
                {
                    throw new InvalidOperationException("Scope not found.");
                }

                return scope;
            }
            else
            {
                Scope scope = _document.Scopes.FirstOrDefault(p => p.Version == version);

                if (scope == null)
                {
                    throw new InvalidOperationException(String.Format("Scope for version '{0}' not found", version));
                }

                return scope;
            }
        }

        private String GetVersion(IDbProvider dbProvider)
        {
            Property versionQuery = GetVersionProperty();
            String version = versionQuery != null ? (String)dbProvider.ExecuteScalar(versionQuery.Value) : null;

            return version;
        }

        private Property GetVersionProperty()
        {
            var prop = _document.Properties.FirstOrDefault(p => p.Name == PROPERTY_QUERY_VERSION);

            if (prop == null || String.IsNullOrWhiteSpace(prop.Value))
            {
                return null;
            }

            return prop;
        }

        private Task RunOnUIContext(Action action, bool isAsync = false)
        {
            var token = Task.Factory.CancellationToken;
            var task = Task.Factory.StartNew(() =>
            {
                action();
            }, token, TaskCreationOptions.None, _uiContext);

            if (!isAsync)
            {
                task.Wait();
            }

            return task;
        }
    }
}
