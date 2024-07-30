using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDeathMessageHandler : MonoBehaviour
{
  [SerializeField]private TextMeshProUGUI deathMessageText;
  [SerializeField]private float maxDeathMessageTime = 3f;
  private float deathMessageTime = 0f;

  void Awake(){
  }

  void Update(){
    deathMessageTime -= Time.deltaTime;

    if(deathMessageTime <= 0f){
      deathMessageText.gameObject.SetActive(false);
    }
  }

  public void ShowDeathMessage(string name){
    deathMessageText.text = "ELIMINATED <color=#FF7171>" + name + "</color>";
    deathMessageTime = maxDeathMessageTime;
    deathMessageText.gameObject.SetActive(true);
  }
}
