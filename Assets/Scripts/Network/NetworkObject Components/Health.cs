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

  [Header("Damage Effect")]
  [SerializeField]private float damageEffectDuration = 0.1f;
  [SerializeField]private SkinnedMeshRenderer[] meshRenderers;
  [SerializeField]private Material damageEffectMaterial;
  private List<Material> normalStateMaterials = new List<Material>();
  private float damageEffectTime;
  private bool damageEffectActive;

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

  void Update(){
    if(damageEffectActive){
      if(damageEffectTime > 0f){
        damageEffectTime -= Time.deltaTime;
      } else {
        ResetDamageEffect();

        damageEffectActive = false;
      }
    }
  }

  public void OnHealthChanged(int previous, int current){
    if(current < previous){
      ShowDamageEffect();

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

  public void ResetDamageEffect(){
    for(int i = 0; i < meshRenderers.Length; ++i){
      meshRenderers[i].material = normalStateMaterials[i];
    }
  }

  public void ShowDamageEffect(){
    if(damageEffectActive){return;}

    normalStateMaterials = new List<Material>(); // constantly making new lists instead of somehow resetting it might cause performance problems?
    for(int i = 0; i < meshRenderers.Length; ++i){
      normalStateMaterials.Add(meshRenderers[i].material);
      meshRenderers[i].material = damageEffectMaterial;
    }
    damageEffectTime = damageEffectDuration;
    damageEffectActive = true;
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
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject;
        respawnHandler.attackerName.Value = new PlayerRespawnHandler.customString{ stringValue = NetworkManager.Singleton.ConnectedClients[lastAttackerId.Value].PlayerObject.gameObject.GetComponent<PlayerId>().playerName.Value.stringValue};
        playerObject.GetComponent<PlayerDeathMessageHandler>().ShowDeathMessageRpc();
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
