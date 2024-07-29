using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthPotion : NetworkBehaviour
{
  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){return;}
    if(col.gameObject.GetComponent<PlayerId>() == null){return;}
    col.gameObject.GetComponent<Health>().ResetHealthRpc();
    DestroyPotionRpc();
  }

  [Rpc(SendTo.Server)]
  public void DestroyPotionRpc(){
    GetComponent<NetworkObject>().Despawn();
  }
}
