using UnityEngine;
using UnityEngine.Events;

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

        [Header("Events")]
        public UnityEvent OnToggle;
        public UnityEvent OnExtend;
        public UnityEvent OnExtendCompleted;
        public UnityEvent OnRetract;
        public UnityEvent OnRetractCompleted;
        public UnityEvent<float> OnRange;


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

            OnRange.Invoke(r);

            rect.sizeDelta = Vector2.Lerp(min, max, r);

            if(_time > duration) // on completed
            {
                if (on) OnExtendCompleted.Invoke();
                else OnRetractCompleted.Invoke();
            }
        }

        public void On()
        {
            on = true;
            _time = 0;
            OnExtend.Invoke();
        }

        public void Off()
        {
            on = false;
            _time = 0;
            OnRetract.Invoke();
        }

        public void Toggle()
        {
            on = !on;
            _time = 0;

            OnToggle.Invoke();
        }
    }
}