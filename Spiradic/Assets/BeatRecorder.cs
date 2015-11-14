using UnityEngine;
using System.Collections.Generic;

public class BeatRecorder : MonoBehaviour {

    [SerializeField] KeyCode _keyA = KeyCode.A;
    [SerializeField] KeyCode _keyB = KeyCode.L;
    [SerializeField] int _sampleOffset = (int)(0.645892f * AudioConstants.SAMPLE_RATE);

    [SerializeField] List<Obstacle> _obstacles = new List<Obstacle>();

    Path _path;

    void Awake () {
        _path = FindObjectOfType<Path>();
    }

	void Update () {
        if (Input.GetKeyDown(_keyA)) {
            _obstacles.Add(new Obstacle(_path.CurrentSample, _path.CurrentSample+_sampleOffset, GameLayer.A));
        }

        if (Input.GetKeyDown(_keyB)) {
            _obstacles.Add(new Obstacle(_path.CurrentSample, _path.CurrentSample+_sampleOffset, GameLayer.B));
        }
	}
}