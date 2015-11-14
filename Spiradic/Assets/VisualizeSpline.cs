using UnityEngine;
using System.Collections;

public class VisualizeSpline : MonoBehaviour {

    [SerializeField] int _resolution = 1000;
    [SerializeField] LineRenderer _renderer;
    [SerializeField] BezierSpline _spline;

	void Update () {

	}

    void UpdatePositions () {
       for (int i = 0; i < _resolution; i++) {
            var pos = _spline.GetPoint(i/(float)(_resolution-1));
            _renderer.SetPosition(i, pos);
       }
    }

    void OnValidate () {
        _renderer.SetVertexCount(_resolution);
        UpdatePositions();
    }
}
