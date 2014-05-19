using UnityEngine;
using System.Collections;

public class Formatter : MonoBehaviour {

	/*
	 * based on the JS example on:
	 * http://forum.unity3d.com/threads/31351-GUIText-width-and-height
	 */

	private string[] words;
	private string[] wordsList;
	private string newString;
	private Rect textWidth;
	public GUIText holder;

	void Start() {
		holder.enabled = false;
	}

	public string Format(string text, int lineLength) {

		newString = "";
		words = text.Split (" "[0]);

		for (int i = 0; i < words.Length; i++) {
			string word = words[i].Trim();

			if(i == 0) {
				newString = words[i] + " ";
				holder.text = newString;
			}

			if(i > 0) {
				newString += word + " ";
				holder.text = newString;
			}

			textWidth = holder.GetScreenRect();

			if(textWidth.width > lineLength) {
				newString = newString.Substring(0, newString.Length - (word.Length + 1));
				newString += "\n" + word + " ";
				holder.text = newString;
			}
		}

		return newString;
	}
	
}
