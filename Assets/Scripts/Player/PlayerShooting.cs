using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
  [SerializeField]private Transform fireTransform;
  [SerializeField]private float maxFiringCooldown = 0.5f;
  [SerializeField]private float bulletForce = 100f;
  [SerializeField]private GameObject projectilePrefab;
  private float firingCooldown;

  public override void OnNetworkSpawn(){
    base.OnNetworkSpawn();
  }

  void Update(){
    if(IsOwner){
      Shoot();
    }
  }

  void Shoot(){
    firingCooldown -= Time.deltaTime;

    if (Input.GetMouseButton(0) && firingCooldown <= 0f){
      if(!IsHost){ //Spawn a fake bullet to make the drooling clients happy
        var localInstance = Instantiate(NetworkManager.GetNetworkPrefabOverride(projectilePrefab));
        localInstance.transform.position = fireTransform.position;

        localInstance.GetComponent<ProjectileController>().isBlank = true;

        Rigidbody localInstanceRB = localInstance.GetComponent<Rigidbody>();
        localInstanceRB.AddForce(bulletForce * fireTransform.forward.normalized, ForceMode.Impulse);
      }

      SpawnBulletRpc(fireTransform.position, fireTransform.forward, NetworkManager.Singleton.LocalClientId);

      firingCooldown = maxFiringCooldown;
    }
  }

  [Rpc(SendTo.Server)]
  private void SpawnBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection, ulong ownerId){
    var instance = Instantiate(NetworkManager.GetNetworkPrefabOverride(projectilePrefab));
    instance.transform.position = spawnPosition;
    var instanceNetworkObject = instance.GetComponent<NetworkObject>();
    instanceNetworkObject.SpawnWithOwnership(ownerId);


    //instance.GetComponent<ProjectileController>().owner = NetworkManager.ConnectedClients[ownerId].PlayerObject;

    Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
    instanceRB.AddForce(bulletForce * spawnDirection.normalized, ForceMode.Impulse);
  }
}
