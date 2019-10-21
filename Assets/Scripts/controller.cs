using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {
	float limit = 1f;
	float ylimit = 4.9f;
	float speed = 8f;
	BoxCollider2D myBounds;

	Rigidbody2D myRb;

	void Start () {
		myRb = GetComponent<Rigidbody2D>();

		myBounds = GetComponent<BoxCollider2D>();
	}

	void Update () {
	}

	public void move(Vector2 myVel, bool isPlayer){
		if((Mathf.Abs(transform.position.x + myVel.x) >= 10.7f)){
			myVel.x = 0;
		}else if((Mathf.Abs(transform.position.x + myVel.x) <= limit)){
			myVel.x = 0;
		}

		//if this is the paddle we have to restrict the movement
		/*if(!isPlayer){
			// myYPosition + half my height + myYVelocity
			if((Mathf.Abs(transform.position.y + ((myBounds.size.y/2)* transform.localScale.y) + myVel.y) >= ylimit)){
				myVel.y = 0f;
			}
		}
		*/

		//Rotate paddle
		if(isPlayer){
			transform.rotation = Quaternion.AngleAxis(myVel.y*-5f, transform.forward);
		}else{
			transform.rotation = Quaternion.AngleAxis(myVel.y*5f, transform.forward);
		}

		myRb.velocity = myVel*speed;
	}

	public float getSpeed(){
		return speed;
	}
	public float getLimit(){
		return limit;
	}
}
