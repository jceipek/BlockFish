using UnityEngine;

public class FollowPath : MonoBehaviour {

    Path _path;

    [SerializeField] [TimeSample] int _sampleOffset;
    [SerializeField] bool _rotateToFollow = false;
    [SerializeField] int _smoothCount = 1;

    Vector2 _sampleDirectionMovingAverageTimesN;

    void Awake () {
        _path = FindObjectOfType<Path>();
    }

	void Update () {
        var pos = _path.SplinePositionForSample(_path.CurrentSample - _sampleOffset);
        transform.position = new Vector3(pos.x,pos.y,transform.position.z);
        if (_rotateToFollow) {
            var currSample = _path.SplineDirectionForSample(_path.CurrentSample + _sampleOffset);
            _sampleDirectionMovingAverageTimesN = _sampleDirectionMovingAverageTimesN + currSample - _sampleDirectionMovingAverageTimesN/_smoothCount;
            transform.forward = _sampleDirectionMovingAverageTimesN/_smoothCount;
        }
	}
}