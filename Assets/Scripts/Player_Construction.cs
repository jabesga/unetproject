using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Player_Construction : NetworkBehaviour {
	
	[SerializeField] private Transform playerCam;

	private RaycastHit hit;
	private float range = 200;

	private Dictionary<int, GameObject> templates;
	public GameObject cubeTemplate;
	public GameObject platformTemplate;
	public GameObject drawerTemplate;

	private GameObject objectInConstruction;

	private int lastItemSelect = 1;
	public bool builtMode = false;
	public bool destructionMode = false;
	private Vector2 screenCenter;

	void Awake () {
		screenCenter = new Vector2(Screen.width/2, Screen.height/2);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	public override void OnStartClient(){
		templates = new Dictionary<int, GameObject>();
		templates.Add(1, cubeTemplate);
		templates.Add(2, platformTemplate);
		templates.Add(3, drawerTemplate);
		templates.Add(-99, drawerTemplate);
	}

	[Server]
	public override void OnStartServer ()
	{
		templates = new Dictionary<int, GameObject>();
		templates.Add(1, cubeTemplate);
		templates.Add(2, platformTemplate);
		templates.Add(3, drawerTemplate);
		templates.Add(-99, drawerTemplate);


		Debug.Log("Trying to load instances");
		LoadInstancePrefabs();
	}

	[Command]
	void CmdDeleteInstancePrefab(String name, Vector3 pos, Quaternion rot, GameObject obj){
		Debug.Log("Deleting object...");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
		ObjectDataList dataList = (ObjectDataList) bf.Deserialize(file);
		file.Close();

		int object_id = -99;
		if(name.Equals("CubeTemplate(Clone)")){
			object_id = 1;
		}
		else if(name.Equals("PlatformTemplate(Clone)")){
			object_id = 2;
		}
		else if(name.Equals("DrawerTemplate(Clone)")){
			object_id = 3;
		}
		bool removed = false;
		int i = 0;
		while (i < dataList.objectsCreated.Count && removed == false) {
			ObjectData o = dataList.objectsCreated[i];
			if(o.getId().Equals(object_id)){
				if(o.getPosition().Equals(pos)){
					if(o.getRotation().Equals(rot)){
						dataList.objectsCreated.RemoveAt(i);
						removed = true;
						Debug.Log ("Object found");
					}
				}
			}
			i++;
		}

		file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Create);
		bf.Serialize(file, dataList);
		file.Close();
		Debug.Log ("Object removed");
		NetworkServer.Destroy (obj);

	}
	[Command]
	void CmdSaveInstancePrefab(String name, Vector3 pos, Quaternion rot){
		Debug.Log("Saving object...");

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
		ObjectDataList dataList = (ObjectDataList) bf.Deserialize(file);
		file.Close();

		int object_id = -99;
		//Debug.Log(name);
		if(name.Equals("CubeTemplate(Clone)")){
			object_id = 1;
		}
		else if(name.Equals("PlatformTemplate(Clone)")){
			object_id = 2;
		}
		else if(name.Equals("DrawerTemplate(Clone)")){
			object_id = 3;
		}
			
		ObjectData data = new ObjectData(object_id, pos, rot);
		dataList.objectsCreated.Add(data);
		//Debug.Log(dataList.objectsCreated.Count);

		file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Create);
		bf.Serialize(file, dataList);
		file.Close();

		Debug.Log("Object saved");
		GameObject obj = (GameObject) Instantiate(templates[object_id],pos, rot);
		obj.layer = 0;
		NetworkServer.Spawn(obj);

	}


	[Server]
	void LoadInstancePrefabs(){
		if(File.Exists(Application.persistentDataPath + "/levelInfo.dat")){
			
			Debug.Log("File levelInfo found");

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
			ObjectDataList data = (ObjectDataList) bf.Deserialize(file);
			//			Debug.Log("Objects in list:" + data.objectsCreated.Count.ToString());
			if(data.objectsCreated.Count > 0){
				Debug.Log("Loading objects...");
				for(int i = 0; i < data.objectsCreated.Count; i++){
					//Debug.Log("POS: " + data.objectsCreated[i].getPosition().ToString());
					//Debug.Log(data.objectsCreated[i].getId());
					GameObject go = (GameObject) Instantiate(templates[data.objectsCreated[i].getId()], data.objectsCreated[i].getPosition(), data.objectsCreated[i].getRotation());
					NetworkServer.Spawn(go);
				}
			}
			else{
				Debug.Log("No objects to load");
			}
			file.Close();
		}
		else{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Create);
			ObjectDataList dataList = new ObjectDataList();

			bf.Serialize(file, dataList);
			file.Close();

			Debug.Log("File levelInfo created");
		}
	}


	// Update is called once per frame
	void Update () {
		if(isLocalPlayer){
			if(Input.GetKeyDown(KeyCode.E)){
				if(builtMode){
					builtMode = false;
					if(objectInConstruction){
						Destroy(objectInConstruction.gameObject);
					}
				}
				else{
					builtMode = true;
				}

			}

			if(builtMode){
				if(Physics.Raycast(playerCam.GetComponent<Camera>().ScreenPointToRay(screenCenter), out hit, range)){ //Input.mousePosition), out hit, range)){
					if(lastItemSelect != this.GetComponent<InventoryHotbar>().itemSelected || objectInConstruction == null){
						lastItemSelect = this.GetComponent<InventoryHotbar>().itemSelected;
						if(objectInConstruction){
							Destroy(objectInConstruction.gameObject);
						}
						objectInConstruction = (GameObject)Instantiate(templates[lastItemSelect], new Vector3(0,0,0), new Quaternion(0,0,0,0));
						objectInConstruction.layer = 2;
					}
					objectInConstruction.transform.position = hit.point - objectInConstruction.transform.FindChild("CustomPivot").transform.localPosition;
				}
				if(Input.GetKeyDown(KeyCode.Mouse0)){
					CmdSaveInstancePrefab(objectInConstruction.name, objectInConstruction.transform.position, objectInConstruction.transform.rotation);
					builtMode = false;
					objectInConstruction = null;
				}
			}
			if(this.GetComponent<InventoryHotbar>().itemSelected == 4){
				destructionMode = true;
			}
			else{
				destructionMode = false;
			}

			if(destructionMode){
				if(Physics.Raycast(playerCam.GetComponent<Camera>().ScreenPointToRay(screenCenter), out hit, range)){
					if(hit.transform.tag != "Indestructible"){
						if(Input.GetKeyDown(KeyCode.Mouse0)){
							CmdDeleteInstancePrefab(hit.transform.name, hit.transform.position, hit.transform.rotation, hit.transform.gameObject);
						}
					}
				}
			}
		}
	}
}

[Serializable]
class ObjectDataList{
	public List<ObjectData> objectsCreated = new List<ObjectData>();

	public void printAll(){
		for(int i = 0; i < objectsCreated.Count; i++){
			Debug.Log(objectsCreated[i]);
		}
	}
}

[Serializable]
class ObjectData{
	private float posX;
	private float posY;
	private float posZ;

	private float quatX;
	private float quatY;
	private float quatZ;
	private float quatW;

	private int object_id;

	public ObjectData(int id, Vector3 pos, Quaternion quat){
		object_id = id;
		posX = pos.x;
		posY = pos.y;
		posZ = pos.z;
		quatX = quat.x;
		quatY = quat.y;
		quatZ = quat.z;
		quatW = quat.w;
	}

	public int getId(){
		return object_id;
	}

	public Vector3 getPosition(){
		return new Vector3(posX, posY, posZ);
	}

	public Quaternion getRotation(){
		return new Quaternion(quatX, quatY, quatZ, quatW);
	}

}
