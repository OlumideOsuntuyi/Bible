using Core.Display;
using Core.Notes;

using UnityEngine;
using UnityEngine.EventSystems;

using Visuals;
using Visuals.Module;

namespace Core.Module
{
    public class InsertableVerse : InsertableText, IPointerDownHandler, IPointerUpHandler
    {
        public bool held;
        public bool marked;
        public float time = 0;
        public Reference reference;

        private Verse verse;

        public const float HOLD_TIME = 0.5f;

        private void Update()
        {
            if (!held) return;

            time += Time.deltaTime;
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if(held)
            {
                return;
            }


            held = true;
            time = 0;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (marked)
            {
                Unmark();
                return;
            }

            if (time > HOLD_TIME)
            {
                Mark();
            }

            time = 0;
            held = false;
        }

        public void SetVerse(Verse verse)
        {
            this.verse = verse;

            string label = ChapterLoader.InsertColor($"({verse.verse})", ThemeManager.Colors[(int)ThemeColorType.VerseLabelColor]);
            _text.color = Highlighter.Get(reference);
            label = ChapterLoader.InsertFontSize(label, _text.fontSize * 0.75f);

            if(marked)
            {
                text = $"{label} <mark=#{ColorUtility.ToHtmlStringRGB(NoteEditor.GetMarkColor())}>{verse.text}</mark>";
            }
            else
            {
                text = $"{label} {verse.text}";
            }
        }

        public void Reload()
        {
            SetVerse(verse);
        }

        public void Mark()
        {
            marked = true;
            Reload();
            ScreenManager.Transition("toolbar", "highlighter");
        }

        public void Unmark()
        {
            marked = false;
            Reload();
            ScreenManager.Transition("toolbar", "main");
        }
    }
}