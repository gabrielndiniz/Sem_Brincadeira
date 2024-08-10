using UnityEngine;
using UnityEngine.Events;

namespace FPHorror
{
    public class Health : MonoBehaviour
    {
        public Health Parent { get; set; }
        public UnityAction OnDeath;

        [SerializeField]
        private float penetrationResistance = 0.5f;
        public float PenetrationResistance
        {
            get => penetrationResistance;
            set => penetrationResistance = value;
        }

        [SerializeField]
        private float damageMultiplier = 1f;
        public float DamageMultiplier
        {
            get => damageMultiplier;
            set => damageMultiplier = value;
        }

        [SerializeField]
        private float healthPoints = 100f;
        public float HealthPoints
        {
            get => healthPoints;
            private set => healthPoints = value;
        }

        [SerializeField]
        private float maxHealthPoints = 100f;
        public float MaxHealthPoints
        {
            get => maxHealthPoints;
            set => maxHealthPoints = value;
        }

        public UnityEvent OnDamageTaken { get; set; } = new UnityEvent();

        private float lastDamage;
        private int lastDamageIndex;

        // New fields for tracking damage direction
        private Vector3 lastDamageDirection;
        private bool hasLastDamageDirection;

        [Header("Regeneration Settings")]
        [SerializeField]
        private float regenerationAmountPerSecond = 1f;
        [SerializeField]
        private float regenerationDelay = 5f;
        [SerializeField]
        private float maxRegenerationPercentage = 50f;

        private float regenerationTimer;
        private bool canRegenerate = true;

        public bool Alive => HealthPoints > 0f;

        void Update()
        {
            if (Alive && canRegenerate)
            {
                RegenerateHealth();
            }
        }

        public void TakeDamage(ushort senderID, float damage, Vector3 damageDirection) => TakeDamage(senderID, damage, Time.frameCount, damageDirection);

        private void TakeDamage(ushort senderID, float damage, int damageIndex, Vector3 damageDirection)
        {
            damage *= DamageMultiplier;

            if (lastDamageIndex == damageIndex)
            {
                if (damage > lastDamage)
                    TakeDamage(senderID, -lastDamage, damageIndex, damageDirection);
                else return;
            }
            lastDamage = damage;
            lastDamageIndex = damageIndex;
            lastDamageDirection = damageDirection; // Save the direction of the last damage
            hasLastDamageDirection = true;

            if (Parent != null)
            {
                Parent.TakeDamage(senderID, damage, damageDirection);
            }
            else if (Alive)
            {
                HealthPoints -= damage;
                canRegenerate = false;
                regenerationTimer = 0f;

                // Dispara o evento de dano
                OnDamageTaken.Invoke();

                if (HealthPoints <= 0f)
                {
                    HealthPoints = 0f;
                    OnDeath.Invoke();
                }
            }
        }

        public Vector3 GetLastDamageDirection()
        {
            if (hasLastDamageDirection)
            {
                return lastDamageDirection;
            }
            else
            {
                Debug.LogWarning("No damage direction recorded.");
                return Vector3.zero;
            }
        }

        private void RegenerateHealth()
        {
            if (!canRegenerate)
            {
                regenerationTimer += Time.deltaTime;
                if (regenerationTimer >= regenerationDelay)
                {
                    canRegenerate = true;
                }
            }
            else
            {
                float maxRegenerationAmount = MaxHealthPoints * (maxRegenerationPercentage / 100f);
                float targetHealth = Mathf.Min(MaxHealthPoints - maxRegenerationAmount, MaxHealthPoints);
                HealthPoints = Mathf.Min(HealthPoints + regenerationAmountPerSecond * Time.deltaTime, targetHealth);
            }
        }
    }
}
