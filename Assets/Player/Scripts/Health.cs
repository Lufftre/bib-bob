using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Prototype.NetworkLobby;
public class Health : NetworkBehaviour {

    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
	public bool destroyOnDeath;

    public RectTransform healthBar;
    //private NetworkStartPosition[] spawnPoints;
    //[SyncVar]
    public TextMesh deathText;

    [SyncVar(hook = "OnDeath")]
    public int lives;

    HeroController pc;
    Rigidbody2D rb;
   
   //[SyncVar(hook = "OnOB")]
   public bool canOB = true;

   // void OnOB(bool ob){
   //     canOB = ob;
   // }
    void Start() {
        
        pc = GetComponent<HeroController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount){
        if (!isServer)
            return;
        
        currentHealth -= amount;
        if (currentHealth <= 0) {
            pc.playAudioHurt();
			if (destroyOnDeath) {
				Destroy(gameObject);
			} else {
                lives -= 1;
                if (lives > 0){
                    RpcRespawn();
                } else {
                    RpcHandlePlayerDeath();
                }
				currentHealth = maxHealth;
			}
        }
    }

    public void OnDeath(int deathCount){
        lives = deathCount;
        deathText.text = deathCount.ToString();
    }

    void OnChangeHealth (int health){
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    public void RpcRespawn(){
        if (isLocalPlayer) {
           // if (lives > 0 ){
                // Set the spawn point to origin as a default value
                Vector3 spawnPoint = Vector3.zero;
                NetworkStartPosition[] spawnPoints = FindObjectsOfType<NetworkStartPosition>();
                // If there is a spawn point array and the array is not empty, pick one at random
                if (spawnPoints != null && spawnPoints.Length > 0) {
                    spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                }

                // Set the player’s position to the chosen spawn point

                rb.velocity = Vector2.zero;
                transform.position = spawnPoint;
                CmdCanOB();
           // } else {
            //}
        }
    }
    [ClientRpc]
    public void RpcHandlePlayerDeath(){
        if (isLocalPlayer) {
            Camera.main.transform.position = new Vector3(0, 0, -35);
            Camera.main.transform.rotation = Quaternion.identity;
            rb.velocity = Vector2.zero;
            transform.position = Vector3.zero;
            CmdHandlePlayerDeath();
        }

    }
    [Command]
    void CmdCanOB(){
        canOB = true;
    }
    [Command]
    void CmdHandlePlayerDeath(){
        GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>().CmdHandlePlayerDeath(gameObject);
    }
}