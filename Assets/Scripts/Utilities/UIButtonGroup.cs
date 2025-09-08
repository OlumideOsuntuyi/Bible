using System;
using System.Collections.Generic;

using TLab.UI.SDF;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostOasis
{

    [ExecuteInEditMode]
    public class UIButtonGroup : MonoBehaviour
    {
        public List<Graphic> buttons;
        public List<GraphicGroup> graphicGroups;

        public Color activeColor;
        public Color inactiveColor;
        public int Current;

        public UnityEvent onValueChanged;
        public UnityEvent<int> onSelected;

        private int prev;

        public int MAX => (buttons == null || buttons.Count == 0) ? (graphicGroups == null || graphicGroups.Count == 0) ? -1 : graphicGroups.Count - 1 : buttons.Count - 1;
        private void OnEnable()
        {
            prev = -1;
        }
        private void Update()
        {
            Current = Mathf.Clamp(Current, 0, MAX);
            if (prev == Current) return;

            prev = Current;

            if(buttons != null)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    var currentColor = i == Current ? activeColor : inactiveColor;
                    var bar = buttons[i];
                    if (bar is SDFQuad quad)
                    {
                        quad.fillColor = currentColor;
                    }
                    else
                    {
                        bar.color = currentColor;
                    }
                }
            }

            if(graphicGroups != null)
            {
                for (int i = 0; i < graphicGroups.Count; i++)
                {
                    foreach (var graphic in graphicGroups[i].graphics)
                    {
                        var currentColor = i == Current ? activeColor : inactiveColor;
                        if (graphic is SDFQuad quad)
                        {
                            quad.fillColor = currentColor;
                        }
                        else
                        {
                            graphic.color = currentColor;
                        }
                    }
                }
            }

            onValueChanged.Invoke();
            onSelected.Invoke(Current);
        }

        public void Click(Image image)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == image)
                {
                    Current = i;
                    break;
                }
            }
        }
        public void Click(int index)
        {
            Current = Mathf.Clamp(index, 0, MAX);
        }
        public void Next()
        {
            Click((Current + 1) % MAX);
        }
        public void Prev()
        {
            int ind = Current - 1;
            if (ind < 0) ind = MAX;
            Click(ind);
        }

        public void SetToScreenControl(ScreenControl ctrl)
        {
            Click(ctrl.active_box);
        }

        [System.Serializable]
        public class GraphicGroup
        {
            public Graphic[] graphics;
        }
    }
}