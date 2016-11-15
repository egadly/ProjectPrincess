using UnityEngine;
using System.Collections;

public class Hitbox : MonoBehaviour {

	public int counterLife;
	public int damage = 1;

	// Use this for initialization
	void Start () {
		counterLife = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
		counterLife++;
		transform.position = transform.parent.transform.position;
		gameObject.GetComponent<SpriteRenderer> ().flipX = !transform.parent.GetComponent<Princess> ().rightDir;
		gameObject.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, ( 42f - counterLife )/84f + 0.25f);
	}
}
