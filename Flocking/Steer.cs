// The Steer component has a collection of functions
// that return forces for steering 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

public class Steer : MonoBehaviour
{
	Vector3 dv = Vector3.zero; 	// desired velocity, used in calculations
	SteeringAttributes attr; 	// attr holds several variables needed for steering calculations
	CharacterController characterController;

	
	void Start ()
	{
		GameObject main = GameObject.Find("MainGO");
		attr = main.GetComponent<SteeringAttributes> ();
		characterController = gameObject.GetComponent<CharacterController> ();	
	}
	
	
	//-------- functions that return steering forces -------------//
	public Vector3 Seek (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;		
		dv = dv.normalized * attr.maxSpeed; 	//scale by maxSpeed
		dv -= characterController.velocity;
		//dv.y = 0;								// only steer in the x/z plane
		return dv;
	}

	public Vector3 Flee (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = transform.position - targetPos;		
		dv = dv.normalized * attr.maxSpeed; 	//scale by maxSpeed
		dv -= characterController.velocity;
		//dv.y = 0;								// only steer in the x/z plane
		return dv;
	}
	
	
	// The Arraylist parameter could be a list maintained by the GameManager,
	// or (later on, using the vision model), a list generated dynamically by each individual
	public Vector3 Separate (List<GameObject> neighbors)
	{
		Vector3 total = Vector3.zero;

		foreach(GameObject n in neighbors)
		{
			Vector3 dv = Vector3.zero;
			dv = transform.position - n.transform.position;		
			
			float dist = dv.magnitude; 
			if (dist> 0 && dist < attr.separationDist)
			{
				dv *=  attr.separationDist / dist; 	//scale by maxSpeed
			
				total += dv;
			}
		}
		total = total.normalized * attr.maxSpeed; 	//scale by maxSpeed
		total -= characterController.velocity;
		 
		return total;
	}
	
	
	// Current strategy: the GameManager component calculates the direction 
	// once per frame so all flockers can make use of it 
	public Vector3 Align (Vector3 direction)
	{
		dv = direction.normalized * attr.maxSpeed; 	 
		dv -= characterController.velocity;
		//dv.y = 0;								 
		return dv;
	}
	
	
	// Current strategy is to have the centroid calculated
	// in the GameManager component once per frame, so that all flockers can make use of it
	public Vector3 Cohere (Vector3 targetPos)
	{
		return Seek(targetPos);
	}
	
	
	
	// tether type containment - not very good! But ok for demo
	public Vector3 StayInBounds (float radius, Vector3 center)
	{
		if (Vector3.Distance (transform.position, center) > radius)
		{
			return Seek (center);
		}
		else
		{
			return Vector3.zero;
		}
	}	

	public Vector3 AvoidObstacle (GameObject obst, float safeDistance)
	{ 
		dv = Vector3.zero;
		float obRadius = obst.GetComponent<ObstacleScript> ().Radius;

		//vector from vehicle to center of obstacle
		Vector3 vecToCenter = obst.transform.position - transform.position;
		//eliminate y component so we have a 2D vector in the x, z plane
		vecToCenter.y = 0;
		float dist = vecToCenter.magnitude;

		// if too far to worry about, out of here
		if (dist > safeDistance + obRadius + attr.radius)
			return Vector3.zero;
		
		//if behind us, out of here
		if (Vector3.Dot (vecToCenter, transform.forward) < 0)
			return Vector3.zero;

		float rightDotVTC = Vector3.Dot (vecToCenter, transform.right);
		
		//if we can pass safely, out of here
		if (Mathf.Abs (rightDotVTC) > attr.radius + obRadius)
			return Vector3.zero;
				
		//obstacle on right so we steer to left
		if (rightDotVTC > 0)
			dv += transform.right * -attr.maxSpeed * safeDistance / dist;
		else
		//obstacle on left so we steer to right
			dv += transform.right * attr.maxSpeed * safeDistance / dist;
			
		return dv;	
	}
	
	
	
}
