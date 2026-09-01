using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ScriptEditor;
using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUI.ToolTips;
using ScriptEditor.TextEditorUtilities;

namespace SfallScriptEditor.Tests
{
    internal static class Program
    {
        private static int passed;
        private static int failed;

        [STAThread]
        private static int Main()
        {
            Run("compiler diagnostic lookahead", CompilerDiagnosticLookahead);
            Run("colon-form compiler errors are retained", ColonFormCompilerErrorsAreRetained);
            Run("diagnostic suppressions are configurable", DiagnosticSuppressionsAreConfigurable);
            Run("document revision rejects stale parser result", DocumentRevisionRejectsStaleResult);
            Run("UTF-8 BOM encoding is preserved", Utf8BomEncodingIsPreserved);
            Run("UTF-8 without BOM is preserved", Utf8WithoutBomEncodingIsPreserved);
            Run("single-instance argument queue", CommandLineArgumentsAreQueuedAtomically);
            Run("stale single-instance arguments are ignored", StaleCommandLineArgumentsAreIgnored);
            Run("invalid parser line ranges are rejected", InvalidParserRangesAreRejected);
            Run("atomic document save replaces content", AtomicDocumentSaveReplacesContent);
            Run("metadata-only file changes are ignored", MetadataOnlyFileChangesAreIgnored);
            Run("content file changes are detected", ContentFileChangesAreDetected);
            Run("procedure folding commands affect member bodies", ProcedureFoldingCommandsAffectMemberBodies);
            Run("dialog procedures are discovered from their content", DialogProceduresAreDiscoveredFromContent);
            Run("multiline object macros retain their identifier", MultilineObjectMacrosRetainTheirIdentifier);
            Run("DPI metrics use 96-DPI logical units", DpiMetricsUseLogicalUnits);
            Run("dark editor host surfaces do not expose light backgrounds", DarkEditorHostSurfacesDoNotExposeLightBackgrounds);
            Run("dark combo boxes keep stable native chrome", DarkComboBoxesKeepStableNativeChrome);
            Run("previous tab session preserves order and selection", PreviousTabSessionPreservesOrderAndSelection);
            Run("tab close retains pressed page identity", TabCloseRetainsPressedPageIdentity);
            Run("managed tab arrows preserve selection and order", ManagedTabArrowsPreserveSelectionAndOrder);
            Run("overflow close targets visible page", OverflowCloseTargetsVisiblePage);
            Run("managed reorder and removal preserve page identity", ManagedReorderAndRemovalPreservePageIdentity);
            Run("managed reorder keeps document view attached", ManagedReorderKeepsDocumentViewAttached);
            Run("document host avoids intermediate layouts", DocumentHostAvoidsIntermediateLayouts);
            Run("document page text can be assigned after insertion", DocumentPageTextCanBeAssignedAfterInsertion);
            Run("designer control collection routes tab pages", DesignerControlCollectionRoutesTabPages);
            Run("tab navigation arrows are not empty tab strip", TabNavigationArrowsAreNotEmptyTabStrip);
            Run("notification severity is conveyed in text", NotificationSeverityIsConveyedInText);
            Run("LF message files populate hover text", LfMessageFilesPopulateHoverText);
            Run("message cache reloads changed files", MessageCacheReloadsChangedFiles);
            Run("script NAME resolves shared message file", ScriptNameResolvesSharedMessageFile);
            Run("message_str resolves explicit message file", MessageStrResolvesExplicitMessageFile);
            Run("message wrapper macro resolves explicit message file", MessageWrapperMacroResolvesExplicitMessageFile);
            Run("random message range populates tooltip text", RandomMessageRangePopulatesTooltipText);
            Run("random message range resolves explicit message file", RandomMessageRangeResolvesExplicitMessageFile);
            Run("message navigation resolves file and physical line", MessageNavigationResolvesFileAndPhysicalLine);

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

                File.WriteAllText(config, "Parser|lib.arrays.h|268|Expected symbol\r\n");
                DiagnosticSuppressionRules rules = DiagnosticSuppressionRules.Load(config);
                True(rules.IsIgnored(new Error(ErrorType.Parser, "Expected symbol", "lib.arrays.h", 268)),
                    "A parser-only rule should suppress the matching live-parser diagnostic.");
                True(!rules.IsIgnored(new Error(ErrorType.Error, "Expected symbol", "lib.arrays.h", 268)),
                    "A parser-only rule must not suppress a genuine compiler error.");
            });
        }

        private static void ColonFormCompilerErrorsAreRetained()
        {
            const string output = "TREAD.ssl:74: error: Procedure 'combat_p_procd' was not declared.\r\n";
            var errors = new List<Error>();

            Error.BuildLog(errors, output, @"C:\scripts\TREAD.ssl", Path.Combine(Path.GetTempPath(), "missing-diagnostic-suppressions.ini"));

            Equal(1, errors.Count);
            Equal(ErrorType.Error, errors[0].type);
            Equal(74, errors[0].line);
            Equal("TREAD.ssl", Path.GetFileName(errors[0].fileName));
            True(errors[0].message.IndexOf("combat_p_procd", StringComparison.OrdinalIgnoreCase) >= 0,
                "The compiler's colon-form error message should remain visible.");
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

        private static void DarkEditorHostSurfacesDoNotExposeLightBackgrounds()
        {
            InterfaceThemeMode originalTheme = Settings.interfaceTheme;
            try {
                Settings.interfaceTheme = InterfaceThemeMode.Dark;
                using (var editor = new ICSharpCode.TextEditor.TextEditorControl()) {
                    InterfaceTheme.Apply(editor);
                    Color expected = Color.FromArgb(40, 40, 42);
                    Equal(expected, editor.BackColor);

                    var pending = new Stack<Control>();
                    foreach (Control child in editor.Controls)
                        pending.Push(child);
                    while (pending.Count > 0) {
                        Control control = pending.Pop();
                        if (control is Panel)
                            Equal(expected, control.BackColor);
                        foreach (Control child in control.Controls)
                            pending.Push(child);
                    }
                }
            } finally {
                Settings.interfaceTheme = originalTheme;
            }
        }

        private static void DarkComboBoxesKeepStableNativeChrome()
        {
            InterfaceThemeMode originalTheme = Settings.interfaceTheme;
            try {
                Settings.interfaceTheme = InterfaceThemeMode.Dark;
                using (var comboBox = new ComboBox {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Bounds = new Rectangle(12, 8, 140, 23)
                }) {
                    comboBox.Items.AddRange(new object[] { "First", "Second" });
                    comboBox.SelectedIndex = 0;
                    IntPtr handle = comboBox.Handle;

                    InterfaceTheme.Apply(comboBox);
                    Rectangle themedBounds = comboBox.Bounds;
                    InterfaceTheme.Apply(comboBox);

                    Equal(FlatStyle.Standard, comboBox.FlatStyle);
                    Equal(DrawMode.Normal, comboBox.DrawMode);
                    Equal(themedBounds, comboBox.Bounds);
                }
            } finally {
                Settings.interfaceTheme = originalTheme;
            }
        }

        private static void MetadataOnlyFileChangesAreIgnored()
        {
            WithTempDirectory(directory => {
                string path = Path.Combine(directory, "unchanged.h");
                File.WriteAllText(path, "#define VALUE 1\r\n", Encoding.ASCII);
                var tab = new TabInfo { filepath = path };
                tab.CaptureFileState();

                File.SetLastWriteTimeUtc(path, File.GetLastWriteTimeUtc(path).AddMinutes(1));

                True(tab.CheckFileTime(),
                    "Changing only the timestamp must not be reported as an external content edit.");
                True(tab.CheckFileTime(), "Accepted metadata should become the new baseline.");
            });
        }

        private static void ContentFileChangesAreDetected()
        {
            WithTempDirectory(directory => {
                string path = Path.Combine(directory, "changed.h");
                File.WriteAllText(path, "#define VALUE 1\r\n", Encoding.ASCII);
                var tab = new TabInfo { filepath = path };
                tab.CaptureFileState();

                File.WriteAllText(path, "#define VALUE 200\r\n", Encoding.ASCII);
                File.SetLastWriteTimeUtc(path, File.GetLastWriteTimeUtc(path).AddMinutes(1));

                True(!tab.CheckFileTime(), "Changed file bytes must still be reported.");
                tab.CaptureFileState();
                True(tab.CheckFileTime(), "An acknowledged content change should become the new baseline.");

                File.Delete(path);
                True(!tab.CheckFileTime(), "Deleting an open file must still be reported.");
                tab.CaptureFileState();
                True(tab.CheckFileTime(), "An acknowledged deletion should not prompt repeatedly.");
            });
        }

        private static void TabCloseRetainsPressedPageIdentity()
        {
            using (var control = new TestDraggableTabControl()) {
                control.Size = new Size(480, 100);
                control.ShowCloseButtons = true;
                var first = new TabPage("First.ssl");
                var pressed = new TabPage("Pressed.ssl");
                var replacement = new TabPage("Replacement.ssl");
                control.TabPages.AddRange(new[] { first, pressed, replacement });
                control.SelectedTab = pressed;
                IntPtr handle = control.Handle;

                Rectangle pressedBounds = control.GetTabRect(control.TabPages.IndexOf(pressed));
                Point closePoint = new Point(pressedBounds.Right - DpiHelper.Scale(control, 9),
                    pressedBounds.Top + pressedBounds.Height / 2);
                TabPage requested = null;
                control.TabCloseRequested += delegate(object sender, TabCloseRequestedEventArgs e) {
                    requested = e.TabPage;
                };

                control.RaiseMouseDown(closePoint);
                control.TabPages.Remove(pressed);
                control.TabPages.Add(pressed);
                control.RaiseMouseUp(closePoint);

                True(requested == null,
                    "Removing the pressed page between mouse-down and mouse-up must not close its replacement.");
            }
        }

        private static void ManagedTabArrowsPreserveSelectionAndOrder()
        {
            using (var control = new TestDraggableTabControl()) {
                control.Size = new Size(360, 100);
                control.ShowCloseButtons = true;
                var pages = new List<TabPage>();
                for (int i = 0; i < 12; i++) {
                    var page = new TabPage("Document" + i + ".ssl");
                    pages.Add(page);
                    control.TabPages.Add(page);
                }
                control.SelectedTab = pages[0];
                IntPtr handle = control.Handle;
                control.PerformLayout();

                Point rightArrow = new Point(control.ClientSize.Width - DpiHelper.Scale(control, 10),
                    Math.Max(1, control.GetTabRect(0).Height / 2));
                control.RaiseMouseDown(rightArrow);
                control.RaiseMouseUp(rightArrow);

                Equal(pages[0], control.SelectedTab);
                True(control.GetTabRect(0).IsEmpty,
                    "Arrow navigation should scroll the managed viewport without changing selection.");
                for (int i = 0; i < pages.Count; i++)
                    Equal(pages[i], control.TabPages[i]);
            }
        }

        private static void OverflowCloseTargetsVisiblePage()
        {
            using (var control = new TestDraggableTabControl()) {
                control.Size = new Size(360, 100);
                control.ShowCloseButtons = true;
                var pages = new List<TabPage>();
                for (int i = 0; i < 12; i++) {
                    var page = new TabPage("Document" + i + ".ssl");
                    pages.Add(page);
                    control.TabPages.Add(page);
                }
                control.SelectedTab = pages[0];
                IntPtr handle = control.Handle;
                control.PerformLayout();

                Point rightArrow = new Point(control.ClientSize.Width - DpiHelper.Scale(control, 10),
                    Math.Max(1, control.GetTabRect(0).Height / 2));
                for (int i = 0; i < 5; i++) {
                    control.RaiseMouseDown(rightArrow);
                    control.RaiseMouseUp(rightArrow);
                }

                TabPage target = pages[6];
                Rectangle targetBounds = control.GetTabRect(control.TabPages.IndexOf(target));
                True(!targetBounds.IsEmpty, "The close target should be visible after managed scrolling.");
                Point closePoint = new Point(targetBounds.Right - DpiHelper.Scale(control, 9),
                    targetBounds.Top + targetBounds.Height / 2);
                TabPage requested = null;
                control.TabCloseRequested += delegate(object sender, TabCloseRequestedEventArgs e) {
                    requested = e.TabPage;
                };

                control.RaiseMouseMove(closePoint);
                control.RaiseMouseDown(closePoint);
                control.RaiseMouseUp(closePoint);

                Equal(target, requested);
                Equal(pages[0], control.SelectedTab);
            }
        }

        private static void ManagedReorderAndRemovalPreservePageIdentity()
        {
            using (var control = new TestDraggableTabControl()) {
                var first = new TabPage("First.ssl");
                var moved = new TabPage("Moved.ssl");
                var selected = new TabPage("Selected.ssl");
                var last = new TabPage("Last.ssl");
                control.TabPages.AddRange(new[] { first, moved, selected, last });
                control.SelectedTab = selected;

                TabsSwappedEventArgs swap = null;
                control.tabsSwapped += delegate(object sender, TabsSwappedEventArgs e) { swap = e; };
                control.MoveTab(moved, 3);

                Equal(first, control.TabPages[0]);
                Equal(selected, control.TabPages[1]);
                Equal(last, control.TabPages[2]);
                Equal(moved, control.TabPages[3]);
                Equal(selected, control.SelectedTab);
                Equal(1, swap.aIndex);
                Equal(3, swap.bIndex);

                control.TabPages.Remove(first);
                Equal(selected, control.SelectedTab);
                control.TabPages.Remove(selected);
                Equal(last, control.SelectedTab);
            }
        }

        private static void DesignerControlCollectionRoutesTabPages()
        {
            using (var control = new TestDraggableTabControl()) {
                var page = new TabPage("Designer.ssl");
                control.Controls.Add(page);

                Equal(1, control.TabCount);
                Equal(page, control.TabPages[0]);
                Equal(page, control.SelectedTab);

                control.Controls.Remove(page);
                Equal(0, control.TabCount);
            }
        }

        private static void DocumentHostAvoidsIntermediateLayouts()
        {
            using (var control = new TestDraggableTabControl()) {
                control.Size = new Size(640, 420);
                var current = new TabPage("Current.ssl");
                control.TabPages.Add(current);
                IntPtr handle = control.Handle;
                control.PerformLayout();

                int sizeChanges = 0;
                current.SizeChanged += delegate { sizeChanges++; };
                control.TabPages.Add(new TabPage("New.ssl"));
                Equal(0, sizeChanges);

                sizeChanges = 0;
                control.Size = new Size(700, 460);
                control.PerformLayout();
                True(sizeChanges <= 1,
                    "One outer resize must not pass the document through intermediate page-host bounds.");
            }
        }

        private static void DocumentPageTextCanBeAssignedAfterInsertion()
        {
            using (var control = new TestDraggableTabControl()) {
                control.Size = new Size(640, 420);
                IntPtr handle = control.Handle;
                var page = new TabPage();
                control.Controls.Add(page);

                page.Text = "Initialized.ssl";

                Equal("Initialized.ssl", page.Text);
                Equal(page, control.SelectedTab);
            }
        }

        private static void ManagedReorderKeepsDocumentViewAttached()
        {
            using (var control = new TestDraggableTabControl()) {
                var first = new TabPage("First.ssl");
                var moved = new TabPage("Moved.ssl");
                var last = new TabPage("Last.ssl");
                control.TabPages.AddRange(new[] { first, moved, last });
                control.SelectedTab = moved;
                IntPtr controlHandle = control.Handle;
                IntPtr pageHandle = moved.Handle;
                Control parent = moved.Parent;
                int parentChanges = 0;
                moved.ParentChanged += delegate { parentChanges++; };

                control.MoveTab(moved, 2);

                True(controlHandle != IntPtr.Zero, "The tab control should have a live window handle.");
                Equal(parent, moved.Parent);
                Equal(pageHandle, moved.Handle);
                Equal(0, parentChanges);
                Equal(moved, control.SelectedTab);
            }
        }

        private static void TabNavigationArrowsAreNotEmptyTabStrip()
        {
            using (var control = new TestDraggableTabControl()) {
                control.Size = new Size(360, 100);
                for (int i = 0; i < 12; i++)
                    control.TabPages.Add(new TabPage("Document" + i + ".ssl"));
                IntPtr handle = control.Handle;
                control.PerformLayout();

                Point leftArrow = new Point(control.ClientSize.Width - DpiHelper.Scale(control, 34),
                    Math.Max(1, control.GetTabRect(0).Height / 2));
                Point rightArrow = new Point(control.ClientSize.Width - DpiHelper.Scale(control, 10),
                    leftArrow.Y);
                True(control.IsTabNavigationArea(leftArrow),
                    "The managed left tab arrow should be recognized as navigation chrome.");
                True(control.IsTabNavigationArea(rightArrow),
                    "The managed right tab arrow should be recognized as navigation chrome.");
                True(!TextEditor.IsEmptyDocumentTabStripLocation(control, leftArrow),
                    "Double-clicking the left tab arrow must not create an empty document.");
                True(!TextEditor.IsEmptyDocumentTabStripLocation(control, rightArrow),
                    "Double-clicking the right tab arrow must not create an empty document.");
            }
        }

        private sealed class TestDraggableTabControl : DraggableTabControl
        {
            internal void RaiseMouseMove(Point location)
            {
                base.OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, location.X, location.Y, 0));
            }

            internal void RaiseMouseDown(Point location)
            {
                base.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
            }

            internal void RaiseMouseUp(Point location)
            {
                base.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
            }
        }

        private static void LfMessageFilesPopulateHoverText()
        {
            var scriptTab = new TabInfo();
            var messageTab = new TabInfo { textEditor = new ICSharpCode.TextEditor.TextEditorControl() };
            messageTab.textEditor.Document.TextContent =
                "{100}{}{First line.}\n{117}{}{I wouldn't work for you if you offered me all the money in this crummy place.}\n";
            scriptTab.msgFileTab = messageTab;

            MessageFile.ParseMessages(scriptTab);

            Equal(2, scriptTab.messages.Count);
            Equal("I wouldn't work for you if you offered me all the money in this crummy place.", scriptTab.messages[117]);
        }

        private static void MessageCacheReloadsChangedFiles()
        {
            WithTempDirectory(directory => {
                string messagePath = Path.Combine(directory, "cached.msg");
                File.WriteAllText(messagePath, "{100}{}{First cached value.}\n");
                var tab = new TabInfo {
                    filepath = Path.Combine(directory, "cached.ssl"),
                    filename = "cached.ssl",
                    msgFilePath = messagePath
                };

                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.EncCodePage = Encoding.UTF8;
                    string message;
                    True(MessageFile.TryGetMessageText(tab, null, 100, out message),
                        "The initial message file should be cached.");
                    Equal("First cached value.", message);

                    File.WriteAllText(messagePath, "{100}{}{Updated value with a different length.}\n");
                    File.SetLastWriteTimeUtc(messagePath, DateTime.UtcNow.AddSeconds(2));

                    True(MessageFile.TryGetMessageText(tab, null, 100, out message),
                        "A changed message file should still resolve.");
                    Equal("Updated value with a different length.", message);
                } finally {
                    Settings.EncCodePage = oldEncoding;
                }
            });
        }

        private static void ScriptNameResolvesSharedMessageFile()
        {
            WithTempDirectory(directory => {
                string scriptsDirectory = Path.Combine(directory, "scripts");
                string dialogDirectory = Path.Combine(directory, "text", Settings.language, "dialog");
                Directory.CreateDirectory(scriptsDirectory);
                Directory.CreateDirectory(dialogDirectory);

                var scripts = Enumerable.Repeat("unused.int", 62).Concat(new[] { "Door.int" });
                File.WriteAllLines(Path.Combine(scriptsDirectory, "scripts.lst"), scripts);
                File.WriteAllText(Path.Combine(dialogDirectory, "door.msg"),
                    "{193}{}{That doesn't even put a scratch on the door.}\n");

                string oldOutputDir = Settings.outputDir;
                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.outputDir = scriptsDirectory;
                    Settings.EncCodePage = Encoding.UTF8;
                    var info = new ProgramInfo(0, 0);
                    info.macros.Add("NAME", new Macro("NAME", "NAME", "SCRIPT_DOOR", "jakedoor.ssl", 1, null));
                    info.macros.Add("SCRIPT_DOOR", new Macro("SCRIPT_DOOR", "SCRIPT_DOOR", "(63)", "scripts.h", 1, null));
                    var tab = new TabInfo {
                        filepath = Path.Combine(directory, "jakedoor.ssl"),
                        filename = "jakedoor.ssl",
                        parseInfo = info
                    };

                    True(MessageFile.TryLoadMessagesForScriptIdentity(tab),
                        "NAME should resolve through scripts.lst to the shared message file.");
                    Equal("That doesn't even put a scratch on the door.", tab.messages[193]);
                    Equal("door.msg", Path.GetFileName(tab.msgFilePath).ToLowerInvariant());
                } finally {
                    Settings.outputDir = oldOutputDir;
                    Settings.EncCodePage = oldEncoding;
                }
            });
        }

        private static void MessageStrResolvesExplicitMessageFile()
        {
            const string code = "display_msg(obj_name(source_obj) + message_str(SCRIPT_JUNKJAIL, 202));";
            string scriptToken;
            True(ToolTipRequest.TryGetMessageStrScriptToken(code, code.IndexOf("202", StringComparison.Ordinal) + 1, 202, out scriptToken),
                "Hovering the second message_str argument should identify its script token.");
            Equal("SCRIPT_JUNKJAIL", scriptToken);

            WithTempDirectory(directory => {
                string scriptsDirectory = Path.Combine(directory, "scripts");
                string dialogDirectory = Path.Combine(directory, "text", Settings.language, "dialog");
                Directory.CreateDirectory(scriptsDirectory);
                Directory.CreateDirectory(dialogDirectory);
                File.WriteAllLines(Path.Combine(scriptsDirectory, "scripts.lst"),
                    Enumerable.Repeat("unused.int", 788).Concat(new[] { "JunkJail.int" }));
                File.WriteAllText(Path.Combine(dialogDirectory, "junkjail.msg"),
                    "{202}{}{ fails to unlock the door.}\n");

                string oldOutputDir = Settings.outputDir;
                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.outputDir = scriptsDirectory;
                    Settings.EncCodePage = Encoding.UTF8;
                    var info = new ProgramInfo(0, 0);
                    info.macros.Add("SCRIPT_JUNKJAIL", new Macro("SCRIPT_JUNKJAIL", "SCRIPT_JUNKJAIL", "(789)", "scripts.h", 1, null));
                    var tab = new TabInfo {
                        filepath = Path.Combine(directory, "jakedoor.ssl"),
                        filename = "jakedoor.ssl",
                        parseInfo = info
                    };

                    string message;
                    True(MessageFile.TryGetMessageText(tab, scriptToken, 202, out message),
                        "message_str should resolve its explicit script's message file.");
                    Equal("fails to unlock the door.", message);
                } finally {
                    Settings.outputDir = oldOutputDir;
                    Settings.EncCodePage = oldEncoding;
                }
            });
        }

        private static void MessageWrapperMacroResolvesExplicitMessageFile()
        {
            const string code = "display_msg(dog_mstr(115));";
            var info = new ProgramInfo(0, 0);
            info.macros.Add("dog_mstr", new Macro("dog_mstr", "dog_mstr(x)",
                "message_str(SCRIPT_ALLDOGS, x)", "dogmeat.ssl", 48, null));
            info.macros.Add("SCRIPT_ALLDOGS", new Macro("SCRIPT_ALLDOGS", "SCRIPT_ALLDOGS",
                "(968)", "scripts.h", 982, null));
            var tab = new TabInfo {
                filepath = "dogmeat.ssl",
                filename = "dogmeat.ssl",
                parseInfo = info
            };

            string scriptToken;
            True(ToolTipRequest.TryGetMessageScriptToken(tab, code,
                    code.IndexOf("115", StringComparison.Ordinal) + 1, 115, out scriptToken),
                "Hovering a wrapper macro's message number should identify message_str's script token.");
            Equal("SCRIPT_ALLDOGS", scriptToken);

            WithTempDirectory(directory => {
                string scriptsDirectory = Path.Combine(directory, "scripts");
                string dialogDirectory = Path.Combine(directory, "text", Settings.language, "dialog");
                Directory.CreateDirectory(scriptsDirectory);
                Directory.CreateDirectory(dialogDirectory);
                File.WriteAllLines(Path.Combine(scriptsDirectory, "scripts.lst"),
                    Enumerable.Repeat("unused.int", 967).Concat(new[] { "AllDogs.int" }));
                File.WriteAllText(Path.Combine(dialogDirectory, "alldogs.msg"),
                    "{115}{}{The dog seems to think you are his owner.}\n");

                string oldOutputDir = Settings.outputDir;
                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.outputDir = scriptsDirectory;
                    Settings.EncCodePage = Encoding.UTF8;
                    tab.filepath = Path.Combine(directory, "dogmeat.ssl");

                    string message;
                    True(MessageFile.TryGetMessageText(tab, scriptToken, 115, out message),
                        "The wrapper macro should resolve its explicit message file.");
                    Equal("The dog seems to think you are his owner.", message);
                } finally {
                    Settings.outputDir = oldOutputDir;
                    Settings.EncCodePage = oldEncoding;
                }
            });
        }

        private static void RandomMessageRangePopulatesTooltipText()
        {
            const string code = "float_katja(random(310, 313));";
            int firstMessage;
            int lastMessage;
            True(ToolTipRequest.TryGetRandomMessageRange(code,
                    code.IndexOf("310", StringComparison.Ordinal) + 1, out firstMessage, out lastMessage),
                "Hovering the first random endpoint should identify the message range.");
            Equal(310, firstMessage);
            Equal(313, lastMessage);
            True(ToolTipRequest.TryGetRandomMessageRange(code,
                    code.IndexOf("313", StringComparison.Ordinal) + 1, out firstMessage, out lastMessage),
                "Hovering the last random endpoint should identify the message range.");

            WithTempDirectory(directory => {
                string scriptsDirectory = Path.Combine(directory, "scripts");
                string dialogDirectory = Path.Combine(directory, "text", Settings.language, "dialog");
                Directory.CreateDirectory(scriptsDirectory);
                Directory.CreateDirectory(dialogDirectory);
                File.WriteAllLines(Path.Combine(scriptsDirectory, "scripts.lst"),
                    Enumerable.Repeat("unused.int", 622).Concat(new[] { "Katja.int" }));
                File.WriteAllText(Path.Combine(dialogDirectory, "katja.msg"),
                    "{310}{}{I'm here.}\n"
                    + "{311}{}{Sorry about that.}\n"
                    + "{312}{}{Whatever.}\n"
                    + "{313}{}{Oh, excuse me.}\n");

                string oldOutputDir = Settings.outputDir;
                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.outputDir = scriptsDirectory;
                    Settings.EncCodePage = Encoding.UTF8;
                    var info = new ProgramInfo(0, 0);
                    info.macros.Add("NAME", new Macro("NAME", "NAME", "SCRIPT_KATJA", "katja.ssl", 1, null));
                    info.macros.Add("SCRIPT_KATJA", new Macro("SCRIPT_KATJA", "SCRIPT_KATJA", "(623)", "scripts.h", 635, null));
                    var tab = new TabInfo {
                        filepath = Path.Combine(directory, "katja.ssl"),
                        filename = "katja.ssl",
                        parseInfo = info
                    };

                    string tooltip;
                    True(ToolTipRequest.TryGetMessageRangeText(tab, firstMessage, lastMessage, out tooltip),
                        "The random range should load Katja's message file without opening it.");
                    True(tooltip.Contains("Messages 310-313"), "The tooltip should identify the full range.");
                    True(tooltip.Contains("310:") && tooltip.Contains("I'm here."),
                        "The tooltip should include message 310.");
                    True(tooltip.Contains("313:") && tooltip.Contains("Oh, excuse me."),
                        "The tooltip should include message 313.");
                } finally {
                    Settings.outputDir = oldOutputDir;
                    Settings.EncCodePage = oldEncoding;
                }
            });
        }

        private static void RandomMessageRangeResolvesExplicitMessageFile()
        {
            const string code = "message_str(SCRIPT_GENRAIDR, random(102, 106))";
            int firstMessage;
            int lastMessage;
            int hoverOffset = code.IndexOf("102", StringComparison.Ordinal) + 1;
            True(ToolTipRequest.TryGetRandomMessageRange(code, hoverOffset, out firstMessage, out lastMessage),
                "Hovering the random range should identify its endpoints.");

            string scriptToken;
            True(ToolTipRequest.TryGetMessageStrRandomScriptToken(code, hoverOffset,
                    firstMessage, lastMessage, out scriptToken),
                "The random range should identify its enclosing message_str script token.");
            Equal("SCRIPT_GENRAIDR", scriptToken);
            True(ToolTipRequest.TryGetMessageScriptToken(null, code, hoverOffset, firstMessage, out scriptToken),
                "Message navigation should resolve the first random endpoint through the enclosing message_str.");
            Equal("SCRIPT_GENRAIDR", scriptToken);
            int lastHoverOffset = code.IndexOf("106", StringComparison.Ordinal) + 1;
            True(ToolTipRequest.TryGetMessageScriptToken(null, code, lastHoverOffset, lastMessage, out scriptToken),
                "Message navigation should resolve the last random endpoint through the enclosing message_str.");
            Equal("SCRIPT_GENRAIDR", scriptToken);

            WithTempDirectory(directory => {
                string scriptsDirectory = Path.Combine(directory, "scripts");
                string dialogDirectory = Path.Combine(directory, "text", Settings.language, "dialog");
                Directory.CreateDirectory(scriptsDirectory);
                Directory.CreateDirectory(dialogDirectory);
                File.WriteAllLines(Path.Combine(scriptsDirectory, "scripts.lst"),
                    Enumerable.Repeat("unused.int", 99).Concat(new[] { "Diana.int", "GenRaidr.int" }));
                File.WriteAllText(Path.Combine(dialogDirectory, "diana.msg"),
                    "{102}{}{Wrong active-script message.}\n");
                string genRaidrPath = Path.Combine(dialogDirectory, "genraidr.msg");
                File.WriteAllText(genRaidrPath,
                    "{102}{}{Correct shared raider message.}\n"
                    + "{106}{}{Correct final raider message.}\n");

                string oldOutputDir = Settings.outputDir;
                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.outputDir = scriptsDirectory;
                    Settings.EncCodePage = Encoding.UTF8;
                    var info = new ProgramInfo(0, 0);
                    info.macros.Add("NAME", new Macro("NAME", "NAME", "SCRIPT_DIANA", "diana.ssl", 1, null));
                    info.macros.Add("SCRIPT_DIANA", new Macro("SCRIPT_DIANA", "SCRIPT_DIANA", "(100)", "scripts.h", 1, null));
                    info.macros.Add("SCRIPT_GENRAIDR", new Macro("SCRIPT_GENRAIDR", "SCRIPT_GENRAIDR", "(101)", "scripts.h", 1, null));
                    var tab = new TabInfo {
                        filepath = Path.Combine(directory, "diana.ssl"),
                        filename = "diana.ssl",
                        parseInfo = info
                    };

                    True(MessageFile.TryLoadMessagesForScriptIdentity(tab),
                        "The active Diana message file should be available for the regression case.");
                    string tooltip;
                    True(ToolTipRequest.TryGetMessageRangeText(tab, scriptToken,
                            firstMessage, lastMessage, out tooltip),
                        "The explicit random range should load the shared raider message file.");
                    True(tooltip.Contains("Correct shared raider message."),
                        "The tooltip should contain the explicit message file text.");
                    True(tooltip.Contains("Correct final raider message."),
                        "The tooltip should include the final range entry.");
                    True(!tooltip.Contains("Wrong active-script message."),
                        "The tooltip must not fall back to the active script's message text.");

                    string path;
                    int line;
                    True(MessageFile.TryGetMessageLocation(tab, scriptToken, firstMessage, out path, out line),
                        "Message navigation should find the first random endpoint in the explicit message file.");
                    True(String.Equals(genRaidrPath, path, StringComparison.OrdinalIgnoreCase),
                        "Message navigation should open GenRaidr.msg rather than Diana.msg.");
                    Equal(1, line);
                } finally {
                    Settings.outputDir = oldOutputDir;
                    Settings.EncCodePage = oldEncoding;
                }
            });
        }

        private static void MessageNavigationResolvesFileAndPhysicalLine()
        {
            const string source = "# comment\n\n{115}{}{The dog seems to think you are his owner.}\n";
            int line;
            True(MessageFile.TryFindMessageLine(source, 115, out line),
                "The message entry should be found after comments and blank lines.");
            Equal(3, line);

            WithTempDirectory(directory => {
                string scriptsDirectory = Path.Combine(directory, "scripts");
                string dialogDirectory = Path.Combine(directory, "text", Settings.language, "dialog");
                Directory.CreateDirectory(scriptsDirectory);
                Directory.CreateDirectory(dialogDirectory);

                string[] scripts = Enumerable.Repeat("unused.int", 968).ToArray();
                scripts[622] = "Katja.int";
                scripts[967] = "AllDogs.int";
                File.WriteAllLines(Path.Combine(scriptsDirectory, "scripts.lst"), scripts);

                string katjaPath = Path.Combine(dialogDirectory, "katja.msg");
                string allDogsPath = Path.Combine(dialogDirectory, "alldogs.msg");
                File.WriteAllText(katjaPath, "\n{310}{}{I'm here.}\n");
                File.WriteAllText(allDogsPath, source);

                string oldOutputDir = Settings.outputDir;
                Encoding oldEncoding = Settings.EncCodePage;
                try {
                    Settings.outputDir = scriptsDirectory;
                    Settings.EncCodePage = Encoding.UTF8;
                    var info = new ProgramInfo(0, 0);
                    info.macros.Add("NAME", new Macro("NAME", "NAME", "SCRIPT_KATJA", "katja.ssl", 1, null));
                    info.macros.Add("SCRIPT_KATJA", new Macro("SCRIPT_KATJA", "SCRIPT_KATJA", "(623)", "scripts.h", 635, null));
                    info.macros.Add("SCRIPT_ALLDOGS", new Macro("SCRIPT_ALLDOGS", "SCRIPT_ALLDOGS", "(968)", "scripts.h", 982, null));
                    var tab = new TabInfo {
                        filepath = Path.Combine(directory, "katja.ssl"),
                        filename = "katja.ssl",
                        parseInfo = info
                    };

                    string path;
                    True(MessageFile.TryGetMessageLocation(tab, null, 310, out path, out line),
                        "A normal message number should resolve through the script NAME.");
                    True(String.Equals(katjaPath, path, StringComparison.OrdinalIgnoreCase),
                        "The normal message path should match regardless of filename casing.");
                    Equal(2, line);

                    True(MessageFile.TryGetMessageLocation(tab, "SCRIPT_ALLDOGS", 115, out path, out line),
                        "An explicit message script token should resolve its own file.");
                    True(String.Equals(allDogsPath, path, StringComparison.OrdinalIgnoreCase),
                        "The explicit message path should match regardless of filename casing.");
                    Equal(3, line);
                } finally {
                    Settings.outputDir = oldOutputDir;
                    Settings.EncCodePage = oldEncoding;
                }
            });
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

        private static void PreviousTabSessionPreservesOrderAndSelection()
        {
            WithTempDirectory(directory => {
                string first = Path.Combine(directory, "first.ssl");
                string second = Path.Combine(directory, "second.ssl");
                File.WriteAllText(first, "procedure start begin end");
                File.WriteAllText(second, "procedure map_enter_p_proc begin end");

                try {
                    Settings.ClearLastSession();
                    Settings.SaveLastSession(new[] { first, second }, 1);

                    int selectedIndex;
                    string[] restored = Settings.LoadLastSession(out selectedIndex);
                    Equal(2, restored.Length);
                    Equal(Path.GetFullPath(first), restored[0]);
                    Equal(Path.GetFullPath(second), restored[1]);
                    Equal(1, selectedIndex);
                }
                finally {
                    Settings.ClearLastSession();
                }
            });
        }

        private static void NotificationSeverityIsConveyedInText()
        {
            Equal(String.Empty, EditorNotifications.GetPrefix(NotificationKind.Information));
            Equal("Success: ", EditorNotifications.GetPrefix(NotificationKind.Success));
            Equal("Notice: ", EditorNotifications.GetPrefix(NotificationKind.Warning));
            Equal("Error: ", EditorNotifications.GetPrefix(NotificationKind.Error));
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

            True(CodeFolder.HasProcedureOutsideLine(document, 4),
                "The toolbar should detect procedures other than the active one.");
            True(!CodeFolder.HasUnfoldedProcedureOutsideLine(document, 4),
                "When all other procedures are folded, the next toolbar action should expand them.");
            CodeFolder.SetProceduresOutsideLineFolded(document, 4, false);
            True(!first.IsFolded && !second.IsFolded,
                "The expand action should unfold procedures other than the active one.");
            True(CodeFolder.HasUnfoldedProcedureOutsideLine(document, 4),
                "An unfolded procedure should switch the next toolbar action to collapse.");
            CodeFolder.SetProceduresOutsideLineFolded(document, 4, true);
            True(first.IsFolded && !second.IsFolded,
                "The collapse action should fold procedures other than the active one.");
            True(!variables.IsFolded, "Toolbar folding must not affect non-procedure regions.");

            True(!CodeFolder.CollapseAllExceptProcedure(document, 7),
                "A line outside a procedure should not trigger the command.");
        }

        private static void DialogProceduresAreDiscoveredFromContent()
        {
            IDocument document = new DocumentFactory().CreateDocument();
            document.TextContent =
                "procedure Helper begin\r\n"
                + "   set_local_var(0, 1);\r\n"
                + "end\r\n"
                + "procedure Gizmo01 begin\r\n"
                + "   Reply(101);\r\n"
                + "   NOption(102, Gizmo02, 4);\r\n"
                + "end\r\n"
                + "procedure Gizmo02 begin\r\n"
                + "   NOption(104, DialogExit, 4);\r\n"
                + "end\r\n"
                + "procedure DialogExit begin\r\n"
                + "   NMessage(103);\r\n"
                + "end\r\n";

            var program = new ProgramInfo(4, 0);
            program.procs[0] = TestProcedure("Helper", 1, 3);
            program.procs[1] = TestProcedure("Gizmo01", 4, 7);
            program.procs[2] = TestProcedure("Gizmo02", 8, 10);
            program.procs[3] = TestProcedure("DialogExit", 11, 13);
            program.RebuildProcedureDictionary();
            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();

            List<string> nodes = DialogueParser.GetAllNodesName(document, program);
            True(nodes.SequenceEqual(new[] { "Gizmo01", "Gizmo02", "DialogExit" }),
                "Dialog operations and their linked procedures should be discovered without requiring '*node*' names or talk_p_proc.");
            True(DialogueParser.ProcedureContainsPreviewableDialog(document, program, program.procs[1]),
                "A procedure containing Reply should enable direct dialog preview.");
            True(!DialogueParser.ProcedureContainsPreviewableDialog(document, program, program.procs[0]),
                "A procedure without dialog operations should not enable direct dialog preview.");
            True(DialogueParser.ProcedureContainsPreviewableDialog(document, program, program.procs[2]),
                "A procedure containing an option without a reply should enable direct dialog preview.");
        }

        private static Procedure TestProcedure(string name, int start, int end)
        {
            return new Procedure {
                name = name,
                fdeclared = "dialog.ssl",
                fstart = "dialog.ssl",
                filename = "dialog.ssl",
                d = new ProcedureData { start = start, end = end, declared = start - 1 },
                variables = new Variable[0],
                references = new Reference[0]
            };
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

        private static void DpiMetricsUseLogicalUnits()
        {
            Equal(13, DpiHelper.Scale(13, 96));
            Equal(16, DpiHelper.Scale(13, 120));
            Equal(20, DpiHelper.Scale(13, 144));
            Equal(26, DpiHelper.Scale(13, 192));
            Equal(1, DpiHelper.Scale(1, 120));
            Equal(2, DpiHelper.Scale(1, 144));
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
