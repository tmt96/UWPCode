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
using UWPCode.Services.SettingsServices;
using Windows.UI.Xaml.Media;

namespace UWPCode.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        SettingsService settings;
        internal BufferOrganizer BufferOrganizer => ((App)Application.Current).BufferOrganizer;

        public MainPageViewModel()
        {
            settings = SettingsService.Instance;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        public TextWrapping WordWrap => settings.WordWrap;

        public int FontSize => settings.EditorFontSize;

        public FontFamily FontFamily => new FontFamily(settings.EditorFontFamily);

        public int TabSize => settings.TabSize;

        public bool UseSoftTab => settings.UseSoftTab;

        public string DocumentName => BufferOrganizer.CurrentBuffer.Name;

        public List<string> BufferNameList
        {
            get
            {
                return BufferOrganizer.BufferDictionary.Keys.ToList<string>();
            }
        }

        internal void SetCurrentBufferUnsaved()
        {
            BufferOrganizer.CurrentBuffer.IsSaved = false;
        }

        public string SelectedBufferName
        {
            get; set;
        }

        internal void SwitchCurrentBuffer(string key)
        {
            var buffer = BufferOrganizer.SwitchCurrentBuffer(key);
            if (buffer != null)
                SelectedBufferName = key;
            RaisePropertyChanged(SelectedBufferName);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            SettingsChanged(ApplicationData.Current, parameter);
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            Windows.Storage.ApplicationData.Current.DataChanged -= SettingsChanged;
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            Windows.Storage.ApplicationData.Current.DataChanged -= SettingsChanged;
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        private void SettingsChanged(Windows.Storage.ApplicationData sender, object args)
        {
            RaisePropertyChanged();
        }

        internal async Task<StorageFile> UpdateAndSaveBuffer(Models.Buffer buffer, string text)
        {
            UpdateBuffer(buffer, text);
            StorageFile file;
            if (!buffer.IsInFileSystem)
            {
                file = await PickSaveFileAsync(buffer);
                file = await BufferOrganizer.SaveBufferToFile(buffer, file);
            }

            file = await BufferOrganizer.SaveBufferToFile(buffer);
            RaisePropertyChanged();
            return file;
        }

        private async Task<StorageFile> PickSaveFileAsync(Models.Buffer buffer)
        {
            var fileSavePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            fileSavePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            fileSavePicker.SuggestedFileName = buffer.Name;

            // TODO: actually handle the case in which the file saving fail
            var file = await fileSavePicker.PickSaveFileAsync();
            if (file == null)
            {
                var dialog = new MessageDialog("Cannot save file. Please retry!!");
                await dialog.ShowAsync();
            }
            return file;
        }

        private void UpdateBuffer(Models.Buffer buffer, string text) => buffer.Text = text;

        public async Task<Models.Buffer> ChooseAndOpenFile()
        {
            var file = await PickOpenFileAsync();
            var buffer = await BufferOrganizer.CreateBufferFromFile(file);
            RaisePropertyChanged();
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

            // TODO: Actually handle the case in which we cannot open file
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                return file;
            }
            else
            {
                var dialog = new MessageDialog("File doesn't exist");
                await dialog.ShowAsync();
                return null;
            }
        }

        public Models.Buffer CreateNewBuffer()
        {
            var buffer = BufferOrganizer.CreateBlankBuffer();
            RaisePropertyChanged();
            return buffer;
        }

        internal void UpdateOldBuffer(string editorText)
        {
            UpdateBuffer(BufferOrganizer.CurrentBuffer, editorText);
        }
    }
}

