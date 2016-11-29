 using UnityEngine;
using System.Collections;

public class Gate : Enemy {

	public GameCamera theCamera;
	public Princess thePrincess;

	public bool isRising = true;
	public bool startRising = true;
	public GameObject master;
	public int counterLife;
	public int counterCycle;
	public int startCycle = 360;
	int counterShake = 0;
	public int offset;
	public int directionKnockback;

	// Use this for initialization
	void Start () {

		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		GameObject panel = GameObject.FindGameObjectWithTag ("Panel");
		if ( panel ) os = panel.GetComponent<Options>();
		GameObject hudinstance = GameObject.FindGameObjectWithTag ("HUD");
		if ( hudinstance ) hud = hudinstance.GetComponent<HUD>();
		theCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameCamera> ();
		thePrincess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();
		counterCycle = startCycle;

		audioSources = gameObject.GetComponents<AudioSource>();
		currentVolume = globalVolume;
		changeVolume (currentVolume);
		
		counterLife = 0;
		gravity = 0.0125f;
		position = transform.position;
		maxVspeed = .2f;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (( os==null || !os.isPaused ) && ( hud==null || !hud.dialogActive )) {
			rigidBody.WakeUp ();
			counterLife = (counterLife + 1) % counterCycle;
			counterShake = Mathf.Max (--counterShake, 0);

			if (master == null) {
				if (counterLife == 0) {
					counterShake = 6;
					isRising = !isRising;
				}
				if (counterLife != 0 && velocity.y == 0 && Mathf.Abs (position.x - thePrincess.position.x) < 1f && isRising) {
					counterLife = 0;
					isRising = false;
				}
			} else {
				if (master.GetComponent<Switch> ().isActive)
					isRising = startRising;
				else
					isRising = !startRising;
			}

			if (isRising) {
				velocity.y += .05f;
			} else
				velocity.y -= .05f;

			velocity.y = Mathf.Min (velocity.y, 0.05f);

			if (velocity.y < -.05f)
				collisionDamage = 3;
			else
				collisionDamage = 0;

			velocity.x = 0;
			physAdjust ();

			if (justLanded) { 
				theCamera.counterShake = 6;
				counterShake = 6;
				Instantiate (particles [0], new Vector3 (position.x - .5f, position.y - .5f, -2f), Quaternion.identity);
				Instantiate (particles [0], new Vector3 (position.x, position.y - .5f, -2f), Quaternion.identity);
				Instantiate (particles [0], new Vector3 (position.x + .5f, position.y - .5f, -2f), Quaternion.identity);
				audioSources [0].Play ();
			}


			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Player"));

			if (other) {
				//BoxCollider2D col = gameObject.GetComponent < BoxCollider2D > ();
				/*if (directionKnockback == 0) {
					if (thePrincess.position.x < position.x)
						thePrincess.position.x = position.x - col.size.x / 2f - thePrincess.gameObject.GetComponent<BoxCollider2D> ().size.x / 2f;
					else
						thePrincess.position.x = position.x + col.size.x / 2f + thePrincess.gameObject.GetComponent<BoxCollider2D> ().size.x / 2f;
				} else
					thePrincess.position.x = position.x + (directionKnockback * col.size.x / 2f) + (directionKnockback * thePrincess.gameObject.GetComponent<BoxCollider2D> ().size.x / 2f);*/
				if (directionKnockback == 0) {
					if (thePrincess.position.x < position.x)
						thePrincess.velocity.x = -.025f;
					else
						thePrincess.velocity.x = .025f;
				} else
					thePrincess.velocity.x = directionKnockback*.025f;
				//thePrincess.transform.position = thePrincess.position;

			}

			gameObject.layer = LayerMask.NameToLayer ("Default");
			other = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));
			if (other == null) {
				other = ifCollision (1 << LayerMask.NameToLayer ("EnemyHitboxes"));
			}
			if (other && other.gameObject != gameObject) {
				if (other.gameObject.GetComponent<Enemy> ().position.x < position.x)
					other.gameObject.GetComponent<Enemy> ().velocity.x = -.025f;
				else
					other.gameObject.GetComponent<Enemy> ().velocity.x = .025f;
			}
			gameObject.layer = LayerMask.NameToLayer ("Enemies");


			if (counterLife == 0)
				counterCycle = Mathf.Min (startCycle, counterCycle + 5);
			other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
			if ( other ) {
				if (counterLife % 10 == 0) {
					audioSources [1].Play ();
					if (other.gameObject.GetComponent<Hitbox> ().damage > 0){
						if (thePrincess.position.x > position.x)
							Instantiate (particles [1], new Vector3 (position.x + gameObject.GetComponent<BoxCollider2D> ().size.x / 2f, thePrincess.position.y, -1f), Quaternion.identity);
						else
							Instantiate (particles [1], new Vector3 (position.x - gameObject.GetComponent<BoxCollider2D> ().size.x / 2f, thePrincess.position.y, -1f), Quaternion.identity);
					}
				}
				counterShake = 1;
				counterCycle = Mathf.Max (counterCycle - 1, 30);
			} 

			if (counterShake == 0)
				transform.position = position;
			else
				transform.position = new Vector3 (position.x + Random.Range (-.25f, .25f), position.y, position.z);
			if (currentVolume != globalVolume) {
				currentVolume = globalVolume;
				changeVolume (currentVolume);
			}
		}
	}
}
