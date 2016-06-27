using UnityEngine;
using System.Collections;

public class NodeScript : MonoBehaviour {

 
    [SerializeField]
    int index;
    [SerializeField]
    public bool draw;

    public void StopDraw()
    {
        draw = false;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
