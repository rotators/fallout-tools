using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DATLib;

namespace DATExplorer
{
    struct sFile
    {
        public string path;    // имя и путь расположения файла в дереве в нижнем регистре
        public FileInfo file;  // дополнительная информация о файле

        public bool isVirtual { get { return file.info.PackedSize == -1;} } // файл добавляемый в DAT

        public sFile(string path, FileInfo file)
        {
            this.path = path;
            this.file = file;
        }

        public sFile(sFile file, string newPath)
        {
            this.file = file.file;
            this.file.pathTree = newPath;
            this.path = (this.file.pathTree + this.file.name).ToLowerInvariant();;
        }
    }

    internal class TreeFiles
    {
        private string folderPath; // путь к папки с сохранением регистра (нужно для отображения в дереве)
        private List<sFile> m_file;

        public TreeFiles(string folderPath)
        {
            this.folderPath = folderPath;
            m_file = new List<sFile>();
        }

        public string FolderPath { get { return folderPath; } }

        public string FolderName(int index)
        {
            return folderPath.Split('\\')[index];
        }

        public void AddFile(sFile file)
        {
            m_file.Add(file);
        }

        public void AddFile(KeyValuePair<string, FileInfo> file)
        {
            m_file.Add(new sFile(file.Key, file.Value));
        }

        public void RemoveFile(string filePath)
        {
            for (int i = 0; i < m_file.Count; i++)
            {
                if (m_file[i].path == filePath) {
                    m_file.RemoveAt(i);
                    return;
                }
            }
        }

        public sFile RenameFile(string filePath, string newName)
        {
            for (int i = 0; i < m_file.Count; i++)
            {
                if (m_file[i].path == filePath) {
                    sFile file = m_file[i];
                    int n = file.path.Length;
                    file.path = file.path.Remove(n - file.file.name.Length) + newName.ToLowerInvariant();
                    file.file.name = newName;
                    m_file[i] = file;
                    return file;
                }
            }
            return new sFile();
        }

        public void UpdateFileInfo(int index, Info info)
        {
            sFile nf = m_file[index];
            nf.file.info.PackedSize = info.PackedSize;
            nf.file.info.IsPacked = info.IsPacked;
            nf.file.info.Size = info.Size;

            m_file[index] = nf;
        }

        public bool FileExist(string fileName)
        {
            return m_file.Exists(x => x.file.name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        public List<sFile> GetFiles()
        {
            return m_file;
        }
    }
}
