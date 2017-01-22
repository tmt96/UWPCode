using System;
using UWPCode.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI;
using System.Linq;

namespace UWPCode.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            CreateAndDisplayNewFile();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        public void UpdateTextArea(string text)
        {
            editor.Document.SetText(Windows.UI.Text.TextSetOptions.None, text);
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
            editor.Document.SetText(Windows.UI.Text.TextSetOptions.None, buffer.Text);
            pageHeader.Text = buffer.Name;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentBuffer();
        }

        private async Task<StorageFile> SaveCurrentBuffer()
        {
            var buffer = ((App)Application.Current).BufferOrganizer.CurrentBuffer;
            var text = GetEditorText();
            var file = await ViewModel.UpdateAndSaveBuffer(buffer, text);
            DisplayBuffer(buffer);
            return file;
        }

        private string GetEditorText()
        {
            string text;
            editor.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);
            return text;
        }

        private void fileListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void splitViewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void functionListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void functionListButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void remoteFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sourceControlButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GotoSettings();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<int> foundPosList = new List<int>();
        private int indexInFoundPosList = -1;
        Color activeSelectionColor = Colors.LightBlue;
        Color inactiveSelectionColor = Colors.LightGray;

        private void findBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            FindAndHighLightAllOccurrence();
        }

        private void FindAndHighLightAllOccurrence()
        {
            var cursorPos = editor.Document.Selection.StartPosition;
            ClearHighlights();
            foundPosList = FindAllOccurence(findBox.Text, 0);
            HighlightAll(inactiveSelectionColor, foundPosList, findBox.Text.Length);
            editor.Document.Selection.SetRange(cursorPos, cursorPos);
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
            string text = GetEditorText();
            int end = text.Length;
            editor.Document.Selection.SetRange(0, end);
            editor.Document.Selection.CharacterFormat.BackgroundColor = (editor.Background as Windows.UI.Xaml.Media.SolidColorBrush).Color;
            editor.Document.Selection.SetRange(cursorPos, cursorPos);
        }

        private List<int> FindAllOccurence(string text, int start)
        {
            var found = new List<int>();
            indexInFoundPosList = -1;
            if (text.Length <= 0) return found;

            string editorText = GetEditorText();
            int pos = editorText.IndexOf(text);
            while (pos > -1)
            {
                found.Add(pos);
                pos = editorText.IndexOf(text, pos + text.Length);
            }
            return found;
        }

        private void searchBoxFlyout_Opening(object sender, object e)
        {
            Flyout flyout = sender as Flyout;
            Style fullWidthFyoutStyle = new Style { TargetType = typeof(FlyoutPresenter) };
            fullWidthFyoutStyle.Setters.Add(new Setter(MinWidthProperty, mainArea.ActualWidth));

            flyout.FlyoutPresenterStyle = fullWidthFyoutStyle;
        }

        private void replaceBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void findNextButton_Click(object sender, RoutedEventArgs e)
        {
            FindAndHighlightNextOccurrence(findBox.Text);
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

            int selectionEnd = editor.Document.Selection.EndPosition;
            int nextOccurrenceIndex = foundPosList.BinarySearch(selectionEnd);
            if (nextOccurrenceIndex < 0) nextOccurrenceIndex = ~ nextOccurrenceIndex;
            if (nextOccurrenceIndex >= foundPosList.Count) nextOccurrenceIndex = 0;

            HighlightAndScrollTo(editor, foundPosList[nextOccurrenceIndex], text.Length, activeSelectionColor);
            return foundPosList[nextOccurrenceIndex];

        }

        private void HighlightAndScrollTo(RichEditBox editor, int indexInFoundPosList, int length, Color highlightColor)
        {
            editor.Document.Selection.SetRange(indexInFoundPosList, indexInFoundPosList + length);
            editor.Document.Selection.CharacterFormat.BackgroundColor = highlightColor;
            editor.Document.Selection.ScrollIntoView(Windows.UI.Text.PointOptions.None);
        }

        private void findPrevButton_Click(object sender, RoutedEventArgs e)
        {
            FindAndHighlightPrevOccerrence(findBox.Text);
        }

        private int FindAndHighlightPrevOccerrence(string text)
        {
            ClearHighlights();
            HighlightAll(inactiveSelectionColor, foundPosList, text.Length)

            if (foundPosList == null || foundPosList.Count == 0) return -1;

            int selectionStart = editor.Document.Selection.StartPosition;
            int prevOccurrenceIndex = foundPosList.BinarySearch(selectionStart);
            if (prevOccurrenceIndex < 0) prevOccurrenceIndex = ~prevOccurrenceIndex;
            prevOccurrenceIndex -= 1;
            if (prevOccurrenceIndex < 0) prevOccurrenceIndex = foundPosList.Count - 1;

            HighlightAndScrollTo(editor, foundPosList[prevOccurrenceIndex], text.Length, activeSelectionColor);
            return foundPosList[prevOccurrenceIndex];
        }

        private void replaceNextButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceNextOccurence(findBox.Text, replaceBox.Text);
        }

        private int ReplaceNextOccurence(string originalWord, string replacementWord)
        {
            int pos = FindAndHighlightNextOccurrence(originalWord);
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
        private void replaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceAllOccurence(findBox.Text, replaceBox.Text);
        }

        private void ReplaceAllOccurence(string originalWord, string replacementWord)
        {
            editor.Document.Selection.SetRange(0, 0);
            while (foundPosList.Count > 0)
            {
                ReplaceNextOccurence(originalWord, replacementWord);
            }
        }

        private void editor_TextChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
