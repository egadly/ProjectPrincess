using UnityEngine;
using System.Collections;

public class Switch : Interact {

	public bool isActive = false;
	public bool prevActive = false;
	public bool curActive = false;
	public bool isToggle = false;
	public bool isTimer;
	public int lengthTimer;
	public int counterTimer;
	public bool playerActive = true;
	public bool enemyActive = true;
	public bool hitboxActive = false;
	public bool ehitboxActive = false;

	public Sprite activeSprite;
	public Sprite idleSprite;
	public SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		position = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!isToggle) {
			isActive = false;
			if (playerActive && ifCollision( 1 << LayerMask.NameToLayer("Player")) )
				isActive = true;
			if (enemyActive && ifCollision( 1 << LayerMask.NameToLayer("Enemies")) )
				isActive = true;
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
			if (hitboxActive && other) {
				if ( other.gameObject.GetComponent<Hitbox>().damage > 1 ) curActive = true;
			}
			if (ehitboxActive && ifCollision( 1 << LayerMask.NameToLayer("EnemyHitboxes")) )
				isActive = true;
		} else {

			counterTimer = Mathf.Max (counterTimer - 1, 0);
			prevActive = curActive;

			curActive = false;
			if (playerActive && ifCollision( 1 << LayerMask.NameToLayer("Player")) )
				curActive = true;
			if (enemyActive && ifCollision( 1 << LayerMask.NameToLayer("Enemies")) )
				curActive = true;
			Collider2D other = ifCollision (1 << LayerMask.NameToLayer ("PlayerHitboxes"));
			if (hitboxActive && other) {
				if ( other.gameObject.GetComponent<Hitbox>().damage > 1 ) curActive = true;
			}
			if (ehitboxActive && ifCollision( 1 << LayerMask.NameToLayer("EnemyHitboxes")) )
				curActive = true;

			if (isTimer && counterTimer == 0)
				isActive = false;

			if (!prevActive && curActive) {
				isActive = !isActive;
				if (isActive && isTimer)
					counterTimer = lengthTimer;
			}

			if (hitboxActive) {
				if (isActive)
					spriteRenderer.sprite = activeSprite;
				else
					spriteRenderer.sprite = idleSprite;
			} else {
				if (curActive)
					spriteRenderer.sprite = activeSprite;
				else
					spriteRenderer.sprite = idleSprite;
			}
			
		}


	
	}
}
