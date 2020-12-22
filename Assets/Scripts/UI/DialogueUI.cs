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
        [SerializeField] private GameObject aiResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;

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
            aiResponse.SetActive(!_playerConversant.IsChoosing);
            choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing);
            if (_playerConversant.IsChoosing)
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
                }
            }
            else
            {
                aiText.text = _playerConversant.Text;
                nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }
    }
}