using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPotionHandler : NetworkBehaviour
{
  void OnTriggerEnter(Collider col){
    if(col.gameObject.GetComponent<Potion>() != null){
      col.gameObject.GetComponent<Potion>().OnHit(this);
    }
  }
}
