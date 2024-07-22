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
      SpawnBulletRpc(fireTransform.position, fireTransform.forward);

      firingCooldown = maxFiringCooldown;
    }
  }

  [Rpc(SendTo.Server)]
  private void SpawnBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection){
    var instance = Instantiate(NetworkManager.GetNetworkPrefabOverride(projectilePrefab));
    var instanceNetworkObject = instance.GetComponent<NetworkObject>();
    instanceNetworkObject.Spawn();

    instance.transform.position = spawnPosition;

    Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
    instanceRB.velocity = GetComponent<Rigidbody>().velocity;
    instanceRB.AddForce(bulletForce * spawnDirection.normalized, ForceMode.Impulse);
  }
}
