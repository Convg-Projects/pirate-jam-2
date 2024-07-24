using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyProjectileController : MonoBehaviour
{
  void OnCollisionEnter(Collision col){
    Destroy(gameObject);
  }
}
