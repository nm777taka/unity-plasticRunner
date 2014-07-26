using UnityEngine;
using System.Collections;

public class BlockCreator : MonoBehaviour {

	public GameObject[] blockPrefabs;
	private int block_count = 0;

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void createBlock(Vector3 block_position) {
		//作成すべきブロックを決める
		int next_block_type = this.block_count % this.blockPrefabs.Length;

		GameObject go = GameObject.Instantiate(this.blockPrefabs[next_block_type]) as GameObject;

		go.transform.position = block_position;
		this.block_count++;
	}
}
