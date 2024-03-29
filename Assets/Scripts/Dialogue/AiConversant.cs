﻿using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AiConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string aiName;
        [SerializeField] private Dialogue dialogue;

        public string AiName => aiName;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue is null) return false;

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
            }
            return true;
        }
    }
}