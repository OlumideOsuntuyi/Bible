using UnityEngine;

using Visuals;
using Visuals.Module;


namespace Core
{
    public class VerseOfTheDay : MonoBehaviour
    {
        public InsertableText verse;
        public InsertableText text;

        private int b;
        private int c;
        private int v;

        private void Start()
        {
            Randomize();

        }

        public void Randomize()
        {
            HolyBible bible = JsonLoader.bible;
            int book = Random.Range(1, 66);
            int chapter = Random.Range(1, bible[book].Count + 1);
            int verse = Random.Range(1, bible[book][chapter].Count + 1);

            this.verse.text = $"{bible[book].Name} {chapter}:{verse} KJV";
            this.text.text = $"{bible[book][chapter][verse].text}";
            this.text._text.color = ColorPallate.Get("text-color");

            b = book;
            c = chapter;
            v = verse;

            Invoke(nameof(Resize), 0.1f);
        }

        void Resize()
        {
            verse.Resize();
            text.Resize();
        }

        public void Open()
        {
            ScreenManager.Set("Main", "Bible");
            ChapterLoaderManager.Instance.current.book = b;
            ChapterLoaderManager.Instance.current.chapter = c;
            ChapterLoaderManager.Instance.Load(v);


            ChapterHistoryManager.Add(new Reference()
            {
                book = b,
                chapter = c,
                verse = v
            });
        }
    }
}