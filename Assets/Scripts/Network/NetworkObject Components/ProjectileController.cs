using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileController : NetworkBehaviour
{
  public int Damage = 10;
  [HideInInspector]public NetworkObject owner;
  private float lifeLeft = 4f;
  private bool dead = false;

  void OnCollisionEnter(Collision col){
    if(!IsSpawned){return;}
    if(col.transform.parent.gameObject.GetComponent<NetworkObject>() == owner){return;}
    if(col.gameObject.GetComponent<Health>() != null){
      Health healthController = col.gameObject.GetComponent<Health>();
      healthController.ChangeHealthServerRpc(-Damage);
      DestroyProjectileRpc();
    }
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
