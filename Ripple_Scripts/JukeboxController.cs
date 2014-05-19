using UnityEngine;
using System.Collections.Generic;

/*
    Usage:
    JukeboxController jukebox = new JukeboxController();
    jukebox.AddClip("mysong", myclip);
    jukebox.PlayClip("mysong");
    jukebox.StopClip();
*/
public class JukeboxController : MonoBehaviour {
	
	List<AudioClip> jukebox;
	AudioSource current_clip;
	bool  firstUpdate = false;
	GameObject juke;
	public JukeboxController()
	{
		jukebox = new List<AudioClip>();
		current_clip = null;


	} // constructor
	
	public void AddClip(string name, AudioClip clip)
	{
		if (jukebox == null)
		{
			jukebox = new List<AudioClip>();
		} // if



		GameObject obj;
		obj = new GameObject();
		obj.AddComponent("AudioSource");
		obj.audio.clip = clip;
		obj.audio.ignoreListenerVolume = true;

		clip.name = name;
		jukebox.Add (clip);
		//DontDestroyOnLoad(obj);
		//jukebox.Add(name, obj);
	} // AddClip()

	void Update()
	{
		//Debug.Log ("hello");
		if (!firstUpdate) {
			
			firstUpdate = true;
			juke = GameObject.FindGameObjectWithTag ("MainCamera");
			Debug.Log (juke);
			current_clip = juke.GetComponent<AudioSource> ();

		}
		if (Input.GetKeyDown (KeyCode.H)) {
			PlayClip ("theme1");
			}

		if (Input.GetKeyDown (KeyCode.J)) {
			PlayClip ("pt 1");
		}

		if (Input.GetKeyDown (KeyCode.K)) {
			PlayClip ("_okay_");
		}

		if (Input.GetKeyDown (KeyCode.L)) {
			PlayClip ("cold snow");
		}
	}

	
	/*
        Play a named audio clip.
        Does not restart the clip if it is played twice in a row.
        Will stop a previously playing clip to play this new clip.
    */
	public void PlayClip(string name)
	{

		var clip = Resources.Load<AudioClip> (name);

		if (clip == current_clip.clip)
		{
			Debug.Log (clip);
			return;


		} // if

		if (current_clip != null)
		{

			StopClip();
		} // if

		current_clip.clip = clip;
		current_clip.Play ();

	} // PlayClip()
	
	public void StopClip()
	{
		current_clip.Stop();
	} // StopClip()
	
}