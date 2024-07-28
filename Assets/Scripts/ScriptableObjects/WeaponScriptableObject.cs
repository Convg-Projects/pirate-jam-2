using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
  public string description = "This is a great weapon choice";

  public int maxAmmo = 100f;

  public bool auto = false;
  public bool hitscan = false;

  public int numberOfShots = 1;
  public float spread = 0f;
  public float maxFiringCooldown = 0.5f;
  public float bulletForce = 100f;

  public int damage = 15;

  public GameObject hitscanHitParticlePrefab;
  public GameObject projectilePrefab;
  public GameObject dummyProjectilePrefab;
}
