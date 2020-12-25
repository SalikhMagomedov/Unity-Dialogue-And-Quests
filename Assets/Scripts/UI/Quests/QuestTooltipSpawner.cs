using GameDevTV.Core.UI.Tooltips;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            var quest = GetComponent<QuestItemUi>().Quest;
            tooltip.GetComponent<QuestTooltipUi>().Setup(quest);
        }
    }
}