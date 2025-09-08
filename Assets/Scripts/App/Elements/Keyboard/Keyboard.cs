using System;

using TMPro;

using UnityEngine;

namespace LostOasis
{
    public class Keyboard : Singleton<Keyboard>
    {
        public AudioSource audioSrc;
        public RectTransform self;
        public TMP_Text currentTextDisplay;
        public float minH, maxH;
        public float hideSpeed;
        private static float hideTime;

        public string text;
        public bool caps;
        public bool capsLock;

        public KeyboardKey[] keys;
        public Transform parent;

        public Sprite[] capsIcons;
        private KeyboardKey capsKey;

        public static Action<string> OnEnter { get; set; }
        public static Action<string> OnClick { get; set; }
        public static Action<char> OnClickedCharacter { get; set; }
        public static Action<Vector2> OnAnchoredPositonChanged { get; set; }
        public static Action OnEndTransition { get; set; }
        public static Action OnClose { get; set; }
        public static Action OnNewLine { get; set; }
        public static Action<string> OnBackSpace { get; set; }
        public static Action OnBackSpaceCleared { get; set; }

        private void Awake()
        {
            hideTime = float.PositiveInfinity;
            keys = parent.GetComponentsInChildren<KeyboardKey>();
            foreach(var k in keys)
            {
                if(k.type is KeyboardKey.KeyType.Function)
                {
                    if(k.function is KeyboardKey.FunctionKeys.Caps)
                    {
                        capsKey = k;
                    }
                }
            }

            Clear();
        }

        private void Update()
        {
            if (hideTime > hideSpeed) return;
            hideTime += Time.deltaTime;

            self.anchoredPosition = Vector2.up * Mathf.Lerp(minH, maxH, hideTime / Mathf.Max(float.Epsilon, hideSpeed));
            OnAnchoredPositonChanged?.Invoke(self.anchoredPosition);

            if(hideTime > hideSpeed)
            {
                OnEndTransition?.Invoke();
                OnEndTransition = () => { };
            }
        }

        public void AudioFeedback()
        {
            audioSrc.Play();
        }

        public void HapticFeedback()
        {
            Vibration.VibratePeek();
        }

        public static void Open()
        {
            hideTime = float.PositiveInfinity;
            Instance.self.anchoredPosition = Vector2.up * Instance.minH;
        }

        public static void Close()
        {
            if(hideTime >= float.PositiveInfinity)
            {
                hideTime = 0;
            }

            Clear();
            OnClickedCharacter = (char c) => { };
            OnEnter = (string text) => { };
            OnAnchoredPositonChanged = (Vector2 vec) => { };
            OnBackSpace = (string text) => { };
            OnBackSpaceCleared = () => { };
        }

        public static void Clear()
        {
            Instance.text = "";
            Instance.currentTextDisplay.text = Instance.text;
            Instance.SetCaps(true);
            OnClose?.Invoke();
            OnClose = () => { };
        }

        public void OnClickCharacter(char c)
        {
            text += c;
            OnClickedCharacter?.Invoke(c);

            if (!capsLock)
            {
                SetCaps(false);
            }

            if(c == ' ')
            {
                currentTextDisplay.text = "";
            }

            AudioFeedback();
            HapticFeedback();
        }

        public void OnSubmit()
        {

        }

        public void OnBackspace()
        {
            if(text.Length < 2)
            {
                if (text.Length == 1)
                {
                    text = "";
                }
                else
                {
                    OnBackSpaceCleared?.Invoke();
                }

                return;
            }

            text = text[..^1];
            OnBackSpace?.Invoke(text);
            HapticFeedback();
        }

        public void OnCapsLock()
        {
            if(!capsLock && caps)
            {
                capsLock = true;
                caps = true;
            }else if(capsLock)
            {
                caps = false;
                capsLock = false;
            }
            else
            {
                caps = true;
            }

            SetCaps(caps);
            HapticFeedback();
        }

        private void SetCaps(bool state)
        {
            Instance.caps = state;
            capsKey.icon.sprite = capsIcons[capsLock ? 2 : caps ? 1 : 0];
            foreach (var k in Instance.keys)
            {
                k.CapsLock(Instance.caps);
            }
        }

        public void OnAction()
        {
            string[] words = text.Split(' ');
            currentTextDisplay.text = words.Length > 0 ? $"{words[^1]}" : "";
            OnClick?.Invoke(text);
        }

        public static void Subscribe(Action<string> action)
        {
            OnEnter += action;
        }

        public static void OnClickCallback(Action<string> action)
        {
            OnClick += action;
        }

        public void Enter()
        {
            text += '\n';
            OnNewLine?.Invoke();
        }
    }
}