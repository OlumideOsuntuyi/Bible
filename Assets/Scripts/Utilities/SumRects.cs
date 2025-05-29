using UnityEngine;

[ExecuteInEditMode]
public class SumRects : MonoBehaviour
{
    public RectTransform target, content;
    public Vector2 init, minimumSize, spacing;
    public bool x, y, useMinimumSize;
    public bool doVerticalLayout, invertVertical;
    public bool doHorizontalLayout, invertHorizontal;
    public Padding padding;
    public int delay = 10;
    int frame = 0;
    bool _update;
    public bool setTargetSizeToContent;
    public bool suppressUpdates;

    private void OnRectTransformDimensionsChange()
    {
        if (suppressUpdates) return;
        _update = true;
        frame = 0;
    }

    private void OnTransformChildrenChanged()
    {
        if (suppressUpdates) return;
        _update = true;
        frame = 0;
    }

    public void PerformUpdate()
    {
        _update = true;
        frame = 0;
    }

    private void LateUpdate()
    {
        if(_update)
        {
            frame += 1;
            if(frame > delay)
            {
                _update = false;
                frame = 0;
                Resize();
            }
        }
    }

    public void Resize()
    {
        if (setTargetSizeToContent)
        {
            target.sizeDelta = content.sizeDelta;
            return;
        }

        if (target && content && frame == 0)
        {
            if (x || y)
            {
                target.sizeDelta = init + new Vector2(!x ? target.sizeDelta.x : 0, !y ? target.sizeDelta.y : 0);
            }
            Vector3 next = Vector3.zero;
            if (content.childCount > 0)
            {
                for (int i = 0; i < content.childCount; i++)
                {
                    var rect = content.GetChild(i).GetComponent<RectTransform>();
                    if (rect.gameObject.activeInHierarchy)
                    {
                        if (x || y)
                            target.sizeDelta += new Vector2(x ? rect.sizeDelta.x : 0, y ? rect.sizeDelta.y : 0) + spacing;

                        if (doVerticalLayout)
                        {
                            rect.localPosition = next + new Vector3(padding.left - padding.right, padding.bottom - padding.top, 0);
                            next -= new Vector3(0, doVerticalLayout ? rect.sizeDelta.y + spacing.y : 0, 0) * (invertVertical ? -1 : 1);
                        }
                        else if (doHorizontalLayout)
                        {
                            rect.localPosition = next + new Vector3(padding.left - padding.right, padding.bottom - padding.top, 0);
                            next += new Vector3(doHorizontalLayout ? rect.sizeDelta.x + spacing.x : 0, 0, 0) * (invertHorizontal ? -1 : 1);
                        }
                    }
                }
            }

            if(x || y)
            {
                target.sizeDelta = new Vector2()
                {
                    x = x && useMinimumSize ? Mathf.Max(target.sizeDelta.x, minimumSize.x) : target.sizeDelta.x,
                    y = y && useMinimumSize ? Mathf.Max(target.sizeDelta.y, minimumSize.y) : target.sizeDelta.y
                };
            }
        }
    }

    [System.Serializable]
    public struct Padding
    {
        public float top, left, right, bottom;
    }
}
