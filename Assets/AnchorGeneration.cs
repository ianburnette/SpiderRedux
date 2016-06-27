using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VineGrowth))]
public class AnchorGeneration : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    Vector2[] checkPositions;
    private float createMargin;
    private VineGrowth growthScript;
    private AnchorHitDetection firstAnchor, lastAnchor;
    #endregion

    #region PublicProperties
    public Vector3 StartLocation{ get; set; }
    public Vector3 EndLocation { get; set; }
    #endregion

    #region UnityFunctions
    void Start()
    {
        growthScript = GetComponent<VineGrowth>();
        firstAnchor = transform.GetChild(1).GetComponent<AnchorHitDetection>();
        lastAnchor = transform.GetChild(2).GetComponent<AnchorHitDetection>();
        firstAnchor.gameObject.SetActive(false);
        lastAnchor.gameObject.SetActive(false);
    }

    void Update()
    {   
            
    }
    #endregion

    #region CustomFunctions
    public void CreateAnchors()
    {
        AnchorCreate(StartLocation);
        AnchorCreate(EndLocation);
    }

    void AnchorCreate(Vector3 origin)
    {
        firstAnchor.gameObject.SetActive(true);
        lastAnchor.gameObject.SetActive(true);
        firstAnchor.MyVineGrowth = lastAnchor.MyVineGrowth = growthScript;
    }
    #endregion
}