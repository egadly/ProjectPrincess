using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

	Vector3 position;
	public bool ifShake = false;
	int counterLife;
	public int lengthLife = 30;

	// Use this for initialization
	void Start () {
		position = transform.position;
		counterLife = 0;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (counterLife == lengthLife)
			Destroy (this.gameObject);
		else counterLife++;

		if ( !ifShake )
			transform.position = position;
		else
			transform.position = new Vector3 (position.x + Random.Range (-.25f, .25f), position.y + Random.Range (-.25f, .25f), position.z); 
	
	}
}
