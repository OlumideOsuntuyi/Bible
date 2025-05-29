using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LostOasis;

using UnityEngine;

namespace Core
{
    public class HolyBible
    {
        public readonly string language;
        private readonly List<Book> books;
        private readonly Dictionary<string, int> bookMap;

        private List<string[]> shortNames = new()
        {
            new string[]{"Gen", "genesis" }
        };

        public Book this[int book]
        {
            get { return books[book - 1]; }
        }
        public Book this[string book]
        {
            get { return books[bookMap[book.MegaTrim()]]; }
        }

        public HolyBible(string name, string shortname, string language="english")
        {
            books = new List<Book>();
            bookMap = new Dictionary<string, int>();
            this.language = language;
        }

        public void Load(JVerse[] verses)
        {
            Book lastBook = null;
            Chapter lastChapter = null;
            string state;
            int i = 0;
            foreach(var verse in verses)
            {
                if (books.Count < verse.book)
                {
                    lastBook = new Book(verse.book, verse.book_name);
                    books.Add(lastBook);
                    bookMap.Add(verse.book_name, verse.book);
                }

                if (verse.verse == 1)
                {
                    lastChapter = new Chapter(verse.book, verse.chapter);
                    lastBook.Push(lastChapter);
                }

                var v = new Verse(verse.book, verse.chapter, verse.verse, verse.text, i);
                state = lastChapter.Push(v);
                if(state != "success")
                {
                    Debug.Log(state);
                }

                i++;
            }
        }

        public (int book, int chapter, int verseStart, int verseEnd, bool success) Parse(string reference)
        {
            int book = 0;
            int chapter = 0;
            int verseStart = 0;
            int verseEnd = 0;
            bool success = false;

            const string pattern = @"^([1-3]?\s*[A-Za-z]+)\s+(\d+):(\d+)(?:-(\d+))?$";

            Match match = Regex.Match(reference, pattern);

            if (match.Success)
            {
                book = FindBookIndex(match.Groups[1].Value);
                chapter = int.Parse(match.Groups[2].Value);
                verseStart = int.Parse(match.Groups[3].Value);
                verseEnd = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0;
            }
            return (book, chapter, verseStart, verseEnd, success);
        }

        public string Parse(int book, int chapter, int verseStart, int verseEnd=0)
        {
            string bk = this[book].Name;
            string vs = verseStart.ToString();

            if(verseEnd > 0)
            {
                vs += $"-{verseEnd}";
            }

            return $"{bk} {chapter}:{vs}";
        }

        public int FindBookIndex(string name)
        {
            name = name.MegaTrim();
            foreach(var b in books)
            {
                if(name == b.Name.MegaTrim())
                {
                    return b.book;
                }
                else
                {
                    foreach(var sn in b.names)
                    {
                        if(sn.MegaTrim() == name)
                        {
                            return b.book;
                        }
                    }
                }
            }

            return 0;
        }
    }
}