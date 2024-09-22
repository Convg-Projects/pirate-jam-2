using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AmmoPotion : Potion
{
  public override void OnHit(PlayerPotionHandler potionHandler){
    if(potionHandler.IsOwner){
      potionHandler.gameObject.GetComponent<PlayerShooting>().ResetAmmo();
    }

    base.OnHit(potionHandler);
  }
}
