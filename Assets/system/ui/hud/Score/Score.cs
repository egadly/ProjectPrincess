using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour {

    public Text scoreText;
    private bool complete = false;
    GameObject freezeScore;
    public int score;
    //public static Score[] Instance;
    /*
    void Awake()
    {
        Instance = new Score[SceneManager.sceneCountInBuildSettings];
        int a = SceneManager.sceneCountInBuildSettings;
        Debug.Log(a + " number of scenes");
        if (Instance[SceneManager.GetActiveScene().buildIndex] == null)
        {
            DontDestroyOnLoad(transform.gameObject);
            DontDestroyOnLoad(Instance[SceneManager.GetActiveScene().buildIndex]);
            Debug.Log("Don't Destroy called");
            Instance[SceneManager.GetActiveScene().buildIndex] = this;
        }
        else if (Instance[SceneManager.GetActiveScene().buildIndex] != this)
        {
            Destroy(gameObject);
            Debug.Log("Destroy called");
        }
    }
    */
    
    void Start()
    {
        freezeScore = GameObject.FindGameObjectWithTag("Door");
        scoreText.text = "5000";
    }

    // Update is called once per frame
    void Update()
    {

        string scoreGet;
        scoreGet = scoreText.text.ToString();
        bool result = int.TryParse(scoreGet, out score);
        if (score > 0)
        {
            complete = freezeScore.GetComponent<KeyCheck>().levelComplete;
            if (!complete)
                score--;
            else
            { 
                if(GlobalScore.Instance[SceneManager.GetActiveScene().buildIndex] <= score)
                    GlobalScore.Instance[SceneManager.GetActiveScene().buildIndex] = score;
                //Debug.Log(GlobalScore.Instance[SceneManager.GetActiveScene().buildIndex].score);
            }
        }

        else
        {
            score = 0;

            //Show level failed screen, retry or exit.
        }
        scoreText.text = score.ToString();
    }
}
