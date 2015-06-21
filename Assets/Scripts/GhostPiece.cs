using UnityEngine;
using System.Collections;

// The "Ghost" indicator of the falling piece
// It shows you where this piece will land.
public class GhostPiece : MonoBehaviour {
	private FallingPiece piece;
	public GameObject blockPrefab;
	public PlayerColorList playerColorList;

	private Board board;
	private float GHOST_COLOR_ALPHA = .3f;
	private GameObject[] blocks;

	void Start () {
		board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
		blocks = piece.Blocks;
		
		for (int i = 0; i < 4; i++) {
			GameObject ghost_block = Instantiate (blockPrefab);
			ghost_block.transform.parent = transform;
			
			transform.GetChild(i).transform.localPosition = blocks[i].transform.localPosition;
			
			MeshRenderer renderer = ghost_block.GetComponent<MeshRenderer>();
			Color player_color = playerColorList.Colors[piece.PlayerNumber];
			
			renderer.material.color = new Color(player_color.r, player_color.g, player_color.b, GHOST_COLOR_ALPHA);
		}
	}
	
	public void Remove() {
		Destroy (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		int y_offset = 0;
		bool collided = false;
		
		while (!collided) {
			y_offset++;
			
			foreach (GameObject block in blocks) {
				if (block == null) {
					Destroy(gameObject);
					return;
				}
				if (board.Collide(block.GetComponent<Block>().GamePosition + Vector3.down * y_offset, piece.PlayerNumber) == BlockCollision.Solid)
					collided = true;
			}
		}
		
		y_offset--;
		
		transform.position = piece.transform.position + Vector3.down * y_offset;
		transform.rotation = piece.transform.rotation;
	}
	
	public FallingPiece Piece {
		set { piece = value; }
	}
}
