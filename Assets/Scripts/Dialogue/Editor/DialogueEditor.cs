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
        [NonSerialized] private DialogueNode _deletingNode;
        [NonSerialized] private DialogueNode _linkingParentNode;
        private Vector2 _scrollPosition;
        [NonSerialized] private bool _draggingCanvas;
        [NonSerialized] private Vector2 _draggingCanvasOffset;
        
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

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                GUILayoutUtility.GetRect(4000, 4000);
                foreach (var node in _selectedDialogue.Nodes)
                {
                    DrawConnections(node);
                }
                foreach (var node in _selectedDialogue.Nodes)
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();

                if (_creatingNode != null)
                {
                    Undo.RecordObject(_selectedDialogue, "Added Dialogue Node");
                    _selectedDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;
                }

                if (_deletingNode == null) return;
                
                Undo.RecordObject(_selectedDialogue, "Delete Dialogue Node");
                _selectedDialogue.DeleteNode(_deletingNode);
                _deletingNode = null;
            }
        }

        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when _draggingNode == null:
                    _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                    if (_draggingNode != null)
                    {
                        _draggingOffset = _draggingNode.rect.position - Event.current.mousePosition;
                    }
                    else
                    {
                        _draggingCanvas = true;
                        _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    }
                    break;
                case EventType.MouseDrag when _draggingNode != null:
                    Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");
                    _draggingNode.rect.position = Event.current.mousePosition + _draggingOffset;
                    GUI.changed = true;
                    break;
                case EventType.MouseDrag when _draggingCanvas:
                    _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                    GUI.changed = true;
                    break;
                case EventType.MouseUp when _draggingNode != null:
                    _draggingNode = null;
                    break;
                case EventType.MouseDrag when _draggingCanvas:
                    _draggingCanvas = false;
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
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("x"))
            {
                _deletingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    _linkingParentNode = node;
                }
            }
            else if (node == _linkingParentNode)
            {
                if (GUILayout.Button("cancel"))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_linkingParentNode.children.Contains(node.uniqueId))
            {
                if (GUILayout.Button("unlink"))
                {
                    _linkingParentNode.children.Remove(node.uniqueId);
                }
            }
            else
            {
                if (!GUILayout.Button("child")) return;
                
                Undo.RecordObject(_selectedDialogue, "Add Dialogue Link");
                _linkingParentNode.children.Add(node.uniqueId);
                _linkingParentNode = null;
            }
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