using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Health : NetworkBehaviour
{
  [SerializeField]private int maxHealth = 100;
  [SerializeField]private bool destroyOnDeath = false;
  [SerializeField]private bool isPlayer = false;
  [SerializeField]private bool displayHealth = false;
  [SerializeField]private TextMeshProUGUI healthText;
  NetworkVariable<int> health = new NetworkVariable<int>();
  public NetworkVariable<bool> dead = new NetworkVariable<bool>();

  public override void OnNetworkSpawn(){
    ChangeHealthServerRpc(maxHealth);
    health.OnValueChanged += OnHealthChanged;
    if(displayHealth){
      healthText.text = maxHealth;
    }

    base.OnNetworkSpawn();
  }

  public void OnHealthChanged(int previous, int current){
    if(displayHealth){
      healthText.Text = health;
    }
    if(current <= 0 && !dead.Value){
      HandleDeathRpc();
    }
  }

  [Rpc(SendTo.Server)]
  public void ChangeHealthServerRpc(int amount){
      health.Value += amount;
  }

  [Rpc(SendTo.Server)]
  public void HandleDeathRpc(){
    if(destroyOnDeath && !dead.Value){
      GetComponent<NetworkObject>().Despawn();
    }
    SetDeadRpc(true);
  }

  [Rpc(SendTo.Server)]
  public void SetDeadRpc(bool isDead){
    Debug.Log("Changing deadness");
    if(isDead && isPlayer){
      PlayerRespawnHandler respawnHandler = GetComponent<PlayerRespawnHandler>();
      respawnHandler.ResetTimerRpc();
    }
    if(!isDead){
      health.Value = maxHealth;
    }
    dead.Value = isDead;
  }
}
