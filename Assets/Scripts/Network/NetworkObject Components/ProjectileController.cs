using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileController : NetworkBehaviour
{
  [SerializeField]private GameObject DestructionAudio;
  [SerializeField]private GameObject impactParticlePrefab;
  [SerializeField]private GameObject renderer;
  [SerializeField]private float blastRadius = 1.5f;
  [SerializeField]private GameObject unparentedAudio;
  [HideInInspector]public int damage = 10;
  private float lifeLeft = 4f;
  private bool dead = false;
  public NetworkVariable<bool> validHit = new NetworkVariable<bool>(false);
  private NetworkObject networkObject;
  bool hasHit = false;

  public override void OnNetworkSpawn(){
    if(!IsHost){
      renderer.SetActive(false);
      unparentedAudio = null;
    }
    if(unparentedAudio != null){
      unparentedAudio.transform.parent = null;
      Destroy(unparentedAudio, 1f);
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
            healthController.ChangeHealthServerRpc(-damage, networkObject.OwnerClientId);
            hasHit = true;
            break;
          }
          if(hitCollider.transform.parent.gameObject.GetComponent<NetworkObject>() == null){
            hasHit = true;
          }
        }
      }
      if(hasHit){
        GameObject audioInstance = GameObject.Instantiate(DestructionAudio);
        audioInstance.transform.position = transform.position;
        audioInstance.transform.parent = transform.parent;
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
    Destroy(particleInstance, 5f);

    particleInstance.transform.position = particleSpawnPosition;

    if(IsHost){
      GetComponent<NetworkObject>().Despawn();
    }
  }
}
