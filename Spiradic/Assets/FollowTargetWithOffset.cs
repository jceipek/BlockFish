﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct FollowInfo {
    [TimeSample] public int TimeSample;
    public float Separation;
    public float AngleOffset;
    public FollowInfo (int timeSample, float separation, float angleOffset) {
        TimeSample = timeSample;
        Separation = separation;
        AngleOffset = angleOffset;
    }
}


public class FollowTargetWithOffset : MonoBehaviour {
    [SerializeField] Path _path;
    [SerializeField] Transform _target;

    [SerializeField] FollowInfo[] _followInfos;

    [SerializeField] int _smoothCount = 120;
    Vector2 _sampleDirectionMovingAverageTimesN = new Vector2(1,0);

    FollowInfo FollowInfoForSample (int sample) {
        float sep = 0f;
        float angleOffset = 0f;
        for (int i = 0; i < _followInfos.Length-1; i++) {
            if (sample >= _followInfos[i].TimeSample && sample <= _followInfos[i+1].TimeSample) {
                float frac = MathHelpers.LinMapTo01((float)_followInfos[i].TimeSample, (float)_followInfos[i+1].TimeSample, (float)sample);
                sep = Mathf.Lerp(_followInfos[i].Separation, _followInfos[i+1].Separation, frac);
                angleOffset = Mathf.Lerp(_followInfos[i].AngleOffset, _followInfos[i+1].AngleOffset, frac);
                break;
            }
        }
        return new FollowInfo(sample, sep, angleOffset);
    }

    [SerializeField] float _DEBUG_SEP;
    [SerializeField] float _DEBUG_OFFSET;
	void LateUpdate () {
        var info = FollowInfoForSample(_path.CurrentSample);
        _DEBUG_SEP = info.Separation;
        _DEBUG_OFFSET = info.AngleOffset;
        // transform.position = _target.position - (Quaternion.Euler(0f,0f,info.AngleOffset)*_target.forward) * info.Separation;
        transform.position = _target.position - (Quaternion.Euler(0f,info.AngleOffset,0f) * _target.forward) * info.Separation;

        Vector2 delta = _target.position-transform.position;
        Vector2 currSample = (delta.sqrMagnitude > 0.01f)? delta.normalized : new Vector2(1,0);
        // Debug.Log(delta.normalized);
        // var currSample = delta.normalized;
        _sampleDirectionMovingAverageTimesN = _sampleDirectionMovingAverageTimesN + currSample - (_sampleDirectionMovingAverageTimesN/_smoothCount);

        transform.rotation = Quaternion.LookRotation((_sampleDirectionMovingAverageTimesN/_smoothCount).normalized, Vector3.up);
        // Debug.Log((_sampleDirectionMovingAverageTimesN/_smoothCount).normalized);
        // transform.rotation = Quaternion.FromToRotation(Vector3.up, _sampleDirectionMovingAverageTimesN/_smoothCount);
        // transform.right = _sampleDirectionMovingAverageTimesN/_smoothCount;
	}

    void OnDrawGizmos () {
        Gizmos.color = Color.green;
        for (int i = 0; i < _followInfos.Length; i++) {
            Gizmos.DrawLine(_path.SplinePositionForSample(_followInfos[i].TimeSample),
                            transform.position + Vector3.right * _followInfos[i].TimeSample/(float)AudioConstants.SAMPLE_RATE);
            // Gizmos.DrawLine(_path.SplinePositionForSample(_followInfos[i].TimeSample), );
        }
    }
}
