using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Network_PlayerID : NetworkBehaviour {

	[SyncVar] public string playerUniqueName;
	private NetworkInstanceId playerNetID;
	private Transform player;

	public override void OnStartLocalPlayer ()
	{
		GetNetIdentity();
		SetIdentity();
	}
	void Awake(){
		player = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(player.name == "" || player.name == "Player(Clone)"){
			SetIdentity();
		}
	}

	[ClientCallback]
	void SetIdentity(){
		if(!isLocalPlayer){
			player.name = playerUniqueName;
		}
		else{
			player.name = MakeUniqueIdentity();
		}
	}

	[ClientCallback]
	void GetNetIdentity(){
		playerNetID = GetComponent<NetworkIdentity>().netId;
		CmdTellServerMyIdentity(MakeUniqueIdentity());
	}

	string MakeUniqueIdentity(){
		return ("Player " + playerNetID.ToString());
	}

	[Command]
	void CmdTellServerMyIdentity(string name){
		playerUniqueName = name;
	}
}
