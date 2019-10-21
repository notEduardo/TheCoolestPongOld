using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour {
	gameManager manager;
	float timer;
	public int itemCode;

	// Use this for initialization
	void Start () {
		GameObject managerInstance = GameObject.FindWithTag("manager");
		manager = managerInstance.GetComponent<gameManager>();

		if(gameObject.tag == "test"){
			itemCode = -1;
		}

		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer >= 5){
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision){
		if(collision.gameObject.tag == "enemy"){
			manager.giveItem(false, itemCode);
		}else if(collision.gameObject.tag == "Player"){
			manager.giveItem(true, itemCode);
		}

	}
}
