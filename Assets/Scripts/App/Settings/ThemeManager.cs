using System;
using System.Collections.Generic;

using UnityEngine;

namespace Visuals
{
    [DefaultExecutionOrder(-99)]
    public class ThemeManager : Singleton<ThemeManager>
    {
        public static Color[] Colors
        {
            get
            {
                return Instance.themes[Instance.activeTheme].colors;
            }
        }
        [SerializeField] private List<Theme> themes;
        public int activeTheme;

        private static Action<Color>[] SetColors { get; set; }
        private void Awake()
        {
            SetColors = new Action<Color>[32];
        }

        private void Start()
        {
            OnThemeChanged();
        }


        public void OnThemeChanged()
        {
            for (int i = 0; i < SetColors.Length; i++)
            {
                SetColors[i]?.Invoke(themes[activeTheme].colors[i]);
            }

            Camera.main.backgroundColor = themes[activeTheme].colors[(int)ThemeColorType.Background];
        }

        public static void Subscribe(Action<Color> callback, ThemeColorType type)
        {
            SetColors[(int)type] += callback;
        }

        public static void UnSubscribe(Action<Color> callback, ThemeColorType type)
        {
            SetColors[(int)type] -= callback;
        }


        public void ToggleDarkMode(Toggle toggle)
        {
            activeTheme = toggle.State ? 0 : 1;
            OnThemeChanged();
        }
    }

    public enum ThemeColorType
    {
        Active, Text, ActiveText, DimText, Background, VerseLabelColor, Background2, TextInActive 
    }

    [System.Serializable]
    public class Theme
    {
        public string name;
        public Color[] colors;
    }
}