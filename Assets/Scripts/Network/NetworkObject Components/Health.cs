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

  NetworkVariable<ulong> lastAttackerId = new NetworkVariable<ulong>();

  public override void OnNetworkSpawn(){
    if(IsOwner){
      ChangeHealthServerRpc(maxHealth, 0);
      health.OnValueChanged += OnHealthChanged;
      if(displayHealth){
        healthText.text = "Health: " + maxHealth.ToString();
      }
    }

    base.OnNetworkSpawn();
  }

  public void OnHealthChanged(int previous, int current){
    if(displayHealth){
      healthText.text = "Health: " + health.Value.ToString();
    }
    if(current <= 0 && !dead.Value){
      HandleDeathRpc();
    }
  }

  [Rpc(SendTo.Server)]
  public void ChangeHealthServerRpc(int amount, ulong attackerId){
      health.Value += amount;
      lastAttackerId.Value = attackerId;
  }

  [Rpc(SendTo.Server)]
  public void HandleDeathRpc(){
    if(dead.Value){return;}
    if(destroyOnDeath){
      GetComponent<NetworkObject>().Despawn();
    }
    if(lastAttackerId.Value < (ulong) 9999){
      NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject.GetComponent<PlayerScore>().UpdateScore(1);
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
