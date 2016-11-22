using UnityEngine;
using System.Collections;

public class VirtualInput : MonoBehaviour {

	public bool[] cur_vKeys;

	public int leftButton;
	public int downButton;
	public int rightButton;
	public int jumpButton;
	public int kickButton;
	public int leapButton;

	public static bool leftDown;
	public static bool leftPos;
	public static bool leftNeg;

	public static bool downDown;
	public static bool downPos;
	public static bool downNeg;

	public static bool rightDown;
	public static bool rightPos;
	public static bool rightNeg;

	public static bool jumpDown;
	public static bool jumpPos;
	public static bool jumpNeg;

	public static bool kickDown;
	public static bool kickPos;
	public static bool kickNeg;

	public static bool leapDown;
	public static bool leapPos;
	public static bool leapNeg;

	public bool polling = false;

	public int pollBuffer = 0;
	public int pollIndex = 0;

///////////////////////////////////////////////

	// Use this for initialization
	void Start () {
		cur_vKeys = new bool[509];
		jumpButton = (int)KeyCode.J;
		kickButton = (int)KeyCode.K;
		leapButton = (int)KeyCode.L;
		rightButton = (int)KeyCode.D;
		leftButton = (int)KeyCode.A;
		downButton = (int)KeyCode.S;		
	}
	
	// Update is called once per frame
	void Update () {

		if (polling) {
			for (int i = 0; i < 509; i++) {
				cur_vKeys [i] = Input.GetKey ((KeyCode)i);
			}
			if (pollBuffer <= 0) {
				HUD hud = GameObject.FindGameObjectWithTag ("HUD").GetComponent<HUD> ();
				if (Input.anyKeyDown) {
					if (pollIndex == 5) polling = false;
					for (int i = 0; i < 509; i++) {
						if (cur_vKeys [i]) {
							switch (pollIndex) {
							case 0:
								leftButton = i;
								break;
							case 1:
								downButton = i;
								break;
							case 2:
								rightButton = i;
								break;
							case 3:
								jumpButton = i;
								break;
							case 4:
								kickButton = i;
								break;
							case 5:
								leapButton = i;
								break;
							}
							break;
						}
					}
					pollIndex++;
				}
				switch (pollIndex) {
				case 0:
					if (!hud.dialogActive)
						hud.createDialog ("Press the button for Left...");
					break;
				case 1:
					if (!hud.dialogActive)
						hud.createDialog ("Press the button for Down...");
					break;
				case 2:
					if (!hud.dialogActive)
						hud.createDialog ("Press the button for Right...");
					break;
				case 3:
					if (!hud.dialogActive)
						hud.createDialog ("Press the button for Jump...");
					break;
				case 4:
					if (!hud.dialogActive)
						hud.createDialog ("Press the button for Kick...");
					break;
				case 5:
					if (!hud.dialogActive)
						hud.createDialog ("Press the button for Leap...");
					break;
				}
			} else
				pollBuffer--;
		} else {		
			
			jumpDown = Input.GetKey ((KeyCode)jumpButton);
			jumpPos = Input.GetKeyDown ((KeyCode)jumpButton);
			jumpNeg = Input.GetKeyUp ((KeyCode)jumpButton);

			kickDown = Input.GetKey ((KeyCode)kickButton);
			kickPos = Input.GetKeyDown ((KeyCode)kickButton);
			kickNeg = Input.GetKeyUp ((KeyCode)kickButton);

			leapDown = Input.GetKey ((KeyCode)leapButton);
			leapPos = Input.GetKeyDown ((KeyCode)leapButton);
			leapNeg = Input.GetKeyUp ((KeyCode)leapButton);



			rightDown = Input.GetKey ((KeyCode)rightButton);
			rightPos = Input.GetKeyDown ((KeyCode)rightButton);
			rightNeg = Input.GetKeyUp ((KeyCode)rightButton);

			leftDown = Input.GetKey ((KeyCode)leftButton);
			leftPos = Input.GetKeyDown ((KeyCode)leftButton);
			leftNeg = Input.GetKeyUp ((KeyCode)leftButton);
            
			downDown = Input.GetKey ((KeyCode)downButton);
			downPos = Input.GetKeyDown ((KeyCode)downButton);
			downNeg = Input.GetKeyUp ((KeyCode)downButton);
				
        }



	}

	public void enablePolling(){
		if (!polling) {					
			polling = true;
			pollBuffer = 6;
			pollIndex = 0;
		}
	}
}
