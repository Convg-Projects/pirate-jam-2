using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPauseHandler : NetworkBehaviour
{
  [SerializeField]private GameObject pauseCanvas;
  [SerializeField]private GameObject gameCanvas;
  private bool isPaused = false;

  void Update(){
    if(Input.GetKeyDown(KeyCode.Tab) && IsOwner){
      SwitchPausedState();
    }
  }

  public void SwitchPausedState(){
    if(isPaused){
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      pauseCanvas.SetActive(false);
      gameCanvas.SetActive(true);
      isPaused = false;
    } else {
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
      pauseCanvas.SetActive(true);
      gameCanvas.SetActive(false);
      isPaused = true;
    }
  }

  public void DisconnectPlayer(){
    GetComponent<PlayerNetworking>().DisconnectSelfRPC(GetComponent<NetworkObject>().OwnerClientId);
  }
}
