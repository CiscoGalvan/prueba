using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveToAnObjectActuator))]
public class MoveToAnObjectComponentEditor : ActuatorEditor
{
	SerializedProperty _waypointData;
	//private static readonly GUIContent _shouldStopLabel = new GUIContent("Should Stop", "Indicates whether the enemy should stop upon reaching the waypoint.");
	private static readonly GUIContent _easingFunctionToAPointLabel = new GUIContent("Easing Function", "Easing function that will describe the progress of the position.");
	private static readonly GUIContent _timeToReachLabel = new GUIContent("Time To Reach", "Time it takes to reach the waypoint.");
	private static readonly GUIContent _waypointTransformLabel = new GUIContent("Waypoint Transform", "Reference to the waypoint transform.");
	private static readonly GUIContent _isAcceleratedLabel = new GUIContent("Is Accelerated", "Is the movement towards the waypoint accelerated?");
	//private static readonly GUIContent _stopDurationLabel = new GUIContent("Stop duration", "Time it will take the enemy to start movement to the next waypoint.");

	private void OnEnable()
	{
		_waypointData = serializedObject.FindProperty("_waypointData");
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		var waypointTransform = _waypointData.FindPropertyRelative("waypoint");
		EditorGUILayout.PropertyField(waypointTransform, _waypointTransformLabel);


		var timeToReach = _waypointData.FindPropertyRelative("timeToReach");
		timeToReach.floatValue = Mathf.Max(0, timeToReach.floatValue);
		EditorGUILayout.PropertyField(timeToReach, _timeToReachLabel);

		var isAccelerated = _waypointData.FindPropertyRelative("isAccelerated");
		EditorGUILayout.PropertyField(isAccelerated, _isAcceleratedLabel);

		if (isAccelerated.boolValue)
		{
			EditorGUI.indentLevel++;
			var easingFunctionProp = _waypointData.FindPropertyRelative("easingFunction");
			EditorGUILayout.PropertyField(easingFunctionProp, _easingFunctionToAPointLabel);
			EasingFunction.Ease easingEnum = (EasingFunction.Ease)easingFunctionProp.intValue;
            DrawEasingCurve(easingEnum, new Vector2(45, 15), new Vector2(15, 2), "X: Time", "Y: Position ", new Vector2(40, 20), new Vector2(60, 20));
            EditorGUI.indentLevel--;
		}
		serializedObject.ApplyModifiedProperties();
	}
    
}
