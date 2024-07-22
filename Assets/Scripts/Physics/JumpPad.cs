using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
  [SerializeField]private float effectRange = 0.5f;
  [SerializeField]private float boostForce = 50f;

  void Update(){
    Collider[] hitColliders = new Collider[20];
    int numColliders = Physics.OverlapSphereNonAlloc(transform.position, effectRange, hitColliders);
    for(int i = 0; i < numColliders; ++i){
      if(hitColliders[i].transform.tag == "Player"){
        hitColliders[i].gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0f, boostForce, 0f));
      }
    }
  }
}
