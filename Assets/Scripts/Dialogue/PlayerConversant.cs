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

        public IEnumerable<string> GetChoices()
        {
            yield return "Building A Choice List";
            yield return "Building A Choice List";
        }

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