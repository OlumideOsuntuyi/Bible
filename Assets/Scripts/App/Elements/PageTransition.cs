using System.Collections.Generic;

using UnityEngine;

namespace Visuals
{
    [ExecuteAlways]
    public class PageTransition : MonoBehaviour
    {
        public string label;
        [SerializeField] private List<Page> pages;
        [SerializeField] private int active;

        public float transitionTime = 0.3f;

        [SerializeField] private int _active;
        [SerializeField] private float time;

        private void Awake()
        {
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].Set(i == active ? 1 : 0);
            }

            time = 0;
        }

        private void Update()
        {
            if (pages.Count == 0) return;
            if (active == _active) return;

            time += Time.deltaTime;
            float range = Mathf.Clamp01(time / Mathf.Max(transitionTime, float.Epsilon));
#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                Instant();
                return;
            }
#endif
            if(range <= 0.5f)
            {
                pages[_active].Set(1.0f - (range * 2.0f));
            }else if(range > 0.5f && range <= 1)
            {
                pages[active].Set((range - 0.5f) * 2.0f);
            }else if (range > 1)
            {
                _active = active;
                time = 0;
            }
        }

        public void SetActive(string name)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if(name == pages[i].name)
                {
                    SetActive(i);
                    return;
                }
            }
        }

        public void SetActive(int active)
        {
            if (active == this.active) return;
            if(_active != this.active && _active != active)
            {
                Instant();
                return;
            }else if(_active == active)
            {
                _active = this.active;
                this.active = active;
                return;
            }

            this.active = active;
            time = 0;
        }

        public void Instant()
        {
            pages[_active].Set(0);
            pages[active].Set(1);
            _active = this.active;

            time = 0;
        }

        [System.Serializable]
        public class Page
        {
            public string name;
            public RectTransform rect;

            [Header("Hiden")]
            public Vector3 hiddenPosition;
            public Vector2 anchorMinHidden;
            public Vector2 anchorMaxHidden;
            public Vector2 pivorHidden;


            [Header("Visible")]
            public Vector3 visiblePosition;
            public Vector2 anchorMinVisible;
            public Vector2 anchorMaxVisible;
            public Vector2 pivorVisible;

            public bool activeMatchesRange;

            public void Set(float range)
            {
                rect.anchorMin = Vector2.Lerp(anchorMinHidden, anchorMinVisible, range);
                rect.anchorMax = Vector2.Lerp(anchorMaxHidden, anchorMaxVisible, range);
                rect.pivot = Vector2.Lerp(pivorHidden, pivorVisible, range);

                rect.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, range);

                if(activeMatchesRange && range is <= 0 or >= 1)
                {
                    rect.gameObject.SetActive(range >= 1);
                }
            }
        }
    }
}