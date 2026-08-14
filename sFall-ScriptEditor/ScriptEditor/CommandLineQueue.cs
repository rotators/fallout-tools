using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptEditor
{
    internal sealed class CommandLineQueue
    {
        private readonly string directory;
        private readonly TimeSpan maximumAge;

        internal CommandLineQueue(string directory, TimeSpan maximumAge)
        {
            this.directory = directory;
            this.maximumAge = maximumAge;
        }

        internal void Enqueue(IEnumerable<string> arguments)
        {
            Directory.CreateDirectory(directory);
            string id = DateTime.UtcNow.Ticks.ToString("D19") + "-" + Guid.NewGuid().ToString("N");
            string temporaryPath = Path.Combine(directory, id + ".tmp");
            string queuePath = Path.Combine(directory, id + ".args");
            try {
                File.WriteAllLines(temporaryPath, arguments);
                File.Move(temporaryPath, queuePath);
            }
            finally
            {
                TryDelete(temporaryPath);
            }
        }

        internal string[] DequeueAll()
        {
            if (!Directory.Exists(directory))
                return new string[0];

            var arguments = new List<string>();
            DateTime oldestAllowed = DateTime.UtcNow - maximumAge;
            foreach (string path in Directory.GetFiles(directory, "*.args").OrderBy(value => value, StringComparer.OrdinalIgnoreCase)) {
                try {
                    if (File.GetLastWriteTimeUtc(path) >= oldestAllowed)
                        arguments.AddRange(File.ReadAllLines(path));
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                finally
                {
                    TryDelete(path);
                }
            }
            return arguments.ToArray();
        }

        internal void Clear()
        {
            if (!Directory.Exists(directory))
                return;

            foreach (string path in Directory.GetFiles(directory))
                TryDelete(path);
            try { Directory.Delete(directory, false); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}