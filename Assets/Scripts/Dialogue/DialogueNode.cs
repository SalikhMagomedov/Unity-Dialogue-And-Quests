using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueId;
        public string text;
        public string[] children;
        public Rect rect = new Rect(0, 0, 200, 100);
    }
}