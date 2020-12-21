using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public void Next()
        {
            var children = currentDialogue.GetAllChildren(_currentNode).ToArray();
            _currentNode = children[Random.Range(0, children.Length)];
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(_currentNode).Any();
        }
    }
}