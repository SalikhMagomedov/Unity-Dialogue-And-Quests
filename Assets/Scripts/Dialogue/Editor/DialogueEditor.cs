using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue _selectedDialogue;
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private DialogueNode _draggingNode;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private DialogueNode _creatingNode;
        
        [MenuItem("Window/Dialogue Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if (dialogue == null) return false;
            
            ShowWindow();
            
            return true;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            _nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node0") as Texture2D,
                    textColor = Color.white
                },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };
        }

        private void OnSelectionChanged()
        {
            var dialogue = Selection.activeObject as Dialogue;

            if (dialogue == null) return;
            
            _selectedDialogue = dialogue;
            Repaint();
        }

        private void OnGUI()
        {
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();
                foreach (var node in _selectedDialogue.Nodes)
                {
                    DrawConnections(node);
                }
                foreach (var node in _selectedDialogue.Nodes)
                {
                    DrawNode(node);
                }

                if (_creatingNode == null) return;
                
                Undo.RecordObject(_selectedDialogue, "Added Dialogue Node");
                _selectedDialogue.CreateNode(_creatingNode);
                _creatingNode = null;
            }
        }

        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when _draggingNode == null:
                    _draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                    if (_draggingNode != null)
                    {
                        _draggingOffset = _draggingNode.rect.position - Event.current.mousePosition;
                    }
                    break;
                case EventType.MouseDrag when _draggingNode != null:
                    Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");
                    _draggingNode.rect.position = Event.current.mousePosition + _draggingOffset;
                    GUI.changed = true;
                    break;
                case EventType.MouseUp when _draggingNode != null:
                    _draggingNode = null;
                    break;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode result = null;
            foreach (var node in _selectedDialogue.Nodes)
            {
                if (node.rect.Contains(point)) result = node;
            }

            return result;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, _nodeStyle);
            EditorGUI.BeginChangeCheck();

            var newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Update Dialogue Text");

                node.text = newText;
            }

            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }
            
            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            var startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach (var childNode in _selectedDialogue.GetAllChildren(node))
            {
                var endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                var controlPointOffset = endPosition - startPosition;
                
                controlPointOffset.y = 0;
                controlPointOffset.x *= .8f;
                
                Handles.DrawBezier(startPosition,
                    endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white,
                    null,
                    4f);
            }
        }
    }
}