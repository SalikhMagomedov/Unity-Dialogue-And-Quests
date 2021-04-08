using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUi : MonoBehaviour
    {
        [SerializeField] private QuestItemUi questPrefab;

        private QuestList _questList;

        private void Start()
        {
            _questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            _questList.OnUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var status in _questList.Statuses)
            {
                var questInstance = Instantiate(questPrefab, transform);
                questInstance.Setup(status);
            }
        }
    }
}