using System.Xml;

using Core.Module;

using TMPro;

using UnityEngine;

namespace Visuals.Module
{
    [ExecuteAlways]
    public class InsertableText : Insertable
    {
        public TMP_Text _text;
        public Vector2 padding;
        public Vector2 min;
        public Vector2 max;
        public float minLength;
        public float fontRatio;
        public Mode mode;

        public enum Mode
        {
            All, Width, Height, None
        }

        public string text
        {
            get
            {
                return _text.text;
            }

            set
            {
                _text.text = value;
                Resize();
            }
        }

        public override void Set(Value value)
        {
            text = value.ToString();
        }

        public override void SetFontSize(float size)
        {
            _text.fontSize = size;
            Resize();
        }

        public void Resize()
        {
            if (mode is Mode.None) return;

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            //int textLength = text.Length;
            //float span = Mathf.Max(minLength, textLength * fontRatio * _text.fontSize);
            //rect.sizeDelta = new Vector2()
            //{
            //    x = span,
            //    y = rect.sizeDelta.y
            //} + padding;

            _text.ForceMeshUpdate();

            // Get preferred width
            var w = _text.GetPreferredValues();
            var s = rect.sizeDelta;
            var sz = new Vector2()
            {
                x = mode is Mode.All or Mode.Width ? w.x : s.x,
                y = mode is Mode.All or Mode.Height ? w.y : s.y
            };

            if(min.x != 0 || min.y != 0)
            {
                sz = Vector2.Max(min, sz);
            }

            if(max.x != 0 || max.y != 0)
            {
                sz = Vector2.Min(max, sz);
            }

            sz += padding;

            rect.sizeDelta = sz;
            // _text.ForceMeshUpdate();
            if (rect.parent)
            {
                rect.parent.SendMessageUpwards(nameof(OnTransformChildrenChanged), SendMessageOptions.DontRequireReceiver);
            }
        }

        protected override void OnEnable()
        {
            Resize();
        }

        protected void OnTransformChildrenChanged()
        {
        }
    }
}