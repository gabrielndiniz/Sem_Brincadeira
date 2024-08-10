using UnityEngine;
using UnityEngine.UI;

namespace FPHorror.Widget
{

    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private Image lowHealthImage; // Imagem que aparece quando a vida está abaixo de 50%
        [SerializeField] private Image criticalHealthImage; // Imagem que aparece quando a vida está abaixo de 25%
        [SerializeField] private Image damageDirectionImage; // Imagem que mostra a direção do dano
        [SerializeField] private Health playerHealth; // Referência ao script que gerencia a vida do player
        [SerializeField] private float fadeSpeed = 2f; // Velocidade de fade in/out das imagens

        private void Update()
        {
            UpdateHealthIndicators();
            RotateDamageDirection();
        }

        private void UpdateHealthIndicators()
        {
            float healthPercent = playerHealth.HealthPoints;

            // Atualiza a transparência da imagem de baixa saúde
            if (healthPercent < 0.5f)
            {
                float alpha = Mathf.Lerp(0, 1, (0.5f - healthPercent) / 0.5f);
                SetImageAlpha(lowHealthImage, alpha);
            }
            else
            {
                SetImageAlpha(lowHealthImage, 0);
            }

            // Atualiza a transparência da imagem de saúde crítica
            if (healthPercent < 0.25f)
            {
                float alpha = Mathf.Lerp(0, 1, (0.25f - healthPercent) / 0.25f);
                SetImageAlpha(criticalHealthImage, alpha);
            }
            else
            {
                SetImageAlpha(criticalHealthImage, 0);
            }
        }

        private void RotateDamageDirection()
        {
            // Supondo que a direção do dano é passada por algum outro script
            Vector3 damageDirection = playerHealth.GetLastDamageDirection(); // Pega a direção do último dano
            float angle = Mathf.Atan2(damageDirection.x, damageDirection.z) * Mathf.Rad2Deg;
            damageDirectionImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);

            // Fade in a imagem da direção do dano
            SetImageAlpha(damageDirectionImage, 1f);
        }

        private void SetImageAlpha(Image image, float alpha)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(color.a, alpha, Time.deltaTime * fadeSpeed);
            image.color = color;
        }
    }
}