using UnityEngine;

namespace FPHorror.Gameplay
{
    public class DamageDealt : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 10f;

        public void DealDamage()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health playerHealth = player.GetComponent<Health>();

                if (playerHealth != null)
                {
                    // Calcula a direção do dano como a direção do ataque para o jogador
                    Vector3 damageDirection = (player.transform.position - transform.position).normalized;
                    
                    // Chama TakeDamage com a direção do dano
                    playerHealth.TakeDamage((ushort)0, damageAmount, damageDirection);
                }
            }
        }
    }
}