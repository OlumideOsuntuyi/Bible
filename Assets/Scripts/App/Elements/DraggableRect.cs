using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Visuals
{
    public class DraggableRect : MonoBehaviour
    {
        public RectTransform rect;
        public RectTransform sizeGetter;
        public Graphic graphic;
        public float holdOffset;
        public float min;
        public float max;
        public float threshold;

        public float target;
        public float value;
        public float decaySpeed;
        public int state;
        public float holdElasticity;

        public UnityEvent onOpen;
        public UnityEvent onOpened;
        public UnityEvent onClose;

        public bool bottom;

        private Vector2 lastPosition;

        private bool open;

        private void Start()
        {
            float width = sizeGetter.rect.width;
            float height = sizeGetter.rect.height;

            rect.pivot = new Vector2(0.5f, bottom ? 0 : 1f);
            rect.anchorMin = new Vector2(0f, bottom ? 0 : 1f);
            rect.sizeDelta = new Vector2()
            {
                x = width,
                y = height
            };

            rect.offsetMin = new Vector2(0, rect.rect.yMin);
            rect.offsetMax = new Vector2(0, rect.rect.yMax);

            max = height * -1.15f;
            Close();

            if(threshold < 0)
            {
                threshold = height * .5f;
            }
        }
        private void Update()
        {
            if (state == 0)
            {
                return;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
                return;
            }

            switch(state)
            {
                case 1:
                    {
                        if(Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            float y = (rect.sizeDelta.y - touch.position.y);
                            if (y < 0) return;
                            var p = touch.phase;
                            switch(p)
                            {
                                case TouchPhase.Began:
                                    {
                                        if(RaycastUtilities.PointerOverUI(touch.position) == graphic.gameObject)
                                        {
                                            state = 2; // start hold  
                                            //Set(-y + holdOffset, true, holdElasticity);
                                            lastPosition = touch.position;
                                        }
                                    }break;
                                default:break;
                            }
                        }
                    }break;
                case 2:
                    {
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);
                            float y = (rect.sizeDelta.y - touch.position.y);
                            if (y < 0) return;
                            var p = touch.phase;
                            switch (p)
                            {
                                case TouchPhase.Moved or TouchPhase.Stationary:
                                    {
                                        float dt = touch.position.y - lastPosition.y;
                                        lastPosition = touch.position;
                                        Set(value + dt);
                                    }
                                    break;
                                case TouchPhase.Ended or TouchPhase.Canceled:
                                    {
                                        if(y < threshold)
                                        {
                                            target = 0;
                                            state = 3;
                                        }
                                        else
                                        {
                                            Close();
                                        }

                                    }
                                    break;
                                default: break;
                            }
                        }
                        else
                        {
                            ReleaseFinger();
                        }
                    }
                    break;
                case 3 or 4:
                    {
                        value = Mathf.Lerp(value, target, Time.deltaTime * decaySpeed);
                        Set(value);

                        if (Mathf.Abs(value - target) < 0.01f)
                        {
                            if (state == 3)
                            {
                                if (target == 0)
                                {
                                    onOpened.Invoke();
                                }
                            }
                            else if (state == 4)
                            {
                                //onClose.Invoke();
                            }
                            state = state == 4 ? 0 : 1;
                            value = target;
                        }
                    }break;
                default: return;
            }
        }

        private void ReleaseFinger()
        {
            float y = -(value - holdOffset);
            if (y < threshold)
            {
                target = 0;
                state = 3;
            }
            else
            {
                Close();
            }
        }

        public void Set(float y, bool hold, float speed)
        {
            Set(Mathf.Lerp(value, y, speed * Time.deltaTime));
        }
        public void Set(float y)
        {
            rect.anchoredPosition = new Vector3(rect.anchoredPosition.x, y + min);
            value = y;
        }

        public void Open()
        {
            state = 3;
            target = 0;
            open = true;
            onOpen.Invoke();
            ScreenManager.FreezePagesEscape = true;
        }

        public void Close()
        {
            state = 4;
            target = max;
            onClose.Invoke();
            open = false;


            ScreenManager.FreezePagesEscape = false;
        }
    }
}