﻿using RPG.Dialogue;
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
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject aiResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private TextMeshProUGUI conversantName;

        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            _playerConversant.ConversationUpdated += UpdateUI;
        }

        private void Start()
        {
            nextButton.onClick.AddListener(_playerConversant.Next);
            quitButton.onClick.AddListener(_playerConversant.Quit);
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.IsActive());
            
            if (!_playerConversant.IsActive()) return;

            conversantName.text = _playerConversant.GetCurrentConversantName();
            aiResponse.SetActive(!_playerConversant.IsChoosing);
            choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing);
            if (_playerConversant.IsChoosing)
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = _playerConversant.Text;
                nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform child in choiceRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var choice in _playerConversant.GetChoices())
            {
                var choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComponent = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();

                textComponent.text = choice.Text;
                
                var button = choiceInstance.GetComponentInChildren<Button>();
                
                button.onClick.AddListener(() =>
                {
                    _playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}