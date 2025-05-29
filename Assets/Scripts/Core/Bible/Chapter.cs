using System.Collections.Generic;

namespace Core
{
    public class Chapter
    {
        public int book;
        public int chapter;
        private List<Verse> verses;

        public int Count
        {
            get { return verses.Count; }
        }

        public Verse this[int verse]
        {
            get
            {
                return verses[verse - 1];
            }
        }

        public Chapter(int book, int chapter)
        {
            this.book = book;
            this.chapter = chapter;
            this.verses = new List<Verse>(); 
        }


        /// <summary>
        /// Add verse to chapter
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns>0=success, 1=repeat, 2=next-chapter, 3=next-book</returns>
        public string Push(Verse verse)
        {
            if (verse.book != book)
            {
                // pushing wrong book's verse
                return "invalid book";
            }

            if(verse.chapter != chapter)
            {
                // pushing wrong chapter's verse
                return "invalid chapter";
            }

            if (verse.verse <= Count)
            {
                // pushing pre-existing verse
                return "already added verse";
            }

            verses.Add(verse);
            return "success";
        }
    }
}