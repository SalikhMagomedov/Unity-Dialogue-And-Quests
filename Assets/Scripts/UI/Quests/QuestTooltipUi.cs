using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveIncompletePrefab;
        [SerializeField] private TextMeshProUGUI rewardText;
        
        public void Setup(QuestStatus status)
        {
            var quest = status.Quest;
            title.text = quest.name;
            
            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var objective in quest.Objectives)
            {
                var prefab = !status.IsObjectiveComplete(objective.reference)
                    ? objectiveIncompletePrefab
                    : objectivePrefab;
                var objectiveInstance = Instantiate(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }

            rewardText.text = GetRewardText(quest);
        }

        private static string GetRewardText(Quest quest)
        {
            var tempText = "";
            foreach (var reward in quest.Rewards)
            {
                if (tempText != "")
                {
                    tempText += ", ";
                }

                if (reward.number > 1)
                {
                    tempText += reward.number + " ";
                }
                tempText += reward.item.GetDisplayName();
            }

            if (tempText == "")
            {
                tempText = "No reward";
            }

            return tempText;
        }
    }
}