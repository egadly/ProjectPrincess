using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour {

	Vector3 position;
	public Vector3 velocity;
	public GameObject particle;
	Options pause;
	public int counterLife;
	public int lengthLife;
	public int counterParticle = 6;
	public bool constantParticle;
	HUD hud;

	// Use this for initialization
	void Start () {
		position = transform.position;
		pause = GameObject.FindGameObjectWithTag ("Panel").GetComponent<Options> ();
		hud = GameObject.FindGameObjectWithTag ("HUD").GetComponent<HUD> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (!pause.isPaused && !hud.dialogActive) {
			counterLife = (counterLife + 1) % 600;
			if (constantParticle && counterLife % counterParticle == 0) {
				Instantiate (particle, transform.position + new Vector3 (0, .375f, 0) + new Vector3 (Random.Range (-.1f, .1f), 0, 0), Quaternion.identity);
			}
		

			if (lengthLife != 0) {
				if (counterLife == lengthLife)
					Destroy (this.gameObject);
				else {
					BoxCollider2D col = gameObject.GetComponent<BoxCollider2D> ();

					Collider2D other = Physics2D.OverlapArea (
						                  new Vector2 (position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y),
						                  new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("Platforms"));
					if ( other == null ) other = Physics2D.OverlapArea (
						new Vector2 (position.x - col.size.x / 2f, position.y + col.size.y / 2f + col.offset.y),
						new Vector2 (position.x + col.size.x / 2f, position.y - col.size.y / 2f + col.offset.y), 1 << LayerMask.NameToLayer ("Enemies"));
					if (other) {
						Destroy (this.gameObject);
						Instantiate (particle, position, Quaternion.identity);
					}
				}
			}
			position += velocity;
			transform.position = position;
		}
	}
}
