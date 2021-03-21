/*
*  Copyright 2016-2021 Disig a.s.
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
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Eto.Drawing;
using Eto.Forms;

namespace Disig.TimeStampClient.Gui
{
    public class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();
            return;
        }

        private Label SourceFileLabel
        {
            get;
            set;
        }

        private TextBox SourceFile
        {
            get;
            set;
        }

        private Label ServerAddressLabel
        {
            get;
            set;
        }

        private TextBox ServerAddress
        {
            get;
            set;
        }

        private Label HashAlgorithmLabel
        {
            get;
            set;
        }

        private DropDown HashAlgorithm
        {
            get;
            set;
        }

        private Label RequestedPolicyLabel
        {
            get;
            set;
        }

        private TextBox RequestedPolicy
        {
            get;
            set;
        }

        private Label NonceLabel
        {
            get;
            set;
        }

        private TextBox Nonce
        {
            get;
            set;
        }

        private Label TSACertificateLabel
        {
            get;
            set;
        }

        private DropDown TSACertificate
        {
            get;
            set;
        }

        private Button RequestTimeStampButton
        {
            get;
            set;
        }

        private Button SourceFileButton
        {
            get;
            set;
        }

        private Button GenerateNonce
        {
            get;
            set;
        }

        private Label LogAreaLabel
        {
            get;
            set;
        }

        private TextArea LogArea
        {
            get;
            set;
        }

        private Label UserCertLabel
        {
            get;
            set;
        }

        private TextBox UserCert
        {
            get;
            set;
        }

        private Button UserCertButton
        {
            get;
            set;
        }

        private PasswordBox UserCertPassword
        {
            get;
            set;
        }

        private Label UserCertPasswordLabel
        {
            get;
            set;
        }

        private TextBox UserName
        {
            get;
            set;
        }

        private Label UserNameLabel
        {
            get;
            set;
        }

        private PasswordBox UserPassword
        {
            get;
            set;
        }

        private Label UserPasswordLabel
        {
            get;
            set;
        }

        private Label ResponseFormatLabel
        {
            get;
            set;
        }

        private DropDown ResponseFormat
        {
            get;
            set;
        }

        private TextBox OutFile
        {
            get;
            set;
        }

        private Label OutFileLabel
        {
            get;
            set;
        }

        private Button OutFileButton
        {
            get;
            set;
        }

        private CheckMenuItem LogExceptions
        {
            get;
            set;
        }

        private void InitializeComponent()
        {
            this.InitControls();
            this.Content = this.CreateMainContent();

            Command loadXml = new Command { MenuText = "Load..." };
            loadXml.Executed += this.MenuConfigurationLoadOnclick;

            Command saveXml = new Command { MenuText = "Save as..." };
            saveXml.Executed += this.MenuConfigurationSaveOnclick;

            Command about = new Command { MenuText = "About" };
            about.Executed += this.AboutCommandOnClick;

            Command exit = new Command { MenuText = "Exit" };
            exit.Executed += this.ExitCommandOnClick;

            Command clearLog = new Command { MenuText = "Clear log" };
            clearLog.Executed += this.ClearLogOnClick;

            this.Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem { Text = "Application", Items  = { new ButtonMenuItem(about), new ButtonMenuItem(exit) } },
                    new ButtonMenuItem { Text = "Configuration", Items  = { loadXml, saveXml } },
                    new ButtonMenuItem { Text = "Log", Items  = { new ButtonMenuItem(clearLog), this.LogExceptions } },
                }
            };

            Assembly assembly = Assembly.GetAssembly(typeof(MainForm));
            using (Stream stream = assembly.GetManifestResourceStream("Disig.TimeStampClient.Gui.TimeStampClient.ico"))
            {
                this.Icon = new Icon(stream);
            }

            this.Title = string.Format("TimeStampClient {0}", SharedUtils.AppVersion);
            this.MinimumSize = new Size(700, 500);
            this.BringToFront();
        }

        private void TextAreaLog(string msg)
        {
            this.AppendLog(msg);
        }

        private void RequestTimeStamp()
        {
            try
            {
                X509Certificate2 sslCert = null;
                string password = null;

                if (!string.IsNullOrEmpty(this.UserCertPassword.Text))
                {
                    password = this.UserCertPassword.Text;
                }

                if (!string.IsNullOrEmpty(this.UserCert.Text))
                {
                    sslCert = new X509Certificate2(this.UserCert.Text, password);
                }

                NetworkCredential networkCredential = null;
                if (!string.IsNullOrEmpty(this.UserPassword.Text) || !string.IsNullOrEmpty(this.UserName.Text))
                {
                    networkCredential = new NetworkCredential(this.UserName.Text, this.UserPassword.Text);
                }

                UserCredentials credentials = null;
                if (networkCredential != null || sslCert != null)
                {
                    credentials = new UserCredentials(sslCert, networkCredential);
                }

                TimeStampToken token = SharedUtils.RequestTimeStamp(
                    this.ServerAddress.Text,
                    this.SourceFile.Text,
                    ((ComboItemHash)this.HashAlgorithm.SelectedValue).Value,
                    this.RequestedPolicy.Text,
                    this.Nonce.Text,
                    ((ComboItemCertReq)this.TSACertificate.SelectedValue).Value,
                    credentials,
                    new LogDelegate(this.TextAreaLog),
                    this.LogExceptions.Checked);

                SharedUtils.SaveInFormat(this.SourceFile.Text, token, ((ComboItemFormat)this.ResponseFormat.SelectedValue).Format, this.OutFile.Text);
                this.TextAreaLog(string.Format("Response saved in {0}", this.OutFile.Text));
                MessageBox.Show(string.Format("Time-stamp successfully received."), "Info", MessageBoxType.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}: {1}", e.GetType(), e.Message), "Error", MessageBoxType.Error);
            }
        }

        private void RequestTimeStampButtonOnClick(object sender, EventArgs e)
        {
            this.RequestTimeStamp();
        }

        private void InitControls()
        {
            this.SourceFileLabel = new Label { Text = "File to time-stamp" };
            this.SourceFile = new TextBox();
            this.SourceFile.TextChanged += this.SourceFileOnTextChanged;

            this.LogExceptions = new CheckMenuItem();
            this.LogExceptions.Text = "Log exceptions";

            this.SourceFileButton = new Button();
            this.SourceFileButton.Text = @"Browse";
            this.SourceFileButton.Click += this.SourceFileButtonOnClick;

            this.SourceFileButton = new Button();
            this.SourceFileButton.Text = @"Browse";
            this.SourceFileButton.Click += this.SourceFileButtonOnClick;

            this.OutFileLabel = new Label { Text = "Output file" };
            this.OutFile = new TextBox();

            this.OutFileButton = new Button();
            this.OutFileButton.Text = @"Browse";
            this.OutFileButton.Click += this.OutFileButtonOnClick;

            this.ServerAddressLabel = new Label { Text = "TSA service URL" };
            this.ServerAddress = new TextBox();

            this.HashAlgorithmLabel = new Label { Text = "Hash algorithm" };
            this.HashAlgorithm = new DropDown();
            this.HashAlgorithm.Items.Add(new ComboItemHash("SHA-1", Oid.SHA1));
            this.HashAlgorithm.Items.Add(new ComboItemHash("SHA-256", Oid.SHA256));
            this.HashAlgorithm.Items.Add(new ComboItemHash("SHA-512", Oid.SHA512));
            this.HashAlgorithm.Items.Add(new ComboItemHash("MD5", Oid.MD5));
            this.HashAlgorithm.SelectedKey = "SHA-256";

            this.ResponseFormatLabel = new Label { Text = "Output format" };
            this.ResponseFormat = new DropDown();
            this.ResponseFormat.Items.Add(new ComboItemFormat("Raw time-stamp token", SharedUtils.ResultFormat.RawTST));
            this.ResponseFormat.Items.Add(new ComboItemFormat("ASiC-S TST (ZIP file with source file and time-stamp token)", SharedUtils.ResultFormat.ASIC_S));
            this.ResponseFormat.SelectedKey = SharedUtils.ResultFormat.RawTST.ToString();
            this.ResponseFormat.SelectedValueChanged += this.ResponseFormatOnValueChanged;

            this.RequestedPolicyLabel = new Label { Text = "Requested policy" };
            this.RequestedPolicy = new TextBox();

            this.NonceLabel = new Label { Text = "Nonce (hex)" };
            this.Nonce = new TextBox();

            this.TSACertificateLabel = new Label { Text = "Request TSA certificate" };
            this.TSACertificate = new DropDown();

            ComboItemCertReq yes = new ComboItemCertReq(true);
            ComboItemCertReq no = new ComboItemCertReq(false);
            this.TSACertificate.Items.Add(no);
            this.TSACertificate.Items.Add(yes);
            this.TSACertificate.SelectedKey = yes.Key;

            this.RequestTimeStampButton = new Button();
            this.RequestTimeStampButton.Text = @"Request time-stamp";
            this.RequestTimeStampButton.Click += this.RequestTimeStampButtonOnClick;

            this.GenerateNonce = new Button();
            this.GenerateNonce.Text = @"Generate";
            this.GenerateNonce.Click += this.GenerateNonceBytesOnClick;

            this.LogAreaLabel = new Label { Text = @"Activity log" };
            this.LogArea = new TextArea();
            this.LogArea.ReadOnly = true;

            this.UserCertLabel = new Label { Text = @"PKCS#12 file with SSL client certificate" };
            this.UserCert = new TextBox();

            this.UserCertButton = new Button();
            this.UserCertButton.Text = @"Browse";
            this.UserCertButton.Click += this.UserCertButtonOnClick;

            this.UserCertPasswordLabel = new Label { Text = @"PKCS#12 file password" };
            this.UserCertPassword = new PasswordBox();

            this.UserNameLabel = new Label { Text = @"HTTP user name" };
            this.UserName = new TextBox();

            this.UserPasswordLabel = new Label { Text = @"HTTP password" };
            this.UserPassword = new PasswordBox();
        }

        private Layout CreateMainContent()
        {
            TabControl tabControl = new TabControl();
            TabPage basicPage = new TabPage();
            TabPage advancedPage = new TabPage();
            TabPage authPage = new TabPage();

            basicPage.Text = "TSA service";
            basicPage.Content = new TableLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.ServerAddressLabel),
                                        new TableRow(new TableCell(this.ServerAddress, true), string.Empty)
                                    },  
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.SourceFileLabel),
                                        new TableRow(new TableCell(this.SourceFile, true), this.SourceFileButton),
                                        new TableRow { ScaleHeight = true }
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.ResponseFormatLabel),
                                        new TableRow(new TableCell(this.ResponseFormat, true)),
                                        new TableRow { ScaleHeight = true }
                                    },
                            }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.OutFileLabel),
                                        new TableRow(new TableCell(this.OutFile, true), this.OutFileButton),
                                        new TableRow { ScaleHeight = true }
                                    },
                                }, true),
                        }
                    },
                }
            };

            advancedPage.Text = "Advanced";
            advancedPage.Content = new TableLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.HashAlgorithmLabel),
                                        new TableRow(new TableCell(this.HashAlgorithm, true), string.Empty)
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.RequestedPolicyLabel),
                                        new TableRow(new TableCell(this.RequestedPolicy, true), string.Empty)
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.NonceLabel),
                                        new TableRow(new TableCell(this.Nonce, true), this.GenerateNonce)
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.TSACertificateLabel),
                                        new TableRow(new TableCell(this.TSACertificate, true), string.Empty)
                                    },
                                }, true),
                        }
                    },
                },
            };

            authPage.Text = "Authentication";
            authPage.Content = new TableLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.UserNameLabel),
                                        new TableRow(new TableCell(this.UserName, true), string.Empty)
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.UserPasswordLabel),
                                        new TableRow(new TableCell(this.UserPassword, true), string.Empty)
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.UserCertLabel),
                                        new TableRow(new TableCell(this.UserCert, true), this.UserCertButton)
                                    },
                                }, true),
                        }
                    },

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.UserCertPasswordLabel),
                                        new TableRow(new TableCell(this.UserCertPassword, true), string.Empty)
                                    }
                                }, true),
                        },
                    },
                },
            };

            tabControl.Pages.Add(basicPage);
            tabControl.Pages.Add(advancedPage);
            tabControl.Pages.Add(authPage);

            return new TableLayout
            {
                Padding = 10,
                Spacing = new Size(5, 5),
                Rows =
                {
                    tabControl,

                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                            new TableRow(new TableCell(this.RequestTimeStampButton, true))
                                    },
                                }, true)
                        }
                    },
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(
                                new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow(this.LogAreaLabel),
                                        new TableRow(new TableCell(this.LogArea, true))
                                    },
                                }, true),
                        },
                    },
                },
            };
        }

        private void UserCertButtonOnClick(object sender, EventArgs e)
        {
            string fileName = this.OpenFileDialog("Select SSL certificate");
            if (null != fileName)
            {
                this.UserCert.Text = fileName;
            }
        }

        private void SourceFileButtonOnClick(object sender, EventArgs e)
        {
            string fileName = this.OpenFileDialog("Select file to time-stamp");
            if (null != fileName)
            {
                this.SourceFile.Text = fileName;
            }
        }

        private void OutFileButtonOnClick(object sender, EventArgs e)
        {
            string fileName;
            if (((ComboItemFormat)this.ResponseFormat.SelectedValue).Format == SharedUtils.ResultFormat.ASIC_S)
            {
                fileName = this.SaveFileDialog("Select file to save time-stamp token", new FileFilter[] { new FileFilter("ASiC-S (*.asics)", "*.asics") }, false);
            }
            else
            {
                fileName = this.SaveFileDialog("Select file to save time-stamp token", new FileFilter[] { new FileFilter("Raw time-stamp tokens (*.tst)", "*.tst") }, false);
            }

            if (null != fileName)
            {
                this.OutFile.Text = fileName;
            }
        }

        private string OpenFileDialog(string title, FileFilter[] filters = null, bool mustExist = true)
        {
            using (FileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = mustExist;
                dialog.Title = title;
                if (filters != null)
                {
                    foreach (FileFilter filter in filters)
                    {
                        dialog.Filters.Add(filter);
                    }
                }

                dialog.Filters.Add(new FileFilter("All files (*.*)", "*.*"));
                if (DialogResult.Ok == dialog.ShowDialog(this))
                {
                    return dialog.FileName;
                }

                return null;
            }
        }

        private string SaveFileDialog(string title, FileFilter[] filters = null, bool mustExist = false)
        {
            using (FileDialog dialog = new SaveFileDialog())
            {
                dialog.CheckFileExists = mustExist;
                dialog.Title = title;
                if (filters != null)
                {
                    foreach (FileFilter filter in filters)
                    {
                        dialog.Filters.Add(filter);
                    }
                }

                dialog.Filters.Add(new FileFilter("All files (*.*)", "*.*"));
                if (DialogResult.Ok == dialog.ShowDialog(this))
                {
                    return dialog.FileName;
                }

                return null;
            }
        }

        private void GenerateNonceBytesOnClick(object sender, EventArgs e)
        {
            this.Nonce.Text = SharedUtils.BytesToHexString(SharedUtils.GenerateNonceBytes());
        }

        private void SetOutFile()
        {
            this.OutFile.Text = string.Format("{0}", this.SourceFile.Text);
            this.SetOutFileExtension();
        }

        private void SourceFileOnTextChanged(object sender, EventArgs e)
        {
            this.SetOutFile();
        }

        private void SetOutFileExtension()
        {
            string outFileExt;
            string fileName;
            string extension;

            if (string.IsNullOrEmpty(this.OutFile.Text))
            {
                return;
            }

            if (((ComboItemFormat)this.ResponseFormat.SelectedValue).Format == SharedUtils.ResultFormat.ASIC_S)
            {
                outFileExt = "asics";
            }
            else
            {
                outFileExt = "tst";
            }

            extension = Path.GetExtension(this.OutFile.Text);
            if (extension == ".asics" || extension == ".tst")
            {
                fileName = Path.ChangeExtension(this.OutFile.Text, outFileExt);
            }
            else
            {
                fileName = string.Format("{0}.{1}", this.OutFile.Text, outFileExt);
            }

            this.OutFile.Text = string.Format("{0}", fileName);
        }

        private void ResponseFormatOnValueChanged(object sender, EventArgs e)
        {
            this.SetOutFileExtension();
        }

        private void AppendLog(string text)
        {
            this.LogArea.Append(string.Format("{0}{1}", text, Environment.NewLine), true);
        }

        private static ClientSettings LoadSettings(string fileName)
        {
            ClientSettings settings = new ClientSettings();
            using (System.IO.FileStream inStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
            {
                settings = XmlSerializer.Deserialize<ClientSettings>(inStream);
            }

            return settings;
        }

        private void SetFormValues(ClientSettings settings)
        {
            this.TSACertificate.SelectedKey = settings.CertReq.ToString();
            this.HashAlgorithm.SelectedKey = settings.HashAlgorithm;
            this.Nonce.Text = settings.Nonce;
            this.RequestedPolicy.Text = settings.Policy;
            this.ServerAddress.Text = settings.ServerAddress;
            this.SourceFile.Text = settings.SourceFile;
            this.UserCert.Text = settings.UserCert;
            this.UserCertPassword.Text = settings.UserCertPassword;
            this.UserName.Text = settings.UserName;
            this.UserPassword.Text = settings.UserPassword;
            this.ResponseFormat.SelectedKey = settings.Format.ToString();
            this.OutFile.Text = settings.OutFile;
        }

        private void MenuConfigurationLoadOnclick(object sender, EventArgs e)
        {
            try
            {
                string fileName = this.OpenFileDialog("Select configuration", new FileFilter[] { new FileFilter("Configuration files (*.cfg)", "*.cfg") }, true);
                if (null == fileName)
                {
                    return;
                }

                ClientSettings settings = LoadSettings(fileName);
                if (null == settings)
                {
                    return;
                }

                this.SetFormValues(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxType.Error);
            }
        }

        private static void SaveSettings(ClientSettings settings, string fileName)
        {
            using (System.IO.FileStream outStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                XmlSerializer.Serialize(settings, outStream);
            }
        }

        private static bool SaveSensitiveData()
        {
            DialogResult result = MessageBox.Show("Your passwords will be saved in a plain text.\nDo you want to save your passwords in a configuration file?", "Warning", MessageBoxButtons.YesNo, MessageBoxType.Warning, MessageBoxDefaultButton.No);
            return (result == DialogResult.Yes);
        }

        private ClientSettings GetFormValues()
        {
            ClientSettings settings = new ClientSettings();

            settings.CertReq = ((ComboItemCertReq)this.TSACertificate.SelectedValue).Value;
            settings.HashAlgorithm = ((ComboItemHash)this.HashAlgorithm.SelectedValue).Text;
            settings.Nonce = this.Nonce.Text;
            settings.Policy = this.RequestedPolicy.Text;
            settings.ServerAddress = this.ServerAddress.Text;
            settings.SourceFile = this.SourceFile.Text;
            settings.UserCert = this.UserCert.Text;
            settings.UserName = this.UserName.Text;
            settings.Format = ((ComboItemFormat)this.ResponseFormat.SelectedValue).Format;
            settings.OutFile = this.OutFile.Text;

            if (!string.IsNullOrEmpty(this.UserCertPassword.Text) || !string.IsNullOrEmpty(this.UserPassword.Text))
            {
                if (SaveSensitiveData())
                {
                    settings.UserCertPassword = this.UserCertPassword.Text;
                    settings.UserPassword = this.UserPassword.Text;
                }
            }

            return settings;
        }

        private void MenuConfigurationSaveOnclick(object sender, EventArgs e)
        {
            try
            {
                string fileName = this.SaveFileDialog("Select file to save configuration", new FileFilter[] { new FileFilter("Configuration files (*.cfg)", "*.cfg") }, false);
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                ClientSettings settings = this.GetFormValues();
                SaveSettings(settings, fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, MessageBoxType.Error);
            }
        }

        private void TerminateApplication()
        {
            this.Close();

            if (SharedUtils.RunningOnMacOs())
                Environment.Exit(0);
        }

        private void ExitCommandOnClick(object sender, EventArgs e)
        {
            this.TerminateApplication();
        }

        private void ShowAboutWindow()
        {
            using (AboutDialog about = new AboutDialog())
            {
                about.ShowModal(this);
            }
        }

        private void AboutCommandOnClick(object sender, EventArgs e)
        {
            this.ShowAboutWindow();
        }

        private void ClearLogArea()
        {
            this.LogArea.Text = string.Empty;
        }

        private void ClearLogOnClick(object sender, EventArgs e)
        {
            this.ClearLogArea();
        }
    }
}