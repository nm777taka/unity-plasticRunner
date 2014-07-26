using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private GameObject player = null;
	private Vector3 position_offset = Vector3.zero;


	// Use this for initialization
	void Start () {
		this.player = GameObject.FindGameObjectWithTag("Player");

		//カメラの位置とプレイヤー位置の差分
		this.position_offset = this.transform.position - this.player.transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//LateUpdateはUpdate処理が終わった段階で呼び出される
	void LateUpdate() {
		Vector3 new_pos = this.transform.position;
		new_pos.x = this.player.transform.position.x + this.position_offset.x;

		this.transform.position = new_pos;
	}
}
