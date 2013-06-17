using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( NetworkView ) )]

/*	Server-side script.
 * 	Receives player inputs and updates authoritative instance.
 */
public class PlayerManager : MonoBehaviour {
	
	private float verticalAxisInput = 0;
	private float horizontalAxisInput = 0;
	private Quaternion rotation = Quaternion.identity;
	private bool jump = false;
	
	private Player playerScript;
	
	// Use this for initialization
	void Start ()
	{
		playerScript = gameObject.GetComponent<Player>();
		Debug.Log ("PlayerManager initialized");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(Network.isClient)
		{
			return;
		}
		
		//authoritative movement logic goes here, updates are automatically sent through the Transform observed by (still uncreated) Predictor script
		//call overloaded Player.ProcessPlayerInput()
		
		// update authoritative simulation
		playerScript.ProcessPlayerInput(verticalAxisInput, horizontalAxisInput, rotation, jump);
	}
	
	[RPC]
	public void UpdateClientInput(float verticalAxis, float horizontalAxis, Quaternion rotation, bool jump) //give client 100% rotation control
	{
		this.verticalAxisInput = verticalAxis;
		this.horizontalAxisInput = horizontalAxis;
		this.rotation = rotation;
		this.jump = jump;
	}
}
