using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue currentDialogue;

        public string Text => currentDialogue == null ? "" : currentDialogue.GetRootNode().Text;
    }
}