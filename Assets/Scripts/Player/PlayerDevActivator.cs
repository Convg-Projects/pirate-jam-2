using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerDevActivator : NetworkBehaviour {

	public NetworkVariable<bool> isDev = new NetworkVariable<bool>(false);
	[SerializeField]private string devPassword = "jayko";
	[SerializeField]private float timeForPassword = 2f;

	private string currentCode = "";
	private float timeLeft = 2f;

	void Update(){
		if(isDev.Value){return;}
		if(!IsOwner){return;}

		if(Input.inputString.Length != 0){
			if(Input.inputString[0] != devPassword[currentCode.Length]){
				currentCode = "";
			} else {
				currentCode += Input.inputString;
			}
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
			SetDevModeRpc(true);
			currentCode = "";
			//potential network performance issue if you dont the string here
		}
		Debug.Log(currentCode);
	}

	[Rpc(SendTo.Server)]
	public void SetDevModeRpc(bool mode){
		isDev.Value = mode;
	}
}
