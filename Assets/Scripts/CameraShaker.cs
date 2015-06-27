using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {
	public CameraShakeParameters shakeOnLand;
	public CameraShakeParameters shakeOnClear;

	private AbstractGoTween _tween;

	public void Shake(CameraShakeType type) {
		CameraShakeParameters csp;
		
		if (type == CameraShakeType.Clear)
			csp = shakeOnClear;
		else
			csp = shakeOnLand;
		
		shakeWithParameters(csp);
	}

	private void shakeWithParameters(CameraShakeParameters csp) {
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

public enum CameraShakeType {
	Land,
	Clear
}
