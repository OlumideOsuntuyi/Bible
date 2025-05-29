using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Core
{
    public static class Highlights
    {
        private static List<Highlight> highlights;


        private static Dictionary<(int, int, int), int> map;

        static Highlights()
        {
            string path = Path.Combine(Application.persistentDataPath, "highlights.bin");
            highlights = FileHandler.LoadObject<List<Highlight>>(path);
            OnLoad();
        }

        public static void OnLoad()
        {
            highlights ??= new List<Highlight>();
            map = new Dictionary<(int, int, int), int>();
            foreach(var h in highlights)
            {
                map.Add((h.book, h.chapter, h.verse), map.Count);
            }
        }

        public static bool Contains(Highlight h)
        {
            return map.ContainsKey((h.book, h.chapter, h.verse));
        }

        private static void Add(Highlight h)
        {
            highlights.Add(h);
            map.Add((h.book, h.chapter, h.verse), map.Count);
        }

        public static void Highlight(string reference)
        {
            var parsed = JsonLoader.bible.Parse(reference);
            for (int i = parsed.verseStart; i < parsed.verseEnd + 1; i++)
            {
                Highlight h = new()
                {
                    book = parsed.book,
                    chapter = parsed.chapter,
                    verse = i
                };

                if(Contains(h))
                {
                    continue;
                }

                Add(h);
            }
        }
    }

    [System.Serializable]
    public struct Highlight
    {
        public int book;
        public int chapter;
        public int verse;
        public int highlight;
    }
}