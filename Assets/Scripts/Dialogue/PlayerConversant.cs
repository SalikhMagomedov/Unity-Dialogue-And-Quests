using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue currentDialogue;

        private DialogueNode _currentNode;

        private void Awake()
        {
            _currentNode = currentDialogue.GetRootNode();
        }

        public string Text => _currentNode == null ? "" : _currentNode.Text;

        public bool IsChoosing { get; private set; }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(_currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            IsChoosing = false;
            Next();
        }

        public void Next()
        {
            var numPlayerResponses = currentDialogue.GetPlayerChildren(_currentNode).Count();
            if (numPlayerResponses > 0)
            {
                IsChoosing = true;
                return;
            }
            
            var children = currentDialogue.GetAiChildren(_currentNode).ToArray();
            _currentNode = children[Random.Range(0, children.Length)];
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(_currentNode).Any();
        }
    }
}