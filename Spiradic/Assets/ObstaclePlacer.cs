using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObstaclePlacer : MonoBehaviour {
    [SerializeField] MinMax _beatIndexRange;
    [SerializeField] [Range(1,10)] int _placeEvery = 1;
    [SerializeField] GameObject _prefabToPlace;
    [SerializeField] float _incomingSeparation = 1f;
    [SerializeField] bool _flipFirst;
    [SerializeField] [Range(1,10)] int _flipEvery = 1;
    [SerializeField] GameLayer _layer;
    Path _path;
    [SerializeField] bool _placeMe;
    [SerializeField] Material[] _materials;

#if UNITY_EDITOR
    void OnValidate () {
        if (!Application.isPlaying) {
            if (_path == null) {
                _path = FindObjectOfType<Path>();
            }
            if (_placeMe) {

                int placementIndex = 0;
                for (int i = _beatIndexRange.Min; i <= _beatIndexRange.Max; i++) {
                    Vector3 anchor;
                    Vector3 startPos;
                    if (Place(i, placementIndex, out anchor, out startPos)) {
                        var obj = PrefabUtility.InstantiatePrefab(_prefabToPlace) as GameObject;
                        obj.transform.position = startPos;
                        obj.transform.right = (startPos - anchor);
                        // obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, startPos - anchor);
                        var animator = obj.GetComponent<ObstacleAnimator>();
                        animator.Initialize(i, _materials[(int)_layer]);
                        placementIndex++;
                    }
                }

                _placeMe = false;
            }
        }
    }
#endif

    bool Place (int index, int placementIndex, out Vector3 anchor, out Vector3 startPos) {
        if (index % _placeEvery != 0 || index >= _path.ObstacleCount) {
            anchor = Vector3.zero;
            startPos = Vector3.zero;
            return false;
        }
        var obstacle = _path.ObstacleAtIndex(index);
        var midSample = obstacle.StartSample + (obstacle.StopSample - obstacle.StartSample)/2;
        var samplePos = _path.SplinePositionForSample(midSample);
        var sampleDir = _path.SplineDirectionForSample(midSample);
        var samplePerp = Vector3.Cross(sampleDir, Vector3.forward);
        float orientation = placementIndex%_flipEvery==0? 1f : -1f;
        orientation = _flipFirst? -orientation : orientation;
        anchor = samplePos;
        startPos = (Vector3)samplePos + (Vector3)samplePerp * _incomingSeparation * orientation;
        return true;
    }


    void OnDrawGizmos () {
        if (_path == null) {
            _path = FindObjectOfType<Path>();
        }
        int placementIndex = 0;
        for (int i = _beatIndexRange.Min; i <= _beatIndexRange.Max; i++) {
            Vector3 anchor;
            Vector3 startPos;
            if (Place(i, placementIndex, out anchor, out startPos)) {
                Gizmos.DrawWireSphere(anchor, 0.1f);
                Gizmos.DrawLine(anchor, startPos);
                placementIndex++;
            }
        }
    }
}