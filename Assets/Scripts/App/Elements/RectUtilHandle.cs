using UnityEngine;

namespace Visuals
{
    public class RectUtilHandle : MonoBehaviour
    {
        public RectTransform rect;
        public StrectRectState a;
        public StrectRectState b;


        public void Set(float range)
        {
            StrectRectState.Lerp(rect, a, b, range);
        }
    }

    [System.Serializable]
    public struct StrectRectState
    {
        public Vector2 offsetMin;
        public Vector2 offsetMax;

        public static void Lerp(RectTransform rect, StrectRectState a, StrectRectState b, float range)
        {
            rect.offsetMin = Vector3.Lerp(a.offsetMin, b.offsetMin, range);
            rect.offsetMax = Vector2.Lerp(a.offsetMax, b.offsetMax, range);
        }
    }
}