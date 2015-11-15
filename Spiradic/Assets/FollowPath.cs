using UnityEngine;

public class FollowPath : MonoBehaviour {

    Path _path;

    [SerializeField] int _sampleOffset;
    [SerializeField] bool _rotateToFollow = false;

    void Awake () {
        _path = FindObjectOfType<Path>();
    }

	void Update () {
        var pos = _path.SplinePositionForSample(_path.CurrentSample - _sampleOffset);
        transform.position = new Vector3(pos.x,pos.y,transform.position.z);
        if (_rotateToFollow) {
            transform.forward = _path.SplineDirectionForSample(_path.CurrentSample + _sampleOffset);
        }
	}
}