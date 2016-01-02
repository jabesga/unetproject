using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Network_PlayerController : NetworkBehaviour {
	
	public GameObject player;
	public Camera playerCam;

	// Use this for initialization
	void Start () {
		if(isLocalPlayer){
			//player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			playerCam.enabled = true;
			playerCam.GetComponent<AudioListener>().enabled = true;
		}
	}
}
