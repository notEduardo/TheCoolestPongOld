using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ai : MonoBehaviour {
	public gameManager manager;
	controller myController;
	BoxCollider2D myBounds;
	public ball myBall;
	//Item Variables
	GameObject myItem;

	//Ball Variables
	bool AITurn;
	List<Vector3> ballPath;

	//Movement Variables
	public Vector3 destination;
	Vector3 myOrigin;
	public string chasing;
	public bool ballChecked;
	public bool itemChecked;
	public Vector2 myVel;
	float speed;
	float limit;
	float yLimit;
	float aiLimitPad;
	int framesAhead;
	public bool ballHit;

	// Use this for initialization
	void Start () {
		myController = GetComponent<controller>();
		myBounds = GetComponent<BoxCollider2D>();
		//Item info
		myItem = null;

		//Ball Info
		AITurn = false;
		ballPath = new List<Vector3>();

		//Movement variables
		myOrigin = transform.position;
		ballChecked = false;
		itemChecked = false;
		myVel = Vector2.zero;
		destination = transform.position;
		chasing = "nothing";
		speed  = myController.getSpeed();
		limit = myController.getLimit()*2f;
		framesAhead = 8;
		yLimit = 4.9f;
		aiLimitPad = 0;

		ballHit = false;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		Debug.Log("--------------------------");
		Debug.Log("position: " + transform.position);
		Debug.Log("velocity: " + myVel);
		Debug.Log("dest: "+ destination);
		Debug.Log("--------------------------");*/

		if(!manager.gameRunning){
			resetLookingVars();
		}

		// Check if we need to calculate a new direction
		if(AITurn && !ballChecked){
			Debug.Log("CalcNextMove - chasing: " + chasing + " goingInWith: " + "ball");

			calculateNextMove("ball");
			ballChecked = true;
			/*
			Debug.Log("position: " + transform.position);
			Debug.Log("velocity: " + myVel);
			Debug.Log("dest: "+ destination);*/
		}

		if(myItem != null && !itemChecked){
			Debug.Log("CalcNextMove - chasing: " + chasing + " goingInWith: " + "item");
			calculateNextMove("item");
			itemChecked = true;
		}

		//Check if we've reached our destination
		if(chasing != "nothing" &&
			Vector3.Distance(transform.position, destination) < .1f){
			Debug.Log("Dest REACHED - chasing: " + chasing + " ballHit:" + ballHit + " destination:" + destination);

			stopMoving();

			if(chasing != "ball"){
				resetLookingVars();
			}else{
				if(transform.position.x >= myBall.transform.position.x){
					resetLookingVars();
				}

				if(ballHit){
					AITurn = false;
					resetLookingVars();
				}
			}

		}

		//if no movement, move towards spawn position
		if(chasing == "nothing" &&
			!AITurn){
			Vector3 dir = myOrigin - transform.position;

			if(dir.magnitude < .5){
				stopMoving();
			}else{
				dir = dir/dir.magnitude;

				myVel = dir;
				destination = myOrigin;
			}
		}


		myController.move(myVel, false);
	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.name == "ball"){
			Debug.Log("ballhit - COLLISION");
			ballHit = true;
		}
	}

	void stopMoving(){
		myVel = Vector2.zero;
		destination = transform.position;
	}

	void resetLookingVars(){
		ballHit = false;
		itemChecked = false;
		ballChecked = false;
		chasing = "nothing";
		AITurn = !myBall.playersTurn;
	}

	void calculateNextMove(string checkWhat){
		Vector3 currentPos = transform.position;
		int frameHitBall = 0;

		// If AI is not moving toward something then move straight to new object
		if(chasing == "nothing"){
			if(checkWhat == "item"){
				// This vector leads to the desired point with the current distance
				Vector3 direction = myItem.transform.position - transform.position;
				// Now we normalize the vector to get direction
				direction = direction/direction.magnitude;

				myVel = direction;
				destination = myItem.transform.position;
			}
			if(checkWhat == "ball"){
				ballPath = myBall.getPath(false);

				//Find the first frame the ball can be hit
				frameHitBall = frameCanHitBall();
				if(frameHitBall == -1){
					Debug.Log("ERROR: Ball can never be hit");
					return;
				}
				// Find what position we have to move to, to meet the ball
				frameHitBall = frameContactBall(frameHitBall, transform.position);

				//Check if its possible to meet the ball
				if(frameHitBall == -1){
					destination = transform.position;
				}else{
					destination = ballPath[frameHitBall];
				}
			}

			chasing = checkWhat;
		}
		// otherwise if the AI is chasing something, find which one is closer
		else{
			if(AITurn){
				ballPath = myBall.getPath(false);

				Debug.Log("GOT NEW PATHS");
			}
			Debug.Log("current itemPos: " + myItem.transform.position);
			bothPossible();
		}

		Debug.Log("--------------------------");
		Debug.Log("position: " + transform.position);
		Debug.Log("velocity: " + myVel);
		Debug.Log("dest: "+ destination);
		//Debug.Log("first BallPath[0]: "+ ballPath[0]);
		Debug.Log("frameHitBall: "+ frameHitBall);
		Debug.Log("--------------------------");
	}

	//------------------------------------------- Finds frame from Position to Item to Ball
	void bothPossible(){
		// FIND DISTANCE TO ITEM
		int framesToItem;
		// This vector leads to the desired point with the current distance
		Vector3 itemHeading = myItem.transform.position - transform.position;
		// Now we normalize the vector to get direction
		Vector3 itemDir = itemHeading/itemHeading.magnitude;
		//find frames to get to vector
		framesToItem = Mathf.RoundToInt(itemHeading.x/(itemDir.x*speed*Time.deltaTime));
		Debug.Log("frames to item : " + framesToItem);

		// FIND DISTANCE TO BALL
		int firstBallFrame = frameCanHitBall();

		if(firstBallFrame == -1){
			Debug.Log("ERROR: Ball can never be hit");
			return;
		}

		//Check which dustance is longer, that is how many pixels we need/have until 
		//we can hit the ball
		int framesNeeded = firstBallFrame + framesToItem;

		//change the value of the amount of frames we are ahead so that we over estimate 
		// the amount of frames it takes to get to the ball if we're chasing the item
		framesAhead = 20;
		//Cannot reach the ball if we go for item
		int canHit = frameContactBall(framesNeeded, myItem.transform.position);
		//change them back so we don't overestimate normally
		framesAhead = 8;
		if(canHit == -1){

			Debug.Log("start frame: " + firstBallFrame + " position at that point: " + ballPath[firstBallFrame]);
			firstBallFrame = frameContactBall(firstBallFrame, transform.position);
			Debug.Log("Can't reach item");
			//Check if its possible to meet the ball
			if(firstBallFrame == -1){
				destination = transform.position;
				Debug.Log("1dest:" + destination);
				return;
			}else{
				chasing = "ball";
				destination = ballPath[firstBallFrame];
				Debug.Log("2dest:" + destination);
				return;
			}
		}

		//If they're both possible, head to the item
		Debug.Log("can reach item at point : " + ballPath[canHit]);
		Debug.Log("dest:" + myItem.transform.position);
		chasing = "item";
		myVel = itemDir;
		destination = myItem.transform.position;
	}

	//------------------------------------------- Finds first frame ball can be hit
	int frameCanHitBall(){
		for(int i = 0; i < ballPath.Count; i++){
			Debug.Log("no: " + ballPath[i] + "-" + i);
			if(ballPath[i].x < -limit){
				Debug.Log("yes: " + ballPath[i] + "-" + i);
				return i;
			}
		}
		return -1;
	}

	//------------------------------------------- Finds frame where ball and paddle should contact
	int frameContactBall(int frame, Vector3 start){
		// In this scenerio we have checked everypoint and we dont 
		// have enough time to make contact
		if((frame + framesAhead) >= ballPath.Count){
			myVel = Vector2.zero;
			return -1;
		}
		//Calculate y padding depending on current size
		aiLimitPad = (myBounds.size.y/2f) * transform.localScale.y;

		//Get the nect point we're testing and add our padding
		Vector3 ballIntersect = ballPath[frame+framesAhead];
		//filter the top bound
		if(ballIntersect.y > (yLimit - aiLimitPad)){
			Debug.Log("y was edited TOP - top limit: " + (yLimit - aiLimitPad) + " ball y pos:" + ballIntersect.y);
			Debug.Log("Old ball position: " + ballIntersect);
			ballIntersect.y = yLimit; //-= (ballIntersect.y - (yLimit - aiLimitPad));
			Debug.Log("New ball position: " + ballIntersect);
		}
		//or filter the bottom bound
		if(ballIntersect.y < (-yLimit + aiLimitPad)){
			Debug.Log("y was edited BOT - bot limit: " + (-yLimit + aiLimitPad) + " ball y pos:" + ballIntersect.y);
			Debug.Log("Old ball position: " + ballIntersect);
			ballIntersect.y = -yLimit; //+= -(ballIntersect.y - (-yLimit + aiLimitPad));
			Debug.Log("New ball position: " + ballIntersect);
		}

		// This vector leads to the desired point with the current distance
		Vector3 heading = ballIntersect - start;
		// Now we normalize the vector to get direction
		Vector3 direction = heading/heading.magnitude;

		// Total vector distance / distance travled per frame
		int framesToReachDest = Mathf.RoundToInt(heading.x / (direction.x*speed*Time.deltaTime));
		Debug.Log("framesToReachDest: " + framesToReachDest + " direction: " + direction);
		//See if paddle can arrive in less frame to the position than the ball
		if(frame > framesToReachDest){
			myVel = direction;
			ballPath[frame+framesAhead] = ballIntersect;
			Debug.Log("YES GOOD Frame: " + frame + " framesToReach:" + framesToReachDest + " point at Frame(dest): " + ballPath[frame + framesAhead]);
			return frame + framesAhead;
		}
			
		Debug.Log("NO GOOD Frame: " + frame + " framesToReach:" + framesToReachDest + " point at Frame(dest): " + ballPath[frame + framesAhead]);

		return frameContactBall(frame+1, start);
	}

	// Public methods
	public void alertItem(GameObject theItem){
		Debug.Log("ALERT: item");
		myItem = theItem;

		if(myItem == null){
			stopMoving();
			resetLookingVars();
		}
	}

	public void alertTurn(bool turn, string temp){
		Debug.Log("ALERT: my turn - " + turn + " collidedWith:" + temp);
		AITurn = turn;

		if(!AITurn){
			stopMoving();
			resetLookingVars();
		}else{
			//so that we recaulculate position in case it collided with another object
			ballHit = false;
		}
	}
}
