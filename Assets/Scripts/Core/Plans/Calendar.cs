using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Visuals.Module;

namespace Core.Plans
{
    public class Calendar : Singleton<Calendar>
    {
        public InsertableText monthText;
        public RectTransform content;
        public Image icon;
        public Sprite[] sprites;
        [SerializeField] private CalendarDay prefab;
        [SerializeField] private GameObject[] calendayObjects;

        public int active { get; private set; }
        public int today { get; private set; }
        public int month { get; private set; }
        public bool calendarOpen { get; private set; }
        private Action onUpdate;

        private readonly static string[] Months =
        {
            "January", "February", "March", "April","May", "June", "July", "August","September", "October", "November", "December"
        };

        private void OnEnable()
        {
            LoadHeader();
            SetCalendar(false);
        }

        private void LoadHeader(bool setActive=true)
        {
            DateTime time = DateTime.Now;
            today = time.Day;
            active = setActive ? today : active;
            month = time.Month;
            monthText.text = $"Readings for {Months[month - 1]} {active}";
        }

        private void Load()
        {
            MegaUtils.ClearChildren(content);
            DateTime time = DateTime.Now;
            onUpdate = () => { };

            ProcessCalendarDays(time, (int day) =>
            {
                var clone = Instantiate(prefab, content);
                clone.Set(day);
                onUpdate += clone.OnUpdate;
            });


            LoadHeader(false);
            onUpdate.Invoke();
        }

        public static void ProcessCalendarDays(DateTime date, Action<int> action)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            int firstDayOfWeek = (int)firstDay.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            for (int i = 0; i < firstDayOfWeek; i++)  // go from Sun - first day
            {
                action(0);
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                action(day);
            }
        }

        public void SelectDay(int day)
        {
            active = day;
            onUpdate.Invoke();
        }

        public void Toggle()
        {
            SetCalendar(!calendarOpen);
        }

        private void SetCalendar(bool state)
        {
            LoadHeader(false);
            foreach (var obj in calendayObjects)
            {
                obj.SetActive(state);
            }

            if (state)
            {
                Load();
            }
            else
            {
                MegaUtils.ClearChildren(content);
            }

            calendarOpen = state;
            icon.sprite = sprites[calendarOpen ? 1 : 0];
        }
    }
}