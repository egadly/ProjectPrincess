using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalScore : MonoBehaviour {

    public int score;
    //public static GlobalScore[] Instance = new GlobalScore[SceneManager.sceneCountInBuildSettings];
    public static int[] Instance = new int[10];
    public GlobalScore test;

    void Awake()
    {
        if (test == null)
        {
            DontDestroyOnLoad(gameObject);
            // use the data from file that persists
            test = this;
        }
        else if (test != this)
            Destroy(gameObject);
        //Instance = new GlobalScore[SceneManager.sceneCountInBuildSettings];
        /*
        if (Instance[SceneManager.GetActiveScene().buildIndex] == null)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(Instance[SceneManager.GetActiveScene().buildIndex]);
            //Debug.Log("Don't Destroy called");
            Instance[SceneManager.GetActiveScene().buildIndex] = this;
        }
        else if (Instance[SceneManager.GetActiveScene().buildIndex] != this)
        {
            Destroy(gameObject);
            //Debug.Log("Destroy called");
        }
        */
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
