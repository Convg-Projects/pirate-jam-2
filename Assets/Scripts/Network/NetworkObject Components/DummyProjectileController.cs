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
    if(col.transform.parent == null){
      Destroy(gameObject);
    } else {
      if(col.transform.parent.gameObject.GetComponent<NetworkObject>() == null){
        Destroy(gameObject);
        return;
      }
      if(col.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId != ownerId){
        Destroy(gameObject);
      }
    }
  }
}
