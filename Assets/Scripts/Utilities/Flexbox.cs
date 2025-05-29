using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals
{
    [ExecuteAlways]
    public class Flexbox : MonoBehaviour
    {
        public FlexDirection direction;
        public FlexWrap wrap;
        public JustifyContent justify;
        public AlignItems align;
        public float gap;
        public float flexGap;
        public bool fitSpan;
        public bool fitLength;
        public Vector2 size;

        public float top;
        public float bottom;
        public float left;
        public float right;

        [Header("DEBUG")]
        public int flexLength;
        [SerializeField] private List<FlexSpan> spans;


        private RectTransform Get(Transform transform)
        {
            return transform.GetComponent<RectTransform>();
        }

        private ChildUpdateData[] GetIn(Transform parent)
        {
            ChildUpdateData[] array = new ChildUpdateData[parent.childCount];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new ChildUpdateData(Get(parent.GetChild(i)));
            }
            return array;
        }

        private void Refresh()
        {
            RectTransform self = Get(transform);
            ChildUpdateData[] children = GetIn(transform);

            if(children.Length == 0)
            {
                return;
            }

            Vector2 selfSize = self.sizeDelta;

            if(fitSpan)
            {
                float totalSpan = 0;
                foreach(var child in children)
                {
                    totalSpan += FlexFor(child.rect.sizeDelta);
                }

                totalSpan += gap * (children.Length - 1);

                selfSize = new Vector2()
                {
                    x = IsX() ? totalSpan : selfSize.y,
                    y = IsX() ? selfSize.x : totalSpan
                };
            }

            float againstLength = FlexAgainst(selfSize);
            int flexLength = FlexLength(children, selfSize, out float flexSpan);
            this.flexLength = flexLength;

            var childQueue = new Queue<ChildUpdateData>(children);
            List<FlexSpan> spans = new();
            for (int i = 0; i < flexLength; i++)
            {
                FlexSpan span = new(flexSpan);
                span.TakeChildren(childQueue, this);

                if(span.childCount > 0)
                {
                    againstLength -= span.length;
                    spans.Add(span);
                }
            }

            float last = 0;
            float flexStart = 0;
            if(justify is JustifyContent.Center)
            {
                last = againstLength * 0.5f;
            }

            foreach(var span in spans)
            {
                Vector2 start = new()
                {
                    x = IsX() ? flexStart : last,
                    y = (IsX() ? -last : flexStart) + top
                };

                last += span.length + flexGap;
                span.start = start;


                span.Draw(this);
            }

            this.spans = spans;

            if (fitLength)
            {
                float totalLength = 0;
                foreach (var span in spans)
                {
                    totalLength += span.length + flexGap;
                }

                selfSize = new Vector2()
                {
                    x = IsX() ? selfSize.x : totalLength,
                    y = IsX() ? totalLength : selfSize.y
                };
            }

            if (selfSize != self.sizeDelta)
            {
                self.sizeDelta = selfSize;
            }

            size = selfSize;
        }

        private int FlexLength(ChildUpdateData[] children, Vector2 size, out float flexSpan)
        {
            float sumSpan = 0;
            float childSpan;
            flexSpan = FlexFor(size);
            foreach (var child in children)
            {
                if (child.rect == null) continue;

                var childSize = child.rect.sizeDelta;
                childSpan = FlexFor(childSize) + gap;
                sumSpan += childSpan;
                flexSpan = Mathf.Max(flexSpan, childSpan);
            }

            return Mathf.CeilToInt(sumSpan / flexSpan);
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnTransformChildrenChanged()
        {
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            Refresh();
        }

        public bool IsX()
        {
            return direction is FlexDirection.Row or FlexDirection.RowReverse;
        }

        public float FlexFor(Vector2 vec)
        {
            return IsX() ? vec.x : vec.y;
        }

        public float FlexAgainst(Vector2 vec)
        {
            return IsX() ? vec.y : vec.x;
        }

        public enum FlexDirection
        {
            Row, Column, RowReverse, ColumnReverse
        }


        public enum FlexWrap
        {
            NoWrap, Wrap
        }

        public enum JustifyContent
        {
            Start, End, Center, SpaceBetween, SpaceAround, SpaceEvenly
        }

        public enum AlignItems
        {
            Start, End, Center
        }

        private class ChildUpdateData
        {
            public readonly RectTransform rect;
            public ChildUpdateData(RectTransform transform)
            {
                this.rect = transform;
            }
        }

        [System.Serializable]
        private class FlexSpan
        {
            public Vector2 start;
            public float span;   // flex direction length
            public float length; // against flex direction
            public float freeSpan;
            public int childCount;
            public List<ChildUpdateData> children;

            public FlexSpan(float span)
            {
                this.span = span;
                children = new List<ChildUpdateData>();
            }

            public void TakeChildren(Queue<ChildUpdateData> queue, Flexbox flex)
            {
                float remainingSpace = span;
                float trueRemainingSpace = span;
                while(queue.Count > 0 && (flex.wrap is Flexbox.FlexWrap.NoWrap || flex.fitSpan || (remainingSpace >= flex.FlexFor(queue.Peek().rect.sizeDelta) + flex.gap)))
                {
                    var child = queue.Dequeue();
                    length = Mathf.Max(length, flex.FlexAgainst(child.rect.sizeDelta));
                    children.Add(child);

                    remainingSpace -= flex.FlexFor(child.rect.sizeDelta) + flex.gap;
                    trueRemainingSpace -= flex.FlexFor(child.rect.sizeDelta);
                }

                childCount = children.Count;
                freeSpan = trueRemainingSpace;
            }

            public void Draw(Flexbox flex)
            {
                float center = flex.align is AlignItems.Center ? length * 0.5f : 0;
                center += flex.justify is JustifyContent.Center ? freeSpan * 0.5f : 0;

                Vector2 pos = start + new Vector2()
                {
                    x = flex.IsX() ? center : 0,
                    y = flex.IsX() ? 0 : center
                };

                foreach(var child in children)
                {
                    child.rect.localPosition = pos;
                    pos += new Vector2()
                    {
                        x = flex.IsX() ? child.rect.sizeDelta.x + flex.gap : 0,
                        y = flex.IsX() ? 0 : -child.rect.sizeDelta.y - flex.gap
                    };
                }
            }
        }
    }
}