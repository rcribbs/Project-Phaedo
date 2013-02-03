using UnityEngine;
using System.Collections;

public class script_MainCamera : MonoBehaviour {
	
	GameObject m_Player;

	// Use this for initialization
	void Start ()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
