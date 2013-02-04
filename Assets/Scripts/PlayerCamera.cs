using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
	
	public GameObject target = null;           // the target camera is following
	public float dampTime = 0.3f;          // offset from the viewport center to fix damping
	public float distance = 10f;          // distance away on z axis from the target
	public float lookSpeed = 60;
	public bool inverted = false;
	
	private float cameraHorizontalOffset = 0;
	private float cameraVerticalOffset = 0;
	private bool inFirstPersonMode = true;
	private static int DefaultCulling = -1;
 
	// Use this for initialization
	void Start ()
	{
		if( !target )
			Debug.LogError( "The camera target has not been set!" );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target)
		{
			if (inFirstPersonMode)
				FirstPersonCameraMode();
			else
				ThirdPersonCameraMode();
		}
	}
	
	/// <summary>
	/// Manipulates camera position based on mouse inputs for first person mode.
	/// </summary>
	void FirstPersonCameraMode()
	{
		// Show only the objects on the Default layer.
		camera.cullingMask = 1 << 0;
		// Compensate for inverted or standard vertical look.
		int invert = -1;
		if (inverted)
			invert = 1;
		// Set the horizontal and vertical offsets for look directions.
		cameraHorizontalOffset += Input.GetAxis ("Mouse X") * lookSpeed * Time.deltaTime;
		cameraVerticalOffset += Input.GetAxis ("Mouse Y") * lookSpeed * Time.deltaTime * invert;
		// Enforce the boundaries for looking up and down.
		if (cameraVerticalOffset > 90)
			cameraVerticalOffset = 90;
		else if (cameraVerticalOffset < -90)
			cameraVerticalOffset = -90;
		// Set the target's rotation.
		Player targetScript = (Player)target.GetComponent ("Player");
		targetScript.rotatePlayer (cameraHorizontalOffset);
		// Move to the target's location.
		transform.position = new Vector3 (target.transform.position.x, 3.5f + target.transform.position.y, target.transform.position.z);
		// Set rotation to the current looking direction.
		transform.rotation = Quaternion.Euler (new Vector3 (cameraVerticalOffset, cameraHorizontalOffset, 0));
	}
	
	void ThirdPersonCameraMode()
	{
	}
}