﻿using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

	Vector3 startPosition;
	Vector3 position;
	public Vector3 velocity;
	public bool ifShake = false;
	int counterLife;
	public int lengthLife = 30;
	Options pause;
	HUD hud;

	// Use this for initialization
	void Start () {
		pause = GameObject.FindGameObjectWithTag ("Panel").GetComponent<Options> ();
		hud = GameObject.FindGameObjectWithTag ("HUD").GetComponent<HUD> ();
		position = transform.position;
		if ( velocity != Vector3.zero ) velocity = new Vector3 ( Random.Range (-.1f, .1f), velocity.y, position.z);
		startPosition = position;
		counterLife = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!pause.isPaused&&!hud.dialogActive) {

			if (counterLife == lengthLife)
				Destroy (this.gameObject);
			else
				counterLife++;

			position += velocity;
			if (position.x > startPosition.x)
				velocity.x -= .05f;
			if (position.x < startPosition.x)
				velocity.x += .05f;


			if (!ifShake)
				transform.position = position;
			else
				transform.position = new Vector3 (position.x + Random.Range (-.25f, .25f), position.y + Random.Range (-.25f, .25f), position.z); 
		}
	
	}
}
