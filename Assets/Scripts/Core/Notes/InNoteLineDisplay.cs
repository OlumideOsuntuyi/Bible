using System.Xml;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Visuals;
using Visuals.Module;

namespace Core.Notes
{
    public class InNoteLineDisplay : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerMoveHandler
    {
        public RectTransform rect;
        public InsertableText text;
        public int _caretPosition;
        public static int caretPosition = 0;
        public static Image caretImage;
        public float[] fontSizes;

        public int line;
        public string txt;
        public static float blinkTimer;
        public static bool isCaretVisible;
        public static float blinkDuration = 0.5f; // windows default speed
        public NoteLineFormat format;

        public void Set(int line, string text, NoteLineFormat format)
        {
            txt = text;
            this.line = line;
            this.format = format;
            var tx = this.text._text;
            tx.fontSize = fontSizes[(int)format.size];

            if(format.list is not NoteLineFormat.ListType.None)
            {
                this.text._text.margin = new Vector4(NoteLineFormat.LIST_INDENT + (format.indentSize * 25), 0, 0, 0);
                switch(format.list)
                {
                    case NoteLineFormat.ListType.Dotted:
                        {
                            text = $"{NoteLineFormat.BULLET_POINT}  {text}";
                        }break;
                    case NoteLineFormat.ListType.Numbered:
                        {
                            text = $"{Mathf.Max(1, format.lineNumber)}.  {text}";
                        }
                        break;
                    default:break;
                }
            }
            else
            {
                this.text._text.margin = new Vector4((format.indentSize * 25), 0, 0, 0);
            }

            if (format.size is not NoteLineFormat.SizeType.N || format.bold)
            {
                text = $"<b>{text}</b>";
            }

            if(format.italic)
            {
                text = $"<i>{text}</i>";
            }

            if(format.underline)
            {
                text = $"<u>{text}</u>";
            }

            if(format.mark)
            {
                text = $"<mark=#{ColorUtility.ToHtmlStringRGB(NoteEditor.GetMarkColor())}>{text}</mark>";
            }

            tx.text = text;
            this.text._text.color = NoteEditor.GetSwatchColor(format.color);
            this.text.Resize();
        }

        public void Reload()
        {
            Set(line, txt, format);
        }

        public void SelectLine()
        {
            NoteEditor.Instance.selectedLine = this;
            NoteEditor.Instance.OpenNewLine();
            SetCaretPosition(int.MaxValue);
            ResetBlinking();
        }

        private Vector3 Center(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return (a + b + c + d) * 0.25f;
        }
        private Vector3 GetCharPos(TMP_CharacterInfo c)
        {
            return Center(c.topLeft, c.topRight, c.bottomLeft, c.bottomRight);
        }

        public int FindClosestCharacterIndex(Vector2 localPoint)
        {
            TMP_TextInfo textInfo = text._text.textInfo;
            float closestDistance = float.MaxValue;
            int closestIndex = 0;

            // Check each character
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                Vector3 charPos = GetCharPos(textInfo.characterInfo[i]);
                float distance = Vector2.Distance(localPoint, charPos);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            // Check if click is after the last character
            if (textInfo.characterCount > 0)
            {
                Vector3 lastCharPos = GetCharPos(textInfo.characterInfo[textInfo.characterCount - 1]);
                float distanceToEnd = Vector2.Distance(localPoint, lastCharPos);

                if (distanceToEnd < closestDistance)
                {
                    closestIndex = textInfo.characterCount;
                }
            }

            return closestIndex;
        }


        public static void HandleCaretBlinking()
        {
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkDuration)
            {
                isCaretVisible = !isCaretVisible;
                caretImage.color = isCaretVisible ? ThemeManager.Colors[(int)ThemeColorType.DimText] : Color.clear;
                blinkTimer = 0f;
            }
        }

        public static void ResetBlinking()
        {
            isCaretVisible = false;
            blinkTimer = 0f;
            caretImage.color = Color.clear;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this != NoteEditor.Instance.selectedLine)
            {
                SelectLine();
                SetCaretPosition();
            }

            Trace(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this != NoteEditor.Instance.selectedLine)
            {
                SelectLine();
                SetCaretPosition();
            }

            Trace(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (this != NoteEditor.Instance.selectedLine)
            {
                SelectLine();
                SetCaretPosition();
            }


            Trace(eventData);
        }

        private void Trace(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.isValid)
            {
                //SetCaretPosition(TMP_TextUtilities.FindNearestCharacter(text._text, eventData.position, Camera.main, true));
                //return;

                // Handle the touch/click
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    text._text.rectTransform,
                    eventData.position,  // This is touch position on mobile
                    eventData.pressEventCamera, // Better than uiCamera
                    out localPoint
                );

                int closestCharIndex = FindClosestCharacterIndex(localPoint);
                SetCaretPosition(closestCharIndex);
            }
        }

        public void MoveCaret(int direction)
        {
            caretPosition = Mathf.Clamp(caretPosition + direction, 0, txt.Length);
            UpdateCaretPosition();
            ResetBlinking();
        }

        void SetCaretPosition(int position)
        {
            caretPosition = Mathf.Clamp(position, 0, txt.Length);
            caretImage.rectTransform.sizeDelta = new Vector2(caretImage.rectTransform.sizeDelta.x, fontSizes[(int)format.size]);

            UpdateCaretPosition();
            ResetBlinking();
        }

        public void SetCaretPosition()
        {
            SetCaretPosition(txt.Length);
        }

        void UpdateCaretPosition()
        {
            TMP_TextInfo textInfo = text._text.textInfo;

            Vector3 caretPos = Vector3.zero;

            if (caretPosition == 0)
            {
                // Position at the beginning
                if (textInfo.characterCount > 0)
                {
                    caretPos = textInfo.characterInfo[0].topLeft;
                }
                else
                {
                    // Empty text, position at text origin
                    caretPos = Vector3.zero;
                }
            }
            else if (caretPosition >= textInfo.characterCount)
            {
                // Position at the end
                if (textInfo.characterCount > 0)
                {
                    TMP_CharacterInfo lastChar = textInfo.characterInfo[textInfo.characterCount - 1];
                    caretPos = lastChar.topRight;
                }
            }
            else
            {
                // Position between characters
                caretPos = textInfo.characterInfo[caretPosition].topRight;
            }

            NoteEditor.Instance.caretParent.position = rect.position;
            caretImage.rectTransform.anchoredPosition = caretPos;

            _caretPosition = caretPosition;
        }

        public static string InsertCharacter(string currentText, char character, int caretPosition)
        {
            currentText = currentText.Insert(caretPosition, character.ToString());
            return currentText;
        }

        public static string RemoveCharAtIndex(string input, int caretPosition)
        {
            if (caretPosition < 0 || caretPosition >= input.Length)
            {
                return input;
            }

            return input.Substring(0, caretPosition) + input.Substring(caretPosition + 1);
        }
    }
}