using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerRespawnHandler : NetworkBehaviour
{
  [SerializeField]private GameObject deathCanvas;
  [SerializeField]private GameObject gameCanvas;
  [SerializeField]private GameObject renderer;
  [SerializeField]private TextMeshProUGUI countdownText;
  public float maxRespawnTime = 10f;
  [HideInInspector]public float respawnTime;

  void Update(){
    Debug.Log(GetComponent<Health>().dead.Value + respawnTime.ToString());
    if(GetComponent<Health>().dead.Value){
      Debug.Log(respawnTime);
      respawnTime -= Time.deltaTime;
      Debug.Log(respawnTime);
      countdownText.text = respawnTime + "s";

      if(respawnTime <= 0f){
        GetComponent<Health>().SetDeadRpc(false);
      }

      ChangeActiveStatus(false);
    } else {
      ChangeActiveStatus(true);
    }
  }

  public void ChangeActiveStatus(bool active){
    if(active){
      GetComponent<PlayerMovement>().enabled = true;
      GetComponent<PlayerShooting>().enabled = true;
      GetComponent<Health>().enabled = true;
      GetComponent<PlayerWorldModelHandler>().DeactivateOwnerWorldmodel();

      if(IsOwner){
        deathCanvas.SetActive(false);
        gameCanvas.SetActive(true);
      }
    } else {
      GetComponent<PlayerMovement>().enabled = false;
      GetComponent<PlayerShooting>().enabled = false;
      GetComponent<Health>().enabled = false;
      renderer.SetActive(false);
      if(IsOwner){
        deathCanvas.SetActive(true);
        gameCanvas.SetActive(false);
      }
    }
  }
}
