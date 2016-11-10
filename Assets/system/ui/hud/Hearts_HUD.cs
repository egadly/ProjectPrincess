using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hearts_HUD : MonoBehaviour {

	public int maximumHealth = 10;
	public int savedHealth = 0;

	public Princess princess;
	public GameObject[] hearts;
	public GameObject heart;

	// Use this for initialization
	void Start () {
		
		princess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();

		hearts = new GameObject[maximumHealth];

		for ( int i = 0; i < maximumHealth; i++ ) {
			//Create the game objects
			hearts[i] = (GameObject)Instantiate( heart, this.transform );
			//Position it in the scene
			hearts[i].transform.position = new Vector3((i* 90) +50, 40, 0);
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
				hearts [i].GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				if ( i >= savedHealth ) hearts [i].GetComponent<Animator> ().Play ("heart_ui_grow");
			}
			else
				//hearts[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
				if ( i < savedHealth ) hearts [i].GetComponent<Animator> ().Play ("heart_ui_shrink");
				else hearts [i].GetComponent<Animator> ().Play ("heart_ui_blank");
		}
		savedHealth = princess.health;
	}
}
