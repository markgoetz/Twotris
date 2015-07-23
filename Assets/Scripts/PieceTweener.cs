using UnityEngine;
using System.Collections;

public class PieceTweener : MonoBehaviour {
	public float movementStretchFactor;
	public float fallStretchFactor = .01f;
	//public float tweenTime;
	public float acceleration;
	public float rotationAcceleration;

	public float appearEffectTime = 1f;

	private Vector3 velocity = Vector3.zero;
	private float rotation_velocity = 0;
	private bool fast_fall;
	
	private GoTween _tween;
	
	public void AppearEffect() {
		_tween = transform.scaleFrom(appearEffectTime, Vector3.one * .01f, false);
		_tween.easeType = GoEaseType.BackOut;
		_tween.autoRemoveOnComplete = true;
		
	}

	public void StartFallEffect() {
		_tween = transform.scaleFrom(appearEffectTime, Vector3.one * .01f, false);
		_tween.easeType = GoEaseType.BackOut;
		_tween.autoRemoveOnComplete = true;
	}

	void Update () {
		handleMovement();
		handleRotation();
	}
	
	void handleMovement() {
		Vector2 direction = new Vector2(
			Mathf.Sign (transform.localPosition.x),
			Mathf.Sign (transform.localPosition.y)
		);
		

		velocity.x += acceleration * Time.deltaTime * -direction.x;
		velocity.y += acceleration * Time.deltaTime * -direction.y;
		
	
		transform.localPosition = transform.localPosition + velocity * Time.deltaTime;
		
		if (Mathf.Sign (transform.localPosition.x) != direction.x) {
			transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
			velocity.x = 0;
		}
		
		if (Mathf.Sign (transform.localPosition.y) != direction.y) {
			transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
			velocity.y = 0;
		}
		
		StretchMovement();
	}
	
	void handleRotation() {
		float z = transform.localRotation.eulerAngles.z;
		
		if (z == 0) return;
		
		float direction = Mathf.Sign (z - 180);
		rotation_velocity += rotationAcceleration * Time.deltaTime * direction;
		
	
		transform.localRotation = Quaternion.Euler (0,0,z + rotation_velocity * Time.deltaTime);
		
		if (Mathf.Sign (transform.localRotation.eulerAngles.z - 180) != direction) {
			transform.localRotation = Quaternion.identity;
			rotation_velocity = 0;
		}
	
	}
	
	public void Move(Vector3 movement) {
		transform.position = transform.position - movement;
	}
	
	public void Rotate(float z_angle) {
		// Wobble
		/*GoTweenConfig wobble_config  = new GoTweenConfig().scale(Vector3.one * 1.2f);
		GoTweenConfig restore_config = new GoTweenConfig().scale(Vector3.one);
		
		GoTween squash_tween  = new GoTween( transform, .1f, wobble_config);
		GoTween restore_tween = new GoTween( transform, .1f, restore_config);
		
		var flow = new GoTweenFlow();
		flow.insert( 0, squash_tween ).insert( .1f, restore_tween );
		flow.play();*/
		
		resetTween();
		_tween = Go.to (
			transform,
			.1f,
			new GoTweenConfig()
				.scale (Vector3.one * 1.2f)
				.setEaseType (GoEaseType.ElasticOut)
				.setIterations(2, GoLoopType.PingPong)
		);
		_tween.autoRemoveOnComplete = true;
		
		transform.localRotation = Quaternion.Euler(0,0,transform.localRotation.eulerAngles.z - z_angle);
	}
	
	public bool Done {
		get {
			return (transform.localPosition == Vector3.zero && transform.localRotation == Quaternion.identity);
		}
	}
	
	public void Stop() {
		resetTween ();
		transform.localPosition = Vector3.zero;
		velocity = Vector3.zero;
		transform.localScale = Vector3.one;
	}
	
	// Internal function to avoid race conditions when squashing and stretching.
	// If a tween is active, it should override StretchMovement.
	bool ShouldStretch() {
		if (_tween.state == GoTweenState.Running) return false;
	
		return true;
	}
	
	
	// Handles squash and stretch.
	void StretchMovement() {
		// Yield to any current tweens.
		if (!ShouldStretch()) {
			return;
		}
	
		Vector3 unrotated_position = transform.parent.rotation * transform.localPosition;
		Vector3 rotated_velocity = transform.parent.rotation * velocity;
		Vector3 scale;
		
		if (fast_fall) {
			float scale_factor = 1 + Mathf.Abs (rotated_velocity.y) * fallStretchFactor;
			scale = transform.parent.rotation * new Vector3(1 / scale_factor, scale_factor, 1 / scale_factor);
		}
		else {
			float scale_factor = 1 + Mathf.Abs (unrotated_position.x) * movementStretchFactor;
			scale = transform.parent.rotation * new Vector3(scale_factor, 1 / scale_factor, 1 / scale_factor);
		}

		if (scale.x < 0)
			scale.x = -scale.x;
		if (scale.y < 0)
			scale.y = -scale.y;
			
		transform.localScale = scale;
	}
	
	private void resetTween() {
		if( _tween != null )
		{
			_tween.complete();
			_tween.destroy();
			_tween = null;
		}
		
		transform.localScale = Vector3.one;
	}
	
	public bool Fastfall {
		get { return fast_fall; }
		set { fast_fall = value; }
	}
}
