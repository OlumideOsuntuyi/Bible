using System;
using System.Collections.Generic;

using Core;

using Dominion;

using LostOasis;

using TMPro;

using Unity.VisualScripting.Antlr3.Runtime;

using UnityEngine;
using UnityEngine.UI;

namespace Visuals
{
    public class NavigationList : Singleton<NavigationList>
    {
        public Transform bookContent;
        public Transform chapterContent;

        public Transform bookCenter;

        public ScollerSnapper bookSnapper;
        public Scrollbar bookScroll;

        public ScollerSnapper chapterSnapper;
        public Scrollbar chapterScroll;

        private Action<int> onScrollCallback;
        private Action<int> onChapterScrollCallback;
        private List<BookInListItem> booksList;
        private List<ChapterInListItem> chapterList;
        public int currentBook;
        public int currentChapter;

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

            IsOpen = true;

            bookSnapper.compare = CompareBook;
            chapterSnapper.compare = CompareChapter;
            ListBooks();

            bookScroll.value = GetBookValue(currentBook);
            chapterScroll.value = GetChapterValue(currentChapter);

        }

        private void OnDisable()
        {
            MegaUtils.ClearChildren(bookContent);
            MegaUtils.ClearChildren(chapterContent);

            IsOpen = false;
        }

        public void Go()
        {
            ChapterLoaderManager.Instance.book = currentBook;
            ChapterLoaderManager.Instance.chapter = currentChapter;
            ChapterLoaderManager.Instance.Load();

            gameObject.SetActive(false);

            ChapterHistoryManager.Add(new Reference()
            {
                book = currentBook,
                chapter = currentChapter
            });
        }

        public int GetCurrentBook()
        {
            float val = 1.0f - bookScroll.value;

            int start = navigationMode is NavigationMode.ALL or NavigationMode.OT ? 1 : 40;
            int end = navigationMode is NavigationMode.ALL or NavigationMode.NT ? 66 : 39;

            float multiplier = 10;
            int bk = Mathf.RoundToInt(Mathf.Lerp(start * multiplier, end * multiplier, val) / multiplier);
            return bk;
        }

        public int GetCurrentChapter()
        {
            float val = 1.0f - chapterScroll.value;

            int start = 1;
            int end = JsonLoader.bible[currentBook].Count;

            float multiplier = 10;
            int ch = Mathf.RoundToInt(Mathf.Lerp(start * multiplier, end * multiplier, val) / multiplier);
            return ch;
        }

        public float GetBookValue(int book)
        {
            int start = navigationMode is NavigationMode.ALL or NavigationMode.OT ? 1 : 40;
            float end = navigationMode is NavigationMode.ALL or NavigationMode.NT ? 66 : 39;
            int count = navigationMode is NavigationMode.ALL ? 66 : navigationMode is NavigationMode.OT ? 39 : 27;

            return Mathf.InverseLerp(1.0f, 0.0f, (book - start) / count);
        }

        public float GetChapterValue(int chapter)
        {
            return Mathf.InverseLerp(1.0f, 0.0f, 1f / JsonLoader.bible[currentBook].Count);
        }


        public void SelectBook(int book)
        {
            return;

            ChapterLoaderManager.Instance.book = book;
            currentBook = book;
            ListBooks();
        }

        public void SelectChapter(int chapter)
        {
            return;

            ChapterLoaderManager.Instance.chapter = chapter;
            currentChapter = chapter;

            onChapterScrollCallback.Invoke(currentChapter);
        }

        public void ListBooks()
        {
            MegaUtils.ClearChildren(bookContent);
            int start = navigationMode is NavigationMode.ALL or NavigationMode.OT ? 1 : 40;
            int end = navigationMode is NavigationMode.ALL or NavigationMode.NT ? 66 : 39;

            booksList ??= new List<BookInListItem>();
            booksList.Clear();

            onScrollCallback = (int current) => { };

            // create initial gap
            MakeGap(initialBookGap, bookContent);
            for (int i = start; i <= end; i++)
            {
                var book = JsonLoader.bible[i];
                var clone = Instantiate(CDM.GetPrefab<BookInListItem>("book-in-list"), bookContent);
                clone.Set(book.Name, book.book);
                onScrollCallback += clone.OnScroll;

                booksList.Add(clone);
            }

            if(sortMode is SortMode.AZ)
            {
                // TODO: Add A-Z Book Sorting
            }

            MakeGap(finalBookGap, bookContent);
            ListChapter(); 
            
            bookScroll.value = GetBookValue(currentBook);
        }

        public void ListChapter()
        {
            MegaUtils.ClearChildren(chapterContent);

            int book = currentBook;
            int chapterCount = JsonLoader.bible[book].Count;
            chapterList ??= new List<ChapterInListItem>();
            chapterList.Clear();

            onChapterScrollCallback = (int value) => { };

            MakeGap(initialBookGap, chapterContent);
            for (int i = 1; i <= chapterCount; i++)
            {
                var clone = Instantiate(CDM.GetPrefab<ChapterInListItem>("chapter-in-list"), chapterContent);
                clone.Set(i);
                chapterList.Add(clone);
                onChapterScrollCallback += clone.OnScroll;
            }

            onChapterScrollCallback.Invoke(currentChapter);

            MakeGap(finalBookGap, chapterContent);
            chapterScroll.value = GetChapterValue(currentChapter);
            SetChapterScrollWheel();
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

        public void SetScrollWheel()
        {
            int current = GetCurrentBook();
            float span = booksList.Count;
            (int i, float d) min = (-1, float.PositiveInfinity);
            foreach (var book in booksList)
            {
                (int index, float comparison) = book.Compare(bookCenter);
                float absC = Mathf.Abs(comparison);
                if(absC < min.d)
                {
                    min = (index, absC);
                }

                book.transform.localEulerAngles = new Vector3()
                {
                    x = comparison * wheelCurvature
                };
            }

            if(min.i != currentBook)
            {
                currentBook = min.i;
                currentChapter = 1;
                ListChapter();
            }


            bookSnapper.snapped = false;
            currentSelectionLabel.text = $"{JsonLoader.bible[currentBook].Name} {currentChapter}";
        }

        public void SetChapterScrollWheel()
        {
            int current = GetCurrentChapter();
            float span = chapterList.Count;
            (int i, float d) min = (-1, float.PositiveInfinity);
            foreach (var chapter in chapterList)
            {
                (int index, float comparison) = chapter.Compare(bookCenter);
                float absC = Mathf.Abs(comparison);
                if (absC < min.d)
                {
                    min = (index, absC);
                }

                chapter.transform.localEulerAngles = new Vector3()
                {
                    x = comparison * wheelCurvature
                };
            }

            if (min.i != currentChapter)
            {
                currentChapter = min.i;
            }

            chapterSnapper.snapped = false;
            currentSelectionLabel.text = $"{JsonLoader.bible[currentBook].Name} {currentChapter}";
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

        public (int, float) CompareBook(Transform center)
        {
            if(booksList.Count == 0)
            {
                return (0, 0);
            }

            int start = navigationMode is NavigationMode.ALL or NavigationMode.OT ? 1 : 40;
            return booksList[currentBook - start].Compare(center);
        }
        public (int, float) CompareChapter(Transform center)
        {
            if (chapterList.Count == 0)
            {
                return (0, 0);
            }

            return chapterList[currentChapter - 1].Compare(center);
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