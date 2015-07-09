using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(BoardSoundManager))]
public class Board : MonoBehaviour {
	[Header("Dimensions")]
	public int height = 20;
	public int width  = 12;
	
	[Header("Wall Construction")]
	public GameObject wall;
	public float wallDepth = 3;
	public float wallThickness = 1;
	
	[Header("Line Clearing")]
	public ScoreKeeper scoreKeeper;
	public int[] pointsPerLine;
	public float timeBetweenClears = .3f;
	
	[Header("Object References")]
	public DifficultyManager dm;
	public UIManager ui;
	
	private int[,] block_grid;
	private bool game_over;
	
	private GameObject[] floors;
	private GameObject[] columns;
	private BoardSoundManager audio_manager;

	void Start () {
		audio_manager = GetComponent<BoardSoundManager>();
	
		// create walls.		
		GameObject left_wall  = Instantiate (wall, new Vector3(-1,    (height - 1) / 2f, 0), Quaternion.identity) as GameObject;
		GameObject right_wall = Instantiate (wall, new Vector3(width, (height - 1) / 2f, 0), Quaternion.identity) as GameObject;
		  
		left_wall.transform.localScale  = new Vector3(wallThickness, height + 1, wallDepth);	
		right_wall.transform.localScale = new Vector3(wallThickness, height + 1, wallDepth);	
		
		int floor_count = Mathf.RoundToInt(width / 1.5f);
		floors = new GameObject[floor_count];
		
		for (int i = 0; i < floor_count; i++) {

			GameObject floor = Instantiate (
				wall,
				new Vector3(.25f + 1.5f * i, -1, 0),
				Quaternion.identity)
			as GameObject;
			
			floor.transform.localScale = new Vector3(1.5f, wallThickness, wallDepth);
			floors[i] = floor;
		}
		
		block_grid = new int[width,height];
		
		columns = new GameObject[width];
		for (int i = 0; i < width; i++) {
			columns[i] = new GameObject();
			columns[i].transform.position = new Vector3(i, 0, 0);
			columns[i].transform.parent = transform;
		}
		
		UpdateBlocks (); // Call this on init because otherwise it will init to 0 (there is a block there)
	}
	
	private void UpdateBlocks() {
		game_over = false;
	
		for (int x = 0; x < block_grid.GetLength(0); x++) {
			for (int y = 0; y < block_grid.GetLength(1); y++) {
				block_grid[x,y] = -1;
			}

			foreach (Transform block in columns[x].transform) {
				Block block_component = block.gameObject.GetComponent<Block>();
								
				if (block_component.Falling) continue;
						
				if (block_component.Active) {
					int y = Mathf.RoundToInt(block.transform.position.y);
				
					if (y >= height) {
						game_over = true;
					}
				
					else {
						block_grid[ x, y ] = 0;
					}
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
	
	public void processClears() {
		//UpdateBlocks();
		
		List<int> clears = new List<int>();
		// Go from top to bottom so that the line numbers line up correctly.
		for (int y = height - 1; y >= 0; y--) {
			if (isRowFilled(y)) {
				clears.Add(y);
			}
		}
		
		if (clears.Count > 0) {
			StartCoroutine("clearLines", clears);
			UpdateBlocks ();
		}
	}
	
	private IEnumerator clearLines(List<int> clears) {
		int line_count = 0;
	
		foreach (int y in clears) {
			line_count++;
			clearLine (y, line_count);
			
			dm.AddLine();
			scoreKeeper.AddScore(pointsPerLine[line_count] - pointsPerLine[line_count - 1]);
			yield return new WaitForSeconds(timeBetweenClears);
		}
	}
	
	private bool isRowFilled(int y) {
		for (int x = 0; x < width; x++) {
			if (block_grid[x,y] != 0) return false;
		}
		
		return true;
	}
	
	private void clearLine(int y, int count) {
		for (int x = 0; x < block_grid.GetLength(0); x++) {
			foreach (Transform block in columns[x].transform) {
				int block_y = Mathf.RoundToInt(block.transform.position.y);
				if (block_y == y) {
					block.SendMessage ("Clear");
				}
				else if (block_y > y) {
					block.SendMessage ("MoveDown");
				}
			}
		}
		
		UpdateBlocks ();
		clearLineEffect(count);
	}
	
	public void AddBlocks(GameObject[] blocks) {
		GoTweenConfig squash_config  = new GoTweenConfig().scale(new Vector3(1.3f, .7f, 1.3f));
		GoTweenConfig restore_config = new GoTweenConfig().scale(new Vector3(1f, 1f, 1f));
	
		foreach (GameObject block in blocks) {
			int x = Mathf.RoundToInt(block.transform.position.x);
			block.transform.parent = columns[x].transform;
			
			GoTween squash_tween  = new GoTween( columns[x].transform, .25f, squash_config);
			GoTween restore_tween = new GoTween( columns[x].transform, .25f, restore_config);
			
			var flow = new GoTweenFlow();
			flow.insert( 0, squash_tween ).insert( .25f, restore_tween );
			flow.play();
		}
		
		audio_manager.PlaySound(BoardSounds.land);
		
		UpdateBlocks();
		processClears();
		
		if (game_over) {
			DieEffect ();
			
			GameObject[] spawners = GameObject.FindGameObjectsWithTag("PieceSpawner");
			foreach (GameObject spawner in spawners) {
				Destroy (spawner);
			}
			
			GameObject[] pieces = GameObject.FindGameObjectsWithTag ("Piece");
			foreach (GameObject piece in pieces) {
				Destroy (piece);
			}
			
			ui.Die();
		}
	}
	
	public int Width  { get { return width;  }}
	public int Height { get { return height; }}
	
	
	private void clearLineEffect(int count) {
		// Sound effect
		if (count == 1)
			audio_manager.PlaySound(BoardSounds.lineClear1);
		if (count == 2)
			audio_manager.PlaySound(BoardSounds.lineClear2);
		if (count == 3)
			audio_manager.PlaySound(BoardSounds.lineClear3);
		if (count == 4)
			audio_manager.PlaySound(BoardSounds.lineClear4);
				
		// Camera shake
		Camera.main.SendMessage ("Shake", CameraShakeType.Clear);
	}
	
	public void DieEffect() {
		foreach (GameObject floor in floors) {
			floor.SendMessage ("Fall");
		}
		
		for (int x = 0; x < width; x++) {
			foreach (Transform block in columns[x].transform) {
				block.gameObject.SendMessage ("Die");
			}
		}
	}
	
	public Vector3 Centroid {
		get {
			int block_count = 0;
			Vector3 total_centroid = Vector3.zero;
			
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (block_grid[x,y] == 0) {
						total_centroid += new Vector3(x, y, 0);
						block_count++;
					}
				}
			}
			
			if (block_count > 0) {
				return total_centroid / block_count;
			}
			else {
				throw new CameraTrackerException();
			}
		}
	}
	
	void OnDrawGizmos() {
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
	}
}

public enum BlockCollision {
	NoCollision,
	Solid,
	OtherPlayer
}
