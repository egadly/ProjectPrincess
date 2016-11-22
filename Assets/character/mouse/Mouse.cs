using UnityEngine;
using System.Collections;

public class Mouse : Enemy {

	public enum MouseStates { Idle, Run, Fall, Hitstun, Dead };
	public MouseStates currentState;
	MouseStates nextState;
	public int counterState;
	public int stateLength;
	public int minLength;

	private int counterShake;



	// Use this for initialization
	void Start () {
		GameObject panel = GameObject.FindGameObjectWithTag ("Panel");
		if ( panel ) os = panel.GetComponent<optionsScript>();
		GameObject hudinstance = GameObject.FindGameObjectWithTag ("HUD");
		if ( hudinstance ) hud = hudinstance.GetComponent<HUD>();

		position = transform.position;

		gravity = 0.0125f;
		maxHspeed = 0.1f;
		maxVspeed = 0.1f;

		runAcceleration = 0.0125f;
		aerialDrift = 0;
		rightDir = false;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();

		currentState = MouseStates.Idle;
		stateLength = (int)Random.Range (minLength, 60);
		minLength = 15;

	
	}
	
	// Update is called once per frame
	void Update () {
		if (( os==null || !os.isPaused ) && ( hud==null || !hud.dialogActive )) {
			if (currentState == nextState)
				counterState++;
			else {
				counterState = 0;
				currentState = nextState;
			}

			if (counterState == 0) {
				stateLength = (int)Random.Range (minLength, 60);
				minLength = 15;
			}
			gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);

			switch (currentState) {
			case MouseStates.Idle:
				stateIdle ();
				break;
			case MouseStates.Run:
				stateRun ();
				break;
			case MouseStates.Fall:
				stateFall ();
				break;
			case MouseStates.Hitstun:
				stateHitstun ();
				break;
			case MouseStates.Dead:
				stateDead ();
				break;
			}

			if (justLanded)
				Instantiate (particles [0], position, Quaternion.identity);

			//Test Hits
			if (currentState != MouseStates.Hitstun && currentState != MouseStates.Dead)
				enemyCollisionCheck ();
			if (currentState != MouseStates.Fall && health <= 0)
				nextState = MouseStates.Dead;
			spriteRenderer.flipX = !rightDir;

			if (counterShake == 0)
				transform.position = position;
			else
				transform.position = new Vector3 (position.x + Random.Range (-.25f, .25f), position.y + Random.Range (-.25f, .25f), position.z); 	
		}
	}

	void enemyCollisionCheck() {
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
			if (other != null) {
				health -= other.gameObject.GetComponent<Hitbox> ().damage;
				if (other.gameObject.transform.position.x >= position.x) {
					Instantiate (particles [1], new Vector3 (other.gameObject.transform.position.x - other.bounds.size.x / 2f, position.y, -1f), Quaternion.identity);
					rightDir = true;
					velocity.x = -maxHspeed;
				} else {
					Instantiate (particles [1], new Vector3 (other.gameObject.transform.position.x + other.bounds.size.x / 2f, position.y, -1f), Quaternion.identity);
					rightDir = false;
					velocity.x = maxHspeed;
				}
			if ( health<= 0 ) velocity.y = 1f;
			nextState = MouseStates.Hitstun;
			}
	}

	void stateIdle() {
		if (counterState == stateLength) {
			Instantiate (particles [0], position, Quaternion.identity);
			nextState = MouseStates.Run;
			if (platformLeft)
				rightDir = true;
			else if (platformRight)
				rightDir = false;
			else {
				if ((((int)Random.Range (0, 99)) % 2) == 0)
					rightDir = true;
				else
					rightDir = false;
			}
		}

		applyFriction (platformBelow);

		physAdjust ();

		if ( !platformBelow ) nextState = MouseStates.Fall;
	}

	void stateRun() {

		if (counterState == stateLength) {
			nextState = MouseStates.Idle;
		}

		applyAcceleration (platformBelow, rightDir);

		if (rightDir) {
			if (platformRight)
				nextState = MouseStates.Idle;
		} else {
			if (platformLeft)
				nextState = MouseStates.Idle;
		}
		physAdjust ();

		if ( !platformBelow ) nextState = MouseStates.Fall;
	}

	void stateFall() {
		velocity.y -= gravity;

		physAdjust ();

		if (platformBelow != null)
			nextState = MouseStates.Idle;
	}

	void stateHitstun() {
		if ( counterState == 0 ) Instantiate (particles [0], position, Quaternion.identity);
		if (counterState < 6) {
			counterShake = 1;
			gameObject.GetComponent<Animator> ().speed = 0;
		} else {
			counterShake = 0;
			gameObject.GetComponent<Animator> ().speed = 1;
		}

		if (!platformBelow) velocity.y -= gravity;
		applyFriction (platformBelow);
		physAdjust ();

		if (counterState >= 15) {
			minLength = 45;
			nextState = MouseStates.Idle;
		}
		if (!platformBelow)
			nextState = MouseStates.Fall;
	}

	void stateDead() {
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;

		if (!platformBelow) velocity.y -= gravity;
		applyFriction (platformBelow);
		physAdjust ();
	}
}
