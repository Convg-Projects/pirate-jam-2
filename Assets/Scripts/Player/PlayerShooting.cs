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
  [SerializeField]private GameObject dummyProjectilePrefab;
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
      SpawnFakeBulletRpc(fireTransform.position, fireTransform.forward, NetworkManager.Singleton.LocalClientId);

      SpawnBulletRpc(fireTransform.position, fireTransform.forward, NetworkManager.Singleton.LocalClientId);

      firingCooldown = maxFiringCooldown;
    }
  }

  [Rpc(SendTo.Everyone)]
  private void SpawnFakeBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection, ulong ownerId){
    if(!IsHost){ //Spawn a fake bullet to make clients happy
      var localInstance = Instantiate(NetworkManager.GetNetworkPrefabOverride(dummyProjectilePrefab));
      localInstance.transform.position = spawnPosition;

      localInstance.GetComponent<DummyProjectileController>().ownerId = ownerId;

      Rigidbody localInstanceRB = localInstance.GetComponent<Rigidbody>();
      localInstanceRB.AddForce(bulletForce * spawnDirection, ForceMode.Impulse);
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
