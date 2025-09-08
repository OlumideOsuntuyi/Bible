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

        public int Active => _active;

        private void Awake()
        {
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].Set(i == active ? 1 : 0, i == active);
            }

            time = 0;
        }

        private void Update()
        {
            if (pages.Count == 0) return;
            if (active == _active) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Instant();
                return;
            }
#endif         
            time += Time.deltaTime;
            float range = time / Mathf.Max(transitionTime * 0.5f, float.Epsilon);
            if (range <= 1)
            {
                pages[_active].Set(1.0f - range, false);
            }
            else if(range <= 2)
            {
                range -= 1.0f;
                pages[active].Set(range, true);
            }else if (range > 2)
            {
                _active = active;
                time = 0;
                Instant();
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
            }

            this.active = active;
            time = 0;
        }

        public void Instant()
        {
            pages[_active].Set(0, false);
            pages[active].Set(1, true);
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

            public void Set(float range, bool active)
            {
                rect.anchorMin = Vector2.Lerp(anchorMinHidden, anchorMinVisible, range);
                rect.anchorMax = Vector2.Lerp(anchorMaxHidden, anchorMaxVisible, range);
                rect.pivot = Vector2.Lerp(pivorHidden, pivorVisible, range);

                rect.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, range);


                if (activeMatchesRange)
                {
                    active = active ? range > 0 : range < 0.02f;
                    rect.gameObject.SetActive(active);
                }
            }
        }
    }
}