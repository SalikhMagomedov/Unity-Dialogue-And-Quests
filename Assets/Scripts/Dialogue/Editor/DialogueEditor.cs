using Unity.MPE;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue _selectedDialogue;
        private GUIStyle _nodeStyle;
        private bool _dragging = false;
        
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
                    OnGUINode(node);
                }
            }
        }

        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when !_dragging:
                    _dragging = true;
                    break;
                case EventType.MouseDrag when _dragging:
                    Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");
                    _selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;
                    GUI.changed = true;
                    break;
                case EventType.MouseUp when _dragging:
                    _dragging = false;
                    break;
            }
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, _nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
            var newText = EditorGUILayout.TextField(node.text);
            var newUniqueId = EditorGUILayout.TextField(node.uniqueId);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Update Dialogue Text");

                node.text = newText;
                node.uniqueId = newUniqueId;
            }
            
            GUILayout.EndArea();
        }
    }
}