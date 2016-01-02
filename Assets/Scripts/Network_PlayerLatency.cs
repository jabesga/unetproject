using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Network_PlayerLatency : NetworkBehaviour {

	private NetworkClient nClient;
	private Text latencyText;

	// Use this for initialization
	void Start () {
		nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		ShowLatency();
	}

	void ShowLatency(){
		if(isLocalPlayer){
			latencyText.text = nClient.GetRTT().ToString();
		}
	}
}
