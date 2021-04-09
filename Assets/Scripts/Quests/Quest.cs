using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameDevTV.Inventories;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Project/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<Objective> objectives = new List<Objective>();
        [SerializeField] private List<Reward> rewards = new List<Reward>();
        
        [System.Serializable]
        private class Reward
        {
            public int number;
            public InventoryItem item;
        }
        
        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

        public string Title => name;

        public int ObjectiveCount => objectives.Count;

        public IEnumerable<Objective> Objectives => objectives;

        public bool HasObjective(string objectiveRef)
        {
            return objectives.Any(objective => objective.reference == objectiveRef);
        }

        public static Quest GetByName(string questName)
        {
            return Resources.LoadAll<Quest>("").FirstOrDefault(quest => quest.name == questName);
        }
    }
}