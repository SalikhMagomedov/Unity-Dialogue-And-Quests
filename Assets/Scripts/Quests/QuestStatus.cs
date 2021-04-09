using System.Collections.Generic;

namespace RPG.Quests
{
    public class QuestStatus
    {
        private Quest _quest;
        private readonly List<string> _completedObjectives = new List<string>();
        
        [System.Serializable]
        private class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            _quest = quest;
        }

        public QuestStatus(object objectState)
        {
            if (!(objectState is QuestStatusRecord state)) return;
            
            _quest = Quest.GetByName(state.questName);
            _completedObjectives = state.completedObjectives;
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

        public object CaptureState()
        {
            return new QuestStatusRecord {questName = _quest.name, completedObjectives = _completedObjectives};
        }
    }
}