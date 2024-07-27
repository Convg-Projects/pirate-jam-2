using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
  public static WeaponManager Instance { get; private set; }

  [SerializeField]private LoadoutEditUI loadoutUI;

  public WeaponScriptableObject weaponData;

  void Awake(){
    if (Instance != null){
      Debug.Log("There is already a Weapon Manager instance. Destroying component!");
      Destroy(this);
    } else {
      Instance = this;
    }

  }

  public void SetWeapon(WeaponScriptableObject newWeaponData){
    weaponData = newWeaponData;
    loadoutUI.weaponData = newWeaponData;
    loadoutUI.Show();
  }
}
