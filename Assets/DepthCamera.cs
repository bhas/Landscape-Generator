using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCamera : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
}