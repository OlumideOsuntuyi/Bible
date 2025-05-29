using UnityEngine;

using Visuals.Module;

namespace Core.Notes
{
    public class NoteItem : MonoBehaviour
    {
        public InsertableText header;
        public InsertableText date;

        public void Set(string title, string text)
        {
            header.text = title;
        }
    }
}