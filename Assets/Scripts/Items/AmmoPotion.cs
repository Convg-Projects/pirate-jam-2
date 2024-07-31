using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AmmoPotion : NetworkBehaviour
{
  [SerializeField]private GameObject pickupSoundPrefab;

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){return;}
    if(col.gameObject.GetComponent<PlayerShooting>() == null){return;}
    col.gameObject.GetComponent<PlayerShooting>().ResetAmmo();
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
