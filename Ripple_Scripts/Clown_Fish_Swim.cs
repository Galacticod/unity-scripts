using UnityEngine;
using System.Collections;

public class Clown_Fish_Swim : MonoBehaviour {

	// Unity variables
	GameObject clownFish;
	CharacterController cc;
	Animator anim;

	// Movement variables
	public float turnSpeed, accSpeed, maxSpeed, VEL, ACC, DRAG;
	private float TurnVerticalSpeed = 4f;
	private float maxTurnSpeed = 4f;
	private float minTurnSpeed = 2.5f;

	// for clamping rotation
	float rotation;
	Quaternion OGrotation;

	void Start () {
		// Set vars
		turnSpeed = minTurnSpeed;
		accSpeed = 0.75f;
		maxSpeed = 10.0f;
		VEL = 0f;
		ACC = 0f;
		DRAG = 0.20f;

		OGrotation = transform.localRotation;

		//Galacticod ENGAGE!
		clownFish = GameObject.Find("_P_Clown_Fish_Character_");
		cc = clownFish.GetComponent<CharacterController>();
		anim = clownFish.GetComponent<Animator>();
	}



	void Update () {
		// Update swim animation parameter based on current velocity
		anim.SetFloat ("Swim_Speed", VEL);

		rotation = transform.localRotation.eulerAngles.x;

		// Update velocity
		UpdateVelocity ();

		if(Input.GetKeyDown(KeyCode.F)){
			clownFish.transform.position = new Vector3(-20,48,-40);
		}
	}
	


/*
 * Velocity functions
*/
	void UpdateVelocity() 
	{
		ApplySpeed ();

		ApplyDrag ();

		// Move fish by multiplying velocity float by fish's forward facing normalized vector
		cc.Move(transform.forward*VEL * Time.deltaTime);
		
		//reset acceleration
		ACC = 0.0f;		
	}

	void ApplySpeed()
	{
		VEL += ACC;
		
		//limit speed
		if(VEL > maxSpeed){
			VEL = maxSpeed;
		}
	}

	public void ApplyDrag()
	{
		//if velocity is greater than 0, apply drag force (decrement velocity),
		//otherwise, set velocity = 0
		if(VEL > 0){
			VEL -= DRAG;
		}
		else{
			VEL = 0;
		}
	}

	public void IncreaseTurnSpeed() {
		turnSpeed = maxTurnSpeed;
	}
	public void DecreaseTurnSpeed() {
		turnSpeed = minTurnSpeed / 4;
	}
	public void ResetTurnSpeed() {
		turnSpeed = minTurnSpeed;
	}




/*
 * Movement functions
*/
	public void MoveForward()
	{
		ACC += accSpeed;
	}
	public void TurnRight()
	{
		transform.Rotate(0,(turnSpeed/2),0,Space.World);
	}
	public void TurnLeft() 
	{
		transform.Rotate(0,(-turnSpeed/2), 0,Space.World);
	}




	/*
	 * Rotation stuff
	 */
	
	
	float rotMax = 320f;
	float rotMin = 45f;

	public void TurnUp()
	{
		if (rotation < rotMin + 1.5f || rotation - turnSpeed > rotMax + 1f) 
			transform.Rotate(Vector3.left * TurnVerticalSpeed / 2,Space.Self);
	}
	public void TurnDown()
	{
		if (rotation < rotMin || rotation >= rotMax)
			transform.Rotate (Vector3.right * TurnVerticalSpeed / 2, Space.Self);
	}


	public void LevelOff()
	{

		Vector3 ogRotation = OGrotation.eulerAngles;
		Vector3 curRotation = transform.localRotation.eulerAngles;
		curRotation.x = ogRotation.x;
		curRotation.z = ogRotation.z;
		Quaternion newRotation = Quaternion.Euler (curRotation);

		Quaternion current = transform.localRotation;
		transform.localRotation = Quaternion.Slerp(current, newRotation, Time.deltaTime);
	}
}
