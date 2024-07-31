using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class Health : NetworkBehaviour
{
  [SerializeField]private int maxHealth = 100;
  [SerializeField]private bool destroyOnDeath = false;
  [SerializeField]private bool isPlayer = false;
  [SerializeField]private bool displayHealth = false;
  [SerializeField]private Slider healthSlider;
  [SerializeField]private GameObject hitAudioPrefab;

  NetworkVariable<int> health = new NetworkVariable<int>();
  public NetworkVariable<bool> dead = new NetworkVariable<bool>();

  NetworkVariable<ulong> lastAttackerId = new NetworkVariable<ulong>();

  public override void OnNetworkSpawn(){
    health.OnValueChanged += OnHealthChanged;
    if(IsOwner){
      ChangeHealthServerRpc(maxHealth, 0);
      if(displayHealth){
        healthSlider.value = 1;
      }
    }

    base.OnNetworkSpawn();
  }

  public void OnHealthChanged(int previous, int current){
    if(current < previous){
      GameObject audioInstance = GameObject.Instantiate(hitAudioPrefab);
      audioInstance.transform.position = transform.position;
      Destroy(audioInstance, 0.5f);
    }

    if(!IsOwner){return;}

    if(displayHealth){
      healthSlider.value = (float) health.Value / (float) maxHealth;
    }
    if(current <= 0 && !dead.Value){
      HandleDeathRpc();
    }
  }

  [Rpc(SendTo.Server)]
  public void ChangeHealthServerRpc(int amount, ulong attackerId){
    lastAttackerId.Value = attackerId;
    health.Value += amount;
    if(health.Value + amount <= 0 && !dead.Value){
      HandleDeathRpc();
    }
  }

  [Rpc(SendTo.Server)]
  public void ResetHealthRpc(){
    health.Value = maxHealth;
  }

  [Rpc(SendTo.Server)]
  public void HandleDeathRpc(){
    if(dead.Value){return;}
    if(destroyOnDeath){
      GetComponent<NetworkObject>().Despawn();
    }
    if(lastAttackerId.Value < (ulong) 9999 && isPlayer){
      GameObject attacker = NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject;
      attacker.GetComponent<PlayerScore>().UpdateScore(1, GetComponent<PlayerId>().playerName.Value.stringValue);
    }
    SetDeadRpc(true);
  }

  [Rpc(SendTo.Server)]
  public void SetDeadRpc(bool isDead){
    if(isDead && isPlayer){
      PlayerRespawnHandler respawnHandler = GetComponent<PlayerRespawnHandler>();
      if(lastAttackerId.Value < (ulong) 9999){
        respawnHandler.attackerName.Value = new PlayerRespawnHandler.customString{ stringValue = NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject.GetComponent<PlayerId>().playerName.Value.stringValue};
        NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject.GetComponent<PlayerDeathMessageHandler>().ShowDeathMessage(NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject.GetComponent<PlayerId>().playerName.Value.stringValue);
      } else {
        respawnHandler.attackerName.Value = new PlayerRespawnHandler.customString{ stringValue = "The Abyss" };
      }
      respawnHandler.ResetTimerRpc();

    }
    if(!isDead){
      health.Value = maxHealth;
    }
    dead.Value = isDead;
  }
}
