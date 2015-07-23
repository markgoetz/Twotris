using UnityEngine;
using System.Collections;

public class SleepManager : MonoBehaviour {
	public void Sleep(SleepSettings settings) {
		StartCoroutine ("SleepCoroutine", settings);
	}
	
	IEnumerator SleepCoroutine(SleepSettings settings) {
		Time.timeScale = settings.timeScale;
		yield return new WaitForSeconds(settings.duration);
		Time.timeScale = 1;
	}
}

[System.Serializable]
public class SleepSettings {
	public float timeScale;
	public float duration;
}
