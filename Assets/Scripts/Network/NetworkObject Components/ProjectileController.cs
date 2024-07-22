using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileController : NetworkBehaviour
{
  public int Damage = 10;
  //[HideInInspector]public NetworkObject owner;
  private float lifeLeft = 4f;
  private bool dead = false;

  void OnCollisionEnter(Collision col){
    if(col.transform.tag == "Player"){
      bool doDamage = OnCollisionRpc(col.gameObject.GetComponent<NetworkObject>().OwnerClientId);

      if(col.gameObject.GetComponent<Health>() != null){
        Debug.Log("ID " + col.gameObject.GetComponent<NetworkObject>().OwnerClientId + " has health")
        Health healthController = col.gameObject.GetComponent<Health>();
        healthController.ChangeHealthServerRpc(-Damage);
        DestroyProjectileRpc();
      }
    }
  }

  [Rpc(SendTo.Server)]
  void OnCollisionRpc(ulong clientId){
    if(!IsSpawned){return false;}
    if(!IsOwner){return false;}
    if(GetComponent<NetworkObject>().OwnerClientId == clientId){
      Debug.Log("i the owner WOWOWOWOW FUCK YOU");
      return false;
    }

    return true;
  }

  void Update(){
    lifeLeft -= Time.deltaTime;
    if(lifeLeft <= 0f && !dead){
      dead = true;
      DestroyProjectileRpc();
    }
  }

  /*[Rpc(SendTo.Server)]
  public void SetShooterRpc(){

  }*/

  [Rpc(SendTo.Server)]
  void DestroyProjectileRpc(){
    GetComponent<NetworkObject>().Despawn();
  }
}
