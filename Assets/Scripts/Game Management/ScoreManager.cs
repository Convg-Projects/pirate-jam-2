using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
  public static ScoreManager Instance { get; private set; }

  [SerializeField]private GameObject timerObject;
  [SerializeField]private GameObject endgameCanvas;
  [SerializeField]private GameObject restartButton;
  [SerializeField]private TextMeshProUGUI goldText;
  [SerializeField]private TextMeshProUGUI silverText;
  [SerializeField]private TextMeshProUGUI bronzeText;
  [SerializeField]private TextMeshProUGUI localText;
  public TextMeshProUGUI clockText;

  [SerializeField]private float gameDuration = 500f;
  public NetworkVariable<float> timeLeft = new NetworkVariable<float>();
  private float localTimeLeft;
  private float timerHeartbeatTime;

  private bool gameEnded = false;

  public override void OnNetworkSpawn(){
    if(IsOwner){
      timeLeft.OnValueChanged += OnTimeChanged;
    }

    if (Instance != null){
      Debug.Log("There is already a Score Manager instance. Destroying component!");
      Destroy(this);
    } else {
      Instance = this;
    }

    if(IsHost){
      timeLeft.Value = gameDuration;
      localTimeLeft = gameDuration;
    }

    base.OnNetworkSpawn();

    SyncTime();
    ActivateTimer();
  }

  void Update(){
    if(IsHost && !gameEnded){
      timerHeartbeatTime -= Time.deltaTime;
      if(timerHeartbeatTime <= 0f){
        timeLeft.Value = gameDuration - (float) NetworkManager.ServerTime.Time;
        timerHeartbeatTime = 15f;
      }

      if(localTimeLeft <= 0f){
        EndGameRpc();
        gameEnded = true;
      }
    }
    localTimeLeft -= Time.deltaTime;

    var ts = TimeSpan.FromSeconds(localTimeLeft);
    clockText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
  }

  public void SyncTime(){
    localTimeLeft = timeLeft.Value;
  }

  public void OnTimeChanged(float previous, float current){
    SyncTime();
  }

  public void ActivateTimer(){
    timerObject.SetActive(true);
  }

  [Rpc(SendTo.Everyone)]
  public void EndGameRpc(){
    endgameCanvas.SetActive(true);

    if(IsHost){
      restartButton.SetActive(true); // Allow only the host to restart the game

      // Find and display scores
      IReadOnlyDictionary<ulong, NetworkClient> connectedClients = NetworkManager.Singleton.ConnectedClients;
      int[] scores = new int[connectedClients.Count];
      string[] names = new string[connectedClients.Count];
      for(ulong i = 0; i < (ulong)connectedClients.Count; ++i){
        scores[i] = connectedClients[i].PlayerObject.GetComponent<PlayerScore>().currentScore.Value;
        names[i] = connectedClients[i].PlayerObject.GetComponent<PlayerId>().playerName.Value.stringValue;
        Debug.Log(scores[i]);
        Debug.Log(names[i]);
      }

      //sort scores
      int[] sortedScores = new int[connectedClients.Count];
      string[] sortedNames = new string[connectedClients.Count];
      for(int i = 0; i < scores.Length; ++i){
        int topScore = -1;
        string topName = "Error";

        for(int j = 0; j < scores.Length; ++j){
          if (scores[j] > topScore){
            topScore = scores[j];
            topName = names[j];
          }
        }

        scores[i] = -1;
        sortedScores[i] = topScore;
        sortedNames[i] = topName;
      }

      if(sortedScores.Length >= 3){
        ShowScoresRpc(sortedScores[0], sortedScores[1], sortedScores[2], sortedNames[0], sortedNames[1], sortedNames[2]);
        return;
      }
      if(sortedScores.Length >= 2){
        ShowScoresRpc(sortedScores[0], sortedScores[1], sortedNames[0], sortedNames[1]);
        return;
      }
      ShowScoresRpc(sortedScores[0], sortedNames[0]);
    }
  }

  [Rpc(SendTo.Everyone)]
  public void ShowScoresRpc(int goldScore, int silverScore, int bronzeScore, string goldName, string silverName, string bronzeName){
    goldText.text = goldName + ": " + goldScore;
    silverText.text = silverName + ": " + silverScore;
    bronzeText.text = bronzeName + ": " + bronzeScore;

    GameObject localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
    localText.text = localPlayerObject.GetComponent<PlayerId>().playerName.Value.stringValue + ": " + localPlayerObject.GetComponent<PlayerScore>().currentScore.Value;

    goldText.gameObject.SetActive(true);
    silverText.gameObject.SetActive(true);
    bronzeText.gameObject.SetActive(true);
    localText.gameObject.SetActive(true);
  }

  [Rpc(SendTo.Everyone)]
  public void ShowScoresRpc(int goldScore, int silverScore, string goldName, string silverName){
    goldText.text = goldName + ": " + goldScore;
    silverText.text = silverName + ": " + silverScore;

    GameObject localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
    localText.text = localPlayerObject.GetComponent<PlayerId>().playerName.Value.stringValue + ": " + localPlayerObject.GetComponent<PlayerScore>().currentScore.Value;

    goldText.gameObject.SetActive(true);
    silverText.gameObject.SetActive(true);
    localText.gameObject.SetActive(true);
  }

  [Rpc(SendTo.Everyone)]
  public void ShowScoresRpc(int goldScore, string goldName){
    goldText.text = goldName + ": " + goldScore;

    GameObject localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
    localText.text = localPlayerObject.GetComponent<PlayerId>().playerName.Value.stringValue + ": " + localPlayerObject.GetComponent<PlayerScore>().currentScore.Value;

    goldText.gameObject.SetActive(true);
    localText.gameObject.SetActive(true);
  }
}
