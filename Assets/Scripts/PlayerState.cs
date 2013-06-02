using UnityEngine;
using System.Collections;

public class PlayerState {
	
	public float timestamp;
	public Vector3 position;
	public Quaternion rotation;
	
	PlayerState(float timestamp, Vector3 position, Quaternion rotation)
	{
		this.timestamp = timestamp;
		this.position = position;
		this.rotation = rotation;
	}
}
