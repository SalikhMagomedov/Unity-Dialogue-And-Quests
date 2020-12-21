using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant _playerConversant;
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Button nextButton;
        

        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        private void Start()
        {
            nextButton.onClick.AddListener(Next);

            UpdateUI();
        }

        private void Next()
        {
            _playerConversant.Next();
            UpdateUI();
        }

        private void UpdateUI()
        {
            aiText.text = _playerConversant.Text;
            nextButton.gameObject.SetActive(_playerConversant.HasNext());
        }
    }
}