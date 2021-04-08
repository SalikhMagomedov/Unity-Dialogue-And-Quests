using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
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
    }
}