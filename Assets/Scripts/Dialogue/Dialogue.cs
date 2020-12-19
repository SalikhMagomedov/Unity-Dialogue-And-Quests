using System;
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
        [SerializeField] private Vector2 newNodeOffset = new Vector2(250, 0);

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
            return from childId in parentNode.Children where _nodeLookup.ContainsKey(childId) select _nodeLookup[childId];
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            var newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Delete Dialogue Node");
            nodes.Remove(nodeToDelete);
            
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            
            if (parent is null) return newNode;
            
            parent.AddChild(newNode.name);
            newNode.IsPlayerSpeaking = !parent.IsPlayerSpeaking;
            newNode.SetPosition(parent.Rect.position + newNodeOffset);
            
            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in Nodes)
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0) AddNode(MakeNode(null));

            if (AssetDatabase.GetAssetPath(this) == "") return;
            
            foreach (var node in Nodes)
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}