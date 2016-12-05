using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject;
	public SaveManager save;
	public bool isLoading;

	public VirtualInput input;
	public GameObject optionsPanel;
	public GameObject controlPanel;
	private bool buttonSelected;
	// Use this for initialization
	void Start () {
		buttonSelected = false;
	}

	// Update is called once per frame
	void Update () {
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

	private void OnDisable(){
		buttonSelected = false;
	}

	public void newGame(){
		if (!isLoading) {
			isLoading = true;
			save.ClearSave ();
		}
	}

	public void continueGame(){
		if (!isLoading) {
			isLoading = true;
			save.LoadLatestLevel ();
		}
	}

	public void quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

}
