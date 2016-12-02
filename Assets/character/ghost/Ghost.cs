using UnityEngine;
using System.Collections;

public class Ghost : Enemy {

	public GameObject target;
	public GameObject otherTarget;
	public bool ifChase;
	public int counterChase;
	public int lengthChase = 120;
	GameObject thePrincess;

	private int counterShake;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		position = transform.position;
		maxVspeed = .025f;
		maxHspeed = .025f;
		aerialDrift = .00125f;
		ifChase = false;
		thePrincess = GameObject.FindGameObjectWithTag ("Player");

		GameObject panel = GameObject.FindGameObjectWithTag ("Panel");
		if ( panel ) os = panel.GetComponent<Options>();
		GameObject hudinstance = GameObject.FindGameObjectWithTag ("HUD");
		if ( hudinstance ) hud = hudinstance.GetComponent<HUD>();

		audioSources = gameObject.GetComponents<AudioSource>();
		currentVolume = globalVolume;
		changeVolume (currentVolume);
	}
	
	// Update is called once per frame
	void Update () {
		if ((os == null || !os.isPaused) && (hud == null || !hud.dialogActive)) {

			if (target == null)
				target = thePrincess;
			if (otherTarget == null)
				otherTarget = thePrincess;

			
			counterShake = Mathf.Max (counterShake - 1, 0);
			if (health > 0 && ifChase) {
				if (target.transform.position.x > position.x )
					rightDir = true;
				if (target.transform.position.x < position.x)
					rightDir = false;

				if (target.transform.position.x > position.x)
					velocity.x += aerialDrift;
				else
					velocity.x -= aerialDrift;
				if (target.transform.position.y > position.y)
					velocity.y += aerialDrift;
				else
					velocity.y -= aerialDrift;
				if (!ifCollision (1 << LayerMask.NameToLayer ("Platforms")))
					physAdjust ();
				else {
					//Maximum Horizontal Speed Applied
					velocity.x = Mathf.Min (maxHspeed, velocity.x);
					velocity.x = Mathf.Max (-maxHspeed, velocity.x);

					//Maximum Vertical Speed Applied
					velocity.y = Mathf.Min (maxVspeed, velocity.y);
					velocity.y = Mathf.Max (-maxVspeed, velocity.y);

					position += velocity;
				}
				spriteRenderer.color = new Color (1f, 1f, 1f, 1f);

				if (counterChase-- == 0) {
					ifChase = false;
					counterChase = lengthChase / 4;
					if (target == thePrincess&& velocity.x == 0) {
						if (rightDir)
							velocity.x = .05f;
						else
							velocity.x = .05f;
					}
				}
				Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
				if (other) {
					audioSources [1].Play ();
					Instantiate (particles [0], position, Quaternion.identity);
					if (other.transform.position.x > position.x)
						velocity.x = -maxHspeed;
					else
						velocity.x = maxHspeed;
					velocity.y *= -1;
					counterShake = 6;
					health -= other.gameObject.GetComponent<Hitbox> ().damage;
					if (health <= 0)
						//GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ().health++;
						GameObject.FindGameObjectWithTag ("Door").GetComponent<Door> ().score += 100;
					ifChase = false;
					counterChase = lengthChase;
				}
			} else {
				//physAdjust ();
				position += velocity;
				if (health > 0 ) spriteRenderer.color = new Color (1f, 1f, 1f, .5f);
				else spriteRenderer.color = new Color (1f, 1f, 1f, .25f);
				if ( health > 0 && counterChase-- == 0) {
					audioSources [0].Play ();
					ifChase = true;
					counterChase = lengthChase;
				}
			}

			gameObject.GetComponent<BoxCollider2D> ().enabled = ifChase;
			spriteRenderer.flipX = rightDir;

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
}
