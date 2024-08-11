using UnityEngine;
using FPHorror.Gameplay;
using Unity.VisualScripting;

namespace FPHorror.Game
{
    public class ObjectiveManager : MonoBehaviour
    {
        public AllObjectivesCompletedEvent OnAllObjectivesCompleted = new AllObjectivesCompletedEvent();

        private int totalObjectives = 2; // Suponha que existam 3 objetivos
        private int completedObjectives = 0;

        public void CompleteObjective()
        {
            completedObjectives++;
            if (completedObjectives >= totalObjectives)
            {
                Debug.Log("All Objectives completed!");
                OnAllObjectivesCompleted.Invoke();
            }
        }
    }
}