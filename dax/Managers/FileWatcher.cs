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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace dax.Managers
{
    public class FileWatcher
    {
        private readonly List<String> _pathes = new List<String>();
        private readonly Dictionary<String, long> _fileNames = new Dictionary<String, long>();
        private readonly TaskScheduler _uiContext;

        public FileWatcher(TaskScheduler uiContext)
        {
            _uiContext = uiContext;
        }

        public void WatchFile(String filePath)
        {
            var info = new FileInfo(filePath);

            if (info.Exists)
            {
                String path = Path.GetDirectoryName(info.FullName);

                lock (_fileNames)
                {
                    _fileNames[info.FullName] = 0;
                }

                if (!_pathes.Contains(path))
                {
                    _pathes.Add(path);
                    StartWatchingPath(path);
                }
            }
        }

        public void UnwatchFile(String filePath)
        {
            lock(_fileNames)
            {
                _fileNames.Remove(filePath);
            }
        }

        private void StartWatchingPath(String path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Changed;
            watcher.Renamed += Watcher_Changed;

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (OnFileChanged != null)
            {
                bool fileChanged = false;

                lock (_fileNames)
                {
                    if (_fileNames.ContainsKey(e.FullPath))
                    {
                        long lastFileTime = GetFileLastTime(e.FullPath);
                        fileChanged = _fileNames[e.FullPath] != lastFileTime;
                    }
                }

                if (fileChanged)
                {
                    RunOnUIContext(() => OnFileChanged(this, new FileChangedEventArgs(e.FullPath)));

                    lock (_fileNames)
                    {
                        long lastFileTime = GetFileLastTime(e.FullPath);
                        _fileNames[e.FullPath] = lastFileTime;
                    }
                }                
            }
        }

        public long GetFileLastTime(String filePath)
        {
            var info = new FileInfo(filePath);

            return info.Exists ? info.LastWriteTime.Ticks : 0;
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

        public event EventHandler<FileChangedEventArgs> OnFileChanged;
    }
}
