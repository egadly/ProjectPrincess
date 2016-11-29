using UnityEngine;
using System.Collections;

public class Statue : Enemy {

	public int counterHit;
	public int counterShake;
	public int counterLife;
	public Princess thePrincess;
	public GameCamera theCamera;

	public GameObject projectile;
	public GameObject master;

	// Use this for initialization
	void Start () {
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();

		GameObject panel = GameObject.FindGameObjectWithTag ("Panel");
		if ( panel ) os = panel.GetComponent<Options>();
		GameObject hudinstance = GameObject.FindGameObjectWithTag ("HUD");
		if ( hudinstance ) hud = hudinstance.GetComponent<HUD>();

		audioSources = gameObject.GetComponents<AudioSource>();
		currentVolume = globalVolume;
		changeVolume (currentVolume);

		thePrincess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();
		theCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameCamera> ();
		collisionDamage = 0;
		position = transform.position;
		maxHspeed = .5f;
		maxVspeed = 0.2f;
		runAcceleration = 0.1625f;
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((os == null || !os.isPaused) && (hud == null || !hud.dialogActive)) {
			rigidBody.WakeUp ();

			counterShake = Mathf.Max (--counterShake, 0);
			counterLife = (counterLife + 1) % 120;
			if (velocity.x == 0)
				counterHit = Mathf.Max (--counterHit, 0);

			if (master != null && master.GetComponent<Switch> ().isActive) {
				if (counterLife == 0) {
					audioSources [0].Play ();
					Instantiate (particles [1], new Vector3 (position.x, position.y, 0), Quaternion.identity);
					if (rightDir) {
						Instantiate (particles [1], new Vector3 (position.x + .5f, position.y, 0), Quaternion.identity);
						Object temp = Instantiate (projectile, new Vector3 (position.x + .5f, position.y , 0), Quaternion.identity);
						((GameObject)temp).GetComponent<Hazard> ().velocity = new Vector3 (.2f, 0, 0);
					} else {
						Instantiate (particles [1], new Vector3 (position.x - .5f, position.y , 0), Quaternion.identity);
						Object temp = Instantiate (projectile, new Vector3 (position.x - .5f, position.y , 0), Quaternion.identity);
						((GameObject)temp).GetComponent<Hazard> ().velocity = new Vector3 (-.2f, 0, 0);
					}

					/*Object temp = Instantiate (projectile, new Vector3 (position.x - .5f, position.y - .25f, 0), Quaternion.identity);
					if (!rightDir)
						((GameObject)temp).GetComponent<Hazard> ().velocity = new Vector3 (.2f, 0, 0);
					else ((GameObject)temp).GetComponent<Hazard> ().velocity = new Vector3 (-.2f, 0, 0);*/
				}
			}

			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("Player"));

			if (other) {
				if (thePrincess.position.x < position.x)
					thePrincess.velocity.x = Mathf.Min (thePrincess.velocity.x, -.025f);
				else
					thePrincess.velocity.x = Mathf.Max (thePrincess.velocity.x, .025f);
			}
			
			other = ifCollision (1 << LayerMask.NameToLayer ("Enemies"));
			if (other && other.gameObject != gameObject) {
				if (other.gameObject.GetComponent<Enemy> ().position.x < position.x)
					other.gameObject.GetComponent<Enemy> ().velocity.x = Mathf.Min (other.gameObject.GetComponent<Enemy> ().velocity.x, -.025f);
				else
					other.gameObject.GetComponent<Enemy> ().velocity.x = Mathf.Max (other.gameObject.GetComponent<Enemy> ().velocity.x, .025f);
			}

			other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
			if (counterHit == 0 && other) {
				audioSources [1].Play ();
				if (other.gameObject.GetComponent<Hitbox> ().damage == 0) {
					if (other.transform.position.x > position.x) {
						if (platformLeft) velocity.x += runAcceleration/1f;
						else velocity.x -= runAcceleration;
					}
					if (other.transform.position.x < position.x) {
						if (platformRight) velocity.x -= runAcceleration/1f;
						else velocity.x += runAcceleration;
					}
				} else {
					rightDir = !rightDir;
					velocity.y += .05f;
				}
				counterShake = 6;
				counterHit = 12;
				counterLife = 1;
			}

			applyFriction (platformBelow);
			if (!platformBelow)
				velocity.y -= 0.0125f;
			else {
				if (velocity.x != 0 && counterLife % 3 == 0) {
					//audioSources [1].Play ();
					Instantiate (particles [0], new Vector3 (position.x, position.y - .125f, -2f), Quaternion.identity);
				}
			}
			physAdjust ();

			if (justLanded) {
				audioSources [0].Play ();
				theCamera.counterShake = 6;
				counterShake = 6;
				Instantiate (particles [0], new Vector3 (position.x - .5f, position.y - .125f, -2f), Quaternion.identity);
				Instantiate (particles [0], new Vector3 (position.x, position.y - .125f, -2f), Quaternion.identity);
				Instantiate (particles [0], new Vector3 (position.x + .5f, position.y - .125f, -2f), Quaternion.identity);
			}

			gameObject.GetComponent<SpriteRenderer> ().flipX = rightDir;
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
