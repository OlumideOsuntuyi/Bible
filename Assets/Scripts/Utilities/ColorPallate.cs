using System.Collections.Generic;

using LostOasis;

using UnityEngine;

namespace Visuals
{
    [DefaultExecutionOrder(-200)]
    public class ColorPallate : Singleton<ColorPallate>
    {
        [SerializeField] private ColorTag[] colors;
        private static Dictionary<string, Color> colorTagMap;
        private static Dictionary<string, string> HTMLMap;

        private void OnEnable()
        {
            colorTagMap = new();
            HTMLMap = new Dictionary<string, string>();
            foreach (var c in colors)
            {
                colorTagMap.Add(c.tag, c.color);
                HTMLMap.Add(c.tag, ColorUtility.ToHtmlStringRGB(c.color));
            }
        }

        public static Color Get(int index)
        {
            return Instance.colors[index].color;
        }

        public static Color Get(string tag)
        {
            return colorTagMap[tag];
        }

        public static string GetHTML(string tag)
        {
            return HTMLMap[tag];
        }

        [System.Serializable]
        private struct ColorTag
        {
            public string tag;
            public Color color;
        }
    }
}