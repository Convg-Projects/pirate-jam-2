using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
  [SerializeField]private Transform fireTransform;
  [SerializeField]private WeaponScriptableObject weaponData;

  private float firingCooldown;
  private NetworkObject networkObject;

  public override void OnNetworkSpawn(){
    networkObject = GetComponent<NetworkObject>();

    base.OnNetworkSpawn();
  }

  void Update(){
    if(IsOwner){
      CheckFiring();
    }
  }

  void CheckFiring(){
    firingCooldown -= Time.deltaTime;

    if (weaponData.auto && Input.GetMouseButton(0) && firingCooldown <= 0f){
      for (int i = 0; i < weaponData.numberOfShots; ++i){
        FireBullet();
      }
    }
    if (!weaponData.auto && Input.GetMouseButtonDown(0) && firingCooldown <= 0f){
      for (int i = 0; i < weaponData.numberOfShots; ++i){
        FireBullet();
      }
    }
  }

  private void FireBullet(){
    Vector3 bulletSpawnDirection = fireTransform.forward;
    Vector3 spreadVector = new Vector3(Random.insideUnitCircle.x * weaponData.spread, Random.insideUnitCircle.y * weaponData.spread, 0f);
    bulletSpawnDirection += transform.TransformDirection(spreadVector);

    if(weaponData.hitscan){
      RaycastHit hit;
      Physics.Raycast(fireTransform.position, bulletSpawnDirection, out hit, 50f);

      for (int i = 0; i < weaponData.numberOfShots; ++i){
        if(hit.transform.parent == null){continue;}
        if(hit.transform.parent.gameObject.GetComponent<Health>() != null && networkObject.OwnerClientId != hit.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId){
          Health healthController = hit.transform.parent.gameObject.GetComponent<Health>();
          healthController.ChangeHealthServerRpc(-weaponData.damage, networkObject.OwnerClientId);
        }

        GameObject particleInstance = GameObject.Instantiate(weaponData.hitscanHitParticlePrefab);
        Destroy(particleInstance, 5f);

        particleInstance.transform.position = hit.transform.position;
      }
    } else {
      SpawnFakeBulletRpc(fireTransform.position, bulletSpawnDirection, NetworkManager.Singleton.LocalClientId);

      SpawnBulletRpc(fireTransform.position, bulletSpawnDirection, weaponData.damage, NetworkManager.Singleton.LocalClientId);
    }

    firingCooldown = weaponData.maxFiringCooldown;
  }

  [Rpc(SendTo.Everyone)]
  private void SpawnFakeBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection, ulong ownerId){
    if(!IsHost){ //Spawn a fake bullet to make clients happy
      var localInstance = Instantiate(NetworkManager.GetNetworkPrefabOverride(weaponData.dummyProjectilePrefab));
      localInstance.transform.position = spawnPosition;

      localInstance.GetComponent<DummyProjectileController>().ownerId = ownerId;

      Rigidbody localInstanceRB = localInstance.GetComponent<Rigidbody>();
      localInstanceRB.AddForce(weaponData.bulletForce * spawnDirection, ForceMode.Impulse);
    }
  }

  [Rpc(SendTo.Server)]
  private void SpawnBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection, int bulletDamage, ulong ownerId){
    var instance = Instantiate(NetworkManager.GetNetworkPrefabOverride(weaponData.projectilePrefab));
    instance.transform.position = spawnPosition;

    var instanceNetworkObject = instance.GetComponent<NetworkObject>();
    instanceNetworkObject.SpawnWithOwnership(ownerId);

    instance.GetComponent<ProjectileController>().damage = bulletDamage;

    Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
    instanceRB.AddForce(weaponData.bulletForce * spawnDirection.normalized, ForceMode.Impulse);
  }
}
