using UnityEngine;
using LitJson;

public class MusicDataParser : MonoBehaviour {

    struct Bar {
        public float Confidence;
        public float Start;
        public float Duration;
        public Bar (double confidence, double start, double duration) {
            Confidence = (float)confidence;
            Start = (float)start;
            Duration = (float)duration;
        }
    }
    Bar[] _bars;
    Bar[] _beats;
    Bar[] _tatums;
    Bar[] _segments;
    [SerializeField] [Range(0f,1f)] float _confidenceThreshold = 0.1f;

    [SerializeField] TextAsset _musicDataFile;

    void OnValidate () {
        if (_musicDataFile != null) {
            LoadMusicData(_musicDataFile.text);
        }
    }

    void LoadMusicData (string jsonText) {
        JsonData data = JsonMapper.ToObject(jsonText);
        JsonData bars = data["bars"];
        _bars = new Bar[bars.Count];
        for (int i = 0; i < bars.Count; i++) {
            _bars[i] = new Bar(confidence: (double)(bars[i]["confidence"]),
                               start: (double)(bars[i]["start"]),
                               duration: (double)(bars[i]["duration"]));
        }

        JsonData beats = data["beats"];
        _beats = new Bar[beats.Count];
        for (int i = 0; i < beats.Count; i++) {
            _beats[i] = new Bar(confidence: (double)(beats[i]["confidence"]),
                               start: (double)(beats[i]["start"]),
                               duration: (double)(beats[i]["duration"]));
        }

        JsonData tatums = data["tatums"];
        _tatums = new Bar[tatums.Count];
        for (int i = 0; i < tatums.Count; i++) {
            _tatums[i] = new Bar(confidence: (double)(tatums[i]["confidence"]),
                               start: (double)(tatums[i]["start"]),
                               duration: (double)(tatums[i]["duration"]));
        }

        JsonData segments = data["segments"];
        _segments = new Bar[segments.Count];
        for (int i = 0; i < segments.Count; i++) {
            _segments[i] = new Bar(confidence: (double)(segments[i]["confidence"]),
                               start: (double)(segments[i]["start"]),
                               duration: (double)(segments[i]["duration"]));
        }
    }

    void DrawWithGizmos (Bar[] bars, float threshold, Color color, float offset) {
        Gizmos.color = color;
        for (int i = 0; i < bars.Length; i++) {
            var start = transform.position + Vector3.right * (bars[i].Start) - offset*Vector3.up;
            if (bars[i].Confidence >= threshold) {
                Gizmos.DrawRay(start+Vector3.up/2f, Vector3.up * bars[i].Confidence);
                Gizmos.DrawRay(start+Vector3.up/2f+Vector3.up * bars[i].Confidence, Vector3.right * bars[i].Duration);
            }
        }
    }

    void OnDrawGizmos () {
        DrawWithGizmos (_bars, _confidenceThreshold, Color.green, 4f);
        DrawWithGizmos (_beats, _confidenceThreshold, Color.red, 5f);
        DrawWithGizmos (_tatums, _confidenceThreshold, Color.blue, 6f);
        DrawWithGizmos (_segments, _confidenceThreshold, Color.magenta, 7f);
    }
}