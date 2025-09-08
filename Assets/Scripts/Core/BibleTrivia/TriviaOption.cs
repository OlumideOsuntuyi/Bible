using UnityEngine;

namespace Core.Trivia
{
    [System.Serializable]
    public class TriviaOption
    {
        public TriviaValue value;
        public TriviaOptionType type;
    }

    public enum TriviaOptionType : byte
    {
        Text, Pictorial, Audio
    }
}