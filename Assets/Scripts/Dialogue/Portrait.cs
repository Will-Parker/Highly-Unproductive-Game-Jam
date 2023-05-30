using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : MonoBehaviour
{
    public void RemoveFromScene()
    {
        if (!DialogueManager.GetInstance().participants.Exists(participant => participant.GO == gameObject))
        {
            Debug.LogWarning("Participant to be removed was not found in list of participants");
        }
        else
        {
            DialogueManager.Participant participantExiting = DialogueManager.GetInstance().participants.Find(participant => participant.GO == gameObject);
            DialogueManager.GetInstance().participants.Remove(participantExiting);
            if (DialogueManager.GetInstance().speaker.Value.Equals(participantExiting))
            {
                DialogueManager.GetInstance().speaker = null;
            }
        }
        Destroy(gameObject);
    }
}
