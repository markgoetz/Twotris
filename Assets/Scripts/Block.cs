using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	private bool active = true;
	public GameObject explosion;
	public float FlashLength;

	private AbstractGoTween _tween;

	public void Clear() {
		if( _tween != null )
		{
			_tween.complete();
			_tween.destroy();
			_tween = null;
		}
		
		active = false;
		
		GameObject x = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		x.GetComponent<ParticleSystem>().startColor = BlockColor;
		
		Destroy (gameObject);
		// TODO: add clear effects / tweening here
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
	
	public void setOutline(Color c) {
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Material material = renderer.material;
		material.SetColor ("_OutlineColor", c);
	}
	
	public void Flash() {
		_tween = Go.to (transform, FlashLength, new GoTweenConfig().materialColor(Color.white).setIterations(2, GoLoopType.PingPong));
		//StartCoroutine("FlashCoroutine");
	}
	
	private IEnumerator FlashCoroutine() {
		Color current_color = BlockColor;
		BlockColor = Color.white;
		
		yield return new WaitForSeconds(FlashLength);
		
		BlockColor = current_color;
	}
	
	public bool Falling {
		get {
			if (transform.parent.tag == "BlockRoot")
				return false;
				
			return true;
		}
	}
	
	public void MoveDown() {
		// The piece transform gets really buggy and rotates unintuitively if you call MoveDown on a falling block.
		if (Falling) return; 
	
		transform.position = transform.position + new Vector3(0,-1,0);
	}
}