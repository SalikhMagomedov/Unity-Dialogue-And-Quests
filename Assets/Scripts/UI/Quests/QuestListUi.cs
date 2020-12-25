using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUi : MonoBehaviour
    {
        [SerializeField] private Quest[] tempQuests;
        [SerializeField] private QuestItemUi questPrefab;

        private void Start()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var quest in tempQuests)
            {
                var questInstance = Instantiate(questPrefab, transform);
                questInstance.Setup(quest);
            }
        }
    }
}