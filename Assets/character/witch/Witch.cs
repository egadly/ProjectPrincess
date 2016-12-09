using UnityEngine;
using System.Collections;

public class Witch : Enemy {

	public enum WitchStates { Idle, Run, Toss, Summon, Cradle, Death, Wait, Rise };
	public WitchStates currentState;
	public WitchStates nextState;
	bool dashDance;

	int counterState;
	int counterShake;
	int counterPause;

	bool ableHit;

	public GameObject projectile;
	public GameObject summonedSoul;
	public GameObject currentGhost;
	public Princess thePrincess;
	public GameObject theKey;
	public GameObject platform;
	public GameObject[] platformsToDestroy;


	public int maxHealth;
	BoxCollider2D collider2d;
	AudioSource music;

	// Use this for initialization
	void Start () {

		health = maxHealth;
		GameObject panel = GameObject.FindGameObjectWithTag ("Panel");
		if ( panel ) os = panel.GetComponent<Options>();
		GameObject hudinstance = GameObject.FindGameObjectWithTag ("HUD");
		if ( hudinstance ) hud = hudinstance.GetComponent<HUD>();
		thePrincess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();
		collider2d = gameObject.GetComponent<BoxCollider2D> ();
		theKey = GameObject.FindGameObjectWithTag ("Key");
		music = GameObject.FindGameObjectWithTag ("Music").GetComponent<AudioSource> ();

		audioSources = gameObject.GetComponents<AudioSource>();
		currentVolume = globalVolume;
		changeVolume (currentVolume);

		position = transform.position;

		aerialDrift = 0.0046875f;
		runAcceleration = 0.05f;

		gravity = 0.0125f;
		maxVspeed = .16f;
		maxHspeed = .2f;

		currentState = WitchStates.Wait;
		nextState = currentState;

		rightDir = true;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((os == null || !os.isPaused) && (hud == null || !hud.dialogActive) && counterPause == 0) {

			gameObject.GetComponent<Animator> ().speed = 1;

			transform.position = position;
			rigidBody.WakeUp ();

			if (thePrincess.currentState == Princess.PrincessStates.Death)
				nextState = WitchStates.Idle;

			if (currentState == nextState)
				counterState++;
			else {
				counterState = 0;
				currentState = nextState;
			}

			counterShake = Mathf.Max (counterShake - 1, 0);

			gameObject.GetComponent<Animator> ().SetInteger ("State", (int)currentState);

			int stateLength;
			float relativeHealth = (float)health / (float)(maxHealth-1);
			collider2d.enabled = true;
			switch (currentState) {

			case WitchStates.Idle:/* ------------------------------------------------------------------------------------------*/
				collider2d.enabled = false;
				if (relativeHealth <= 0.4f)
					stateLength = 90;
				else
					stateLength = 180;
				if (counterState == 0) {
					ableHit = true;
					//health--;
				}
				
				applyFriction (platformBelow);

				enemyCollisionCheck ();
				if (!platformBelow)
					velocity.y -= gravity;
				
				if (counterState >= stateLength) {
					if (relativeHealth > 0.75f)
						nextState = WitchStates.Run;
					else
						nextState = WitchStates.Summon;
				}
				break;

			case WitchStates.Run:/* ------------------------------------------------------------------------------------------*/
				int dashDanceTotal, dashDanceMajor;
				if (relativeHealth > 0.5) {
					dashDanceTotal = 40;
					dashDanceMajor = 25;
				} else {
					dashDanceTotal = 60;
					dashDanceMajor = 45;
				}

				if (counterState == 0) {
					Instantiate (particles [0], position, Quaternion.identity);
					if (thePrincess.position.x > position.x)
						rightDir = true;
					else
						rightDir = false;
					dashDance = false;
				}
				if (counterState != 0 && ((dashDance && (counterState % dashDanceTotal) == 0) || (!dashDance && (counterState % dashDanceTotal) % dashDanceMajor == 0))) {
					audioSources [2].Play ();
					rightDir = !rightDir;
					dashDance = !dashDance;
				}

				if ( counterState%24 == 0 && relativeHealth <= .4 ) {
					GameObject temp = ((GameObject)Instantiate (projectile, new Vector3 (position.x + 0.1f, position.y, position.z), Quaternion.identity));
					audioSources [3].Play ();
					if (rightDir)
						temp.GetComponent<Hazard> ().velocity = new Vector3 (temp.GetComponent<Hazard> ().velocity.x * -0.5f, temp.GetComponent<Hazard> ().velocity.y, temp.GetComponent<Hazard> ().velocity.z);
					else
						temp.GetComponent<Hazard> ().velocity = new Vector3 (temp.GetComponent<Hazard> ().velocity.x * 0.5f, temp.GetComponent<Hazard> ().velocity.y, temp.GetComponent<Hazard> ().velocity.z);
					temp.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					temp.GetComponent<Hazard> ().particle = particles [2];
				}

				if (counterState % 6 == 0) {
					Instantiate (particles [2], new Vector3 (position.x, position.y, position.z), Quaternion.identity);
				}
				if (counterState%24 == 0 ) audioSources [1].Play ();
				if (counterState%3==0 &&( (velocity.x > 0 && !rightDir) || (velocity.x < 0 && rightDir)  )) Instantiate (particles [0], position, Quaternion.identity);


				
				applyAcceleration (platformBelow, rightDir);

				if ((!rightDir && platformLeft) || (rightDir && platformRight)) {
					rightDir = platformLeft;
					if (rightDir)
						velocity.x = maxVspeed/4f;
					else
						velocity.x = -maxVspeed/4f;
					velocity.y = .025f;
					nextState = WitchStates.Toss;
				}
				break;

			case WitchStates.Toss:/* ------------------------------------------------------------------------------------------*/
				if (relativeHealth > 0.75f)
					stateLength = 48;
				else {
					stateLength = 108;
					if (counterState == 0) {
						audioSources [0].Play ();
						velocity.x /= 2f;
					}
					if (health % 2 == 0 && ((counterState > 6 && counterState < 17) || (counterState > 80 && counterState < 95 && velocity.y == 0))) {
						if ( platformBelow ) audioSources [0].Play ();
						velocity.y = .2f;
					}
				}
				
				if ((counterState - 6) % 30 == 0) {
					audioSources [2].Play ();
					audioSources [3].Play ();
					GameObject temp = ((GameObject)Instantiate (projectile, new Vector3 (position.x + 0.1f, position.y, position.z), Quaternion.identity));
					if (rightDir)
						temp.GetComponent<Hazard> ().velocity = new Vector3 (temp.GetComponent<Hazard> ().velocity.x * -1, temp.GetComponent<Hazard> ().velocity.y, temp.GetComponent<Hazard> ().velocity.z);
					temp.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					temp.GetComponent<Hazard> ().particle = particles [2];
				}
				if (!platformBelow)
					velocity.y -= gravity;
				
				if (counterState > stateLength) {
					if ( relativeHealth < .5f ) nextState = WitchStates.Cradle;
					else nextState = WitchStates.Idle;
				}
				break;

			case WitchStates.Summon:/* ------------------------------------------------------------------------------------------*/
				if (counterState < 15) {
					enemyCollisionCheck ();
					collider2d.enabled = false;
				}
				if (relativeHealth > .5f)
					stateLength = 47;
				else {
					if (health == 0)
						stateLength = (48 * 20) - 1;
					else stateLength = ((maxHealth - health) * 48) - 1;
				}
				if (counterState%48 == 0) {
					Instantiate (particles [2], new Vector3 (thePrincess.position.x - .2f, position.y, position.z), Quaternion.identity);
					Instantiate (particles [2], new Vector3 (thePrincess.position.x + .2f, position.y, position.z), Quaternion.identity);
				}
				if ((counterState+18)%48 == 0) {
					if ( relativeHealth >= 0.6f) {
						currentGhost = ((GameObject)Instantiate (summonedSoul, position, Quaternion.identity));
						currentGhost.GetComponent<Ghost> ().health = 1;
						currentGhost.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					}
					audioSources [2].Play ();
					audioSources [2].Play ();
					GameObject temp = ((GameObject)Instantiate (projectile, new Vector3 (thePrincess.position.x, position.y, position.z), Quaternion.identity));
					temp.GetComponent<Hazard> ().velocity = new Vector3 (0f, .2f, 0f);
					temp.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					temp.GetComponent<Hazard> ().particle = particles [2];
				}
				if (counterState > stateLength)
					nextState = WitchStates.Run;
				break;

			case WitchStates.Cradle:/* ------------------------------------------------------------------------------------------*/
				counterShake = 1;
				if (counterState == 0) {
					if (thePrincess.position.x > position.x)
						velocity.x = (maxHspeed / 2f) + .01f;
					else
						velocity.x = (-maxHspeed / 2f) - .01f;
				}
				if (counterState % 6 == 0) {
					Instantiate (particles [2], new Vector3 (position.x - .2f, position.y, position.z), Quaternion.identity);
					Instantiate (particles [2], new Vector3 (position.x + .2f, position.y, position.z), Quaternion.identity);
				}
				if ( platformRight )  velocity.x = (-maxHspeed / 2f) - .01f;
				if ( platformLeft ) velocity.x = (maxHspeed / 2f) + .01f;
				if (position.x < .5f && position.x > -.5f) {
					if (thePrincess.position.x > position.x)
						velocity.x = (maxHspeed / 2f) + .01f;
					else
						velocity.x = (-maxHspeed / 2f) - .01f;
				}
					
				if (counterState % 6 == 0) {
					audioSources [2].Play ();
					audioSources [3].Play ();
					GameObject temp = ((GameObject)Instantiate (projectile, position, Quaternion.identity));
					temp.GetComponent<Hazard> ().velocity = new Vector3 (0f, .2f, 0f);
					temp.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					temp.GetComponent<Hazard> ().particle = particles [2];
					temp = ((GameObject)Instantiate (projectile, position + new Vector3 (0, 1f, 0), Quaternion.identity));
					temp.GetComponent<Hazard> ().velocity = new Vector3 (0f, .2f, 0f);
					temp.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					temp.GetComponent<Hazard> ().particle = particles [2];
					temp = ((GameObject)Instantiate (projectile, position + new Vector3 (0, 2f, 0), Quaternion.identity));
					temp.GetComponent<Hazard> ().velocity = new Vector3 (0f, .2f, 0f);
					temp.GetComponent<SpriteRenderer> ().color = new Color (0, 0, .5f, 1f);
					temp.GetComponent<Hazard> ().particle = particles [2];
				}

				if (relativeHealth <= .1f) {
					if (health == 0)
						stateLength = 300;
					else
						stateLength = 210;
				}
				else
					stateLength = 15 * (maxHealth - health);
				if (counterState > stateLength) {
					if (health <= 0)
						nextState = WitchStates.Death;
					else
						nextState = WitchStates.Idle;
				}
				break;

			case WitchStates.Death:/* ------------------------------------------------------------------------------------------*/
				collider2d.enabled = false;
				if (counterState == 0) {
					music.Pause ();
					Ghost[] ghostArray = GameObject.FindObjectsOfType<Ghost> ();
					if (ghostArray != null) {
						for (int i = 0; i < ghostArray.GetLength (0); i++) {
							Instantiate (particles [2], ghostArray [i].position, Quaternion.identity);
							Destroy (ghostArray [i].gameObject);
						}
					}

					theKey.transform.position = new Vector3 (0, 0, 1f);
					Instantiate (particles [2], new Vector3 (0, 0, -1f), Quaternion.identity);
				}
				position.y += .025f;
				position.x = (position.x * 95f) / 100f;
				if (counterState % 6 == 0) {
					Instantiate (particles [2], new Vector3 (position.x - .2f, position.y, position.z), Quaternion.identity);
					Instantiate (particles [2], new Vector3 (position.x + .2f, position.y, position.z), Quaternion.identity);
				}
				if (counterState < 120) {
					counterShake = 120;
					if ( counterState%15 == 0 )audioSources [3].Play ();
				} else
					counterShake = 0;
				if (counterState/6 < platformsToDestroy.GetLength(0) && platformsToDestroy [counterState / 6] != null ) {
					Instantiate (particles [2], platformsToDestroy[counterState / 6].transform.position, Quaternion.identity);
					Destroy (platformsToDestroy [counterState / 6]);
				}
				break;

			case WitchStates.Wait:/* ------------------------------------------------------------------------------------------*/
				collider2d.enabled = false;
				float arenaSize = 10f;
				if (Mathf.Abs (thePrincess.position.x - position.x) < 3f ) {

					platformsToDestroy = new GameObject[6];

					platformsToDestroy[0] = ((GameObject)Instantiate (platform, new Vector3 (-arenaSize, 0f, -1.5f), Quaternion.identity));
					Instantiate (particles [2], platformsToDestroy[0].transform.position, Quaternion.identity);
					platformsToDestroy[1] = ((GameObject)Instantiate (platform, new Vector3 (-arenaSize-1f, 0f, -1.5f), Quaternion.identity));
					Instantiate (particles [2], platformsToDestroy[1].transform.position, Quaternion.identity);
					platformsToDestroy[2] = ((GameObject)Instantiate (platform, new Vector3 (-arenaSize, 1f, -1.5f), Quaternion.identity));
					Instantiate (particles [2], platformsToDestroy[2].transform.position, Quaternion.identity);

					platformsToDestroy[3] = ((GameObject)Instantiate (platform, new Vector3 (arenaSize, 0f, -1.5f), Quaternion.identity));
					Instantiate (particles [2], platformsToDestroy[3].transform.position, Quaternion.identity);
					platformsToDestroy[4] = ((GameObject)Instantiate (platform, new Vector3 (arenaSize+1f, 0f, -1.5f), Quaternion.identity));
					Instantiate (particles [2], platformsToDestroy[4].transform.position, Quaternion.identity);
					platformsToDestroy[5] = ((GameObject)Instantiate (platform, new Vector3 (arenaSize, 1f, -1.5f), Quaternion.identity));
					Instantiate (particles [2], platformsToDestroy[5].transform.position, Quaternion.identity);

					nextState = WitchStates.Rise;
				}
				break;

			case WitchStates.Rise:/* ------------------------------------------------------------------------------------------*/
				if (counterState == 0) {
					audioSources [1].Play ();
					music.Play ();
				}
				collider2d.enabled = false;
				if (counterState >= 53) 
					nextState = WitchStates.Idle;
				break;

			}

			if ( justLanded ) {
				Instantiate (particles [0], position, Quaternion.identity);
				audioSources [4].Play ();
				audioSources [1].Play ();
			}
			enemyCollisionCheck ();
			if ( currentState != WitchStates.Death ) physAdjust ();

			spriteRenderer.flipX = !rightDir;
			if ( counterShake == 0)
				transform.position = position;
			else
				transform.position = new Vector3 (position.x + Random.Range (-.15f, .15f), position.y + Random.Range (-.15f, .15f), position.z);
			if (currentVolume != globalVolume) {
				currentVolume = globalVolume;
				changeVolume (currentVolume);
			}
		
		}
		counterPause = Mathf.Max (counterPause - 1, 0);
	}

	void enemyCollisionCheck() {
		Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
		if (other) {
			if (ableHit == true&&other.GetComponent<Hitbox> ().damage > 0) {
				audioSources [4].Play ();
				health--;
				Instantiate (particles [1], position, Quaternion.identity);
				counterPause = 6;
				ableHit = false;
			}
			counterShake = 6;
			counterPause += 6;
			gameObject.GetComponent<Animator> ().speed = 0;
		}
	}
}
