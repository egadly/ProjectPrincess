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
	public bool ehitboxActive = false;
    public string message = "";

    HUD hud;

	public Sprite activeSprite;
	public Sprite idleSprite;
	public SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		position = transform.position;

        GameObject hudInstance = GameObject.FindGameObjectWithTag("HUD");
        if (hudInstance)
            hud = hudInstance.GetComponent<HUD>();

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
			if (ehitboxActive && ifCollision( 1 << LayerMask.NameToLayer("EnemyHitboxes")) )
				isActive = true;
			if (isActive)
				spriteRenderer.sprite = activeSprite;
			else
				spriteRenderer.sprite = idleSprite;
		} else {
			
			prevActive = curActive;

			curActive = false;
			if (playerActive && ifCollision( 1 << LayerMask.NameToLayer("Player")) )
				curActive = true;
			if (enemyActive && ifCollision( 1 << LayerMask.NameToLayer("Enemies")) )
				curActive = true;
			if (hitboxActive && ifCollision( 1 << LayerMask.NameToLayer("PlayerHitboxes")) )
				curActive = true;
			if (ehitboxActive && ifCollision( 1 << LayerMask.NameToLayer("EnemyHitboxes")) )
				curActive = true;

			if (curActive)
				spriteRenderer.sprite = activeSprite;
			else
				spriteRenderer.sprite = idleSprite;

            if (!prevActive && curActive)
            {
                isActive = !isActive;
                if (message != "")
                {
                    hud.createDialog(message, 1);
                }
            }
		}	
	}
}
