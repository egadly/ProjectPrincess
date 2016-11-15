using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public GameObject target;
	public Vector3 position;
	public int counterShake;

	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag ("Player");
		position = new Vector3 (target.transform.position.x, target.transform.position.y, -10);
	
	}

	// Update is called once per frame
	void Update () {
		if (position != target.transform.position) {
			position += new Vector3 ((target.transform.position.x - position.x) / 16f, (target.transform.position.y - position.y) / 16f, 0);
		}

		counterShake = Mathf.Max (counterShake - 1, 0);

		if (counterShake == 0)
			transform.position = position;
		else
			transform.position = new Vector3(position.x + Random.Range (-.25f, .25f), position.y + Random.Range (-.25f, .25f), position.z); 
	
	}
}
