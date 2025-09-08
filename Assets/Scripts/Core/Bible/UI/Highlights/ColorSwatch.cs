using Core.Notes;

using UnityEngine;
using UnityEngine.UI;

namespace Visuals
{
    public class ColorSwatch : MonoBehaviour
    {
        public int index;
        public Image image;
        public Sprite[] icons;
        public Vector2[] sizes;
        public bool underlineMode;

        public void Set(Color color)
        {

        }

        public void Select()
        {
            if(underlineMode)
            {

            }
            else
            {
                Highlighter.Instance.HighlightedSelectedVerses(index);
            }
        }

        public void ChangeMode(bool state=false)
        {
            underlineMode = state;
            image.sprite = icons[state ? 1 : 0];
            image.rectTransform.sizeDelta = sizes[state ? 1 : 0];
        }
    }
}