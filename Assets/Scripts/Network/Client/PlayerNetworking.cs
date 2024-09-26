using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerNetworking : NetworkBehaviour
{
  public void Disconnect(){
    ServerRpcParams serverRpcParams = default;
    ulong clientId = NetworkManager.Singleton.LocalClientId;

    LobbyManager.Instance.LeaveLobby();
    DisconnectSelfRPC(clientId);
  }

  [Rpc(SendTo.Server)]
  public void DisconnectSelfRPC(ulong clientId){
    Debug.Log("Disconnecting client " + clientId);
    if(NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)){
      NetworkManager.Singleton.DisconnectClient(clientId);
      NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.Despawn();
    }
  }

  public override void OnNetworkDespawn(){
    if(!IsOwner){return;}

    NetworkManager.Singleton.Shutdown();

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
    SceneManager.LoadScene(0);
  }

  void OnApplicationQuit() {
    Debug.Log("QUIT");

    LobbyManager.Instance.LeaveLobby();
  }
}
