using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HorizontalActuator))]
public class HorizontalComponentEditor : ActuatorEditor
{
    private static readonly GUIContent _onCollisionReactionLabel = new GUIContent("Reaction After Collision", "What will the object do after collision?\n" +
        "None: The object will not react to the collision.\n" +
        "Bounce: The object will bounce to the opposite direction.\n" +
        "Destroy: The object will be destroyed after the contact.");
    private static readonly GUIContent _directionLabel = new GUIContent("Direction", "Direction of the horizontal movement");
    private static readonly GUIContent _followPlayerLabel = new GUIContent("Follow Player", "Direction of the horizontal movement will be the nearest to the player");
    private static readonly GUIContent _layersToCollideLabel = new GUIContent("Layers To Collide", "Layers that will activate the Reaction After Collision event in case there is a collision.");
    private bool _showMovementInfo = true;

    #region Accelerated movement
    private static readonly GUIContent _goalSpeedLabel = new GUIContent("Goal Speed", "Speed the object will reach");
	private static readonly GUIContent _interpolationTimeLabel = new GUIContent("Interpolation Time", "Time it takes to reach Goal Speed");
	private static readonly GUIContent _isAcceleratedLabel = new GUIContent("Is Accelerated", "Is the movement towards the waypoint accelerated?");
	#endregion

	#region  Non-accelerated movement
	private static readonly GUIContent _throwLabel = new GUIContent("Throw", "The object will be moved only once, when the actuator is activated.");
    private static readonly GUIContent _constantSpeedLabel = new GUIContent("Speed", "The object will move with this constant speed.");
    #endregion

    private SerializedProperty _followPlayerProperty;
    private SerializedProperty _directionProperty;
    private SerializedProperty _interpolationTime;
    private SerializedProperty _onCollisionReaction;
    private SerializedProperty _throw;
    private SerializedProperty _layersToCollide;
    private SerializedProperty _isAccelerated;
    private SerializedProperty _goalSpeed;
    private SerializedProperty _easingFunction;
    private SerializedProperty _speed;


    private void OnEnable()
    {
        _followPlayerProperty = serializedObject.FindProperty("_followPlayer");
        _directionProperty = serializedObject.FindProperty("_direction");
        _onCollisionReaction = serializedObject.FindProperty("_onCollisionReaction");
        _throw = serializedObject.FindProperty("_throw");
        _layersToCollide = serializedObject.FindProperty("_layersToCollide");
		_isAccelerated = serializedObject.FindProperty("_isAccelerated");
		_goalSpeed = serializedObject.FindProperty("_goalSpeed");
		_interpolationTime = serializedObject.FindProperty("_interpolationTime");
		_easingFunction = serializedObject.FindProperty("_easingFunction");
		_speed = serializedObject.FindProperty("_speed");
    }

    public override void OnInspectorGUI()
    {
		serializedObject.Update();
		EditorGUILayout.PropertyField(_onCollisionReaction, _onCollisionReactionLabel);
        if (_onCollisionReaction.intValue != 0)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_layersToCollide, _layersToCollideLabel);
            EditorGUI.indentLevel--;
        }

        _showMovementInfo = EditorGUILayout.Foldout(_showMovementInfo, "Movement Info", true);
        EditorGUI.indentLevel++;
        if (_showMovementInfo)
        {
            EditorGUILayout.PropertyField(_followPlayerProperty, _followPlayerLabel);
            if (!_followPlayerProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_directionProperty, _directionLabel);
            }


            if (_followPlayerProperty.boolValue && _onCollisionReaction.intValue == 1) // 1 = Bounce
            {
                EditorGUILayout.HelpBox("The object won't bounce off collisions while following the player.", MessageType.Warning);
            }
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
                EditorGUILayout.PropertyField(_throw, _throwLabel);
				_speed.floatValue = Mathf.Max(0, _speed.floatValue);
				EditorGUILayout.PropertyField(_speed, _constantSpeedLabel);
			}
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
     
}
