using System;
using UWPCode.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace UWPCode.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        public void UpdateTextArea(string text)
        {
            editor.Text = text;
        }

        public async void OpenAndDisplayFile()
        {
            var buffer = await ViewModel.ChooseAndOpenFile();
            DisplayBuffer(buffer);
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenAndDisplayFile();
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            CreateAndDisplayNewFile();
        }

        private void CreateAndDisplayNewFile()
        {
            var buffer = ViewModel.CreateNewBuffer();
            DisplayBuffer(buffer);
        }

        private void DisplayBuffer(Models.Buffer buffer)
        {
            editor.Text = buffer.Text;
            pageHeader.Text = buffer.Name;
        }
    }
}
