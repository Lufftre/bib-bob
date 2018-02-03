using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class PlayerLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(
		NetworkManager manger, GameObject lobbyPlayer, GameObject gamePlayer) {
			LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
			HeroController hc = gamePlayer.GetComponent<HeroController>();

			hc.playerName = lPlayer.playerName;
			hc.playerColor = lPlayer.playerColor;

		}

}
