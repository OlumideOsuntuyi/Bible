using System;

using Core.Audio;

using Core.Module;

using UnityEngine;
using UnityEngine.UI;

namespace Visuals.Module
{
    public class InsertableSound : Insertable
    {
        public Image icon;
        string sound;

        public AudioSource src;

        public override void Set(Value value)
        {
            sound = value.GetString();
        }

        public override void SetFontSize(float size)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = 1.75f * size * Vector2.one;
        }

        public void Click()
        {
            src = AudioSystem.PlayAudio($"{sound}", AudioChannelType.SFX, SourceMode.Playing);
            src.Play();
            src.loop = false;
        }
    }
}