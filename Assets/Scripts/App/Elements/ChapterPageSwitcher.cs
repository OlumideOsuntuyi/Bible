using Core;

using UnityEngine;
using UnityEngine.Events;

namespace Visuals
{
    public class ChapterPageSwitcher : MonoBehaviour
    {
        public RectTransform parent;
        public RectTransform[] nodes;
        public RectTransform area;
        public float spacing;
        public int active;
        public float speed;
        public float threshold;

        public UnityEvent onSwipeLeft;
        public UnityEvent onSwipeRight;

        [SerializeField] private Page[] pages;
        private int a;

        [SerializeField] private float disp;
        [SerializeField] private Vector2 startingPosition;
        [SerializeField] private Vector2 currentPosition;

        [SerializeField] private bool isHorizontalScrolling;
        [SerializeField] private int moveFrame;

        public int Previous
        {
            get { return Shift(active - 1, 0, nodes.Length); }
        }
        public int Next
        {
            get { return Shift(active + 1, 0, nodes.Length); }
        }

        private void Awake()
        {
            spacing = parent.rect.width;
            pages = new Page[nodes.Length];
            int center = Mathf.FloorToInt(nodes.Length * 0.5f);
            for (int i = 0; i < nodes.Length; i++)
            {
                var pos = (i - center) * spacing;
                pages[i] = new Page()
                {
                    node = nodes[i],
                    index = i,
                    currentAnchoredPosition = nodes[i].anchoredPosition.x,
                    swipeTargetPosition = pos,
                    pageValidPosition = pos
                };

                var r = nodes[i];
                r.anchorMin = new Vector2(0f, 1f);
                r.anchorMax = new Vector2(0f, 1f);
                r.pivot = new Vector2(0f, 1f);

                r.sizeDelta = new Vector2(parent.rect.width, parent.rect.height);
                pages[i].Move(1);
            }

            active = center;
        }


        public static void Set(RectTransform rect, float x)
        {
            rect.anchoredPosition = new Vector3(x, rect.anchoredPosition.y);
        }

        private void Update()
        {
            if (NavigationList.IsOpen) return;
            if (ScreenManager.GetTransition("toolbar").Active is 1) return;


            float range = Time.deltaTime * speed;
            bool move = false;
            if(Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                float diff = t.position.x - currentPosition.x;
                float absDiff = Mathf.Abs(diff);

                disp += diff;
                currentPosition = t.position;

                switch (t.phase)
                {
                    case TouchPhase.Began:
                        {
                            startingPosition = currentPosition;
                            disp = 0;
                            moveFrame = 0;
                            return;
                        }
                    case TouchPhase.Moved:
                        {
                            if(moveFrame++ == 0)
                            {
                                if(Mathf.Abs(currentPosition.x - startingPosition.x) > Mathf.Abs(currentPosition.y - startingPosition.y))
                                {
                                    // horizontal swipe
                                    isHorizontalScrolling = true;
                                    ChapterLoaderManager.Instance.PauseVerticalScrolls();
                                }
                                else
                                {
                                    
                                }
                            }

                            if(isHorizontalScrolling)
                            {
                                // scroll horizontally
                                foreach (var p in pages)
                                {
                                    float x = p.pageValidPosition;
                                    x += disp;
                                    p.swipeTargetPosition = x;
                                }
                            }
                            else
                            {
                                // ignore scroll
                                disp = 0;

                            }


                        }break;
                    case TouchPhase.Canceled or TouchPhase.Ended:
                        {
                            if(isHorizontalScrolling)
                            {
                                isHorizontalScrolling = false;
                                ChapterLoaderManager.Instance.ReleaseVerticalScrolls();
                            }
                        }break;
                }

                move = t.phase is TouchPhase.Moved or TouchPhase.Stationary;
            }

            foreach (var m in pages)
            {
                m.Move(4 * Time.deltaTime);
            }

            // if there was horizontal swiping
            if(!move && disp != 0)
            {
                // if swiped more than thresh, move to next page
                // else slack back to valid position
                if (Mathf.Abs(disp) > spacing * threshold)
                {
                    startingPosition = currentPosition;
                    int center = Mathf.FloorToInt(nodes.Length * 0.5f);
                    int d = (int)Mathf.Sign(disp);

                    active = Shift(active - d, 0, nodes.Length);
                    if (a != active)
                    {
                        if (d > 0)
                        {
                            onSwipeLeft.Invoke();
                        }
                        else
                        {
                            onSwipeRight.Invoke();
                        }
                    }

                    a = active;

                    for (int i = 0; i < nodes.Length; i++)
                    {
                        var p = pages[i];

                        var newIndex = Shift(p.index + d, 0, nodes.Length);
                        p.pageValidPosition = (newIndex - center) * spacing;
                        if (p.index + d != newIndex)
                        {
                            p.swipeTargetPosition = p.pageValidPosition * 1.1f;
                            p.currentAnchoredPosition = p.swipeTargetPosition;
                        }

                        p.index = newIndex;
                    }
                }


                startingPosition = currentPosition;
                disp = 0;
            }


            foreach (var p in pages)
            {
                if (!move)
                {
                    p.swipeTargetPosition = p.pageValidPosition;
                }

                p.Move(range);
            }
        }

        public void Swipe()
        {

        }

        public static int Shift(int a, int min, int max)
        {
            if (a < min) return max - 1;
            if (a >= max) return min;
            return a;
        }


        [System.Serializable]
        public class Page
        {
            public RectTransform node;
            public int index;
            public float swipeTargetPosition;
            public float currentAnchoredPosition;

            public float pageValidPosition;


            public void Move(float range)
            {
                currentAnchoredPosition = Mathf.Lerp(currentAnchoredPosition, swipeTargetPosition, range);
                Set(node, currentAnchoredPosition);
            }
        }
    }
}