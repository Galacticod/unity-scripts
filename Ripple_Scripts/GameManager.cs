using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	private List<GameObject> flockList;
	public List<GameObject> FlockList { get { return flockList;	} }
	private Dictionary<string, List<GameObject>> allFlocks;
	private List<GameObject> redFlock;
	private List<GameObject> blueFlock;
	private List<GameObject> goldFlock;
	public Dictionary<string, List<GameObject>> AllFlocks 	{ get { return allFlocks;	} }
	public List<GameObject> RedFlock 	{ get { return redFlock;	} }
	public List<GameObject> BlueFlock 	{ get { return blueFlock;	} }
	public List<GameObject> GoldFlock 	{ get { return goldFlock;	} }
	
	
	//these variable are visible in the Inspector
	public GameObject BluePrefab;
	public GameObject RedPrefab;
	public GameObject GoldPrefab;
	public GameObject ObstaclePrefab;
	public int flockSize = 40;
	private Vector3 centroidModifier;
	
	private Dictionary<string, Vector3> flockDirections;
	public Dictionary<string, Vector3> FlockDirections {
		get { return flockDirections; }
	}
	private Dictionary<string, Vector3>  centroids;
	public Dictionary<string, Vector3> Centroids { get { return centroids; } }
	
	void Start () {
		flockList = new List<GameObject>();
		allFlocks = new Dictionary<string, List<GameObject>>(); //the complete set of fish
		redFlock = new List<GameObject>();
		blueFlock = new List<GameObject>();
		goldFlock = new List<GameObject>();
		allFlocks["red"] = redFlock;
		allFlocks["blue"] = blueFlock;
		allFlocks["gold"] = goldFlock;

		centroidModifier = new Vector3(0,40,0);
		centroids = new Dictionary<string, Vector3>() {{"red", Vector3.forward},{"blue", Vector3.forward},{"gold", Vector3.forward}};
		flockDirections = new Dictionary<string, Vector3>() {{"red", Vector3.forward},{"blue", Vector3.forward},{"gold", Vector3.forward}};
		
		Vector3 pos;
		float ypos;
		
		//make schools of fish
		//bluies
		for (int i=0; i<flockSize; i++) 
		{
			//select fish type randomly
			float col = Random.value;
			GameObject prefab;
			if (col < 0.33) prefab = BluePrefab;
			else if (col < 0.66) prefab = RedPrefab;
			else prefab = GoldPrefab;

			//fish spawn locations
			pos =  new Vector3(Random.Range(-100, 100), Random.Range (20f,70f), Random.Range(-100, 100));
			//ypos = Terrain.activeTerrain.SampleHeight(pos);
			//pos.y = ypos + 20;
			Quaternion randomRotation = Quaternion.Euler( Random.Range(0, 360) , 0 , 0);
			GameObject fish = (GameObject)GameObject.Instantiate(prefab, pos, randomRotation);

			//AnimationState anim = prefab.GetComponent<Animation>().animation["Take 001"];
//			anim["Take 001"].speed = 2;
			//prefab.animation["Take 001"].speed = 10;
			//Animation animState = prefab.animation["Take 001"];
			//animState.enabled = false;
			//Debug.Log("Animation Speed = "+ animState.isPlaying);			

			if (prefab == RedPrefab) 
			{
				fish.GetComponent<Flocker>().FishColor = "red";
				redFlock.Add(fish);
				flockList.Add(fish);
			}
			if (prefab == GoldPrefab) 
			{
				fish.GetComponent<Flocker>().FishColor = "gold";
				goldFlock.Add(fish);
				flockList.Add(fish);
			}
			if (prefab == BluePrefab) 
			{
				fish.GetComponent<Flocker>().FishColor = "blue";
				blueFlock.Add(fish);
				flockList.Add(fish);
			}
		}
		
		modifyAnimations ();
	}

	void modifyAnimations(){

		foreach(GameObject fish in flockList){
			AnimationState swimAnim = fish.animation["Take 001"];
			swimAnim.time = Random.Range (0.0f,0.9f);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
		calcCentroids( );//find average position of each flocker 
		calcFlockDirections( );//find average "forward" for each flocker

		if(Input.GetKeyDown(KeyCode.R)){
			Application.LoadLevel("Ripple_Scene");
		}
		
	}
	
	
	private void calcCentroids()
	{
		Vector3 cent = Vector3.zero;
		string colr = null;
		foreach( KeyValuePair<string, List<GameObject>> pair in allFlocks)
		{
			cent = Vector3.zero;
			foreach (GameObject f in pair.Value)
			{
				cent += f.transform.position;
			}
			colr = pair.Key;
			cent /= pair.Value.Count;
			//cent -= centroidModifier;
			centroids[colr] = cent;
		}
		
		//this gameObject is used for camera control
		//gameObject.transform.position = centroids["blue"];
	}
	
	private void calcFlockDirections()
	{		
		foreach( KeyValuePair<string, List<GameObject>> pair in allFlocks)
		{
			Vector3 dir = Vector3.zero;
			foreach (GameObject f in pair.Value)
			{
				dir += f.transform.forward;
			}
			string colr = pair.Key;
			flockDirections[colr] = dir / pair.Value.Count;
		}
		gameObject.transform.forward = flockDirections["blue"];
	}
}
