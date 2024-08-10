using UnityEngine;

namespace FPHorror.Gameplay
{
    public class AttackRange : MonoBehaviour
    {
        [SerializeField] private float attackRange = 3f;
        private Transform player;
        private bool isPlayerInRange = false;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                if (!isPlayerInRange)
                {
                    isPlayerInRange = true;
                    TriggerAttack();
                }
            }
            else
            {
                isPlayerInRange = false;
            }
        }

        private void TriggerAttack()
        {
            // Aciona o ataque - pode ser expandido com animação ou outros efeitos
            GetComponent<DamageDealt>().DealDamage();
        }

        void OnDrawGizmosSelected()
        {
            // Visualiza o alcance do ataque no editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}