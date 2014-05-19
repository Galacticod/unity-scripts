using UnityEngine;
using System.Collections;

public class StartHalo : MonoBehaviour {

	Component newHalo;

	// Use this for initialization
	void Start () {
		newHalo = GetComponent("Halo"); 
		newHalo.GetType().GetProperty("enabled").SetValue(newHalo, true, null);
		newHalo.GetType().GetProperty("enabled").SetValue(newHalo, false, null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
