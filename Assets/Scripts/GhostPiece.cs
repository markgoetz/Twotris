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

	void Start () {
		board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
		
		for (int i = 0; i < 4; i++) {
			GameObject ghost_block = Instantiate (blockPrefab);
			ghost_block.transform.parent = transform;
			
			MeshRenderer renderer = ghost_block.GetComponent<MeshRenderer>();
			Color player_color = playerColorList.Colors[piece.PlayerNumber];
			
			renderer.material.color = new Color(player_color.r, player_color.g, player_color.b, GHOST_COLOR_ALPHA);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// HACK: GameManager or FallingPiece should send a message to destroy this.
		if (piece == null) {
			Destroy (gameObject);
		}
	
		GameObject[] blocks = piece.Blocks;
		
		int y_offset = 0;
		bool collided = false;
		
		while (!collided) {
			y_offset++;
			
			foreach (GameObject block in blocks) {
				if (block == null) {
					Destroy(gameObject);
					return;
				}
				if (board.Collide(block.transform.position + Vector3.down * y_offset, piece.PlayerNumber) != BlockCollision.NoCollision)
					collided = true;
			}
		}
		
		y_offset--;
		
		transform.position = piece.transform.position + Vector3.down * y_offset;
		transform.rotation = piece.transform.rotation;
		
		// HACK: this shouldn't be necessary every frame.
		for (int i = 0; i < blocks.Length; i++) {
			transform.GetChild(i).transform.localPosition = blocks[i].transform.localPosition;
		}
	}
	
	public FallingPiece Piece {
		set { piece = value; }
	}
}
