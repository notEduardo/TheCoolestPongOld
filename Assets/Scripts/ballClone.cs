using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballClone : MonoBehaviour {
	public Rigidbody2D myRb;
	public float speed;
	public int startDir;
	public bool started;

	void Start () {
		myRb = GetComponent<Rigidbody2D>();
		speed = 9f;
		started = false;

	}

	// Update is called once per frame
	void Update () {
		
		if(!started){
			started = true;

			startDir = Random.Range(0, 2);
			Vector3 direction;

			if(startDir == 1){
				direction = new Vector3(1f, Random.Range(-1f, 1f), 0f);
			}
			else{
				direction = new Vector3(-1f, Random.Range(-1f, 1f), 0f);
			}

			rotateBall(direction);
			myRb.velocity = direction*speed;
		}

		if(Mathf.Abs(transform.position.x) > 9){
			float dotProd;
			Vector3 reflection;
			Vector2 normalVec = new Vector2();

			if(transform.position.x > 9f){
				normalVec = -Vector2.right;
			}
			if(transform.position.x < -9f){
				normalVec = Vector2.right;
			}
			dotProd = Vector2.Dot(normalVec, (-transform.right));
			dotProd *= 2;

			reflection = normalVec * dotProd;
			reflection += transform.right;

			rotateBall(reflection);
			myRb.velocity = reflection*speed;
		}
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
	}
}
