using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Project/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<string> objectives = new List<string>();

        public string Title => name;

        public int ObjectiveCount => objectives.Count;

        public IEnumerable<string> Objectives => objectives;

        public bool HasObjective(string objective)
        {
            return objectives.Contains(objective);
        }

        public static Quest GetByName(string questName)
        {
            return Resources.LoadAll<Quest>("").FirstOrDefault(quest => quest.name == questName);
        }
    }
}