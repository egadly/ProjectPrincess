using UnityEngine;
using System.Collections;

public class Princess: Character{

	float gravity = 0.025f;
	float maxVspeed = 1f;

	public enum PrincessStates { Idle, Run, Jump, Fall, Crouch, Land };
	public PrincessStates currentState;
	public PrincessStates nextState;
	public int counterState;

	public bool rightDir;

	public SpriteRenderer rend;

	// Use this for initialization
	void Start () {
		position = transform.position;
		currentState = PrincessStates.Idle;
		rightDir = true;
		rend = gameObject.GetComponent<SpriteRenderer> ();
		/*Application.targetFrameRate = 5;
		QualitySettings.vSyncCount = 0;*/

	}
	
	// Update is called once per frame
	void Update () {
			if (currentState == nextState)
				counterState++;
			else {
				counterState = 0;
				currentState = nextState;
			}

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
			}
			
			rend.flipX = !rightDir;
			transform.position = position;
	}

	/*void physicAdjust() {

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

		isGrounded = Physics2D.OverlapCircle (Utilities.Vec2 (position.x, position.y - col.size.y / 2f), 0.05f, platforms);
	}*/

	//Begin State Functions
	void stateIdle() {
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);
		if (velocity.x > 0 || velocity.x < 0 )
			velocity.x /= 2f;
		physicAdjust ();
		if (!isGrounded)
			nextState = PrincessStates.Fall;
		if (Input.GetKey (KeyCode.J))
			nextState = PrincessStates.Crouch;
		if (Input.GetKey (KeyCode.D)||Input.GetKey (KeyCode.A))
			nextState = PrincessStates.Run;
	}

	void stateRun() {
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);
		if (Input.GetKey (KeyCode.D)) {
			rightDir = true;
			velocity.x += 0.1f;
		}
		if (Input.GetKey (KeyCode.A)) {
			rightDir = false;
			velocity.x -= 0.1f;
		}
		if (velocity.x > 0.2f)
			velocity.x = 0.2f;
		if (velocity.x < -0.2f)
			velocity.x = -0.2f;
		physicAdjust ();
		if (Input.GetKeyUp (KeyCode.D)||Input.GetKeyUp (KeyCode.A)) {
			nextState = PrincessStates.Idle;
		} else
		if (Input.GetKeyDown(KeyCode.J) ){
			nextState = PrincessStates.Jump;
			}
		if (!isGrounded)
			nextState = PrincessStates.Fall;
				
	}

	void stateJump() {
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);
		if (counterState <= 10 && Input.GetKey (KeyCode.J))
			velocity.y = 0.2f;
		else {
			if (Input.GetKeyUp (KeyCode.J)) {
				counterState = 11;
			} else if (counterState == 0) {
				velocity.y = 0.2f;
			}
			velocity.y -= gravity;
		}
		physicAdjust ();
		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;
	}

	void stateFall() {
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);
		velocity.y -= gravity;
		if (velocity.y <= -0.2f)
			velocity.y = -0.2f;
		physicAdjust ();
		if (isGrounded) nextState = PrincessStates.Land; 
	}

	void stateCrouch() {
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);
		physicAdjust ();
		if (counterState == 4) {
			nextState = PrincessStates.Jump; //Change this to Jump when Finalizing
		}
		if (!isGrounded)
			nextState = PrincessStates.Fall;
	}

	void stateLand() {
		gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);
		physicAdjust ();
		if (counterState == 3)
			nextState = PrincessStates.Idle;
	}

	void OnDrawGizmos() {
		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (Utilities.Vec3 (position.x - col.size.x/2f +col.size.x/4f, position.y + col.size.y/2f, 1f), .1f);
	}
}
