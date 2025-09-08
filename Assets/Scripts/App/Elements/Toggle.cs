using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Visuals
{
    [ExecuteInEditMode]
    public class Toggle : MonoBehaviour
    {
        public Image handle;
        public Image background;

        public Gradient handleColor;
        public Gradient backgroundColor;

        public float duration = 0.7f;
        public bool transitionColors = false;

        [Header("Handle Position")]
        public Vector3 handlePositionOff;
        public Vector2 handleMinAnchorOff;
        public Vector2 handleMaxAnchorOff;
        public Vector2 handlePivotOff;

        public Vector3 handlePositionOn;
        public Vector2 handleMinAnchorOn;
        public Vector2 handleMaxAnchorOn;
        public Vector2 handlePivotOn;
        public bool transitionHandlePosition;

        [SerializeField] private bool state;
        [SerializeField] private float time;

        public UnityEvent onValueChanged;

        private bool _state;

        public bool State
        {
            get { return _state; }
        }

        public bool InTransition
        {
            get
            {
                return (state && time < duration) || (!state && time > 0);
            }
        }

        public int direction
        {
            get { return state ? 1 : -1; }
        }

        private void Update()
        {
            if (_state == state) return;
            if (!InTransition)
            {
                _state = state;
                onValueChanged.Invoke();
                return;
            }

            time = Mathf.Clamp(time + (Time.deltaTime * direction), 0, duration);
            float range = time / Mathf.Max(duration, float.Epsilon);

            Color handleColor = this.handleColor.Evaluate(range);
            Color backgroundColor = this.backgroundColor.Evaluate(range);

            Vector2 handleAnchorMin = Vector2.Lerp(handleMinAnchorOff, handleMinAnchorOn, range);
            Vector2 handleAnchorMax = Vector2.Lerp(handleMaxAnchorOff, handleMaxAnchorOn, range);
            Vector2 handlePivot = Vector2.Lerp(handlePivotOff, handlePivotOn, range);
            Vector2 handlePosition = Vector3.Lerp(handlePositionOff, handlePositionOn, range);

            var handleRect = handle.rectTransform;
            handleRect.anchorMin = handleAnchorMin;
            handleRect.anchorMax = handleAnchorMax;

            handleRect.pivot = handlePivot;
            handle.color = handleColor;
            background.color = backgroundColor;

        }
        public void Flick()
        {
            if (!(time == 0 || time == duration)) return;
            state = !state;
        }

    }
}
