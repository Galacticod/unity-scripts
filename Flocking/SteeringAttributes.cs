using UnityEngine;
using System.Collections;

public class SteeringAttributes : MonoBehaviour {
	
	//these are common attributes required for steering calculations 
	public float maxSpeed = 12.0f;
	public float maxForce = 12.0f;
	public float mass = 1.0f;
	public float radius = 1.0f;
	
	public float seekWt = 0.0f;
	public float inBoundsWt = 30.0f;
	public float avoidWt = 20.0f;
	public float avoidDist = 6.0f;

	public float separationDist = 3.0f;

	public float alignmentWt = 8.0f;
	public float separationWt = 12.0f;
	public float cohesionWt = 1.0f;
	public float centerWt = 1.0f;

	
}
