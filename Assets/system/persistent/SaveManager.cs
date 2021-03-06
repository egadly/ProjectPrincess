﻿// via https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

// English spelling for Behavior
public class SaveManager : MonoBehaviour {

	public SaveManager test;
	public VirtualInput vInput;
	public PlayerData data;
	public AudioSource music;

	// makes certain the data is the right(current) data
	// as opposed to onStart(), (which comes afterwards) 
	void Awake () {
		/*if( test == null)
		{
			DontDestroyOnLoad(gameObject);
			// use the data from file that persists
			test = this;
		}
		else if(test != this)
			Destroy(gameObject);*/
	}

	void Start() {
		vInput = GameObject.FindGameObjectWithTag ("GameController").GetComponent<VirtualInput> ();
		music = GameObject.FindGameObjectWithTag ("Music").GetComponent<AudioSource> ();

		Load ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.F1))
			Save ();
		//if (Input.GetKeyDown (KeyCode.P))
			//Load();
	}

	// In order to write the file…
	public void Save( int score = 0 ){
		data.scores[SceneManager.GetActiveScene().buildIndex] = Mathf.Max( score, data.scores[SceneManager.GetActiveScene().buildIndex] );
		data = new PlayerData ( data.scores, music.volume, Character.globalVolume, vInput.jumpButton, vInput.kickButton, vInput.leapButton, vInput.leftButton, vInput.downButton, vInput.rightButton );
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/saveTest.dat");
		bf.Serialize(file, data);
		file.Close();
	}

	// In order to read the file…
	public void Load(){
		// if file DNE, a default C# exception will display
		BinaryFormatter bf = new BinaryFormatter();
        try
        {
            FileStream file = File.Open(Application.persistentDataPath + "/saveTest.dat", FileMode.Open);
            
            PlayerData tempData = (PlayerData)bf.Deserialize(file);
			data = new PlayerData (tempData.scores, tempData.musVolume, tempData.sfxVolume, tempData.jumpButton, tempData.kickButton, tempData.leapButton, tempData.leftButton, tempData.downButton, tempData.rightButton);
            file.Close();
            
        }
        catch (IOException e)
        {
			e = e;
			data = new PlayerData(new int[SceneManager.sceneCountInBuildSettings], music.volume, Character.globalVolume, vInput.jumpButton, vInput.kickButton, vInput.leapButton, vInput.leftButton, vInput.downButton, vInput.rightButton );

        }
		// when the serialized data is pulled fr container, the Unity will expect a generic file
		// so it has to be cast into a bin file.
		music.volume = data.musVolume;
		Character.globalVolume = data.sfxVolume;
		vInput.jumpButton = data.jumpButton;
		vInput.kickButton = data.kickButton;
		vInput.leapButton = data.leapButton;
		vInput.leftButton = data.leftButton;
		vInput.downButton = data.downButton;
		vInput.rightButton = data.rightButton;
	}

	// the container. the [] makes it serialized.
	[Serializable]
	public class PlayerData
	{
		public int jumpButton;
		public int kickButton;
		public int leapButton;
		public int leftButton;
		public int downButton;
		public int rightButton;
		public int[] scores;
		public float sfxVolume;
		public float musVolume;

		public PlayerData( int[] sc, float mus = 1, float sfx = 1, int j = 106, int k=107, int l=108, int a=97, int s=115, int d=100 ) {
			jumpButton = j;
			kickButton = k;
			leapButton = l;
			leftButton = a;
			downButton = s;
			rightButton = d; 
			if ( sc != null && sc.Length == SceneManager.sceneCountInBuildSettings ) scores = sc;
			else scores = new int[SceneManager.sceneCountInBuildSettings];
			sfxVolume = sfx;
			musVolume = mus;
		}
	}

    public void ClearSave()
    {
        data = new PlayerData(new int[SceneManager.sceneCountInBuildSettings], 1f, 1f, vInput.jumpButton, vInput.kickButton, vInput.leapButton, vInput.leftButton, vInput.downButton, vInput.rightButton);
        Save();
		LoadLatestLevel ();

    }
    public void LoadLatestLevel()
    {
        bool load = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (data.scores[i] == 0)
            {
                if (load == true)
                {
					GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>().createDialog("Loading......", -1);
					if (i == 11)
						SceneManager.LoadSceneAsync (3);
					else
                    	SceneManager.LoadSceneAsync(i);
					break;
                }
            }
            else
            {
                load = true;
            }
        }
		if (!load) {
			Save ();
			GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>().createDialog("Loading......", -1);
			SceneManager.LoadSceneAsync (2);
		}
    }
    // Should be it.


}