﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.UI
{
    using Crevice.Logging;
    using Crevice.Config;

    public partial class LauncherForm : Form
    {
        private readonly MainFormBase _mainForm;

        public LauncherForm(MainFormBase mainForm)
        {
            _mainForm = mainForm;
            Icon = Properties.Resources.CreviceIcon;
            InitializeComponent();
        }

        private static Microsoft.Win32.RegistryKey AutorunRegistry()
            => Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
        
        private static bool AutoRun
        {
            get
            {
                var registry = AutorunRegistry();
                try
                {
                    var res = registry.GetValue(Application.ProductName);
                    return res != null &&
                           (string)res == Application.ExecutablePath;
                }
                finally
                {
                    registry.Close();
                }
            }
            set
            {
                if (value)
                {
                    var registry = AutorunRegistry();
                    registry.SetValue(Application.ProductName, Application.ExecutablePath);
                    Verbose.Print("Autorun was set to true");
                    registry.Close();
                }
                else
                {
                    var registry = AutorunRegistry();
                    if (registry.GetValue(Application.ProductName) != null)
                    {
                        try
                        {
                            registry.DeleteValue(Application.ProductName);
                            Verbose.Print("Autorun was set to false");
                        }
                        catch (ArgumentException ex)
                        {
                            Verbose.Error("An exception was thrown while writing registory value: {0}", ex.ToString());
                        }
                    }
                    registry.Close();
                }
            }
        }

        private void SaveSettings()
        {
            AutoRun = checkBox1.Checked;
        }

        private void ShowProductInfoForm()
        {
            string[] args = { "--help" };
            var cliOption = CLIOption.Parse(args);
            var form = new ProductInfoForm(cliOption);
            form.Show();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            checkBox1.Checked = AutoRun;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Opacity = 0;
            Show();
            var rect = Screen.PrimaryScreen.Bounds;
            Left = (rect.Width - Width) / 2;
            Top = (rect.Height - Height) / 2;
            Opacity = 1;
            Activate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SaveSettings();
            base.OnClosing(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            _mainForm.Close();
        }

        private void button2_Click(object sender, EventArgs e)
            => _mainForm.OpenUserScriptWithNotepad();

        private void button3_Click(object sender, EventArgs e)
            => ShowProductInfoForm();
    }
}
