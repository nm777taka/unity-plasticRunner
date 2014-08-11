using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {

	public float step_timer = 0.0f;
	private PlayerControl player = null;

	// Use this for initialization
	void Start () 
	{
		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () {

		this.step_timer += Time.deltaTime;

		if (this.player.isPlayerEnd()) {
			Application.LoadLevel("TitleScene");
		}
	
	}

	public float getPlayTime()
	{
		float time;
		time = this.step_timer;
		return(time);
	}
}
