using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCosmeticHandler : NetworkBehaviour
{
  [SerializeField]private SkinnedMeshRenderer[] meshRenderers;
  public Material[] defaultSkinMaterials;

  public NetworkVariable<int> currentSkinIndex = new NetworkVariable<int>(0);

  public override void OnNetworkSpawn(){
    currentSkinIndex.OnValueChanged += OnSkinChanged;
    if(IsOwner){
      SetSkinRPC();
    } else {
      for(int i = 0; i < meshRenderers.Length; ++i){
        meshRenderers[i].material = defaultSkinMaterials[currentSkinIndex.Value];
      }
    }

    base.OnNetworkSpawn();
  }

  public void Update(){
  }

  public void OnSkinChanged(int previous, int current){
    for(int i = 0; i < meshRenderers.Length; ++i){
      meshRenderers[i].material = defaultSkinMaterials[current];
    }
  }

  [Rpc(SendTo.Server)]
  public void SetSkinRPC(){
    currentSkinIndex.Value = Random.Range(0, defaultSkinMaterials.Length);
  }
}
