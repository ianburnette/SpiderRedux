using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCFollow : MonoBehaviour {

    #region PrivateFields
    [SerializeField]
    Transform followTransform;
    [SerializeField]
    float followSpeed;
    [SerializeField]
    float stopDistance;

    NavMeshAgent nav;
    #endregion

#region PublicProperties

#endregion

#region UnityFunctions
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        nav.destination = followTransform.position;
        nav.speed = followSpeed;
        nav.stoppingDistance = stopDistance;
    }
#endregion

#region CustomFunctions

#endregion
}



