using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	private bool active = true;

	public void Clear() {
		active = false;
		Destroy (gameObject);
		// TODO: add clear effects / tweening here
	}
	
	public bool Active { 
		get { return active; }
	}
	
	public void setColor(Color c) {
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Material material = renderer.material;
		material.SetColor ("_Color", c);
	}
	
	public void setOutline(Color c) {
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Material material = renderer.material;
		material.SetColor ("_OutlineColor", c);
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