using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Inventories;
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
            var status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }
            OnUpdate?.Invoke();
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.Rewards)
            {
                var success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            return _statuses.Find(status => status.Quest == quest);
        }

        public object CaptureState()
        {
            return _statuses.Select(status => status.CaptureState()).ToList();
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