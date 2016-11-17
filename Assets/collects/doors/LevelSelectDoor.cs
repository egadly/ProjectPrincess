using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelectDoor : MonoBehaviour {

    public int doorNum;
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SceneManager.LoadScene(doorNum + 1);
            }
        }
    }
}
