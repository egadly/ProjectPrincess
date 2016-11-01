using UnityEngine;
using System.Collections;

public class Princess: Character{

	public enum PrincessStates { Idle, Run, Jump, Fall, Crouch, Land, Brace, WallJump };
	public PrincessStates currentState;
	public PrincessStates nextState;
	public int counterState;

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
		}


		//col = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));

		if ( col != null ) {
			velocity.y = 0.2f;
		}
			
		spriteRenderer.flipX = !rightDir;
		transform.position = position;
	
	}

	/*void physAdjust() {

		position += velocity;

		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();
		LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");

		Collider2D other;
		//Bottom Collider Check
		other = Physics2D.OverlapArea( Utilities.Vec2( position.x - .05f, position.y), Utilities.Vec2( position.x + .05f, position.y- col.size.y/2f), platforms);
		if (other!= null) {
			position.y = other.transform.position.y + 1f;
			velocity.y = 0;
		}
		//Top Collider Check
		other = Physics2D.OverlapArea( Utilities.Vec2( position.x - .05f, position.y + col.size.y/2f), Utilities.Vec2( position.x + .05f, position.y), platforms);
		if (other!= null) {
			position.y = other.transform.position.y - 1f;
			velocity.y = 0;
		}
		//Right Collider Check
		other = Physics2D.OverlapArea( Utilities.Vec2( position.x, position.y + .25f), Utilities.Vec2( position.x + col.size.x/2f, position.y- .25f), platforms);
		if (other!= null) {
			position.x = other.transform.position.x - 0.75f;
			velocity.x = 0;
		}
		//Left Collider Check
		other = Physics2D.OverlapArea( Utilities.Vec2( position.x-col.size.x/2f, position.y +.25f), Utilities.Vec2( position.x , position.y-.25f), platforms);
		if (other!= null) {
			position.x = other.transform.position.x + 0.75f;
			velocity.x = 0;
		}

		platformBelow = Physics2D.OverlapCircle (Utilities.Vec2 (position.x, position.y - col.size.y / 2f), 0.05f, platforms);
	}*/

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
			nextState = PrincessStates.Fall;
	}

	void stateLand() {

		if (velocity.x > 0)
			velocity.x = Mathf.Max (velocity.x - .0125f, 0f);
		if (velocity.x < 0)
			velocity.x = Mathf.Min (velocity.x + .0125f, 0f);

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

		
}
