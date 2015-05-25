using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {
	public int height = 25;
	public int width  = 25;
	
	public GameObject wall;
	public float wallDepth = 3;
	public float wallThickness = 1;
	
	private int[,] block_grid;
	private bool game_over;

	// Use this for initialization
	void Start () {
		// create walls.
		GameObject floor      = Instantiate (wall, new Vector3((width - 1) / 2f, -1,                0), Quaternion.identity) as GameObject;
		GameObject left_wall  = Instantiate (wall, new Vector3(-1,               (height - 1) / 2f, 0), Quaternion.identity) as GameObject;
		GameObject right_wall = Instantiate (wall, new Vector3(width,            (height - 1) / 2f, 0), Quaternion.identity) as GameObject;
		  
		floor.transform.localScale      = new Vector3(width + 2, wallThickness, wallDepth);	
		left_wall.transform.localScale  = new Vector3(wallThickness, height, wallDepth);	
		right_wall.transform.localScale = new Vector3(wallThickness, height, wallDepth);	
		
		block_grid = new int[width,height];		
	}
	
	void Update() {
		UpdateBlocks ();
	}
	
	private void UpdateBlocks() {
		game_over = false;
	
		for (int i = 0; i < block_grid.GetLength(0); i++) {
			for (int j = 0; j < block_grid.GetLength(1); j++) {
				block_grid[i,j] = -1;
			}
		}
		
		GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
		foreach (GameObject block in blocks) {
			if (block.activeSelf) {
				int x = Mathf.RoundToInt(block.transform.position.x);
				int y = Mathf.RoundToInt(block.transform.position.y);
			
				if (y >= height) {
					game_over = true;
				}
			
				else {
					block_grid[ x, y ] = 0;
				}
			}
		}
		
		GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
		foreach (GameObject piece_object in pieces) {
			if (piece_object.activeSelf == false) continue;
			
			FallingPiece piece = piece_object.GetComponent<FallingPiece>();
			
			foreach (GameObject piece_block in piece.Blocks) {
				int x = Mathf.RoundToInt(piece_block.transform.position.x);
				int y = Mathf.RoundToInt(piece_block.transform.position.y);
				
				if (y >= height) continue;
				
				if (piece_block.activeSelf && block_grid[x, y] == -1) {
					block_grid[x, y] = piece.PlayerNumber;
				}
			}
		}
		
	}
	
	public BlockCollision Collide(Vector3 position, int player_number) {
		BlockCollision block = CollideWithBlock (position, player_number);
		BlockCollision wall  = CollideWithWall (position);
	
		if (block == BlockCollision.Solid || wall == BlockCollision.Solid)
			return BlockCollision.Solid;
		if (block == BlockCollision.OtherPlayer)
			return BlockCollision.OtherPlayer;
			
		return BlockCollision.NoCollision;
	}
	
	public BlockCollision CollideWithWall(Vector3 position) {
		int x = Mathf.RoundToInt(position.x);
		int y = Mathf.RoundToInt(position.y);
		
		if (x < 0 || y < 0 || x >= width)
			return BlockCollision.Solid;
		
		return BlockCollision.NoCollision;
	}
	
	public BlockCollision CollideWithBlock(Vector3 position, int player_number) {
		int x = Mathf.RoundToInt(position.x);
		int y = Mathf.RoundToInt(position.y);
		
		if (x < 0 || y < 0 || x >= width || y >= height)
			return BlockCollision.NoCollision;
	
		int value = block_grid[
			Mathf.RoundToInt(position.x),
			Mathf.RoundToInt(position.y)
		];
		
		if (value == -1) return BlockCollision.NoCollision;
		if (value == player_number) return BlockCollision.NoCollision;
		
		if (value == 0) return BlockCollision.Solid;

		return BlockCollision.OtherPlayer;
	}
	
	public int processClears() {
		UpdateBlocks();
		
		List<int> clears = new List<int>();
		// Go from top to bottom so that the line numbers line up correctly.
		for (int y = height - 1; y >= 0; y--) {
			if (isRowFilled(y)) {
				clears.Add(y);
			}
		}
		
		if (clears.Count > 0) {
			foreach (int y in clears) {
				clearLine(y);
			}
			
			UpdateBlocks();
		}
		
		return clears.Count;
	}
	
	private bool isRowFilled(int y) {
		for (int x = 0; x < width; x++) {
			if (block_grid[x,y] != 0) return false;
		}
		
		return true;
	}
	
	private void clearLine(int y) {
		GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
		foreach (GameObject block in blocks) {
			int block_y = Mathf.RoundToInt(block.transform.position.y);
		
			if (block_y == y) {
				block.SetActive(false);
				Destroy(block); // TODO: Show the cleared effect.
			}
			else if (block_y > y) {
				block.transform.position = block.transform.position + new Vector3(0,-1,0);
			}
		}
	}
	
	public int Width  { get { return width;  }}
	public int Height { get { return height; }}
	public bool GameOver { get { return game_over; }}
	
	/*void OnDrawGizmos() {
		if (block_grid == null) return;
	
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (block_grid[x,y] > -1) {
					if (block_grid[x,y] == 0)
						Gizmos.color = Color.red;
					if (block_grid[x,y] == 1)
						Gizmos.color = Color.green;
					if (block_grid[x,y] == 2)
						Gizmos.color = Color.blue;		
					
					Gizmos.DrawCube(new Vector3(x,y,0f), new Vector3(.5f,.5f,.5f));
				}
			}
		}
	}*/
}

public enum BlockCollision {
	NoCollision,
	Solid,
	OtherPlayer
}
