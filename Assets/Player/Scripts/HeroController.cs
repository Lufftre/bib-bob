using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class HeroController : NetworkBehaviour {

	[SyncVar (hook = "OnNameChanged")] public string playerName;
	[SyncVar (hook = "OnColorChanged")] public Color playerColor;

	Rigidbody2D rb;
	Mesh mesh;
	public float moveForce;
	public float friction;
	public float jumpVelocity;

	public bool grounded = true;
	Vector2 playerSize;
	Vector2 boxSize;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	float bulletSpeed = 20;
	int maxJumps = 2;
	int curJumps = 0;

	public float fireRate;
    private float nextFire;

	public AudioClip audioFire;
	public AudioClip audioJump;
	public AudioClip audioHurt;
	AudioSource audioSource;

	Health health;
	public LayerMask layerMask;
	public float rayDistance;

	public float fireKnockback;

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
		boxSize = new Vector2(playerSize.x - .05f, 0.1f);
		//ColorPlayer(Color.white);
		audioSource = GetComponent<AudioSource>();
		health = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update(){
		if (!isLocalPlayer) return;


		// Jump
		if(Input.GetButtonDown("Jump") && (grounded || curJumps < maxJumps)){
			// rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
			rb.velocity = new Vector2(rb.velocity.x, 10);
			grounded = false;
			curJumps += 1;
			audioSource.PlayOneShot(audioJump, 0.2F);
		} else {
			bool prevGrounded = grounded;
			//Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (Mathf.Sqrt(playerSize.y*2) + boxSize.y) * .5f;
			//grounded = (Physics2D.Raycast(rb.position, Vector2.down, 1f, layerMask) != null);
			RaycastHit2D rayhit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, layerMask);
			if(rayhit) grounded = true; else grounded = false;
			//grounded = (Physics2D.OverlapBox(boxCenter, boxSize, 0f, LayerMask.GetMask("World")) != null);
			if (grounded){
				curJumps = 0;
			} else if (prevGrounded && !grounded){
				curJumps += 1;
			}
		}
		// Horizontal Move
		float horizontalInput = Input.GetAxis ("Horizontal");
		rb.AddForce (Vector2.right * horizontalInput * moveForce, ForceMode2D.Impulse);
		rb.velocity = new Vector2 (rb.velocity.x * friction, rb.velocity.y);

		// Shoot
		if (Input.GetButton("Fire1") && Time.time > nextFire){
			nextFire = Time.time + fireRate;
			rb.AddForce(transform.right * -fireKnockback, ForceMode2D.Impulse);
			audioSource.PlayOneShot(audioFire, 0.5F);
			CmdFire();
		}

		if (transform.position.y < -50){
			// health.TakeDamage(health.currentHealth);
			health.RpcRespawn();
		}
	}

	public override void OnStartLocalPlayer(){
		//ColorPlayer(Color.green);
		Camera.main.GetComponent<CameraFollow>().setTarget(gameObject.transform);
		health.lives = 1;
	}

	[Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.forward * bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 20 seconds
        Destroy(bullet, 20.0f);
    }

	public void playAudioHurt(){
		audioSource.PlayOneShot(audioHurt, 0.5F);
	}

	void OnNameChanged(string value) {
		playerName = value;
		gameObject.name = playerName;
	}

	void OnColorChanged(Color value) {
		playerColor = value;
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = playerColor;
	}
}
