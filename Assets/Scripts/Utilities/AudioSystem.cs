using System.Collections.Generic;

using LostOasis;

using UnityEngine;

namespace Core.Audio
{
    [DefaultExecutionOrder(-30)]
    public class AudioSystem : Singleton<AudioSystem>
    {
        private static Dictionary<AudioChannelType, AudioChannel> channels;
        private static Dictionary<string, AudioClip> clips;

        [SerializeField] private List<AudioChannelData> channelsData;
        [SerializeField] private List<AudioClipData> clipsData;


        private void Awake()
        {
            clips = new Dictionary<string, AudioClip>();
            channels = new Dictionary<AudioChannelType, AudioChannel>();

            foreach (var clipData in clipsData)
            {
                clips.Add(clipData.uniqueName.MegaTrim(), clipData.clip);
            }

            foreach (var cd in channelsData)
            {
                var channel = new AudioChannel(cd.capacity, cd.type);
                channel.Fill();
                channels.Add(cd.type, channel);
            }
        }

        public void PlayButton()
        {
            PlayUI("button");
        }

        public void PlayUI(string name)
        {
            _ = PlayAudio(name, AudioChannelType.UI, SourceMode.Playing);
        }

        public static AudioSource PlayAudio(string clipName, AudioChannelType type, SourceMode mode)
        {
            AudioSource src = channels[type].Get(mode);

            // if clip not found in bank
            if (clips.TryGetValue(clipName.MegaTrim(), out var clip))
            {
                src.clip = clip;
            }
            src.Play();
            return src;
        }

        [System.Serializable]
        private struct AudioClipData
        {
            public string uniqueName;
            public AudioClip clip;
        }
    }
}