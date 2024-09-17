using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCosmeticHandler : NetworkBehaviour
{
  public SkinnedMeshRenderer[] meshRenderers;
  public Material[] defaultSkinMaterials;
  public Material devSkinMaterial;

  public override void OnNetworkSpawn(){
    CheckSkinRPC();

    base.OnNetworkSpawn();
  }

  public void Update(){
    CheckSkinRPC();
  }

  [Rpc(SendTo.Everyone)]
  public void CheckSkinRPC(){
    for(int i = 0; i < meshRenderers.Length; ++i){
      Debug.Log("isDev: " + GetComponent<PlayerDevActivator>().isDev.Value);

      if(GetComponent<PlayerDevActivator>().isDev.Value){
        meshRenderers[i].material = devSkinMaterial;
      } else {
        meshRenderers[i].material = defaultSkinMaterials[0];
      }
    }
  }
}
