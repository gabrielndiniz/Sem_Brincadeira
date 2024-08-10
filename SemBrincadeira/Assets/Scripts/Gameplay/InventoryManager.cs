using System.Collections.Generic;
using UnityEngine;

namespace FPHorror.Gameplay
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        private List<Pickup> inventory = new List<Pickup>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddToInventory(Pickup pickup)
        {
            inventory.Add(pickup);
        }

        public bool HasPickup(Pickup pickup)
        {
            return inventory.Contains(pickup);
        }
    }
}