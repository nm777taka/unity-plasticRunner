using UnityEngine;
using System.Collections;

public class Block {
	//ブロックの種類を表す列挙体
	public enum TYPE {
		NONE = -1,
		FLOOR = 0,
		HOLE,
		NUM,
	};
};




public class MapCreator : MonoBehaviour {

	public static float BLOCK_WIDTH = 1.0f;
	public static float BLOCK_HEIGHT = 0.2f;
	public static int BLOCK_NUM_IN_SCREEN = 24;

	private LevelControl level_control = null;

	private struct FloorBlock {
		public bool is_created;
		public Vector3 position;
	};

	private FloorBlock last_block;
	private PlayerControl player = null;
	private BlockCreator block_creator;
	private GameRoot game_root = null;

	public TextAsset level_data_text = null;

	// Use this for initialization
	void Start () {

		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
		this.last_block.is_created = false;
		this.block_creator = this.gameObject.GetComponent<BlockCreator>();

		this.level_control = new LevelControl();
		this.level_control.initialize();

		//テキストからデータを読み出す
		this.level_control.loadLevelData(this.level_data_text);

		this.game_root = this.gameObject.GetComponent<GameRoot>();

		this.player.level_control = this.level_control;


	
	}
	
	// Update is called once per frame
	void Update () {

		float block_generate_x = this.player.transform.position.x;

		block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1)/2.0f;

		while(this.last_block.position.x < block_generate_x) {
			this.create_floor_block();
		}
	
	}

	private void create_floor_block() {

		Vector3 block_position;
		//初回作成時

		if (! this.last_block.is_created) {

			block_position = this.player.transform.position;

			block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

			block_position.y = 0.0f;
		} else {
			block_position = this.last_block.position;
		}

		block_position.x += BLOCK_WIDTH;

		//ブロック作成メソッド呼び出し
		//this.block_creator.createBlock(block_position);

		//this.level_control.update(); //Levelcontrolを更新
		this.level_control.update(this.game_root.getPlayTime());

		//level_controlに置かれたcurrent_block(今作るブロックの情報)の
		//heightをシーン上の座標に変換.
		block_position.y = level_control.current_block.height * BLOCK_HEIGHT;

		//今回作るブロックに関する情報を変数currentにおさめる
		LevelControl.CreationInfo current = this.level_control.current_block;

		//今回作るブロックが床なら
		//床じゃなかったら処理に入らない = 穴
		if(current.block_type == Block.TYPE.FLOOR) {
			//block_positionの位置にブロックを作成
			this.block_creator.createBlock(block_position);
		}

		//ポジションを更新
		this.last_block.position = block_position;

		this.last_block.is_created = true;

	}

	public bool isDelete(GameObject block_object) {

		bool ret = false;

		float left_limit = this.player.transform.position.x - BLOCK_WIDTH *((float)BLOCK_NUM_IN_SCREEN / 2.0f);

		if (block_object.transform.position.x < left_limit) {
			ret = true;
		} else {
			ret = false;
		}

		return ret;
	}
}
