using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NodeSystem : MonoBehaviour {

    [SerializeField]
    private PlayerNodeScript[] playerNodes;
    [SerializeField]
    private Vector3[] cameraPositions;
    [SerializeField]
    private float repeatTime;
    [SerializeField]
    bool draw;
    private Vector3 calculatedCenterPosition, weightedCenterPosition, weightedPosition;

    private List<int> currentCameraNodes;

    public Vector3 WeightedPosition
    {get{return weightedPosition;}
    set{weightedPosition = value;}}

    public delegate void Calc();
    public static event Calc OnCalc;

    void Start()
    {
       
        cameraPositions = new Vector3[playerNodes.Length];
        for (int i = 0; i < playerNodes.Length; i++)
        {
            cameraPositions[i] = playerNodes[i].CamPos;
            //playerNodes[i].GetComponent<PlayerNodeScript>().CoorespondingCameraNode = cameraNodes[i];
            //cameraNodes[i] = transform.GetChild(i).GetChild(0).GetComponent<CameraNodeScript>();
           // if (!draw)
           // {
           //     transform.GetChild(i).GetComponent<PlayerNodeScript>().StopDraw();
          //      cameraNodes[i].StopDraw();
          //  }
        }
        
    }
    
    void OnEnable () {
        InvokeRepeating("CalculatePositions", 0f, repeatTime);
	}
	
	void Update () {
        //CalculatePositions();
	}

    void CalculatePositions()
    {
        //print("calculating");
        OnCalc();
        Vector3 temp = Vector3.zero;
        foreach (Vector3 pos in cameraPositions)
            temp += pos;
        temp /= cameraPositions.Length;
        calculatedCenterPosition = temp;
        temp = Vector3.zero;
        float tempWeight = 0;
        foreach (PlayerNodeScript node in playerNodes)
        {
            float weight = node.weight;
          //  print("weight is " + weight);
            tempWeight += weight;
           // temp += node.transform.position + ((calculatedCenterPosition - node.transform.position) * node.weight);
           // Debug.DrawRay(node.transform.position, (calculatedCenterPosition - node.transform.position) * node.Weight, Color.yellow);
            temp = calculatedCenterPosition - node.transform.position;
        }
        //  print("total weight is " + tempWeight);

        weightedPosition = Vector3.zero;

        foreach (PlayerNodeScript node in playerNodes)
        {
            float weight = node.weight;
            //print("weight for node is " + weight);
            float weightedPercentage = (weight - 0) * 100 / tempWeight;
            //print("weighted percentage for " + node + " is " + weightedPercentage);
            weightedPosition += node.transform.position * weightedPercentage;
        }
        weightedPosition /= 100f;


        temp /= playerNodes.Length;
        weightedCenterPosition = temp;
        //print("calced");
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(calculatedCenterPosition, .2f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(weightedPosition, Vector3.one);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(weightedCenterPosition, .4f);
        }
    }
}
