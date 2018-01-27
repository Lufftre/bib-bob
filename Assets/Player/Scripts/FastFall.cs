using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastFall : MonoBehaviour {

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;
	public float stopMultiplier = 5f;

	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (rb.velocity.y < 0) {
			rb.gravityScale = fallMultiplier;
		} else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
			rb.gravityScale = lowJumpMultiplier;
		} else {
			rb.gravityScale = 1;
		}

		if (Mathf.Abs(rb.velocity.x) > 0 && !Input.GetButton("Horizontal")){
			rb.velocity = new Vector2 (rb.velocity.x * (1 - 1/stopMultiplier), rb.velocity.y);
		}
	}
	
}
