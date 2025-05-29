using System;

using UnityEngine;

namespace Core
{
    using Timer = System.Diagnostics.Stopwatch;
    [DefaultExecutionOrder(-99)]
    public class TickSystem : Singleton<TickSystem>
    {
        public const float TICK_SPEED = 50;
        public const float TICK_SPEED_MS = TICK_SPEED / 1000;
        public const float TPS = 1000 / TICK_SPEED;
        private Timer timer;
        [SerializeField] private float totalTime;
        [SerializeField] private int ticks;
        [SerializeField] private int subscribers;

        public float SyncSpeed;

        public int tps;
        public int tickDropTimes;
        public float maxTickDrop;
        public static float SYNC_RANGE { get; private set; }

        private event Action OnTick;
        private int tickCounter;
        private float time;

        private void Awake()
        {
            OnTick = () =>
            {
            };

            timer = new Timer();
            timer.Restart();

            maxTickDrop = 20;
        }

        private void Update()
        {
            if (subscribers <= 0) return;

            time += Time.deltaTime;
            //if (time >= 1)
            //{
            //    time -= 1.0f;
            //    tps = tickCounter;
            //    tickCounter = 0;

            //    if (tps < 20)
            //    {
            //        tickDropTimes++;
            //        maxTickDrop = Mathf.Min(maxTickDrop, tps);
            //    }
            //}

            if (timer.Elapsed.TotalMilliseconds < TICK_SPEED)
            {
                return;
            }

            tickCounter++;
            totalTime += Time.deltaTime;
            Tick();
        }

        private void LateUpdate()
        {
            SYNC_RANGE = Time.deltaTime * (1f / SyncSpeed);
        }

        private void Tick()
        {
            ticks++;
            totalTime = 0;
            timer.Restart();
            OnTick.Invoke();
        }

        public void Subscribe(Action action)
        {
            OnTick += action;
            subscribers++;
        }

        public void Unsubscribe(Action action)
        {
            OnTick -= action;
            subscribers--;
        }


    }
}