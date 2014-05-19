using UnityEngine;  
using System.Collections;  

public class Underwater_Orig : MonoBehaviour {  
	
	// Attach this script to main camera  
	// And Define variable underwaterLevel up  
	// to your water level or sea level Y-coordinate  
	public float underwaterLevel = 7;
	public float fogDensity = .025f;
	
	// These variable to store  
	// The scene default fog settings  
	private bool defaultFog = true;  
	private Color defaultFogColor;  
	private float defaultFogDensity;  
	private Material defaultSkybox;  
	private float defaultStartDistance;  
	
	void Start () {  
		// store default fog setting  
		// we need to restore fog setting  
		// after we go to surface again  
		defaultFog = RenderSettings.fog;  
		defaultFogColor = RenderSettings.fogColor;  
		defaultFogDensity = RenderSettings.fogDensity;  
		defaultSkybox = RenderSettings.skybox;  
		defaultStartDistance = RenderSettings.fogStartDistance;  
		
		// set the background color  
		//camera.backgroundColor = new Color(0, 0.4f, 0.7f, 1);  
	}  
	
	void Update () {  
		// check if we below the sea or water level  
		if (transform.position.y < underwaterLevel) {  
			// render new fog with blue color  
			// Or you can change the color to  
			// match your water  
			RenderSettings.fog = true;  
			RenderSettings.fogColor = new Color(0, 0.5f, 0.7f, 0.6f);  
			RenderSettings.fogDensity = fogDensity;  
			RenderSettings.fogStartDistance = 0.0f;  
			
			// add this if you want to add blur effect to your underwater  
			// but first add Image Effect (Pro) Package to your project  
			// Add component Image Effect > Blur to Main camera  
			//this.GetComponent<BlurEffect>().enabled = true;  
		} else {  
			// revert back to default setting  
			RenderSettings.fog = defaultFog;  
			RenderSettings.fogColor = defaultFogColor;  
			RenderSettings.fogDensity = defaultFogDensity;  
			RenderSettings.skybox = defaultSkybox;  
			RenderSettings.fogStartDistance = defaultStartDistance;  
			
			// add this if you want to add blur effect to your underwater  
			// but first add Image Effect (Pro) Package to your project  
			// Add component Image Effect > Blur to Main camera  
			//this.GetComponent<BlurEffect>().enabled = false;  
		}  
	}  
}  