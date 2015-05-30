using UnityEngine;
using System.Collections;

public class PieceList : ScriptableObject {
	public PieceTemplate[] pieces;

	public PieceTemplate getRandom() {
		int piece_number = Random.Range (0, pieces.Length - 1);
		Debug.Log (piece_number);
		return pieces[piece_number];
	}
}

[System.Serializable]
public class PieceTemplate {
	public string name; // pretty much only for editor use.
	public Color color;
	public Vector2[] locations;
}