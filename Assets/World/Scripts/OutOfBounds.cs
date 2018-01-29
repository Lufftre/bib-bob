using UnityEngine;
using System.Collections;

public class OutOfBounds : MonoBehaviour {

    
    void OnCollisionEnter2D(Collision2D collision){
		var hit = collision.gameObject;
		var health = hit.GetComponent<Health>();
		if (health != null){
			health.TakeDamage(health.currentHealth);
		}
    }
}
