using Core;

using TMPro;

using UnityEngine;

using Visuals.Module;

namespace Visuals
{
    public class HistoricalReference : MonoBehaviour
    {
        public TMP_Text header;
        public TMP_Text text;
        public InsertableText insertable;
        public Reference reference;

        public void Set(Reference reference)
        {
            reference.verse = Mathf.Max(1, reference.verse);
            this.reference = reference;
            var parses = JsonLoader.bible.Parse(reference.book, reference.chapter, Mathf.Max(1, reference.verse));
            header.text = $"{parses}";
            text.text = $"{JsonLoader.bible[reference.book][reference.chapter][Mathf.Max(1, reference.verse)].text}";

            Invoke(nameof(Resize), 0.05f);
        }

        private void Resize()
        {
            insertable.Resize();
        }

        public void Click()
        {
            ChapterLoaderManager.Instance.Load(reference);
            ChapterHistoryManager.Instance.ClosePage();

            ChapterHistoryManager.Add(reference);
        }
    }
}