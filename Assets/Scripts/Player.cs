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
	public float LerpingErrorDistanceThreshold = 0.2f;
 
    bool m_TouchingGround = true;
    private float m_horizontalRotation = 0;
    private bool isWalking = false;
	
	void Awake ()
	{
		controlEnabled = false;
	}
 
    // Use this for initialization
    void Start ()
    {
        Terrain theMap = GameObject.FindGameObjectWithTag ("Map").GetComponent<Terrain> ();
        transform.position = new Vector3 (transform.position.x, theMap.SampleHeight (transform.position) + 10, transform.position.z);
        
        Dictionary<string,float> d = DamagableAttribute.deSerialize<float> (damagableData.text);
        
        //Debug.Log (d["str"]);
        
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
		
        ProcessPlayerInput();
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
	
	/// <summary>
	/// Processes all player input that the server needs to know about.  Should only be called by client.
	/// </summary>
	public void ProcessPlayerInput ()
	{
		// keyboard input
		float verticalInput = Input.GetAxisRaw("Vertical");
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		bool jumpInput = Input.GetButtonDown("Jump");
		
		/* NOTE: rotation input is read and processed in the client sim by PlayerCamera.cs.  
		 * Just send transform.rotation updates to the server. */
		
		// tell server this client's input
		this.networkView.RPC("UpdateClientInput", RPCMode.Server, verticalInput, horizontalInput, transform.rotation, jumpInput);
		
		// update client simulation
		ProcessPlayerInput(verticalInput, horizontalInput, Quaternion.identity, jumpInput);
	}
	
	/// <summary>
	/// Executes input values on player object.
	/// </summary>
	/// <param name='vertical'>
	/// Vertical.
	/// </param>
	/// <param name='horizontal'>
	/// Horizontal.
	/// </param>
	/// <param name='rotation'>
	/// Rotation.  
	/// </param>
	/// <param name='jump'>
	/// Jump.
	/// </param>
	public void ProcessPlayerInput (float vertical, float horizontal, Quaternion rotation, bool jump)
	{
		// Lateral movement
        if (m_TouchingGround)
        {
            MoveOnGround (vertical, horizontal);
        }
        else
        {
            MoveInAir (vertical, horizontal);
        }
     
        // Jumping
        if (jump)
        {
            if (m_TouchingGround)
            {
                rigidbody.AddForce (0, JumpStrength, 0);
            }
        }
		
		// Rotation
		if (rotation != Quaternion.identity)
		{
			rotatePlayer(rotation.eulerAngles);
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
	public void rotatePlayer(Vector3 eulerAngles)
	{
		// horizontal rotation
		m_horizontalRotation = eulerAngles.y;  //Justin: is this class variable needed?
		transform.rotation = Quaternion.Euler( new Vector3( 0, m_horizontalRotation, 0) );
		
		// vertical rotation (head/gun movement?)
		
	}
	/// <summary>
	/// Instantaneous velocity change, a la CharacterController movement
	/// </summary>
	private void MoveOnGround(float vertical, float horizontal)
	{
//------------------- Rohan: Modified your code a little to work better with rotation. -------------------
        // Only apply impulses if we're actually pressing the key.
        if ((vertical != 0) || (horizontal != 0))
        {
            // Use the rotation of the character in the calculation for which direction is forward/backward/left/right.
            Vector3 forwardVelocity = transform.forward * vertical * Time.deltaTime * WalkSpeed;
            Vector3 sideVelocity = transform.right * horizontal * Time.deltaTime * WalkSpeed;
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
	private void MoveInAir(float vertical, float horizontal)
	{
		Vector3 velocityChange = new Vector3( horizontal * MidAirSpeed,
												0,
												vertical * MidAirSpeed );
		rigidbody.AddForce( velocityChange );
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo networkMessageInfo)
	{
		
	}
	
	/// <summary>
	/// Sets a client to become "owner" of this player character.
	/// </summary>
	/// <param name='networkPlayer'>
	/// The NetworkPlayer identifier of the client which is to become the owner.
	/// </param>
	[RPC]
	void SetOwner(NetworkPlayer networkPlayer)
	{
		Debug.Log ("setting player owner");
		
		if ( networkPlayer == Network.player )
		{
			Debug.Log ("Owner set to me");
			
			AttachPlayerCamera();
			
			controlEnabled = true;
		}
	}
	
	void AttachPlayerCamera()
	{
		GameObject playerCamera = (GameObject) Instantiate(playerCameraPrefab);
		PlayerCamera playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
		playerCameraScript.SetTarget(this.gameObject);
		playerCamera.camera.enabled = true;
	}
	
	/// <summary>
	/// Lerps (linearly interpolates) the client's player character state to match the authoritative simulation.
	/// </summary>
	/// <param name='serverPosition'>
	/// Server position.
	/// </param>
	/// <param name='serverRotation'>
	/// Server rotation.
	/// </param>
	public void lerpToTarget(Vector3 serverPosition, Quaternion serverRotation)
	{
		float errorDistance = Vector3.Distance(transform.position, serverPosition);
		
		if (errorDistance >= LerpingErrorDistanceThreshold)
		{
			float lerp = (WalkSpeed / errorDistance) / 100; //Justin: haven't figured out why 100 yet...
			
			transform.position = Vector3.Lerp(transform.position, serverPosition, lerp);
			transform.rotation = Quaternion.Slerp(transform.rotation, serverRotation, lerp);
		}
		
	}
}
