using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PortalDoor : NetworkBehaviour {


	void OnTriggerEnter(Collider other){
		if (other.tag.Equals ("Player")) {
			other.GetComponent<Player_PhaseOut>().TransmitPhaseOut();
		}
	}
}
