using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerNetworking : NetworkBehaviour
{
  public void Update(){
    if(Input.GetKeyDown(KeyCode.Escape) && IsOwner){
      ServerRpcParams serverRpcParams = default;
      ulong clientId = serverRpcParams.Receive.SenderClientId;

      DisconnectSelfRPC(clientId);
    }
  }

  [Rpc(SendTo.Server)]
  public void DisconnectSelfRPC(ulong clientId){
    Debug.Log("Disconnecting client " + clientId);
    if(NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)){
      NetworkManager.Singleton.DisconnectClient(clientId);
      NetworkManager.ConnectedClients[clientId].PlayerObject.Despawn();
    }
  }

  public override void OnNetworkDespawn(){
    if(IsOwner){
      SceneManager.LoadScene(0);
    }
  }
}
