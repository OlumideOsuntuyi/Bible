using System.Collections.Generic;

namespace Core
{
    public class Book
    {
        public string Name;
        public string[] names;

        public int book;
        private List<Chapter> chapters;

        public int Count
        {
            get { return chapters.Count; }
        }

        public Chapter this[int chapter]
        {
            get
            {
                return chapters[chapter - 1];
            }
        }

        public Book(int book, string name)
        {
            this.book = book;
            this.Name = name;
            this.chapters = new List<Chapter>();
        }


        /// <summary>
        /// Add chapter to book
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns>0=success, 1=repeat, 2=next</returns>
        public int Push(Chapter chapter)
        {
            if(chapter.book != book)
            {
                // pushing wrong book's chapter
                return -2;
            }

            if(chapter.chapter <= Count)
            {
                // pushing pre-existing chapters
                return -1;
            }

            chapters.Add(chapter);
            return 0;
        }
    }
}