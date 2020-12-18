using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

        private readonly Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (var node in nodes)
            {
                _nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> Nodes => nodes;

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            return from childId in parentNode.children where _nodeLookup.ContainsKey(childId) select _nodeLookup[childId];
        }

        public void CreateNode(DialogueNode parent)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = System.Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (!(parent is null)) parent.children.Add(newNode.name);
            nodes.Add(newNode);
            // AssetDatabase.AddObjectToAsset(newNode, this);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in Nodes)
            {
                node.children.Remove(nodeToDelete.name);
            }
        }

        public void OnBeforeSerialize()
        {
            if (nodes.Count == 0) CreateNode(null);

            if (AssetDatabase.GetAssetPath(this) == "") return;
            
            foreach (var node in Nodes)
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
        }

        public void OnAfterDeserialize()
        {
        }
    }
}