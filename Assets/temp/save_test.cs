

// via https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// English spelling for Behavior
public class save_test : MonoBehaviour {

	public save_test test;
	public Princess thePrincess;

	// makes certain the data is the right(current) data
	// as opposed to onStart(), (which comes afterwards) 
	void Awake () {
		if( test == null)
		{
			DontDestroyOnLoad(gameObject);
			// use the data from file that persists
			test = this;
		}
		else if(test != this)
			Destroy(gameObject);
	}

	void Start() {
		thePrincess = GameObject.FindGameObjectWithTag ("Player").GetComponent<Princess> ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.O))
			Save ();
		if (Input.GetKeyDown (KeyCode.P))
			Load();
	}

	// In order to write the file…
	public void Save(){
		Debug.Log ("Saving...");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/saveTest.dat");
		PlayerData data = new PlayerData (thePrincess.health, thePrincess.keys) ;
		bf.Serialize(file, data);
		file.Close();
	}

	// In order to read the file…
	public void Load(){
		Debug.Log ("Loading...");
		// if file DNE, a default C# exception will display
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/saveTest.dat", FileMode.Open);
		// when the serialized data is pulled fr container, the Unity will expect a generic file
		// so it has to be cast into a bin file.
		PlayerData data = (PlayerData)bf.Deserialize(file); 
		file.Close();
		thePrincess.health = data.health;
		thePrincess.keys = data.keys;
	}

	// the container. the [] makes it serialized.
	[Serializable]
	class PlayerData
	{
		public int health;
		public int keys;

		public PlayerData( int h, int k ) {
			health = h;
			keys = k;
		}
	}

	// Should be it.


}