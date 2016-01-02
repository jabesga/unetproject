using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class Network_PlayerTransform : NetworkBehaviour {

	[SyncVar(hook="SyncPositionValues")] Vector3 syncPos;

	[SyncVar] Quaternion syncRot;
	[SyncVar] Quaternion syncRotCam;


	[SerializeField] Transform player;
	[SerializeField] Transform playerCam;

	private float lerpRate = 15;
	private float normalLerpRate = 16; // HistoricalLerping or Timetravel magic!
	private float fasterLerpRate = 27; // HistoricalLerping or Timetravel magic!

	private  float thresholdPos = 0.5f;
	private  float thresholdRot = 5;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private  Quaternion lastRotCam;

	private List<Vector3> syncPosList = new List<Vector3>(); // HistoricalLerping or Timetravel magic!
	[SerializeField] private bool useHistoricalLerping = false; // HistoricalLerping or Timetravel magic!
	private float closeEnough = 0.11f; // HistoricalLerping or Timetravel magic!

	void Update(){
		LerpPosition();
		LerpRotations();
	}

	// Update is called once per frame
	void FixedUpdate () {
		TransmitPosition();
		TransmitRotations();

	}

	[ClientCallback]
	void SyncPositionValues(Vector3 latestPos){ // HistoricalLerping or Timetravel magic!
		syncPos = latestPos;
		syncPosList.Add(syncPos);
	}

	void LerpPosition(){
		if(!isLocalPlayer){
			// HistoricalLerping or Timetravel magic!
			if(useHistoricalLerping){
				if(syncPosList.Count > 0){
					player.transform.position = Vector3.Lerp(player.transform.position, syncPosList[0], Time.deltaTime * lerpRate);

					if(Vector3.Distance(player.transform.position, syncPosList[0]) < closeEnough){
						syncPosList.RemoveAt(0);
					}

					if(syncPosList.Count > 10){
						lerpRate = fasterLerpRate;
					}
					else{
						lerpRate = normalLerpRate;
					}

					//Debug.Log(syncPosList.Count.ToString());
				}
			}
			// HistoricalLerping or Timetravel magic!

			else{
				// OrdinaryLerping
				player.transform.position = Vector3.Lerp(player.transform.position, syncPos, Time.deltaTime * lerpRate);
			}

		}
	}

	void LerpRotations(){
		if(!isLocalPlayer){
			player.transform.rotation = Quaternion.Lerp(player.transform.rotation, syncRot, Time.deltaTime * lerpRate);
			playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, syncRotCam, Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdSyncPosition(Vector3 pos){
		syncPos = pos;
	}

	[Command]
	void CmdSyncRotations(Quaternion rot, Quaternion rotCam){
		syncRot = rot;
		syncRotCam = rotCam;
	}

	[ClientCallback]
	void TransmitPosition(){
		if(isLocalPlayer){
			if(Vector3.Distance(player.transform.position, lastPos) > thresholdPos){
				CmdSyncPosition(player.transform.position);
				lastPos = player.transform.position;
			}
		}
	}

	[ClientCallback]
	void TransmitRotations(){
		if(isLocalPlayer){
			if(Quaternion.Angle(player.transform.rotation, lastRot) > thresholdRot || Quaternion.Angle(playerCam.transform.rotation, lastRotCam) > thresholdRot){
				CmdSyncRotations(player.transform.rotation, playerCam.transform.rotation);
				lastRot = player.transform.rotation;
				lastRotCam = playerCam.transform.rotation;
			}
		}
	}
}
