using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Options : MonoBehaviour {

	public EventSystem eventSystem;
    public GameObject selectedObject;
	public VirtualInput input;
	public SaveManager save;

	public GameObject pausePanel;
	public GameObject optionsPanel;
	public GameObject controlPanel;
	public GameObject exitPanel;

	Canvas disabler;

    private bool buttonSelected;
    public bool isPaused;

	// Use this for initialization
	void Start () {
		save = GameObject.FindGameObjectWithTag ("Save").GetComponent<SaveManager> ();
		input = GameObject.FindGameObjectWithTag ("GameController").GetComponent<VirtualInput> ();
		disabler = GetComponentInChildren<Canvas> ();
		if (pausePanel != null)
			pausePanel.SetActive (false);
		if ( disabler != false ) disabler.enabled = false;
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
		if ( (optionsPanel!=null && controlPanel!=null) && controlPanel.activeSelf && !input.polling) {
			controlPanel.SetActive (false);
			optionsPanel.SetActive (true);
		}
    }

	public void togglePauseMenu() {
		if ( pausePanel != null ) {
			if (!pausePanel.activeSelf) {
				pausePanel.SetActive (true);
			} else {
				pausePanel.SetActive (false);
			}
		}
		if (disabler != null && !disabler.enabled)
		{
			disabler.enabled = true;
			isPaused = true;
			Time.timeScale = 0f;
		}
		else
		{
			disabler.enabled = false;
			isPaused = false;
			Time.timeScale = 1.0f;
			save.Save ();
		}
	}

	public void pausePress()
	{
        //have to add extra condition
        //&& current scene != main menu
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if ( pausePanel != null && !( optionsPanel.activeSelf||controlPanel.activeSelf||exitPanel.activeSelf ) )
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