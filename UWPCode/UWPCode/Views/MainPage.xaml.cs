using System;
using UWPCode.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.Threading.Tasks;

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

        private Models.Buffer CreateAndDisplayNewFile()
        {
            var buffer = ViewModel.CreateNewBuffer();
            DisplayBuffer(buffer);
            return buffer;
        }

        private void DisplayBuffer(Models.Buffer buffer)
        {
            editor.Text = buffer.Text;
            pageHeader.Text = buffer.Name;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentBuffer();
        }

        private async Task<StorageFile> SaveCurrentBuffer()
        {
            var buffer = ((App)Application.Current).BufferOrganizer.CurrentBuffer;
            return await ViewModel.UpdateAndSaveBuffer(buffer, editor.Text);
        }
    }
}
