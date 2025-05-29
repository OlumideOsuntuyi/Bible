using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Core.Notes
{
    public class NoteManager : Singleton<NoteManager>
    {
        private Dictionary<string, List<Note>> notes;
        private void Awake()
        {
            notes = new();
            var list = FileHandler.LoadObject<List<Note>>(Path.Combine(Application.persistentDataPath, "Notes.bin"));
            foreach (var n in list)
            {
                if (!notes.TryGetValue(n.group, out var l))
                {
                    l = new List<Note>();
                    notes.Add(n.group, list);
                }

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

        public void LoadNotes()
        {

        }
    }
}