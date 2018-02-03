using UnityEngine;
using System.Collections;
using System.Xml;
using UnityEngine.Networking;
public class MapEditor : NetworkBehaviour {

	Texture2D map;
	public ColorToPrefab[] colorMappings;
	// public string url = "http://klingstael.se/bib-bob/levels/Level1.png";
	public string levels = "http://klingstael.se/bib-bob/levels/levels.xml";

	// Use this for initialization
	IEnumerator Start () {
        // Start a download of the given URL
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(levels);
		XmlNodeList nodes = xmlDoc.SelectNodes("levels/level");
		string levelText = nodes[Random.Range(0, nodes.Count)].SelectSingleNode("url").InnerText;
		string url = "http://klingstael.se/bib-bob/levels/" + levelText;
		print(levelText);
/* 		foreach(XmlNode node in xmlDoc.SelectNodes("levels/level")){
			// print(node.SelectSingleNode("url").InnerText);
			url = "http://klingstael.se/bib-bob/levels/" + node.SelectSingleNode("url").InnerText;
		} */

        using (WWW www = new WWW(url))
        {
            // Wait for download to complete
            yield return www;

            // assign texture
            map = www.texture;
        }
		GenerateLevel();

		MovePlayers();
		
	}

	void MovePlayers(){
		// Vector3 spawnPoint = Vector3.zero;
		NetworkStartPosition[] spawnPoints = FindObjectsOfType<NetworkStartPosition>();

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players){
			Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			player.transform.position = spawnPoint;
			
		}

	}

	void GenerateLevel() {
		for(int x=0; x<map.width; x++) {
			for(int y=0; y<map.height; y++) {
				GenerateTile(x, y);
			}
		}
	}

	void GenerateTile(int x, int y) {
		Color pixelColor = map.GetPixel(x, y);

		if (pixelColor.a == 0) {
			return;
		}

		foreach (ColorToPrefab colorMapping in colorMappings) {
			if (colorMapping.color.Equals(pixelColor)){
				Vector2 position = new Vector2(x - map.width/2, y-map.height/2);
				// print(colorMapping.prefab);
				var mapBlock = (GameObject)Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
				NetworkServer.Spawn(mapBlock);
			}
		}
	}


}
