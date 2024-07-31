using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerDeathMessageHandler : NetworkBehaviour
{
  [SerializeField]private TextMeshProUGUI deathMessageText;
  [SerializeField]private float maxDeathMessageTime = 3f;
  private float deathMessageTime = 0f;

  void Awake(){
  }

  void Update(){
    if(IsOwner){
      deathMessageTime -= Time.deltaTime;

      if(deathMessageTime <= 0f){
        Debug.Log("3");
        deathMessageText.gameObject.SetActive(false);
      } else {
        Debug.Log(deathMessageTime);
      }
    }
  }

  [Rpc(SendTo.Owner)]
  public void ShowDeathMessageRpc(){
    Debug.Log("2");
    deathMessageText.text = "ELIMINATED <color=#FF7171>" + GetComponent<PlayerRespawnHandler>().attackerName.Value.stringValue + "</color>";
    deathMessageTime = maxDeathMessageTime;
    deathMessageText.gameObject.SetActive(true);
  }
}
