using UnityEngine;
using System.Collections;

public class Mouse : Enemy {

	public enum MouseStates { Idle, Run, Fall };
	public MouseStates currentState;
	MouseStates nextState;
	public int counterState;
	public int stateLength;

	// Use this for initialization
	void Start () {

		position = transform.position;

		gravity = 0.0125f;
		maxHspeed = 0.1f;
		maxVspeed = 0.1f;

		runAcceleration = 0.0125f;
		aerialDrift = 0;
		rightDir = false;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();

		currentState = MouseStates.Idle;
		stateLength = (int)Random.Range (15, 60);

	
	}
	
	// Update is called once per frame
	void Update () {

		if (currentState == nextState)
			counterState++;
		else {
			counterState = 0;
			currentState = nextState;
		}

		if ( counterState == 0 ) stateLength = (int)Random.Range (15, 60);

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
		}

		//Test Hits
		if ( ifCollision( 1 << LayerMask.NameToLayer( "PlayerHitboxes" ) ) ) Destroy( this.gameObject );

		spriteRenderer.flipX = !rightDir;
		transform.position = position;
	
	}

	void stateIdle() {
		if (counterState == stateLength) {
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
}
