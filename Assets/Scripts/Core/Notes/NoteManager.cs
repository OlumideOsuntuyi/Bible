using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Dominion;

using LostOasis;

using TMPro;

using UnityEngine;

using Visuals;
using Visuals.Module;

namespace Core.Notes
{
    using Random = UnityEngine.Random;

    public class NoteManager : Singleton<NoteManager>
    {
        private Dictionary<string, List<Note>> notes;
        private List<int> ids;

        public RectTransform sortedNotesContent;
        public InJournalGroupSelector noteGroupSelectorPrefab;
        public InJournalGroupSelector[] specialGroups;

        public RectTransform notesInGroupContent;
        public NoteItem noteSelectorPrefab;

        public InsertableText inGroupHeader;
        public InsertableText inGroupHeaderTwo;

        public TMP_Text newFolderName;
        [SerializeField] private string selectedNoteGroup;

        private static readonly string[] specialGroupingNames =
        {
            "Recent", "Notes", "Saves", "Verse Notes", "Highlights"
        };

        private static Func<int>[] specialGroupingCounts;


        private void Awake()
        {
            specialGroupingCounts = new Func<int>[]
            {
                () => { return -1; }, Count, 
                () => {return 0; }, VerseNotesCount, HighlightCount

            };

            selectedNoteGroup = "Notes";
            notes = new();
            ids = new List<int>();
            var list = FileHandler.LoadObject<List<Note>>(Path.Combine(Application.persistentDataPath, "Notes.bin"));

            foreach (var n in list)
            {
                if (!notes.TryGetValue(n.group, out var l))
                {
                    l = new List<Note>();
                    notes.Add(n.group, list);
                }

                ids.Add(n.ID);
                l.Add(n);
            }
        }

        private void OnDestroy()
        {
            List<Note> notes = new();
            foreach(var kvp in this.notes)
            {
                foreach(var note in kvp.Value)
                {
                    notes.Add(note);
                }
            }

            FileHandler.SaveObject(notes, Path.Combine(Application.persistentDataPath, "Notes.bin"));
        }
        public void ReturnToHome()
        {
            selectedNoteGroup = "Notes";
            ScreenManager.Set("Notes", "Main");
        }

        public void LoadNoteGroups()
        {
            for (int i = 0; i < specialGroups.Length; i++)
            {
                specialGroups[i].Set(specialGroupingNames[i], specialGroupingCounts[i].Invoke());
            }

            MegaUtils.ClearChildren(sortedNotesContent);
            foreach(var noteList in notes)
            {
                var clone = Instantiate(noteGroupSelectorPrefab, sortedNotesContent);
                clone.Set(noteList.Key, noteList.Value.Count);
            }
        }

        private void LoadHighlightNotes()
        {
            inGroupHeader.text = $"Highlights";
            inGroupHeaderTwo.text = $"Highlights";
            MegaUtils.ClearChildren(notesInGroupContent);
            var list = Highlighter.GetAll();
            foreach(var h in list)
            {
                var clone = Instantiate(CDM.GetPrefab<NoteHighlightItem>("Highlight Item"), notesInGroupContent);
                clone.Set(h.Key, h.Value);

                clone.name = $"{h.Value.id} {h.Value.inID} {h.Value.date}";
            }
        }

        private void LoadNotes(List<Note> notes, string title)
        {
            // set current note group
            if (specialGroupingNames.Contains(tag))
            {
                // if current note is a special type
                selectedNoteGroup = "Notes";
            }
            else
            {
                selectedNoteGroup = title;
            }

            inGroupHeader.text = $"{title}";
            inGroupHeaderTwo.text = $"{title}";
            MegaUtils.ClearChildren(notesInGroupContent);
            NavigationList.MakeGap(20, notesInGroupContent);

            foreach (var note in notes)
            {
                var clone = Instantiate(noteSelectorPrefab, notesInGroupContent);
                clone.Set(note.content, note.ID);
            }
        }

        public void OpenSelectedNoteGroup(string tag)
        {
            List<Note> notes = new();
            if (specialGroupingNames.Contains(tag))
            {
                switch (tag)
                {
                    case "Highlights":
                        {
                            LoadHighlightNotes();
                        }
                        break;
                    default: break;
                }
            }
            else
            {
                notes = this.notes[tag];
                LoadNotes(notes, tag);
            }

            ScreenManager.Set("Notes", "Note List");
        }

        public Note Get(int id)
        {
            foreach(var grp in notes.Values)
            {
                foreach(var n in grp)
                {
                    if (n.ID == id) return n;
                }
            }

            return null;
        }

        public void OpenSelectedNote(int noteID)
        {
            var note = Get(noteID);
            NoteEditor.Instance.LoadNote(note);
            
            ScreenManager.Set("Notes", "Note");
        }

        public int GetID()
        {
            int rand = 0;
            while(rand != 0 || ids.Contains(rand))
            {
                rand = Random.Range(int.MinValue, int.MaxValue);
            }

            return rand;
        }

        public void NewNote(string group="")
        {
            bool nullGroup = string.IsNullOrEmpty(group);
            group = selectedNoteGroup;
            var sc = ScreenManager.Get("NoteFunctions");
            sc.SetActiveBox(0);
            sc.gameObject.SetActive(false);

            Note note = new()
            {
                ID = GetID(),
                dateCreated = System.DateTime.Now.ToString(),
                dateOpened = System.DateTime.Now.ToString(),
                content = "",
                group = group
            };

            if(!notes.ContainsKey(group))
            {
                notes.Add(group, new List<Note>());
            }

            notes[group].Add(note);

            if(nullGroup)
            {
                OpenSelectedNote(note.ID);
            }
        }

        public void StartCreateNewFolder()
        {
            Keyboard.OnClick += OnClickKeyboard;
            Keyboard.Open();
        }

        public void StopCreateNewFolder()
        {
            Keyboard.OnClick -= OnClickKeyboard;
            Keyboard.Close();
            var sc = ScreenManager.Get("NoteFunctions");
            sc.SetActiveBox(0);
            sc.gameObject.SetActive(false);
        }

        public void OnClickKeyboard(string text)
        {
            newFolderName.text = text;
        }

        public void CreateNewFolder()
        {
            var text = Keyboard.Instance.text;
            Keyboard.Clear();

            if(text.Length == 0)
            {
                return;
            }

            if (notes.ContainsKey(text))
            {
                return;
            }

            notes.Add(text, new List<Note>());
            newFolderName.text = "";
            StopCreateNewFolder();
            LoadNoteGroups();
            ScreenManager.Set("Notes", "Main");
        }



        // Special Groupings
        private int Count()
        {
            int c = 0;
            foreach(var v in notes.Values)
            {
                c += v.Count;
            }

            return c;
        }

        private int VerseNotesCount()
        {
            if(!notes.TryGetValue("Verse Summary", out var list))
            {
                return 0;
            }

            return list.Count;
        }

        private int HighlightCount()
        {
            return Highlighter.Count;
        }
    }
}