using UnityEditor;
using UnityEngine;

public class ActuatorEditor : Editor
{
	protected const int EASING_GRAPH_WIDTH = 100;
	protected const int EASING_GRAPH_HEIGHT = 250;
	protected const int EASING_GRAPH_NUMBER_OF_POINTS = 20;

	protected static readonly GUIContent _easingFunctionLabel = new GUIContent("Easing Function", "Easing function that will describe the progress of the velocity");
	protected AnimationCurve easingCurve = new AnimationCurve();
    public void DrawEasingCurve(EasingFunction.Ease easing, Vector2 xposition, Vector2 yposition, string xtext, string  ytext, Vector2 xwidth, Vector2 ywidth)
    {
        // We clean the curve's keyframes before updating them.
        easingCurve.keys = new Keyframe[0];
        float step = 1f / (EASING_GRAPH_NUMBER_OF_POINTS - 1);
        for (int i = 0; i < EASING_GRAPH_NUMBER_OF_POINTS; i++)
        {
            float t = i * step;
            float y = EasingFunction.GetEasingFunction(easing)(0, 1, t);
            easingCurve.AddKey(t, y);
        }

        // Reserve space and draw the easing curve
        Rect curveRect = GUILayoutUtility.GetRect(EASING_GRAPH_WIDTH, EASING_GRAPH_HEIGHT, GUILayout.ExpandWidth(true));
        EditorGUI.CurveField(curveRect, easingCurve);

        // Draw axis labels (small offsets to place them properly)
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 10;
        GUI.Label(new Rect(curveRect.max -xposition, xwidth), xtext, labelStyle);

       
        GUI.Label(new Rect(new Vector2(curveRect.xMin + yposition.x, curveRect.yMin - yposition.y), ywidth), ytext, labelStyle);
    }

}
