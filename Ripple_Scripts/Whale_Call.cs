using UnityEngine;
using System.Collections;

public class Whale_Call : MonoBehaviour {

	//AudioClip whaleAudio;

	// Use this for initialization
	void Start () {
	
		//whaleAudio = gameObject.GetComponent<AudioSource>();
		//audio.clip = whaleAudio;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider otherObj){
		if(otherObj.name == "_P_Clown_Fish_Character_"){
			Debug.Log ("Fuckin triggggaaaaa");
			audio.Play();
		}
	}
}
