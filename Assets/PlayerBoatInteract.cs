using UnityEngine;
using System.Collections;

public class PlayerBoatInteract : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    BoatInteract boatInteractionScript;
    [SerializeField]
    PlayerMove movementScript;
    [SerializeField]
    CharacterMotor mainMotor;
    [SerializeField]
    PlayerBoatControls boatMotor;
    [SerializeField]
    Behaviour[] toDisableInBoat;
    [SerializeField]
    CameraFollow cameraScript;
    bool inBoat;
    #endregion

    #region PublicProperties
          
    #endregion

    #region UnityFunctions
        void Start()
        {
            
        }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && boatInteractionScript.ReadyToEnter && !inBoat)
        {
            BoatToggle(true);
        }else if (Input.GetButtonDown("Interact") && inBoat)
        {
            BoatToggle(false);
        }
        if (inBoat)
        {
            transform.position = boatInteractionScript.transform.position + boatInteractionScript.ModelPosition;
        }
    }
    #endregion

    #region CustomFunctions
    void BoatToggle(bool state)
    {
        foreach (Behaviour behav in toDisableInBoat)
        {
            behav.enabled = !state;
        }
        movementScript.CharacterMotor = state ? boatMotor : mainMotor;
        movementScript.CharacterMotor.SetupRigidbody();
        GetComponent<Rigidbody>().isKinematic = !state;
        cameraScript.target = state ? boatInteractionScript.transform : transform;
        inBoat = state;

    }
    #endregion
}