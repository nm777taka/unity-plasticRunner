using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public static float ACCELERATION = 10.0f;
	public static float SPEED_MIN = 4.0f;
	public static float SPEED_MAX = 8.0f;
	public static float JUMP_HEIGHT_MAX = 3.0f;
	public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; //ジャンプからの減速値
	public static float NARAKU_HEIGHT = -0.5f;
	private float click_timer = -1.0f;//ボタンが押されてからの時間
	private float CLICK_GRACE_TIME = 0.5f; //ジャンプしたい意志を受ける時間

	public enum STEP { //プレイヤーの状態
		NONE = -1,
		RUN = 0,
		JUMP,
		MISS,
		NUM,   //状態が何種類あるのか示す

	};

	public STEP step = STEP.NONE;
	public STEP next_step = STEP.NONE;

	public float step_timer = 0.0f; //経過時間
	private bool is_landed = false; //着地してるか
	private bool is_collided = false; //何かとぶつかっているか
	private bool is_key_released = false; //ボタンが離されてるかどうか

	public float current_speed = 0.0f;
	public LevelControl level_control = null;



	// Use this for initialization
	void Start () {
		this.next_step = STEP.RUN;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = this.rigidbody.velocity;
		this.current_speed  = this.level_control.getPlayerSpeed();
		this.check_landed();

		switch(this.step) {
			case STEP.RUN:
			case STEP.JUMP:
			//現在の位置が閾値より低い
			if(this.transform.position.y < NARAKU_HEIGHT) {
				this.next_step = STEP.MISS;
			}
			break;
		}

		this.step_timer += Time.deltaTime; //経過時間を進める
		if(Input.GetMouseButtonDown(0)) {
			Debug.Log("押された");
			this.click_timer = 0.0f;
		} else {
			if(this.click_timer >= 0.0f) {
				this.click_timer += Time.deltaTime;
			}
		}



		if(this.next_step == STEP.NONE) {

			switch(this.step) {
				case STEP.RUN:

				//click_timerが0以上,CLICK_GRACE_TIME以下ならば
				if(0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME) {
					if(this.is_landed) {
						this.click_timer = -1.0f; //ボタンが押されてないことを表す
						Debug.Log("はいった");
						this.next_step = STEP.JUMP;
					}
				}

				break;

				case STEP.JUMP:
					if(this.is_landed) {
						this.next_step = STEP.RUN;
					}
					break;
			}
		}

		//状態が変化した瞬間
		while(this.next_step != STEP.NONE) {
			this.step = this.next_step;
			this.next_step = STEP.NONE;
			switch(this.step) {
				case STEP.JUMP:
					//ジャンプの高さから初速計算
				velocity.y = Mathf.Sqrt(
					2.0f*9.8f*PlayerControl.JUMP_HEIGHT_MAX);
				this.is_key_released = false;
				break;
			}

			this.step_timer = 0.0f;
		}

		//各状態ごとの毎フレームの更新処理
		//------------------------------
		switch(this.step) {
			case STEP.RUN:
				//速度上げる
				velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;

				//計算で求めたスピードが設定すべきスピードを超えていたら
				if(Mathf.Abs(velocity.x) >= this.current_speed) {
					//超えないように調整
					velocity.x *= this.current_speed/Mathf.Abs(velocity.x);
				}
				break;

			case STEP.JUMP:
				do {
					//ボタンが離された瞬間じゃなかったら
					if(!Input.GetMouseButtonUp(0)) {
						break;
					}

					//減速済みなら(2回以上減速しないように)
					if(this.is_key_released) {
						break;
					}

					//上下方向の速度が0以下なら(下降中なら)
					if(velocity.y <= 0.0f) {
						break;
					}

					//ボタンが離されていて上昇中なら下降開始
					//ジャンプの上昇はここで終わり
					velocity.y *= JUMP_KEY_RELEASE_REDUCE;

					this.is_key_released = true;

				} while(false);
				break;

				case STEP.MISS:
					velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
					if(velocity.x < 0.0f) {
						velocity.x = 0.0f;
					}
					break;
		}

		this.rigidbody.velocity = velocity;







	
	}

	private void check_landed() {

		this.is_landed = false;  //とりあえずfalse

		do {

			Vector3 s = this.transform.position; //Playerの現在の位置
			Vector3 e = s + Vector3.down * 1.0f; //sから下に1.0fに移動した位置

			RaycastHit hit;
			if(! Physics.Linecast(s,e,out hit)) { //sからeの間に何もない場合
				break;
			}

			//sからeの間に何かがあった場合
			if (this.step == STEP.JUMP) {
				//経過時間が3.0f未満ならば
				if(this.step_timer < Time.deltaTime * 3.0f) {
					break;
				}
			}

			//sからeの間に何かがあって、JUMP直後ではない場合
			this.is_landed = true;
		} while(false);
	}


}
