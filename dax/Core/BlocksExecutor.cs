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

using dax.Db;
using dax.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace dax.Core
{
    public class BlocksExecutor
    {
        private readonly Dictionary<Block, IQueryBlock> _acceptedBlocks;
        private bool _canceled;
        private bool _finished;
        
        private BlocksExecutor(Dictionary<Block, IQueryBlock> acceptedBlocks)
        {
            this._acceptedBlocks = acceptedBlocks;
        }

        public bool Canceled
        {
            get { return _canceled; }
        }

        public bool Finished
        {
            get { return _finished; }
        }

        public int ExecutedQueriesCount
        {
            get;
            private set;
        }

        public long ElapsedTime
        {
            get;
            private set;
        }

        public void Cancel()
        {
            _canceled = true;
        }

        public void Execute(Action<Block, IQueryBlock> handler)
        {
            List<Task> currentTasks = new List<Task>();
            int queryCounter = 0;
            var watcher = Stopwatch.StartNew();

            try
            {
                foreach (var item in _acceptedBlocks)
                {
                    Block block = item.Key;
                    IQueryBlock queryBlock = item.Value;

                    var task = queryBlock.UpdateAsync();
                    currentTasks.Add(task);

                    task.GetAwaiter().OnCompleted(() =>
                    {
                        if (!_canceled && (!queryBlock.IsEmpty || block.ShowOnEmpty))
                        {
                            Interlocked.Increment(ref queryCounter);
                            handler(block, queryBlock);
                        }
                    });
                }

                while (!Task.WaitAll(currentTasks.ToArray(), 100))
                {
                    if (_canceled)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // we not show error if user canceled operation
                if (!_canceled)
                {
                    throw;
                }
            }
            finally
            {
                ExecutedQueriesCount = queryCounter;
                watcher.Stop();
                ElapsedTime = watcher.ElapsedMilliseconds;
                _finished = true;
            }
        }

        public static BlocksExecutor Create(Dictionary<Block, IQueryBlock> acceptedBlocks)
        {
            return new BlocksExecutor(acceptedBlocks);
        }
    }
}
