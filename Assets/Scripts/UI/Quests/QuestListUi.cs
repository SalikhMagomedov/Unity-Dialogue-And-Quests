﻿using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUi : MonoBehaviour
    {
        [SerializeField] private QuestItemUi questPrefab;

        private void Start()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            var tempQuests = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            foreach (var status in tempQuests.Statuses)
            {
                var questInstance = Instantiate(questPrefab, transform);
                questInstance.Setup(status);
            }
        }
    }
}