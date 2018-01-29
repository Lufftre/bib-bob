using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour {

    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
	public bool destroyOnDeath;

    public RectTransform healthBar;
    private NetworkStartPosition[] spawnPoints;
    public TextMesh deathText;

    [SyncVar(hook = "OnDeath")]
    public int deaths = 0;

    PlayerController pc;
    

    void Start() {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        pc = GetComponent<PlayerController>();
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
                deaths += 1;
				currentHealth = maxHealth;
				RpcRespawn();
			}
        }
    }

    void OnDeath(int deathCount){
        deathText.text = deathCount.ToString();
    }

    void OnChangeHealth (int health){
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn(){
        if (isLocalPlayer) {
            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0) {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }
}