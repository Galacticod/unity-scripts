using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Leap;

public class LEAP_Manager : MonoBehaviour {
	
	// LEAP variables
	Controller controller;
	Frame frame = null;
	Hand hand = null;
// 	Finger finger = null;
	int fingers = 0;
	Vector3 palm;
	GestureList gestures;
	
	// vars for LEAP data
	float leftOrRight;
	float upOrDown;
	float handRotation;
	
	
	// set LEAP ranges
	public float leapHorizontalRange = 500f;// based on output from the leap

	public float rotateUpValue = -0.55f;
	public float rotateDownValue = 0.25f;
	
	
	// setting divisions for LEAP control	
	private float left_box;
	private float right_box;
	private float top_box;
	private float bottom_box;
	private Vector2 center;


	// Clownfish
	public Clown_Fish_Swim clownfish; /* this is set in the Inspector editor */
	
	
	
	// other scripts
	private LEAP_HUD hud;
	private Tutorial tut;
	public GUI_Manager gui;

	// LEAP controls
	public GUITexture controls;
	private bool showControls = false;
	private float controlsTimer = 0f;
	public float controlsTimeLimit = 2f;


	public GUIText feedback;
	
	

	void Start () {
		// HUD
		hud = GetComponent<LEAP_HUD> ();
		tut = GetComponent<Tutorial> ();

		//Position Tutorial Controls Image
		//float controlsXPos = (UnityEngine.Screen.width/2)-640;
		controls.pixelInset = new Rect(UnityEngine.Screen.width/2-640,UnityEngine.Screen.height/2-360,1280,720);
		
		// set up LEAP
		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 10f);
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 3f);
		controller.Config.Save();
		
		// set control area
		SetupLEAP ();
		tut.SetArea (top_box, right_box, bottom_box, left_box); // pass dimensions to tutorial

		// setup controls image helper
		if (controls.enabled) controls.enabled = false;
	}
	
	void SetupLEAP() {

		float unit = 0.5f / 10 * leapHorizontalRange; // 25

		// horizontal zones (left / right)
		left_box = unit * 9;	// 225
		right_box = unit * 11;	// 275

		// vertical zones
//		top_box = unit * 6;		// 150
		top_box = unit * 5;		// 125
		bottom_box = unit * -2; // -50

		center = new Vector2 (unit * 10, unit * 2);
	}

	/*
	 * 
	 */

	void Update () {
		// LEAP stuff
		frame = controller.Frame ();


		// hand check
		// (hand check? what is this high school? thanks mom)
		if(frame.Hands.Count > 0)
		{
			// update variables
			hand = frame.Hands[0];
			fingers = hand.Fingers.Count;
			palm = hand.PalmPosition.ToUnity(); // ToUnity() is called from Plugins/LeapUnityExtensions.cs
			gestures = frame.Gestures ();
			
			leftOrRight = palm.y;
			upOrDown = palm.x * -2;
			handRotation = hand.Direction.Yaw;


			if(showControls) IdleMode();
			else {
				if(tut.guiActive) Tutorial();
				else {
					if(gui.activeGUI) GUITalk();
					Swim();
				}
			}
			// make sure controls.png is hidden
			controlsTimer = 0f;
		} else {
			clownfish.LevelOff();
			hud.direction = 0;
			controlsTimer += Time.deltaTime;
			if (controlsTimer > controlsTimeLimit) {
				showControls = true;
				tut.Reset();
			}
		}
		// end hand check


		// for easy debugging
		WASDInput ();
	}
	

	float dimAlpha = 0f;
	void OnGUI() 
	{
		if (showControls) {
			if(!controls.enabled) controls.enabled = true;
			dimAlpha = Mathf.Lerp(dimAlpha, 1f, 0.8f * Time.deltaTime);
			controls.color = new Color(255,255,255, dimAlpha);
			gui.dimInfoControlScreen = true;
		} else {
			// fade out png
			dimAlpha = Mathf.Lerp(dimAlpha, 0f, 2f * Time.deltaTime);
			controls.color = new Color(1,1,1, dimAlpha);
			gui.dimInfoControlScreen = false;
			// disable texture when finished lerp-ing
			if (dimAlpha <= 0.0125f) controls.enabled = false;
		}

	}
	


	// wait 3 seconds before starting anything
	float tutorialTimer = 0f;
	float tutorialTimeLimit = 1f;

	float swimTimer = 0f;
	float swimTimeLimit = 0.75f;

	void IdleMode()
	{
		// check for hand input before closing controls
		// open palm --> start swimming
		if (fingers >= 3) {
			swimTimer += Time.deltaTime;
			if (swimTimer > swimTimeLimit) {
				swimTimer = 0f;
				showControls = false;
			}
		} else swimTimer = 0f;

		if (fingers <= 1) {
			tutorialTimer += Time.deltaTime;
			if (tutorialTimer > tutorialTimeLimit) {
				showControls = false;
				tut.guiActive = true;
				tutorialTimer = 0f;
			}
		} else tutorialTimer = 0f;
		
	}




	/*
	 * Tutorial
	 */
	void Tutorial() {

		Vector2 pos = new Vector2 (leftOrRight, upOrDown);
		feedback.text = pos.ToString ();
		tut.Learning (pos, fingers);

	}








	/*
	 * GUI stuff
	 */

	void GUITalk() {
		// make fist to open gui fact
		if (fingers <= 1) gui.activateLearning = true;
		if (fingers >= 3) gui.activateLearning = false;
	}
	










/*
 * Special Features [..:: sparkle, sparkle ::..]
 */
	float oldHandPosition = 0;
	float timer = 0;
	void Swim()
	{	

		
		/* 	fist	=	brake	*/
		if (fingers <= 1) {
			ApplyDrag ();
			if(!gui.activeGUI) clownfish.IncreaseTurnSpeed();
			else clownfish.DecreaseTurnSpeed();
		}
		else {
			clownfish.ResetTurnSpeed();
			// swipe = swim forward
			if (gestures.Count > 0) MoveForward();


			// store the current l/r pos
			if(oldHandPosition == 0) oldHandPosition = leftOrRight;

			float range = 10f;
			if (leftOrRight > oldHandPosition + range || leftOrRight < oldHandPosition - range) {
				MoveForward();
			}

			timer += Time.deltaTime;
			if (timer > 2f) {
				oldHandPosition = leftOrRight;
				timer = 0f;
			}
		}

		/* LEFT or RIGHT */
		if( leftOrRight < left_box ) TurnLeft();		// left box
		if( leftOrRight > right_box ) TurnRight();		// right box
		
		
		/* UP or DOWN */
		if( upOrDown > top_box ) TurnUp();
		if( upOrDown < bottom_box ) TurnDown();
		
		if(handRotation < rotateUpValue) TurnUp();
		if(handRotation > rotateDownValue) TurnDown();

		// level off
		if(upOrDown > bottom_box && upOrDown < top_box) clownfish.LevelOff();

		// center box
		if (leftOrRight > left_box && leftOrRight < right_box && upOrDown > bottom_box && upOrDown < top_box) {
			hud.direction = 1;		
		}

	}
	
	
	
	// Movement functions
	// - speed up/down
	void MoveForward() {
		clownfish.MoveForward();
		hud.direction = 1;
	}
	void ApplyDrag() {
		clownfish.ApplyDrag();
		hud.direction = 1;
	}
	
	// Turn functions
	// - move the clownfish & update HUD
	void TurnLeft(){
		clownfish.TurnLeft();
		hud.direction = 5;
	}
	void TurnRight(){
		clownfish.TurnRight();
		hud.direction = 3;
	}
	void TurnUp(){
		clownfish.TurnUp();
		hud.direction = 2;
	}
	void TurnDown(){
		clownfish.TurnDown();
		hud.direction = 4;
	}
	
	
	
	

	
	
	/*
 * WASD input (for easy debugging)
*/
	void WASDInput()
	{
		//add to acceleration if forward movement key is being held
		if (Input.GetKey(KeyCode.W)) { MoveForward(); }
		if (Input.GetKey(KeyCode.S)) { ApplyDrag(); }
		// left/right
		if (Input.GetKey(KeyCode.D)) { TurnRight(); }
		if (Input.GetKey(KeyCode.A)) { TurnLeft(); }
		
		// up/down
		if (Input.GetKey ("up")) { TurnUp(); }
		if (Input.GetKey ("down")) { TurnDown(); }

	}
	
}