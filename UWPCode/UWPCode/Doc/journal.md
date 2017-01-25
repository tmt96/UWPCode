**Development Journal - UWPCode**

Minh Tuan Tran

## 1/6/2017

Decided to work on a code editor for the UWP platform. Goals could be found in README. Plan for the next few days:
- 1/8/2016: Finish basic UI & nonfunctional Setting page. Open, edit, and save files.
- 1/15/2016: Syntax highlighting
- 1/19/2016: Syntax theme
- 1/25/2016: Other stuffs: Comment line, polish setting, wrapping & folding, split view. Polishing & other features if possible.

## 1/8/2017

Spent two days battling my laptop with updates, VS errors & others. Fed up and fulled off my Win2Go disk. Good call!

Process really picked up (despite spending an afternoon setting up the drive for UWP development). Have the desired UI. However, I'm well 2 days behind schedule now.

Program GUI

![Program GUI](GUI.PNG)

## 1/11/2017
Trouble with adding logic to the program. Since my hamburger menu is in a different layer than my Mainpage, I cannot easily carry out action from the hamburger menu (add/open file) and have it change the Mainpage (editor, etc). One option is to reorganize my hamburger menu & command bar.

The goal today is to have the program successfully open & save a file. Hopefully I could make it.

## 1/11/2017
Finished with open & add file. Working on save and close file.

![Open File](OpenFile.png)
Open an existing file.
![Display File](DisplayFile.png)
The editor display the file

## 1/13/2017
Happy birthday to me! Got caught in birthday celebration & GRE stuffs for the last two days (ok I got a bit lazy). Anyway, the goal now is to move to syntax highlighting part by 1/16, which means that I need to finish save & close files by tomorrow and search/replace by Sunday.

Search and replace seems to be the difficult one. To highlight result I will need a RichEditBox instead of a TextBox, which is awkward to use (no RichEditBox.Text....). It also means I will need to change a bit of my code. We'll see how that go.

## 1/14/2017
Finished with save files. Close file is possible, but I haven't created a GUI for the task

![Save File](SaveFile.png)
Saving a file

## 1/15/2017
Troubles with highlighting search result although searching works (not that it is difficult). Need to solve the problem before moving to syntax highlighting: if I cannot conditionally change the text format, then I can't change do any syntax highlight.

## 1/17/2017
Able to correctly highlight search result in editor. Turns out it was a weird bug in UWP that automatically hide `SelectionHighlightColor` to show when unfocused. New problem: editor change color theme when in focus. Looking to the solution. I'll start looking at syntax highlighting soon.

## 1/18/2017
Things to do: Fix the flyout appearance (it is too narrow and the searchbox has no room to breathe), fix the focus problem with the editor, and change the data structure for efficiency (we want the text to be an array of string for faster execution).

Now that I only have a week left, I need to make a choice: should I implement TextMate grammar, or should I implement syntax highlighting on my own? The first option is longer to get things done but more flexible in the future; the second is easier for one language, but not sustainable in the long haul.

## 1/21/2017
Another period of laziness. I have added the ability to search forward & backward, as well as replacing functionality. It's slow, but it works. The search bar now stretches horizontally, which is neat.

Things to do next:
- Fix editor appearance
- Settings
- Multi-file functionality
- Clean code
- Change text representation to using array of strings to improve performance

![Search](Search.png)
![Replace](Replace.png)
Search & Replace functions in action

## 1/22/2017
A good day in the office. The editor now has a consistent background throughout. Settings page is added and functions properly, with option to change font, font size, word wrap, etc... Code has a light clean up and is now easier to follow. This is now a functional Notepad replacement, albeit slow. Next stop: Multiple files functionality.

(Still bummed that I could not do the syntax highlighting. Also realized that I have not taken enough screenshots, so here we go!)

![Settings](SettingsPage.png)
Settings page
![Light Theme](LightTheme.png)
![No Word Wrap](NoWordWrap.png)
![Changing Font](FontChange.png)
Different appearance from changing settings


## 1/23/2017
Think I will have to postpone tabbed interface for now. The reason is that I have trouble in determining the buffer we need to display in the editor when we select new item from the flyout if there are two files having the same name (maybe different path). Without careful handling, we could easily mess up the relation between tab name and file name. Moreover, with the current implementation, reloading an opened file would take a long time.

A better approach would be to create a new custom Page any time we open/create a new file. Basically, the left-hand-side bar serve as a Navigation Shell while the editor & command bar would belong to the custom Page. However, this approach is difficult, requires time & lots of reorganization, maybe even close to a rewrite. For now, let's assume we only open one file at a time.

So wish list:
- Change data structure to array of strings
- Programmatically generate custom Page for each buffer- Editor line numbers
- Syntax highlighting

## 1/23/2017
Now that I think about it, the custom View approach simply asks for too much effort. Definitely Over-engineer. I will have a second try with the Dictionary approach tomorrow.

## 1/24/2017
Positive: The opened file sidebar now works and shows all opened files correctly. Code is incredibly ugly and messy, but hey it works!

Negative: I tried to be too smart, pulling off the StorageFile property from Buffer and using a path instead to reduce memory load. The problem: UWP apps do not have the access to all files in the system, especially those chosen from FilePicker. As a result, saving old Buffers is now broken. ~~Looking to fix.~~ Fixed that.

![Side Bar](OpenFileBar.png)
Side panel displaying opened files.
