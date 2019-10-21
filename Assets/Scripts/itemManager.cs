using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ITEMS
 * 1: Fast Ball
 * 2: Big Paddles
 * 3: Invisible Ball
 * 4: Multi Ball
 * 5: Opponent Small paddle
*/
public class itemManager : MonoBehaviour {
	public Transform playerTrans;
	public Transform opponentTrans;
	public ball myBall;
	public GameObject bClones;
	public Image[] opIcons;
	public Image[] plIcons;
	GameObject[] ballClones;
	Vector2 xLimits;
	Vector2 yLimits;
	bool isPlayer;
	float[] items;
	bool hasItem;

	// extra vars
	int scaleMultiplier;
	int smallScaleMulti;


	// Use this for initialization
	void Start () {
		if(gameObject.tag == "Player"){
			isPlayer = true;
		}else{
			isPlayer = false;
		}

		foreach(Image i in plIcons){
			Color tempCol = i.color;
			tempCol.a = 0f;
			i.color = tempCol;
		}
		foreach(Image i in opIcons){
			Color tempCol = i.color;
			tempCol.a = 0f;
			i.color = tempCol;
		}

		smallScaleMulti = 0;
		scaleMultiplier = 0;
		ballClones = new GameObject[4];
		xLimits = new Vector2(-8f, 8f);
		yLimits = new Vector2(-4.5f, 4.5f);

		items = new float[5];
		items[0] = 5f;
		items[1] = 5f;
		items[2] = 5f;
		items[3] = 5f;
		items[4] = 5f;
	}
	
	// Update is called once per frame
	void Update () {
		// Take Care of item
		if(hasItem){
			manageItem();
		}
	}

	void removeItem(int code){

		if(isPlayer){
			Color tempCol = plIcons[code].color;
			tempCol.a = 0f;
			plIcons[code].color = tempCol;
		}else{
			Color tempCol = opIcons[code].color;
			tempCol.a = 0f;
			opIcons[code].color = tempCol;
		}

		switch(code){
		case 0:
			if(isPlayer){
				myBall.fastBall(false, 0);
			}else{
				myBall.fastBall(false, 1);
			}
			break;
		case 1:
			transform.localScale -= new Vector3(0f, scaleMultiplier*.1f, 0f);
			scaleMultiplier = 0;
			break;
		case 2:
			myBall.setInvisible(false);
			break;
		case 3:
			Destroy(ballClones[0]);
			Destroy(ballClones[1]);
			Destroy(ballClones[2]);
			Destroy(ballClones[3]);
			break;
		case 4:
			if(isPlayer){
				opponentTrans.localScale += new Vector3(0f, smallScaleMulti*.05f, 0f);
			}else{
				playerTrans.localScale += new Vector3(0f, smallScaleMulti*.05f, 0f);
			}
			smallScaleMulti = 0;
			break;
		default: 
			Debug.Log("No Item Code to add");
			break;
		}
	}

	void manageItem(){
		hasItem = false;

		for(int i = 0; i < items.Length; i++){
			if(items[i] < 5f){
				items[i] +=Time.deltaTime;
				hasItem = true;

			} else if(items[i] >= 5f){
				removeItem(i);
			}
		}
	}

	// Public methods
	public void removeAll(){
		hasItem = false;

		for(int i = 0; i < items.Length; i++){
			items[i] = 5f;
			removeItem(i);
		}
	}

	public void addItem(int code){
		if(isPlayer){
			Color tempCol = plIcons[code].color;
			tempCol.a = 0.25f;
			plIcons[code].color = tempCol;
		}else{
			Color tempCol = opIcons[code].color;
			tempCol.a = 0.25f;
			opIcons[code].color = tempCol;
		}

		hasItem = true;
		items[code] = 0f;

		switch(code){
		case 0:
			if(isPlayer){
				myBall.fastBall(true, 0);
			}else{
				myBall.fastBall(true, 1);
			}

			break;
		case 1:
			transform.localScale += new Vector3(0f, .1f, 0f);
			scaleMultiplier += 1; 
			break;
		case 2:
			myBall.setInvisible(true);
			break;
		case 3:
			if(ballClones[0] != null){
				return;
			}

			ballClones[0] = Instantiate(bClones, new Vector3(Random.Range(xLimits.x, xLimits.y), Random.Range(yLimits.x, yLimits.y), 0f), Quaternion.identity);
			ballClones[1] = Instantiate(bClones, new Vector3(Random.Range(xLimits.x, xLimits.y), Random.Range(yLimits.x, yLimits.y), 0f), Quaternion.identity);
			ballClones[2] = Instantiate(bClones, new Vector3(Random.Range(xLimits.x, xLimits.y), Random.Range(yLimits.x, yLimits.y), 0f), Quaternion.identity);
			ballClones[3] = Instantiate(bClones, new Vector3(Random.Range(xLimits.x, xLimits.y), Random.Range(yLimits.x, yLimits.y), 0f), Quaternion.identity);
			break;
		case 4:
			if(isPlayer){
				opponentTrans.localScale -= new Vector3(0f, .05f, 0f);
			}else{
				playerTrans.localScale -= new Vector3(0f, .05f, 0f);
			}
			smallScaleMulti += 1;
			break;
		default: 
			Debug.Log("No Item Code to add");
			break;
		}
	}
}
