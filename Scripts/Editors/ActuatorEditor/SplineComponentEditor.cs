using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineFollowerActuator))]
public class SplineFollowetEditor : ActuatorEditor
{
   
    private bool _showMovementInfo = true;
    private static readonly GUIContent _splineContainerLabel = new GUIContent("Spline Container", "The reference to the spline that will follow");
    private static readonly GUIContent _teleportToClosestPointLabel = new GUIContent("Teleport to Colosest Point", "if Enemy: the enemy will move to the spline. If Spline: the spline will move to de Enemy");
    #region Accelerated movement
    private static readonly GUIContent _goalSpeedLabel = new GUIContent("Goal Speed", "Speed the object will reach");
	private static readonly GUIContent _interpolationTimeLabel = new GUIContent("Interpolation Time", "Time it takes to reach Goal Speed");
	private static readonly GUIContent _isAcceleratedLabel = new GUIContent("Is Accelerated", "Is the movement towards the waypoint accelerated?");
	#endregion

	#region  Non-accelerated movement
    private static readonly GUIContent _constantSpeedLabel = new GUIContent("Speed", "The object will move with this constant speed.");
    #endregion

    
    private SerializedProperty _splineContainerTime;
    private SerializedProperty _teleportToClosestPointTime;
    private SerializedProperty _interpolationTime;
    private SerializedProperty _isAccelerated;
    private SerializedProperty _goalSpeed;
    private SerializedProperty _easingFunction;
    private SerializedProperty _speed;


    private void OnEnable()
    {
        _splineContainerTime = serializedObject.FindProperty("_splineContainer");
        _teleportToClosestPointTime = serializedObject.FindProperty("_teleportToClosestPoint");
        _isAccelerated = serializedObject.FindProperty("_isAccelerated");
		_goalSpeed = serializedObject.FindProperty("_goalSpeed");
		_interpolationTime = serializedObject.FindProperty("_interpolationTime");
		_easingFunction = serializedObject.FindProperty("_easingFunction");
		_speed = serializedObject.FindProperty("_speed");
    }

    public override void OnInspectorGUI()
    {
       
        serializedObject.Update();
        EditorGUILayout.PropertyField(_splineContainerTime, _splineContainerLabel);
        EditorGUILayout.PropertyField(_teleportToClosestPointTime, _teleportToClosestPointLabel);
        _showMovementInfo = EditorGUILayout.Foldout(_showMovementInfo, "Movement Info", true);
        EditorGUI.indentLevel++;
        if (_showMovementInfo)
        {
           
			EditorGUILayout.PropertyField(_isAccelerated, _isAcceleratedLabel);
            if (_isAccelerated.boolValue)
            {
                _goalSpeed.floatValue = Mathf.Max(0, _goalSpeed.floatValue);
                EditorGUILayout.PropertyField(_goalSpeed, _goalSpeedLabel);


                _interpolationTime.floatValue = Mathf.Max(0, _interpolationTime.floatValue);
                EditorGUILayout.PropertyField(_interpolationTime, _interpolationTimeLabel);

                EditorGUILayout.PropertyField(_easingFunction, _easingFunctionLabel);
                EditorGUI.indentLevel++;
                EasingFunction.Ease easingEnum = (EasingFunction.Ease)_easingFunction.intValue;
                DrawEasingCurve(easingEnum, new Vector2(45, 15), new Vector2(30, 2), "X: Time", "Y: Speed ", new Vector2(40, 20), new Vector2(60, 20));
                EditorGUI.indentLevel--;
            }
            else
            {
                _speed.floatValue = Mathf.Max(0, _speed.floatValue);
                EditorGUILayout.PropertyField(_speed, _constantSpeedLabel);
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
     
}
