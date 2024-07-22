using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerWorldModelHandler : NetworkBehaviour
{
  [SerializeField]private GameObject worldModel;

  public override void OnNetworkSpawn(){
    DeactivateOwnerWorldmodel();
    base.OnNetworkSpawn();
  }

  public void DeactivateOwnerWorldmodel(){
    if(IsOwner){
      worldModel.SetActive(false);
    }
  }
}
