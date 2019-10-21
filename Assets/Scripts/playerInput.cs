using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInput : MonoBehaviour {
	bool isPlayer;
	controller myController;

	// Use this for initialization
	void Start () {
		if(gameObject.tag == "Player"){
			isPlayer = true;
		}

		myController = GetComponent<controller>();
	}
	
	// Update is called once per frame
	void Update () {
		//Take care of movement
		Vector2 myVel = Vector2.zero;

		if(isPlayer){
			if(Input.GetKey(KeyCode.W)){
				myVel.y += 1;
			}
			if(Input.GetKey(KeyCode.A)){
				myVel.x -= 1;
			}
			if(Input.GetKey(KeyCode.S)){
				myVel.y -= 1;
			}
			if(Input.GetKey(KeyCode.D)){
				myVel.x += 1;
			}
		}else{
			if(Input.GetKey(KeyCode.UpArrow)){
				myVel.y += 1;
			}
			if(Input.GetKey(KeyCode.LeftArrow)){
				myVel.x -= 1;
			}
			if(Input.GetKey(KeyCode.DownArrow)){
				myVel.y -= 1;
			}
			if(Input.GetKey(KeyCode.RightArrow)){
				myVel.x += 1;
			}
		}

		myController.move(myVel, isPlayer);
	}
}
