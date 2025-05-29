using System.Text.RegularExpressions;

using Visuals;
using Core.Display;
using System.Collections.Generic;

namespace Core
{
    public class Verse
    {
        const string pattern = @"‹([^‹›]+)›";

        public int book;
        public int chapter;
        public int verse;
        public string text;

        public int index;
        public Verse(int book, int chapter, int verse, string text, int index)
        {
            this.book = book;
            this.chapter = chapter;
            this.verse = verse;

            this.text = book >= 40 ? Regex.Replace(text, pattern, match =>
            {
                string content = match.Groups[1].Value;
                return ChapterLoader.InsertColor(content, ColorPallate.GetHTML("words-of-christ"));
            }) : text;

            this.index = index;
        }
    }
}