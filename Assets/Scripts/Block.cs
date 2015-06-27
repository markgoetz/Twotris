﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Block : MonoBehaviour {
	private bool active = true;
	public GameObject explosion;
	public float FlashLength;

	private AbstractGoTween _tween;
	
	void Start() {
		GetComponent<Rigidbody>().detectCollisions = false;
	}

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
			Vector3 position = piece.position + piece.rotation * transform.localPosition;
			
			position.x = Mathf.RoundToInt(position.x);
			position.y = Mathf.RoundToInt(position.y);
			position.z = Mathf.RoundToInt(position.z);
			
			return position;
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
	
	public void Die() {
		GetComponent<Rigidbody>().detectCollisions = true;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().useGravity = true;
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