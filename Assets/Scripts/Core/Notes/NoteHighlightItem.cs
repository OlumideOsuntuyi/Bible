using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Visuals.Module;

namespace Core.Notes
{
    public class NoteHighlightItem : MonoBehaviour
    {
        public Image icon;
        public InsertableText content;
        public TMP_Text date;

        public Reference reference;

        public void Set(Reference reference, Highlighter.HighlightData data)
        {
            this.reference = reference;
            icon.color = NoteEditor.GetSwatchColor(data.index + 1);

            int end = Mathf.Max(reference.verse + 1, reference.verseEnd);
            string txt = JsonLoader.bible.Parse(reference.book, reference.chapter, reference.verse, end == reference.verse + 1 ? 0 : end) + '\n';
            for (int i = reference.verse; i < end; i++)
            {
                txt += JsonLoader.bible[reference.book][reference.chapter][i].text;
            }

            content.text = txt + '\n' + '\n';
            this.date.text = DateTime.Parse(data.date).ToShortDateString();
        }

        public void Open()
        {
            ChapterLoaderManager.Instance.Load(reference);
            ScreenManager.Set("Main", "Bible");
        }
    }
}