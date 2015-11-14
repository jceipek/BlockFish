using UnityEngine;

public class FollowPath : MonoBehaviour {

    Path _path;

    [SerializeField] [TimeSample] int _sampleOffset;

    void Awake () {
        _path = FindObjectOfType<Path>();
    }

	void Update () {
        var pos = _path.SplinePositionForSample(_path.CurrentSample - _sampleOffset);
        transform.position = new Vector3(pos.x,pos.y,transform.position.z);
	}
}