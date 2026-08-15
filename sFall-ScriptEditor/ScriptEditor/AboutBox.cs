using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ScriptEditor
{
    partial class AboutBox : Form
    {
        public const string appName = "Sfall Script Editor";
        public const string repositoryUrl = "https://github.com/rotators/fallout-tools";
        public const string sfallDocumentationUrl = "https://sfall-team.github.io/sfall/";
        public const string appVersion = "5.0";
        public static readonly string appDescription = " - Rotators Build";
        private int licenseLinkStart = -1;
        private int licenseLinkLength;
        public AboutBox()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            this.Text += appName + appDescription;
            this.labelVersion.Text = String.Format("Version {0}   Build {1}", appVersion,
                File.GetLastWriteTime(Application.ExecutablePath).ToString("yyyy-MM-dd HH:mm"));
            InterfaceTheme.Apply(this);
            FormatDescription();
        }

        private void Repository_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenRepository(this);
        }

        private void SfallDocumentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenSfallDocumentation(this);
        }

        internal static void OpenRepository(IWin32Window owner)
        {
            OpenExternalLink(owner, repositoryUrl, "repository");
        }

        internal static void OpenSfallDocumentation(IWin32Window owner)
        {
            OpenExternalLink(owner, sfallDocumentationUrl, "scripting documentation");
        }

        private void LocalDocumentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenDocumentationFolder(this);
        }

        internal static void OpenDocumentationFolder(IWin32Window owner)
        {
            string documentationPath = Path.Combine(Application.StartupPath, "docs");
            if (!Directory.Exists(documentationPath)) {
                MessageBox.Show(owner, "The local documentation folder could not be found:\n" + documentationPath,
                    "Open documentation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try {
                Process.Start(new ProcessStartInfo(documentationPath) { UseShellExecute = true });
            } catch (Exception ex) {
                MessageBox.Show(owner, "Could not open the documentation folder.\n" + ex.Message,
                    "Open documentation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Description_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            OpenUrl(e.LinkText);
        }

        private void Description_MouseMove(object sender, MouseEventArgs e)
        {
            textBoxDescription.Cursor = IsLicenseLink(e.Location) ? Cursors.Hand : Cursors.IBeam;
        }

        private void Description_MouseLeave(object sender, EventArgs e)
        {
            textBoxDescription.Cursor = Cursors.IBeam;
        }

        private void Description_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsLicenseLink(e.Location))
                OpenLicenseFile();
        }

        private bool IsLicenseLink(Point location)
        {
            int index = textBoxDescription.GetCharIndexFromPosition(location);
            return licenseLinkStart >= 0 && index >= licenseLinkStart &&
                index < licenseLinkStart + licenseLinkLength;
        }

        private void OpenLicenseFile()
        {
            string licensePath = Path.Combine(Application.StartupPath, "licences.txt");
            if (!File.Exists(licensePath)) {
                MessageBox.Show("The license file could not be found:\n" + licensePath,
                    "Open licenses", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenUrl(licensePath);
        }

        private void OpenUrl(string url)
        {
            OpenExternalLink(this, url, "link");
        }

        private static void OpenExternalLink(IWin32Window owner, string url, string description)
        {
            try {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            } catch (Exception ex) {
                MessageBox.Show(owner, "Could not open the " + description + ".\n" + ex.Message,
                    "Open link", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDescription()
        {
            bool dark = InterfaceTheme.IsDark;
            Color headingColor = dark ? Color.FromArgb(105, 180, 255) : Color.FromArgb(35, 85, 145);
            Color bodyColor = dark ? Color.Gainsboro : SystemColors.WindowText;
            Color secondaryColor = dark ? Color.FromArgb(185, 185, 190) : Color.DimGray;

            labelProductName.ForeColor = headingColor;
            textBoxDescription.Clear();
            textBoxDescription.DetectUrls = true;
            textBoxDescription.LinkClicked += Description_LinkClicked;
            textBoxDescription.MouseMove += Description_MouseMove;
            textBoxDescription.MouseLeave += Description_MouseLeave;
            textBoxDescription.MouseClick += Description_MouseClick;

            using (Font headingFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (Font bodyFont = new Font("Segoe UI", 9F, FontStyle.Regular))
            using (Font linkFont = new Font("Segoe UI", 9F, FontStyle.Underline)) {
                AppendHeading("Copyright", headingFont, headingColor);
                AppendParagraph(AssemblyCopyright, bodyFont, bodyColor);
                AppendParagraph("Original editor by the Sfall Team, 2010-2021\nExtended version by Mr.Stalin", bodyFont, secondaryColor);

                AppendHeading("Code editor", headingFont, headingColor);
                AppendParagraph("ICSharpCode.TextEditor 3.2.1 and DiagramCanvas 3.2.1 - LGPL", bodyFont, bodyColor);
                AppendParagraph("Copyright 2002-2010 AlphaSierraPapa and Christoph Wille\nControls modified by Mr.Stalin\nhttps://www.icsharpcode.net/opensource/sd/", bodyFont, secondaryColor);

                AppendHeading("Script compiler", headingFont, headingColor);
                AppendParagraph("SSLC, sfall edition - for the sfall 4.0 series", bodyFont, bodyColor);

                AppendHeading("Preprocessors", headingFont, headingColor);
                AppendParagraph("MCPP 2.7.2 (BCD)\nCopyright 1998, 2002-2008 Kiyoshi Matsui", bodyFont, bodyColor);
                AppendParagraph("Open Watcom C32 Optimizing Compiler 2.0 beta\nCopyright 1984-2002 Sybase, Inc.\nCopyright 2002-2017 The Open Watcom Contributors", bodyFont, secondaryColor);

                AppendHeading("Decompiler", headingFont, headingColor);
                AppendParagraph("int2ssl 8.4.6\nCopyright 2005-2009 Anchorite (TeamX)", bodyFont, bodyColor);

                AppendHeading("Licenses", headingFont, headingColor);
                AppendFormatted("Full license texts are available in ", bodyFont, secondaryColor);
                AppendLicenseLink("licences.txt", linkFont,
                    dark ? Color.FromArgb(86, 156, 214) : Color.MediumBlue);
                AppendFormatted("." + Environment.NewLine, bodyFont, secondaryColor);
            }

            textBoxDescription.SelectAll();
            textBoxDescription.SelectionIndent = 8;
            textBoxDescription.SelectionRightIndent = 12;
            textBoxDescription.Select(0, 0);
            textBoxDescription.ScrollToCaret();
        }

        private void AppendHeading(string text, Font font, Color color)
        {
            if (textBoxDescription.TextLength > 0)
                textBoxDescription.AppendText(Environment.NewLine);
            AppendFormatted(text + Environment.NewLine, font, color);
        }

        private void AppendParagraph(string text, Font font, Color color)
        {
            AppendFormatted(text + Environment.NewLine, font, color);
        }

        private void AppendLicenseLink(string text, Font font, Color color)
        {
            licenseLinkStart = textBoxDescription.TextLength;
            licenseLinkLength = text.Length;
            AppendFormatted(text, font, color);
        }

        private void AppendFormatted(string text, Font font, Color color)
        {
            int start = textBoxDescription.TextLength;
            textBoxDescription.AppendText(text);
            textBoxDescription.Select(start, text.Length);
            textBoxDescription.SelectionFont = font;
            textBoxDescription.SelectionColor = color;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0) {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "") {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

    }
}
