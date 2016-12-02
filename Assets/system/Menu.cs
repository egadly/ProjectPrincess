using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject;
	public SaveManager save;

	private bool buttonSelected;
	//private bool isPaused = true;
	private Options os;
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
	}

	private void OnDisable(){
		buttonSelected = false;
	}

	public void newGame(){
		save.ClearSave ();
	}

	public void continueGame(){
		save.LoadLatestLevel ();
	}

	public void quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

}
