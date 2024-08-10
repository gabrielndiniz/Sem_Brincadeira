using UnityEngine;
using UnityEngine.UI;

namespace FPHorror.Widget
{
    public class MessageDisplay : MonoBehaviour
    {
        [SerializeField] private Text messageText;
        [SerializeField] private Image background;
        [SerializeField] private float fadeSpeed = 2f;

        private bool isDisplayingMessage = false;
        private float targetAlpha = 0f;

        private void Update()
        {
            // Faz o fade in/out do texto e do background
            UpdateAlpha();
        }

        public void ShowMessage(string message, bool isVictory)
        {
            messageText.text = message;
            SetColors(isVictory);
            targetAlpha = 1f;
            isDisplayingMessage = true;
        }

        public void HideMessage()
        {
            targetAlpha = 0f;
            isDisplayingMessage = false;
        }

        private void UpdateAlpha()
        {
            Color textColor = messageText.color;
            textColor.a = Mathf.Lerp(textColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
            messageText.color = textColor;

            Color backgroundColor = background.color;
            backgroundColor.a = Mathf.Lerp(backgroundColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
            background.color = backgroundColor;
        }

        private void SetColors(bool isVictory)
        {
            if (isVictory)
            {
                background.color = Color.white;
                messageText.color = new Color(1f, 0.84f, 0f); // Dourado
            }
            else
            {
                background.color = Color.black;
                messageText.color = Color.red;
            }
        }
    }
}