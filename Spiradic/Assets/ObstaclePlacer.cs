using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObstaclePlacer : MonoBehaviour {
    [SerializeField] MinMax _beatIndexRange;
    [SerializeField] [Range(1,10)] int _placeEvery = 1;
    [SerializeField] GameObject _prefabToPlace;
    [SerializeField] bool _placeMe;
    [SerializeField] float _incomingSeparation = 1f;
    [SerializeField] bool _flipFirst;
    [SerializeField] [Range(1,10)] int _flipEvery = 1;
    Path _path;

#if UNITY_EDITOR
    void OnValidate () {
        if (!Application.isPlaying) {
            if (_path == null) {
                _path = FindObjectOfType<Path>();
            }
            if (_placeMe) {
                // EditorUtility.InstantiatePrefab(_prefabToPlace);
                _placeMe = false;
            }
        }
    }
#endif


    void OnDrawGizmos () {
        if (_path == null) {
            _path = FindObjectOfType<Path>();
        }
        int placementIndex = 0;
        for (int i = _beatIndexRange.Min; i <= _beatIndexRange.Max; i++) {
            if (i % _placeEvery == 0) {
                if (i >= _path.ObstacleCount) {
                    break;
                }
                var obstacle = _path.ObstacleAtIndex(i);
                var midSample = obstacle.StartSample + (obstacle.StopSample - obstacle.StartSample)/2;
                var samplePos = _path.SplinePositionForSample(midSample);
                var sampleDir = _path.SplineDirectionForSample(midSample);
                var samplePerp = Vector3.Cross(sampleDir, Vector3.forward);
                float orientation = placementIndex%_flipEvery==0? 1f : -1f;
                orientation = _flipFirst? -orientation : orientation;
                Gizmos.DrawWireSphere(samplePos, 0.1f);
                Gizmos.DrawLine((Vector3)samplePos, (Vector3)samplePos + (Vector3)samplePerp * _incomingSeparation * orientation);
                placementIndex++;
            }
        }
    }
}