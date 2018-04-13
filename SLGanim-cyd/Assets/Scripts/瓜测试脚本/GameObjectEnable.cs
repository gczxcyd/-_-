using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(){
		this.gameObject.SetActive (false);
	}

	void OnTriggerExit(){
		this.gameObject.SetActive (true);
	}
}
