using UnityEngine;
using System.Collections;

public class GlobalColorGenerator : MonoBehaviour {

    public static GlobalColorGenerator G;

    [SerializeField] Color[] _startColors = new Color[2];
    [SerializeField] FMinMax _saturationRange = new FMinMax(0.1f, 1f);
    [SerializeField] AnimationCurve _saturationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    Path _path;
    void Awake () {
        _path = FindObjectOfType<Path>();
        G = this;
    }

    public Color GetColor (GameLayer layer) {
        float sat = MathHelpers.LinMapFrom01(_saturationRange.Min, _saturationRange.Max, _saturationCurve.Evaluate(_path.CurrFraction));
        var color = HSBColor.FromColor(_startColors[(int)layer]);
        color.s = sat;
        return color.ToColor();
    }
}