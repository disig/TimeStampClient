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

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Disig.TimeStampClient.Gui
{
    /// <summary>
    /// Simple modal message box (Avalonia does not provide one)
    /// </summary>
    internal static class MessageBox
    {
        public static Task Show(Window owner, string text, string title)
        {
            return ShowDialog(owner, text, title, false);
        }

        /// <summary>
        /// Shows Yes/No dialog with "No" as the default button
        /// </summary>
        /// <returns>True if user clicked "Yes"</returns>
        public static Task<bool> ShowYesNo(Window owner, string text, string title)
        {
            return ShowDialog(owner, text, title, true);
        }

        private static async Task<bool> ShowDialog(Window owner, string text, string title, bool yesNo)
        {
            bool result = false;

            Window dialog = new Window
            {
                Title = title,
                Icon = owner.Icon,
                SizeToContent = SizeToContent.WidthAndHeight,
                MinWidth = 300,
                MaxWidth = 500,
                CanResize = false,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 5
            };

            Button defaultButton;
            if (yesNo)
            {
                Button yesButton = new Button { Content = "Yes" };
                yesButton.Click += (sender, e) => { result = true; dialog.Close(); };

                Button noButton = new Button { Content = "No", IsDefault = true, IsCancel = true };
                noButton.Click += (sender, e) => dialog.Close();

                buttons.Children.Add(yesButton);
                buttons.Children.Add(noButton);
                defaultButton = noButton;
            }
            else
            {
                Button okButton = new Button { Content = "OK", IsDefault = true, IsCancel = true };
                okButton.Click += (sender, e) => dialog.Close();

                buttons.Children.Add(okButton);
                defaultButton = okButton;
            }

            StackPanel content = new StackPanel { Margin = new Thickness(15), Spacing = 15 };
            content.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
            content.Children.Add(buttons);
            dialog.Content = content;
            dialog.Opened += (sender, e) => defaultButton.Focus();

            await dialog.ShowDialog(owner);
            return result;
        }
    }
}
