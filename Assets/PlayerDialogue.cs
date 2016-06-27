using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;
using Yarn.Unity.Example;

public class PlayerDialogue : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    float interactionRadius = 4f;
    #endregion

    #region PublicProperties
          
    #endregion

    #region UnityFunctions
        void Start()
        {
            
        }

        void Update()
        {
            if (Input.GetButtonDown("Interact"))
                CheckForNearbyNPC();
        }
    #endregion

    #region CustomFunctions
    public void CheckForNearbyNPC()
    {
        // Find all DialogueParticipants, and filter them to
        // those that have a Yarn start node and are in range; 
        // then start a conversation with the first one
        var allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
        var target = allParticipants.Find(delegate (NPC p) {
            return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null)
        {
            // Kick off the dialogue at this node.
            FindObjectOfType<DialogueRunner>().StartDialogue(target.talkToNode);
        }
    }
    #endregion
}