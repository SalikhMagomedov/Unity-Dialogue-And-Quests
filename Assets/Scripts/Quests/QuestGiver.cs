using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest quest;

        public void GiveQuest()
        {
            var tempQuests = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            tempQuests.AddQuest(quest);
        }
    }
}