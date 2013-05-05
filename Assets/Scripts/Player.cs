using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Damageable;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( NetworkView ) )]

public class Player : MonoBehaviour
{
	public GameObject playerCameraPrefab;
	
	private bool controlEnabled;
	
    public TextAsset damagableData;
	public float WalkSpeed = 8;
	public float MidAirSpeed = 8;
	public float JumpStrength = 280;
	public float JumpingSurfaceAngleStrictness = 0.7f;  //between 0 to 1; higher value requires flatter surface
 
    bool m_TouchingGround = true;
    private float m_horizontalRotation = 0;
    private bool isWalking = false;
 
    // Use this for initialization
    void Start ()
    {
		controlEnabled = false;
    	
        Terrain theMap = GameObject.FindGameObjectWithTag ("Map").GetComponent<Terrain> ();
        transform.position = new Vector3 (transform.position.x, theMap.SampleHeight (transform.position) + 10, transform.position.z);
        
        Dictionary<string,float> d = DamagableAttribute.deSerialize<float> (damagableData.text);
        
        Debug.Log (d["str"]);
        
    }
 
    // Update is called once per frame
    void Update ()
    {
     
    }
 
    void FixedUpdate ()
    {
		if ( !controlEnabled )
		{
			return;
		}
		
        /* Lateral movement */
        if (m_TouchingGround)
        {
            MoveOnGround ();
        }
        else
        {
            MoveInAir ();
        }
     
        /* Jumping */
        if (Input.GetButtonDown ("Jump"))
        {
            if (m_TouchingGround)
            {
                rigidbody.AddForce (0, JumpStrength, 0);
            }
        }
    }
 
    void OnCollisionStay (Collision collision)
    {
        if (ContactPointIsJumpoffable (collision))
        {
            m_TouchingGround = true;
        }
    }
 
    void OnCollisionExit (Collision collision)
    {
        if (ContactPointIsJumpoffable (collision))
        {
            m_TouchingGround = false;
        }
    }
 
    private bool ContactPointIsJumpoffable (Collision collision)
    {
        foreach (ContactPoint contactPoint in collision.contacts)
        {
            if (contactPoint.thisCollider == this.collider)
            {
                if (contactPoint.normal.y > JumpingSurfaceAngleStrictness)
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
	/// <summary>
	/// Instantaneous velocity change, a la CharacterController movement
	/// </summary>
	private void MoveOnGround()
	{
//------------------- Rohan: Modified your code a little to work better with rotation. -------------------
        // Only apply impulses if we're actually pressing the key.
        if ((Input.GetAxis ("Vertical") != 0) || (Input.GetAxis ("Horizontal") != 0))
        {
            // Use the rotation of the character in the calculation for which direction is forward/backward/left/right.
            Vector3 forwardVelocity = transform.forward * Input.GetAxisRaw ("Vertical") * Time.deltaTime * WalkSpeed;
            Vector3 sideVelocity = transform.right * Input.GetAxisRaw ("Horizontal") * Time.deltaTime * WalkSpeed;
            // The target velocity is the sum of these two velocities.
            Vector3 targetVelocity = forwardVelocity + sideVelocity;
            rigidbody.AddForce (targetVelocity, ForceMode.VelocityChange);
            // Signify that we have pressed the key, so releasing the key will cause the player to stop.
            isWalking = true;
        }
        else if (m_TouchingGround && isWalking)
         // If we were walking and we're currently touching the ground, half the player's speed
         // so that they can transition into a stop more naturally.
        {
            rigidbody.velocity = rigidbody.velocity * .5f;
            isWalking = false;
        }
//------------------- Rohan: This is where my modifications end. --------------------------------------------
		
		/* Rohan: I've preserved your code down here.
		Vector3 targetVelocity = new Vector3( Input.GetAxisRaw( "Horizontal" ) * WalkSpeed - rigidbody.velocity.x,
										0,
										Input.GetAxisRaw( "Vertical" ) * WalkSpeed - rigidbody.velocity.z );
		rigidbody.AddForce( targetVelocity , ForceMode.VelocityChange );
		*/
	}
	
	/// <summary>
	/// Gradual velocity change.
	/// </summary>
	private void MoveInAir()
	{
		Vector3 velocityChange = new Vector3( Input.GetAxisRaw( "Horizontal" ) * MidAirSpeed,
												0,
												Input.GetAxisRaw( "Vertical" ) * MidAirSpeed );
		rigidbody.AddForce( velocityChange );
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo networkMessageInfo)
	{
		
	}
	
	[RPC]
	void SetOwner(NetworkPlayer networkPlayer)
	{
		Debug.Log ("setting owner");
		
		if ( networkPlayer == Network.player )
		{
			Debug.Log ("Owner set to me");
			
			//Player playerScript = player.GetComponent<Player>();
			
			AttachPlayerCamera();
		}
	}
	
	void AttachPlayerCamera()
	{
		GameObject playerCamera = (GameObject) Instantiate(playerCameraPrefab);
		PlayerCamera playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
		playerCameraScript.SetTarget(this.gameObject);
		playerCamera.camera.enabled = true;
	}
}
