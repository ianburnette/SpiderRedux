using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerMove))]
public class PlayerVineGrow : MonoBehaviour {

#region PrivateFields
    [SerializeField]
    Transform vineBase, vineParent;
    [SerializeField]
    float vineBaseOffset;
    [SerializeField]
    bool growingVine;
    [SerializeField]
    int vineMax;
    PlayerMove moveScript;
    Transform currentVine;
    VineGrowth vineScript;


    #endregion

    #region PublicProperties
    public bool GrowingVine{get{return growingVine;}set {growingVine = value;}}
    public List<GameObject> CurrentVines { get; set; }
    #endregion

    #region UnityFunctions
    void Start()
    {
        moveScript = GetComponent<PlayerMove>();
        CurrentVines = new List<GameObject>();
        
    }


void Update()
{
    if (Input.GetButtonDown("Plant") && moveScript.GroundTag == "GroundGrowable" && !growingVine)
        {
            GrowVine();
        }
    else if (Input.GetButtonDown("Plant") && growingVine)
        {
            CancelVine();
        }
}
#endregion

#region CustomFunctions
    void GrowVine()
    {
        if (CurrentVines != null)
        {
            if (CurrentVines.Count > vineMax)
            {
                CurrentVines[0].GetComponent<VineGrowth>().DestroyVine();
            }
        }
        currentVine = (Transform)Instantiate(vineBase, new Vector3(transform.position.x, transform.position.y - vineBaseOffset, transform.position.z), Quaternion.identity);
        currentVine.parent = vineParent;
        vineScript = currentVine.GetComponent<VineGrowth>();
        vineScript.playerVine = this;
        vineScript.Player = transform;
        growingVine = true;
    }

    void CancelVine()
    {
        vineScript.StopGrowing();
    }
#endregion
}