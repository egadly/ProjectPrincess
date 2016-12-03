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
    public int level = 0;
    public string levelName = "";
    GameObject s;


    // Use this for initialization
    void Start () {
		position = transform.position;
        s = GameObject.FindGameObjectWithTag("Save");
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

		if (options==null || !options.isPaused ) {
			if (ifCollision (1 << LayerMask.NameToLayer ("Player")))
            {
                if (level == 0)
                {
                    if (nextScene == null || nextScene == "")
                    {
                        if (thePrincess.keys == numKeys)
                        {
                            if (VirtualInput.downPos)
                            {
                                thePrincess.audioSources[6].Play();
                                hud.createDialog("You've gotten all the keys!\nYou're final score is " + fullScore + "\nLoading Next Level......", -1);
                            }
                            if (hud.dialogActive && hud.currentText == hud.givenText && !isLoading)
                            {
                                save.Save(fullScore);
                                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                                isLoading = true;
                            }
                        }
                        else if (VirtualInput.downPos)
                        {
                            hud.createDialog("The door remains locked. You need " + (numKeys - thePrincess.keys) + " more keys!", 1);
                        }
                    }
                    else
                    {
                        if (VirtualInput.downPos)
                            hud.createDialog("Loading Next Level......", -1);
                        if (hud.dialogActive && hud.currentText == hud.givenText && !isLoading)
                        {
                            save.Save();
                            SceneManager.LoadSceneAsync(nextScene);
                            isLoading = true;
                        }
                    }
                }

                else
                {
                    if (VirtualInput.downPos)
                    {

                        if (level == 4)
                        {
                            SceneManager.LoadSceneAsync(4);
                        }
                        else
                        {
                            int levelScore = s.GetComponent<SaveManager>().data.scores[level - 1];
                            if (levelScore == 0)
                            {
                                hud.createDialog("You must beat the previous levels open this door!", 1);
                            }
                            else
                            {
                                SceneManager.LoadSceneAsync(level);
                            }
                        }
                    }
                }
			}



		}
		if (thePrincess.health <= 0 && thePrincess.counterState >= 60) {
			if ( (thePrincess.counterState++) == 60 ) hud.createDialog ("You have died. Reloading Level........", -1);
			if ( hud.dialogActive && hud.currentText == hud.givenText && !isLoading ) {
					SceneManager.LoadSceneAsync( SceneManager.GetActiveScene().name );
					isLoading = true;
				}
		}

	}
}
