using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostOasis
{
    [ExecuteInEditMode]
    public class MultiBubbleProgressBar : MonoBehaviour
    {
        public float range { get; private set; }
        public Transform parent;
        public List<Image> bubbles;
        public Gradient color;
        public float min, max;
        public TMP_Text label;
        public bool useLabel;
        public float currentValue;
        public float blendSpeed;
        public bool doSmoothLerp;
        public bool useSingleColor;

        [SerializeField] private float previousValue;


        private void Awake()
        {
            previousValue = -1;
            if(parent && (bubbles == null || bubbles.Count == 0))
            {
                bubbles = parent.GetComponentsInChildren<Image>().ToList();
            }
        }
        void Update()
        {
            if (previousValue != currentValue && bubbles.Count > 0)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            previousValue = currentValue;
            float newrange = Mathf.InverseLerp(min, max, currentValue);
            range = doSmoothLerp ? Mathf.Lerp(range, newrange, Time.deltaTime * blendSpeed) : newrange;

            float ratio = 1.0f / bubbles.Count;
            Color singleColor = color.Evaluate(range);
            for (int i = 0; i < bubbles.Count; i++)
            {
                float lower_bound = (i) * ratio;
                float upper_bound = (i + 1) * ratio;
                float specific_range = Mathf.InverseLerp(lower_bound, upper_bound, range);
                bubbles[i].fillAmount = specific_range;
                bubbles[i].color = useSingleColor ? singleColor : color.Evaluate(Mathf.Min(upper_bound, range));
            }

            if (useLabel && label)
            {
                label.text = $"{Mathf.Round(range * 100)}%";
            }
        }

        public void AcceptSlider(Slider slider)
        {
            currentValue = slider.value;
        }
    }
}