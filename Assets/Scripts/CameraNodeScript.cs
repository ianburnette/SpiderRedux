using UnityEngine;
using System.Collections;

public class CameraNodeScript : NodeScript {

    [SerializeField]
    [Range(0f,1f)]
    float weight;
 

    public float Weight
    {
        get{return weight;}
        set{weight = value;}
    }

    void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.parent.position - transform.position);
        }
    }
}
