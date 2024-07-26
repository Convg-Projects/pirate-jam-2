using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
  public WeaponScriptableObject activeWeapon;

  public void SetWeapon(WeaponScriptableObject weapon){
    activeWeapon = weapon;
  }
}
