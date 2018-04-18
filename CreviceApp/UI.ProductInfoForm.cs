using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Crevice.UI
{
    using Crevice.Config;

    public partial class ProductInfoForm : Form
    {
        private CLIOption.Result cliOption;

        public ProductInfoForm(CLIOption.Result cliOption)
        {
            this.cliOption = cliOption;
            this.Icon = Properties.Resources.CreviceIcon;
            InitializeComponent();
        }

        private const string url = "https://github.com/creviceapp/creviceapp";

        protected string GetInfoMessage()
        {
            return string.Format(Properties.Resources.ProductInfo,
                "Crevice",
                Application.ProductVersion,
                ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright,
                url,
                cliOption.helpMessageHtml);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            webBrowser1.DocumentText = GetInfoMessage();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme.StartsWith("http"))
            {
                e.Cancel = true;
                Process.Start(e.Url.ToString());
            }
        }
    }
}
