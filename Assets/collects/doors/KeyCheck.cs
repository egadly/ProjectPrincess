using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class KeyCheck : MonoBehaviour {

    GameObject[] keys;
    public int numKeys;
   public bool levelComplete;
    bool checkDead = false;
    // Use this for initialization
    void Start()
    {
        //Get total number of keys and store them in integer variable to compare with 
        //the number of keys the player has
        keys = GameObject.FindGameObjectsWithTag("Key");
        numKeys = keys.Length;
        //Debug.Log(numKeys.ToString());
        levelComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Princess>().health == 0)
        {
            checkDead = true;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            //Debug.Log("Door touched");
            // Debug.Log(collider.GetComponent<TotKeys>().numKeys);
            if (numKeys > collider.GetComponent<Princess>().keys)
            {

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    levelComplete = true;
                    // SceneManager.LoadScene("scene1GUI");
                }
            }
        }
    }
    public void OnGUI()
    {
        if (levelComplete)
        {
            int b = 0;
            for (int a = 0; a < SceneManager.sceneCountInBuildSettings; a++)
            {
                //if (GlobalScore.Instance[a] )
                //{
                    b += GlobalScore.Instance[a];
                    Debug.Log("Score: from "+ a + " " + b);
                //}
            }
            GUI.BeginGroup(new Rect((Screen.width - 300) / 2, (Screen.height - 300) / 2, 300, 300)); 
            GUI.Box(new Rect(0, 0, 300, 300), "Congratulations!");
            GUI.Label(new Rect(100, 100, 230, 75), "LEVEL COMPLETE");
            GUI.Label(new Rect(100, 150, 100, 72), "Total Score: " + b);

            //Score.Instance[SceneManager.GetActiveScene().buildIndex].score.ToString()

            if (GUI.Button(new Rect(100, 200, 100, 30), "Continue") || Input.GetKeyDown(KeyCode.Return))
            {
                int i = SceneManager.GetActiveScene().buildIndex;
                i++;
                //Debug.Log(SceneManager.sceneCount + "Scenes");
                if (i > 4)
                    i = 0;
                SceneManager.LoadScene(i);
            }
            GUI.EndGroup();
        }

        //Test message to reload same scene if player dies.
        if (checkDead)
        {
            GUI.BeginGroup(new Rect((Screen.width - 300) / 2, (Screen.height - 300) / 2, 300, 300));
            GUI.Box(new Rect(0, 0, 300, 300), "Failure");
            GUI.Label(new Rect(100, 100, 230, 75), "You are Dead!");
            if (GUI.Button(new Rect(100, 200, 100, 30), "Continue") || Input.GetKeyDown(KeyCode.Return))
            {
                int i = SceneManager.GetActiveScene().buildIndex;
                //i++;
                //Debug.Log(SceneManager.sceneCount + "Scenes");
                //if (i > 4)
                 //   i = 0;
                SceneManager.LoadScene(i);
            }
            GUI.EndGroup();
        }
    }
}

