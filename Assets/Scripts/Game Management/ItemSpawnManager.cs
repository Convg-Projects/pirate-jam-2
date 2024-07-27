using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
  [SerializeField]private Transform[] spawnPositions;
  [SerializeField]private GameObject[] items;
  [SerializeField]private float maxTimeBetweenSpawns = 10f;
  [SerializeField]private float timeRange = 5f;

  private float timeBetweenSpawns;

  void Awake (){
    timeBetweenSpawns = maxTimeBetweenSpawns + Random.Range(-timeRange/2, timeRange/2);
  }

  void Update(){
    DoSpawns();
  }

  void DoSpawns(){
    timeBetweenSpawns -= Time.deltaTime;

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

      instance.transform.position = spawnPoint.position;
      instance.transform.parent = spawnPoint;

      timeBetweenSpawns = maxTimeBetweenSpawns + Random.Range(-timeRange/2, timeRange/2);
    }
  }
}
