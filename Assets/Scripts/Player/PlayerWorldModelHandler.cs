using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerWorldModelHandler : NetworkBehaviour
{
  [SerializeField]private GameObject[] worldModels;

  public override void OnNetworkSpawn(){
    DeactivateOwnerWorldmodel();
    base.OnNetworkSpawn();
  }

  public void DeactivateOwnerWorldmodel(){
    if(IsOwner){
      foreach(GameObject G in worldModels){
        G.SetActive(false);
      }
    }
  }
}
