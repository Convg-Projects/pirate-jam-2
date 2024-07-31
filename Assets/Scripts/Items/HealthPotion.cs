using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthPotion : NetworkBehaviour
{
  [SerializeField]private GameObject pickupSoundPrefab;

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){return;}
    if(col.gameObject.GetComponent<PlayerId>() == null){return;}
    col.gameObject.GetComponent<Health>().ResetHealthRpc();
    DestroyPotionRpc();
  }

  [Rpc(SendTo.Everyone)]
  public void DestroyPotionRpc(){
    GameObject audioInstance = GameObject.Instantiate(pickupSoundPrefab);
    audioInstance.transform.position = transform.position;
    Destroy(audioInstance, 3f);

    GetComponent<NetworkObject>().Despawn();
  }
}
