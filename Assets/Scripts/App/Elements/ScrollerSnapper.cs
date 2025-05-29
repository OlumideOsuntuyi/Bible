using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System;
namespace Visuals
{
    public class ScollerSnapper : MonoBehaviour
    {
        public RectTransform content;
        public RectTransform viewport;
        public RectTransform center;
        public Scrollbar scrollbar;
        public ScrollRect rect;

        public float freeScrollTime = 0.3f;
        public float snapVelocity = 0.6f;
        public float gap = 100f;
        public float distanceOffset = 0;
        public float thresh = 10;


        [SerializeField] private Vector2 velocity;

        [SerializeField] private float distance;
        [SerializeField] private float noScrollTime;
        [SerializeField] private bool snapping;
        [SerializeField] public bool snapped;

        public Func<Transform, (int index, float distance)> compare;
        private float height;

        private void LateUpdate()
        {
            if (snapped) return;
            velocity = rect.velocity;

            noScrollTime += Time.deltaTime;
            if (noScrollTime <= Time.deltaTime)
            {
                return;
            }

            // if stopped scrolling
            if (noScrollTime < freeScrollTime)
            {

            }
            else
            {

                if(!snapping)
                {
                    snapping = true;
                    snapped = false;
                }

                var compareResults = compare.Invoke(center);
                distance = compareResults.distance + distanceOffset;
                float range = distance / gap;
                range *= Mathf.Abs(range);

                float push = range * snapVelocity * Time.deltaTime;

                // snap
                scrollbar.value += push;
                if(Mathf.Abs(distance) < thresh)
                {
                    snapped = true;
                    snapping = false;
                    return;
                }
            }
        }

        public void SetScrollPosition(RectTransform content, RectTransform viewport, float normalizedPosition)
        {
            // Get the heights
            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;

            // Calculate the scrollable distance
            float scrollableHeight = Mathf.Max(0, contentHeight - viewportHeight);

            // For bottom-to-top scrolling:
            // 0 = content at bottom (showing top of content)
            // 1 = content at top (showing bottom of content)
            float targetY = normalizedPosition * scrollableHeight;

            // Set the anchored position
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
        }

        public int Direction()
        {
            return scrollbar.direction is Scrollbar.Direction.TopToBottom or Scrollbar.Direction.LeftToRight ? -1 : 1;
        }

        public void OnDragScroll()
        {
            if (Input.touchCount == 0) return;

            noScrollTime = 0;
            snapped = false;
            snapping = false;
        }

        [System.Serializable]
        private class Rect
        {
            public RectTransform rect;
            public float height;
            public float centerPoint;
        }
    }
}