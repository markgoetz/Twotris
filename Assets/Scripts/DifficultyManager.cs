using UnityEngine;
using System.Collections;

public class DifficultyManager : MonoBehaviour {
	private int lines = 0;
	public DifficultyLevel[] levels = {
		new DifficultyLevel(2f,   2f),
		new DifficultyLevel(1.8f, 1.8f),
		new DifficultyLevel(1.6f, 1.6f),
		new DifficultyLevel(1.4f, 1.4f),
		new DifficultyLevel(1.2f, 1.2f),
		new DifficultyLevel(1f,   1f),
		new DifficultyLevel(.8f,  .8f),
		new DifficultyLevel(.6f, .6f),
        new DifficultyLevel(.4f, .4f)
	};

	public int Lines {
		get { return lines; }
		set { lines = value; }
	}

	private DifficultyLevel _getCurrentLevel() {
		return levels[Level];
	}
	
	public int Level {
		get { 
			return lines / 10;
		}
	}

	public float PieceFallDelay {
		get { 
			return _getCurrentLevel().PieceFallDelay;
		}
	}

	public float BetweenPieceDelay {
		get { return _getCurrentLevel().BetweenPieceDelay; }
	}
}

[System.Serializable]
public class DifficultyLevel {
	public float PieceFallDelay;
	public float BetweenPieceDelay;

	public DifficultyLevel(float pfd, float bpd) {
		PieceFallDelay = pfd;
		BetweenPieceDelay = bpd;
	}
}


