using UnityEngine;
using System.Collections;

public class Screenshot_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("space")){
			Application.CaptureScreenshot("Screenshot_11.png", 4);
			Debug.Log ("Screenshot captured!");		
	}
	
	}
}
