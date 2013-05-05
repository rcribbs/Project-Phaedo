using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	public bool isServer = true;
	public bool isConnectingToServer = false;
	public string ipAddress = "127.0.0.1";
	public GameObject playerPrefab;
	
	int playerCount = 0;

	// Use this for initialization
	void Start ()
	{
		if(isServer)
			Network.InitializeServer( 10, 6758, false );
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
	
	void OnConnectedToServer()
	{
		Debug.Log ( "Connected to server." );
		
		
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log( "Player connected from: " + player.externalIP );
		playerCount++;
		
		GameObject playerObject = (GameObject) Network.Instantiate( playerPrefab, new Vector3(82, 28, 294), Quaternion.identity, 0 );
		
		NetworkView networkView = playerObject.GetComponent<NetworkView>();
		networkView.RPC( "SetOwner", RPCMode.AllBuffered, player );
	}
	
	
}
