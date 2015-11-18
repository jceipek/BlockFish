using UnityEngine;
using System.Collections.Generic;


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

[System.Serializable]
public struct TimeMap {
    [SerializeField, TimeSample] public int TimeSample;
    [SerializeField, Range(0f,1f)] public float CurveFrac;
}

public class Path : MonoBehaviour {
    int _currentObstacleIndex = 0;
    [SerializeField] Obstacle[] _obstacles = new Obstacle[0];

    AudioSource _audioSource;
    [SerializeField, TimeSample] int _startSample = 0;

    [SerializeField] BezierSpline _spline;
    [SerializeField, TimeSample] int _prePathSamples = AudioConstants.SAMPLE_RATE;

    [SerializeField] MinMax _cheatAdvanceSamplesRange = new MinMax(7675,11025); // AudioConstants.SAMPLE_RATE is 1 second

    [SerializeField] TimeMap[] _timeMap = new TimeMap[0];

    public int CheatNextSampleToSwapOn (GameLayer objectOnLayer) {
        int advanceSamples = Random.Range(_cheatAdvanceSamplesRange.Min, _cheatAdvanceSamplesRange.Max);
        for (int index = _currentObstacleIndex; index < _obstacles.Length; index++) {
            if (_obstacles[index].Layer != objectOnLayer) {
                return _obstacles[index].StartSample - advanceSamples;
            }
        }

        return _audioSource.clip.samples + 1; // Never swap if no more obstacles
    }

    public Obstacle ObstacleAtIndex (int obstacleIndex) {
        return _obstacles[obstacleIndex];
    }

    public int ObstacleCount {
        get {
            return _obstacles.Length;
        }
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

    public float FracForSample (int sample, bool logme = false) {
        float frac = (sample+_prePathSamples)/(float)(_audioSource.clip.samples+_prePathSamples);
        for (int i = 0; i < _timeMap.Length-1; i++) {
            if (sample >= _timeMap[i].TimeSample && sample <= _timeMap[i+1].TimeSample) {
                frac = MathHelpers.LinMap(_timeMap[i].TimeSample, _timeMap[i+1].TimeSample, _timeMap[i].CurveFrac, _timeMap[i+1].CurveFrac, sample);
                if (logme) {
                    Debug.Log(frac);
                }
                break;
            }
        }
        return frac;
    }

    public Vector2 SplinePositionForSample (int sample) {
        return _spline.GetPoint(FracForSample(sample));
    }

    public Vector2 SplineDirectionForSample (int sample) {
        return _spline.GetDirection(FracForSample(sample));
    }

    public void SetObstaclesToList (List<Obstacle> obstacles) {
        _obstacles = new Obstacle[obstacles.Count];
        for (int i = 0; i < obstacles.Count; i++) {
            var o = obstacles[i];
            o.Layer = (GameLayer)(i%2);
            _obstacles[i] = o;
        }
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

    int _lastSample;
    void Update () {
        // Debug.Log("HEY:");
        // Debug.Log(_audioSource.time);
        // Debug.Log(_audioSource.timeSamples/(float)AudioConstants.SAMPLE_RATE);
        if (_lastSample > CurrentSample) {
            Debug.Log(FracForSample(_lastSample));
            Debug.Log(FracForSample(CurrentSample));
        }
        _lastSample = CurrentSample;
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

        Gizmos.color = Color.red;
        for (int i = 0; i < _timeMap.Length; i++) {
            Gizmos.DrawLine(SplinePositionForSample(_timeMap[i].TimeSample), transform.position + Vector3.right * _timeMap[i].TimeSample/(float)AudioConstants.SAMPLE_RATE);
        }
    }
}