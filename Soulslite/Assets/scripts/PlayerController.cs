using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float speed;
	private Rigidbody2D rb2d;

	// Used for initialization
	void Start() {
		rb2d = GetComponent<Rigidbody2D>();
	}

	// FixedUpdate is called at a fixed interval independent of frame rate -- Physics code goes here.
	void FixedUpdate() {
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
        rb2d.velocity.Set(moveHorizontal * speed, moveVertical * speed);
	}
}
