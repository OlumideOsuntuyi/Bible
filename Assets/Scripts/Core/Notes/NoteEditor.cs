using System.Collections.Generic;

using Core.Notes;

using LostOasis;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Visuals;

using static Core.NoteEditor;

namespace Core
{
    public class NoteEditor : Singleton<NoteEditor>
    {
        public float formattedButtonsGap;

        public Image lockedIcon;
        public Image caretImage;
        public RectTransform caretParent;
        public Sprite[] lockedSprites;
        public GameObject[] lockGameobjects;

        public InNoteLineDisplay inNoteLine;
        public RectTransform content;
        public RectTransform formatButtonsRect;
        public FormattingButtons formattingButtons;
        public InNoteLineDisplay selectedLine;


        private Note loadedNote;
        private List<InNoteLineDisplay> lines;
        private bool locked;
        private string currentText;

        private void Awake()
        {
            InNoteLineDisplay.caretImage = caretImage;
        }

        private void Update()
        {
            if (!locked) return;
            InNoteLineDisplay.HandleCaretBlinking();
        }

        public void LoadNote(Note note)
        {
            loadedNote = note;
            Clear();

            if(note.lines.Count == 0)
            {
                note.lines.Add(new InNoteLine()
                {
                    text = ""
                });
            }

            foreach(var l in note.lines)
            {
                var clone = Instantiate(inNoteLine, content);
                clone.Set(lines.Count, l.text, l.format);
                lines.Add(clone);
            }

            selectedLine = lines[^1];
            LayoutRebuilder.MarkLayoutForRebuild(content);
        }

        public void Clear()
        {
            lines ??= new List<InNoteLineDisplay>();
            lines.Clear();
            MegaUtils.ClearChildren(content);
        }

        public void Open()
        {
            formatButtonsRect.gameObject.SetActive(true);
            foreach(var g in lockGameobjects)
            {
                g.SetActive(false);
            }

            locked = true;
            lockedIcon.sprite = lockedSprites[1];
            loadedNote.dateOpened = System.DateTime.Now.ToString();

            currentText = selectedLine.txt;
            string[] words = currentText.Split(' ');
            Keyboard.Instance.text = currentText;
            Keyboard.Instance.currentTextDisplay.text = words.Length > 0 ? words[^1] : "";
            Keyboard.Open();


            ScreenManager.Transition("toolbar", "hidden");
            Keyboard.OnClick += OnClick;
            Keyboard.OnClickedCharacter += OnClickCharacter;
            Keyboard.OnNewLine += OnNewLine;
            Keyboard.OnBackSpace += OnBackspace;
            Keyboard.OnBackSpaceCleared += OnBackSpaceEmptyLine;
            Keyboard.OnClose += () =>
            {
                if(locked)
                {
                    Close();
                }
            };


            SetButtons();
        }

        public void Close()
        {
            formattingButtons.colorSwatchParent.SetActive(false); // close color swatch
            formatButtonsRect.gameObject.SetActive(false); // hide formatting options
            foreach (var g in lockGameobjects)
            {
                g.SetActive(true);
            }
            ScreenManager.Transition("toolbar", "main");

            locked = false;
            lockedIcon.sprite = lockedSprites[0];

            Keyboard.OnClick -= OnClick;
            Keyboard.OnClickedCharacter -= OnClickCharacter;
            Keyboard.OnNewLine -= OnNewLine;
            Keyboard.OnBackSpace -= OnBackspace;
            Keyboard.OnBackSpaceCleared -= OnBackSpaceEmptyLine;
            Keyboard.Clear();
            Keyboard.Close();
            InNoteLineDisplay.ResetBlinking(); // stop carat blinking
        }

        public void OpenNewLine()
        {
            currentText = selectedLine.txt;
            string[] words = currentText.Split(' ');
            Keyboard.Instance.text = currentText;
            Keyboard.Instance.currentTextDisplay.text = words.Length > 0 ? words[^1] : "";

            SetButtons();
        }

        private void OnNewLine()
        {
            var line = new InNoteLine()
            {
                text = "",
                format = selectedLine.format
            };

            int newLine = selectedLine.line + 1;
            loadedNote.lines.Insert(newLine, line);
            LoadNote(loadedNote);

            selectedLine = lines[newLine];
            selectedLine.SelectLine();

            UpdateNumberedList(selectedLine.format);
        }

        private void OnBackspace(string text)
        {
            if (currentText.Length == 0) return; // on no text, skip

            currentText = InNoteLineDisplay.RemoveCharAtIndex(currentText, InNoteLineDisplay.caretPosition - 1);
            loadedNote.lines[selectedLine.line].text = currentText;
            selectedLine.txt = currentText;
            selectedLine.Reload();

            // move caret backwards
            selectedLine.MoveCaret(-1);
        }

        private void OnBackSpaceEmptyLine()
        {
            if (lines.Count < 2) return; // only with two or more lines
            loadedNote.lines.RemoveAt(selectedLine.line);
            LoadNote(loadedNote);
        }

        private void OnClickCharacter(char c)
        {
            currentText = InNoteLineDisplay.InsertCharacter(currentText, c, InNoteLineDisplay.caretPosition);

            loadedNote.lines[selectedLine.line].text = currentText;
            selectedLine.txt = currentText;
            selectedLine.Reload();

            // move caret forward
            selectedLine.MoveCaret(1);
        }

        private void OnClick(string text)
        {
            if (!locked) return;
        }


        public void ToggleLock()
        {
            if(locked)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void ReturnToHome()
        {
            if(locked)
            {
                Close();
            }
            else
            {
                NoteManager.Instance.ReturnToHome();
            }
        }

        private void UpdateNumberedList(NoteLineFormat format)
        {
            selectedLine.format = format;
            if (format.list is not NoteLineFormat.ListType.Numbered)  // decrement
            {
                int i = selectedLine.line;
                while (i < lines.Count)
                {
                    var f = lines[i].format;
                    if (i != selectedLine.line)
                    {
                        if (f.lineNumber == 0) break;
                    }

                    if (i == 0)
                    {
                        format.lineNumber = 0;
                    }
                    else
                    {
                        f.lineNumber -= 1;
                    }

                    loadedNote.lines[i].format = f;
                    lines[i].format = f;
                    lines[i].Reload();
                    i++;
                }
            }
            else // increment
            {
                int i = selectedLine.line;
                while (i < lines.Count)
                {
                    var f = lines[i].format;
                    if (i != selectedLine.line)
                    {
                        if (f.lineNumber == 0) break;
                    }

                    if (i == 0)
                    {
                        f.lineNumber = 1;
                    }
                    else
                    {
                        f.lineNumber = lines[i - 1].format.lineNumber + 1;
                    }

                    loadedNote.lines[i].format = f;
                    lines[i].format = f;
                    lines[i].Reload();
                    i++;
                }
            }
        }

        public void SetButtons()
        {
            NoteLineFormat format = selectedLine.format;

            formattingButtons.Set(formattingButtons.boldButton, false);
            formattingButtons.Set(formattingButtons.italicsButton, false);
            formattingButtons.Set(formattingButtons.underlineButton, false);


            formattingButtons.Set(formattingButtons.normalSizeButton, format.size is NoteLineFormat.SizeType.N);
            formattingButtons.Set(formattingButtons.h1Button, format.size is NoteLineFormat.SizeType.H1);
            formattingButtons.Set(formattingButtons.h2Button, format.size is NoteLineFormat.SizeType.H2);
            formattingButtons.Set(formattingButtons.h3Button, format.size is NoteLineFormat.SizeType.H3);

            formattingButtons.Set(formattingButtons.numberedListButton, format.list is NoteLineFormat.ListType.Numbered);
            formattingButtons.Set(formattingButtons.dottedListButton, format.list is NoteLineFormat.ListType.Dotted);


            formattingButtons.Set(formattingButtons.markButton, format.mark);
        }

        public void SetButton(string button)
        {
            NoteLineFormat format = selectedLine.format;
            switch (button)
            {
                case "bold":
                    {
                        format.bold = !format.bold;
                    }
                    break;
                case "italics":
                    {
                        format.italic = !format.italic;
                    }
                    break;
                case "underline":
                    {
                        format.underline = !format.underline;
                    }
                    break;
                case "clear":
                    {
                        format = new NoteLineFormat();
                    }
                    break;
                case "n":
                    {
                        format.size = NoteLineFormat.SizeType.N;
                    } break;
                case "h1":
                    {
                        format.size = NoteLineFormat.SizeType.H1;
                    }
                    break;
                case "h2":
                    {
                        format.size = NoteLineFormat.SizeType.H2;
                    }
                    break;
                case "h3":
                    {
                        format.size = NoteLineFormat.SizeType.H3;
                    }
                    break;
                case "nlist":
                    {
                        format.list = format.list is NoteLineFormat.ListType.Numbered ? NoteLineFormat.ListType.None : NoteLineFormat.ListType.Numbered;
                        UpdateNumberedList(format);
                        SetButtons();
                        return;
                    }
                case "dlist":
                    {
                        format.list = format.list is NoteLineFormat.ListType.Dotted ? NoteLineFormat.ListType.None : NoteLineFormat.ListType.Dotted;
                    }
                    break;
                case "aindent":
                    {
                        format.indentSize = Mathf.Min(5, format.indentSize + 1);
                    }
                    break;
                case "rindent":
                    {
                        format.indentSize = Mathf.Max(0, format.indentSize - 1);
                    }
                    break;
                case "mark":
                    {
                        format.mark = !format.mark;
                    } break;
                case "color":
                    {
                        ToggleColors();
                        formattingButtons.scrollbar.value = 1;
                    }
                    break;
                default:break;
            }

            loadedNote.lines[selectedLine.line].format = format;
            selectedLine.format = format;
            selectedLine.Reload();
            SetButtons();
        }

        public void SelectedColor(int swatch)
        {
            NoteLineFormat format = selectedLine.format;
            format.color = swatch;


            loadedNote.lines[selectedLine.line].format = format;
            selectedLine.format = format;
            selectedLine.Reload();
            SetButtons();
        }

        public void ToggleColors()
        {
            formattingButtons.colorSwatchParent.SetActive(!formattingButtons.colorSwatchParent.activeSelf);
        }

        public static Color GetSwatchColor(int color)
        {
            return Instance.formattingButtons.colors[color].image.color;
        }
        public static Color GetMarkColor()
        {
            return Instance.formattingButtons.markColor;
        }


        [System.Serializable]
        public class FormattingButtons
        {
            public Image[] boldButton;

            public Image[] italicsButton;

            public Image[] underlineButton;

            public Image[] normalSizeButton;

            public Image[] h1Button;

            public Image[] h2Button;

            public Image[] h3Button;

            public Image[] numberedListButton;

            public Image[] dottedListButton;

            public Image[] markButton;

            public Scrollbar scrollbar;

            public GameObject colorSwatchParent;
            public Color markColor;
            public ColorSwatch[] colors;

            public void Set(Image[] images, bool state)
            {
                images[0].color = state ? ThemeManager.Colors[(int)ThemeColorType.Active] : Color.clear;
                images[1].color = state ? ThemeManager.Colors[(int)ThemeColorType.TextInActive] : ThemeManager.Colors[(int)ThemeColorType.Text];
            }

        }
    }
}