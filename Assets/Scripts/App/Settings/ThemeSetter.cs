using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Visuals
{
    [RequireComponent(typeof(Graphic))]
    public class ThemeSetter : UIBehaviour
    {
        public ThemeColorType type;
        private bool subscribed;

        private void Set(Color color)
        {
            var graphic = gameObject.GetComponent<Graphic>();
            graphic.color = color;
        }

        public void TrySubscribe()
        {
            Subscribed();
        }

        private void Subscribed()
        {
            if (subscribed) return;
            subscribed = true;
            ThemeManager.Subscribe(Set, type);
        }

        protected override void Awake()
        {
            Subscribed();
        }

        protected override void OnDestroy()
        {
            ThemeManager.UnSubscribe(Set, type);
        }
    }
}