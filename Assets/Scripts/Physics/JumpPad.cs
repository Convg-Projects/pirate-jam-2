using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class JumpPad : NetworkBehaviour
{
  [SerializeField]private GameObject jumpAudioPrefab;
  [SerializeField]private float effectRange = 0.5f;
  [SerializeField]private float boostForce = 50f;
  [SerializeField]private float maxBoostCooldown = 0.5f;
  private float boostCooldown = 0f;

  void FixedUpdate(){
    boostCooldown -= Time.fixedDeltaTime;

    Collider[] hitColliders = new Collider[20];

    Vector3 centerPosition = transform.position;
    centerPosition.y += 2f;

    int numColliders = Physics.OverlapSphereNonAlloc(centerPosition, effectRange, hitColliders);

    for(int i = 0; i < numColliders; ++i){
      if(hitColliders[i].transform.tag == "Player" && boostCooldown <= 0f){
        PlayJumpSoundRpc();

        Rigidbody playerRB = hitColliders[i].transform.parent.gameObject.GetComponent<Rigidbody>();

        Vector3 newVelocity = playerRB.velocity;
        newVelocity.x = Mathf.Clamp(newVelocity.x, -3f, 3f);
        newVelocity.z = Mathf.Clamp(newVelocity.z, -3f, 3f);
        newVelocity.y = 0f;
        playerRB.velocity = newVelocity;

        hitColliders[i].transform.parent.GetComponent<PlayerMovement>().haltMovementTime = 0.1f; //stop the player from brining their velocity back up before they leave the ground

        playerRB.AddForce(new Vector3(0f, boostForce * 50f, 0f));
        boostCooldown = maxBoostCooldown;
      }
    }
  }

  [Rpc(SendTo.Everyone)]
  private void PlayJumpSoundRpc(){
    GameObject audioInstance = GameObject.Instantiate(jumpAudioPrefab);
    audioInstance.transform.position = transform.position;
    Destroy(audioInstance, 2f);
  }
}
