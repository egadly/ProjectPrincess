using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	protected Vector3 position;
	public Vector3 velocity;
	protected Collider2D isGrounded;



	protected void physicAdjust() {

		position += velocity;

		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();
		Vector2 topLeft =  Utilities.Vec2 (position.x - col.size.x / 2f, position.y + col.size.y / 2f);
		Vector2 botRight = Utilities.Vec2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f);

		if ((velocity.x * velocity.x) > (velocity.y * velocity.y)) {
			physAdjustHor (topLeft, botRight, col);
			physAdjustVert (topLeft, botRight, col);
		} else {
			physAdjustVert (topLeft, botRight, col);
			physAdjustHor (topLeft, botRight, col);
		}

		LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");
		isGrounded = Physics2D.OverlapCircle (Utilities.Vec2 (position.x, position.y - col.size.y / 2f), 0.05f, platforms);

	}

		protected void physAdjustHor( Vector2 topLeft, Vector2 botRight, BoxCollider2D col ) {
			
			botRight.y += col.size.y / 4f;
			topLeft.y -= col.size.y / 4f;
			LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");
			Collider2D other = null;
			other = Physics2D.OverlapArea (topLeft, botRight, platforms);

			if ( other != null ) {
				if (velocity.x >= 0) {
					position.x = other.transform.position.x - other.bounds.extents.x - col.size.x / 2f ;
					velocity.x = 0;
				} else {
					position.x = other.transform.position.x + other.bounds.extents.x + col.size.x / 2f;
					velocity.x = 0;
				}
			}

		}
		protected void physAdjustVert( Vector2 topLeft, Vector2 botRight, BoxCollider2D col ) {
			
			topLeft.x += col.size.x/4f;
			botRight.x += col.size.x/4f;
			LayerMask platforms = 1 << LayerMask.NameToLayer ("Platforms");

			Collider2D other = null;
			other = Physics2D.OverlapArea (topLeft, botRight, platforms);

			if ( other != null ) {
				if (velocity.y <= 0) {
					position.y = other.transform.position.y + other.bounds.size.y / 2f + col.size.y / 2f + 0.01f;
					velocity.y = 0;
				} else {
					position.y = other.transform.position.y - other.bounds.size.y / 2f - col.size.y / 2f;
					velocity.y = 0;
				}
			}
		}

	void onDrawGizmoz() {
		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

	}

}
