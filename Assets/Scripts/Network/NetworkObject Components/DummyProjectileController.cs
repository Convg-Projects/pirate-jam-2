using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DummyProjectileController : MonoBehaviour
{
  public ulong ownerId;

  void Start(){
    Destroy(gameObject, 4f);
  }

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){
      Destroy(gameObject);
      return;
    }
    if(col.gameObject.GetComponent<NetworkObject>().OwnerClientId != ownerId){
      Destroy(gameObject);
    }
  }
}
