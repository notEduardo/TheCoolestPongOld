using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class gameManager : MonoBehaviour {
	// Public Variables
	public ball myBall;
	public Button aiGameBtn;
	public Button noAIGameBtn;
	public Text title;
	public Text instructions;
	public Text opPoints;
	public Text plPoints;
	public Text winner;
	public playerInput opControl;
	public ai opAI;
	public myAI plAI;
	public playerInput plControl;
	public GameObject[] items;

	public itemManager playerCont;
	public itemManager opponentCont;

	// Game Managing variables
	int pntsToWin;
	int opPointsCnt;
	int plPointsCnt;
	bool gameWon;
	public bool gameRunning;
	bool aiGame;

	// Item Variables
	float itemTimer;
	float itemLastTimeSpawned;
	float itemTimeToSpawn;
	bool itemSpawned;
	GameObject itemOne;
	GameObject itemTwo;
	Vector2 xLimits;
	Vector2 yLimits;

	//AI ONLY
	public bool isAIOnlyGame;

	void Start () {
		// Game Variables
		gameRunning = false;
		aiGame = false;
		gameWon = false;
		opPointsCnt = 0;
		plPointsCnt = 0;
		pntsToWin = 5;

		opPoints.text = opPointsCnt.ToString();
		plPoints.text = plPointsCnt.ToString();

		instructions.gameObject.SetActive(false);
		title.gameObject.SetActive(true);
		aiGameBtn.gameObject.SetActive(true);
		noAIGameBtn.gameObject.SetActive(true);

		// Item Variables
		itemTimer = 0f;
		itemTimeToSpawn = 3;
		itemLastTimeSpawned = 0;
		xLimits = new Vector2(2f, 9.7f);
		yLimits = new Vector2(-4.5f, 4.5f);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Mouse1)){
			Debug.Break();
		}


		if(gameRunning){
			itemTimer += Time.deltaTime;
			if(itemTimer > itemLastTimeSpawned+itemTimeToSpawn && itemTimer%itemTimeToSpawn <= .1f){
				itemLastTimeSpawned = itemTimer;
				spawnItems();
			}
		}

		if(!gameRunning && 
			Input.GetKeyDown(KeyCode.Space)){
			if(gameWon){
				resetGame();
			}else{
				startGame(aiGame);
			}
		}
		else if(gameRunning == false && 
			Input.GetKeyDown(KeyCode.N)){
			resetGame();
		}

		if(myBall.transform.position.x > 10f){
			opPointsCnt += 1;
			opPoints.text = opPointsCnt.ToString();

			if(opPointsCnt == pntsToWin){
				if(aiGame){
					endGame(true, "I Win", Color.red);
				}else{
					endGame(true, "Player 2 Wins", Color.red);
				}
			}else{
				endGame(false, "", Color.red);
			}

		}
		else if(myBall.transform.position.x < -10f){
			plPointsCnt += 1;
			plPoints.text = plPointsCnt.ToString();

			if(plPointsCnt == pntsToWin){
				//for AI v AI
				if(isAIOnlyGame){
					endGame(true, "TheCoolestAI Wins", Color.red);
				}
				//for player v AI
				else{
					endGame(true, "Player Wins!", Color.blue);
				}
			}else{
				endGame(false, "", Color.blue);
			}
		}

	}


	public void setGameType(bool gameTyp){
		aiGame = gameTyp;
		startGame(aiGame);
	}

	void startGame(bool ai){
		gameRunning = true;
		myBall.startMoving();

		if(ai){
			//for player v AI
			opAI.enabled = true;
			opControl.enabled = false;

			//for AI v AI
			if(isAIOnlyGame){
				plAI.enabled = true;
				plControl.enabled = false;
			}else{
				plAI.enabled = false;
				plControl.enabled = true;
			}
		}
		else{
			opAI.enabled = false;
			opControl.enabled = true;
		}

		instructions.gameObject.SetActive(false);
		title.gameObject.SetActive(false);
		aiGameBtn.gameObject.SetActive(false);
		noAIGameBtn.gameObject.SetActive(false);


	}

	void endGame(bool victory, string msg, Color textColor){
		gameRunning = false;
		myBall.resetBall();

		if(victory){
			winner.gameObject.SetActive(true);
			winner.text = msg;
			winner.color = textColor;

			gameWon = true;
		}

		instructions.gameObject.SetActive(true);
		title.gameObject.SetActive(false);
		aiGameBtn.gameObject.SetActive(false);
		noAIGameBtn.gameObject.SetActive(false);

		playerCont.removeAll();
		opponentCont.removeAll();

		//Delete all items
		GameObject[] itemsALive = GameObject.FindGameObjectsWithTag("item");
		foreach(GameObject i in itemsALive){
			Destroy(i);
		}
	}

	void resetGame(){
		gameRunning = false;
		myBall.resetBall();

		gameWon = false;
		opPointsCnt = 0;
		plPointsCnt = 0;
		opPoints.text = opPointsCnt.ToString();
		plPoints.text = plPointsCnt.ToString();

		winner.text = "";
		winner.gameObject.SetActive(false);


		instructions.gameObject.SetActive(false);
		title.gameObject.SetActive(true);
		aiGameBtn.gameObject.SetActive(true);
		noAIGameBtn.gameObject.SetActive(true);

		playerCont.removeAll();
		opponentCont.removeAll();

		//Delete all items
		GameObject[] itemsALive = GameObject.FindGameObjectsWithTag("item");
		foreach(GameObject i in itemsALive){
			Destroy(i);
		}
	}

	void spawnItems(){
		int item1 = Random.Range(0, items.Length);
		int item2 = Random.Range(0, items.Length);

		itemOne = Instantiate(items[item1], new Vector2(Random.Range(xLimits.x, xLimits.y), Random.Range(yLimits.x, yLimits.y)), Quaternion.identity);
		itemTwo = Instantiate(items[item2], new Vector2(-(Random.Range(xLimits.x, xLimits.y)), Random.Range(yLimits.x, yLimits.y)), Quaternion.identity);

		if(opAI.enabled){
			opAI.alertItem(itemTwo);
		}
		if(isAIOnlyGame && plAI.enabled){
			plAI.alertItem(itemOne);
		}
	}

	public void giveItem(bool player, int code){
		Destroy(itemOne);
		Destroy(itemTwo);

		//Tell the Ai that the items are no longer alive
		if(opAI.enabled){
			opAI.alertItem(null);
		}
		if(isAIOnlyGame && plAI.enabled){
			plAI.alertItem(null);
		}

		// Give the item to whoever won it
		if(player){
			playerCont.addItem(code);
		}
		else{
			opponentCont.addItem(code);
		}
	}
}
