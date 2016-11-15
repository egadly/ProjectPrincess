﻿using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	public int health;

	public Vector3 position;
	public Vector3 velocity;
	protected Collider2D platformAbove;
	public Collider2D platformBelow;
	protected Collider2D platformRight;
	protected Collider2D platformLeft;

	protected float aerialDrift;
	protected float runAcceleration;
	protected float gravity;
	protected float maxVspeed;
	protected float maxHspeed;

	public bool rightDir;
	public bool justLanded;

	public SpriteRenderer spriteRenderer;
	public Rigidbody2D rigidBody;

	public GameObject[] particles = new GameObject[2];

	protected void physAdjust() {

		bool prevLanded = (bool)platformBelow;

		LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");
		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

		//Maximum Horizontal Speed Applied
		velocity.x = Mathf.Min( maxHspeed, velocity.x );
		velocity.x = Mathf.Max( -maxHspeed, velocity.x );

		//Maximum Vertical Speed Applied
		velocity.y = Mathf.Min( maxVspeed, velocity.y );
		velocity.y = Mathf.Max( -maxVspeed, velocity.y );

		position += velocity;


		if ((velocity.x * velocity.x) > (velocity.y * velocity.y)) {
			physAdjustHor (col, platforms);
			physAdjustVert (col, platforms);
		} else {
			physAdjustVert (col, platforms);
			physAdjustHor (col, platforms);
		}

		adjacentPlatformCheck ( col, platforms );

		if (platformBelow != null && !prevLanded)
			justLanded = true;
		else
			justLanded = false;

	}

	protected void physAdjustHor( BoxCollider2D col, LayerMask platforms ) {

		Vector2 topLeft = new Vector2 (position.x - col.size.x / 2f, position.y + col.size.y / 4f);
		Vector2 botRight = new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 4f);

		Collider2D other = null;
		other = Physics2D.OverlapArea (topLeft, botRight, platforms);

		if (other != null) {
			if ( other.transform.position.x > position.x )  {
				position.x = other.transform.position.x - other.bounds.extents.x - col.size.x / 2f -.05f;
				velocity.x = 0;
			} else {
				position.x = other.transform.position.x + other.bounds.extents.x + col.size.x / 2f +.05f;
				velocity.x = 0;
			}
		}

	}
	protected void physAdjustVert( BoxCollider2D col, LayerMask platforms ) {

		Vector2 topLeft = new Vector2 (position.x - col.size.x / 4f, position.y + col.size.y / 2f);
		Vector2 botRight = new Vector2 (position.x + col.size.x / 4f, position.y - col.size.y /2f);

		Collider2D other = null;
		other = Physics2D.OverlapArea (topLeft, botRight, platforms);

		if ( other != null ) {
			if (other.transform.position.y < position.y) {
				position.y = other.transform.position.y + other.bounds.size.y / 2f + col.size.y / 2f - col.offset.y;
				velocity.y = 0;
			} else {
				position.y = other.transform.position.y - other.bounds.size.y / 2f - col.size.y / 2f + col.offset.y;
				velocity.y = 0;
			}
		}
	}

	protected void adjacentPlatformCheck( BoxCollider2D col, LayerMask platforms ) {

		platformBelow = Physics2D.OverlapArea (
			new Vector2 (position.x - col.size.x / 4f, position.y - col.size.y / 2f + col.offset.y),
			new Vector2 (position.x + col.size.x / 4f, position.y - col.size.y / 2f + col.offset.y- 0.05f), platforms);
		platformRight= Physics2D.OverlapArea (
			new Vector2 (position.x + col.size.x / 2f,         position.y + col.size.y / 4f + col.offset.y),
			new Vector2 (position.x + col.size.x / 2f + 0.05f, position.y - col.size.y / 4f + col.offset.y), platforms);
		platformLeft= Physics2D.OverlapArea (
			new Vector2 (position.x - col.size.x / 2f - 0.05f, position.y + col.size.y / 4f + col.offset.y),
			new Vector2 (position.x - col.size.x / 2f,         position.y - col.size.y / 4f + col.offset.y), platforms);
		platformAbove = Physics2D.OverlapArea (
			new Vector2 (position.x - col.size.x / 4f, position.y + col.size.y / 2f + col.offset.y + 0.05f),
			new Vector2 (position.x + col.size.x / 4f, position.y + col.size.y / 2f + col.offset.y), platforms);
		
	}

	protected Collider2D ifCollision( LayerMask mask ) {

		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

		return Physics2D.OverlapArea (
			new Vector2 (position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y),
			new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), mask);
	}

	protected void applyFriction( Collider2D platform ) {
		if ( platform ) {
			float frictionC = platform.gameObject.GetComponent<Platform> ().frictionConstant;
			if (velocity.x > 0)
				velocity.x = Mathf.Max (velocity.x - (frictionC * 0.0125f), 0f);
			if (velocity.x < 0)
				velocity.x = Mathf.Min (velocity.x + (frictionC * 0.0125f), 0f);
		}
	}

	protected void applyAcceleration( Collider2D platform, bool rightDir ) {
		int direction;
		if (rightDir)
			direction = 1;
		else
			direction = -1;
		if (platform) {
			float frictionC = platform.gameObject.GetComponent<Platform> ().frictionConstant;
			velocity.x += direction * frictionC * runAcceleration;
		} else
			velocity.x += direction * aerialDrift;
	}



}
