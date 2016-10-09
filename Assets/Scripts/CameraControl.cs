using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public GameObject target;
    private Vector3 diff;

	// Use this for initialization
	void Start () {
        diff = target.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position - diff;
	}
}
