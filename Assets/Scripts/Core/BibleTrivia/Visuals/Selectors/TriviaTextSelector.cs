using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Core.Trivia
{
    public class TriviaTextSelector : MonoBehaviour
    {
        public TMP_Text label;
        public Graphic graphic;
        public Button button;

        public Color[] colors;

        private Trivia trivia;
        private TriviaValue value;

        public void Set(TriviaValue value, Trivia trivia)
        {
            label.text = value.text;
            this.value = value;

            this.trivia = trivia;
            button.onClick.AddListener(Clicked);
        }

        public void Clicked()
        {
            trivia.EnterAnswer(value);
        }
    }
}