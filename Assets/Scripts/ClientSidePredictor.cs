using UnityEngine;
using System.Collections;

public class ClientSidePredictor : MonoBehaviour {
	
	public Transform observedTransform;
	public Player playerScript;

	// Use this for initialization
	void Start ()
	{
		if (observedTransform == null)
		{
			Debug.LogError("Observed Transform field is unassigned!");
		}
		if (playerScript == null)
		{
			Debug.LogError("Player Script field is unassigned!");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnSerializeNetworkView(BitStream bitstream, NetworkMessageInfo networkMessageInfo)
	{
		Vector3 position = observedTransform.position;
		Quaternion rotation = observedTransform.rotation;
		
		if (bitstream.isWriting)
		{
			// we are the server.  Write values to clients.
			bitstream.Serialize(ref position);
			bitstream.Serialize(ref rotation);
		}
		else
		{
			// we are the client.  Read values (in same order as they were written!) and lerp (or herp derp).
			// serialize into 'position' and 'rotation'.
			bitstream.Serialize(ref position);
			bitstream.Serialize(ref rotation);
			
			playerScript.lerpToTarget(position, rotation);
		}
			
	}
}
