using UnityEngine;
using System.Collections;

public class Hitbox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.transform.position;
		gameObject.GetComponent<SpriteRenderer> ().flipX = !transform.parent.GetComponent<Princess> ().rightDir;
	}
}
