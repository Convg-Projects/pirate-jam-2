using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DummyProjectileController : MonoBehaviour
{
  [HideInInspector]public ulong ownerId;
  [SerializeField]private GameObject DestructionAudio;

  void Start(){
    //Destroy(gameObject, 4f);
  }

  void OnCollisionEnter(Collision col){
    if(col.gameObject.GetComponent<NetworkObject>() == null){
      GameObject audioInstance = GameObject.Instantiate(DestructionAudio);
      audioInstance.transform.position = transform.position;
      audioInstance.transform.parent = transform.parent;

      Destroy(gameObject);
      return;
    }
    if(col.gameObject.GetComponent<NetworkObject>().OwnerClientId != ownerId){
      GameObject audioInstance = GameObject.Instantiate(DestructionAudio);
      audioInstance.transform.position = transform.position;
      audioInstance.transform.parent = transform.parent;

      Destroy(gameObject);
    }
  }
}
