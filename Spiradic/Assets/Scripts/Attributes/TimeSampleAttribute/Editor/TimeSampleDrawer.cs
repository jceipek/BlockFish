using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TimeSampleAttribute))]
public class TimeSampleDrawer : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return GUI.skin.box.lineHeight * 2;
    }

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		// CurveRangeAttribute curveRange = attribute as CurveRangeAttribute;
		// // EditorGUILayout.CurveField(property.animationCurveValue, params GUILayoutOption[] options)

		EditorGUI.BeginProperty (position, label, property);
        if (property.propertyType != SerializedPropertyType.Integer) {
            EditorGUI.HelpBox(position, string.Format("{0} is not an Integer but has [TimeSample].", property.name), MessageType.Error);
        } else {
            float fVal = EditorGUI.FloatField(position, label, property.intValue/(float)AudioConstants.SAMPLE_RATE);
            if (fVal < 0f) {
                fVal = 0f;
            }
            property.intValue = (int)(fVal * (float)AudioConstants.SAMPLE_RATE);
            var bottomRect = position;
            bottomRect.y += GUI.skin.box.lineHeight;
            EditorGUI.LabelField(bottomRect, property.intValue.ToString());
        }
		EditorGUI.EndProperty();
	}
}