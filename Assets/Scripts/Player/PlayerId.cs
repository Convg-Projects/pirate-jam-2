using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using TMPro;

public class PlayerId : NetworkBehaviour
{
  public NetworkVariable<customString> playerName = new NetworkVariable<customString>(
    new customString {
      stringValue = "name"
    }
  );
  [SerializeField]private TextMeshProUGUI nameText;

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

      if(IsOwner){
        nameText.gameObject.SetActive(false);
      }

      base.OnNetworkSpawn();
    }
  }

  [Rpc(SendTo.Everyone)]
  public void SetNameRpc(string newName){
    if(IsHost){
      playerName.Value = new customString{ stringValue = newName };
    }
    nameText.text = "<mark=#00000099> " + newName + " </mark>";
  }

  void Update(){
    if(!IsOwner){
      playerNameString = playerName.Value.stringValue;
      nameText.text = "<mark=#00000099> " + playerName.Value.stringValue + " </mark>";
      nameText.transform.parent.LookAt(NetworkManager.Singleton.LocalClient.PlayerObject.transform.position);
    }
  }
}
