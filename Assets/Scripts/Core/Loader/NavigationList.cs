using System.Collections.Generic;

using Core;

using Dominion;

using LostOasis;

using TMPro;

using UnityEngine;

using UScrollSnap;

namespace Visuals
{
    public class NavigationList : Singleton<NavigationList>
    {
        public Transform bookContent;
        public Transform chapterContent;
        public Transform verseContent;

        public Transform bookCenter;

        public ScrollSnap bookSnapper;
        public ScrollSnap chapterSnapper;
        public ScrollSnap verseSnapper;

        private List<BookInListItem> booksList;
        private List<ChapterInListItem> chapterList;
        private List<ChapterInListItem> verseList;

        public int currentBook;
        public int currentChapter;
        public int currentVerse;

        public NavigationMode navigationMode;
        public SortMode sortMode;


        [Header("Scroll Wheel")]
        public float initialBookGap;
        public float finalBookGap;
        public float bookInListSize;
        public float wheelCurvature;
        public TMP_Text currentSelectionLabel;

        public static bool IsOpen { get; private set; }

        private void OnEnable()
        {
            currentBook = ChapterLoaderManager.Instance.book;
            currentChapter = ChapterLoaderManager.Instance.chapter;
            currentVerse = ChapterLoaderManager.Instance.verse;

            IsOpen = true;
            ListBooks();

        }

        private void OnDisable()
        {
            MegaUtils.ClearChildren(bookContent);
            MegaUtils.ClearChildren(chapterContent);
            MegaUtils.ClearChildren(verseContent);

            IsOpen = false;
        }

        public void Go()
        {
            ChapterLoaderManager.Instance.book = currentBook;
            ChapterLoaderManager.Instance.chapter = currentChapter;
            ChapterLoaderManager.Instance.verse = currentVerse;
            ChapterLoaderManager.Instance.Load();

            gameObject.SetActive(false);

            ChapterHistoryManager.Add(new Reference()
            {
                book = currentBook,
                chapter = currentChapter
            });
        }

        public void SelectBook(int book)
        {
            bookSnapper.ScrollToItem(book);
        }

        public void SelectChapter(int chapter)
        {
            chapterSnapper.ScrollToItem(chapter);
        }

        public void SelectVerse(int verse)
        {
            verseSnapper.ScrollToItem(verse);
        }

        public void ListBooks()
        {
            MegaUtils.ClearChildren(bookContent);
            int start = navigationMode is NavigationMode.ALL or NavigationMode.OT ? 1 : 40;
            int end = navigationMode is NavigationMode.ALL or NavigationMode.NT ? 66 : 39;

            booksList ??= new List<BookInListItem>();
            booksList.Clear();

            // create initial gap
            MakeGap(initialBookGap, bookContent);
            for (int i = start; i <= end; i++)
            {
                var book = JsonLoader.bible[i];
                var clone = Instantiate(CDM.GetPrefab<BookInListItem>("book-in-list"), bookContent);
                clone.Set(book.Name, book.book);

                booksList.Add(clone);
            }

            if(sortMode is SortMode.AZ)
            {
                // TODO: Add A-Z Book Sorting
            }

            // scroll to first item in list
            bookSnapper.ScrollToItem(0); // first item is gap, however we are scrolling to gap

            MakeGap(finalBookGap, bookContent);
            ListChapter();
        }

        public void ListChapter()
        {
            MegaUtils.ClearChildren(chapterContent);

            int book = currentBook;
            int chapterCount = JsonLoader.bible[book].Count;
            chapterList ??= new List<ChapterInListItem>();
            chapterList.Clear();

            MakeGap(initialBookGap, chapterContent);
            for (int i = 1; i <= chapterCount; i++)
            {
                var clone = Instantiate(CDM.GetPrefab<ChapterInListItem>("chapter-in-list"), chapterContent);
                clone.Set(i);
                chapterList.Add(clone);
            }

            // scroll to first chapter
            chapterSnapper.ScrollToItem(0); 

            MakeGap(finalBookGap, chapterContent);
            ListVerse();
        }

        public void ListVerse()
        {
            MegaUtils.ClearChildren(verseContent);

            int verseCount = JsonLoader.bible[currentBook][currentChapter].Count;
            verseList ??= new List<ChapterInListItem>();
            verseList.Clear();

            MakeGap(initialBookGap, verseContent);
            for (int i = 1; i <= verseCount; i++)
            {
                var clone = Instantiate(CDM.GetPrefab<ChapterInListItem>("verse-in-list"), verseContent);
                clone.Set(i);
                verseList.Add(clone);
            }

            // scroll to first verse
            verseSnapper.ScrollToItem(0);

            MakeGap(finalBookGap, verseContent);
        }

        public static void MakeGap(float size, Transform content)
        {
            return;

            var gapObj = new GameObject("gap");
            var rect = gapObj.AddComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.SetParent(content);
            rect.localScale = Vector3.up;
            rect.sizeDelta = Vector2.up * size;
        } 
        
        public void SetSelectionLabel()
        {
            currentSelectionLabel.text = $"{JsonLoader.bible[currentBook].Name} {currentChapter}:{currentVerse}";
        }

        public void ClickedBook(int index)
        {
            index += 1;
            if (index == currentBook) return;
            currentBook = index + 1;

            // reset chapter selection
            currentChapter = 1;
            ListChapter();

            SetSelectionLabel();
        }

        public void ClickedChapter(int index)
        {
            index += 1;
            if (index == currentChapter) return;
            currentBook = index;

            // reset verse selection
            currentVerse = 1;
            ListVerse();

            SetSelectionLabel();
        }

        public void ClickedVerse(int index)
        {
            index += 1;
            if (index == currentVerse) return;
            currentVerse = index;

            SetSelectionLabel();
        }

        public void SetSortMode(UIButtonGroup button)
        {
            sortMode = (SortMode)button.Current;
            ListBooks();
        }

        public void SetNavMode(UIButtonGroup button)
        {
            navigationMode = (NavigationMode)button.Current;
            ListBooks();
        }

        public enum NavigationMode
        {
            ALL, OT, NT
        }

        public enum SortMode
        {
            BIBLE, AZ
        }
    }
}