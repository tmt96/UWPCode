using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace UWPCode.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public bool ShowHamburgerButton
        {
            get { return _helper.Read<bool>(nameof(ShowHamburgerButton), true); }
            set
            {
                _helper.Write(nameof(ShowHamburgerButton), value);
            }
        }

        public bool IsFullScreen
        {
            get { return _helper.Read<bool>(nameof(IsFullScreen), false); }
            set
            {
                _helper.Write(nameof(IsFullScreen), value);
            }
        }

        public TextWrapping WordWrap {
            get
            {
                var wrapMode = TextWrapping.Wrap;
                var value = _helper.Read<string>(nameof(WordWrap), wrapMode.ToString());
                return Enum.TryParse<TextWrapping>(value, out wrapMode) ? wrapMode : TextWrapping.NoWrap;
            }
            set
            {
                _helper.Write(nameof(WordWrap), value);
            }
        }

        public int TabSize {
            get
            {
                return _helper.Read<int>(nameof(TabSize), 4);
            }
            set
            {
                _helper.Write(nameof(TabSize), value);
            }
        }

        public int EditorFontSize {
            get {
                return _helper.Read<int>(nameof(EditorFontSize), 12);
            }
            set
            {
                _helper.Write(nameof(EditorFontSize), value);
            }
        }

        public bool UseSoftTab
        {
            get
            {
                return _helper.Read<bool>(nameof(UseSoftTab), true);
            }
            set
            {
                _helper.Write(nameof(UseSoftTab), value);
            }
        }

        public string EditorFontFamily
        {
            get
            {
                return _helper.Read<string>(nameof(EditorFontFamily), "Consolas");
            }
            set
            {
                _helper.Write(nameof(EditorFontFamily), value);
            }
        }
    }
}

