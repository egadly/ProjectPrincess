using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	protected Vector3 position;
	public Vector3 velocity;
	protected Collider2D isGrounded;

	protected float gravity;
	protected float maxVspeed;
	protected float maxHspeed;

	public SpriteRenderer spriteRenderer;
	public Rigidbody2D rigidBody;

	protected void physAdjust() {

		//Maximum Horizontal Speed Applied
		velocity.x = Mathf.Min( maxHspeed, velocity.x );
		velocity.x = Mathf.Max( -maxHspeed, velocity.x );

		//Maximum Vertical Speed Applied
		velocity.y = Mathf.Min( maxVspeed, velocity.y );
		velocity.y = Mathf.Max( -maxVspeed, velocity.y );

		position += velocity;

		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

		if ((velocity.x * velocity.x) > (velocity.y * velocity.y)) {
			physAdjustHor (col);
			physAdjustVert (col);
		} else {
			physAdjustVert (col);
			physAdjustHor (col);
		}

		LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");
		isGrounded = Physics2D.OverlapArea (
			Utilities.Vec2 (position.x - col.size.x / 4f + 0.03f, position.y - col.size.y / 2f + col.offset.y),
			Utilities.Vec2 (position.x + col.size.y / 4f - 0.03f, position.y - col.size.y / 2f + col.offset.y- 0.05f), platforms); // 0.03f some magic number bullshit or something

	}

	protected void physAdjustHor( BoxCollider2D col ) {

		LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");

		Vector2 topLeft = Utilities.Vec2 (position.x - col.size.x / 2f, position.y + col.size.y / 4f);
		Vector2 botRight = Utilities.Vec2 (position.x + col.size.x / 2f, position.y - col.size.y / 4f);

		Collider2D other = null;
		other = Physics2D.OverlapArea (topLeft, botRight, platforms);

		if (other != null) {
			if ( other.transform.position.x > position.x )  {
				position.x = other.transform.position.x - other.bounds.extents.x - col.size.x / 2f -.01f;
				velocity.x = 0;
			} else {
				position.x = other.transform.position.x + other.bounds.extents.x + col.size.x / 2f +.01f;
				velocity.x = 0;
			}
		}

	}
	protected void physAdjustVert( BoxCollider2D col ) {
		
		LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");

		Vector2 topLeft = Utilities.Vec2 (position.x - col.size.x / 4f, position.y + col.size.y / 2f);
		Vector2 botRight = Utilities.Vec2 (position.x + col.size.x / 4f, position.y - col.size.y /2f);

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

	protected Collider2D ifCollision( LayerMask mask ) {
		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

		return Physics2D.OverlapArea (
			Utilities.Vec2 (position.x - col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y),
			Utilities.Vec2 (position.x + col.size.y / 2f, position.y - col.size.y / 2f + col.offset.y), mask);
	}

	void onDrawGizmoz() {
	}

}
