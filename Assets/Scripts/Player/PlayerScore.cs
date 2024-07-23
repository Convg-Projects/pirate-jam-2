using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerScore : NetworkBehaviour
{
  [SerializeField]private TextMeshProUGUI ScoreText;
  public NetworkVariable<int> currentScore = new NetworkVariable<int>();

  public override void OnNetworkSpawn(){
    currentScore.Value = 0;

    base.OnNetworkSpawn();
  }

  private void Update(){
    ScoreText.text = "Kills: " + currentScore.Value;
  }

  public void UpdateScore(int increase){
    currentScore.Value += increase;
  }
}
