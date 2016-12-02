using UnityEngine;
using System.Collections;

public class Interact : MonoBehaviour {
	
	protected Vector3 position;
	protected Collider2D ifCollision( LayerMask mask ) {

		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

		return Physics2D.OverlapArea (
			new Vector2 (position.x - col.size.x / 2f + col.offset.x, position.y + col.size.y / 2f + col.offset.y),
			new Vector2 (position.x + col.size.x / 2f + col.offset.x, position.y - col.size.y / 2f + col.offset.y), mask);
	}

}
