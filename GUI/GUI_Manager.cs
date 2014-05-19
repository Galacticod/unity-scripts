using UnityEngine;
using System.Collections;

public class GUI_Manager : MonoBehaviour {

	public Tutorial tut;
	
	// basic things
	public bool activeGUI = false; // changed by LEAP manager
	public bool activateLearning = false;
	
	/* ---------------- */
	// the GUI manager knows which fact we are trying to view
	public GameObject currentTrigger; 
	public KnowledgeDrop currentKDrop;
//	Formatter setFormatting;

	public GUITexture backgroundGradient;
	public GUITexture theIcon; //used to grab icon from knowledge drop script
	public GUITexture bodyImageCol1; //used to grab image 1 from knowledge drop script
	public GUITexture bodyImageCol2; //used to grab image 2 from knowledge drop script
	
	/* ---------------- */
	
	/* TEXT OBJECTS */
	public GUIText topicText;
	public GUIText titleText; 
	public GUIText bodyTextCol1Row1;
	public GUIText bodyTextCol1Row2; 
	public GUIText bodyTextCol2Row1;
	public GUIText bodyTextCol2Row2;
	public GUIText promptText;
	public GUIText swimPromptText;

	//used to add spacing for positioning
	public int padding10 = 10;
	public int padding25 = 25;
	public int padding50 = 50;

	int bodyTextWidth = 200;

	float dimAlpha; //alpha that controls the black texture overlay
	float guiElementsAlpha;
	float bodyTextAlpha;
	float promptAlpha;
	float swimTextAlpha;

	bool swimFadeControl = false;

	//for blur lerps
	float blurIteration;
	float blurSpreading;


	//for position lerps
	float promptPosYLerp;
	float promptPosYLerpStart;
	float promptPosYLerpEnd;

	//x,y center values for prompt prompt text
	float centerPromptTextPositionX;
	float centerPromptTextPositionY;

	//x,y values of half screen width and height
	float halfScreenCenterY;
	float halfScreenCenterX;
	//split screen vertically into ninths, then get y value of 3rd row
	float screenYSplit9set3;

	//allow access to Blur Effect Script
	public BlurEffect blurControl;

	bool guiElementsVisible = false;

	//info image height and width
	float imageHeight;
	float imageWidth;
	float imageHeight2;
	float imageWidth2;


	public bool dimInfoControlScreen = false;
	
	void Start () 
	{
		disableGUIElements ();
		backgroundGradient.enabled = false;
		promptPosYLerpEnd = -((Screen.height/9)*8);
		promptPosYLerp = promptPosYLerpEnd + padding50;
		promptPosYLerpStart = promptPosYLerp;
		screenYSplit9set3 = -((Screen.height / 9) *3);
//		setFormatting = GetComponent<Formatter> ();

		blurControl = Camera.main.GetComponent<BlurEffect> ();
//		Debug.Log ("BLUR CONTROL ENABLED?: " + blurControl.enabled);
		blurControl.enabled = false;
	}
	
	
	void Update()
	{	
//		Debug.Log (activateLearning);
//		Debug.Log ("TOGGLE: " + guiElementsVisible);
		if(activeGUI)
		{
			//dimScreen();
			tut.overlay.enabled = true;
//			tut.overlay.pixelInset = new Rect (0,0,Screen.width,Screen.height);
			tut.okayToDimForFact = true;
			undimScreen();
			unblurScreen();
//			Debug.Log(tut.okayToDim);
			setKnowledge();
		//toggle
			if(!activateLearning) 
			{
				showPrompt();
				if(!guiElementsVisible) fadeGUIElementsOut();

			}


			if(activateLearning)
			{
				tut.okayToDimForFact = false;
				dimScreen();
				blurScreen();
				hidePrompt();
//				Debug.Log(promptAlpha);
				if(promptAlpha < .1f)
				{
					promptAlpha = 0;
//					Debug.Log("Knowledge dropped");
					enableGUIElements();
					fadeGUIElementsIn();
				}
			}

			if(Input.GetKeyDown(KeyCode.P))
			{
				activateLearning = !activateLearning;
				toggleGUIElementsVisible();
			}		
		}

		else if(!activeGUI && !activateLearning)
		{
			unblurScreen();
			tut.okayToDimForFact = false;
			fadeGUIElementsOut();
			if(guiElementsVisible) toggleGUIElementsVisible();
			hidePrompt();
			undimScreen();
		}

		if(dimInfoControlScreen) 
		{
			backgroundGradient.pixelInset = new Rect(0,0,Screen.width, Screen.height);
			dimScreen(); //****************UNCOMMENT THIS TO DIM SCREEN WHILE CONTROLS IMAGE IS ACTIVE*********************
//			Debug.Log("DIM INFO CONTROL SCREEN IN GUI MANAGER AFFECTED");
//			Debug.Log(dimAlpha);
//			Debug.Log(backgroundGradient.enabled);
		}
		else 
		{ 
			undimScreen(); 
		}
	}

	void toggleGUIElementsVisible()
	{
		guiElementsVisible = !guiElementsVisible;
	}

	void fadeGUIElementsIn()
	{
		Debug.Log ("FADE GUI ELEMENTS IN CALLED");


		guiElementsAlpha = Mathf.Lerp(guiElementsAlpha, 1f, 10f * Time.deltaTime);
		topicText.color = new Color(1,1,1, guiElementsAlpha);
		titleText.color = new Color(1,1,1, guiElementsAlpha);
		theIcon.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol1Row1.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol1Row2.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol2Row1.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol2Row2.color = new Color(1,1,1, guiElementsAlpha);
		if(bodyImageCol1 != null) bodyImageCol1.color = new Color(1,1,1, guiElementsAlpha);
		if(bodyImageCol2 != null) bodyImageCol2.color = new Color(1,1,1, guiElementsAlpha);
		pulseFadeSwimText ();
	}

	void pulseFadeSwimText()
	{
		if(swimFadeControl)
		{
			fadeSwimTextOut();
		}
		
		else
		{
			fadeSwimTextIn();
		}
	}

	void fadeSwimTextIn()
	{

			swimTextAlpha = Mathf.Lerp(swimTextAlpha, 1f, 6f * Time.deltaTime);
			swimPromptText.color = new Color(1,1,1, swimTextAlpha);
			if(swimTextAlpha > .98f)
			{
				swimTextAlpha = 1f;
				swimFadeControl = !swimFadeControl;
			}
	}

	void fadeSwimTextOut()
	{
		swimTextAlpha = Mathf.Lerp(swimTextAlpha, .2f, 10f * Time.deltaTime);
		swimPromptText.color = new Color(1,1,1, swimTextAlpha);
		if(swimTextAlpha < .3f)
		{
			swimTextAlpha = .2f;
			swimFadeControl = !swimFadeControl;
		}
	}

	void fadeGUIElementsOut()
	{
//		Debug.Log ("FADE GUI ELEMENTS OUT CALLED");

		if(Time.timeScale != .3f)
		{
			guiElementsAlpha = Mathf.Lerp(guiElementsAlpha, 0f, 7f * Time.deltaTime);
		}

		else 
		{
			guiElementsAlpha = Mathf.Lerp(guiElementsAlpha, 0f, 15f * Time.deltaTime);
		}

		topicText.color = new Color(1,1,1, guiElementsAlpha);
		titleText.color = new Color(1,1,1, guiElementsAlpha);
		if(theIcon != null) theIcon.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol1Row1.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol1Row2.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol2Row1.color = new Color(1,1,1, guiElementsAlpha);
		bodyTextCol2Row2.color = new Color(1,1,1, guiElementsAlpha);
		swimPromptText.color = new Color(1,1,1, guiElementsAlpha);
		if(bodyImageCol1 != null) bodyImageCol1.color = new Color(1,1,1, guiElementsAlpha);
		if(bodyImageCol2 != null) bodyImageCol2.color = new Color(1,1,1, guiElementsAlpha);

		if(guiElementsAlpha < .01) disableGUIElements();
	}

	public void showPrompt()
	{
//		Debug.Log ("SHOW PROMPT CALLED");
		promptAlpha = Mathf.Lerp(promptAlpha, 1f, 3f * Time.deltaTime);
		promptText.color = new Color(1,1,1, promptAlpha);
		promptPosYLerp = Mathf.Lerp(promptPosYLerp, promptPosYLerpEnd, 11f*Time.deltaTime);
		promptText.pixelOffset = new Vector2(centerPromptTextPositionX,promptPosYLerp);
	}

	void hidePrompt()
	{
//		Debug.Log ("HIDE PROMPT CALLED");

		promptAlpha = Mathf.Lerp(promptAlpha, 0f, 16f * Time.deltaTime);
		promptText.color = new Color(1,1,1, promptAlpha);
		promptPosYLerp = Mathf.Lerp(promptText.pixelOffset.y,promptPosYLerpStart, 10f* Time.deltaTime);
		promptText.pixelOffset = new Vector2(centerPromptTextPositionX,promptPosYLerp);

	}

	public void dimScreen()
	{
		backgroundGradient.enabled = true;
		//lerp for background texture alpha
		dimAlpha = Mathf.Lerp(dimAlpha, .8f, 4f * Time.deltaTime);
		//apply color fading lerps to background
		backgroundGradient.color = new Color(1,1,1, dimAlpha);
	}

	public void undimScreen()
	{
		//lerp for background texture alpha
		dimAlpha = Mathf.Lerp(dimAlpha, 0f, 1f * Time.deltaTime);
		//apply color fading lerps to background
		backgroundGradient.color = new Color(1,1,1, dimAlpha);
		if(dimAlpha <.01) backgroundGradient.enabled = false;
	}

	void blurScreen()
	{
		blurControl.enabled = true;
		blurIteration = Mathf.Lerp(blurIteration, 3f, 12f * Time.deltaTime);
		blurSpreading = Mathf.Lerp(blurSpreading, .5f, 1.1f* Time.deltaTime);
		
		blurControl.iterations = Mathf.RoundToInt(blurIteration);
		
		blurControl.blurSpread = blurSpreading;
	}

	void unblurScreen()
	{
		blurIteration = Mathf.Lerp(blurIteration, 0f, 5f * Time.deltaTime);
		blurSpreading = Mathf.Lerp(blurSpreading, 0f, 10f* Time.deltaTime);
		
		if(blurIteration < 1) blurControl.enabled = false;

		blurControl.iterations = Mathf.RoundToInt(blurIteration);
		blurControl.blurSpread = blurSpreading;
	}
	
	public void setKnowledge()
	{
		setGUIElementsValues ();
		setScreenCenterXY ();
		setPromptCenterXY ();
		positionGUIConstants ();
		positionGUIVariations ();
	
	}

	//layout positions vary
	void positionGUIVariations()
	{
		//POSITION Body text and images
		
		if(currentTrigger.tag == "OF1") //overfishing - DONE 
		{
			imageHeight = 109;
			imageWidth = 298;
			layout1(); //(text-image-text-text)
		}
		
		if(currentTrigger.tag == "OF2") //bottom trawling - DONE
		{
			imageHeight = 166;
			imageWidth = 300;
			layout2(); //(text-text-image)
		}
		
		if(currentTrigger.tag == "OF3") //bycatch - DONE
		{
			imageHeight = 81;
			imageWidth = 300;
			layout2 (); //(text-text-image)
		}
		
		if(currentTrigger.tag == "ME1") //end of ocean - DONE
		{
			imageHeight = 66;
			imageWidth = 300;
			layout1(); //(text-image-text-text)
		}
		
		if(currentTrigger.tag == "ME2") //reef at risk - DONE
		{
			imageHeight = 268;
			imageWidth = 300;
			layout2 (); //(text-text-image)
		}
		
		if(currentTrigger.tag == "ME3") //bluefin - DONE
		{
			imageHeight = 94;
			imageWidth = 299;
			layout3(); //(text-image-text)
		}
		
		if(currentTrigger.tag == "OIL1") //greasy situation - DONE
		{
			imageHeight = 200;
			imageWidth = 300;
			layout3(); //(text-image-text)
		}
		
		if(currentTrigger.tag == "ACID1") //underwater Graveyard - DONE
		{
			imageHeight = 98;
			imageWidth = 300;
			layout1(); //(text-image-text-text)
		}
		
		if(currentTrigger.tag == "ACID2") //world without coral - DONE
		{
			imageHeight = 104;
			imageWidth = 300;
			layout3(); //(text-image-text)
		}
		
		if(currentTrigger.tag == "MD1") //trash pile, pie chart - DONE
		{
			imageHeight = 150;
			imageWidth = 300;
			imageHeight2 = 112;
			imageWidth2 = 293;
			layout4(); //(text-image-text-text-image-text)
		}
		
		if(currentTrigger.tag == "MD2") //plastic ocean - DONE
		{
			imageHeight = 116;
			imageWidth = 300;
			layout1(); //(text-image-text-text)
		}
		
		if(currentTrigger.tag == "MD3") //island of trash - DONE
		{
			imageHeight = 127;
			imageWidth = 300;
			layout1(); //(text-image-text-text)
		}
		
		
	}

	//col1row1 followed by imagecol1 - col1row2 - col2row1 (text-image-text-text) 
	void layout1()
	{
		bodyImageCol1.pixelInset = new Rect(bodyTextCol1Row1.pixelOffset.x,bodyTextCol1Row1.pixelOffset.y-bodyTextCol1Row1.GetScreenRect().height-imageHeight-(padding10),imageWidth,imageHeight);
		bodyTextCol1Row2.pixelOffset = new Vector2 (bodyTextCol1Row1.pixelOffset.x, bodyImageCol1.pixelInset.y-(padding10 + padding10));
		bodyTextCol2Row1.pixelOffset = new Vector2 (halfScreenCenterX + (padding50), bodyTextCol1Row1.pixelOffset.y);
	}

	//col1row1 followed by col2row1 - imagecol2 (text-text-image)
	void layout2()
	{
		bodyTextCol2Row1.pixelOffset = new Vector2 (halfScreenCenterX + (padding50), bodyTextCol1Row1.pixelOffset.y);
		bodyImageCol2.pixelInset = new Rect(bodyTextCol2Row1.pixelOffset.x,bodyTextCol2Row1.pixelOffset.y-bodyTextCol2Row1.GetScreenRect().height-imageHeight-(padding10),imageWidth,imageHeight);
	}

	void layout3()//col1row1 followed by imagecol1 - col2row1 (text-image-text)
	{
		bodyImageCol1.pixelInset = new Rect(bodyTextCol1Row1.pixelOffset.x,bodyTextCol1Row1.pixelOffset.y-bodyTextCol1Row1.GetScreenRect().height-imageHeight-(padding10),imageWidth,imageHeight);
		bodyTextCol1Row2.pixelOffset = new Vector2 (bodyTextCol1Row1.pixelOffset.x, bodyImageCol1.pixelInset.y-(padding10 + padding10));
		bodyTextCol2Row1.pixelOffset = new Vector2 (halfScreenCenterX + (padding50), bodyTextCol1Row1.pixelOffset.y);
	}

	void layout4()//col1row1 followed by imagecol1 - col1row1 - col2row1 - imagecol2 - col2row2 (text-image-text-text-image-text)
	{
		bodyImageCol1.pixelInset = new Rect(bodyTextCol1Row1.pixelOffset.x,bodyTextCol1Row1.pixelOffset.y-bodyTextCol1Row1.GetScreenRect().height-imageHeight-(padding10),imageWidth,imageHeight);
		bodyTextCol1Row2.pixelOffset = new Vector2 (bodyTextCol1Row1.pixelOffset.x, bodyImageCol1.pixelInset.y-(padding10 + padding10));
		bodyTextCol2Row1.pixelOffset = new Vector2 (halfScreenCenterX + (padding50), bodyTextCol1Row1.pixelOffset.y);
		bodyImageCol2.pixelInset = new Rect(bodyTextCol2Row1.pixelOffset.x,bodyTextCol2Row1.pixelOffset.y-bodyTextCol2Row1.GetScreenRect().height-imageHeight2-(padding10),imageWidth2,imageHeight2);
		bodyTextCol2Row2.pixelOffset = new Vector2 (bodyTextCol2Row1.pixelOffset.x, bodyImageCol2.pixelInset.y-(padding10 + padding10));

	}

	//layout never changes
	void positionGUIConstants()
	{
		//pixel inset = x,y,width,height
		//offset = x,y

		//POSITION BODY TEXT COLUMN 1 & 2
		bodyTextCol1Row1.pixelOffset = new Vector2 (halfScreenCenterX - (bodyTextWidth + (padding50*3-padding25)), screenYSplit9set3 + padding50);

		//POSITION ICON
		theIcon.pixelInset = new Rect(bodyTextCol1Row1.pixelOffset.x,bodyTextCol1Row1.pixelOffset.y + padding50,100,100);
//		Debug.Log("THE ICON Y: " + theIcon.pixelInset.y);
		float halfIconYPos = theIcon.pixelInset.y + (theIcon.pixelInset.height / 2);
		
		//POSITION TOPIC TEXT
		topicText.pixelOffset = new Vector2 (titleText.pixelOffset.x, halfIconYPos + (padding10/2));
//		Debug.Log("THE TOPIC Y: " + topicText.pixelOffset.y);

		//POSITION TITLE TEXT
		titleText.pixelOffset = new Vector2 (theIcon.pixelInset.x + theIcon.pixelInset.width + padding25, halfIconYPos + padding10);

		//POSITION SWIM PROMPT TEXT
		swimPromptText.pixelOffset = new Vector2 (halfScreenCenterX - (swimPromptText.GetScreenRect().width/2), -((Screen.height/9)*8 + padding50));


		//POSITION BACKGROUND DIM
		backgroundGradient.pixelInset = new Rect(0,0,Screen.width, Screen.height);
	}

	void setScreenCenterXY()
	{
		//CENTER SCREEN COORDINATES
		halfScreenCenterX = Screen.width / 2;
		halfScreenCenterY = -(Screen.height / 2);
	}

	void setPromptCenterXY()
	{
		//CENTER PROMPT TEXT
		float promptTextWidth = promptText.GetScreenRect().width;
		float halfPromptTextWidth = promptTextWidth/2;
		centerPromptTextPositionX = halfScreenCenterX - halfPromptTextWidth;
		centerPromptTextPositionY = (halfScreenCenterY - promptText.GetScreenRect().height * 2)-padding50;
	}

	void setGUIElementsValues()
	{
		topicText.text = currentKDrop.topic; //get title text from knowledgedrop script
		titleText.text = currentKDrop.title; //get topic text from knowledge drop script
		bodyTextCol1Row1.text = currentKDrop.bodyCol1Row1; //get body text column 1 row 1 from knowledge drop script
		bodyTextCol1Row2.text = currentKDrop.bodyCol1Row2; //get body text column 1 row 2 from knowledge drop script
		bodyTextCol2Row1.text = currentKDrop.bodyCol2Row1; //get body text column 2 row 1 from knowledge drop script
		bodyTextCol2Row2.text = currentKDrop.bodyCol2Row2; //get body text column 2 from knowledge drop script
		promptText.text = "Close Fist to Learn More"; 
		swimPromptText.text = "Swim Away To Keep Exploring"; 
		theIcon = currentKDrop.icon;
		bodyImageCol1 = currentKDrop.bodyImageCol1;
		bodyImageCol2 = currentKDrop.bodyImageCol2;
	}
	
	/*
	 * GUI stuff
	 */
	public void TurnOnGUI(GameObject _trigger)
	{
		//signal that it's ok to show text
		activeGUI = true; 
		// set current trigger & kDrop
		currentTrigger = _trigger;
		currentKDrop = currentTrigger.GetComponent<KnowledgeDrop> ();
		//enable prompt text
		enablePrompt ();
		Time.timeScale = 0.3f; //slow down the world speed
	}
	
	public void TurnOffGUI()
	{
		activeGUI = false;//signal to turn off text
		activateLearning = false;
		Time.timeScale = 1f; //resume world speed to real time
	}

	void enablePrompt()
	{
		promptText.enabled = true;
	}

	void disablePrompt()
	{
		promptText.enabled = false;
	}

	public void enableGUIElements()
	{
		//constants-  text and images that always appear on screen
		topicText.enabled = true;
		titleText.enabled = true;
		bodyTextCol1Row1.enabled = true;
		theIcon.enabled = true;
		swimPromptText.enabled = true;

		//variations - may not appear based on gui layout
		if(bodyTextCol2Row1.text != "") bodyTextCol2Row1.enabled = true;
		if(bodyTextCol1Row2.text != "") bodyTextCol1Row2.enabled = true;
		if(bodyTextCol2Row2.text != "") bodyTextCol2Row2.enabled = true;
		if(bodyImageCol1 != null) bodyImageCol1.enabled = true;
		if(bodyImageCol2 != null)  bodyImageCol2.enabled = true;

	}

	//disable gui elements that appear when fist is closed
	void disableGUIElements()
	{
		topicText.enabled = false;
		titleText.enabled = false;
		bodyTextCol1Row1.enabled = false;
		bodyTextCol1Row2.enabled = false;
		bodyTextCol2Row1.enabled = false;
		bodyTextCol2Row2.enabled = false;
		swimPromptText.enabled = false;

		if(theIcon != null) theIcon.enabled = false;
		if(bodyImageCol1 != null) bodyImageCol1.enabled = false;
		if(bodyImageCol2 != null) bodyImageCol2.enabled = false;
		if(promptAlpha <.1) disablePrompt();
	}
}
