using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Door : Interact {

	HUD hud;
	SaveManager save;
	Options options;

	public int numKeys;
	public bool isLoading = false;

	public int score = 0;
	public int fullScore;
	public int counterLife = 0;
	public int timeLimitInSec = 300;
	private int timeLimitInFrames;

	Princess thePrincess;
	public string nextScene;

	// Use this for initialization
	void Start () {
		position = transform.position;

		GameObject hudInstance = GameObject.FindGameObjectWithTag ("HUD");
		if (hudInstance)
			hud = hudInstance.GetComponent<HUD> ();

		GameObject[] keys = GameObject.FindGameObjectsWithTag ("Key");
		numKeys = keys.Length;


		thePrincess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();
		save = GameObject.FindGameObjectWithTag ("Save").GetComponent<SaveManager> ();
		options = GameObject.FindGameObjectWithTag ("Panel").GetComponent<Options> ();
		timeLimitInFrames = timeLimitInSec * 60;
	
	}
	
	// Update is called once per frame
	void Update () {

		if ( !hud.dialogActive ) counterLife++;
		fullScore = (int)Mathf.Max  ((score + (1000 * (timeLimitInFrames - counterLife)/(float)timeLimitInFrames) + 100 * thePrincess.health ) , 0);
		hud.score = "" + fullScore;
		hud.gameTime = counterLife;

		if (!options.isPaused) {
			if (ifCollision (1 << LayerMask.NameToLayer ("Player"))) {
				if (nextScene == null || nextScene == "") {
					if (thePrincess.keys == numKeys) {
						if (VirtualInput.downPos)
							hud.createDialog ("You've gotten all the keys!\nYou're final score is " + fullScore + "\nLoading Next Level......", -1);
						if (hud.dialogActive && hud.currentText == hud.givenText && !isLoading) {
							save.Save (fullScore);
							SceneManager.LoadSceneAsync( SceneManager.GetActiveScene ().buildIndex + 1 );
							isLoading = true;
						}
					} else if (VirtualInput.downPos) {
						hud.createDialog ("You need more keys! Numbnuts!", 1);
						if (hud.dialogActive && hud.givenText == hud.currentText)
								hud.changeDialog ("You need more keys! Numbnuts!\nQuit pressing DOWN while you're at it!");
					}
				} else {
					if (VirtualInput.downPos)
						hud.createDialog ("Loading Next Level......", -1);
					if (hud.dialogActive && hud.currentText == hud.givenText && !isLoading) {
						save.Save ();
						SceneManager.LoadSceneAsync( nextScene );
						isLoading = true;
					}
				}
		
			}
		}
		if (thePrincess.health <= 0 && thePrincess.counterState >= 60) {
			if ( (thePrincess.counterState++) == 60 ) hud.createDialog ("Haha! She Dead! Reloading Level........", -1);
			if ( hud.dialogActive && hud.currentText == hud.givenText && !isLoading ) {
					SceneManager.LoadSceneAsync( SceneManager.GetActiveScene().name );
					isLoading = true;
				}
		}

	}
}
