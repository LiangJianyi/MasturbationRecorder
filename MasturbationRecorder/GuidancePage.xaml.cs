using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MasturbationRecorder.SqlDbHelper;
using System.Threading.Tasks;

namespace MasturbationRecorder {
    using Debug = System.Diagnostics.Debug;

    public sealed partial class GuidancePage : Page {
        public GuidancePage() {
            this.InitializeComponent();
        }

        private async void Login_ClickAsync(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(AccountTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password)) {
                PopErrorDialogAsync("账户和密码不能为空");
            }
            else if (string.IsNullOrWhiteSpace(AccountTextBox.Text) ||
                     string.IsNullOrWhiteSpace(PasswordBox.Password)) {
                PopErrorDialogAsync("账户和密码不能包含空格");
            }
            else {
                if (await AzureSqlDbHelper.LoginAsync(new Configuration(AccountTextBox.Text, PasswordBox.Password))) {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage)); 
                }
                else {
                    PopErrorDialogAsync("账号或密码错误");
                }
            }
        }

        private static async void PopErrorDialogAsync(string content) {
            ContentDialog fileOpenFailDialog = new ContentDialog {
                Title = "Error",
                Content = content,
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await fileOpenFailDialog.ShowAsync();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            AccountTextBox.Width = TitleBox.ActualWidth;
            PasswordBox.Width = TitleBox.ActualWidth;
        }

        private void Register_Click(object sender, RoutedEventArgs e) {

        }

        private static void CommitUserNameAndPassword(string connectionString, string username, string password) {
            string queryString = $"select UserName,Password,PersonData from dbo.MasturbationRecorderUser " +
                                 $"where UserName='{username}' and Password='{password}'";

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader()) {
                    // Call Read before accessing data.
                    while (reader.Read()) {
                        Debug.WriteLine($"UserName: {reader[0]}, Password: {reader[1]}");
                    }
                }
            }
        }
    }
}
