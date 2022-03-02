using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI.ToolTips;

namespace ScriptEditor.TextEditorUI.CompleteList
{
    /// <summary>
    /// Отрисовывает границы списка цветом определенным BorderColor
    /// </summary>
    public class CompleteListBox : ListBox
    {
        const int  WM_NCPAINT     = 0x85;
        //const uint RDW_INVALIDATE = 0x1;
        //const uint RDW_IUPDATENOW = 0x100;
        //const uint RDW_FRAME      = 0x400;

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /*
        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprc, IntPtr hrgn, uint flags);

        void RedrawWindowControl()
        {
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RedrawWindowControl();
        }
        */

        public Color BorderColor { get; set; }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCPAINT && BorderStyle != System.Windows.Forms.BorderStyle.None && BorderColor != Color.Transparent) {
                var hdc = GetWindowDC(this.Handle);

                using (var g = Graphics.FromHdcInternal(hdc))
                using (var p = new Pen(BorderColor))
                    g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));

                ReleaseDC(this.Handle, hdc);
            }
        }
    }

    public class AutoComplete
    {
        private const int countItems = 12;
        private const int height = 17;

        private int x, y, itemWidth, itemHeight;
        private bool hidden;

        private LinearGradientBrush SelectItemBrush = new LinearGradientBrush(
                                        new PointF(0, 0), new PointF(0, height),
                                        Color.White, Color.FromArgb(240, 190, 100));

        public bool ShiftCaret { get; set; } // используется для возврата курсора каретки на ключевое слово.

        bool colored;
        public bool Colored {
            private get { return colored; }
            set { colored = value;
                  AutoComleteList.Font = colored ? font.BoldFont : font.RegularFont; }
        }

        public KeyValuePair<int, int> WordPosition { get; set; }
        private TextAreaControl TAC { get; set; }

        private CompleteListBox AutoComleteList;
        private ToolTip tipAC;
        private Panel panel;
        private ImageList imageList;
        private FontContainer font;
        private Point mousePosition = new Point();

        public AutoComplete(Panel panel, bool colored)
        {
            this.panel = panel;
            this.colored = colored;

            FontFamily family = Settings.Fonts.Families.FirstOrDefault(f => f.Name == "InputMono");
            this.font = new FontContainer(new Font((family ?? FontFamily.GenericMonospace), 10, FontStyle.Regular, GraphicsUnit.Point));

            AutoComleteList = new CompleteListBox();
            AutoComleteList.Cursor = Cursors.Help;
            AutoComleteList.ItemHeight = height;
            AutoComleteList.MaximumSize = new Size(350, (countItems * height) + 4);
            AutoComleteList.MinimumSize = new Size(120, height + 3);
            AutoComleteList.Font = colored ? font.BoldFont : font.RegularFont;
            AutoComleteList.Visible = false;
            AutoComleteList.DrawMode = DrawMode.OwnerDrawFixed;
            AutoComleteList.IntegralHeight = false;

            AutoComleteList.DrawItem += ACL_Draw;
            AutoComleteList.VisibleChanged += ACL_VisibleChanged;
            AutoComleteList.SelectedIndexChanged += ACL_SelectedIndexChanged;
            AutoComleteList.MouseMove += ACL_MouseMove;
            AutoComleteList.MouseEnter += ACL_MouseEnter;
            AutoComleteList.PreviewKeyDown += ACL_PreviewKeyDown;
            AutoComleteList.MouseClick += ACL_MouseClick;
            AutoComleteList.KeyDown += ACL_KeyDown;

            tipAC = new ToolTip();
            tipAC.OwnerDraw = true;
            tipAC.Draw += tipDraw;

            imageList = new ImageList();
            imageList.TransparentColor = Color.FromArgb(255, 0, 255);
            imageList.Images.Add(NameType.Macro.ToString(), Properties.Resources.macros);
            imageList.Images.Add(NameType.Proc.ToString(), Properties.Resources.procedure);
            imageList.Images.Add(NameType.GVar.ToString(), Properties.Resources.variable);
            imageList.Images.Add(NameType.None.ToString(), Properties.Resources.opcode);

            panel.Controls.Add(AutoComleteList);
            AutoComleteList.BringToFront();

            UpdateColor();
            Program.SetDoubleBuffered(AutoComleteList);
        }

        public void UpdateColor()
        {
            if (ColorTheme.IsDarkTheme) {
                AutoComleteList.BackColor = ColorTheme.TipGradient.BackgroundColor;
                AutoComleteList.BorderColor = Color.DimGray;
                AutoComleteList.BorderStyle = BorderStyle.FixedSingle;
            } else {
                AutoComleteList.BackColor = Color.GhostWhite;
                AutoComleteList.BorderColor = Color.Transparent;
                AutoComleteList.BorderStyle = BorderStyle.Fixed3D;
            }
        }

        public void Hide()
        {
            if (AutoComleteList.Visible) {
                TAC.TextEditorProperties.MouseWheelTextZoom = false;
                hidden = true;
                AutoComleteList.Hide();
            }
        }

        public void UnHide()
        {
            if (hidden) {
                hidden = false;
                if (SetScrollPosition(TAC)) AutoComleteList.Show();
            }
            if (TAC != null) TAC.TextEditorProperties.MouseWheelTextZoom = true;
        }

        public void Close()
        {
            hidden = false;
            AutoComleteList.Hide();
            AutoComleteList.Items.Clear();
            mousePosition = new Point();
        }

        public void Show()
        {
            hidden = false;
            AutoComleteList.Show();
        }

        public bool IsVisible
        {
            get { return (AutoComleteList.Visible | hidden); }
        }

        private void ShowItemTip()
        {
            AutoCompleteItem acItem = (AutoCompleteItem)AutoComleteList.SelectedItem;
            if (acItem != null /*&& AutoComleteList.Focused*/) {
                tipAC.Show(acItem.Hint, panel, AutoComleteList.Left
                           + AutoComleteList.Width + 5, AutoComleteList.Top, 50000);
            }
        }

        private void PasteSelectedItem()
        {
            ShiftCaret = false;

            AutoCompleteItem item = (AutoCompleteItem)AutoComleteList.SelectedItem;

            int startOffs = TextUtilities.FindWordStart(TAC.Document, WordPosition.Key); //WordPosition.Key - WordPosition.Value.Length;
            TAC.Document.Replace(startOffs, WordPosition.Value, item.Name);
            TAC.Caret.Position = TAC.Document.OffsetToPosition(startOffs + item.NameLength);

            AutoComleteList.Hide();
            TAC.TextArea.Focus();
            TAC.TextArea.Select();
        }

        public void GenerateList(string keyChar, TabInfo cTab, int caretOffset, object showTip, bool back = false)
        {
            if (!cTab.shouldParse)
                return;

            bool pressKeyAutoComplite = (keyChar == String.Empty);

            TAC = cTab.textEditor.ActiveTextAreaControl;

            string word = TextUtilities.GetWordAt(TAC.Document, caretOffset) + keyChar;
            int wordLen = word.Length;
            if (pressKeyAutoComplite) {
                if (word == String.Empty) {
                    word = TextUtilities.GetWordAt(TAC.Document, --caretOffset);
                    wordLen = word.Length;
                } else {
                    if (wordLen > 2) {
                        int wordStart = caretOffset - TextUtilities.FindWordStart(TAC.Document, caretOffset);
                        word = word.Remove(wordStart);
                    } else
                        word = null;
                }
            }

            if (back && word != null) {
                if (wordLen > 2) {
                    wordLen--;
                    //word = word.Remove(wordLen);
                } else
                    word = null;
            }

            if (word != null && word.Length > 1) {
                var matches = (cTab.parseInfo != null)
                    ? cTab.parseInfo.LookupAutosuggest(word)
                    : ProgramInfo.LookupOpcode(word);

                int shift = (back) ? -1 : pressKeyAutoComplite ? 0 : 1;

                if (matches.Count > 0) {
                    AutoComleteList.BeginUpdate();
                    AutoComleteList.Items.Clear();
                    int maxLen = 0;
                    foreach (string item in matches)
                    {
                        AutoCompleteItem acItem = new AutoCompleteItem(item);
                        AutoComleteList.Items.Add(acItem);
                        if (acItem.NameLength > maxLen)
                            maxLen = acItem.NameLength;
                    }
                    AutoComleteList.EndUpdate();

                    // size
                    AutoComleteList.Height = AutoComleteList.PreferredHeight - 3;
                    AutoComleteList.Width = (maxLen * 10) + shift_x
                                             + ((AutoComleteList.Items.Count > countItems) ? 15 : 0);

                    if (!AutoComleteList.Visible || back) {
                        var caretPos = TAC.Caret.ScreenPosition;
                        var tePos = TAC.FindForm().PointToClient(TAC.Parent.PointToScreen(TAC.Location));
                        tePos.Offset(caretPos);
                        if (back)
                            tePos.Offset(-6, 18);
                        else
                            tePos.Offset(pressKeyAutoComplite ? 0 : 10, 18);

                        if (showTip != null && (bool)showTip)
                            tePos.Offset(0, 22);

                        if (!back || AutoComleteList.Location.X > tePos.X) {
                            AutoComleteList.Location = tePos;
                        }
                        AutoComleteList.Show();
                    }
                    WordPosition = new KeyValuePair<int, int>(TAC.Caret.Offset + shift, wordLen);
                } else if (AutoComleteList.Visible)
                    WordPosition = new KeyValuePair<int, int>(TAC.Caret.Offset + shift, wordLen);
            } else if (AutoComleteList.Visible) {
                        AutoComleteList.Hide();
            }
        }

        private const int shiftY = 50;

        private bool SetScrollPosition(TextAreaControl tac)
        {
            int bottom = shiftY + tac.Height + tac.Location.Y;
            int top = shiftY + tac.Parent.Height - tac.Height;

            var tePos = tac.FindForm().PointToClient(tac.Parent.PointToScreen(tac.Location));
            var caretPos = tac.Caret.ScreenPosition;

            tePos.Offset(caretPos);
            tePos.Offset(0, 18);

            if (!hidden && (tePos.Y > bottom || tePos.Y < tac.Location.Y + shiftY)) {
                Close();
                return false;
            } else
                tipAC.Hide(panel);

            tePos.X = AutoComleteList.Location.X;
            AutoComleteList.Location = tePos;
            return true;
        }

        public void TA_MouseScroll(TextAreaControl tac)
        {
            if (IsVisible) SetScrollPosition(tac);
        }

        public void TA_PreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down
                || (e.KeyCode == Keys.Up && AutoComleteList.SelectedIndex != -1)) {
                ShiftCaret = true;
                if (AutoComleteList.SelectedIndex < 0)
                    AutoComleteList.SelectedIndex = 0;
                AutoComleteList.Focus();
                ShowItemTip();
                if (e.KeyCode == Keys.Down)
                    TAC.Caret.Line -= 1;
                else if (e.KeyCode == Keys.Up)
                    TAC.Caret.Line += 1;
            } else if (e.KeyCode == Keys.Enter && AutoComleteList.SelectedIndex != -1) {
                PasteSelectedItem();
                e.IsInputKey = true;
            } else if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.Up && AutoComleteList.SelectedIndex == -1)
                      || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space) {
                    ACListClose();
            } else if (!e.Shift && !e.Control && !e.Alt) {
                    int caret = TAC.Caret.Offset;
                    if (e.KeyCode == Keys.Left)
                        caret--;
                    //else if (e.KeyCode == Keys.Right)
                    //    caret++;
                    if (!TextUtilities.IsLetterDigitOrUnderscore(TAC.Document.GetCharAt(caret)))
                        Close();
            }
        }

        private void ACListClose()
        {
            Close();
            ShiftCaret = false;
            TAC.Caret.Position = TAC.Document.OffsetToPosition(WordPosition.Key);
            TAC.TextArea.Focus();
        }

        private void ACL_MouseClick(object sender, MouseEventArgs e)
        {
            PasteSelectedItem();
        }

        private void ACL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if ((e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter) && AutoComleteList.SelectedIndex != -1)
                PasteSelectedItem();
            else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space) {
                ACListClose();
            } else if (e.KeyCode == Keys.Left ) {
                TAC.TextArea.Focus();
                TAC.Caret.Column -= 1;
            } else if (e.KeyCode == Keys.Right) {
                TAC.TextArea.Focus();
                TAC.Caret.Column += 1;
            }
        }

        private void ACL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                e.Handled = true;
                tipAC.Hide(panel);
            }
        }

        private void ACL_MouseEnter(object sender, EventArgs e)
        {
            if (AutoComleteList.SelectedIndex < 0) return;
            AutoComleteList.Focus();
        }

        private void ACL_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePosition.IsEmpty) mousePosition = e.Location;
            if (mousePosition == e.Location) return;

            // устранение дребезга мышки
            int jitter = Math.Abs(e.Location.X - mousePosition.X) + Math.Abs(e.Location.Y - mousePosition.Y);
            if (jitter < 5) return;
            mousePosition = e.Location;

            int item = 0;
            if (e.Y != 0)
                item = e.Y / AutoComleteList.ItemHeight;
            int selIndex = AutoComleteList.TopIndex + item;
            if (selIndex < AutoComleteList.Items.Count)
                AutoComleteList.SelectedIndex = selIndex;
        }

        private void ACL_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowItemTip();
        }

        private void ACL_VisibleChanged(object sender, EventArgs e)
        {
            tipAC.Hide(panel);
        }

        void tipDraw(object sender, DrawToolTipEventArgs e)
        {
            TipPainter.DrawInfo(e);
        }

        // Specify custom text formatting flags
        static StringFormat sf = new StringFormat() { Trimming = StringTrimming.EllipsisCharacter };

        const int shift_x = 20;

        private void ACL_Draw(object s, DrawItemEventArgs e)
        {
            AutoCompleteItem acItem = (AutoCompleteItem)AutoComleteList.Items[e.Index];

            x = e.Bounds.X;
            y = e.Bounds.Y;
            itemWidth = e.Bounds.Width;
            itemHeight = e.Bounds.Height;

            Image image = imageList.Images[acItem.GetType.ToString()];

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (!colored) e.Graphics.TextContrast = 0;

            e.DrawBackground();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                e.Graphics.FillRectangle(SelectItemBrush, e.Bounds);
                e.Graphics.DrawImage(image, x + 2, y, 16, 16);
                e.Graphics.DrawRectangle(new Pen(Color.Peru, 1), x, y, itemWidth - 1, itemHeight - 1);
                e.Graphics.DrawString(acItem.Name, e.Font, Brushes.Black, new RectangleF(x + shift_x, y, itemWidth - shift_x + 5, itemHeight), sf);
            } else {
                e.Graphics.DrawImage(image, x + 2, y, 16, 16);
                e.Graphics.DrawString(acItem.Name, e.Font, acItem.GetBrush(colored), new RectangleF(x + shift_x, y, itemWidth - shift_x + 5, itemHeight), sf);
            }
        }
    }
}