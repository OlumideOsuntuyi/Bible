using UnityEngine;
using UnityEngine.UI;

using Visuals.Module;

namespace Visuals
{
    public class ChapterInListItem : MonoBehaviour
    {
        public InsertableText chapter;
        public Image img;
        public int index = 0;

        public void Set(int index)
        {
            chapter.text = index.ToString();
            this.index = index;
        }

        public void Select()
        {
            NavigationList.Instance.SelectChapter(index);
        }

        public void SelectVerse()
        {
            NavigationList.Instance.SelectVerse(index);
        }

        public (int book, float distance) Compare(Transform center)
        {
            return (index, (transform.position.y - center.position.y));
        }

        public void OnScroll(int current)
        {
            img.color = ColorPallate.Get(current == index ? "active-in-list" : "inactive-in-list");
        }
    }
}