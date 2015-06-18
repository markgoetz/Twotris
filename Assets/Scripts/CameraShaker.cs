using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {
	private AbstractGoTween _tween;

	public void Shake(CameraShakeParameters csp) {
		stopRunningTween();
		_tween = Go.to( transform, csp.duration, new GoTweenConfig().shake( Vector3.one * csp.amount, GoShakeType.Position ).shake (Vector3.one * csp.amount, GoShakeType.Eulers));
	}
	
	private void stopRunningTween() {
		if( _tween != null ) {
			_tween.complete();
			_tween.destroy();
			_tween = null;
		}
	}
}

[System.Serializable]
public class CameraShakeParameters {
	public float amount;
	public float duration;
}
