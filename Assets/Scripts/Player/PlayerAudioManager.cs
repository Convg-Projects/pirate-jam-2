using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAudioManager : NetworkBehaviour {
  public NetworkVariable<bool> moving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
  public NetworkVariable<bool> crouched = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  [SerializeField]private GameObject[] footstepSounds;

  private float maxFootstepTime = 0.6f;
  private float footstepTime;

  void Update(){
    PlayMoveSounds();
  }

  void PlayMoveSounds(){
    if(!moving.Value){return;}

    footstepTime -= Time.deltaTime;
    if(footstepTime <= 0f){
      GameObject soundInstance = GameObject.Instantiate(footstepSounds[Random.Range(0, footstepSounds.Length)]);
      soundInstance.transform.position = transform.position;

      Destroy(soundInstance, 0.5f);

      footstepTime = maxFootstepTime * (crouched.Value ? 1f : 0.5f);
    }
  }

  /*[Rpc(SendTo.Everyone)]
  public void BulletAudioRpc(){

  }*/
}
