﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public GameObject player;
    
    void OnCollisionEnter2D(Collision2D collision){
		var hit = collision.gameObject;
		var health = hit.GetComponent<Health>();
		if (health != null){
			health.TakeDamage(100);
		}
        Destroy(gameObject);
    }
}