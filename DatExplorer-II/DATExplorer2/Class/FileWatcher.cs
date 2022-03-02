using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DATExplorer
{
    public class FileWatcher
    {
        private readonly string tmpAppFolder;

        // Filename to identify a drag from the application
        private string dragDropTmpFile;

        // The item which we drag is being stored here
        //internal static object objDragItem;

        // A FileSystemWatcher to monitor the System's Temp Directory for a drag
        private FileSystemWatcher directoryWatcher;

        // A List to keep multiple FileSystemWatchers
        private List<FileSystemWatcher> watchers = null;

        public bool IsRunning { get; private set; }

        public FileWatcher(string tempFolder)
        {
            tmpAppFolder = tempFolder;
            if (!Directory.Exists(tmpAppFolder)) Directory.CreateDirectory(tmpAppFolder);

            directoryWatcher = new FileSystemWatcher();
            directoryWatcher.Path = tmpAppFolder;
            directoryWatcher.NotifyFilter = NotifyFilters.FileName;
            directoryWatcher.IncludeSubdirectories = false;
            directoryWatcher.Created += new FileSystemEventHandler(TempDirectoryWatcherCreated);
        }

        public void StartWatcher(ref string dragDropFile)
        {
            this.dragDropTmpFile = Path.GetRandomFileName() + ".tmp";
            dragDropFile = directoryWatcher.Path + this.dragDropTmpFile;
            directoryWatcher.Filter = this.dragDropTmpFile;
            directoryWatcher.EnableRaisingEvents = true;
            var file = File.Create(dragDropFile);
            file.Close();
            IsRunning = true;
        }

        public void StopWatcher()
        {
            IsRunning = false;
            directoryWatcher.EnableRaisingEvents = false;
            ClearFileWatchers();
            //File.Delete(directoryWatcher.Path + dragDropTmpFile); // тут иногда возникает исключение о том что файл занят другим приложением
        }

        private void TempDirectoryWatcherCreated(object sender, FileSystemEventArgs e)
        {
            if (watchers == null) {
                List<FileSystemWatcher> tempWatchers = new List<FileSystemWatcher>();
                FileSystemWatcher watcher;

                // Adding FileSystemWatchers and adding Created event to it
                foreach (string driveName in Directory.GetLogicalDrives())
                {
                    if (Directory.Exists(driveName)) {
                        watcher = new FileSystemWatcher();
                        watcher.Filter = dragDropTmpFile;
                        watcher.NotifyFilter = NotifyFilters.FileName;
                        watcher.Created += new FileSystemEventHandler(FileWatcherCreated);
                        watcher.IncludeSubdirectories = true;
                        watcher.Path = driveName;
                        watcher.EnableRaisingEvents = true;
                        tempWatchers.Add(watcher);
                    }
                }
                watchers = tempWatchers;
            }
        }

        private void FileWatcherCreated(object sender, FileSystemEventArgs e)
        {
            //if (objDragItem == null) return;
            OnDropped(e.FullPath);

        reTry:
            try {
                File.Delete(e.FullPath);
            }
            catch (System.Exception) {
                goto reTry;
            }
            //objDragItem = null;
        }

        private void ClearFileWatchers()
        {
            if (watchers != null) {
                foreach (FileSystemWatcher watch in watchers)
                {
                    watch.Dispose();
                }
                watchers.Clear();
                watchers = null;
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(tmpAppFolder)) Directory.Delete(tmpAppFolder, true);
        }

        private void OnDropped(string path)
        {
            if (DragDrop != null) {
                DragDrop(new DropEventArgs(path));
            }
        }

        #region Drop Event
        public delegate void DropExplorerEvent(DropEventArgs e);
        public event DropExplorerEvent DragDrop;

        public class DropEventArgs
        {
            protected string path;

            public string PathDrop { get { return path; } }

            public DropEventArgs(string path)
            {
                this.path = Path.GetDirectoryName(path);
            }
        }
        #endregion
    }
}
