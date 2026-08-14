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
            Run("diagnostic suppressions are configurable", DiagnosticSuppressionsAreConfigurable);
            Run("document revision rejects stale parser result", DocumentRevisionRejectsStaleResult);
            Run("UTF-8 BOM encoding is preserved", Utf8BomEncodingIsPreserved);
            Run("UTF-8 without BOM is preserved", Utf8WithoutBomEncodingIsPreserved);
            Run("single-instance argument queue", CommandLineArgumentsAreQueuedAtomically);
            Run("stale single-instance arguments are ignored", StaleCommandLineArgumentsAreIgnored);
            Run("invalid parser line ranges are rejected", InvalidParserRangesAreRejected);
            Run("atomic document save replaces content", AtomicDocumentSaveReplacesContent);
            Run("procedure folding commands affect member bodies", ProcedureFoldingCommandsAffectMemberBodies);
            Run("multiline object macros retain their identifier", MultilineObjectMacrosRetainTheirIdentifier);

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

        private static void DiagnosticSuppressionsAreConfigurable()
        {
            WithTempDirectory(directory => {
                string config = Path.Combine(directory, DiagnosticSuppressionRules.ConfigFileName);
                File.WriteAllText(config,
                    "# Example suppression\r\n"
                    + "malformed rule\r\n"
                    + "Warning|debug.h|16|Illegal multi-byte character sequence\r\n");

                const string output = "[Warning] <debug.h>:16: Illegal multi-byte character sequence in quotation:\r\n"
                    + "#define STYLE_debug(text) ANSI_SGR('1;38;2;60;248;0') + text\r\n"
                    + "[Warning] <debug.h>:17: A useful warning.\r\n"
                    + "Additional warning context.\r\n";
                var errors = new List<Error>();
                Error.BuildLog(errors, output, @"C:\scripts\test.ssl", config);
                Equal(1, errors.Count);
                Equal(17, errors[0].line);

                File.WriteAllText(config, "Warning|*|*|A useful warning\r\n");
                Error.BuildLog(errors, output, @"C:\scripts\test.ssl", config);
                Equal(1, errors.Count);
                Equal(16, errors[0].line);

                File.WriteAllText(config, "Warning|debug.h|not-a-line|A useful warning\r\n");
                Error.BuildLog(errors, output, @"C:\scripts\test.ssl", config);
                Equal(2, errors.Count);
            });
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
            Equal("Unexpected identifier 'foo'; assignment operator expected.",
                CompilerDiagnosticLineResolver.Clarify(assignmentError, 1, 1, 8, line => sameLineError[line]));

            string[] trailingIdentifier = {
                "remove_wm_town_names(true);q",
                "town_list := [ AREA_VAULT_13 ];"
            };
            Equal("Unexpected identifier 'q'; assignment operator expected.",
                CompilerDiagnosticLineResolver.Clarify(assignmentError, 1, 0, 10, line => trailingIdentifier[line]));
            Equal("Unexpected token.",
                CompilerDiagnosticLineResolver.Clarify("Unexpected token.", 1, 0, 10, line => trailingIdentifier[line]));
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

        private static void ProcedureFoldingCommandsAffectMemberBodies()
        {
            IDocument document = new DocumentFactory().CreateDocument();
            document.TextContent = "0\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n";
            var first = new FoldMarker(document, 0, 0, 2, 1, FoldType.MemberBody, " FIRST ");
            var second = new FoldMarker(document, 3, 0, 5, 1, FoldType.MemberBody, " SECOND ");
            var variables = new FoldMarker(document, 6, 0, 8, 1, FoldType.TypeBody, " VARIABLES ");
            document.FoldingManager.UpdateFoldings(new List<FoldMarker> { first, second, variables });

            CodeFolder.SetAllProceduresFolded(document, true);
            True(first.IsFolded && second.IsFolded, "Collapse all should fold every procedure.");
            True(!variables.IsFolded, "Procedure folding must not affect non-procedure regions.");
            True(CodeFolder.HasProcedure(document, true), "Folded procedures should be detected.");

            CodeFolder.SetAllProceduresFolded(document, false);
            True(!first.IsFolded && !second.IsFolded, "Expand all should unfold every procedure.");

            True(CodeFolder.CollapseAllExceptProcedure(document, 4),
                "A line inside a procedure should identify that procedure.");
            True(first.IsFolded && !second.IsFolded,
                "Only the procedure at the target line should remain expanded.");
            True(!variables.IsFolded, "Other folding regions must remain unchanged.");

            True(!CodeFolder.CollapseAllExceptProcedure(document, 7),
                "A line outside a procedure should not trigger the command.");
        }

        private static void MultilineObjectMacrosRetainTheirIdentifier()
        {
            const string macroName = "VOODOO_disable_YouEncounter_message";
            string[] lines = {
                "#define " + macroName + " \\",
                "              begin                               \\",
                "               write_int(0x4C100C, 0x909039EB);   \\",
                "              end                                 \\",
                "              noop"
            };
            var macros = new SortedDictionary<string, Macro>();

            new GetMacros(lines, "voodoo.h", String.Empty, macros, false);

            True(macros.ContainsKey(macroName),
                "A multiline object-like macro must be stored under its declared identifier.");
            Equal(macroName, macros[macroName].token);
            Equal(1, macros[macroName].declared);
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
