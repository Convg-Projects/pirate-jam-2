using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadoutEditUI : MonoBehaviour
{
  [SerializeField]private TextMeshProUGUI descriptionText;
  public WeaponScriptableObject weaponData;

  public void Hide() {
    gameObject.SetActive(false);
  }

  public void Show() {
    descriptionText.text = weaponData.description;
    gameObject.SetActive(true);
  }
}
