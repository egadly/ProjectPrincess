using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class optionsScript : MonoBehaviour {

	public EventSystem eventSystem;
    public GameObject selectedObject;
    private bool buttonSelected;
    public bool isPaused;

	// Use this for initialization
	void Start () {
		GetComponentInChildren<Canvas>().enabled = false;
		isPaused = false;
		Time.timeScale = 1.0f;
	}

	// Update is called once per frame
	void Update () {
		pausePress();
		//isPaused is added to if b/c w/o it
		//when paused, the first option in panel
		//is not the hilighted option
		if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false /*&& isPaused == true*/)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
    }

	public void togglePauseMenu()
	{
		if (!GetComponentInChildren<Canvas>().enabled)
		{
			GetComponentInChildren<Canvas>().enabled = true;
			isPaused = true;
			Time.timeScale = 0f;
		}
		else
		{
			GetComponentInChildren<Canvas>().enabled = false;
			isPaused = false;
			Time.timeScale = 1.0f;
		}
	}

	public void pausePress()
	{
        //have to add extra condition
        //&& current scene != main menu
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			togglePauseMenu();
		}
	}


	private void OnDisable()
    {
        buttonSelected = false;
    }

    public void quit()
    {
        #if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}