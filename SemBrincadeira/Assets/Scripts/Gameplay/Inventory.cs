using System.Collections.Generic;
using UnityEngine;

namespace FPHorror.Gameplay
{
    public class Inventory : MonoBehaviour
    {
        private List<Pickup> _pickups = new List<Pickup>();

        public void AddPickup(Pickup pickup)
        {
            if (!_pickups.Contains(pickup))
            {
                _pickups.Add(pickup);
            }
        }

        public bool HasPickup()
        {
            // Verifique se o inventário contém algum pickup
            return _pickups.Count > 0;
        }
    }
}