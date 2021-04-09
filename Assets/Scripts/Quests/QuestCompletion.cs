using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;

        public void CompleteObjective()
        {
            var tempQuests = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            tempQuests.CompleteObjective(quest, objective);
        }
    }
}