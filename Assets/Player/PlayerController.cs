using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour {

	Rigidbody2D rb;
	Mesh mesh;
	public float moveForce;
	public float friction;
	public float jumpVelocity;

	bool grounded = true;
	Vector2 playerSize;
	Vector2 boxSize;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	void ColorPlayer(Color frontColor){
		Color[] c = new Color[24]{
			Color.black,
			Color.black,
			Color.black,
			Color.black,
			Color.red,
			Color.red,
			frontColor,
			frontColor,
			Color.red,
			Color.red,
			frontColor,
			frontColor,
			Color.blue,
			Color.blue,
			Color.blue,
			Color.blue,
			Color.yellow,
			Color.yellow,
			Color.yellow,
			Color.yellow,
			Color.cyan,
			Color.cyan,
			Color.cyan,
			Color.cyan
		};
        GetComponent<MeshFilter>().mesh.colors = c;

	}
	void Awake () {
		rb = GetComponent<Rigidbody2D>();
		playerSize = GetComponent<BoxCollider2D>().size;
		boxSize = new Vector2(playerSize.x, 0.05f);
		ColorPlayer(Color.white);
	}
	
	// Update is called once per frame
	void Update(){
		if (!isLocalPlayer) return;


		// Jump
		Vector2 boxCenter = (Vector2)transform.position;
		// grounded = Physics2D.OverlapBox()
		if(Input.GetButtonDown("Jump") && grounded){
			rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
		}

		// Horizontal Move
		float horizontalInput = Input.GetAxis ("Horizontal");
		rb.AddForce (Vector2.right * horizontalInput * moveForce, ForceMode2D.Impulse);
		rb.velocity = new Vector2 (rb.velocity.x * friction, rb.velocity.y);

		// Shoot
	if (Input.GetKeyDown(KeyCode.F)){
		CmdFire();
	}

	}
	// void FixedUpdate () {
	public override void OnStartLocalPlayer(){
		ColorPlayer(Color.green);
	}
	// }
	[Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

}
