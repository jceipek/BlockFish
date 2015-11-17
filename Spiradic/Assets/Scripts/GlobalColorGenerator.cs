using UnityEngine;

public class GlobalColorGenerator : MonoBehaviour {

    [SerializeField] Color[] _startColors = new Color[2];
    [SerializeField] FMinMax _saturationRange = new FMinMax(0.1f, 1f);
    [SerializeField] AnimationCurve _saturationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] Material[] _mainMaterials;
    static Path _path;


    static GlobalColorGenerator _g;
    public static GlobalColorGenerator G {
        get {
            if (_g == null) {
                _path = FindObjectOfType<Path>();
                _g = FindObjectOfType<GlobalColorGenerator>();
            }
            return _g;
        }
        protected set {
            _g = value;
        }
    }

    public Color GetColor (GameLayer layer) {
        if (_path == null) {
            _path = FindObjectOfType<Path>();
        }
        float sat = MathHelpers.LinMapFrom01(_saturationRange.Min, _saturationRange.Max, _saturationCurve.Evaluate(_path.CurrFraction));
        var color = HSBColor.FromColor(_startColors[(int)layer]);
        color.s = sat;
        return color.ToColor();
    }

    void Update () {
        for (int i = 0; i < 2; i++) {
            _mainMaterials[i].color = GetColor((GameLayer)i);
        }
    }

    public Material GetMaterial (GameLayer layer) {
        return _mainMaterials[(int)layer];
    }
}