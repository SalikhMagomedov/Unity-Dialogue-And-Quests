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
        private AiConversant _currentConversant;

        public event Action ConversationUpdated;

        public string Text => _currentNode == null ? "" : _currentNode.Text;

        public bool IsChoosing { get; private set; }

        public bool IsActive()
        {
            return _currentDialogue != null;
        }

        public void StartDialogue(AiConversant newConversant, Dialogue newDialogue)
        {
            _currentConversant = newConversant;
            _currentDialogue = newDialogue;
            _currentNode = _currentDialogue.GetRootNode();
            TriggerEnterAction();
            OnConversationUpdated();
        }

        public void Quit()
        {
            _currentDialogue = null;
            TriggerExitAction();
            _currentConversant = null;
            _currentNode = null;
            IsChoosing = false;
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
            if (!(_currentNode is null))
            {
                TriggerAction(_currentNode.OnEnterAction);
            }
        }

        private void TriggerExitAction()
        {
            if (!(_currentNode is null))
            {
                TriggerAction(_currentNode.OnExitAction);
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (var trigger in _currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        private void OnConversationUpdated()
        {
            ConversationUpdated?.Invoke();
        }
    }
}