using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class ChapterSwiper : MonoBehaviour
    {
        public RectTransform parent;
        [Header("Pages")]
        public Transform[] pages = new Transform[3];

        [Header("Swipe Settings")]
        public float swipeSpeed = 5f;
        public float swipeThreshold = 100f;
        public float swipeCooldown = 0.5f;

        [Header("Animation")]
        public AnimationCurve swipeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Events")]
        public UnityEvent onSwipedCallback;
        public UnityEvent onSwipeLeft;
        public UnityEvent onSwipeRight;
        public UnityEvent onStartSwipe;
        public UnityEvent onEndSwipe;

        private int currentPageIndex = 0;
        private bool isAnimating = false;
        private bool canSwipe = true;
        private float pageSpacing = 1920f; // Default screen width spacing

        // Touch/Mouse input variables
        private Vector2 startTouchPosition;
        private Vector2 currentTouchPosition;
        private bool isHolding = false;
        private float holdOffset = 0f;

        // Infinite scrolling variables
        private int[] pageOrder = new int[3]; // Tracks which logical page each physical page represents

        public int Current
        {
            get { return currentPageIndex; }
        }

        public int Prev
        {
            get
            {
                int a = Current;
                if (a - 1 < 0) return 2;
                return a;
            }
        }

        public int Next
        {
            get
            {
                int a = Current;
                if (a + 1 > 2) return 0;
                return a;
            }
        }

        void Start()
        {
            InitializePages();
            SetupInfinitePages();
        }

        void Update()
        {
            HandleInput();
        }

        void InitializePages()
        {
            if (pages.Length != 3)
            {
                Debug.LogError("ChapterSwiper requires exactly 3 pages!");
                return;
            }

            // Get screen width for proper spacing
            var s = new Vector2(parent.rect.width, parent.rect.height);
            pageSpacing = s.x; //Screen.width;

            // Ensure all pages are assigned
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] == null)
                {
                    Debug.LogError($"Page {i} is not assigned!");
                }

                var r = pages[i].GetComponent<RectTransform>();
                r.sizeDelta = s;
            }
        }

        void SetupInfinitePages()
        {
            // Initialize page order: left = prev, center = current, right = next
            pageOrder[0] = GetPreviousPageIndex(currentPageIndex); // Left page
            pageOrder[1] = currentPageIndex;                       // Center page (current)
            pageOrder[2] = GetNextPageIndex(currentPageIndex);     // Right page

            // Position pages: left at -pageSpacing, center at 0, right at +pageSpacing
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    Vector3 pos = pages[i].localPosition;
                    pos.x = (i - 1) * pageSpacing; // i-1 so center page (index 1) is at position 0
                    pages[i].localPosition = pos;
                }
            }
        }

        int GetNextPageIndex(int index)
        {
            return (index + 1) % 3;
        }

        int GetPreviousPageIndex(int index)
        {
            return (index - 1 + 3) % 3;
        }

        void HandleInput()
        {
            if (isAnimating || !canSwipe) return;

            // Handle touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                HandleTouchInput(touch.position, touch.phase);
                return;
            }

            // Handle mouse input for testing
            else if (Input.GetMouseButtonDown(0))
            {
                HandleTouchInput(Input.mousePosition, TouchPhase.Began);
            }
            else if (Input.GetMouseButton(0))
            {
                HandleTouchInput(Input.mousePosition, TouchPhase.Moved);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleTouchInput(Input.mousePosition, TouchPhase.Ended);
            }
        }

        void HandleTouchInput(Vector2 position, TouchPhase phase)
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = position;
                    currentTouchPosition = position;
                    isHolding = true;
                    holdOffset = 0f;
                    break;

                case TouchPhase.Moved:
                    if (isHolding)
                    {
                        currentTouchPosition = position;
                        float deltaX = currentTouchPosition.x - startTouchPosition.x;
                        FingerHold(deltaX);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isHolding)
                    {
                        float deltaX = currentTouchPosition.x - startTouchPosition.x;

                        if (Mathf.Abs(deltaX) > swipeThreshold)
                        {
                            if (deltaX < 0)
                            {
                                SwipeRight();
                            }
                            else
                            {
                                SwipeLeft();
                            }
                        }
                        else
                        {
                            // Return to original position
                            StartCoroutine(AnimateToCenter());
                        }

                        isHolding = false;
                        holdOffset = 0f;
                    }
                    break;
            }
        }

        public void FingerHold(float deltaX)
        {
            if (isAnimating) return;

            // Apply elastic resistance at the edges for better feel
            float resistance = 1f;
            if (Mathf.Abs(deltaX) > pageSpacing * 0.5f)
            {
                resistance = 0.3f;
            }

            holdOffset = deltaX * resistance;

            // Apply the offset to all pages
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    Vector3 pos = pages[i].localPosition;
                    pos.x = (i - 1) * pageSpacing + holdOffset;
                    pages[i].localPosition = pos;
                }
            }
        }

        public void SwipeLeft()
        {
            if (!canSwipe || isAnimating) return;

            StartCoroutine(SwipeCooldownCoroutine());

            // Move to next page
            currentPageIndex = GetNextPageIndex(currentPageIndex);
            StartCoroutine(AnimateSwipeLeft());

            onSwipeLeft?.Invoke();
        }

        public void SwipeRight()
        {
            if (!canSwipe || isAnimating) return;

            StartCoroutine(SwipeCooldownCoroutine());

            // Move to previous page
            currentPageIndex = GetPreviousPageIndex(currentPageIndex);
            StartCoroutine(AnimateSwipeRight());

            onSwipeRight?.Invoke();
        }

        IEnumerator AnimateSwipeLeft()
        {
            isAnimating = true;
            onStartSwipe?.Invoke();

            float animationTime = 1f / swipeSpeed;
            float elapsed = 0f;

            // Store starting positions
            Vector3[] startPositions = new Vector3[3];
            Vector3[] targetPositions = new Vector3[3];

            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    startPositions[i] = pages[i].localPosition;
                    targetPositions[i] = startPositions[i];
                    targetPositions[i].x = startPositions[i].x - pageSpacing; // Move all pages left
                }
            }

            // Animate to target positions
            while (elapsed < animationTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / animationTime;
                float curveValue = swipeCurve.Evaluate(progress);

                for (int i = 0; i < pages.Length; i++)
                {
                    if (pages[i] != null)
                    {
                        Vector3 pos = Vector3.Lerp(startPositions[i], targetPositions[i], curveValue);
                        pages[i].localPosition = pos;
                    }
                }

                yield return null;
            }

            // Reposition pages for infinite effect
            RepositionPagesAfterSwipeLeft();

            isAnimating = false;
            onSwipedCallback?.Invoke();
            onEndSwipe?.Invoke();
        }

        IEnumerator AnimateSwipeRight()
        {
            isAnimating = true;
            onStartSwipe?.Invoke();

            float animationTime = 1f / swipeSpeed;
            float elapsed = 0f;

            // Store starting positions
            Vector3[] startPositions = new Vector3[3];
            Vector3[] targetPositions = new Vector3[3];

            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    startPositions[i] = pages[i].localPosition;
                    targetPositions[i] = startPositions[i];
                    targetPositions[i].x = startPositions[i].x + pageSpacing; // Move all pages right
                }
            }

            // Animate to target positions
            while (elapsed < animationTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / animationTime;
                float curveValue = swipeCurve.Evaluate(progress);

                for (int i = 0; i < pages.Length; i++)
                {
                    if (pages[i] != null)
                    {
                        Vector3 pos = Vector3.Lerp(startPositions[i], targetPositions[i], curveValue);
                        pages[i].localPosition = pos;
                    }
                }

                yield return null;
            }

            // Reposition pages for infinite effect
            RepositionPagesAfterSwipeRight();

            isAnimating = false;
            onSwipedCallback?.Invoke();

            onEndSwipe?.Invoke();
        }

        void RepositionPagesAfterSwipeLeft()
        {
            // After swiping left, the leftmost page becomes the new rightmost page
            Transform leftmostPage = pages[0];

            // Shift pages array left
            pages[0] = pages[1];
            pages[1] = pages[2];
            pages[2] = leftmostPage;

            // Update page order
            pageOrder[0] = GetPreviousPageIndex(currentPageIndex);
            pageOrder[1] = currentPageIndex;
            pageOrder[2] = GetNextPageIndex(currentPageIndex);

            // Reset positions
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    Vector3 pos = pages[i].localPosition;
                    pos.x = (i - 1) * pageSpacing;
                    pages[i].localPosition = pos;
                }
            }
        }

        void RepositionPagesAfterSwipeRight()
        {
            // After swiping right, the rightmost page becomes the new leftmost page
            Transform rightmostPage = pages[2];

            // Shift pages array right
            pages[2] = pages[1];
            pages[1] = pages[0];
            pages[0] = rightmostPage;

            // Update page order
            pageOrder[0] = GetPreviousPageIndex(currentPageIndex);
            pageOrder[1] = currentPageIndex;
            pageOrder[2] = GetNextPageIndex(currentPageIndex);

            // Reset positions
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    Vector3 pos = pages[i].localPosition;
                    pos.x = (i - 1) * pageSpacing;
                    pages[i].localPosition = pos;
                }
            }
        }

        IEnumerator AnimateToCenter()
        {
            isAnimating = true;

            float animationTime = 0.6f;
            float elapsed = 0f;
            float startOffset = holdOffset;

            while (elapsed < animationTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / animationTime;
                float curveValue = swipeCurve.Evaluate(progress);

                float currentOffset = Mathf.Lerp(startOffset, 0f, curveValue);

                for (int i = 0; i < pages.Length; i++)
                {
                    if (pages[i] != null)
                    {
                        Vector3 pos = pages[i].localPosition;
                        pos.x = (i - 1) * pageSpacing + currentOffset;
                        pages[i].localPosition = pos;
                    }
                }

                yield return null;
            }

            // Ensure final positions are exact
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    Vector3 pos = pages[i].localPosition;
                    pos.x = (i - 1) * pageSpacing;
                    pages[i].localPosition = pos;
                }
            }

            holdOffset = 0f;
            isAnimating = false;
        }

        IEnumerator SwipeCooldownCoroutine()
        {
            canSwipe = false;
            yield return new WaitForSeconds(swipeCooldown);
            canSwipe = true;
        }

        // Public methods for external access
        public int GetCurrentPageIndex()
        {
            return currentPageIndex;
        }

        public bool IsAnimating()
        {
            return isAnimating;
        }

        public void SetPageSpacing(float spacing)
        {
            pageSpacing = spacing;
            SetupInfinitePages();
        }

        public Transform GetCurrentPage()
        {
            return pages[1]; // Center page is always the current page
        }

        public int[] GetPageOrder()
        {
            return (int[])pageOrder.Clone();
        }
    }
}