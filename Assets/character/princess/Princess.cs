using UnityEngine;
using System.Collections;

public class Princess: Character{

	public enum PrincessStates { Idle, Run, Jump, Fall, Crouch, Land, Brace, WallJump, Hitstun };
	public PrincessStates currentState;
	public PrincessStates nextState;
	public int counterState;

	public int counterShake;

	public int counterInvulnerable;

	public Collider2D col;

	public bool rightDir;

	// Use this for initialization
	void Start () {
		position = transform.position;

		gravity = 0.0125f;
		maxVspeed = .2f;
		maxHspeed = .1f;

		currentState = PrincessStates.Idle;
		rightDir = true;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		/*Application.targetFrameRate = 5;
		QualitySettings.vSyncCount = 0;*/

	}
	
	// Update is called once per frame
	void Update () {

		rigidBody.WakeUp ();
		
		if (currentState == nextState)
			counterState++;
		else {
			counterState = 0;
			currentState = nextState;
		}
		counterInvulnerable = Mathf.Max (--counterInvulnerable, 0);
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);

		switch (currentState) {
		case PrincessStates.Idle:
			stateIdle ();
			break;
		case PrincessStates.Run:
			stateRun ();
			break;
		case PrincessStates.Jump:
			stateJump ();
			break;
		case PrincessStates.Fall:
			stateFall ();
			break;
		case PrincessStates.Crouch:
			stateCrouch ();
			break;
		case PrincessStates.Land:
			stateLand ();
			break;
		case PrincessStates.Brace:
			stateBrace ();
			break;
		case PrincessStates.WallJump:
			stateWallJump();
			break;
		case PrincessStates.Hitstun:
			stateHitstun ();
			break;
		}


		//col = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));

		if (counterInvulnerable!=0)
			spriteRenderer.color = Color.red;
		else
			spriteRenderer.color = Color.white;
		spriteRenderer.flipX = !rightDir;
		if (counterShake == 0)
			transform.position = position;
		else
			transform.position = Utilities.Vec3 (position.x + Random.Range (-0.1f, 0.1f), position.y + Random.Range (-0.1f, 0.1f), position.z); 
	
	}

	//Begin Common Functions
	void enemyCollisionCheck() {
		if (counterInvulnerable == 0) {
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));
			if (other != null) {
				if (other.gameObject.transform.position.x >= position.x)
					velocity.x = -0.1f;
				else
					velocity.x = 0.1f;
				velocity.y = 0;
				nextState = PrincessStates.Hitstun;
			}
		}
	}

	//Begin State Functions
	void stateIdle() {
		
		if (velocity.x > 0)
			velocity.x = Mathf.Max (velocity.x - .0125f, 0f);
		if (velocity.x < 0)
			velocity.x = Mathf.Min (velocity.x + .0125f, 0f);
		
		physAdjust ();
		if (!platformBelow)
			nextState = PrincessStates.Fall;
		if (Input.GetKey (KeyCode.J))
			nextState = PrincessStates.Crouch;
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A)) {
			rightDir = Input.GetKey (KeyCode.D);
			nextState = PrincessStates.Run;
		}
		enemyCollisionCheck ();
	}

	void stateRun() {

		if (Input.GetKey (KeyCode.D))
			velocity.x += 0.025f;
		if (Input.GetKey (KeyCode.A))
			velocity.x -= 0.025f;

		if (velocity.x>0) rightDir = true;
		if (velocity.x<0) rightDir = false;

		velocity.x = Mathf.Min( maxHspeed, velocity.x );
		velocity.x = Mathf.Max( -maxHspeed, velocity.x );

		
		physAdjust ();
		if (Input.GetKeyUp (KeyCode.D)||Input.GetKeyUp (KeyCode.A)) {
			nextState = PrincessStates.Idle;
		} else
		if (Input.GetKeyDown(KeyCode.J) ){
			nextState = PrincessStates.Crouch;
			}
		if (!platformBelow)
			nextState = PrincessStates.Fall;
		enemyCollisionCheck ();
				
	}

	void stateJump() {

		if (counterState <= 5 && Input.GetKey (KeyCode.J))
			velocity.y = maxVspeed;
		else {
			if (Input.GetKeyUp (KeyCode.J)) {
				counterState = 11;
			}
			velocity.y -= gravity;
		}

		if (Input.GetKey (KeyCode.D))
			velocity.x += 0.003125f;
		if (Input.GetKey (KeyCode.A))
			velocity.x -= 0.003125f;

		if (Input.GetKeyDown( KeyCode.J ) && ((platformRight)||(platformLeft)) ){
			nextState = PrincessStates.Brace;
		}

		physAdjust ();
		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;
	}

	void stateFall() {

		velocity.y -= gravity;

		if (Input.GetKey (KeyCode.D))
			velocity.x += 0.003125f;
		if (Input.GetKey (KeyCode.A))
			velocity.x -= 0.003125f;

		if (Input.GetKeyDown( KeyCode.J ) && ((platformRight)||(platformLeft)) ){
			nextState = PrincessStates.Brace;
		}
		
		physAdjust ();

		if (platformBelow) nextState = PrincessStates.Land; 
	}

	void stateCrouch() {

		physAdjust ();
		if (counterState == 5) {
			velocity.y = maxVspeed;
			nextState = PrincessStates.Jump;
		}
		if (!platformBelow)
			nextState = PrincessStates.Jump;
		enemyCollisionCheck ();
	}

	void stateLand() {

		if (velocity.x > 0)
			velocity.x = Mathf.Max (velocity.x - .0125f, 0f);
		if (velocity.x < 0)
			velocity.x = Mathf.Min (velocity.x + .0125f, 0f);

		physAdjust ();

		if (counterState == 5)
			nextState = PrincessStates.Idle;
		enemyCollisionCheck ();
	}

	void stateBrace() {
		if (counterState == 0)
			velocity.y = 0;

		physAdjust ();
		if (counterState == 6) {
			velocity.y = maxVspeed;
			nextState = PrincessStates.WallJump;
		}
	}

	void stateWallJump() {

		if (counterState == 0) {
			velocity.y = 0.4f;
			if (platformRight) {
				rightDir = false;
				velocity.x = -0.1f;
			} else {
				rightDir = true;
				velocity.x = 0.1f;
			}
		}

		velocity.y -= gravity;

		if (Input.GetKey (KeyCode.D))
			velocity.x += 0.003125f;
		if (Input.GetKey (KeyCode.A))
			velocity.x -= 0.003125f;

		if (Input.GetKeyDown( KeyCode.J ) && ((platformRight)||(platformLeft)) ){
			nextState = PrincessStates.Brace;
		}

		physAdjust ();
		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;
		
	}

	void stateHitstun() {

		counterInvulnerable = 1;

		if (counterState > 6) {
			physAdjust ();
			counterShake = 0;
		}
		else
			counterShake = 1;

		if (counterState == 15) {
			nextState = PrincessStates.Idle;
			counterInvulnerable = 60;
		}
	}

		
}
