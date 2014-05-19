using UnityEngine;
using System.Collections;

public class GUI_Trigger : MonoBehaviour {

	private float triggerTimer;
	private float timerLimit = 0.0f;
	private bool activated = false;

	public GUI_Manager gui;
	private KnowledgeDrop kDrop;

	bool playAudio = true;

	//*******
//	public Light lightToDim;
//	bool lightDim = false;

	void Start ()
	{

	}

	void update()
	{

	}

	void OnTriggerStay(Collider other)
	{
		if (!activated && other.name == "_P_Clown_Fish_Character_") {
			triggerTimer += Time.deltaTime;
			//Debug.Log("loading...");
			//Debug.Log(triggerTimer);
			//lightToDim.intensity = lightToDim.intensity - .08f;

			// activate after timerLimit seconds
			if (triggerTimer > timerLimit) 
			{
				gui.TurnOnGUI(this.gameObject);

				if (playAudio) 
				{
					if(!audio.isPlaying)
					{
						audio.Play();
						playAudio = false; //signal that it is not okay for audio to continuously repeat
					}
				}
			}
		}
	}

	void OnTriggerExit()
	{
		triggerTimer = 0.0f;
		gui.TurnOffGUI ();
		playAudio = true; //signal that it's okay to play audio
	}
}
