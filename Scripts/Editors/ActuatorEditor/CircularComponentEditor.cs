using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircularActuator))]
public class CircularComponentEditor : ActuatorEditor
{
	#region Accelerated movement
	private static readonly GUIContent _goalAngularSpeedLabel = new GUIContent("Goal Speed", "Speed the object will reach");
	private static readonly GUIContent _interpolationTimeLabel = new GUIContent("Interpolation Time", "Time it takes to reach Max Speed");
	private static readonly GUIContent _isAcceleratedLabel = new GUIContent("Is Accelerated", "Is the movement towards the waypoint accelerated?");
	#endregion

	#region  Non-accelerated movement
	private static readonly GUIContent _angularSpeedLabel = new GUIContent("Speed", "The object will move with this constant speed.");
	#endregion

	private static readonly GUIContent _pointPlayerLabel = new GUIContent("Point Player", "The object will rotate considering the player's position." +
		"There must be an object with Player label, if not the Actuator won't work.");
	private static readonly GUIContent _rotationPointPositionLabel = new GUIContent("Rotation Point", "Center of rotation (if not specified, uses the object's initial position)");
	private static readonly GUIContent _maxAngleLabel= new GUIContent("Max Angle", "Maximum allowed angle in degrees. Use 360 for a full circle, less for pendulum-like motion");
	private static readonly GUIContent _canRotateLabel= new GUIContent("Can Rotate", "In case it is true, the entire object will rotate with it's movement");

	private SerializedProperty _pointPlayer;
	private SerializedProperty _isAccelerated;
	private SerializedProperty _angularSpeed;
	private SerializedProperty _goalAngularSpeed;
	private SerializedProperty _interpolationTime;
	private SerializedProperty _easingFunction;
	private SerializedProperty _rotationPointPosition;
	private SerializedProperty _maxAngle;
	private SerializedProperty _currentAngularSpeed;
	private SerializedProperty _canRotate;
	private void OnEnable()
	{
		_pointPlayer = serializedObject.FindProperty("_pointPlayer");
		_isAccelerated = serializedObject.FindProperty("_isAccelerated");
		_angularSpeed = serializedObject.FindProperty("_angularSpeed");
		_goalAngularSpeed = serializedObject.FindProperty("_goalAngularSpeed");
		_interpolationTime = serializedObject.FindProperty("_interpolationTime");
		_easingFunction = serializedObject.FindProperty("_easingFunction");
		_rotationPointPosition = serializedObject.FindProperty("_rotationPointPosition");
		_maxAngle = serializedObject.FindProperty("_maxAngle");
		_currentAngularSpeed = serializedObject.FindProperty("_currentAngularSpeed");
		_canRotate = serializedObject.FindProperty("_canRotate");
	}
	public override void OnInspectorGUI()
	{

	    serializedObject.Update();
		EditorGUILayout.PropertyField(_rotationPointPosition, _rotationPointPositionLabel);
		EditorGUILayout.PropertyField(_pointPlayer, _pointPlayerLabel);
		EditorGUILayout.PropertyField(_canRotate, _canRotateLabel);
		if (!_pointPlayer.boolValue)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_isAccelerated, _isAcceleratedLabel);

			EditorGUILayout.PropertyField(_maxAngle, _maxAngleLabel);
			if (_isAccelerated.boolValue)
			{
				_goalAngularSpeed.floatValue = Mathf.Max(0, _goalAngularSpeed.floatValue);
				EditorGUILayout.PropertyField(_goalAngularSpeed, _goalAngularSpeedLabel);


				_interpolationTime.floatValue = Mathf.Max(0, _interpolationTime.floatValue);
				EditorGUILayout.PropertyField(_interpolationTime, _interpolationTimeLabel);

				EditorGUILayout.PropertyField(_easingFunction, _easingFunctionLabel);
				EditorGUI.indentLevel++;
				EasingFunction.Ease easingEnum = (EasingFunction.Ease)_easingFunction.intValue;
				DrawEasingCurve(easingEnum, new Vector2(45, 15), new Vector2(65, 2), "X: Time", "Y: Angular Velocity ", new Vector2(80, 20), new Vector2(60, 20));
				EditorGUI.indentLevel--;
			}
			else
			{
				_angularSpeed.floatValue = Mathf.Max(0, _angularSpeed.floatValue);
				EditorGUILayout.PropertyField(_angularSpeed, _angularSpeedLabel);

				_currentAngularSpeed.floatValue = _angularSpeed.floatValue * Mathf.Deg2Rad;
			}
			EditorGUI.indentLevel--;
		}
		
		
		
		serializedObject.ApplyModifiedProperties();
	}
}
