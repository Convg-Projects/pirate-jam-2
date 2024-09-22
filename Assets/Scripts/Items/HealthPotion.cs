using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthPotion : Potion
{
  public override void OnHit(PlayerPotionHandler potionHandler){
    if(potionHandler.IsOwner){
      potionHandler.gameObject.GetComponent<Health>().ResetHealthRpc();
    }

    base.OnHit(potionHandler);
  }
}
