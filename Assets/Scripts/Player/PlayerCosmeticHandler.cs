using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCosmeticHandler : NetworkBehaviour
{
  public SkinnedMeshRenderer[] meshRenderers;
  public Material[] defaultSkinMaterials;

  public NetworkVariable<int> currentSkinIndex = new NetworkVariable<int>(0);

  public override void OnNetworkSpawn(){
    currentSkinIndex.OnValueChanged += OnSkinChanged;
    SetSkinRPC();

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
