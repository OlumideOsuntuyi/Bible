using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace LostOasis
{
    public class KeyboardKey : MonoBehaviour
    {
        public KeyType type;
        public FunctionKeys function;

        public Image background;
        public Image icon;
        public TMP_Text mainKey;
        public TMP_Text sideKey;

        public void CapsLock(bool caps)
        {
            if(type == KeyType.Character)
            {
                mainKey.text = caps ? mainKey.text.ToUpper() : mainKey.text.ToLower();
            }
        }

        public void Click()
        {
            switch(type)
            {
                case KeyType.Character: EnterCharacter(); break;
                case KeyType.Function: EnterFunction(); break;
                case KeyType.Submit: EnterSubmit(); break;
                default:break;
            }
        }


        private void EnterCharacter()
        {
            Keyboard.Instance.OnClickCharacter(mainKey.text[0]);

            Keyboard.Instance.OnAction();
        }

        private void EnterFunction()
        {
            switch (function)
            {
                case FunctionKeys.Caps: Keyboard.Instance.OnCapsLock(); break;
                case FunctionKeys.Space: Keyboard.Instance.OnClickCharacter(' '); break;
                case FunctionKeys.Backspace: Keyboard.Instance.OnBackspace(); Keyboard.Instance.OnAction(); break;
                case FunctionKeys.Enter: Keyboard.Instance.Enter(); break;
                default: break;
            }
        }

        private void EnterSubmit()
        {
            Keyboard.Instance.Enter();
        }


        public enum KeyType
        {
            None, Character, Function, Submit
        }

        public enum FunctionKeys
        {
           None, Caps, NumOpen, EmojiOpen, Lang, Space, Backspace, Enter, 
        }

    }
}