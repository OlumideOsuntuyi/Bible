using UnityEngine;

namespace Visuals
{
    [ExecuteInEditMode]
    public class RectSizeInterpolator : MonoBehaviour
    {
        public RectTransform rect;
        public Vector2 min;
        public Vector2 max;
        public float duration;
        public bool on;


        [Header("Debug")]
        [SerializeField] private float _time;

        private void Update()
        {
            if (!rect || _time > duration) return;
            _time += Time.deltaTime;
            float r = _time / duration;
            if (!on)
            {
                r = 1.0f - r;
            }

            rect.sizeDelta = Vector2.Lerp(min, max, r);
        }

        public void On()
        {
            on = true;
            _time = 0;
        }

        public void Off()
        {
            on = false;
            _time = 0;
        }

        public void Toggle()
        {
            on = !on;
            _time = 0;
        }
    }
}