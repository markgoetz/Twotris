using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
	public GameObject objectToBePooled;
	public int poolSize;
	public bool willGrow = true;
	
	private List<GameObject> pool;

	void Start () {
		pool = new List<GameObject>();
		for (int i = 0; i < poolSize; i++) {
			GameObject obj = Instantiate(objectToBePooled);
			pool.Add (obj);
		}
	}
	
	public GameObject Get() {
		foreach (GameObject obj in pool) {
			if (!obj.activeInHierarchy) {
				return obj;
			}
		}
		
		if (willGrow) {
			GameObject obj = Instantiate(objectToBePooled);
			pool.Add (obj);
			return obj;
		}
		
		throw new PoolEmptyException();
	}
}

public class PoolEmptyException : System.Exception {}