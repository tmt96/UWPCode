using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using UWPCode.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace UWPCode.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }


        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), Value);


        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);


        internal async Task<StorageFile> UpdateAndSaveBuffer(Models.Buffer buffer, string text)
        {
            UpdateBuffer(buffer, text);
            if (!buffer.IsInFileSystem)
                buffer.File = await PickSaveFileAsync(buffer);
            return await buffer.SaveFile();
        }

        private async Task<StorageFile> PickSaveFileAsync(Models.Buffer buffer)
        {
            var fileSavePicker = new Windows.Storage.Pickers.FileSavePicker();
            fileSavePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            fileSavePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            fileSavePicker.SuggestedFileName = buffer.Name;

            // TODO: actually handle the case in which the file saving fail
            var file = await fileSavePicker.PickSaveFileAsync();
            if (file == null)
            {
                MessageDialog dialog = new MessageDialog("Cannot save file. Please retry!!");
                await dialog.ShowAsync();
            }
            return file;
        }

        private void UpdateBuffer(Models.Buffer buffer, string text)
        {
            buffer.Text = text;
        }

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public async Task<Models.Buffer> ChooseAndOpenFile()
        {
            var file = await PickOpenFileAsync();
            var buffer = await ((App)Application.Current).BufferOrganizer.CreateBufferFromFile(file);
            return buffer;
        }

        private async Task<StorageFile> PickOpenFileAsync()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            // TODO: Actually handle the casee in which we cannot open file
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                return file;
            }
            else
            {
                MessageDialog dialog = new MessageDialog("File doesn't exist");
                await dialog.ShowAsync();
                return null;
            }
        }

        public Models.Buffer CreateNewBuffer()
        {
            var buffer = ((App)Application.Current).BufferOrganizer.CreateBlankBuffer();
            return buffer;
        }

    }
}

