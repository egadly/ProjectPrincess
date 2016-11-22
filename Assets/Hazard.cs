using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {

	public GameObject particle;
	Options pause;
	public int counterLife;

	// Use this for initialization
	void Start () {
		pause = GameObject.FindGameObjectWithTag ("Panel").GetComponent<Options> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!pause.isPaused) {
			counterLife = (counterLife + 1) % 2;
			if (particle != null && counterLife == 0)
				Instantiate (particle, transform.position + new Vector3 (Random.Range (-.1f, .1f), 0, 0), Quaternion.identity);
		}
	}
}
