using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileController : NetworkBehaviour
{
  [SerializeField]private GameObject impactParticlePrefab;
  [SerializeField]private GameObject renderer;
  [SerializeField]private float blastRadius = 1.5f;
  public int Damage = 10;
  private float lifeLeft = 4f;
  private bool dead = false;
  public NetworkVariable<bool> validHit = new NetworkVariable<bool>(false);
  private NetworkObject networkObject;
  bool hasHit = false;

  public override void OnNetworkSpawn(){
    if(!IsHost){
      renderer.SetActive(false);
    }
    networkObject = GetComponent<NetworkObject>();
    base.OnNetworkSpawn();
  }

  void OnCollisionEnter(Collision col){
    if(!hasHit && IsHost){
      Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
      foreach (Collider hitCollider in hitColliders){
        if(hitCollider.transform.parent != null){
          if(hitCollider.transform.parent.gameObject.GetComponent<Health>() != null && networkObject.OwnerClientId != hitCollider.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId){
            Health healthController = hitCollider.transform.parent.gameObject.GetComponent<Health>();
            healthController.ChangeHealthServerRpc(-Damage, networkObject.OwnerClientId);
            hasHit = true;
            break;
          }
          if(hitCollider.transform.parent.gameObject.GetComponent<NetworkObject>() == null){
            hasHit = true;
          }
        }
      }
      if(hasHit){
        DestroyProjectileRpc(transform.position);
      }
    }
  }

  void Update(){
    lifeLeft -= Time.deltaTime;
    if(lifeLeft <= 0f && !dead){
      dead = true;
      DestroyProjectileRpc(transform.position);
    }
  }

  [Rpc(SendTo.Everyone)]
  void DestroyProjectileRpc(Vector3 particleSpawnPosition){
    GameObject particleInstance = GameObject.Instantiate(impactParticlePrefab);

    particleInstance.transform.position = particleSpawnPosition;

    if(IsHost){
      GetComponent<NetworkObject>().Despawn();
    }
  }
}
