using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;

        public string OnEnterAction => onEnterAction;

        public string OnExitAction => onExitAction;

        public bool IsPlayerSpeaking
        {
            get => isPlayerSpeaking;
#if UNITY_EDITOR
            set
            {
                Undo.RecordObject(this, "Change Dialogue Speaker");
                isPlayerSpeaking = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public string Text
        {
            get { return text; }
#if UNITY_EDITOR
            set
            {
                if (text == value) return;

                Undo.RecordObject(this, "Update Dialogue Text");
                text = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public List<string> Children => children;

        public Rect Rect => rect;

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childId);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childId);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}