using UnityEngine;

public class CameraColorist : MonoBehaviour {
    Avatar _avatar;
    Camera _camera;
    void Awake () {
        _avatar = FindObjectOfType<Avatar>();
        _camera = GetComponent<Camera>();
    }

    void Update () {
        _camera.backgroundColor = GlobalColorGenerator.G.GetColor(_avatar.CurrentLayer);
    }
}