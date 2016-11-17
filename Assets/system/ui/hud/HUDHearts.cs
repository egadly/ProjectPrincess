using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDHearts : MonoBehaviour {

	public int maximumHealth = 10;
	public int savedHealth = 0;
	public float scale;

	public Princess princess;
	public GameObject[] hearts;
	public GameObject heart;

	// Use this for initialization
	void Start () {

		
		princess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();

		hearts = new GameObject[maximumHealth];

		scale = Screen.height / 360;

		for ( int i = 0; i < maximumHealth; i++ ) {
			//Create the game objects
			hearts[i] = Instantiate( heart, this.transform ) as GameObject;
			hearts[i].GetComponent<RectTransform> ().localScale = new Vector3(scale, scale, 1f);
			//Position it in the scene
			hearts[i].transform.position = new Vector3((i* ((40*scale)+(10*scale))) + ((20*scale)+(10*scale)), (20*scale), 0);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		//get player current health
		if (savedHealth != princess.health) {
			updateHealth ();
		}
	
	}

	void updateHealth() {
		for (int i = 0; i < maximumHealth; i++) {
			if (i < princess.health) {
				if ( i >= savedHealth ) hearts[i].GetComponent<Animator>().Play ("heart_ui_grow");
			}
			else
				if ( i < savedHealth ) hearts [i].GetComponent<Animator> ().Play ("heart_ui_shrink");
				else hearts [i].GetComponent<Animator> ().Play ("heart_ui_blank");
		}
		savedHealth = princess.health;
	}
}
