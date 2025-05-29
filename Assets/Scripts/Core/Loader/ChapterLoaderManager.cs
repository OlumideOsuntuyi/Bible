using Core.Display;

using UnityEngine;

using Visuals;
using Visuals.Module;

namespace Core
{
    public class ChapterLoaderManager : Singleton<ChapterLoaderManager>
    {
        public ChapterLoader[] loaders;
        public ChapterPageSwitcher swiper;
        public InsertableText bookLabel;
        public InsertableText module;

        public int chapter
        {
            get
            {
                return current.chapter;
            }
            set
            {
                current.chapter = value;
            }
        }

        public int book
        {
            get
            {
                return current.book;
            }
            set
            {
                current.book = value;
            }
        }

        public ChapterLoader current
        {
            get { return loaders[swiper.active]; }
        }

        public void Load(Reference reference)
        {
            book = reference.book;
            chapter = reference.chapter;
            Load();
        }
        public void Load()
        {
            current.Load();
            LoadAll();
        }

        public void Load(int v)
        {
            current.Load(v);
            LoadAll();
        }

        public void LoadAll()
        {
            foreach(var l in loaders)
            {
                l.book = current.book;
                l.chapter = current.chapter;

                l.verticalScrollBar.value = 1;
            }

            loaders[swiper.Previous].Prev();
            loaders[swiper.Next].Next();


            bookLabel.text = $"{JsonLoader.bible[current.book].Name} {current.chapter}";
            module.text = $"KJV";
        }

        public void PauseVerticalScrolls()
        {
            foreach (var l in loaders)
            {
                l.verticalScrollBar.interactable = false;
                l.verticalScrollBar.StopAllCoroutines();
            }
        }

        public void ReleaseVerticalScrolls()
        {
            foreach (var l in loaders)
            {
                l.verticalScrollBar.interactable = true;
                l.verticalScrollBar.StopAllCoroutines();
            }
        }

        public void OnSwipeLeft()
        {
            LoadAll();
        }

        public void OnSwipeRight()
        {

            LoadAll();
        }

        public void OnStartSwipe()
        {
            foreach(var l in loaders)
            {
                l.verticalScrollBar.enabled = false;
            }
        }

        public void OnEndSwipe()
        {
            foreach (var l in loaders)
            {
                l.verticalScrollBar.enabled = true;
            }
        }
    }
}