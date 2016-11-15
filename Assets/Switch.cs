using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public bool isActive = false;
	public bool prevActive = false;
	public bool curActive = false;
	public bool isToggle = false;
	public bool playerActive = true;
	public bool enemyActive = true;
	public bool hitboxActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
		Vector3 position = transform.position;

		if (!isToggle) {
			isActive = false;
			if (playerActive && Physics2D.OverlapArea (new Vector2 (transform.position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y), new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("Player")))
				isActive = true;
			if (enemyActive && Physics2D.OverlapArea (new Vector2 (transform.position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y), new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("Enemies")))
				isActive = true;
			if (hitboxActive && Physics2D.OverlapArea (new Vector2 (transform.position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y), new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("PlayerHitboxes")))
				isActive = true;
		} else {
			
			prevActive = curActive;

			curActive = false;
			if (playerActive && Physics2D.OverlapArea (new Vector2 (transform.position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y), new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("Player")))
				curActive = true;
			if (enemyActive && Physics2D.OverlapArea (new Vector2 (transform.position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y), new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("Enemies")))
				curActive = true;
			if (hitboxActive && Physics2D.OverlapArea (new Vector2 (transform.position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y), new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("PlayerHitboxes")))
				curActive = true;

			if (!prevActive && curActive)
				isActive = !isActive;
			
		}


	
	}
}
