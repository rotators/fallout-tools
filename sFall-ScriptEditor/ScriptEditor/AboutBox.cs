using System;
using System.Reflection;
using System.Windows.Forms;

namespace ScriptEditor
{
    partial class AboutBox : Form
    {
        public const string appName = "Sfall Script Editor";
        public static readonly string appVersion = Application.ProductVersion + ".RC1";
        public static readonly string appDescription = " - extended version by Mr.Stalin";
        
        public AboutBox()
        {
            InitializeComponent();

            if (logoPictureBox.Size.Height - 10 > logoPictureBox.Image.Height)
                logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            this.Text += appName + appDescription;
            this.labelProductName.Text = appName;
            this.labelVersion.Text = String.Format("Version {0}", appVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.textBoxDescription.Text =
@"Code editor control is from ICSharpCode.TextEditor 3.2.1 (LGPL)
and ICSharpCode.DiagramCanvas 3.2.1 (LGPL)
http://www.icsharpcode.net/opensource/sd/
Copyright 2002-2010 by AlphaSierraPapa, Christoph Wille
Controls modified by Mr.Stalin

Script compilation is handled by sslc sfall edition
The sfall script compiler, for sfall 4.0 series

Script preprocessing handled by mcpp 2.7.2 (BCD)
Copyright (c) 1998, 2002-2008 Kiyoshi Matsui

Script preprocessing handled by Open Watcom C32
Optimizing Compiler version 2.0 beta (01/11/2017)
Copyright (c) 1984-2002 Sybase, Inc. All Rights Reserved.
Copyright (c) 2002-2017 The Open Watcom Contributors.

Script decompilation handled by int2ssl 8.4.6
Copyright (C) Anchorite (TeamX), 2005-2009

See licences.txt for licence texts.
";
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
