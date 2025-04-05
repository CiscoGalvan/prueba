using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveToAPointActuator))]
public class MoveToAPoint_ActuatorEditor : ActuatorEditor
{
	private SerializedProperty _waypointsData;
	private SerializedProperty _isACicle;
	private SerializedProperty _usageWay;
	private SerializedProperty _randomArea;
	private SerializedProperty _timeBetweenRandomPoints;
	private SerializedProperty _allWaypointsHaveTheSameData;



	private bool _showWaypointsData= true;
	private bool _showAllSameData = false;

	#region Values for same data in case it's needed
	private SerializedProperty _timeToReachForAllWaypoints;
	private SerializedProperty _areAccelerated;
	private SerializedProperty _shouldThemStop;
	private SerializedProperty _stopTime;
	private SerializedProperty _easingFunctionForAllWaypoints;
	private static readonly GUIContent _allTheSameDataLabel = new GUIContent("Same Waypoint Behaviour", "If true, every movement toward any waypoint will have the same behaviour.\n" +
	"A Waypoint Transform will still be needed.");
	private static readonly GUIContent _timeToReachEveryWaypointLabel = new GUIContent("Time Between Waypoints", "Time it takes to reach each waypoint.");
	private static readonly GUIContent _areAcceleratedLabel = new GUIContent("Are Accelerated", "Are the movements accelerated?");
	private static readonly GUIContent _shouldThemStopLabel = new GUIContent("Should Stop", "Indicates whether the enemy should stop upon reaching each waypoint.");
	#endregion

	private static readonly GUIContent _usageWayLabel = new GUIContent("Usage Way", "How will the waypoints be set?\n" +
		"Random Area: A collider will be given and the waypoints will be generated inside it.\n" +
		"Waypoint: A sequence of waypoints will be given from start.\n");
	private static readonly GUIContent _randomAreaLabel = new GUIContent("Random Area", "Area that will describe where the next waypoints will be generated.");
	private static readonly GUIContent _timeBetweenWaypointsLabel = new GUIContent("Time Between Random Points", "Time that will take to go from one point to another.");
	private static readonly GUIContent _stopDurationLabel = new GUIContent("Stop Duration", "Time it will take the enemy to start movement to the next waypoint.");
	private static readonly GUIContent _shouldStopLabel = new GUIContent("Should Stop", "Indicates whether the enemy should stop upon reaching the waypoint.");
	private static readonly GUIContent _easingFunctionToAPointLabel = new GUIContent("Easing Function", "Easing function that will describe the progress of the position.");
	private static readonly GUIContent _timeToReachLabel = new GUIContent("Time To Reach", "Time it takes to reach the waypoint.");
	private static readonly GUIContent _waypointTransformLabel = new GUIContent("Waypoint Transform", "Reference to the waypoint transform.");
	private static readonly GUIContent _isAcceleratedLabel = new GUIContent("Is Accelerated", "Is the movement towards the waypoint accelerated?");
	private static readonly GUIContent _sizeLabel = new GUIContent("Size", "Number of waypoints");
	private static readonly GUIContent _isACicleLabel = new GUIContent("Is A Cicle", "If true, the waypoint path will loop: after reaching the last waypoint, it will return to the first one");



	
	private void OnEnable()
	{
		_waypointsData = serializedObject.FindProperty("_waypointsData");
		_isACicle = serializedObject.FindProperty("_isACicle");
		_usageWay = serializedObject.FindProperty("_usageWay");
		_randomArea = serializedObject.FindProperty("_randomArea");
		_timeBetweenRandomPoints = serializedObject.FindProperty("_timeBetweenRandomPoints");
		_allWaypointsHaveTheSameData = serializedObject.FindProperty("_allWaypointsHaveTheSameData");

		#region Values for same data in case it's needed
		_timeToReachForAllWaypoints = serializedObject.FindProperty("_timeToReachForAllWaypoints");
		_areAccelerated = serializedObject.FindProperty("_areAccelerated");
		_shouldThemStop = serializedObject.FindProperty("_shouldThemStop");
		_stopTime = serializedObject.FindProperty("_stopTime");
		_easingFunctionForAllWaypoints = serializedObject.FindProperty("_easingFunctionForAllWaypoints");
		#endregion
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(_usageWay, _usageWayLabel);
		if (_usageWay.intValue == 1)
		{
			EditorGUILayout.PropertyField(_randomArea, _randomAreaLabel);
			_timeBetweenRandomPoints.floatValue = Mathf.Max(0, _timeBetweenRandomPoints.floatValue);
			EditorGUILayout.PropertyField(_timeBetweenRandomPoints, _timeBetweenWaypointsLabel);
		}
		else
		{
			EditorGUILayout.PropertyField(_isACicle, _isACicleLabel);
			EditorGUILayout.PropertyField(_allWaypointsHaveTheSameData, _allTheSameDataLabel);
			EditorGUI.indentLevel++;
			if (_allWaypointsHaveTheSameData.boolValue)
			{
				_showAllSameData = EditorGUILayout.Foldout(_showAllSameData, "Waypoints Data", true);
				if (_showAllSameData)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(_timeToReachForAllWaypoints, _timeToReachEveryWaypointLabel);
					_timeToReachForAllWaypoints.floatValue = Mathf.Max(0, _timeToReachForAllWaypoints.floatValue);

					EditorGUILayout.PropertyField(_areAccelerated, _areAcceleratedLabel);
					if (_areAccelerated.boolValue)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(_easingFunctionForAllWaypoints, _easingFunctionToAPointLabel);
						EasingFunction.Ease easingEnum = (EasingFunction.Ease)_easingFunctionForAllWaypoints.intValue;
						EditorGUILayout.LabelField("X-axis: Time, Y-axis: Position");
                        DrawEasingCurve(easingEnum, new Vector2(45, 15), new Vector2(65, 2), "X: Time", "Y: Position ", new Vector2(40, 20), new Vector2(60, 20));
                        EditorGUI.indentLevel--;
					}
					EditorGUILayout.PropertyField(_shouldThemStop, _shouldThemStopLabel);
					if (_shouldThemStop.boolValue)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(_stopTime, _stopDurationLabel);
						_stopTime.floatValue = Mathf.Max(0, _stopTime.floatValue);
						EditorGUI.indentLevel--;
					}
					EditorGUI.indentLevel--;
				}
			}
			_showWaypointsData = EditorGUILayout.Foldout(_showWaypointsData, "Waypoints", true);
			if (_showWaypointsData)
			{
				EditorGUI.indentLevel++;
				_waypointsData.arraySize = EditorGUILayout.IntField(_sizeLabel, _waypointsData.arraySize);
				EditorGUI.indentLevel++;
				for (int i = 0; i < _waypointsData.arraySize; i++)
				{
					var waypoint = _waypointsData.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(waypoint, new GUIContent("Waypoint " + i), false);
					if (waypoint.isExpanded)
					{
						EditorGUI.indentLevel++;

						var waypointTransform = waypoint.FindPropertyRelative("waypoint");
						EditorGUILayout.PropertyField(waypointTransform, _waypointTransformLabel);

						if (!_allWaypointsHaveTheSameData.boolValue)
						{
							var timeToReach = waypoint.FindPropertyRelative("timeToReach");
							timeToReach.floatValue = Mathf.Max(0, timeToReach.floatValue);
							EditorGUILayout.PropertyField(timeToReach, _timeToReachLabel);

							var isAccelerated = waypoint.FindPropertyRelative("isAccelerated");
							EditorGUILayout.PropertyField(isAccelerated, _isAcceleratedLabel);

							if (isAccelerated.boolValue)
							{
								var easingFunctionProp = waypoint.FindPropertyRelative("easingFunction");
								EditorGUILayout.PropertyField(easingFunctionProp, _easingFunctionToAPointLabel);
								EasingFunction.Ease easingEnum = (EasingFunction.Ease)easingFunctionProp.intValue;
                                DrawEasingCurve(easingEnum, new Vector2(45, 15), new Vector2(65, 2), "X: Time", "Y: Position ", new Vector2(40, 20), new Vector2(60, 20));
                            }

							var shouldStop = waypoint.FindPropertyRelative("shouldStop");
							EditorGUILayout.PropertyField(shouldStop, _shouldStopLabel);
							if (shouldStop.boolValue)
							{
								EditorGUI.indentLevel++;
								var stopDuration = waypoint.FindPropertyRelative("stopDuration");
								stopDuration.floatValue = Mathf.Max(0f, EditorGUILayout.FloatField(_stopDurationLabel, stopDuration.floatValue));
							}
							EditorGUI.indentLevel--;
						}
						else
						{
							var timeToReach = waypoint.FindPropertyRelative("timeToReach");
							timeToReach.floatValue = _timeToReachForAllWaypoints.floatValue;
							var isAccelerated = waypoint.FindPropertyRelative("isAccelerated");
							isAccelerated.boolValue = _areAccelerated.boolValue;
							if (isAccelerated.boolValue)
							{
								var easingFunctionProp = waypoint.FindPropertyRelative("easingFunction");
								easingFunctionProp.intValue = (int)_easingFunctionForAllWaypoints.enumValueIndex;
							}
							var shouldStop = waypoint.FindPropertyRelative("shouldStop");
							shouldStop.boolValue = _shouldThemStop.boolValue;
							if (shouldStop.boolValue)
							{
								var stopDuration = waypoint.FindPropertyRelative("stopDuration");
								stopDuration.floatValue = _stopTime.floatValue;
							}
						}
						EditorGUI.indentLevel--;
					}
				}
				EditorGUI.indentLevel--;
				EditorGUI.indentLevel--;
			}
		}
		serializedObject.ApplyModifiedProperties();
	}
   
}
