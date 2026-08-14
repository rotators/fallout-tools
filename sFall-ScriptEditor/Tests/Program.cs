using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.TextEditor.Document;
using ScriptEditor;
using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUtilities;

namespace SfallScriptEditor.Tests
{
    internal static class Program
    {
        private static int passed;
        private static int failed;

        private static int Main()
        {
            Run("compiler diagnostic lookahead", CompilerDiagnosticLookahead);
            Run("document revision rejects stale parser result", DocumentRevisionRejectsStaleResult);
            Run("UTF-8 BOM encoding is preserved", Utf8BomEncodingIsPreserved);
            Run("UTF-8 without BOM is preserved", Utf8WithoutBomEncodingIsPreserved);
            Run("single-instance argument queue", CommandLineArgumentsAreQueuedAtomically);
            Run("stale single-instance arguments are ignored", StaleCommandLineArgumentsAreIgnored);
            Run("invalid parser line ranges are rejected", InvalidParserRangesAreRejected);
            Run("atomic document save replaces content", AtomicDocumentSaveReplacesContent);

            Console.WriteLine();
            Console.WriteLine("Tests: {0} passed, {1} failed", passed, failed);
            return failed == 0 ? 0 : 1;
        }

        private static void Run(string name, Action test)
        {
            try {
                test();
                passed++;
                Console.WriteLine("PASS  " + name);
            }
            catch (Exception ex) {
                failed++;
                Console.WriteLine("FAIL  " + name);
                Console.WriteLine("      " + ex.Message);
            }
        }

        private static void CompilerDiagnosticLookahead()
        {
            const string assignmentError = "Assignment operator expected.";
            Equal(8, ResolveDiagnostic(9, -1, assignmentError, "skill := has_skill_party(SKILL_OUTDOORSMAN);k"));
            Equal(8, ResolveDiagnostic(9, -1, assignmentError, "endjjj"));
            Equal(9, ResolveDiagnostic(9, -1, assignmentError, "skill := 100;"));
            Equal(9, ResolveDiagnostic(9, -1, "Unexpected token.", "endjjj"));

            string[] separatedByComments = {
                "register_hook_proc(HOOK_GAMEMODECHANGE, mark_locations);j",
                String.Empty,
                "// Removes the location names under green circles",
                "remove_wm_town_names(true);"
            };
            Equal(0, CompilerDiagnosticLineResolver.Resolve(3, -1, assignmentError, line => separatedByComments[line]));

            string[] malformedKeyword = {
                "if (game_loaded) then beginj",
                "set_global_script_type(0);"
            };
            Equal(0, CompilerDiagnosticLineResolver.Resolve(1, 23, assignmentError, line => malformedKeyword[line]));

            string[] sameLineError = {
                "procedure start begin",
                "foo bar;"
            };
            Equal(1, CompilerDiagnosticLineResolver.Resolve(1, 8, assignmentError, line => sameLineError[line]));
        }

        private static int ResolveDiagnostic(int reportedLine, int reportedColumn, string message, string previousLine)
        {
            return CompilerDiagnosticLineResolver.Resolve(reportedLine, reportedColumn, message,
                line => line == reportedLine - 1 ? previousLine : String.Empty);
        }
        private static void DocumentRevisionRejectsStaleResult()
        {
            var tab = new TabInfo();
            var args = new WorkerArgs("procedure start begin end", tab);
            True(args.IsCurrent, "A new parser request should match its document revision.");
            tab.MarkTextChanged();
            True(!args.IsCurrent, "An edit must invalidate an in-flight parser request.");
        }

        private static void Utf8BomEncodingIsPreserved()
        {
            WithTempDirectory(directory => {
                string path = Path.Combine(directory, "bom.ssl");
                File.WriteAllText(path, "Příliš žluťoučký", new UTF8Encoding(true));
                TextFileContents contents = TextFileEncoding.Read(path);
                Equal("Příliš žluťoučký", contents.Text);
                Equal(3, contents.Encoding.GetPreamble().Length);
            });
        }

        private static void Utf8WithoutBomEncodingIsPreserved()
        {
            WithTempDirectory(directory => {
                string path = Path.Combine(directory, "utf8.ssl");
                File.WriteAllText(path, "árvíztűrő tükörfúrógép", new UTF8Encoding(false));
                TextFileContents contents = TextFileEncoding.Read(path);
                Equal("árvíztűrő tükörfúrógép", contents.Text);
                Equal(0, contents.Encoding.GetPreamble().Length);
            });
        }

        private static void CommandLineArgumentsAreQueuedAtomically()
        {
            WithTempDirectory(directory => {
                var queue = new CommandLineQueue(Path.Combine(directory, "queue"), TimeSpan.FromMinutes(5));
                Parallel.Invoke(
                    () => queue.Enqueue(new[] { "one.ssl", "two.ssl" }),
                    () => queue.Enqueue(new[] { "three.ssl" }));
                string[] values = queue.DequeueAll();
                Equal(3, values.Length);
                True(values.Contains("one.ssl") && values.Contains("two.ssl") && values.Contains("three.ssl"),
                    "Every queued argument should be returned exactly once.");
                Equal(0, queue.DequeueAll().Length);
            });
        }

        private static void StaleCommandLineArgumentsAreIgnored()
        {
            WithTempDirectory(directory => {
                string queueDirectory = Path.Combine(directory, "queue");
                var queue = new CommandLineQueue(queueDirectory, TimeSpan.FromMinutes(5));
                queue.Enqueue(new[] { "stale.ssl" });
                string queuedFile = Directory.GetFiles(queueDirectory, "*.args").Single();
                File.SetLastWriteTimeUtc(queuedFile, DateTime.UtcNow.AddHours(-1));
                Equal(0, queue.DequeueAll().Length);
            });
        }

        private static void InvalidParserRangesAreRejected()
        {
            IDocument document = new DocumentFactory().CreateDocument();
            document.TextContent = "procedure start begin\r\nend\r\n";
            var procedure = new Procedure();
            procedure.d = new ProcedureData { start = 50, end = 60 };
            Equal(null, Utilities.GetProcedureCode(document, procedure));
            Equal(String.Empty, Utilities.GetRegionText(document, -1, 10));

            var info = new ProgramInfo(1, 0);
            procedure.name = "start";
            info.procs[0] = procedure;
            info.BuildDictionaries();
            True(Utilities.ReplaceProcedureCode(document, info, "start", "begin\r\nend"),
                "Replacement should fail safely when parser offsets are stale.");
        }

        private static void AtomicDocumentSaveReplacesContent()
        {
            WithTempDirectory(directory => {
                string path = Path.Combine(directory, "atomic.ssl");
                File.WriteAllText(path, "old", Encoding.ASCII);
                TabInfo.WriteAllTextAtomic(path, "new", Encoding.ASCII);
                Equal("new", File.ReadAllText(path, Encoding.ASCII));
                Equal(1, Directory.GetFiles(directory).Length);
            });
        }

        private static void WithTempDirectory(Action<string> action)
        {
            string directory = Path.Combine(Path.GetTempPath(), "SfallScriptEditorTests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try { action(directory); }
            finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        }

        private static void True(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException(String.Format("Expected <{0}> but received <{1}>.", expected, actual));
        }
    }
}