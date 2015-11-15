﻿using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ObstacleAnimator : MonoBehaviour {

    [SerializeField] int _obstacleIndex;
    [SerializeField] Renderer _renderer;
    [SerializeField, TimeSample] int _sampleOffset = AudioConstants.SAMPLE_RATE;
    [SerializeField, CurveRange] AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public void Initialize (int obstacleIndex, Material material) {
        _obstacleIndex = obstacleIndex;
#if UNITY_EDITOR
        _renderer.sharedMaterial = material;
        EditorUtility.SetDirty(gameObject);
#endif
    }

    Vector3 _targetPos;
    int _targetSample;
    Path _path;

    void Start () {
        _path = FindObjectOfType<Path>();
        var obstacle = _path.ObstacleAtIndex(_obstacleIndex);
        _targetSample = obstacle.StartSample + (obstacle.StopSample - obstacle.StartSample)/2;
        _targetPos = _path.SplinePositionForSample(_targetSample);
    }

	void Update () {
        int start = _targetSample - _sampleOffset;
        float frac = MathHelpers.LinMapTo01(start, _targetSample, _path.CurrentSample);
        if (frac >= 0f && frac <= 1f) {
            Debug.Log(frac);
            Vector3.Lerp(transform.position, _targetPos, _curve.Evaluate(frac));
        }
	}
}