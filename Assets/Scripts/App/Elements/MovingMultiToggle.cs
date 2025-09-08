using TLab.UI.SDF;

using UnityEngine;
using UnityEngine.UI;

namespace Visuals
{
    [ExecuteInEditMode]
    public class MovingMultiToggle : MonoBehaviour
    {
        public Graphic bar;
        public RectTransform[] targets;
        public int active;
        public float transitionDuration;
        public bool useUnscaledDeltaTime;
        [Tooltip("This controls direction of bar movement")] public Vector2 movementScale;
        public Vector2 offset;

        [Header("UI Settings")]
        public Gradient gradient;
        public bool evaluateGradient;


        [Header("Debug")]
        [SerializeField] private bool test;
        [SerializeField] private float _time;
        [SerializeField] private int lastIndex;


        private void Update()
        {
            if (_time > transitionDuration || targets.Length == 0) return;

            _time += useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float range = _time / Mathf.Max(float.Epsilon, transitionDuration);
            bar.rectTransform.anchoredPosition = Vector2.Scale(movementScale, Vector2.Lerp(targets[lastIndex].anchoredPosition, targets[active].anchoredPosition, range)) + offset;

            if(evaluateGradient)
            {
                float count = targets.Length;
                float preTime = (lastIndex + 1) / count;
                float currTime = (active + 1) / count;


                Color currentColor = gradient.Evaluate(Mathf.Lerp(preTime, currTime, range));
                if(bar is SDFQuad quad)
                {
                    quad.fillColor = currentColor;
                }
                else
                {
                    bar.color = currentColor;
                }
            }
        }

        public void Set(int active)
        {
            _time = 0;
            lastIndex = this.active;
            this.active = active;
        }
    }
}