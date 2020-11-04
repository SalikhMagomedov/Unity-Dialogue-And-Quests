using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

        private readonly Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());
            }
        }
#endif

        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (var node in nodes)
            {
                _nodeLookup[node.uniqueId] = node;
            }
        }

        public IEnumerable<DialogueNode> Nodes => nodes;

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            return from childId in parentNode.children
                where _nodeLookup.ContainsKey(childId)
                select _nodeLookup[childId];
        }
    }
}