using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI progress;

        public void Setup(Quest quest)
        {
            Quest = quest;
            title.text = quest.Title;
            progress.text = $"0/{quest.ObjectiveCount}";
        }

        public Quest Quest { get; private set; }
    }
}