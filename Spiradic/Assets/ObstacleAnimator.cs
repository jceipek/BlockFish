using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ObstacleAnimator : MonoBehaviour {

    [SerializeField] int _obstacleIndex;
    [SerializeField] Renderer _renderer;
    [SerializeField, TimeSample] int _sampleOffset = AudioConstants.SAMPLE_RATE;
    [SerializeField, CurveRange] AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] Material _mat;

    public void Initialize (int obstacleIndex, int sampleOffset) {
        _sampleOffset = sampleOffset;
        _obstacleIndex = obstacleIndex;
        _path = FindObjectOfType<Path>();
        var obstacle = _path.ObstacleAtIndex(_obstacleIndex);
        _renderer.sharedMaterial = _mat;
        // _renderer.sharedMaterial = GlobalColorGenerator.G.GetMaterial(obstacle.Layer);
#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }

    Vector3 _targetPos;
    Vector3 _startPos;
    int _targetSample;
    Path _path;

    void Start () {
        _path = FindObjectOfType<Path>();
        var obstacle = _path.ObstacleAtIndex(_obstacleIndex);
        _targetSample = obstacle.StartSample + (obstacle.StopSample - obstacle.StartSample)/2;
        // _renderer.sharedMaterial = GlobalColorGenerator.G.GetMaterial(obstacle.Layer);
        _renderer.sharedMaterial = _mat;
        _startPos = transform.position;
        _targetPos = _path.SplinePositionForSample(_targetSample);
    }

	void Update () {
        int start = _targetSample - _sampleOffset;
        float frac = MathHelpers.LinMapTo01(start, _targetSample, _path.CurrentSample);
        if (frac >= 0f && frac <= 1f) {
            transform.position = Vector3.Lerp(_startPos, _targetPos, _curve.Evaluate(frac));
        }
	}
}