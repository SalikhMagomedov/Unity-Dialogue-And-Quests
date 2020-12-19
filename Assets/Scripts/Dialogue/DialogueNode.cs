using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 100);

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