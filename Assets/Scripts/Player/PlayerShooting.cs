using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerShooting : NetworkBehaviour
{
  [SerializeField]private Transform fireTransform;
  public WeaponScriptableObject[] weaponDataObjects;
  [SerializeField]private TextMeshProUGUI ammoText;

  public NetworkVariable<int> weapon = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  [HideInInspector]public int currentAmmo;

  private float firingCooldown;
  private NetworkObject networkObject;

  public override void OnNetworkSpawn(){
    networkObject = GetComponent<NetworkObject>();
    if(IsOwner){
      weapon.Value = WeaponManager.Instance.weapon;
      currentAmmo = weaponDataObjects[weapon.Value].maxAmmo;
    }

    base.OnNetworkSpawn();
  }

  void Update(){
    if(IsOwner){
      CheckFiring();
      ammoText.text = currentAmmo + "/" + weaponDataObjects[weapon.Value].maxAmmo;
    }
  }

  void CheckFiring(){
    firingCooldown -= Time.deltaTime;
    if(currentAmmo <= 0){return;}

    if (weaponDataObjects[weapon.Value].auto && Input.GetMouseButton(0) && firingCooldown <= 0f){
      for (int i = 0; i < weaponDataObjects[weapon.Value].numberOfShots; ++i){
        FireBullet();
      }
    }
    if (!weaponDataObjects[weapon.Value].auto && Input.GetMouseButtonDown(0) && firingCooldown <= 0f){
      for (int i = 0; i < weaponDataObjects[weapon.Value].numberOfShots; ++i){
        FireBullet();
      }
    }
  }

  private void FireBullet(){
    --currentAmmo;
    Vector3 bulletSpawnDirection = fireTransform.forward;
    Vector3 spreadVector = new Vector3(Random.insideUnitCircle.x * weaponDataObjects[weapon.Value].spread, Random.insideUnitCircle.y * weaponDataObjects[weapon.Value].spread, 0f);
    bulletSpawnDirection += transform.TransformDirection(spreadVector);

    if(weaponDataObjects[weapon.Value].hitscan){
      RaycastHit hit;
      Physics.Raycast(fireTransform.position, bulletSpawnDirection, out hit, 50f);
      Debug.DrawRay(fireTransform.position, bulletSpawnDirection * 50f, Color.cyan, 5f, true);

      if(hit.collider == null){return;}

      GameObject particleInstance = GameObject.Instantiate(weaponDataObjects[weapon.Value].hitscanHitParticlePrefab);
      Destroy(particleInstance, 5f);
      particleInstance.transform.position = hit.point;

      if(hit.collider.transform.parent == null){return;}

      if(hit.collider.transform.parent.gameObject.GetComponent<Health>() != null && networkObject.OwnerClientId != hit.collider.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId){
        Health healthController = hit.collider.transform.parent.gameObject.GetComponent<Health>();
        healthController.ChangeHealthServerRpc(-weaponDataObjects[weapon.Value].damage, networkObject.OwnerClientId);
      }

    } else {
      SpawnFakeBulletRpc(fireTransform.position, bulletSpawnDirection, NetworkManager.Singleton.LocalClientId);

      SpawnBulletRpc(fireTransform.position, bulletSpawnDirection, weaponDataObjects[weapon.Value].damage, NetworkManager.Singleton.LocalClientId);
    }

    firingCooldown = weaponDataObjects[weapon.Value].maxFiringCooldown;
  }

  public void ResetAmmo(){
    currentAmmo = weaponDataObjects[weapon.Value].maxAmmo;
  }

  [Rpc(SendTo.Everyone)]
  private void SpawnFakeBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection, ulong ownerId){
    if(!IsHost){ //Spawn a fake bullet to make clients happy
      var localInstance = Instantiate(NetworkManager.GetNetworkPrefabOverride(weaponDataObjects[weapon.Value].dummyProjectilePrefab));
      localInstance.transform.position = spawnPosition;
      localInstance.transform.forward = spawnDirection;

      localInstance.GetComponent<DummyProjectileController>().ownerId = ownerId;

      Rigidbody localInstanceRB = localInstance.GetComponent<Rigidbody>();
      localInstanceRB.AddForce(weaponDataObjects[weapon.Value].bulletForce * spawnDirection, ForceMode.Impulse);
    }
  }

  [Rpc(SendTo.Server)]
  private void SpawnBulletRpc(Vector3 spawnPosition, Vector3 spawnDirection, int bulletDamage, ulong ownerId){
    var instance = Instantiate(NetworkManager.GetNetworkPrefabOverride(weaponDataObjects[weapon.Value].projectilePrefab));
    instance.transform.position = spawnPosition;
    instance.transform.forward = spawnDirection;

    var instanceNetworkObject = instance.GetComponent<NetworkObject>();
    instanceNetworkObject.SpawnWithOwnership(ownerId);

    instance.GetComponent<ProjectileController>().damage = bulletDamage;

    Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
    instanceRB.AddForce(weaponDataObjects[weapon.Value].bulletForce * spawnDirection.normalized, ForceMode.Impulse);
  }
}
