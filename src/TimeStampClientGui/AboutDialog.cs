/*
*  Copyright 2016 Disig a.s.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*/

/*
*  Written by:
*  Marek KLEIN <kleinmrk@gmail.com>
*/

using System;
using Eto.Drawing;
using Eto.Forms;

namespace Disig.TimeStampClient.Gui
{
    internal class AboutDialog : Dialog
    {
        internal AboutDialog()
        {
            this.InitializeComponent();
            return;
        }        

        private Button WebsiteButton
        {
            get;
            set;
        }

        private Button CloseButton
        {
            get;
            set;
        }

        private TextArea LicenseTextArea
        {
            get;
            set;
        }

        private TextArea ComponentsTextArea
        {
            get;
            set;
        }

        private void InitializeComponent()
        {
            this.Title = "About";
            this.InitControls();

            this.Content = this.CreateAboutContent();
            this.MinimumSize = new Size(400, 300);
        }

        private Layout CreateAboutContent()
        {
            TabControl tabControl = new TabControl();
            TabPage licensePage = new TabPage();

            licensePage.Text = "License";
            licensePage.Content = new TableLayout
            {
                Rows =
                {
                    new TableRow
                    {
                        ScaleHeight = true,
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                Rows =
                                    {
                                    new TableRow { Cells = { this.LicenseTextArea }, ScaleHeight = true },
                                    },
                                }, true),
                        }
                    },
                }
            };

            TabPage componentsPage = new TabPage();
            componentsPage.Text = "3rd party components";
            componentsPage.Content = new TableLayout
            {
                Rows =
                {
                    new TableRow
                    {
                        ScaleHeight = true,
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow { Cells = { this.ComponentsTextArea }, ScaleHeight = true },
                                    },
                                }, true),
                        }
                    },
                }
            };

            tabControl.Pages.Add(licensePage);
            tabControl.Pages.Add(componentsPage);

            return new TableLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    new TableRow { Cells = { tabControl }, ScaleHeight = true },
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.WebsiteButton, new TableCell { ScaleWidth = true }, this.CloseButton),
                                        new TableRow { ScaleHeight = true },
                                    },
                                }, true),
                        },
                    },
                },
            };
        }

        private void InitControls()
        {
            this.WebsiteButton = new Button();
            this.WebsiteButton.Text = "Website";
            this.WebsiteButton.Click += this.WebsiteButtonOnClick;

            this.CloseButton = new Button();
            this.CloseButton.Text = "Close";
            this.CloseButton.Click += this.CloseButtonOnClick;

            this.LicenseTextArea = new RichTextArea();
            this.FillLicenseTextArea();

            this.ComponentsTextArea = new RichTextArea();
            this.ComponentsTextArea.ReadOnly = true;
            this.ComponentsTextArea.Text = string.Empty;
            this.ComponentsTextArea.Append(@"TimeStampClient uses following 3rd party components (in alphabetical order):" + Environment.NewLine
                + Environment.NewLine + "- DotNetZip.Reduced" + Environment.NewLine + "- Eto.Forms" + Environment.NewLine + "- Eto.Platform.Gtk" + Environment.NewLine
                + "- Eto.Platform.Gtk3" + Environment.NewLine + "- Eto.Platform.Mac" + Environment.NewLine
                + "- Eto.Platform.Windows" + Environment.NewLine + "- Eto.Platform.Wpf" + Environment.NewLine + "- Portable.BouncyCastle"
                + Environment.NewLine + Environment.NewLine + "Full license text for each of these components can be found in the installation directory.");
        }

        private void FillLicenseTextArea()
        {
            this.LicenseTextArea.ReadOnly = true;
            this.LicenseTextArea.Append(@"TimeStampClient " + SharedUtils.AppVersion + Environment.NewLine);
            this.LicenseTextArea.Append(@"Copyright 2016 Disig a.s." + Environment.NewLine);
            this.LicenseTextArea.Append(Environment.NewLine + "Licensed under the Apache License, Version 2.0 (the \"License\"); " +
                                        "you may not use this file except in compliance with the License. You may obtain a copy of the License at " +
                                        Environment.NewLine + Environment.NewLine + "http://www.apache.org/licenses/LICENSE-2.0" + Environment.NewLine + Environment.NewLine +
                                        "Unless required by applicable law or agreed to in writing, software distributed under the License is " +
                                        "distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. " +
                                        "See the License for the specific language governing permissions and limitations under the License." + Environment.NewLine);
        }

        private void CloseDialogWindow()
        {
            this.Close();
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            this.CloseDialogWindow();
        }

        private static void OpenWebSite()
        {
            System.Diagnostics.Process.Start("https://github.com/disig/TimeStampClient");
        }

        private void WebsiteButtonOnClick(object sender, EventArgs e)
        {
            OpenWebSite();
        }
    }
}
