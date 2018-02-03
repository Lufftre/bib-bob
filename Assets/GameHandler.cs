using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class GameHandler : NetworkBehaviour {
	private GameObject[] getCount;
	public GameObject winText;
	public void HandlePlayerDeath(GameObject dyingObject){	
		getCount = GameObject.FindGameObjectsWithTag ("Player");
		int count = getCount.Length;
		if (count <= 2){
			foreach (GameObject g in getCount){
				if(g != dyingObject){
					Color color = g.GetComponent<HeroController>().playerColor;
					ShowWinner(g.name, color);
					break;
				}
			}
			
			StartCoroutine(ServerCountdownCoroutine());
		}
		Destroy(dyingObject);
		
		
	}

	public void ShowWinner(string name, Color color){
		print("winner " + name);
		var mapBlock = (GameObject)Instantiate(winText, Vector3.zero, Quaternion.identity);
		TextMesh tm = mapBlock.GetComponent<TextMesh>();
		tm.text = "Winner: " + name;
		tm.color = color;
		NetworkServer.Spawn(mapBlock);
	}

	public IEnumerator ServerCountdownCoroutine(){
		float remainingTime = 5;
		int floorTime = Mathf.FloorToInt(remainingTime);

		

		while (remainingTime > 0)
		{
			yield return null;

			remainingTime -= Time.deltaTime;
			int newFloorTime = Mathf.FloorToInt(remainingTime);
		}

		LobbyManager.s_Singleton.ServerReturnToLobby();
	}

}
