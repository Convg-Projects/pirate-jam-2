using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeathZoneManager : NetworkBehaviour
{
  public static DeathZoneManager Instance { get; private set; }
  [SerializeField]private float maxDistance;

  void OnNetworkSpawn(){
    if (Instance != null){
      Debug.Log("There is already a Death Zone Manager instance. Destroying component!");
      Destroy(this);
    } else {
      Instance = this;
    }
    maxDistance -= transform.position.y;

    base.OnNetworkSpawn();
  }

  void Update(){
    if(IsOwner){
      transform.position = new Vector3(transform.position.x, transform.position.y + maxDistance / (ScoreManager.Instance.gameDuration / Time.deltaTime), transform.position.z);
    }
  }
}
