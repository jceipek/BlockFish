using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

    [SerializeField] GameLayer _currentLayer;
    public GameLayer CurrentLayer {
        get {
            return _currentLayer;
        }
    }
    [SerializeField] bool _doCheat = true;

    public void SwapLayer () {
        _currentLayer = (GameLayer)(((int)_currentLayer+1)%System.Enum.GetNames(typeof(GameLayer)).Length);
    }

    Path _path;
    void Awake () {
        _path = FindObjectOfType<Path>();
    }

    bool _swapped;
    int _nextSampleToSwapOn;

    void Start () {
        _nextSampleToSwapOn = _path.CheatNextSampleToSwapOn(_currentLayer);
        _swapped = false;
    }

	void Update () {
        if (_doCheat) {
            if (_nextSampleToSwapOn <= _path.CurrentSample && !_swapped) {
                SwapLayer();
                _swapped = true;
                _nextSampleToSwapOn = _path.CheatNextSampleToSwapOn(_currentLayer);
                _swapped = false;
            }
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            SwapLayer();
        }

        if (_path.IsCurrentlyBlocked(_currentLayer)) {
            Debug.Log("Theoretical death");
        }
	}

    void OnDrawGizmos () {
        if (_path != null) {
            Gizmos.color = _currentLayer == GameLayer.A? Color.blue : Color.magenta;
            Gizmos.DrawRay(_path.transform.position + Vector3.right * (_path.CurrentSample/((float)AudioConstants.SAMPLE_RATE)) - Vector3.up, Vector3.up);
        }
    }
}