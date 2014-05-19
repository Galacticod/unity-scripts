using UnityEngine;
using System.Collections;

public class LEAP_HUD : MonoBehaviour
{
	public Texture2D[] quadrant;
	public int direction = 0;
	public int padding = 50;

	private Vector2 startCorner;

	// not reall sure what these new xt vars are for but there they are
	private Color auxColor;
	private Color cColor;

	void OnGUI() 
	{
		auxColor = cColor = GUI.color;

		// bottom left corner
		startCorner = new Vector2 (padding, Screen.height - quadrant[direction].height - padding);

		// update image
		ShowDirection (quadrant[direction]);

		GUI.color = cColor;
	}

	void ShowDirection(Texture2D dir)
	{
		GUI.color = auxColor;
		float x = startCorner.x;
		float y = startCorner.y;
		Rect image = new Rect (x, y, dir.width, dir.height);
		GUI.DrawTexture (image, dir);
	}
}
