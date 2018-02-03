using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapBlock : MonoBehaviour {

	void Start(){
		Transform parent = GameObject.FindWithTag("Respawn").transform;
		transform.parent = parent;
	}
}
