using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

	int counterLife;
	public int lengthLife = 30;

	// Use this for initialization
	void Start () {

		counterLife = 0;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (counterLife == lengthLife)
			Destroy (this.gameObject);
		else counterLife++;
	
	}
}
