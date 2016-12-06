using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryTeller : MonoBehaviour {
	public HUD hud;
	public int count = 0;
	public string[] text;
	public bool isLoading = false;
	// Use this for initialization
	void Start () {
		hud = GameObject.FindGameObjectWithTag ("HUD").GetComponent<HUD> ();
		if (SceneManager.GetActiveScene ().name == "_Finale") {
			Debug.Log ("FUCK");
			GetScore ();
		}
	 
		//text[0] ="this is the tale of a princess. The princess was loved by all her people. One day the princess was wandering through the woods when she was kidnapped and taken to a tower.";
		//text [1] = "The princess decides to escape the tower on her own and without the help of prince. In so begins the adventure of a lifetime to escape the tower and return to her people.";
	}
	
	// Update is called once per frame
	void Update () {
			
		if(!hud.dialogActive && count<text.GetLength(0)){
			hud.createDialog (text [count++]);
			//hud.createDialog("this is the tale of a princess. The princess was loved by all her people. One day the princess was wandering through the woods when she was kidnapped and taken to a tower. ");
			//hud.createDialog ("The princess decides to escape the tower on her own and without the help of prince. In so begins the adventure of a lifetime to escape the tower and return to her people.");
		}
		if (!hud.dialogActive && count == text.GetLength (0) && !isLoading) {
			isLoading = true;
			SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex + 1);
		}
	}

	void GetScore() {
		int[] scores = GameObject.FindGameObjectWithTag ("Save").GetComponent<SaveManager> ().data.scores;
		string[] scoreText = new string[6];
		for (int i = 0; i < 6; i++) {
			scoreText [i] = "Level " + (i + 1) + " Score: " + scores [i + 4];
		}
		text = scoreText;
	}
}
