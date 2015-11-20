using UnityEngine;

public class SpecialFollowPath : MonoBehaviour {

    [SerializeField] Path _path;
    [SerializeField] BezierSpline _spline;

    [SerializeField] [TimeSample] int _sampleOffset;
    [SerializeField] bool _rotateToFollow = false;
    [SerializeField] int _smoothCount = 1;
    [SerializeField] TimeMap[] _timeMap = new TimeMap[0];

    Vector2 _sampleDirectionMovingAverageTimesN;

	void Update () {
        var pos = SplinePositionForSample(_path.CurrentSample - _sampleOffset);
        transform.position = new Vector3(pos.x,pos.y,transform.position.z);
        if (_rotateToFollow) {
            var currSample = SplineDirectionForSample(_path.CurrentSample + _sampleOffset);
            _sampleDirectionMovingAverageTimesN = _sampleDirectionMovingAverageTimesN + currSample - _sampleDirectionMovingAverageTimesN/_smoothCount;
            transform.forward = _sampleDirectionMovingAverageTimesN/_smoothCount;
        }
	}

    float FracForSample (int sample) {
        float frac = sample/_path.TotalSamples;
        for (int i = 0; i < _timeMap.Length-1; i++) {
            if (sample >= _timeMap[i].TimeSample && sample <= _timeMap[i+1].TimeSample) {
                frac = MathHelpers.LinMap(_timeMap[i].TimeSample, _timeMap[i+1].TimeSample, _timeMap[i].CurveFrac, _timeMap[i+1].CurveFrac, sample);
                break;
            }
        }
        return frac;
    }

    Vector2 SplinePositionForSample (int sample) {
        return _spline.GetPoint(FracForSample(sample));
    }

    Vector2 SplineDirectionForSample (int sample) {
        return _spline.GetDirection(FracForSample(sample));
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.green;
        for (int i = 0; i < _timeMap.Length; i++) {
            Gizmos.DrawLine(SplinePositionForSample(_timeMap[i].TimeSample), transform.position + Vector3.right * _timeMap[i].TimeSample/(float)AudioConstants.SAMPLE_RATE);
        }
    }
}