using UnityEngine;
using System.Collections;

public class Gate : Enemy {

	public GameCamera theCamera;

	public bool isRising = true;
	public bool startRising = true;
	public GameObject master;
	public int counterLife;
	public int counterCycle;
	public int startCycle = 360;
	int counterShake = 0;
	public int offset;

	// Use this for initialization
	void Start () {

		theCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameCamera> ();
		counterCycle = startCycle;
		
		counterLife = 0;
		gravity = 0.0125f;
		collisionDamage = 0;
		position = transform.position;
		maxVspeed = .2f;
	
	}
	
	// Update is called once per frame
	void Update () {
		counterLife = (counterLife + 1) % counterCycle;
		counterShake = Mathf.Max (--counterShake, 0);

		if (master == null) {
			if (counterLife == 0)
				isRising = !isRising;
		} else {
			if (master.GetComponent<Switch> ().isActive)
				isRising = startRising;
			else
				isRising = !startRising;
		}

		if (isRising) {
			velocity.y += .05f;
		}
		else
			velocity.y -= .05f;

		velocity.y = Mathf.Min (velocity.y, 0.05f);

		physAdjust ();

		if (justLanded) { 
			theCamera.counterShake = 6;
			counterShake = 6;
			Instantiate (particles [0], new Vector3 (position.x - .5f, position.y - .5f, -2f), Quaternion.identity);
			Instantiate (particles [0], new Vector3 (position.x, position.y - .5f, -2f), Quaternion.identity);
			Instantiate (particles [0], new Vector3 (position.x + .5f, position.y - .5f, -2f), Quaternion.identity);
		}


		Collider2D other = ifCollision (1<<LayerMask.NameToLayer ("Player"));

		if ( other ) {
			Princess princess = other.gameObject.GetComponent<Princess> ();
			BoxCollider2D col = gameObject.GetComponent < BoxCollider2D >();
			if (princess.position.x < position.x)
				princess.position.x = position.x - col.size.x / 2f - princess.gameObject.GetComponent<BoxCollider2D> ().size.x / 2f;
			else princess.position.x = position.x + col.size.x / 2f + princess.gameObject.GetComponent<BoxCollider2D> ().size.x / 2f;
			princess.transform.position = princess.position;
			//if (princess.position.x < position.x)
			//	princess.velocity.x -= .05f;
			//else
			//	princess.velocity.x += 0.05f;
		}

		if (counterLife == 0)
			counterCycle = Mathf.Min (startCycle, counterCycle + 5);
		if (ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"))) {
			counterShake = 1;
			counterCycle = Mathf.Max (counterCycle - 1, 30);
		} 

		if ( counterShake == 0 )
			transform.position = position;
		else
			transform.position = new Vector3(position.x + Random.Range (-.25f, .25f), position.y, position.z); 
	}
}
