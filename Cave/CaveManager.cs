using UnityEngine;
using System.Collections;

public class CaveManager : MonoBehaviour {

	public GUITexture backgroundTexture;
	float dimAlpha;
	
	bool okayToDim = false;

	// Use this for initialization
	void Start () {
		backgroundTexture.enabled = false;
		backgroundTexture.pixelInset = new Rect (0,0,Screen.width,Screen.height);

	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("dimalpha?: " + dimAlpha);
		
		if (okayToDim) 
		{
			dimScreen ();
		}
		else 
		{
			undimScreen ();
		}
	}

	void dimScreen()
	{
		Debug.Log ("IN DIM SCREEN");
		backgroundTexture.enabled = true;
		//lerp for background texture alpha
		dimAlpha = Mathf.Lerp(dimAlpha, .7f, 1f * Time.deltaTime);
		//apply color fading lerps to background
		backgroundTexture.color = new Color(1,1,1, dimAlpha);
	}
	
	void undimScreen()
	{
		//lerp for background texture alpha
		dimAlpha = Mathf.Lerp(dimAlpha, 0f, 1f * Time.deltaTime);
		//apply color fading lerps to background
		backgroundTexture.color = new Color(1,1,1, dimAlpha);
		if(dimAlpha <.01) backgroundTexture.enabled = false;
	}

	public void toggleOkayToDim()
	{
		okayToDim = !okayToDim;
	}
}
