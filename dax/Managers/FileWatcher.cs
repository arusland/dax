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
