using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueId;
        public string text;
        public List<string> children = new List<string>();
        public Rect rect = new Rect(0, 0, 200, 100);
    }
}