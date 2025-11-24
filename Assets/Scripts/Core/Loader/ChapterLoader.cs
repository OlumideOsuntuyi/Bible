using System.Collections.Generic;

using Core.Module;
using Core.Notes;

using Dominion;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Visuals;
using Visuals.Module;

using Voxel;

namespace Core.Display
{
    public class ChapterLoader : MonoBehaviour
    {
        public State bufferState;

        public int book;
        public int chapter;
        public int verse;

        public VerticalLayoutGroup insertable;
        public float fontSize;

        public ScrollView view;
        public Scrollbar verticalScrollBar;

        public List<InsertableVerse> verses { get; set; }

        public void Load()
        {
            Load(JsonLoader.bible[book][chapter]);
        }

        public void Load(int verse)
        {
            this.verse = verse;
            Load(JsonLoader.bible[book][chapter]);
            MarkVerse(verse, true);
        }

        private void Load(Chapter chapter)
        {
            verses ??= new List<InsertableVerse>();
            verses.Clear();

            MegaUtils.ClearChildren(insertable.transform);
            var font = LoadLanguages.Instance.GetFont(JsonLoader.bible.language);
            MakeGap(70f, insertable.transform);
            for (int i = 1; i <= chapter.Count; i++)
            {
                var verse = chapter[i];

                InsertableVerse text = Instantiate(CDM.GetPrefab<InsertableVerse>("verse-text"), insertable.transform);
                Reference reference = new()
                {
                    book = chapter.book,
                    chapter = chapter.chapter,
                    verse = i,
                    verseEnd = i
                };
                text.reference = reference;
                text._text.font = font;
                text.SetFontSize(fontSize);
                text.SetVerse(verse);
                verses.Add(text);
            }

            MakeGap(150f, insertable.transform);


            // scroll
            verticalScrollBar.value = 1.0f - Mathf.Clamp01((verse * 1.0f) / verses.Count);

            ThreadsManager.QueueFutureAction(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)insertable.transform);  
            }, 0.05f);
        }

        void MakeGap(float size, Transform bookContent)
        {
            var gapObj = new GameObject("gap");
            var rect = gapObj.AddComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.SetParent(bookContent);
            rect.localScale = Vector3.up;
            rect.sizeDelta = Vector2.up * size;
        }

        public void MarkVerse(int verse, bool scroll=true)
        {
            if (scroll)
            {
                float c = JsonLoader.bible[book][chapter].Count;
                verticalScrollBar.value = Mathf.InverseLerp(c, 1, verse);
            }

            verse--;
            if(verse >= verses.Count)
            {
                Debug.Log($"Has {verses.Count} this cannot move to index {verse}");
                return;
            }
            verses[verse].Mark();
        }


        public static string InsertFontSize(string text, float size)
        {
            return $"<size={size}>{text}</size>";
        }

        public static string InsertColor(string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }

        public static string InsertColor(string text, string color)
        {
            return $"<color=#{color}>{text}</color>";
        }

        public void Next()
        {
            chapter += 1;
            if(chapter > JsonLoader.bible[book].Count)
            {
                if(book == 66)
                {
                    book = 1;
                }
                else
                {
                    book = Mathf.Clamp(book + 1, 1, 66);
                }
                chapter = 1;
            }

            verse = 1;
            Load();
        }

        public void Prev()
        {
            chapter -= 1;
            if (chapter <= 0)
            {
                if(book == 1)
                {
                    book = 66;
                }
                else
                {
                    book = Mathf.Clamp(book - 1, 1, 66);
                }
                chapter = JsonLoader.bible[book].Count;
            }

            verse = 1;
            Load();
        }


        public (int book, int chapter) GetNext()
        {
            int book = this.book;
            int chapter = this.chapter;

            chapter += 1;
            if (chapter > JsonLoader.bible[book].Count)
            {
                book = Mathf.Clamp(book + 1, 1, 66);
                chapter = 1;
            }

            return (book, chapter);
        }

        public (int book, int chapter) GetPrev()
        {
            int book = this.book;
            int chapter = this.chapter;

            chapter -= 1;
            if (chapter <= 0)
            {
                book = Mathf.Clamp(book - 1, 1, 66);
                chapter = JsonLoader.bible[book].Count;
            }

            return (book, chapter);
        }

        public void SwipePrev()
        {

        }


        [System.Serializable]
        public struct State
        {
            public int book;
            public int chapter;
        }
    }
}