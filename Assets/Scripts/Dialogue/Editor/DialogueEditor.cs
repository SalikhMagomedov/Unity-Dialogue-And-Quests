using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private const float CanvasSize = 4000;
        private const float BackgroundSize = 50;
        
        private Dialogue _selectedDialogue;
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private GUIStyle _playerNodeStyle;
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
            
            _playerNodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node1") as Texture2D,
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
                
                var canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
                var backgroundTex = Resources.Load("background") as Texture2D;
                var texCoords = new Rect(0, 0, CanvasSize / BackgroundSize, CanvasSize / BackgroundSize);

                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);
                
                foreach (var node in _selectedDialogue.Nodes)
                {
                    DrawConnections(node);
                }
                foreach (var node in _selectedDialogue.Nodes)
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();

                if (!(_creatingNode is null))
                {
                    _selectedDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;
                }

                if (_deletingNode is null) return;
                
                _selectedDialogue.DeleteNode(_deletingNode);
                _deletingNode = null;
            }
        }

        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when _draggingNode is null:
                    _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                    if (!(_draggingNode is null))
                    {
                        _draggingOffset = _draggingNode.Rect.position - Event.current.mousePosition;
                        Selection.activeObject = _draggingNode;
                    }
                    else
                    {
                        _draggingCanvas = true;
                        _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                        Selection.activeObject = _selectedDialogue;
                    }
                    break;
                case EventType.MouseDrag when !(_draggingNode is null):
                    _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
                    GUI.changed = true;
                    break;
                case EventType.MouseDrag when _draggingCanvas:
                    _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                    GUI.changed = true;
                    break;
                case EventType.MouseUp when !(_draggingNode is null):
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
                if (node.Rect.Contains(point)) result = node;
            }

            return result;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, node.IsPlayerSpeaking ? _playerNodeStyle : _nodeStyle);
            
            node.Text = EditorGUILayout.TextField(node.Text);
            
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
            else if (_linkingParentNode.Children.Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    _linkingParentNode.RemoveChild(node.name);
                }
            }
            else
            {
                if (!GUILayout.Button("child")) return;
                
                _linkingParentNode.AddChild(node.name);
                _linkingParentNode = null;
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            var startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);
            foreach (var childNode in _selectedDialogue.GetAllChildren(node))
            {
                var endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);
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