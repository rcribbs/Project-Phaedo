using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Rigidbody ) )]

public class Player : MonoBehaviour
{
	
	public float WalkSpeed = 8;
	public float MidAirSpeed = 8;
	public float JumpStrength = 280;
	public float JumpingSurfaceAngleStrictness = 0.7f;  //between 0 to 1; higher value requires flatter surface
	
	bool m_TouchingGround = true;
	
	private float m_horizontalRotation = 0;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void FixedUpdate ()
	{
		/* Lateral movement */
		if( m_TouchingGround )
		{
			MoveOnGround();
		}
		else
		{
			MoveInAir();
		}
		
		/* Jumping */
		if( Input.GetButtonDown("Jump") )
		{
			if( m_TouchingGround )
			{
				rigidbody.AddForce( 0, JumpStrength, 0 );
			}
		}
	}
	
	void OnCollisionStay (Collision collision)
	{
		if( ContactPointIsJumpoffable( collision))
		{
			m_TouchingGround = true;
		}
	}
	
	void OnCollisionExit (Collision collision)
	{
		if( ContactPointIsJumpoffable( collision ))
		{
			m_TouchingGround = false;
		}
	}
	
	private bool ContactPointIsJumpoffable( Collision collision )
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
	

	/// <summary>
	/// Rotates the player with the camera.
	/// </summary>
	/// <param name='horizontalRotation'>
	/// Horizontal rotation of the player.
	/// </param>
	public void rotatePlayer(float horizontalRotation)
	{
		m_horizontalRotation = horizontalRotation;
		transform.rotation = Quaternion.Euler( transform.forward + new Vector3( 0, m_horizontalRotation, 0) );
	}
	
	/* Instantaneous velocity change, a la CharacterController movement */
	private void MoveOnGround()
	{
		Vector3 targetVelocity = new Vector3( Input.GetAxisRaw( "Horizontal" ) * WalkSpeed - rigidbody.velocity.x,
										0,
										Input.GetAxisRaw( "Vertical" ) * WalkSpeed - rigidbody.velocity.z );
		rigidbody.AddForce( targetVelocity , ForceMode.VelocityChange );
	}
	
	/* Gradual velocity change */
	private void MoveInAir()
	{
		Vector3 velocityChange = new Vector3( Input.GetAxisRaw( "Horizontal" ) * MidAirSpeed,
												0,
												Input.GetAxisRaw( "Vertical" ) * MidAirSpeed );
		rigidbody.AddForce( velocityChange );
	}
}
