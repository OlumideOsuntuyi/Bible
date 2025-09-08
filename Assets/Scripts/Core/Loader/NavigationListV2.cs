using System;

using Core;

using LostOasis;

using TMPro;

using UnityEngine;

namespace Visuals
{
    public class NavigationListV2 : Singleton<NavigationListV2>
    {
        public int book;
        public int chapter;
        public int verse;
        public SelectionMode mode;

        public TMP_Text selectionLabel;
        public RectTransform bookContentOT;
        public RectTransform bookContentNT;
        public RectTransform chapterContent;
        public RectTransform verseContent;
        public NavV2Item prefab;
        public ScreenControl pageNavigation;
        public UIButtonGroup pageNavigators;
        public PageTransition toolbars;


        public bool IsOT(int book) => book <= 39;

        public void Open()
        {
            ScreenManager.Set("Navigation", "tap");
            toolbars.SetActive("hidden");
            pageNavigation.SetActiveBox(0);
            pageNavigators.Click(0);
            ListBooks();
        }

        public void Close()
        {
            ScreenManager.Set("Navigation", 0);
            toolbars.SetActive(0);
            Clear();
        }

        private void Clear()
        {
            MegaUtils.ClearChildren(bookContentOT);
            MegaUtils.ClearChildren(bookContentNT);
            MegaUtils.ClearChildren(chapterContent);
            MegaUtils.ClearChildren(verseContent);
        }

        public void ListBooks()
        {
            MegaUtils.ClearChildren(bookContentOT);
            MegaUtils.ClearChildren(bookContentNT);
            for (int i = 1; i <= 66; i++)
            {
                var book = Instantiate(prefab, IsOT(i) ? bookContentOT : bookContentNT);
                book.Set(i, 0);
            }

            SetLabel();
        }

        public void ListChapters()
        {
            MegaUtils.ClearChildren(chapterContent);
            var book = JsonLoader.bible[this.book];
            int count = book.Count;

            for (int i = 1; i <= count; i++)
            {
                var chapter = Instantiate(prefab, chapterContent);
                chapter.Set(i, 1);
            }

            SetLabel();
        }

        public void ListVerses()
        {
            MegaUtils.ClearChildren(verseContent);
            var chapter = JsonLoader.bible[this.book][Mathf.Max(1, this.chapter)];
            int count = chapter.Count;

            for (int i = 1; i <= count; i++)
            {
                var verse = Instantiate(prefab, verseContent);
                verse.Set(i, 2);
            }

            SetLabel();
        }

        public void GoToNavigation()
        {
            ChapterLoaderManager.Instance.book = book;
            ChapterLoaderManager.Instance.chapter = chapter > 0 ? chapter : 0;
            ChapterLoaderManager.Instance.verse = verse > 0 ? verse : 1;  // if verse was selected
            ChapterLoaderManager.Instance.Load();

            ChapterHistoryManager.Add(new Reference()
            {
                book = book,
                chapter = chapter > 0 ? chapter : 0,
                verse = verse > 0 ? verse : 0,
                verseEnd = verse > 0 ? verse : 0
            });

            pageNavigation.SetActiveBox(0);
            Close();
        }

        public void SetLabel()
        {
            selectionLabel.text = $"{Book.ABBREVIATIONS[book - 1].FullName} {Mathf.Max(1, chapter)}";
        }

        public void SelectPage(int index)
        {
            switch(index)
            {
                case 0: SelectBook(); break;
                case 1: SelectChapter(); break;
                case 2: SelectVerse(); break;
                default: break;
            }
        }

        public void SelectBook()
        {
            ListBooks();
            pageNavigation.SetActiveBox(0);
        }

        public void SelectChapter()
        {
            ListChapters();
            pageNavigation.SetActiveBox(1);
        }

        public void SelectVerse()
        {
            ListVerses();
            pageNavigation.SetActiveBox(2);
        }

        public void SelectBook(int index)
        {
            chapter = -1; // no chapter selected
            book = index;
            pageNavigators.Click(1);
        }

        public void SelectChapter(int index)
        {
            verse = -1; // no verse selected
            chapter = index;
            pageNavigators.Click(2);
        }

        public void SelectVerse(int index)
        {
            verse = index;
            GoToNavigation();
        }


        public enum SelectionMode
        {
            TwoTap, ThreeTap
        }
    }
}