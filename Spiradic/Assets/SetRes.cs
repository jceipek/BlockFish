using UnityEngine;
using System.Collections;

public class SetRes : MonoBehaviour {
	void Start () {
	   Screen.SetResolution (1920, 1080, true);
	}

    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Application.LoadLevel(1);
        }
    }
}