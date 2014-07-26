using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData {
	public struct Range {
		public int min;
		public int max;
	};

	public float end_time;
	public float player_speed;

	public Range floor_count;
	public Range hole_count;
	public Range height_diff;

	//コンストラクタ
	public LevelData()
	{
		this.end_time = 15.0f;
		this.player_speed = 6.0f;
		this.floor_count.min = 10;
		this.floor_count.max = 10;
		this.hole_count.min = 2;
		this.hole_count.max = 6;
		this.height_diff.min = 0;
		this.height_diff.max = 0;
	}
}

public class LevelControl : MonoBehaviour {

	//テキストから抽出したレベルデータを入れる
	private List<LevelData> level_datas = new List<LevelData>();

	public int HEIGHT_MAX = 20;
	public int HEIGHT_MIN = -4;
	
	public struct CreationInfo {
		public Block.TYPE block_type;
		public int max_count;
		public int height;
		public int current_count;
	};

	public CreationInfo previout_block; //前回どんなブロックを作ったか
	public CreationInfo current_block; //今回どんなブロックを作ったか
	public CreationInfo next_block;

	public int block_count = 0;
	public int level = 0;

	

	private void update_level(ref CreationInfo current, CreationInfo previous, float passage_time) 
	{
		//レベル1-5の繰り返し
		//pasage_time は 経過時間
		float local_time =  Mathf.Repeat(passage_time,this.level_datas[this.level_datas.Count - 1].end_time);

		//現在のレベルを求める
		int i;
		for(i = 0;i<this.level_datas.Count;i++) {
			if(local_time <= this.level_datas[i].end_time) {
				break;
			}
		}
		this.level = i;

		current.block_type = Block.TYPE.FLOOR;
		current.max_count = 1;

		if(this.block_count >= 10) {
			//現在のレベル用のデータを取得
			LevelData level_data;
			level_data = this.level_datas[this.level];

			switch(previous.block_type) {

				case Block.TYPE.FLOOR:
					current.block_type = Block.TYPE.HOLE;

					current.max_count = Random.Range(level_data.hole_count.min,level_data.hole_count.max);

					current.height = previous.height;
					break;

				case Block.TYPE.HOLE:
					current.block_type = Block.TYPE.FLOOR;

					current.max_count = Random.Range(level_data.floor_count.min,level_data.floor_count.max);

					//床の高さの最小値と最大値を求める
					int height_min = previous.height + level_data.height_diff.min;
					int height_max = previous.height + level_data.height_diff.max;
					height_min = Mathf.Clamp(height_min,HEIGHT_MIN,HEIGHT_MAX);
					height_max = Mathf.Clamp(height_max,HEIGHT_MIN,HEIGHT_MAX);

					//床の高さ
					current.height = Random.Range(height_min,height_max);
					break;

				}
			}

	}

	//何回も呼び出される
	public void update(float passage_time)
	{
		//今回作ったブロックを増やす
		this.current_block.current_count++;

		//今回作ったブロックの数が予定数以上なら
		if(this.current_block.current_count >= this.current_block.max_count) {
			this.previout_block = this.current_block;
			this.current_block = this.next_block;


			//次に作るブロックを初期化
			this.clear_next_block(ref this.next_block);
			//次に作るべきブロックを設定
			this.update_level(ref this.next_block,this.current_block,passage_time);
		}

		this.block_count++;

	}

	public void initialize()
	{
		this.block_count = 0; //ブロックの総数
		this.clear_next_block(ref this.previout_block); //refで変数そのものを渡す。(参照渡し)
		this.clear_next_block(ref this.current_block);
		this.clear_next_block(ref this.next_block);

	}

	//プロフ帳を初期化
	private void clear_next_block(ref CreationInfo block)
	{
		block.block_type = Block.TYPE.FLOOR;
		block.max_count = 15;
		block.height = 0;
		block.current_count = 0;
	}

	//テキストデータ解析
	public void loadLevelData(TextAsset level_data_text)
	{
		string level_texts = level_data_text.text;

		//改行コードごとに分割して、文字列の配列に入れる
		string[] lines = level_texts.Split('\n');

		foreach(var line in lines) {
			if(line == "") {
				continue;
			};

			Debug.Log(line);
			string[] words = line.Split();
			int n = 0;

			LevelData level_data = new LevelData();

			foreach(var word in words) {
				if(word.StartsWith("#")) {
					break;
				}
				if(word == "") {
					continue;
				}

				switch(n) {
					case 0: level_data.end_time = float.Parse(word);
					break;

					case 1: level_data.player_speed = float.Parse(word);
					break;

					case 2: level_data.floor_count.min = int.Parse(word);
					break;

					case 3: level_data.floor_count.max = int.Parse(word);
					break;

					case 4: level_data.hole_count.min = int.Parse(word);
					break;

					case 5: level_data.hole_count.max = int.Parse(word);
					break;

					case 6: level_data.height_diff.min = int.Parse(word);
					break;

					case 7: level_data.height_diff.max = int.Parse(word);
					break;
				}

				n++;
			}

			if (n>=8) {
				this.level_datas.Add(level_data);
			} else {
				if(n==0) {

				} else {
					Debug.LogError("[LevelData] out of parameter.\n");
				}
			}
		}

		//level_datasにデータがひとつもないならば
		if(this.level_datas.Count == 0) {
			Debug.LogError("[LevelData] Has no Data \n");
			this.level_datas.Add(new LevelData());
		}

	}

	//現在のプレイヤーのスピードを返す
	public float getPlayerSpeed()
	{
		return (this.level_datas[this.level].player_speed);
	}
}
