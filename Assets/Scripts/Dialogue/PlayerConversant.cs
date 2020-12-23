using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;

        public event Action ConversationUpdated;

        public string Text => _currentNode == null ? "" : _currentNode.Text;

        public bool IsChoosing { get; private set; }

        public void Quit()
        {
            _currentDialogue = null;
            TriggerExitAction();
            _currentNode = null;
            IsChoosing = false;
            OnConversationUpdated();
        }

        public bool IsActive()
        {
            return _currentDialogue != null;
        }

        public void StartDialogue(Dialogue newDialogue)
        {
            _currentDialogue = newDialogue;
            _currentNode = _currentDialogue.GetRootNode();
            TriggerEnterAction();
            OnConversationUpdated();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return _currentDialogue.GetPlayerChildren(_currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            IsChoosing = false;
            Next();
        }

        public void Next()
        {
            var numPlayerResponses = _currentDialogue.GetPlayerChildren(_currentNode).Count();
            if (numPlayerResponses > 0)
            {
                IsChoosing = true;
                TriggerExitAction();
                OnConversationUpdated();
                return;
            }
            
            var children = _currentDialogue.GetAiChildren(_currentNode).ToArray();
            TriggerExitAction();
            _currentNode = children[Random.Range(0, children.Length)];
            TriggerEnterAction();
            OnConversationUpdated();
        }

        public bool HasNext()
        {
            return _currentDialogue.GetAllChildren(_currentNode).Any();
        }

        private void TriggerEnterAction()
        {
            if (_currentNode != null && _currentNode.OnEnterAction != "")
            {
                Debug.Log(_currentNode.OnEnterAction);
            }
        }

        private void TriggerExitAction()
        {
            if (_currentNode != null && _currentNode.OnExitAction != "")
            {
                Debug.Log(_currentNode.OnExitAction);
            }
        }

        private void OnConversationUpdated()
        {
            ConversationUpdated?.Invoke();
        }
    }
}