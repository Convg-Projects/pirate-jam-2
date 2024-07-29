using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DummyProjectileController : MonoBehaviour
{
  public ulong ownerId;

  void Start(){
    //Destroy(gameObject, 4f);
  }

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){
      Debug.Log("hit object");
      Destroy(gameObject);
      return;
    }
    if(col.gameObject.GetComponent<NetworkObject>().OwnerClientId != ownerId){
      Debug.Log("hit other player");
      Destroy(gameObject);
    }
    Debug.Log("hit local player");
  }
}
