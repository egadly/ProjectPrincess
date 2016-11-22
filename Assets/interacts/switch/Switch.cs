using UnityEngine;
using System.Collections;

public class Switch : Interact {

	public bool isActive = false;
	public bool prevActive = false;
	public bool curActive = false;
	public bool isToggle = false;
	public bool playerActive = true;
	public bool enemyActive = true;
	public bool hitboxActive = false;

	// Use this for initialization
	void Start () {
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
			if (hitboxActive && ifCollision( 1 << LayerMask.NameToLayer("PlayerHitboxes")) )
				isActive = true;
		} else {
			
			prevActive = curActive;

			curActive = false;
			if (playerActive && ifCollision( 1 << LayerMask.NameToLayer("Player")) )
				curActive = true;
			if (enemyActive && ifCollision( 1 << LayerMask.NameToLayer("Enemies")) )
				curActive = true;
			if (hitboxActive && ifCollision( 1 << LayerMask.NameToLayer("PlayerHitboxes")) )
				curActive = true;

			if (!prevActive && curActive)
				isActive = !isActive;
			
		}


	
	}
}
