using TMPro;

using UnityEngine;

namespace Core.Trivia
{
    public class TriviaTextTemplate : TriviaVisualiser
    {
        public TMP_Text label;
        public TriviaTextSelector selectorPrefab;
        public RectTransform content;

        private void Awake()
        {
            Load(Trivia.DEFAULT_TRIVIA);
        }

        public override void Load(Trivia trivia)
        {
            label.text = trivia.question.text;
            foreach(var value in trivia.options)
            {
                var selector = Instantiate(selectorPrefab, content);
                selector.Set(value, trivia);
            }

            trivia.onEnterAnswer += OnEnterAnswer;
        }

        public void OnEnterAnswer(TriviaValue triviaValue)
        {

        }
    }
}