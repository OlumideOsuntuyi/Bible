using UnityEngine;
using UnityEngine.UI;

using Visuals.Module;

namespace Core.Notes
{
    public class NoteItem : MonoBehaviour
    {
        public Image icon;
        public InsertableText content;
        public InsertableText date;
        private int id;

        public void Set(string text, int id)
        {
            this.id = id;
            content.text = text;
        }

        public void Open()
        {
            NoteManager.Instance.OpenSelectedNote(id);
        }
    }
}