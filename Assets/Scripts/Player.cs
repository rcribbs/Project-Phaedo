using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float walkSpeed = 12;
	public float jumpSpeed = 12;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalDirection = Input.GetAxis("Horizontal");
		float verticalDirection = Input.GetAxis("Vertical");
		int jumping = 0;
		if( Input.GetButtonDown("Jump"))
			jumping = 1;
		transform.Translate( walkSpeed * Time.deltaTime * horizontalDirection, jumpSpeed * Time.deltaTime * jumping * -1, walkSpeed * Time.deltaTime * verticalDirection );
	}
}
