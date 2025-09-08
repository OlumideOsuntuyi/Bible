using Core;

using TLab.UI.SDF;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Visuals
{
    public class NavV2Item : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// Book = 0, Chapter = 1, Verse = 2
        /// </summary>
        public int type;

        /// <summary>
        /// Add Event Listener
        /// </summary>
        public UnityEvent onClickEvent;

        public Color[] labelColor;
        public TMP_Text label;
        public Graphic background;

        private int index;

        public void Set(int index, int type)
        {
            this.index = index;
            this.type = type;
            if (type == 0)
            {
                label.text = $"{Book.ABBREVIATIONS[index - 1].Abbrev}"; // index starts from 1 for genesis
                background.color = labelColor[Color(index)];
            }
            else
            {
                label.text = $"{index}";
            }

            onClickEvent.AddListener(Select);
        }

        public int Color(int index)
        {
            if (index <= 5) return 0; // mosaic
            else if (index <= 17) return 1; // israel
            else if (index <= 22) return 2; // wise books
            else if (index < 27) return 3; // major prophets
            else if (index <= 39) return 4; // minor prophets
            else if (index <= 43) return 0; // gospels
            else if (index <= 44) return 1; // acts of the apostles
            else if (index <= 53) return 2; // epistles to churches
            else if (index <= 57) return 3; // epistles to people
            else return 4; // hebrews, apostolic letters, revelations
        }

        public void Select()
        {
            switch(type)
            {
                case 0: SelectBook(); break;
                case 1: SelectChapter(); break;
                case 2: SelectVerse(); break;
                default:break;
            }
        }

        private void SelectBook()
        {
            NavigationListV2.Instance.SelectBook(index);  
        }

        private void SelectChapter()
        {
            NavigationListV2.Instance.SelectChapter(index);
        }

        private void SelectVerse()
        {
            NavigationListV2.Instance.SelectVerse(index);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClickEvent.Invoke();
        }
    }
}