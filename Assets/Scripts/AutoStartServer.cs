using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AutoStartServer : MonoBehaviour {

	NetworkManager manager;
	public string ip = "52.16.108.94";
	public int port = 7778;


	void Awake(){
		manager = this.GetComponent<NetworkManager>();
	}
	void Start () {
		manager.networkAddress = ip;
		manager.networkPort = port;
		bool response = manager.StartServer();

		if(response){
			print("Server started!");
		}
	}
}
