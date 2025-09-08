using UnityEngine;

namespace Core.Trivia
{
    [System.Serializable]
    public class TriviaValue
    {
        public string text;
        public const string PICTORALTAG = "<p></p>";
        public const string AUDIOTAG = "<a></a>";
    }

    [System.Serializable]
    public class TriviaBooleanValue : TriviaValue
    {
        public bool isTrue;
    }



    [System.Serializable]
    public class TriviaPictorialValue : TriviaValue
    {
        public bool isIcon;
    }


    [System.Serializable]
    public class TriviaAudioValue : TriviaValue
    {
        
    }
}