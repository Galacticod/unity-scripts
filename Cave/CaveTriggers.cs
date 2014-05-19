using UnityEngine;
using System.Collections;

public class CaveTriggers : MonoBehaviour {

	public CaveManager cave;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name == "_P_Clown_Fish_Character_") 
		{
			Debug.Log("I'M IN THE CAAAAAAAVEEE");
			cave.toggleOkayToDim();
		}
	}
}
