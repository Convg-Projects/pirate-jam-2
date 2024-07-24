using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DummyProjectileController : MonoBehaviour
{
  public ulong ownerId;

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>().OwnerClientId != ownerId){
      Destroy(gameObject);
    }
  }
}
