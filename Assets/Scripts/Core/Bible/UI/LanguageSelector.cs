using Core;

using TMPro;

using UnityEngine;

namespace Visuals
{
    public class LanguageSelector : MonoBehaviour
    {
        public TMP_Text label;
        public TMP_Text language;

        private string path;
        private string lang;

        public void Set(string item, string lang, string path, TMP_FontAsset font)
        {
            label.text = item;
            language.text = lang;

            this.path = path;
            this.lang = lang;

            if(font != null)
            {
                label.font = font;
            }
        }

        public async void Load()
        {
            await LoadLanguages.LoadBible(path, lang);
        }
    }
}