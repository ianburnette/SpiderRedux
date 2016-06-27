using UnityEngine;
using System.Collections;

public class CameraNodeFollow : MonoBehaviour {

    [SerializeField]
    private NodeSystem nodes;
    [SerializeField]
    float lerpSpeed;
    [SerializeField]
    Transform target;

    private Transform parentRig;

    void Awake()
    {
        parentRig = transform.parent;
    }

	void Update () {
        if (!float.IsNaN(nodes.WeightedPosition.x))
            parentRig.transform.position = Vector3.Lerp(parentRig.transform.position, nodes.WeightedPosition, lerpSpeed * Time.deltaTime);
        transform.LookAt(target);
	}
}
