using System;

namespace dax.Managers
{
    public class FileChangedEventArgs
    {
        public FileChangedEventArgs(String filePath)
        {
            FilePath = filePath;
        }
        
        public String FilePath
        {
            get;
            private set;
        }
    }
}
