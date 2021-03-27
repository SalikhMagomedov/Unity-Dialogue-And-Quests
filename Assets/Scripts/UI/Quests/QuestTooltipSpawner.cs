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
            var status = GetComponent<QuestItemUi>().QuestStatus;
            tooltip.GetComponent<QuestTooltipUi>().Setup(status);
        }
    }
}