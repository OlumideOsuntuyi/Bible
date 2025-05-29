using System;

using Core;

using TMPro;

using UnityEngine;

namespace Visuals
{
    public class HomeTime : MonoBehaviour
    {
        [SerializeField] private TMP_Text greeting;
        [SerializeField] private TMP_Text date;

        private void Awake()
        {
            TickSystem.Instance.Subscribe(Display);
        }

        void Display()
        {
            DateTime time = DateTime.Now;
            int hour = time.Hour;
            string greeting = "Night";
            if (hour >= 3) greeting = "Morning";
            if (hour >= 12) greeting = "Afternoon";
            if (hour >= 17) greeting = "Evening";
            if(hour >= 22) greeting = "Night";

            this.greeting.text = $"Good {greeting}";
            date.text = time.ToString("dddd, MMMM dd");
        }
    }
}
