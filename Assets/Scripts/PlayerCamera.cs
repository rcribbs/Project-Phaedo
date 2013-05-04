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
	private int mousewheel = 0;
	private float scrollOffset = 0;
 
	// Use this for initialization
	void Start ()
	{
		if( !target )
			Debug.LogError( "The camera target has not been set!" );
		Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target)
		{
			if (inFirstPersonMode)
			{
				FirstPersonCameraMode();
				CheckAttack();
			}
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
		// This means that the mousewheel is being moved towards the user aka zooming out.
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (mousewheel > 3)
			{
				inFirstPersonMode = false;
				mousewheel = 0;
			}
			else
			{
				Debug.Log(mousewheel);
				if (mousewheel == 0)
				{
					StartCoroutine(ResetMouseWheel());
				}
				mousewheel += 1;
			}
		}
	}
	
	void ThirdPersonCameraMode()
	{
		camera.cullingMask = DefaultCulling;
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
		else if (cameraVerticalOffset <= 0)
			cameraVerticalOffset = 0;
		// Set the target's rotation.
		Player targetScript = (Player)target.GetComponent ("Player");
		targetScript.rotatePlayer (cameraHorizontalOffset);
		float zoomOffset = -5 + scrollOffset;
		// Move to the target's location.
		transform.position = new Vector3 (target.transform.position.x, 2.0f + target.transform.position.y, target.transform.position.z) + transform.forward * zoomOffset;
		// Set rotation to the current looking direction.
		transform.rotation = Quaternion.Euler (new Vector3 (cameraVerticalOffset, cameraHorizontalOffset, 0));
		if ((scrollOffset > -15) && Input.GetAxis("Mouse ScrollWheel") > 0)
			scrollOffset -= Input.GetAxis("Mouse ScrollWheel");
		if ((scrollOffset < 0) && Input.GetAxis("Mouse ScrollWheel") < 0)
			scrollOffset -= Input.GetAxis("Mouse ScrollWheel");
		if ((Input.GetAxis("Mouse ScrollWheel") < 0) && scrollOffset >= 0)
		{
			if (mousewheel > 6)
			{
				inFirstPersonMode = true;
				mousewheel = 0;
			}
			else
			{
				Debug.Log(mousewheel);
				if (mousewheel == 0)
				{
					Debug.Log("Inside if!");
					StartCoroutine(ResetMouseWheel());
				}
				mousewheel += 1;
			}
		}
	}
	
	void CheckAttack()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			float pokeForce = 350;
			RaycastHit hit;
			Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			if (Physics.Raycast(ray, out hit))
			{
                if (hit.rigidbody != null)
				{
					Debug.Log("Yahtzee! You hit: " + hit.rigidbody.name + "!");
					hit.rigidbody.AddForceAtPosition(ray.direction * pokeForce, hit.point);
				}
				else
					Debug.Log("You hit nothing! :(");
                   // hit.rigidbody.AddForceAtPosition(ray.direction * pokeForce, hit.point);
			}
		}
	}
		
	IEnumerator ResetMouseWheel()
	{
		Debug.Log("Starting mouswheel wait!");
		yield return new WaitForSeconds(4.0f);
		mousewheel = 0;
		Debug.Log("Reset mousewheel parameter!");
	}
}