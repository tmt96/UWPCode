using System;
using UWPCode.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI;
using Windows.System;

namespace UWPCode.Views
{
    public sealed partial class MainPage : Page
    {
        private List<int> foundPosList = new List<int>();
        Color activeSelectionColor = Colors.LightBlue;
        Color inactiveSelectionColor = Colors.LightGray;

        public MainPage()
        {
            InitializeComponent();
            CreateAndDisplayNewFile();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        /**************************************
         * Editor controls
         * Most controls, by design, call a function when come across an event.
         * This allows maximal distinction btw UI and functionality
         * ************************************/

        private string EditorText
        {
            get
            {
                string text;
                editor.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);
                return text;
            }
        }

        private void editor_TextChanged(object sender, RoutedEventArgs e) => ViewModel.SetCurrentBufferUnsaved();

        private void editor_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var richEditBox = sender as RichEditBox;
            if (richEditBox != null)
            {
                switch (e.Key)
                {
                    case VirtualKey.Tab:
                        EnterTabKey(richEditBox, e);
                        break;
                }
            }

        }

        /****************************
         *  Command bar buttons 
         ****************************/

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e) => await OpenAndDisplayFileAsync();

        private void AddFileButton_Click(object sender, RoutedEventArgs e) => CreateAndDisplayNewFile();

        private async void SaveButton_Click(object sender, RoutedEventArgs e) => await SaveCurrentBufferAsync();

        /************
         * menu bar button
         * **********/

        private void fileListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void splitViewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void functionListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void remoteFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sourceControlButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settingButton_Click(object sender, RoutedEventArgs e) => ViewModel.GotoSettings();

        /****************
         * Search bar controls
         * **************/

        /*** Searchbox open & close ***/
        private void searchBoxFlyout_Opening(object sender, object e)
        {
            Flyout flyout = sender as Flyout;
            Style fullWidthFlyoutStyle = new Style { TargetType = typeof(FlyoutPresenter) };
            fullWidthFlyoutStyle.Setters.Add(new Setter(MinWidthProperty, mainArea.ActualWidth));

            flyout.FlyoutPresenterStyle = fullWidthFlyoutStyle;
        }

        private void searchBoxFlyout_Closed(object sender, object e) => ClearHighlights();

        private void searchBoxFlyout_Opened(object sender, object e) => FindAndHighLightAllOccurrence(findBox.Text);

        /*** Find controls **/
        private void findBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) => FindAndHighLightAllOccurrence(sender.Text);

        private void findNextButton_Click(object sender, RoutedEventArgs e) => FindAndHighlightNextOccurrence(findBox.Text);

        private void findPrevButton_Click(object sender, RoutedEventArgs e) => FindAndHighlightPrevOccerrence(findBox.Text);

        /***Replace Control***/
        private void replaceBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void replaceNextButton_Click(object sender, RoutedEventArgs e) => ReplaceNextOccurence(findBox.Text, replaceBox.Text);

        private void replaceAllButton_Click(object sender, RoutedEventArgs e) => ReplaceAllOccurence(findBox.Text, replaceBox.Text);

        /*****************
         * Buffer List Controls
         * ******************/

        private void BufferListFlyout_Opening(object sender, object e)
        {
            var flyout = sender as Flyout;
            var fullHeightFlyoutStyle = new Style { TargetType = typeof(FlyoutPresenter) };
            fullHeightFlyoutStyle.Setters.Add(new Setter(MinHeightProperty, mainArea.ActualHeight));

            flyout.FlyoutPresenterStyle = fullHeightFlyoutStyle;
        }

        private void BufferListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SwitchToNewBuffer((sender as ListView).SelectedItem as string);
        }   
        
        /****************************
         * Helper functions
         * **************************/

        private void SwitchToNewBuffer(string newKey)
        {
            ViewModel.UpdateOldBuffer(EditorText);
            ViewModel.SwitchCurrentBuffer(newKey);
            DisplayBuffer(ViewModel.BufferOrganizer.CurrentBuffer);
        }

        private void DisplayBuffer(Models.Buffer buffer)
        {
            editor.Document.SetText(Windows.UI.Text.TextSetOptions.None, buffer.Text);
            pageHeader.Text = buffer.Name;
        }

        private async Task OpenAndDisplayFileAsync()
        {
            var buffer = await ViewModel.ChooseAndOpenFile();
            if (buffer != null) DisplayBuffer(buffer);
        }

        private void CreateAndDisplayNewFile()
        {
            var buffer = ViewModel.CreateNewBuffer();
            if (buffer != null) DisplayBuffer(buffer);
        }

        private async Task<StorageFile> SaveCurrentBufferAsync()
        {
            var buffer = ((App)Application.Current).BufferOrganizer.CurrentBuffer;
            var text = EditorText;
            var file = await ViewModel.UpdateAndSaveBuffer(buffer, text);
            DisplayBuffer(buffer);
            return file;
        }

        private void FindAndHighLightAllOccurrence(string text)
        {
            if (text.Length > 0)
            {
                var cursorPos = editor.Document.Selection.StartPosition;
                ClearHighlights();
                foundPosList = FindAllOccurence(text, 0);
                HighlightAll(inactiveSelectionColor, foundPosList, text.Length);
                editor.Document.Selection.SetRange(cursorPos, cursorPos);
            }
        }

        private void HighlightAll(Color highlightColor, List<int> foundIndex, int length)
        {
            foreach (var index in foundIndex)
            {
                editor.Document.Selection.SetRange(index, index + length);
                editor.Document.Selection.CharacterFormat.BackgroundColor = highlightColor;
            }
        }

        private void ClearHighlights()
        {
            var cursorPos = editor.Document.Selection.StartPosition;
            var text = EditorText;
            var end = text.Length;
            editor.Document.Selection.SetRange(0, end);
            editor.Document.Selection.CharacterFormat.BackgroundColor = (editor.Background as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            editor.Document.Selection.SetRange(cursorPos, cursorPos);
        }

        private void HighlightAndScrollTo(RichEditBox editor, int indexInFoundPosList, int length, Color highlightColor)
        {
            editor.Document.Selection.SetRange(indexInFoundPosList, indexInFoundPosList + length);
            editor.Document.Selection.CharacterFormat.BackgroundColor = highlightColor;
            editor.Document.Selection.ScrollIntoView(Windows.UI.Text.PointOptions.None);
        }

        private List<int> FindAllOccurence(string text, int start)
        {
            var found = new List<int>();
            if (text.Length <= 0) return found;

            string editorText = EditorText;
            int pos = editorText.IndexOf(text);
            while (pos > -1)
            {
                found.Add(pos);
                pos = editorText.IndexOf(text, pos + text.Length);
            }
            return found;
        }

        private int FindAndHighlightNextOccurrence(string text)
        {
            if (foundPosList.Contains(editor.Document.Selection.StartPosition))
            {
                editor.Document.Selection.CharacterFormat.BackgroundColor = inactiveSelectionColor;
            }
            else
            {
                editor.Document.Selection.CharacterFormat.BackgroundColor = (editor.Background as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            }

            if (foundPosList == null || foundPosList.Count == 0) return -1;

            var selectionEnd = editor.Document.Selection.EndPosition;
            var nextOccurrenceIndex = foundPosList.BinarySearch(selectionEnd);
            if (nextOccurrenceIndex < 0) nextOccurrenceIndex = ~ nextOccurrenceIndex;
            if (nextOccurrenceIndex >= foundPosList.Count) nextOccurrenceIndex = 0;

            HighlightAndScrollTo(editor, foundPosList[nextOccurrenceIndex], text.Length, activeSelectionColor);
            return foundPosList[nextOccurrenceIndex];

        }

        private int FindAndHighlightPrevOccerrence(string text)
        {
            ClearHighlights();
            HighlightAll(inactiveSelectionColor, foundPosList, text.Length);

            if (foundPosList == null || foundPosList.Count == 0) return -1;

            var selectionStart = editor.Document.Selection.StartPosition;
            var prevOccurrenceIndex = foundPosList.BinarySearch(selectionStart);
            if (prevOccurrenceIndex < 0) prevOccurrenceIndex = ~prevOccurrenceIndex;
            prevOccurrenceIndex -= 1;
            if (prevOccurrenceIndex < 0) prevOccurrenceIndex = foundPosList.Count - 1;

            HighlightAndScrollTo(editor, foundPosList[prevOccurrenceIndex], text.Length, activeSelectionColor);
            return foundPosList[prevOccurrenceIndex];
        }

        private int ReplaceNextOccurence(string originalWord, string replacementWord)
        {
            var pos = FindAndHighlightNextOccurrence(originalWord);
            if (pos > -1)
            {
                foundPosList.Remove(pos);
                editor.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, replacementWord);
                for (int index = 0; index < foundPosList.Count; index++)
                {
                    if (foundPosList[index] > pos) foundPosList[index] += replacementWord.Length - originalWord.Length;
                }
            }
            return pos;
        }

        private void ReplaceAllOccurence(string originalWord, string replacementWord)
        {
            editor.Document.Selection.SetRange(0, 0);
            while (foundPosList.Count > 0)
            {
                ReplaceNextOccurence(originalWord, replacementWord);
            }
        }

        private void EnterTabKey(RichEditBox richEditBox, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (!ViewModel.UseSoftTab)
            {
                richEditBox.Document.Selection.TypeText("\t");
                e.Handled = true;
            }
            else
            {
                var start = richEditBox.Document.Selection.StartPosition;
                var end = richEditBox.Document.Selection.EndPosition;
                richEditBox.Document.Selection.HomeKey(Windows.UI.Text.TextRangeUnit.Line, false);
                var lineBeginPos = richEditBox.Document.Selection.StartPosition;
                var numSpaces = ViewModel.TabSize - ((start - lineBeginPos) % ViewModel.TabSize);
                richEditBox.Document.Selection.SetRange(start, end);
                richEditBox.Document.Selection.TypeText(new string(' ', numSpaces));
                e.Handled = true;

            }
        }
    }
}
