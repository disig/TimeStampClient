/*
*  Copyright 2016-2026 Disig a.s.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace Disig.TimeStampClient.Gui
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.InitControls();
            this.Title = string.Format("TimeStampClient {0}", SharedUtils.AppVersion);
        }

        private ComboItemFormat SelectedFormat
        {
            get { return (ComboItemFormat)this.ResponseFormat.SelectedItem; }
        }

        private void InitControls()
        {
            this.HashAlgorithm.ItemsSource = new ComboItemHash[]
            {
                new ComboItemHash("SHA-1", Oid.SHA1),
                new ComboItemHash("SHA-256", Oid.SHA256),
                new ComboItemHash("SHA-384", Oid.SHA384),
                new ComboItemHash("SHA-512", Oid.SHA512),
                new ComboItemHash("MD5", Oid.MD5),
            };
            SelectByKey(this.HashAlgorithm, "SHA-256");

            this.ResponseFormat.ItemsSource = new ComboItemFormat[]
            {
                new ComboItemFormat("Raw time-stamp token", SharedUtils.ResultFormat.RawTST),
                new ComboItemFormat("ASiC-S TST (ZIP file with source file and time-stamp token)", SharedUtils.ResultFormat.ASIC_S),
            };
            SelectByKey(this.ResponseFormat, SharedUtils.ResultFormat.RawTST.ToString());

            ComboItemCertReq yes = new ComboItemCertReq(true);
            ComboItemCertReq no = new ComboItemCertReq(false);
            this.TSACertificate.ItemsSource = new ComboItemCertReq[] { no, yes };
            SelectByKey(this.TSACertificate, yes.Key);
        }

        private static void SelectByKey(ComboBox comboBox, string key)
        {
            foreach (object item in comboBox.ItemsSource)
            {
                if (item is IComboItem comboItem && comboItem.Key == key)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
        }

        private void TextAreaLog(string msg)
        {
            // Logger is invoked from background thread during time-stamp request
            Dispatcher.UIThread.Post(() => this.AppendLog(msg));
        }

        private async Task RequestTimeStamp()
        {
            try
            {
                this.RequestTimeStampButton.IsEnabled = false;

                X509Certificate2 sslCert = null;
                string password = null;

                if (!string.IsNullOrEmpty(this.UserCertPassword.Text))
                {
                    password = this.UserCertPassword.Text;
                }

                if (!string.IsNullOrEmpty(this.UserCert.Text))
                {
                    sslCert = X509CertificateLoader.LoadPkcs12FromFile(this.UserCert.Text, password);
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

                string serverAddress = this.ServerAddress.Text;
                string sourceFile = this.SourceFile.Text;
                Oid hashAlgorithm = ((ComboItemHash)this.HashAlgorithm.SelectedItem).Value;
                string policy = this.RequestedPolicy.Text;
                string nonce = this.Nonce.Text;
                bool certReq = ((ComboItemCertReq)this.TSACertificate.SelectedItem).Value;
                bool logExceptions = this.LogExceptions.IsChecked;
                SharedUtils.ResultFormat format = this.SelectedFormat.Format;
                string outFile = this.OutFile.Text;

                await Task.Run(() =>
                {
                    TimeStampToken token = SharedUtils.RequestTimeStamp(
                        serverAddress,
                        sourceFile,
                        hashAlgorithm,
                        policy,
                        nonce,
                        certReq,
                        credentials,
                        new LogDelegate(this.TextAreaLog),
                        logExceptions);

                    SharedUtils.SaveInFormat(sourceFile, token, format, outFile);
                });

                this.TextAreaLog(string.Format("Response saved in {0}", outFile));
                await MessageBox.Show(this, "Time-stamp successfully received.", "Info");
            }
            catch (Exception e)
            {
                await MessageBox.Show(this, string.Format("{0}: {1}", e.GetType(), e.Message), "Error");
            }
            finally
            {
                this.RequestTimeStampButton.IsEnabled = true;
            }
        }

        private async void RequestTimeStampButtonOnClick(object sender, RoutedEventArgs e)
        {
            await this.RequestTimeStamp();
        }

        private async void UserCertButtonOnClick(object sender, RoutedEventArgs e)
        {
            string fileName = await this.OpenFileDialog("Select SSL certificate");
            if (null != fileName)
            {
                this.UserCert.Text = fileName;
            }
        }

        private async void SourceFileButtonOnClick(object sender, RoutedEventArgs e)
        {
            string fileName = await this.OpenFileDialog("Select file to time-stamp");
            if (null != fileName)
            {
                this.SourceFile.Text = fileName;
            }
        }

        private async void OutFileButtonOnClick(object sender, RoutedEventArgs e)
        {
            string fileName;
            if (this.SelectedFormat.Format == SharedUtils.ResultFormat.ASIC_S)
            {
                fileName = await this.SaveFileDialog("Select file to save time-stamp token", new FilePickerFileType[] { CreateFileFilter("ASiC-S (*.asics)", "*.asics") });
            }
            else
            {
                fileName = await this.SaveFileDialog("Select file to save time-stamp token", new FilePickerFileType[] { CreateFileFilter("Raw time-stamp tokens (*.tst)", "*.tst") });
            }

            if (null != fileName)
            {
                this.OutFile.Text = fileName;
            }
        }

        private static FilePickerFileType CreateFileFilter(string name, string pattern)
        {
            return new FilePickerFileType(name) { Patterns = new string[] { pattern } };
        }

        private static List<FilePickerFileType> CreateFileFilters(FilePickerFileType[] filters)
        {
            List<FilePickerFileType> fileFilters = new List<FilePickerFileType>();
            if (filters != null)
            {
                fileFilters.AddRange(filters);
            }

            fileFilters.Add(CreateFileFilter("All files (*.*)", "*"));
            return fileFilters;
        }

        private async Task<string> OpenFileDialog(string title, FilePickerFileType[] filters = null)
        {
            IReadOnlyList<IStorageFile> files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = CreateFileFilters(filters)
            });

            if (files.Count == 0)
            {
                return null;
            }

            return files[0].TryGetLocalPath();
        }

        private async Task<string> SaveFileDialog(string title, FilePickerFileType[] filters = null)
        {
            IStorageFile file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                FileTypeChoices = CreateFileFilters(filters),
                ShowOverwritePrompt = true
            });

            return file?.TryGetLocalPath();
        }

        private void GenerateNonceBytesOnClick(object sender, RoutedEventArgs e)
        {
            this.Nonce.Text = SharedUtils.BytesToHexString(SharedUtils.GenerateNonceBytes());
        }

        private void SetOutFile()
        {
            this.OutFile.Text = string.Format("{0}", this.SourceFile.Text);
            this.SetOutFileExtension();
        }

        private void SourceFileOnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetOutFile();
        }

        private void SetOutFileExtension()
        {
            string outFileExt;
            string fileName;
            string extension;

            if (string.IsNullOrEmpty(this.OutFile.Text) || this.SelectedFormat == null)
            {
                return;
            }

            if (this.SelectedFormat.Format == SharedUtils.ResultFormat.ASIC_S)
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

        private void ResponseFormatOnValueChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetOutFileExtension();
        }

        private void AppendLog(string text)
        {
            this.LogArea.Text += string.Format("{0}{1}", text, Environment.NewLine);
            this.LogArea.CaretIndex = this.LogArea.Text.Length;
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
            SelectByKey(this.TSACertificate, settings.CertReq.ToString());
            SelectByKey(this.HashAlgorithm, settings.HashAlgorithm);
            this.Nonce.Text = settings.Nonce;
            this.RequestedPolicy.Text = settings.Policy;
            this.ServerAddress.Text = settings.ServerAddress;
            this.SourceFile.Text = settings.SourceFile;
            this.UserCert.Text = settings.UserCert;
            this.UserCertPassword.Text = settings.UserCertPassword;
            this.UserName.Text = settings.UserName;
            this.UserPassword.Text = settings.UserPassword;
            SelectByKey(this.ResponseFormat, settings.Format.ToString());
            this.OutFile.Text = settings.OutFile;
        }

        private async void MenuConfigurationLoadOnclick(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = await this.OpenFileDialog("Select configuration", new FilePickerFileType[] { CreateFileFilter("Configuration files (*.cfg)", "*.cfg") });
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
                await MessageBox.Show(this, ex.Message, "Error");
            }
        }

        private static void SaveSettings(ClientSettings settings, string fileName)
        {
            using (System.IO.FileStream outStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                XmlSerializer.Serialize(settings, outStream);
            }
        }

        private Task<bool> SaveSensitiveData()
        {
            return MessageBox.ShowYesNo(this, "Your passwords will be saved in a plain text.\nDo you want to save your passwords in a configuration file?", "Warning");
        }

        private async Task<ClientSettings> GetFormValues()
        {
            ClientSettings settings = new ClientSettings();

            settings.CertReq = ((ComboItemCertReq)this.TSACertificate.SelectedItem).Value;
            settings.HashAlgorithm = ((ComboItemHash)this.HashAlgorithm.SelectedItem).Text;
            settings.Nonce = this.Nonce.Text;
            settings.Policy = this.RequestedPolicy.Text;
            settings.ServerAddress = this.ServerAddress.Text;
            settings.SourceFile = this.SourceFile.Text;
            settings.UserCert = this.UserCert.Text;
            settings.UserName = this.UserName.Text;
            settings.Format = this.SelectedFormat.Format;
            settings.OutFile = this.OutFile.Text;

            if (!string.IsNullOrEmpty(this.UserCertPassword.Text) || !string.IsNullOrEmpty(this.UserPassword.Text))
            {
                if (await this.SaveSensitiveData())
                {
                    settings.UserCertPassword = this.UserCertPassword.Text;
                    settings.UserPassword = this.UserPassword.Text;
                }
            }

            return settings;
        }

        private async void MenuConfigurationSaveOnclick(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = await this.SaveFileDialog("Select file to save configuration", new FilePickerFileType[] { CreateFileFilter("Configuration files (*.cfg)", "*.cfg") });
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                ClientSettings settings = await this.GetFormValues();
                SaveSettings(settings, fileName);
            }
            catch (Exception ex)
            {
                await MessageBox.Show(this, ex.Message, "Error");
            }
        }

        private void ExitCommandOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void AboutCommandOnClick(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            await about.ShowDialog(this);
        }

        private void ClearLogOnClick(object sender, RoutedEventArgs e)
        {
            this.LogArea.Text = string.Empty;
        }
    }
}
