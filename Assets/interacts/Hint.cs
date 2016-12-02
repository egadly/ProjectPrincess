using UnityEngine;
using System.Collections;

public class Hint : MonoBehaviour {
    Princess thePrincess;
    HUD hud;
    public int hintType;
    int timeTouched = 0;
	// Use this for initialization
	void Start ()
    {
        thePrincess = GameObject.FindGameObjectWithTag("Player").GetComponent<Princess>();

        GameObject hudInstance = GameObject.FindGameObjectWithTag("HUD");
        if (hudInstance)
            hud = hudInstance.GetComponent<HUD>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<Collider2D>() == thePrincess.GetComponent<Collider2D>())
        timeTouched++;
        if (timeTouched >= 2)
        {
            hud.createDialog("Don't forget! You can use your kick while in the air to glide!", 1);
            gameObject.SetActive(false);
        }
    }
}
