﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue testDialogue;

        private Dialogue _currentDialogue = null;
        private DialogueNode _currentNode;

        public event Action ConversationUpdated;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            
            StartDialogue(testDialogue);
        }

        public string Text => _currentNode == null ? "" : _currentNode.Text;

        public bool IsChoosing { get; private set; }

        public bool IsActive()
        {
            return _currentDialogue != null;
        }

        private void StartDialogue(Dialogue newDialogue)
        {
            _currentDialogue = newDialogue;
            _currentNode = _currentDialogue.GetRootNode();
            OnConversationUpdated();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return _currentDialogue.GetPlayerChildren(_currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            IsChoosing = false;
            Next();
        }

        public void Next()
        {
            var numPlayerResponses = _currentDialogue.GetPlayerChildren(_currentNode).Count();
            if (numPlayerResponses > 0)
            {
                IsChoosing = true;
                OnConversationUpdated();
                return;
            }
            
            var children = _currentDialogue.GetAiChildren(_currentNode).ToArray();
            _currentNode = children[Random.Range(0, children.Length)];
            OnConversationUpdated();
        }

        public bool HasNext()
        {
            return _currentDialogue.GetAllChildren(_currentNode).Any();
        }

        protected virtual void OnConversationUpdated()
        {
            ConversationUpdated?.Invoke();
        }
    }
}