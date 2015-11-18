using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Path))]
public class BeatLengthEditor : Editor {

    void OnSceneGUI () {
        Tools.hidden = true;
        var path = target as Path;
        var obstacles = serializedObject.FindProperty("_obstacles");
        for (int i = obstacles.arraySize - 1; i >= 0; i--) {
            var obstacle = obstacles.GetArrayElementAtIndex(i);

            var startSampleProp = obstacle.FindPropertyRelative("StartSample");
            var stopSampleProp = obstacle.FindPropertyRelative("StopSample");
            var layerProp = obstacle.FindPropertyRelative("Layer");

            var startSample = startSampleProp.intValue;
            var stopSample = stopSampleProp.intValue;
            var midSample = startSample + (stopSample - startSample)/2;

            Vector3 pos = path.SplinePositionForSample(midSample);

            // float frac = path.FracForSample(midSample);
            float frac = (stopSample - startSample)/2f;
            // int newFrac = (int)Handles.DoScaleHandle(Vector3.one * frac, pos, Quaternion.identity, size: 1f).x;
            Handles.color = Color.white;
            int newFrac = (int)Handles.ScaleValueHandle(frac, pos, Quaternion.identity, 1f, Handles.DotCap, 1f);

            // float posDelta = Handles.ScaleValueHandle(1f, pos-Vector3.Cross(Vector3.forward,path.SplineDirectionForSample(startSample))/3f, Quaternion.identity, 0.5f, Handles.DotCap, 0.5f);

            // Handles.Slider
            // Debug.Log(path.SplinePositionForSample(startSample) + " " + path.SplinePositionForSample(stopSample));


            startSampleProp.intValue = midSample - newFrac;
            stopSampleProp.intValue = midSample + newFrac;

            if (startSampleProp.intValue > stopSampleProp.intValue) {
                var temp = startSampleProp.intValue;
                startSampleProp.intValue = stopSampleProp.intValue;
                stopSampleProp.intValue = temp;
            }
            if (Mathf.Abs(startSampleProp.intValue - stopSampleProp.intValue) < 10) {
                startSampleProp.intValue = midSample - 5;
                stopSampleProp.intValue = midSample + 5;
            }

            // startSampleProp.intValue = startSampleProp.intValue + (int)(posDelta-1f);

            const int divisions = 20;
            int stepSize = (stopSample - startSample)/divisions;
            Handles.color = (GameLayer)layerProp.intValue == GameLayer.A? Color.blue : Color.magenta;
            Vector2 start, end;
            for (int n = startSample+stepSize; n < stopSample; n+= stepSize) {
                start = path.SplinePositionForSample(n);
                end = path.SplinePositionForSample(n-stepSize);
                // Debug.Log(start);
                Handles.DrawLine(start, end);
            }

            Handles.DrawSolidDisc(path.SplinePositionForSample(startSample), Vector3.forward, 0.1f);

            if (Handles.Button(pos - Vector3.Cross(Vector3.forward,path.SplineDirectionForSample(midSample)), Quaternion.identity, 0.5f, 0.5f, Handles.CircleCap)) {
                layerProp.intValue = (layerProp.intValue+1)%2;
            }

            // Handles.color = (GameLayer)layerProp.intValue == GameLayer.A? Color.blue : Color.magenta;
            // Handles.DrawLine(path.SplinePositionForSample(startSampleProp.intValue), path.SplinePositionForSample(stopSampleProp.intValue));

            const float buttonSize = 0.1f;
            var buttonPos = pos + Vector3.Cross(Vector3.forward,path.SplineDirectionForSample(midSample))/2f;
            if (Handles.Button(buttonPos, Quaternion.identity, buttonSize, buttonSize, Handles.DotCap)) {
                obstacles.DeleteArrayElementAtIndex(i);
            }
            Vector3[] corners = { new Vector3(-1f,1f,0f) * buttonSize,
                                  new Vector3(1f,-1f,0f) * buttonSize,
                                  new Vector3(-1f,-1f,0f) * buttonSize,
                                  new Vector3(1f,1f,0f) * buttonSize
                                };
            Handles.color = Color.black;
            Handles.DrawLine(buttonPos + corners[0], buttonPos + corners[1]);
            Handles.DrawLine(buttonPos + corners[2], buttonPos + corners[3]);

            Handles.color = Color.black;
            Handles.Label(pos, i.ToString());
        }
        serializedObject.ApplyModifiedProperties();
    }
}