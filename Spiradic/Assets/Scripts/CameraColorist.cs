using UnityEngine;

public class CameraColorist : MonoBehaviour {
    // Avatar _avatar;
    Path _path;
    Camera _camera;
    void Awake () {
        // _avatar = FindObjectOfType<Avatar>();
        _path = FindObjectOfType<Path>();
        _camera = GetComponent<Camera>();
    }

    void Update () {
        _camera.backgroundColor = GlobalColorGenerator.G.GetColorForSample(_path.CurrentSample); //GlobalColorGenerator.G.GetColor(_avatar.CurrentLayer);
    }
}