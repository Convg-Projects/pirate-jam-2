using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AmmoPotion : NetworkBehaviour
{
  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){return;}
    if(col.gameObject.GetComponent<PlayerShooting>() == null){return;}
    col.gameObject.GetComponent<PlayerShooting>().ResetAmmo();
    DestroyPotionRpc();
  }

  [Rpc(SendTo.Server)]
  public void DestroyPotionRpc(){
    GetComponent<NetworkObject>().Despawn();
  }
}
