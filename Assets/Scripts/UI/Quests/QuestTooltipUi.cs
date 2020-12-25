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
        
        public void Setup(Quest quest)
        {
            title.text = quest.name;
            
            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var objective in quest.Objectives)
            {
                var objectiveInstance = Instantiate(objectivePrefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective;
            }
        }
    }
}