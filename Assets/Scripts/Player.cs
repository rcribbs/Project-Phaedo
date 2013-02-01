using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public float WalkSpeed = 12;
	public float JumpStrength = 280;  //amount of force in a jump
	public float JumpingSurfaceAngleStrictness = 0.7f;  //between 0 to 1; higher value requires flatter surface
	
	bool TouchingGround = true; //whether player is touching the floor
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		/* Check if grounded */
		
		
		float horizontalDirection = Input.GetAxis( "Horizontal" );
		float verticalDirection = Input.GetAxis( "Vertical" );
		
		
		
		if( Input.GetButtonDown("Jump") )
		{
			if( TouchingGround ){
				rigidbody.AddForce(0, JumpStrength, 0);
			}
		}
	}
	
	void OnCollisionStay (Collision collision)
	{
		if( ContactPointIsJumpoffable( collision))
		{
			TouchingGround = true;
		}
	}
	
	void OnCollisionExit (Collision collision)
	{
		if( ContactPointIsJumpoffable( collision ))
		{
			TouchingGround = false;
		}
	}
	
	private bool ContactPointIsJumpoffable (Collision collision)
	{
		foreach( ContactPoint contactPoint in collision.contacts )
		{
			if( contactPoint.thisCollider == this.collider )
			{
				if( contactPoint.normal.y > JumpingSurfaceAngleStrictness )
				{
					return true;
				}
			}
			
			break;
		}
		
		return false;
	}
}
