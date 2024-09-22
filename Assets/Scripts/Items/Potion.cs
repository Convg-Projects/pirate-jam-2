using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Potion : NetworkBehaviour
{
  [SerializeField]private GameObject pickupSoundPrefab;

  public virtual void OnHit(PlayerPotionHandler potionHandler){
    DestroyPotionRpc();
  }

  [Rpc(SendTo.Everyone)]
  public void DestroyPotionRpc(){
    GameObject audioInstance = GameObject.Instantiate(pickupSoundPrefab);
    audioInstance.transform.position = transform.position;
    Destroy(audioInstance, 3f);

    if(IsHost){
      GetComponent<NetworkObject>().Despawn();
    }
  }
}
