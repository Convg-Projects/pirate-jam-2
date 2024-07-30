using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using TMPro;

public class PlayerRespawnHandler : NetworkBehaviour
{
  [SerializeField]private GameObject deathCanvas;
  [SerializeField]private GameObject gameCanvas;
  [SerializeField]private GameObject[] rendererObjects;
  [SerializeField]private TextMeshProUGUI countdownText;
  [SerializeField]private TextMeshProUGUI killerText;
  [SerializeField]private GameObject colliderParent;

  public NetworkVariable<customString> attackerName = new NetworkVariable<customString>(
    new customString {
      stringValue = "name"
    }
  );

  public struct customString : INetworkSerializable {
    public string stringValue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
      serializer.SerializeValue(ref stringValue);
    }
  }

  public float maxRespawnTime = 10f;
  public NetworkVariable<float> respawnTime = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  private ulong lastAttackerId;
  private bool isDead = false;

  public override void OnNetworkSpawn(){
    if(!IsOwner){
      gameCanvas.SetActive(false);
    }
    if(IsHost){
      attackerName.Value = new customString{ stringValue = "name error" };
    }

    base.OnNetworkSpawn();
  }

  void Update(){
    if(GetComponent<Health>().dead.Value){
      if(IsServer){
      }
      if(IsOwner){
        respawnTime.Value -= Time.deltaTime;
        countdownText.text = Mathf.FloorToInt(respawnTime.Value) + "   seconds";

        if(respawnTime.Value <= 0f){
          GetComponent<Health>().SetDeadRpc(false);
          transform.position = PlayerSpawnManager.Instance.GetRandomSpawn();
        }
      }

      if(isDead){
        ChangeActiveStatus(false);
      }
    } else {
      if(!isDead){
        ChangeActiveStatus(true);
      }
    }
  }

  [Rpc(SendTo.Owner)]
  public void ResetTimerRpc(){
    respawnTime.Value = maxRespawnTime;
  }

  public void ChangeActiveStatus(bool active){
    if(active){
      isDead = true;
      colliderParent.SetActive(true);

      GetComponent<Rigidbody>().isKinematic = false;
      GetComponent<PlayerMovement>().enabled = true;
      GetComponent<PlayerShooting>().enabled = true;
      GetComponent<Health>().enabled = true;

      foreach(GameObject G in rendererObjects){
        G.SetActive(true);
      }
      GetComponent<PlayerWorldModelHandler>().DeactivateOwnerWorldmodel();

      if(IsOwner){
        deathCanvas.SetActive(false);
        gameCanvas.SetActive(true);
      }
    } else {
      isDead = false;
      colliderParent.SetActive(false);

      killerText.text = "<mark=#00000099> " + attackerName.Value.stringValue + " </mark>";

      GetComponent<Rigidbody>().isKinematic = true;
      GetComponent<PlayerMovement>().enabled = false;
      GetComponent<PlayerShooting>().enabled = false;
      GetComponent<Health>().enabled = false;

      foreach(GameObject G in rendererObjects){
        G.SetActive(false);
      }
      if(IsOwner){
        deathCanvas.SetActive(true);
        gameCanvas.SetActive(false);
      }
    }
  }
}
