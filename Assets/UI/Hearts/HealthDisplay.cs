using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
    //get max player health
    //int maxHealth = GameObject.FindGameObjectWithTag("Princess").GetComponent<Princess>().health;
    int maxHealth = 10;

    public GameObject[] hearts;
    public GameObject h;


    public Sprite showHearts;
    public Sprite hideHearts;
    public Image numHearts;

    // Use this for initialization
    void Start()
    {
        //maxHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Princess>().health;
        maxHealth = 6;
        Debug.Log(maxHealth + " Player max health.");
        hearts = new GameObject[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {

            //Create the game objects
            hearts[i] = Instantiate(h, Vector3.zero, Quaternion.identity) as GameObject;
            if (hearts[i] == null)
                Debug.Log("Failed to load Object: " + i.ToString());
            //Position it in the scene
            else
            {
                //Debug.Log("Hearts loaded properly: " + i.ToString());
                hearts[i].transform.position = new Vector3((i* 70) + 20,10, 0);
                hearts[i].transform.SetParent(this.transform, false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //get player current health
        //playerHealth = GameObject.FindGameObjectWithTag("Princess").GetComponent<Princess>().health;
       // int playerHealth = 6;
        int playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Princess>().health;
        Debug.Log(GameObject.FindGameObjectWithTag("Player").GetComponent<Princess>().health + " Player current health.");
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < playerHealth)
            {
                hearts[i].gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                hearts[i].gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }
            // numHearts.sprite = hideHearts;
            //hearts[i].gameObject.GetComponent<Image>().sprite = hideHearts;
            //Debug.Log(i);
        }

    }
}
