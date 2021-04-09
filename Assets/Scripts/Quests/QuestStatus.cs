using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        private Quest _quest;
        private readonly List<string> _completedObjectives = new List<string>();

        public QuestStatus(Quest quest)
        {
            _quest = quest;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return _completedObjectives.Contains(objective);
        }

        public Quest Quest => _quest;
        public int CompletedCount => _completedObjectives.Count;

        public void CompleteObjective(string objective)
        {
            if(!_quest.HasObjective(objective))
                return;
            _completedObjectives.Add(objective);
        }
    }
}