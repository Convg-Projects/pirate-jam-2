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
      OnCollisionRpc(col.gameObject.GetComponent<NetworkObject>().OwnerClientId);
    }
  }

  [Rpc(SendTo.Server)]
  void OnCollisionRpc(ulong clientId){
    if(!IsSpawned){return;}
    if(!IsOwner){return;}
    if(GetComponent<NetworkObject>().OwnerClientId == clientId){
      Debug.Log("i the owner WOWOWOWOW FUCK YOU");
      return;
    }

    if(NetworkManager.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<Health>() != null){
      Health healthController = NetworkManager.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<Health>();
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
