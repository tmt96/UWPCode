using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Windows.UI.Xaml;

namespace UWPCode.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        public bool WordWrap
        {
            get { return _settings.WordWrap.Equals(TextWrapping.Wrap); }
            set
            {
                _settings.WordWrap = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
                base.RaisePropertyChanged();
            }
        }

        public int EditorFontSize
        {
            get { return _settings.EditorFontSize; }
            set
            {
                _settings.EditorFontSize = value;
                base.RaisePropertyChanged();
            }
        }

        public List<string> FontsList
        {
            get
            {
                var fontsList = new List<string>(Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies());
                fontsList.Sort();
                return fontsList;
            }
        }

        public string EditorFontFamily
        {
            get { return _settings.EditorFontFamily; }
            set
            {
                _settings.EditorFontFamily = value;
                base.RaisePropertyChanged();
            }

        }

        public int TabSize
        {
            get { return _settings.TabSize; }
            set
            {
                _settings.TabSize = value;
                base.RaisePropertyChanged();
            }
        }

        public bool UseSoftTab
        {
            get { return _settings.UseSoftTab; }
            set
            {
                _settings.UseSoftTab = value;
                base.RaisePropertyChanged();
            }
        }
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}

