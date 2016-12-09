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
	 
		//text[0] ="this is the tale of a princess. The princess was loved by all her people. One day the princess was wandering through the woods when she was kidnapped and taken to a tower.";
		//text [1] = "The princess decides to escape the tower on her own and without the help of prince. In so begins the adventure of a lifetime to escape the tower and return to her people.";
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene().buildIndex == 11 && text.GetLength(0) < 10 ) {
			GetScore ();
		}
			
		if(!hud.dialogActive && count<text.GetLength(0)){
			hud.createDialog (text [count++]);
			//hud.createDialog("this is the tale of a princess. The princess was loved by all her people. One day the princess was wandering through the woods when she was kidnapped and taken to a tower. ");
			//hud.createDialog ("The princess decides to escape the tower on her own and without the help of prince. In so begins the adventure of a lifetime to escape the tower and return to her people.");
		}
		if (!hud.dialogActive && count == text.GetLength (0) && !isLoading) {
			isLoading = true;
			if (SceneManager.GetActiveScene ().buildIndex == 11) {
				hud.createDialog ("Loading Next Level......", -1);
				SceneManager.LoadSceneAsync (3);
			} else {
				hud.createDialog ("Loading......", -1);
				SceneManager.LoadSceneAsync (SceneManager.GetActiveScene ().buildIndex + 1);
			}
		}
	}

	void GetScore() {
		int[] scores = GameObject.FindGameObjectWithTag ("Save").GetComponent<SaveManager> ().data.scores;
		string[] scoreText = new string[8];
		for (int i = 0; i < 7; i++) {
			scoreText [i] = "Level " + (i + 1) + " Score: " + scores [i + 4];
		}
		scoreText [7] = "Created By Group Five \n Damien Miller \n Ernest Gaddi \n Ivan Guzman \n Kai He \n Kevin Mazas \n Tom Perez";
		string[] tempText = new string[10];
		for (int i = 0; i < 2; i++)
			tempText [i] = text [i];
		for (int i = 2; i < 10; i++) {
			tempText [i] = scoreText [i-2];
		}
		text = tempText;
	}
}
