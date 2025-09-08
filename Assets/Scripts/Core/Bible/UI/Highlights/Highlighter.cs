using System;
using System.Collections.Generic;
using System.IO;

using Core.Module;

using UnityEngine;

using Visuals;

namespace Core.Notes
{
    [DefaultExecutionOrder(-100)]
    public class Highlighter : Singleton<Highlighter>
    {
        public ColorSwatch[] swatches;

        private static int highlightsCount;
        public static int Count
        {
            get 
            { 
                if(countChanged)
                {
                    Count = GetAll().Count;
                }
                return highlightsCount; 
            }
            set { highlightsCount = value; countChanged = false; }
        }
        private static Dictionary<Reference, HighlightData> highlights;
        private static bool countChanged;

        string PATH => Path.Combine(Application.persistentDataPath, "Highlights.bin");

        private void Awake()
        {
            highlights = DictionarySerializer<Reference, HighlightData>.LoadFromFile(PATH);
            highlights ??= new Dictionary<Reference, HighlightData>();

            Count = GetAll().Count;
        }

        private void OnDestroy()
        {
            var sr = new DictionarySerializer<Reference, HighlightData>(highlights);
            sr.SaveToFile(PATH);
        }

        

        public void HighlightedSelectedVerses(int swatch)
        {
            var curr = ChapterLoaderManager.Instance.current;
            List<InsertableVerse> marked = new();

            int id = swatch < 0 ? 0 : GetID();
            foreach (var v in curr.verses)
            {
                if(v.marked)
                {
                    if (!highlights.ContainsKey(v.reference))
                    {
                        highlights.Add(v.reference, default);
                    }

                    if (swatch < 0)
                    {
                        highlights.Remove(v.reference);
                        return;
                    }

                    highlights[v.reference] = new HighlightData(swatch, id, marked.Count);

                    marked.Add(v);
                    v.Reload();
                }
            }

            countChanged = true;
        }

        public void CommentOnVerse()
        {
            var curr = ChapterLoaderManager.Instance.current;
            List<InsertableVerse> marked = new();

            foreach (var v in curr.verses)
            {
                if (v.marked)
                {
                    if (marked.Count > 0) return;
                    marked.Add(v);
                }
            }
        }

        public void StopWritingVerseComment()
        {

        }

        public static Color Get(Reference reference)
        {
            if(!highlights.TryGetValue(reference, out var data) || data.index < 0)
            {
                return ThemeManager.Colors[(int)ThemeColorType.Text];
            }

            return Instance.swatches[data.index].image.color;
        }


        public static Color GetData(Reference reference, out DateTime date)
        {
            if (!highlights.TryGetValue(reference, out var data) || data.index < 0)
            {
                date = System.DateTime.Now;
                return ThemeManager.Colors[(int)ThemeColorType.Text];
            }

            date = DateTime.Parse(data.date);
            return Instance.swatches[data.index].image.color;
        }

        public static List<KeyValuePair<Reference, HighlightData>> GetAll()
        {
            var kvp = new List<KeyValuePair<Reference, HighlightData>>(highlights);

            List<KeyValuePair<Reference, HighlightData>> list = new();
            for (int i = 0; i < kvp.Count; i++)
            {
                bool found = false;
                var k = kvp[i];
                var r = k.Key;

                for (int j = 0; j < kvp.Count; j++)
                {
                    if (i == j) continue;
                    var jKvp = kvp[j];
                    if (jKvp.Value.id == k.Value.id)
                    {
                        int a = r.verse;
                        int c = jKvp.Key.verse;

                        r.verse = Mathf.Min(a, c);
                        r.verseEnd = Mathf.Max(a, c);

                        found = k.Value.inID != 0;
                    }
                }

                if(!found)
                {
                    list.Add(new KeyValuePair<Reference, HighlightData>(r, k.Value));
                }
            }

            return list;
        }

        public void ToggleUnderline()
        {
            foreach(var s in swatches)
            {
                s.ChangeMode(!s.underlineMode);
            }
        }

        private static int GetID()
        {
            int id = 0;
            bool In()
            {
                foreach(var v in highlights.Values)
                {
                    if (v.id == id) return true;
                }

                return false;
            }

            while(id == 0 || In())
            {
                id = MegaUtils.Random.Next(int.MinValue, int.MaxValue);
            }

            return id;
        }

        [System.Serializable]
        public struct HighlightData
        {
            public int index;
            public int id;
            public int inID;

            public string date;

            public HighlightData(int index, int id, int inID)
            {
                this.index = index;
                this.inID = inID;
                this.id = id;
                date = System.DateTime.Now.ToString();
            }
        }
    }
}