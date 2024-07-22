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
  [SerializeField]private GameObject colliderParent;
  public float maxRespawnTime = 10f;
  [HideInInspector]public float respawnTime;

  void Update(){
    //Debug.Log(GetComponent<Health>().dead.Value + respawnTime.ToString());
    if(GetComponent<Health>().dead.Value){
      if(IsOwner){
        Debug.Log(respawnTime);
        respawnTime -= Time.deltaTime;
        Debug.Log(respawnTime);
        countdownText.text = respawnTime + "s";

        if(respawnTime <= 0f){
          GetComponent<Health>().SetDeadRpc(false);
          transform.position = Vector3.zero;
        }
      }

      ChangeActiveStatus(false);
    } else {
      ChangeActiveStatus(true);
    }
  }

  public void ChangeActiveStatus(bool active){
    if(active){
      colliderParent.SetActive(true);
      GetComponent<Rigidbody>().isKinematic = false;
      GetComponent<PlayerMovement>().enabled = true;
      GetComponent<PlayerShooting>().enabled = true;
      GetComponent<Health>().enabled = true;
      renderer.SetActive(true);
      GetComponent<PlayerWorldModelHandler>().DeactivateOwnerWorldmodel();

      if(IsOwner){
        deathCanvas.SetActive(false);
        gameCanvas.SetActive(true);
      }
    } else {
      colliderParent.SetActive(false);
      GetComponent<Rigidbody>().isKinematic = true;
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
