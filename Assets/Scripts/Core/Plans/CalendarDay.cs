using TLab.UI.SDF;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Visuals;

namespace Core.Plans
{
    public class CalendarDay : MonoBehaviour
    {
        public TMP_Text day;
        public Graphic background;

        private int _day;


        public void Set(int day)
        {
            if (day == 0)
            {
                this.day.text = "";
                background.color = Color.clear;
                return;
            }

            this.day.text = day.ToString();
            _day = day;
        }

        public void OnUpdate()
        {
            bool highlight = _day == Calendar.Instance.today || _day == Calendar.Instance.active;

            if(background is SDFQuad quad)
            {
                quad.fillColor = ColorPallate.Get(_day == Calendar.Instance.active ? "active" : "clear");
            }
            else
            {
                background.color = ColorPallate.Get(_day == Calendar.Instance.active ? "active" : "clear");
            }
            day.color = ColorPallate.Get(highlight ? "text-color" : "text-color");

            if(_day == Calendar.Instance.today && _day != Calendar.Instance.active)
            {
                day.color = ColorPallate.Get("active");
            }
        }

        public void Click()
        {
            Calendar.Instance.SelectDay(_day);
        }
    }
}