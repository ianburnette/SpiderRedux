using UnityEngine;
using System.Collections;

public class PlayerNodeScript : NodeScript{

    [SerializeField]
    private Vector3 camPos;
    [SerializeField]
    private Transform target;
    [SerializeField]
    float farDistance, nearDistance;
    [SerializeField]
    private float time;


    public float dist, weight;

    public Vector3 CamPos{get{return camPos;}set{camPos = value;}}

    //public CameraNodeScript CoorespondingCameraNode{get{return coorespondingCameraNode;}set{coorespondingCameraNode = value;}}

    void OnEnable()
    {
        NodeSystem.OnCalc += PlayerNodeCalc;
    }


    void OnDisable()
    {
        NodeSystem.OnCalc -= PlayerNodeCalc;
    }

    void PlayerNodeCalc()
    {
        dist = Vector3.Distance(target.position, transform.position);
       // print("dist is " + dist);
        if (dist <= farDistance)
        {
            if (dist < nearDistance)
                weight = 1f;
            else
            {
                float range = farDistance - nearDistance;
                float correctedStartValue = dist - nearDistance;
                float percentage = (correctedStartValue * 100f) / range;
                weight = (100f-percentage) / 100f;
            }
        }
        else
        {
            weight = 0f;
        }
        //coorespondingCameraNode.Weight = weight;//= Mathf.Lerp(coorespondingCameraNode.Weight, weight, time * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (draw)
        {
            if (weight > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, farDistance);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, farDistance);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + CamPos,2f);
        }
        
        //Gizmos.DrawRay(transform.position, (target.position - transform.position) * nearDistance);
        Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position + (Vector3.up * .2f), (target.position - transform.position) * farDistance);
    }
}
