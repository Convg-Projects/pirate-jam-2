using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Networking;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class RelayController : MonoBehaviour
{
  public static RelayController Instance { get; private set; }

  private void Awake() {
    if (Instance != null){
      Debug.Log("There is already a relay controller instance. Destroying component!");
      Destroy(this);
    } else {
      Instance = this;
    }
  }

  private async void Start(){
    await UnityServices.InitializeAsync();

    AuthenticationService.Instance.SignedIn += () => {
      Debug.Log("Signed in as" + AuthenticationService.Instance.PlayerId);
    };
    await AuthenticationService.Instance.SignInAnonymouslyAsync();
  }

  public async Task<string> CreateRelay(){
    try{
      Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

      string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

      Debug.Log("Join Code: " + joinCode);

      RelayServerData relayServerData = new RelayServerData(allocation, "wss");
      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

      NetworkManager.Singleton.StartHost();

      return joinCode;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return null;
    }
  }

  /*public async Task<string> JoinRelay(string joinCode){
    try {
      JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

      RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

      NetworkManager.Singleton.StartClient();
      Debug.Log("Joining relay with " + joinCode);
      return joinCode;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return null;
    }
  }*/

  public async void JoinRelay(string joinCode){
    try {
      JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

      RelayServerData relayServerData = new RelayServerData(joinAllocation, "wss");
      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

      NetworkManager.Singleton.StartClient();
      Debug.Log("Joining relay with " + joinCode);
    } catch (RelayServiceException e) {
      Debug.Log(e);
    }
  }
}
