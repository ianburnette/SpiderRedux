using UnityEngine;
using System.Collections;

public class AnchorHitDetection : MonoBehaviour {

    #region PrivateFields

    #endregion

    #region PublicProperties
    public bool OtherUsed { get; set; }
    public VineGrowth MyVineGrowth { get; set; } 
    #endregion

    #region UnityFunctions
        void Start()
        {
            
        }

        void Update()
        {
            
        }

    void OnDestroy()
    {
        if (!OtherUsed)
            MyVineGrowth.DestroyVine();
    }
    #endregion

    #region CustomFunctions
        
    #endregion
}