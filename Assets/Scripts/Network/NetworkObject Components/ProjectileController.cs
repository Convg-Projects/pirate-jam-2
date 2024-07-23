using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileController : NetworkBehaviour
{
  public int Damage = 10;
  private float lifeLeft = 4f;
  private bool dead = false;
  public NetworkVariable<bool> validHit = new NetworkVariable<bool>(false);
  private NetworkObject networkObject;

  public override void OnNetworkSpawn(){
    networkObject = GetComponent<NetworkObject>();
    base.OnNetworkSpawn();
  }

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<Health>() != null && networkObject.OwnerClientId != col.gameObject.GetComponent<NetworkObject>().OwnerClientId){
      Health healthController = col.gameObject.GetComponent<Health>();
      healthController.ChangeHealthServerRpc(-Damage, networkObject.OwnerClientId);
      DestroyProjectileRpc();
      return;
    }
    if(col.gameObject.GetComponent<Health>() == null){
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

  [Rpc(SendTo.Server)]
  void DestroyProjectileRpc(){
    GetComponent<NetworkObject>().Despawn();
  }
}
