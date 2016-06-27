using UnityEngine;
using System.Collections;

public class BoatInteract : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    private Vector3 modelPosition;
    #endregion

    #region PublicProperties
        public bool ReadyToEnter { get; set; }
        public Vector3 ModelPosition{get{return modelPosition;}set{modelPosition = value;}}
    #endregion

    #region UnityFunctions
    void Start()
        {
            
        }

        void Update()
        {
            
        }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            ReadyToEnter = true;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            ReadyToEnter = false;
        }
    }
    #endregion

    #region CustomFunctions

    #endregion
}