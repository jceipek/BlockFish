using UnityEngine;
using System.Collections;

public static class AudioConstants {
    public const int SAMPLE_RATE = 48000;
}

[System.Serializable]
public enum GameLayer {
    A = 0,
    B = 1
}

[System.Serializable]
public struct Obstacle {
    public int StartSample;
    public int StopSample;
    public GameLayer Layer;
    public Obstacle (int startSample, int stopSample, GameLayer layer) {
        StartSample = startSample;
        StopSample = stopSample;
        Layer = layer;
    }
}

[System.Serializable]
public struct MinMax {
    public int Min;
    public int Max;
    public MinMax (int min, int max) {
        Min = min;
        Max = max;
    }
}

[System.Serializable]
public struct FMinMax {
    public float Min;
    public float Max;
    public FMinMax (float min, float max) {
        Min = min;
        Max = max;
    }
}

public class Path : MonoBehaviour {
    int _currentObstacleIndex = 0;
    [SerializeField] Obstacle[] _obstacles = new Obstacle[0];

    AudioSource _audioSource;
    [SerializeField, TimeSample] int _startSample = 0;

    [SerializeField] BezierSpline _spline;
    [SerializeField, TimeSample] int _prePathSamples = AudioConstants.SAMPLE_RATE;

    [SerializeField] MinMax _cheatAdvanceSamplesRange = new MinMax(7675,11025); // AudioConstants.SAMPLE_RATE is 1 second

    public int CheatNextSampleToSwapOn (GameLayer objectOnLayer) {
        int advanceSamples = Random.Range(_cheatAdvanceSamplesRange.Min, _cheatAdvanceSamplesRange.Max);
        for (int index = _currentObstacleIndex; index < _obstacles.Length; index++) {
            if (_obstacles[index].Layer != objectOnLayer) {
                return _obstacles[index].StartSample - advanceSamples;
            }
        }

        return _audioSource.clip.samples + 1; // Never swap if no more obstacles
    }

    public int CurrentSample {
        get {
            return _audioSource.timeSamples;
        }
    }

    public float CurrFraction {
        get {
            return _audioSource.timeSamples/(float)_audioSource.clip.samples;
        }
    }

    public Vector2 SplinePositionForSample (int sample) {
        return _spline.GetPoint((sample+_prePathSamples)/(float)(_audioSource.clip.samples+_prePathSamples));
    }

    public Vector2 SplineDirectionForSample (int sample) {
        return _spline.GetDirection((sample+_prePathSamples)/(float)(_audioSource.clip.samples+_prePathSamples));
    }

    void Awake () {
        _audioSource = GetComponent<AudioSource>();
    }

	void Start () {
        _audioSource.timeSamples = _startSample;
        // 73.60023
        Debug.Log(_audioSource.clip.samples/(float)AudioConstants.SAMPLE_RATE);
        _audioSource.Play();
	}

    void Update () {
        // Debug.Log("HEY:");
        // Debug.Log(_audioSource.time);
        // Debug.Log(_audioSource.timeSamples/(float)AudioConstants.SAMPLE_RATE);
        while (_currentObstacleIndex <= _obstacles.Length-1 && _obstacles[_currentObstacleIndex].StopSample < CurrentSample) {
            _currentObstacleIndex++;
        }
    }

    public bool IsCurrentlyBlocked (GameLayer objectOnLayer) {
        if (_currentObstacleIndex <= _obstacles.Length-1) {
            bool withinObstacle = _obstacles[_currentObstacleIndex].StartSample <= CurrentSample && _obstacles[_currentObstacleIndex].StopSample >= CurrentSample;
            return _obstacles[_currentObstacleIndex].Layer != objectOnLayer && withinObstacle;
        }
        return false;
    }

    void OnDrawGizmos () {
        if (_audioSource == null) {
            _audioSource = GetComponent<AudioSource>();
        }
        Gizmos.DrawRay(transform.position, Vector3.right * (_audioSource.clip.samples/((float)AudioConstants.SAMPLE_RATE)));

        var pos = SplinePositionForSample(CurrentSample);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.1f);

        pos = SplinePositionForSample(_startSample);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.1f);

        foreach (var obstacle in _obstacles) {
            Gizmos.color = obstacle.Layer == GameLayer.A? Color.blue : Color.magenta;
            const int divisions = 20;
            int stepSize = (obstacle.StopSample - obstacle.StartSample)/divisions;
            Vector2 start, end;
            for (int i = obstacle.StartSample+stepSize; i < obstacle.StopSample; i+= stepSize) {
                start = SplinePositionForSample(i);
                end = SplinePositionForSample(i-stepSize);
                Gizmos.DrawLine(start, end);
            }
            start = transform.position + Vector3.right * (obstacle.StartSample/((float)AudioConstants.SAMPLE_RATE)) - Vector3.up;
            end = transform.position + Vector3.right * (obstacle.StopSample/((float)AudioConstants.SAMPLE_RATE)) - Vector3.up;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(start, Vector3.up);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(end, Vector3.up);
            Gizmos.color = obstacle.Layer == GameLayer.A? Color.blue : Color.magenta;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
            start = transform.position + Vector3.right * (_startSample/((float)AudioConstants.SAMPLE_RATE)) - Vector3.up;
            Gizmos.DrawRay(start+Vector2.up/2f, Vector2.up);
        }
    }
}