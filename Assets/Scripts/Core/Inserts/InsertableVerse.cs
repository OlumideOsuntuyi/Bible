using UnityEngine;
using UnityEngine.EventSystems;

using Visuals;
using Visuals.Module;

namespace Core.Module
{
    public class InsertableVerse : InsertableText, IPointerDownHandler, IPointerUpHandler
    {
        string pre_mark_text = "";
        bool held;
        bool marked;
        float time = 0;

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

        public void Mark()
        {
            pre_mark_text = _text.text;
            string[] strs = _text.text.Split(')');
            _text.text = $"{strs[0]})<mark=#{ColorUtility.ToHtmlStringRGBA(ColorPallate.Get("verse-highlight"))}>{strs[1]}</mark>";
            marked = true;
        }

        public void Unmark()
        {
            _text.text = pre_mark_text;
            pre_mark_text = null;
            marked = false;
        }
    }
}