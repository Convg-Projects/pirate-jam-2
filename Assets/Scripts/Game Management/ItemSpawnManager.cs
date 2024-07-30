using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSpawnManager : NetworkBehaviour
{
  public static ItemSpawnManager Instance { get; private set; }

  [SerializeField]private Transform[] spawnPositions;
  [SerializeField]private GameObject[] items;
  [SerializeField]private float maxTimeBetweenSpawns = 10f;
  [SerializeField]private float timeRange = 5f;

  private float timeBetweenSpawns;

  void Awake (){
    timeBetweenSpawns = 99999f;
  }

  public override void OnNetworkSpawn(){
    if (Instance != null){
      Debug.Log("There is already an Item Spawn Manager instance. Destroying component!");
      Destroy(this);
    } else {
      Instance = this;
    }

    if(IsHost){
     timeBetweenSpawns = maxTimeBetweenSpawns + Random.Range(-timeRange/2, timeRange/2);
    }

    base.OnNetworkSpawn();
  }

  void Update(){
    if(IsHost){
      DoSpawns();
    }
  }

  void DoSpawns(){
    timeBetweenSpawns -= Time.deltaTime;
    Debug.Log(timeBetweenSpawns);

    if(timeBetweenSpawns <= 0f){
      List<Transform> availableSpawns = new List<Transform>();
      foreach(Transform T in spawnPositions){
        if(T.childCount == 0){
          availableSpawns.Add(T);
        }
      }

      if(availableSpawns.Count == 0){return;}
      GameObject instance = GameObject.Instantiate(items[Random.Range(0, items.Length)]);
      Transform spawnPoint = availableSpawns[Random.Range(0, availableSpawns.Count)];

      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.Spawn();

      instance.transform.position = spawnPoint.position;
      instance.transform.parent = spawnPoint;

      timeBetweenSpawns = maxTimeBetweenSpawns + Random.Range(-timeRange/2, timeRange/2);
    }
  }
}
