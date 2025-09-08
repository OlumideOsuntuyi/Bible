using System;
using System.Collections.Generic;

using UnityEngine;

namespace Core.Trivia
{
    [System.Serializable]
    public class Trivia
    {
        public TriviaValue question;
        public List<TriviaValue> options;

        public Action<TriviaValue> onEnterAnswer;
        public Action<TriviaValue> onCorrectAnswer;
        public Action<TriviaValue> onWrongAnswer;
        public Action onCompleted;

        public Trivia()
        {
            
        }

        public Trivia(string json)
        {
            options = new();
        }

        public void EnterAnswer(TriviaValue value)
        {
            onEnterAnswer?.Invoke(value);
        }


        public static Trivia DEFAULT_TRIVIA = new()
        {
            question = new()
            {
                text = "Who is Abraham?"
            },
            options = new List<TriviaValue>()
            {
                new()
                {
                    text = "Choir Master"
                },
                new()
                {
                    text = "Drummer"
                },
                new()
                {
                    text = "Prophet"
                },
                new()
                {
                    text = "Painter"
                },
            }
        };
    }
}