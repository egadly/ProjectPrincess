using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public GameObject target;
	public Vector3 position;

	// Use this for initialization
	void Start () {
		position = transform.position;
		target = GameObject.FindGameObjectWithTag ("Player");
	
	}

	// Update is called once per frame
	void Update () {
		if (position != target.transform.position) {
			position += Utilities.Vec3 ((target.transform.position.x - position.x) / 16f, (target.transform.position.y - position.y) / 16f, 0);
		}
		transform.position = position;
	
	}
}
