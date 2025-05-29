using System.Collections.Generic;

using UnityEngine;

namespace Core.Audio
{
    public class AudioChannel
    {
        private int capacity;
        private AudioChannelType type;

        private Queue<PooledSource> queue;
        private List<PooledSource> running;

        public AudioChannel(int capacity, AudioChannelType type)
        {
            this.capacity = capacity;
            this.type = type;
            queue = new Queue<PooledSource>();
            running = new List<PooledSource>();
        }

        public void Fill()
        {
            PooledSource s;
            while (queue.Count < capacity)
            {
                s = new PooledSource();
                s.Generate();
                s.MarkRetrieved();

                queue.Enqueue(s);
            }
        }

        public void Refresh()
        {
            int i = 0;
            PooledSource s;
            while (i < running.Count)
            {
                s = running[i++];
                if (s.retrievalMode is SourceMode.Playing && !s.source.isPlaying)
                {
                    Remove();
                }
                else if (s.retrievalMode is SourceMode.Clip && !s.source.clip)
                {
                    Remove();
                }

                void Remove()
                {
                    running.RemoveAt(--i);
                    s.source.transform.SetParent(null);
                    s.source.transform.position = new Vector3();
                    queue.Enqueue(s);
                }
            }
        }

        public AudioSource Get(SourceMode mode)
        {
            Refresh();

            if (queue.Count == 0)
            {
                PooledSource tempSrc = new()
                {
                    retrievalMode = mode
                };
                tempSrc.Generate();
                return tempSrc.source;
            }

            var s = queue.Dequeue();
            s.retrievalMode = mode;
            return s.source;
        }

    }

    [System.Serializable]
    public struct AudioChannelData
    {
        public AudioChannelType type;
        public int capacity;
    }

    public enum AudioChannelType : byte
    {
        Music, UI, SFX, Voice
    }

    public enum SourceMode : byte
    {
        None, Playing, Clip
    }


    [System.Serializable]
    public class PooledSource
    {
        public AudioSource source;
        public bool retrieved;
        public SourceMode retrievalMode;

        public void Generate()
        {
            GameObject obj = new();
            obj.hideFlags = HideFlags.HideInHierarchy;
            source = obj.AddComponent<AudioSource>();
        }

        public void MarkRetrieved()
        {

        }

        public void OnRelease()
        {

        }
    }
}