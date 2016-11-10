using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

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
		transform.position = position;
	
	}
}
