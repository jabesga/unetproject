using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LookAtCamera : NetworkBehaviour
{
	public GameObject player;
	[SyncVar] public Camera SourceCamera;
	public Text name;

	void Start(){
		if (isLocalPlayer) {
			TransmitSourceCamera(player.GetComponent<NetworkIdentity>().netId);
		}
	}

	void Update()
	{
		if(SourceCamera != null){
			name.transform.rotation = SourceCamera.transform.rotation;
		}

	}

	[Command]
	void CmdTellServerSourceCamera(NetworkInstanceId id){
		SourceCamera = NetworkServer.FindLocalObject(id).transform.FindChild("FirstPersonCharacter").GetComponent<Camera>();
	}

	[ClientCallback]
	void TransmitSourceCamera(NetworkInstanceId id){
		CmdTellServerSourceCamera (id);
	}
}