using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {
	Rigidbody2D myRb;
	SpriteRenderer myRend;
	public gameManager manager;
	public ai opAi;
	public myAI plAi;
	public bool playersTurn;

	bool[] fastball;
	bool[] fastActivate;
	bool invisible;
	float invisTimer;
	float speed;
	int startDir;

	//Predictive variables
	List<Vector3> futurePath;
	Vector2 yLimit;

	void Start () {
		myRb = GetComponent<Rigidbody2D>();
		myRend = GetComponent<SpriteRenderer>();

		playersTurn = true;
		futurePath = new List<Vector3>();
		yLimit = new Vector2(-5.4f, 5.4f);

		speed = 10f;

		invisTimer = 0;

		// 0 is player 1 is ooponent
		fastball = new bool[2];
		fastball[0] = false;
		fastball[1] = false;

		fastActivate = new bool[2];
		fastActivate[0] = false;
		fastActivate[1] = false;
	}

	// Update is called once per frame
	void Update () {
		/* Testing Output
		 * 
		 * Debug.Log("Direction : " + transform.right);
		Debug.Log("Speed : " + speed);
		Debug.Log("Direction * Speed : " + speed*transform.right);
		Debug.Log("Positon : " + transform.position);
		Debug.Log("Elapsed Time * velocity (amount advanced last update): " + (speed*transform.right)*Time.deltaTime);
		Vector3 predict = ((speed*transform.right)*Time.deltaTime) + transform.position;
		Debug.Log("Predicted next position : " + predict);
		Debug.Log("--------------------------------");
		*/


		// code for invisible ball
		if(invisible){
			Color newColor = myRend.color;
			invisTimer += Time.deltaTime;
			float alpha = (invisTimer%1);
			if(alpha < .5f){
				alpha = 0f;
			}else{
				alpha = 1f;
			}
			newColor.a = alpha;
			myRend.color = newColor;
		}

		// code for fastball
		if(manager.gameRunning){
			if(transform.position.x < 0){
				// If its on the ooponent side we check if theres a fast ball
				// also we check if we've already activated it
				if(fastball[0] && !fastActivate[0]){
					speed = 15f;
					myRb.velocity = transform.right * speed;
					fastActivate[0] = true;
				}// Otherwise we check if fastball is off and see if we've
				//unactivated it
				else if(!fastball[0] && fastActivate[0]){
					speed = 10f;
					myRb.velocity = transform.right * speed;
					fastActivate[0] = false;
				}// last case is if the fast ball is off but is on the
				// other side
				else if(!fastball[0] && fastActivate[1]){
					speed = 10f;
					myRb.velocity = transform.right * speed;
					fastActivate[1] = false;
				}

			} else if(transform.position.x > 0){
				// If its on the ooponent side we check if theres a fast ball
				// also we check if we've already activated it
				if(fastball[1] && !fastActivate[1]){
					speed = 15f;
					myRb.velocity = transform.right * speed;
					fastActivate[1] = true;
				}// Otherwise we check if fastball is off and see if we've
				//unactivated it
				else if(!fastball[1] && fastActivate[1]){
					speed = 10f;
					myRb.velocity = transform.right * speed;
					fastActivate[1] = false;
				}// last case is if the fast ball is off but is on the
				// other side
				else if(!fastball[1] && fastActivate[0]){
					speed = 10f;
					myRb.velocity = transform.right * speed;
					fastActivate[0] = false;
				}
			}
		}

	}

	public void startMoving() {
		startDir = Random.Range(0, 2);
		Vector3 direction;

		if(startDir == 1){
			direction = new Vector3(1f, Random.Range(-.4f, .4f), 0f);
			playersTurn = true;
			opAi.alertTurn(false, "start");
			if(plAi.enabled){
				plAi.alertTurn(true, "start");
			}

		}
		else{
			direction = new Vector3(-1f, Random.Range(-.75f, .75f), 0f);
			playersTurn = false;
			opAi.alertTurn(true, "start");
			if(plAi.enabled){
				plAi.alertTurn(false, "start");
			}
		}

		rotateBall(direction);
		myRb.velocity = direction*speed;

	}

	public void resetBall() {
		transform.position = new Vector3(0f, 0f, 0f);
		transform.rotation = Quaternion.identity;
		myRb.velocity = Vector2.zero;

		//reset tracking variables
		playersTurn = true;
	}

	void rotateBall(Vector3 direction){
		float turnAngle;

		turnAngle = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(turnAngle, transform.forward);
	}

	void OnCollisionEnter2D(Collision2D collision){
		ContactPoint2D contact;
		float dotProd;
		Vector3 reflection;

		contact = collision.contacts[0];
		dotProd = Vector2.Dot(contact.normal, (-transform.right));
		dotProd *= 2;

		reflection = contact.normal * dotProd;
		reflection += transform.right;

		rotateBall(reflection);
		myRb.velocity = reflection*speed;

		if(reflection.x > 0){
			playersTurn = true;
			opAi.alertTurn(false, (collision.gameObject.name));
			if(plAi.enabled){
				plAi.alertTurn(true, (collision.gameObject.name));
			}
		}else{
			playersTurn = false;
			opAi.alertTurn(true, (collision.gameObject.name));
			if(plAi.enabled){
				plAi.alertTurn(false, (collision.gameObject.name));
			}
		}
	}

	bool predictPath(Vector3 startPos, Vector3 velocity, bool isPlAi){
		float dotProd;
		Vector3 newPos;
		Vector3 newVel;
		Vector3 reflection = velocity/speed;

		// If the ball has passed the AI's goal, return
		if(startPos.x <= -9f && !isPlAi){
			return true;
		}
		if(startPos.x >= 9f && isPlAi){
			return true;
		}

		newVel = reflection*speed;
		newPos = startPos + (newVel *Time.deltaTime);

		//Calculate new direction if hits wall
		if(startPos.y >= yLimit.y || newPos.y >= yLimit.y){
			dotProd = Vector2.Dot(-Vector2.up, -(velocity/speed));
			dotProd *= 2;

			reflection = -Vector2.up * dotProd;
			reflection += velocity/speed;

			newVel = reflection*speed;
			newPos = startPos + (newVel *Time.deltaTime);

			/*Testing Output
			 * 
			Debug.Log("----------------------");
			Debug.Log("TOP COLLISION");
			Debug.Log("dotProd: " + dotProd);
			Debug.Log("vel: " + velocity/speed);
			Debug.Log("position: " + startPos.y);
			Debug.Log("reflection: " + reflection);
			Debug.Log("----------------------");
			*/
		}else if(startPos.y <= yLimit.x || newPos.y >= yLimit.y){
			dotProd = Vector2.Dot(Vector2.up, -(velocity/speed));
			dotProd *= 2;

			reflection = Vector2.up * dotProd;
			reflection += velocity/speed;

			newVel = reflection*speed;
			newPos = startPos + (newVel *Time.deltaTime);

			/*Testing Output
			 * 
			Debug.Log("----------------------");
			Debug.Log("BOT COLLISION");
			Debug.Log("dotProd: " + dotProd);
			Debug.Log("vel: " + velocity/speed);
			Debug.Log("position: " + startPos.y);
			Debug.Log("reflection: " + reflection);
			Debug.Log("----------------------");
			*/
		}

		futurePath.Add(newPos);

		return predictPath(newPos, newVel, isPlAi);

	}

	//public methods
	public List<Vector3> getPath(bool isPlAi){
		futurePath.Clear();

		predictPath(transform.position, myRb.velocity, isPlAi);

		return futurePath;
	}

	public void fastBall(bool fastOn, int player){
		fastball[player] = fastOn;
	}

	public void setInvisible(bool invisOn){
		invisible = invisOn;
		invisTimer = 0f;

		if(!invisOn){
			Color newColor = myRend.color;
			newColor.a = 1f;
			myRend.color = newColor;
		}
	}
}