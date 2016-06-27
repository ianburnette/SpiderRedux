using UnityEngine;
using System.Collections;

public class SeedCarrier : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    Transform seed;
    [SerializeField]
    float disturbForce;
    [SerializeField]
    float baseSpring = 20f;
    [SerializeField]
    float baseDamper = 1f;
    [SerializeField]
    bool hasSeed;
    
    Rigidbody rb;

    #endregion

    #region PublicProperties

    #endregion

    #region UnityFunctions
    void Start()
    {
        baseSpring *= transform.parent.localScale.x;
        baseDamper += transform.parent.localScale.x;
        SetHingeControls();
        rb = GetComponent<Rigidbody>();
        disturbForce /= transform.parent.localScale.magnitude;
    }

    void Update()
    {
            
    }

    void OnTriggerEnter(Collider col)
    {
        string colTag = col.transform.tag;
        if (colTag == "Player")
            Disturb(true, col.transform.position);
        else if (colTag == "NPC")
            Disturb(false, col.transform.position);
    }
    #endregion

    #region CustomFunctions
    void Disturb(bool isPlayer, Vector3 disruptionPosition)
    {
        if (hasSeed && isPlayer)
            DropSeed();
        Vector3 forceDir = (disruptionPosition.x > transform.position.x) ? Vector3.left : Vector3.right;
        rb.AddForce((forceDir) * disturbForce, ForceMode.Impulse);
    }

    void DropSeed()
    {
        Instantiate(seed, transform.position, Quaternion.identity);
        print("disable blob shadow");
        hasSeed = false; 
    }

    void SetHingeControls()
    {
        HingeJoint hinge = GetComponent<HingeJoint>();
        JointSpring spring = hinge.spring;
        print("setting " + spring);
        spring.spring = baseSpring;
        spring.damper = baseDamper;
        hinge.spring = spring;
    }
    #endregion
}

