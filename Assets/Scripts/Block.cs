using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	private bool active = true;
	public GameObject explosion;
	public float FlashLength;

	private AbstractGoTween _tween;

	public void Clear() {
		active = false;
		
		ClearEffect();
		
		Destroy (gameObject);
	}

	private void ClearEffect() {
		if( _tween != null ) {
			_tween.complete();
			_tween.destroy();
			_tween = null;
		}
		
		GameObject x = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		x.GetComponent<ParticleSystem>().startColor = BlockColor;
	}
			
	public bool Active { 
		get { return active; }
	}
	
	public Color BlockColor {
		get {
			return GetComponent<MeshRenderer>().material.color;
		}
		set {
			GetComponent<MeshRenderer>().material.color = value;
		}
	}
	
	public Vector3 GamePosition {
		get {
			Transform piece = transform.parent.parent;
			return piece.position + piece.rotation * transform.localPosition;
		}
	}
	
	public void setOutline(Color c) {
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Material material = renderer.material;
		material.SetColor ("_OutlineColor", c);
	}
	
	public void Flash() {
		_tween = Go.to (
			transform,
			FlashLength,
			new GoTweenConfig()
				.materialColor(Color.white)
				.setIterations(2, GoLoopType.PingPong)
				.setEaseType(GoEaseType.CubicOut)
			);
	}
	
	public bool Falling {
		get {
			if (transform.parent.tag == "Board")
				return false;
				
			return true;
		}
	}
	
	public void MoveDown() {
		// The piece transform gets really buggy and rotates unintuitively if you call MoveDown on a falling block.
		if (Falling) return; 
	
		transform.position = transform.position + new Vector3(0,-1,0);
	}
	
	/*void OnDrawGizmos() {
		if (!Falling) return;
		Gizmos.color = Color.red;
		Gizmos.DrawCube (GamePosition, Vector3.one * .5f);
	}*/
}