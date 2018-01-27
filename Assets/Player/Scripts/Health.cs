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

    public void TakeDamage(int amount){
        if (!isServer)
            return;
        
        currentHealth -= amount;
        if (currentHealth <= 0) {
			if (destroyOnDeath) {
				Destroy(gameObject);
			} else {
				currentHealth = maxHealth;
				RpcRespawn();
			}
        }
    }

    void OnChangeHealth (int health){
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

	[ClientRpc]
	void RpcRespawn(){
		if (isLocalPlayer){
			// move back to zero location
			transform.position = Vector3.zero;
		}
	}
}