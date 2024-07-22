using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
  [SerializeField]private int maxHealth = 100;
  [SerializeField]private bool destroyOnDeath = false;
  [SerializeField]private bool isPlayer;
  NetworkVariable<int> health = new NetworkVariable<int>();
  public NetworkVariable<bool> dead = new NetworkVariable<bool>();

  public override void OnNetworkSpawn(){
    ChangeHealthServerRpc(maxHealth);
    health.OnValueChanged += OnHealthChanged;

    base.OnNetworkSpawn();
  }

  public void OnHealthChanged(int previous, int current){
    if(current <= 0 && !dead.Value){
      HandleDeathRpc();
      SetDeadRpc(true);
    }
  }

  [Rpc(SendTo.Server)]
  public void ChangeHealthServerRpc(int amount){
      health.Value += amount;
  }

  [Rpc(SendTo.Server)]
  public void HandleDeathRpc(){
    if(destroyOnDeath){
      GetComponent<NetworkObject>().Despawn();
    }
  }

  [Rpc(SendTo.Server)]
  public void SetDeadRpc(bool isDead){
    Debug.Log("Changing deadness");
    if(isDead && isPlayer){
      PlayerRespawnHandler respawnHandler = GetComponent<PlayerRespawnHandler>();
      respawnHandler.respawnTime = respawnHandler.maxRespawnTime;
    }
    dead.Value = isDead;
  }
}
