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
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Disig.TimeStampClient.Gui
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            this.InitializeComponent();
            this.FillLicenseTextArea();
            this.FillComponentsTextArea();
        }

        private void FillLicenseTextArea()
        {
            this.LicenseTextArea.Text = @"TimeStampClient " + SharedUtils.AppVersion + Environment.NewLine
                + @"Copyright (c) 2016-2026 Disig a.s." + Environment.NewLine
                + Environment.NewLine + "Licensed under the Apache License, Version 2.0 (the \"License\"); " +
                "you may not use this file except in compliance with the License. You may obtain a copy of the License at " +
                Environment.NewLine + Environment.NewLine + "https://www.apache.org/licenses/LICENSE-2.0" + Environment.NewLine + Environment.NewLine +
                "Unless required by applicable law or agreed to in writing, software distributed under the License is " +
                "distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. " +
                "See the License for the specific language governing permissions and limitations under the License." + Environment.NewLine;
        }

        private void FillComponentsTextArea()
        {
            this.ComponentsTextArea.Text = @"TimeStampClient uses following 3rd party components and their dependencies:" + Environment.NewLine
                + Environment.NewLine
                + "- Avalonia" + Environment.NewLine
                + "- BouncyCastle.Cryptography" + Environment.NewLine
                + "- SharpZipLib" + Environment.NewLine
                + Environment.NewLine
                + "Full license text for each of these components can be found in the installation directory.";
        }

        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void WebsiteButtonOnClick(object sender, RoutedEventArgs e)
        {
            await this.Launcher.LaunchUriAsync(new Uri("https://github.com/disig/TimeStampClient"));
        }
    }
}
