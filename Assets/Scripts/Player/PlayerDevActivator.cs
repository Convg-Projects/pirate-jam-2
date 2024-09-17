using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerDevActivator : NetworkBehaviour {

	public NetworkVariable<bool> isDev = new NetworkVariable<bool>(false);
	[SerializeField]private static string devPassword = "jayko";
	[SerializeField]private static float timeForPassword = 2f;

	private string currentCode = "";
	private float timeLeft = 2f;

	void Update(){
		if(Input.inputString[0] != devPassword[currentCode.Length]){
			currentCode = "";
		} else {
			currentCode += Input.inputString;
		}

		if(currentCode == ""){
			timeLeft = timeForPassword;
		} else {
			timeLeft -= Time.deltaTime;
		}

		if(timeLeft <= 0f){
			currentCode = "";
		}

		if(currentCode == devPassword){
			isDev.Value = true;
		}
	}
}
