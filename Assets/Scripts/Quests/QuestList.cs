using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable
    {
        private List<QuestStatus> _statuses = new List<QuestStatus>();

        public event Action OnUpdate;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest))
            {
                return;
            }
            _statuses.Add(new QuestStatus(quest));
            OnUpdate?.Invoke();
        }

        private bool HasQuest(Quest quest)
        {
            return _statuses.Any(status => status.Quest == quest);
        }

        public IEnumerable<QuestStatus> Statuses => _statuses;

        public void CompleteObjective(Quest quest, string objective)
        {
            GetQuestStatus(quest).CompleteObjective(objective);
            OnUpdate?.Invoke();
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            return _statuses.Find(status => status.Quest == quest);
        }

        public object CaptureState()
        {
            var state = new List<object>();
            foreach (var status in _statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            if (!(state is List<object> stateList)) return;

            _statuses.Clear();
            foreach (var objectState in stateList)
            {
                _statuses.Add(new QuestStatus(objectState));
            }
        }
    }
}