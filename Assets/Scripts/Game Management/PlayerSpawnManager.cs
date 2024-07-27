using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
  public static PlayerSpawnManager Instance { get; private set; }

  public Transform[] spawnPositions;

  void Awake(){
    if(Instance != null){
      Debug.Log("There is already a Player Spawn Manager instance. Destroying component!");
    } else {
      Instance = this;
    }
  }

  public Vector3 GetRandomSpawn(){
    return spawnPositions[Random.Range(0, spawnPositions.Length)].position;
  }
}
