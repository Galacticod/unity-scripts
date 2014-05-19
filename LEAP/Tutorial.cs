using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	private string[] tutorialArray;
	public string currentText;
	public GUIText feedback;
	private Rect textSize;

	public bool guiActive = false;

	private int index = 0;

	private float left_box = 0f;
	private float right_box = 0f;
	private float top_box = 0f;
	private float bottom_box = 0f;

	public Clown_Fish_Swim fish;
	private LEAP_HUD hud;

	public GUITexture overlay;
//	public GUITexture dimAllTexure;

	float dimAlpha; //for bottom screen texture

	float padding50 = 50f;
	public bool okayToDim = false;
	public bool okayToDimForFact = false;



	// Use this for initialization
	void Start () {

//		dimAllTexure.enabled = true;
//		dimAllTexure.pixelInset = new Rect (0,0,Screen.width,Screen.height);
//		Debug.Log ("DIM ALL TEXTURE: " + dimAllTexure.enabled);
		// setup overlay
		overlay.pixelInset = new Rect (0, 0, Screen.width, Screen.height);
		overlay.enabled = false;

		tutorialArray = new string[9];

		tutorialArray [0] = "Remove any reflective jewelry.\nOpen your hand, keep your fingers loose and spread apart.";
		tutorialArray [1] = "Place your open hand in the center and move it towards the LEAP device.\nRemember to keep your fingers spread apart.";
		tutorialArray [2] = "Move your hand away from the LEAP device to steer right";
		tutorialArray [3] = "Move your hand towards the LEAP device to steer left";
		tutorialArray [4] = "Move your hand higher to pivot upward";
		tutorialArray [5] = "Move your hand lower to pivot downward";
		tutorialArray [6] = "Gently swipe your hand back and forth to move forward";
		tutorialArray [7] = "Make a fist to stop moving";
		tutorialArray [8] = "You're all set! Have fun exploring!";

		index = 0;
		// set feedback position
		UpdateText (index); // index starts at 0
		feedback.enabled = false;

		// set up HUD
		hud = GetComponent<LEAP_HUD> ();
	}

	void Update()
	{
		Debug.Log ("OKAY TO DIM IN TUTORIAL SCRIPT: " + okayToDim);
		if(okayToDim || okayToDimForFact)
		{
			overlayDim();
		}
		else
		{
			overlayUnDim();
		}
	}

	void updateTutTextPos()
	{
		feedback.pixelOffset = new Vector2(Screen.width/2 - (feedback.GetScreenRect().width/2),-(Screen.height-feedback.GetScreenRect().height-padding50));

	}

	void overlayDim()
	{
		Debug.Log("OVERLAY DIM FUNCTION CALLED ");
		overlay.pixelInset = new Rect (0,0,Screen.width,Screen.height);
		//lerp for background texture alpha
		dimAlpha = Mathf.Lerp(dimAlpha, .5f, 4f * Time.deltaTime);
		//apply color fading lerps to background
		overlay.color = new Color(0,0,0, dimAlpha);
	}
	
	void overlayUnDim()
	{
		//lerp for background texture alpha
		dimAlpha = Mathf.Lerp(dimAlpha, 0f, .6f * Time.deltaTime);
		//apply color fading lerps to background
		overlay.color = new Color(0,0,0, dimAlpha);
		if(dimAlpha <.01) 
		{
			dimAlpha = 0f;
			overlay.enabled = false;
		}
	}

	public void SetArea(float top, float right, float bottom, float left) {
		top_box = top;
		right_box = right;
		bottom_box = bottom;
		left_box = left;
	}




	/*
	 * get hand position
	 */

	private Vector2 hand;
	private int fingers;
	public void Learning(Vector2 palm, int _fingers)
	{
		// make sure the text is active
		if (!feedback.enabled)
		{
			feedback.enabled = true;

			dimAlpha = 0f;
		}

		// make sure gradient is enabled
		if (!overlay.enabled)
		{
			overlay.enabled = true;
			okayToDim = true;
		}

		// set new data
		hand = palm;
		fingers = _fingers;

		FadeInText ();

		// the tutorial
		if (index == 0) OpenHand();
		if (index == 1) Center();
		if (index == 2) MoveRight();
		if (index == 3) MoveLeft();
		if (index == 4) MoveUp();
		if (index == 5) MoveDown();
		if (index == 6) Swim();
		if (index == 7) Stop();
		if (index == 8) Finish();

	}




	/*
	 * Tutorial Utility functions
	 */

	// next step in tutorial
	public void Next() {
		index++;

		// check range
		if (index > tutorialArray.Length - 1) {
			Reset();
			return;
		}

		playTheAudio ();

		// set the text
		UpdateText (index);

		// reset delay for learning
		delayTimer = 0f;
	}

	public void Reset() {
		index = 0;
		UpdateText (index);
		feedback.enabled = false;
		guiActive = false;
		okayToDim = false;
//		if(dimAlpha < .1f) overlay.enabled = false;
	}

	void UpdateText (int i) {
		// updates the text
		currentText = tutorialArray [index];

		feedback.text = currentText;
		
		// recenters it
		textSize = feedback.GetScreenRect();
		float offset = 100f;
//		feedback.pixelOffset = new Vector2(-textSize.width/2 , -textSize.height/2 - offset);
		updateTutTextPos();
		
		textAlpha = alphaStart; // reset to 0 opacity
	}

	void playTheAudio()
	{
		if(!audio.isPlaying)
		{
			audio.Play();
		}
	}


	float delayTimer = 0f;
	float delay = 1.5f; // this is so the tutorial doesn't immediately jump to the next step
	void UpdateTimer() {
		delayTimer += Time.deltaTime;
		if (delayTimer > delay) {
			Next ();
		}
	}

	




	/*
	 * The actual tutorial part
	 */


	void OpenHand() {
		if (fingers >= 3) Next();
	}

	void Center() {
		if (hand.x > left_box && hand.x < right_box && hand.y > bottom_box && hand.y < top_box) {
			hud.direction = 0;
			Next();
		}
	}

	void MoveRight() {
		if (hand.x > right_box) {
			fish.TurnRight();
			hud.direction = 3;
			UpdateTimer ();
		}
	}
	void MoveLeft() {
		if (hand.x < left_box) {
			fish.TurnLeft();
			hud.direction = 5;
			UpdateTimer ();
		}
	}
	void MoveUp() {
		if (hand.y > top_box) {
			fish.TurnUp();
			hud.direction = 2;
			UpdateTimer ();
		}
	}
	void MoveDown() {
		if (hand.y < bottom_box) {
			fish.TurnDown();
			hud.direction = 4;
			UpdateTimer ();
		}
	}

	float oldHandPosition = 0f;
	float timer = 0f;
	void Swim() {
		// store the current l/r pos
		if(oldHandPosition == 0) oldHandPosition = hand.x;

		float range = 20f;
		if (hand.x > oldHandPosition + range || hand.x < oldHandPosition - range) {
			fish.MoveForward();
			hud.direction = 1;
			UpdateTimer ();
		}

		timer += Time.deltaTime;
		if (timer > 2f) {
			oldHandPosition = hand.x;
			timer = 0f;
		}
	}
	void Stop() {
		if (fingers <= 1) {
			hud.direction = 1;
			UpdateTimer ();
		}
	}

	void Finish() {
//		dimAlpha = Mathf.Lerp(dimAlpha, 0f, .8f * Time.deltaTime);
//		//apply color fading lerps to text objects
//		backgroundGradient.color = new Color(1,1,1, dimAlpha);
//		overlay.enabled = false;
		UpdateTimer ();
		okayToDim = false;
	}





	/*
	 * text effects
	 */
	
	float textAlpha = 0f;
	float alphaStart = 0f;
	float alphaEnd = 1f;
	


	void FadeInText() {
		// fade in lerp
		textAlpha = Mathf.Lerp(textAlpha, 1f, 3f * Time.deltaTime);
		feedback.color = new Color(1,1,1, textAlpha);
	}
}