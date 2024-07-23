using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileController : NetworkBehaviour
{
  public bool isBlank = false;
  [SerializeField]private GameObject renderer;
  public int Damage = 10;
  private float lifeLeft = 4f;
  private bool dead = false;
  public NetworkVariable<bool> validHit = new NetworkVariable<bool>(false);
  private NetworkObject networkObject;

  public override void OnNetworkSpawn(){
    if(!isBlank){
      networkObject = GetComponent<NetworkObject>();
    }
    if(IsOwner && !IsHost){
      renderer.SetActive(false);
    }
    base.OnNetworkSpawn();
  }

  void OnCollisionEnter(Collision col){
    if(!isBlank){
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
    if(isBlank){
      Destroy(gameObject);
    }
  }

  void Update(){
    lifeLeft -= Time.deltaTime;
    if(lifeLeft <= 0f && !dead){
      dead = true;
      if(isBlank){
        Destroy(gameObject);
      } else {
        DestroyProjectileRpc();
      }
    }
  }

  [Rpc(SendTo.Server)]
  void DestroyProjectileRpc(){
    GetComponent<NetworkObject>().Despawn();
  }
}
