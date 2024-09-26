using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
  [SerializeField]private GameObject buttonSoundPrefab;

  public void PlayButtonSound(){
    GameObject audioInstance = GameObject.Instantiate(buttonSoundPrefab);
    Destroy(audioInstance, 1f);
  }
}
