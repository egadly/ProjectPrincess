using UnityEngine;
using System.Collections;

public class Princess: Character{

	public enum PrincessStates { Idle, Run, Jump, Fall, Crouch, Land, Brace, WallJump, Hitstun, Reel, Rise, Pirouette, Spinend, Death };
	public PrincessStates currentState;
	public PrincessStates nextState;
	public int counterState;

	public int check = (int)PrincessStates.Death;

	public int keys;

	private Vector3 startPosition;

	private int counterShake;
	private int counterInvulnerable;

	public Object hitbox;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;


		health = 3;
		position = transform.position;

		aerialDrift = 0.0046875f;
		runAcceleration = 0.025f;

		gravity = 0.0125f;
		maxVspeed = .2f;
		maxHspeed = .1f;

		currentState = PrincessStates.Idle;
		rightDir = true;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;

	}


	// Update is called once per frame
	void Update () {

		rigidBody.WakeUp ();

		if (Input.GetKeyDown (KeyCode.Q))
			position = startPosition;

		if (currentState == nextState)
			counterState++;
		else {
			counterState = 0;
			currentState = nextState;
			gameObject.GetComponent<Animator> ().speed = 1;
			destroyChildren ();
		}

		counterInvulnerable = Mathf.Max (--counterInvulnerable, 0);
		counterShake = Mathf.Max (--counterShake, 0);

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
		case PrincessStates.Reel:
			stateReel ();
			break;
		case PrincessStates.Rise:
			stateRise ();
			break;
		case PrincessStates.Pirouette:
			statePirouette ();
			break;
		case PrincessStates.Spinend:
			stateSpinend ();
			break;
		case PrincessStates.Death:
			stateDeath ();
			break;
		}

		collectCollisionCheck ();
		if ( currentState != PrincessStates.Hitstun && currentState != PrincessStates.Reel && currentState != PrincessStates.Rise && currentState != PrincessStates.Pirouette && currentState != PrincessStates.Death ) {
			enemyCollisionCheck ();
			hazardCollisionCheck ();
		}

		if (counterInvulnerable!=0)
			spriteRenderer.color = new Color( 1f, 1f, 1f, .5f);
		else
			spriteRenderer.color = Color.white;
		spriteRenderer.flipX = !rightDir;
		if (counterShake == 0)
			transform.position = position;
		else
			transform.position = Utilities.Vec3 (position.x + Random.Range (-.25f, .25f), position.y + Random.Range (-.25f, .25f), position.z); 

	}

	//Begin Common Functions
	void enemyCollisionCheck() {
		if (counterInvulnerable == 0) {
			
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));
			if (other != null) {
				health -= other.gameObject.GetComponent<Enemy> ().collisionDamage;
				if (other.gameObject.transform.position.x >= position.x) {
					rightDir = true;
					velocity.x = -0.1f;
				} else {
					rightDir = false;
					velocity.x = 0.1f;
				}
				if (platformBelow) {
					velocity.y = 0;
					if (health <= 0)
						nextState = PrincessStates.Reel;
					else
						nextState = PrincessStates.Hitstun;
				}
				else {
					velocity.y = 0.1f;
					nextState = PrincessStates.Reel;
				}
			}
		}
	}

	void hazardCollisionCheck() {
		if (counterInvulnerable == 0) {
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Hazards"));
			if (other != null) {
				health -= 1;
				if (rightDir) {
					velocity.x = -0.1f;
				} else {
					velocity.x = 0.1f;
				}
				velocity.y = 0.1f;
				nextState = PrincessStates.Reel;
			}
		}
	}

	void collectCollisionCheck() {
		Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Collects"));
		if (other != null) {
			switch (other.gameObject.tag) {
			case "Key":
				keys++;
				break;
			case "Heart":
				health++;
				break;
			default:
				break;
			}
			Destroy (other.gameObject);
		}
	}

	//Begin State Functions
	void stateIdle() {
		
		applyFriction (platformBelow);
		
		physAdjust ();
		if (!platformBelow)
			nextState = PrincessStates.Fall;
		if (Input.GetKey (KeyCode.J))
			nextState = PrincessStates.Crouch;
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A)) {
			rightDir = Input.GetKey (KeyCode.D);
			nextState = PrincessStates.Run;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			nextState = PrincessStates.Pirouette;
		}

	}

	void stateRun() {

		if (Input.GetKey (KeyCode.D))
			applyAcceleration (platformBelow, true);
		if (Input.GetKey (KeyCode.A))
			applyAcceleration (platformBelow, false);

		if (velocity.x>0) rightDir = true;
		if (velocity.x<0) rightDir = false;

		velocity.x = Mathf.Min( maxHspeed, velocity.x );
		velocity.x = Mathf.Max( -maxHspeed, velocity.x );

		
		physAdjust ();
		if (!Input.GetKey (KeyCode.D)&&!Input.GetKey (KeyCode.A)) {
			nextState = PrincessStates.Idle;
		} else
		if (Input.GetKeyDown(KeyCode.J) ){
			nextState = PrincessStates.Crouch;
			}
		if (!platformBelow)
			nextState = PrincessStates.Fall;

				
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
			applyAcceleration (platformBelow, true);
		if (Input.GetKey (KeyCode.A))
			applyAcceleration (platformBelow, false);

		if (Input.GetKeyDown( KeyCode.J ) ){
			if (((platformRight) || (platformLeft)))
				nextState = PrincessStates.Brace;
		}

		physAdjust ();

		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;

		if (Input.GetKey (KeyCode.K)) {
			nextState = PrincessStates.Pirouette;
		}

	}

	void stateFall() {

		velocity.y -= gravity;

		if (Input.GetKey (KeyCode.D))
			applyAcceleration (platformBelow, true);
		if (Input.GetKey (KeyCode.A))
			applyAcceleration (platformBelow, false);

		if (Input.GetKeyDown( KeyCode.J ) && ((platformRight)||(platformLeft)) ){
			nextState = PrincessStates.Brace;
		}
		
		physAdjust ();

		if (platformBelow) nextState = PrincessStates.Land; 

		if (Input.GetKey (KeyCode.K)) {
			nextState = PrincessStates.Pirouette;
		}


	}

	void stateCrouch() {

		physAdjust ();
		if (counterState == 5) {
			velocity.y = maxVspeed;
			nextState = PrincessStates.Jump;
		}
		if (!platformBelow)
			nextState = PrincessStates.Jump;
	}

	void stateLand() {

		applyFriction (platformBelow);

		physAdjust ();

		if (counterState == 5)
			nextState = PrincessStates.Idle;

	}

	void stateBrace() {
		if (counterState == 0)
			velocity.y = 0;

		physAdjust ();
		if (counterState == 6) {
			velocity.y = maxVspeed;
			nextState = PrincessStates.WallJump;
			if (platformRight) {
				rightDir = false;
				velocity.x = -0.1f;
			} else {
				rightDir = true;
				velocity.x = 0.1f;
			}
		}
	}

	void stateWallJump() {

		if (counterState == 0) {
			velocity.y = 0.2f;
		}

		velocity.y -= gravity;

		if (Input.GetKey (KeyCode.D))
			applyAcceleration (platformBelow, true);
		if (Input.GetKey (KeyCode.A))
			applyAcceleration (platformBelow, false);

		physAdjust ();

		if (Input.GetKeyDown( KeyCode.J ) && ((platformRight)||(platformLeft)) ){
			nextState = PrincessStates.Brace;
		}
		if (Input.GetKey (KeyCode.K)) {
			nextState = PrincessStates.Pirouette;
		}

		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;
		
	}

	void stateHitstun() {

		if (counterState > 6) {
			physAdjust ();
			counterShake = 0;
		} else
			counterShake = 1;

		if (counterState == 15) {
			nextState = PrincessStates.Idle;
			counterInvulnerable = 60;
		}
		if (!platformBelow) {
			nextState = PrincessStates.Fall;
			counterInvulnerable = 60;
		}
	}

	void stateReel() {
		
		if (counterState > 6) {
			gameObject.GetComponent<Animator> ().speed = 1;
			velocity.y -= gravity;
			physAdjust ();
			counterShake = 0;
		} else {
			gameObject.GetComponent<Animator> ().speed = 0;
			counterShake = 1;
		}

		if (Input.GetKey (KeyCode.D))
			velocity.x += 0.00625f;
		if (Input.GetKey (KeyCode.A))
			velocity.x -= 0.00625f;

		if (counterState == 107) {
			nextState = PrincessStates.Fall;
			counterInvulnerable = 60;
		}
		if (counterState > 6 && platformBelow && velocity.y <= 0) {
			if (health <= 0)
				nextState = PrincessStates.Death;
			else nextState = PrincessStates.Rise;
		}

	}

	void stateRise() {

		if (counterState > 5) {
			applyFriction (platformBelow);
		}
		physAdjust ();

		if (counterState == 22 ) {
			nextState = PrincessStates.Idle;
			counterInvulnerable = 60;
		}
	}

	void statePirouette () {

		if (counterState == 0) {
			Instantiate ( hitbox, transform);
			velocity.y = Mathf.Min (velocity.y/2f, 0);
		}

		if (Input.GetKey (KeyCode.D))
			applyAcceleration (null, true );
		if (Input.GetKey (KeyCode.A))
			applyAcceleration (null, false);

		if (counterState > 5 && counterState < 48 && Input.GetKeyDown (KeyCode.K))
			velocity.y += gravity;

		if (!platformBelow) velocity.y -= gravity /8f;

		physAdjust ();

		hazardCollisionCheck ();

		if (platformBelow && velocity.y < 0f)
			nextState = PrincessStates.Spinend;

		if (counterState == 42) {
			nextState = PrincessStates.Spinend;
		}
	
	}

	void stateSpinend() {

		if (platformBelow)
			applyFriction (platformBelow);
		else
			velocity.y -= gravity;

		physAdjust ();

		if (counterState == 12) {
			if (platformBelow)
				nextState = PrincessStates.Land;
			else
				nextState = PrincessStates.Fall;
		}
	}

	void stateDeath() {

		applyFriction (platformBelow);

		physAdjust ();
	}

	void destroyChildren() {
		Object[] children = GameObject.FindGameObjectsWithTag ("PlayerHitbox");
		for (int i = 0; i < children.GetLength(0) ; i++) {
			Destroy (children [i]);
		}
	}
		
}
