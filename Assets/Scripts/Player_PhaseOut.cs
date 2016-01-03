using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_PhaseOut : NetworkBehaviour {

	[SyncVar] public bool phaseOut = false;
	public LayerMask secretHouse;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (phaseOut) {
			PhaseOut();
		}
	}

	void PhaseOut(){
		this.gameObject.layer = 8;
		foreach(Transform child in this.transform)
		{
			child.gameObject.layer = 8;
			foreach(Transform child2 in child.transform){
				child2.gameObject.layer = 8;
			}
		}
		this.transform.FindChild("FirstPersonCharacter").GetComponent<Camera>().cullingMask = secretHouse;
		//CmdUpdatePlayerLayer (player.GetComponent<NetworkIdentity> ().netId);
	}

	[Command]
	void CmdSendPhaseOutToServer(bool p){
		phaseOut = p;
	}

	[ClientCallback]
	public void TransmitPhaseOut(){
		CmdSendPhaseOutToServer (true);
	}
}
