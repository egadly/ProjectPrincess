using UnityEngine;
using System.Collections;

public class Princess: Character{

	public enum PrincessStates { Idle, Run, Jump, Fall, Crouch, Land, Brace, WallJump, Hitstun, Reel, Rise, Pirouette, Spinend, Dive, Stomp, Death };
	public PrincessStates currentState;
	public PrincessStates nextState;
	public int counterState;

	public int check = (int)PrincessStates.Death;

	public int keys;

	private Vector3 startPosition;

	private int counterShake;
	private int counterInvulnerable;

	public Object[] hitbox = new Object[3];
	public Menu m;

	//public optionsScript os;

	// Use this for initialization
	void Start () {
		GameObject panel = GameObject.FindGameObjectWithTag ("Panel");
		if ( panel ) os = panel.GetComponent<Options>();
		GameObject hudinstance = GameObject.FindGameObjectWithTag ("HUD");
		if ( hudinstance ) hud = hudinstance.GetComponent<HUD>();
		startPosition = transform.position;

		audioSources = gameObject.GetComponents<AudioSource>();
		currentVolume = globalVolume;
		changeVolume (currentVolume);

		position = transform.position;

		aerialDrift = 0.0046875f;
		runAcceleration = 0.025f;

		gravity = 0.0125f;
		maxVspeed = .16f;
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
		if ( ( os==null || !os.isPaused ) && ( hud==null || !hud.dialogActive ) ) {
			rigidBody.WakeUp ();
			if (Input.GetKeyDown (KeyCode.Q)) {
				health = 20;
				position = startPosition;
			}

			if (currentState == nextState)
				counterState++;
			else {
				counterState = 0;
				currentState = nextState;
				destroyChildren ();
				gameObject.GetComponent<Animator> ().speed = 1;
			}

			if ( currentState != PrincessStates.Hitstun&&currentState != PrincessStates.Reel&&currentState!=PrincessStates.Rise ) counterInvulnerable = Mathf.Max (--counterInvulnerable, 0);
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
				stateWallJump ();
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
			case PrincessStates.Dive:
				stateDive ();
				break;
			case PrincessStates.Stomp:
				stateStomp();
				break;
			case PrincessStates.Death:
				stateDeath ();
				break;
			}

			if (justLanded) {
				audioSources [1].Play ();
				Instantiate (particles [0], position, Quaternion.identity);
			}

			collectCollisionCheck ();
			if (currentState != PrincessStates.Hitstun && currentState != PrincessStates.Reel && currentState != PrincessStates.Pirouette && currentState != PrincessStates.Death && currentState != PrincessStates.Dive&& currentState != PrincessStates.Stomp) {
				enemyCollisionCheck ();
				hazardCollisionCheck ();
			}

			if (counterInvulnerable != 0)
				spriteRenderer.color = new Color (1f, 1f, 1f, .5f);
			else
				spriteRenderer.color = Color.white;
			spriteRenderer.flipX = !rightDir;
			if (counterShake == 0)
				transform.position = position;
			else
				transform.position = new Vector3 (position.x + Random.Range (-.25f, .25f), position.y + Random.Range (-.25f, .25f), position.z);
			if (currentVolume != globalVolume) {
				currentVolume = globalVolume;
				changeVolume (currentVolume);
			}
		}
	}

	//Begin Common Functions
	void enemyCollisionCheck() {
		if (counterInvulnerable == 0) {
			
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));
			if ( other == null ) other = ifCollision (1 << LayerMask.NameToLayer ("EnemyHitboxes"));
			if (other != null) {
				counterInvulnerable = 60;
				health -= other.gameObject.GetComponent<Enemy> ().collisionDamage;
				if (health > 0)
					audioSources [3].Play ();
				else
					audioSources [4].Play ();
				if (other.gameObject.transform.position.x >= position.x) {
					Instantiate (particles [1], new Vector3 (other.gameObject.transform.position.x - other.bounds.size.x / 2f, position.y, -1f), Quaternion.identity);
					rightDir = true;
					velocity.x = -0.1f;
				} else {
					Instantiate (particles [1], new Vector3 (other.gameObject.transform.position.x + other.bounds.size.x / 2f, position.y, -1f), Quaternion.identity);
					rightDir = false;
					velocity.x = 0.1f;
				}
				if (platformBelow) {
					velocity.y = 0;
					if (health <= 0) {
						velocity.y = 0.1f;
						nextState = PrincessStates.Reel;
					}
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
				counterInvulnerable = 60;
				Instantiate (particles [1], new Vector3 (position.x, position.y, -1f), Quaternion.identity);
				health -= 1;
				if (health > 0)
					audioSources [3].Play ();
				else
					audioSources [4].Play ();
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
			audioSources [5].Play ();
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

		if ( counterState == 0 && velocity.x != 0 ) Instantiate (particles [0], position, Quaternion.identity);
		
		physAdjust ();
		if (!platformBelow)
			nextState = PrincessStates.Fall;
		if ( VirtualInput.leftDown || VirtualInput.rightDown ) {
			Instantiate (particles [0], position, Quaternion.identity);
			nextState = PrincessStates.Run;
		}
		if ( VirtualInput.kickPos )
			nextState = PrincessStates.Pirouette;
		if ( VirtualInput.jumpPos )
			nextState = PrincessStates.Crouch;
		if ( VirtualInput.leapPos )
			nextState = PrincessStates.Dive;

	}

	void stateRun() {
		bool prevDir = rightDir;
		if ( VirtualInput.leftDown && VirtualInput.rightDown ) {
			if (velocity.x > 0)
				rightDir = true;
			if (velocity.x < 0)
				rightDir = false;
		} else if ( VirtualInput.rightDown ) {
			applyAcceleration (platformBelow, true);
			rightDir = true;
		} else if ( VirtualInput.leftDown ) {
			applyAcceleration (platformBelow, false);
			rightDir = false;
		}
		if ( prevDir != rightDir )  Instantiate (particles [0], position, Quaternion.identity);

		if ((counterState % 36) == 12||(counterState % 36) == 30||counterState==0)
			audioSources [1].Play ();
		
		physAdjust ();

		if ( !VirtualInput.rightDown && !VirtualInput.leftDown ) {
			nextState = PrincessStates.Idle;
		}
		if ( VirtualInput.kickPos ) {
			nextState = PrincessStates.Pirouette;
		}
		if ( VirtualInput.leapPos)
			nextState = PrincessStates.Dive;
		if ( VirtualInput.jumpPos ){
			nextState = PrincessStates.Crouch;
		}


		if (!platformBelow)
			nextState = PrincessStates.Fall;

				
	}

	void stateJump() {

		if (counterState == 0)
			audioSources [0].Play ();

		if (counterState <= 9 && VirtualInput.jumpDown )
			velocity.y = maxVspeed;
		else {
			if ( VirtualInput.jumpNeg ) {
				counterState = 11;
			}
			velocity.y -= gravity;
		}

		if ( VirtualInput.rightDown )
			applyAcceleration (platformBelow, true);
		if ( VirtualInput.leftDown )
			applyAcceleration (platformBelow, false);

		physAdjust ();

		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;
	
		if ( VirtualInput.kickPos ) 
			nextState = PrincessStates.Pirouette;
		if ( counterState >= 6 && VirtualInput.leapPos) 
			nextState = PrincessStates.Stomp;
		if ( VirtualInput.jumpPos && ((platformRight)||(platformLeft)) )
			nextState = PrincessStates.Brace;

	}

	void stateFall() {

		velocity.y -= gravity;
		if (VirtualInput.downPos)
			velocity.y = -maxVspeed;
		if ( VirtualInput.rightDown )
			applyAcceleration (platformBelow, true);
		if ( VirtualInput.leftDown )
			applyAcceleration (platformBelow, false);

		
		physAdjust ();

		if ( VirtualInput.kickPos )
			nextState = PrincessStates.Pirouette;

		if ( VirtualInput.leapPos )
			nextState = PrincessStates.Stomp;

		if ( VirtualInput.jumpPos && ((platformRight)||(platformLeft)) )
			nextState = PrincessStates.Brace;

		if (platformBelow) {
			if (health == 0)
				nextState = PrincessStates.Death;
			else 
				nextState = PrincessStates.Land;
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

		float width = gameObject.GetComponent < BoxCollider2D> ().size.x / 2f;
		if (counterState == 0)
			velocity.y = 0;

		physAdjust ();
		if (counterState == 6) {
			velocity.y = maxVspeed;
			nextState = PrincessStates.WallJump;
			if (platformRight) {
				Instantiate (particles [0], new Vector3( position.x - (.5f - width), position.y, -2f ), Quaternion.Euler (new Vector3 (0f, 0f, 90f)));
				rightDir = false;
				velocity.x = -0.1f;
			} else {
				Instantiate (particles [0], new Vector3( position.x+ (.5f - width), position.y, -2f ), Quaternion.Euler (new Vector3 (0f, 0f, -90f)));
				rightDir = true;
				velocity.x = 0.1f;
			}
		}
	}

	void stateWallJump() {

		if (counterState == 0) {
			velocity.y = 0.2f;
			audioSources [0].Play ();
		}

		velocity.y -= gravity;

		if ( VirtualInput.rightDown)
			applyAcceleration (platformBelow, true);
		if ( VirtualInput.leftDown)
			applyAcceleration (platformBelow, false);

		physAdjust ();

		if ( VirtualInput.kickPos )
			nextState = PrincessStates.Pirouette;

		if (VirtualInput.leapPos) {
			nextState = PrincessStates.Stomp;
		}

		if (VirtualInput.jumpPos && ((platformRight)||(platformLeft)) )
			nextState = PrincessStates.Brace;

		if (velocity.y <= 0)
			nextState = PrincessStates.Fall;
		
	}

	void stateHitstun() {

		if ( counterState == 0 ) Instantiate (particles [0], position, Quaternion.identity);

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

		if ( VirtualInput.rightDown )
			applyAcceleration (null, true);
		if ( VirtualInput.leftDown )
			applyAcceleration (null, false);

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

		if (counterState == 0) {
			Instantiate (particles [0], position, Quaternion.identity);
			GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameCamera> ().counterShake = 6;
		}

		if (counterState > 5) 
			applyFriction (platformBelow);

		physAdjust ();

		if (!platformBelow) {
			nextState = PrincessStates.Fall;
		}

		if (counterState == 22 ) {
			nextState = PrincessStates.Idle;
		}
	}

	void statePirouette () {

		if (counterState == 0) {
			Instantiate (hitbox[0], transform);
			velocity.y = Mathf.Min (velocity.y/2f, 0);
		}

		if ( VirtualInput.rightDown )
			applyAcceleration (null, true );
		if ( VirtualInput.leftDown )
			applyAcceleration (null, false);

		if (counterState > 5 && counterState < 48 && VirtualInput.kickPos) {
			if (platformBelow)
				velocity.y += gravity * 2f;
			else
				velocity.y += gravity;
		}

		if (!platformBelow)
			velocity.y -= gravity / 8f;
		else if (counterState == 6 || (counterState % 6 == 0 && velocity.x != 0)) {
			audioSources [1].Play ();
			Instantiate (particles [0], position, Quaternion.identity);
		}

		if (counterState % 12 == 0) {
			audioSources [2].Play ();
		}

		physAdjust ();

		if (counterState >= 36)
			enemyCollisionCheck ();
		hazardCollisionCheck ();

		if ( justLanded || counterState >= 42) {
			nextState = PrincessStates.Spinend;
		}

		if ( counterState > 2 && !platformBelow && VirtualInput.jumpPos && ((platformRight)||(platformLeft)) )
			nextState = PrincessStates.Brace;
	
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

	void stateDive() {

		int direction = 1;
		if (!rightDir)
			direction *= -1;

		if (counterState == 0) {
			Instantiate (hitbox[1], transform);
			velocity.x = maxHspeed * direction;
			velocity.y = .05f;
		}
		if ( counterState >= 24 ) applyFriction ( platformBelow );

		if (!platformBelow)
			velocity.y -= gravity;
		else if ( counterState%6 == 0 && counterState < 24 ) Instantiate (particles [0], position, Quaternion.identity);


		physAdjust ();

		if (counterState >= 24) {
			enemyCollisionCheck ();
			hazardCollisionCheck ();
			if ( !platformBelow )
				nextState = PrincessStates.Fall;
		}
		if (counterState == 35) {
			nextState = PrincessStates.Idle;
		}
	}

	void stateStomp() {

		counterInvulnerable = 0;
		velocity.y = -maxVspeed;


		if (counterState==0) {
			audioSources [2].Play ();
			Instantiate (hitbox [2], this.transform);
		}

		physAdjust ();

		hazardCollisionCheck ();
		if (counterState >= 4)
			enemyCollisionCheck ();

		if (platformBelow) {
			audioSources [3].Play ();
			nextState = PrincessStates.Rise;
		}


	}

	void stateDeath() {

		applyFriction (platformBelow);

		physAdjust ();

		if (health > 0)
			nextState = PrincessStates.Rise;
		if (!platformBelow)
			nextState = PrincessStates.Reel;
	}

	void destroyChildren() {
		Object[] children = GameObject.FindGameObjectsWithTag ("PlayerHitbox");
		for (int i = 0; i < children.GetLength(0) ; i++) {
			Destroy (children [i]);
		}
	}
		
}
