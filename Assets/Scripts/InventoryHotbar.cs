using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryHotbar : MonoBehaviour {

	private GameObject hotBar;
	public int itemSelected = 1;
	private int itemMin = 1;
	private int itemMax = 4;
	public Sprite selectedImage;
	public Sprite unselectedImage;

	// Use this for initialization
	void Start () {
		hotBar = GameObject.Find("Hotbar");
	}
	
	// Update is called once per frame
	void Update () {
		if(itemSelected > itemMin){
			if(Input.GetAxis("Mouse ScrollWheel") < 0f){ // backwards
				hotBar.transform.FindChild("Item" + itemSelected).GetComponent<Image>().sprite = unselectedImage;
				itemSelected--;
				hotBar.transform.FindChild("Item" + itemSelected).GetComponent<Image>().sprite = selectedImage;
			}
		}

		if(itemSelected < itemMax){
			if(Input.GetAxis("Mouse ScrollWheel") > 0f){ // forward
				hotBar.transform.FindChild("Item" + itemSelected).GetComponent<Image>().sprite = unselectedImage;
				itemSelected++;
				hotBar.transform.FindChild("Item" + itemSelected).GetComponent<Image>().sprite = selectedImage;
			}
		}
	}
}
