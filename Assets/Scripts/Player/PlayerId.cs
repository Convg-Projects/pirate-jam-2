using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class PlayerId : NetworkBehaviour
{
  public NetworkVariable<customString> playerName = new NetworkVariable<customString>(
    new customString {
      stringValue = "name"
    }
  );

  public struct customString : INetworkSerializable {
    public string stringValue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
      serializer.SerializeValue(ref stringValue);
    }
  }

  public string playerNameString = "not the player name";

  public override void OnNetworkSpawn(){
    if(IsOwner){
      Lobby joinedLobby = LobbyManager.Instance.GetJoinedLobby();
      foreach (Player player in joinedLobby.Players) {
        if (player.Id == AuthenticationService.Instance.PlayerId) {
          SetNameRpc(LobbyManager.Instance.playerName);
        }
      }

      base.OnNetworkSpawn();
    }
  }

  [Rpc(SendTo.Server)]
  public void SetNameRpc(string newName){
    playerName.Value = new customString{ stringValue = newName };
  }

  void Update(){
    playerNameString = playerName.Value.stringValue;
  }
}
