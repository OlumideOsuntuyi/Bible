using UnityEngine;
using UnityEngine.UI;

using Visuals.Module;

namespace Visuals
{ 
    public class BookInListItem : MonoBehaviour
    {
        public InsertableText book;
        public Image img;
        public int index = 0;

        public void Set(string name, int index)
        {
            book.text = name;
            this.index = index;
        }

        public void Select()
        {
            NavigationList.Instance.SelectBook(index);
        }

        public (int book, float distance) Compare(Transform center)
        {
            float d = transform.position.y - center.position.y;
            return (index, d);
        }

        public void OnScroll(int current)
        {
            img.color = ColorPallate.Get(current == index ? "active-in-list" : "inactive-in-list");
        }
    }
}