using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

	public int maximumHealth = 10;
	public int savedHealth = 0;
	public float scale;

	public Princess princess;
	public GameObject[] hearts;
	public GameObject heart;

	public GameObject dialogBox;
	public GameObject dialogMessage;
	public GameObject dialogContinue;

	public GameObject currentBox;
	public GameObject currentMessage;
	public GameObject currentContinue;

	public string givenText;
	public string currentText="";
	public bool dialogActive;
	public int dialogKiller;
	public int counterLife;
	public int typeSpeed;

	public GameObject timer;
	public int gameTime;
	public string score;

	private Door door;


	// Use this for initialization
	void Start () {
		Time.timeScale = 1;

		//Hearts
		princess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();
		door = GameObject.FindGameObjectWithTag ("Door").GetComponent<Door> ();

		scale = Screen.height / 360f;


		if (door.nextScene == null || door.nextScene == "") {
			timer = Instantiate (timer, transform) as GameObject;
			timer.GetComponent<RectTransform> ().localScale = new Vector3 (timer.GetComponent<RectTransform> ().localScale.x * scale, timer.GetComponent<RectTransform> ().localScale.y * scale, 1f);
			timer.transform.position = new Vector3 (Screen.width - (125f * scale), (45f * scale), 0);
				
			hearts = new GameObject[maximumHealth];
			for (int i = 0; i < maximumHealth; i++) {
				//Create the game objects
				hearts [i] = Instantiate (heart, this.transform) as GameObject;
				hearts [i].GetComponent<RectTransform> ().localScale = new Vector3 (scale, scale, 1f);
				//Position it in the scene
				hearts [i].transform.position = new Vector3 ((i * ((40f * scale) + (10f * scale))) + ((20f * scale) + (10f * scale)), (20f * scale), 0);
			}
		} else
			hearts = null;

	}
	
	// Update is called once per frame
	void Update () {
		counterLife = (counterLife + 1);
		//get player current health
		if (hearts != null && savedHealth != princess.health) updateHealth ();
		if (dialogActive) updateDialog ();
		if ( timer != null )if ( timer != null ) updateTimer ();

	
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
	void updateTimer() {
		string time = "Time " + ((gameTime / 60) / 60) + ":";
		int seconds = (gameTime / 60) % 60;
		if (seconds < 10)
			time += "0" + seconds;
		else
			time += seconds;
		timer.GetComponent<Text> ().text = time + "\nScore: " + score;
	}

	void updateDialog() {
		if ( ((dialogKiller==0&&Input.anyKeyDown) || (givenText == currentText&&dialogKiller == 1&&VirtualInput.jumpPos) ) ) {
			Destroy (currentBox);
			Destroy (currentMessage);
			Destroy (currentContinue);
			dialogActive = false;
			Time.timeScale = 1;
			currentText = "";
			if (dialogKiller == 1)
				VirtualInput.jumpPos = false;
		} else {
			if (currentText != givenText && ( counterLife%typeSpeed == 0 ) ) {
				if (givenText [currentText.Length] == '.')
					typeSpeed = 10;
				else
					typeSpeed = 1;
				currentText += givenText [currentText.Length];
				if ( dialogKiller == 1 && VirtualInput.jumpDown && currentText.Length != givenText.Length ) currentText += givenText [currentText.Length];
				currentMessage.GetComponent<Text> ().text = currentText;
			}
		}
	}

	public void createDialog( string text = "", int killer = 0) {
		if (!dialogActive) {
			Destroy (currentBox);
			Destroy (currentMessage);
			Destroy (currentContinue);
			currentBox = (GameObject)Instantiate (dialogBox, transform);
			currentBox.transform.position = new Vector3 (Screen.width / 2f, 100f * scale, -5f);
			currentMessage = (GameObject)Instantiate (dialogMessage, transform);
			currentMessage.transform.position = new Vector3 (Screen.width / 2f, 105f * scale, -5f);
			currentContinue = (GameObject)Instantiate (dialogContinue, transform);
			currentContinue.transform.position = new Vector3 (Screen.width / 2f, 105f * scale, -5f);
			currentMessage.GetComponent<Text> ().text = "";


			currentBox.GetComponent<RectTransform> ().localScale = 
			new Vector3 (currentBox.GetComponent<RectTransform> ().localScale.x * scale, currentBox.GetComponent<RectTransform> ().localScale.y * scale, 1f);
			currentMessage.GetComponent<RectTransform> ().localScale = 
			new Vector3 (currentMessage.GetComponent<RectTransform> ().localScale.x * scale, currentMessage.GetComponent<RectTransform> ().localScale.y * scale, 1f);
			currentContinue.GetComponent<RectTransform> ().localScale = 
			new Vector3 (currentContinue.GetComponent<RectTransform> ().localScale.x * scale, currentContinue.GetComponent<RectTransform> ().localScale.y * scale, 1f);

			currentText = "";
			givenText = text;
			dialogKiller = killer;
			if (dialogKiller == 1)
				currentContinue.GetComponent<Text> ().text = " Press Jump to Continue...";
			else if (dialogKiller == -1) {
				currentContinue.GetComponent<Text> ().text = ""; // THIS IS ONLY FOR LOADING LEVELS
			}
			dialogActive = true;
			Time.timeScale = 0;
		}
	}

	public void changeDialog( string text = "" ) {
		givenText = text;
	}
}
