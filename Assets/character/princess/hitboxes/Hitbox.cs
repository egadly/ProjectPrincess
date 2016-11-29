using UnityEngine;
using System.Collections;

public class Hitbox : MonoBehaviour {

	public int counterLife;
	public int lengthLife = 30;
	public int damage = 1;

	// Use this for initialization
	void Start () {
		counterLife = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
		counterLife++;
		transform.position = transform.parent.transform.position;
		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		if ( spriteRenderer ) {
			spriteRenderer.flipX = !transform.parent.GetComponent<Princess> ().rightDir;
			float trans= ((float)(lengthLife-counterLife)/ (float)lengthLife);
			spriteRenderer.color = new Color (1f, 1f, 1f, 3f*trans*trans - 2f*trans*trans*trans );
		}
		if (counterLife == lengthLife)
			Destroy (gameObject);
	}
}
