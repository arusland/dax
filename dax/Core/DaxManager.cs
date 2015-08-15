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
using System.Threading;
using System.Threading.Tasks;

namespace dax.Core
{
    public class DaxManager
    {
        private const String PROPERTY_QUERY_VERSION = "version.query";
        private DaxDocument _document;
        private String _scopeVersion;
        //private readonly TaskScheduler _uiContext;
        private List<Group> _groups;
        private OperationState _currentState;
        private readonly SynchronizationContext syncContext;
        private readonly List<BlocksExecutor> _executingBlocks = new List<BlocksExecutor>();
        public event EventHandler<QueryReloadedEventArgs> OnQueryReloaded;
        public event EventHandler<NewBlockAddedEventArgs> OnNewBlockAdded;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<QueryProviderEventArgs> OnQueryProvider;
        public event EventHandler<EventArgs> OnScopeVersionChanged;
        public event EventHandler<EventArgs> OnGroupsChanged;
        public event EventHandler<EventArgs> OnStateChanged;
        public event EventHandler<QueryFinishedEventArgs> OnQueryFinished;

        public DaxManager(String filePath, SynchronizationContext syncContext)
        {
            //_uiContext = uiContext;
            _document = DaxDocument.Load(filePath);
            this.syncContext = syncContext;
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

        public OperationState CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;

                    RunOnUIContext(() => OnStateChanged(this, EventArgs.Empty));
                }
            }
        }

        public bool CanCancel
        {
            get
            {
                return CurrentState == OperationState.Searching;
            }
        }

        public bool CanSearch
        {
            get
            {
                return CurrentState == OperationState.Ready;
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

        public void Search(Dictionary<String, String> inputValues, Func<Block, bool> blockPredicate)
        {
            try
            {
                SearchInternal(inputValues, blockPredicate);
            }
            catch (Exception ex)
            {
                DoErrorEvent(ex);
            }
        }

        public async Task SearchAsync(Dictionary<String, String> inputValues, Func<Block, bool> blockPredicate)
        {
            Task task = Task.Factory.StartNew(() => Search(inputValues, blockPredicate));

            await task;
        }

        public Task Reconnect()
        {
            return Task.Factory.StartNew(() =>
            {
                GetQueryProvider(true);
            });
        }

        public void Cancel()
        {
            if (CanCancel)
            {
                lock (_executingBlocks)
                {
                    _executingBlocks.ForEach(p => p.Cancel());

                    // remove canceled executors
                    _executingBlocks.Where(p => p.Finished)
                        .ToList()
                        .ForEach(p => _executingBlocks.Remove(p));
                }

                DoQueryFinishedCanceledEvent();
                CurrentState = OperationState.Ready;
            }
        }

        private void SearchInternal(Dictionary<String, String> inputValues, Func<Block, bool> blockPredicate)
        {
            if (!CanSearch)
            {
                return;
            }

            ExecutedCount = 0;
            CurrentState = OperationState.Searching;
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

                ExecutedCount = acceptedBlocks.Count;

                var executor = BlocksExecutor.Create(acceptedBlocks);
                AddExecutor(executor);
                executor.Execute(DoNewBlockAddedEvent);

                if (!executor.Canceled)
                {
                    DoQueryFinishedEvent(executor);
                    CurrentState = OperationState.Ready;
                }
            }
            else
            {
                CurrentState = OperationState.Ready;
            }
        }

        private void AddExecutor(BlocksExecutor executor)
        {
            lock (_executingBlocks)
            {
                _executingBlocks.Add(executor);
            }
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

        private void DoNewBlockAddedEvent(Block block, IQueryBlock queryBlock)
        {
            RunOnUIContext(() => OnNewBlockAdded(this, new NewBlockAddedEventArgs(block, queryBlock)));
        }

        private void DoQueryFinishedEvent(BlocksExecutor executor)
        {
            RunOnUIContext(() => OnQueryFinished(this, new QueryFinishedEventArgs(executor.ExecutedQueriesCount, executor.ElapsedTime)));
        }

        private void DoQueryFinishedCanceledEvent()
        {
            RunOnUIContext(() => OnQueryFinished(this, QueryFinishedEventArgs.CanceledEventArgs));
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
                RunOnUIContext(() => OnError(this, new ErrorEventArgs(queryException.InnerException.Message, queryException.Query)));
            }
            else
            {
                RunOnUIContext(() => OnError(this, new ErrorEventArgs(error.Message)));
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

        private void RunOnUIContext(Action action)
        {
            SynchronizeCall(syncContext, action);
        }

        /// <summary>
        /// Allows performing the given func synchronously on the thread the dispatcher is associated with.
        /// </summary>
        /// <param name="syncContext">The synchronization object.</param>
        /// <param name="action">The func (function) to call.</param>
        public static void SynchronizeCall(SynchronizationContext syncContext, Action action)
        {
            syncContext.Send((o) => action(), null);
        }
    }
}
