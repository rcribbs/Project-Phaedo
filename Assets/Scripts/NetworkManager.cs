using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	public bool isServer = true;
	public bool isConnectingToServer = false;
	public string ipAddress = "127.0.0.1";

	// Use this for initialization
	void Start ()
	{
		if(isServer)
			Network.InitializeServer( 2, 6758 );
		else if(isConnectingToServer)
			ConnectToServer();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void ConnectToServer()
	{
		Network.Connect( ipAddress, 6758, "" );
	}
}
