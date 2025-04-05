using UnityEditor;
using UnityEngine;
using static DistanceSensor;

[CustomEditor(typeof(DistanceSensor))]
public class DistanceSensorEditor : Editor
{
    private static readonly GUIContent _distanceTypeLabel = new GUIContent("Distance Type", "Select the type of distance measurement.");
    private static readonly GUIContent _axisLabel = new GUIContent("Axis", "Choose the axis along which distance is measured.");
    private static readonly GUIContent _partOfAxisLabel = new GUIContent("Part of Axis", "Defines whether detection is on one side or the other.");
    private static readonly GUIContent _detectionSideLabel = new GUIContent("Detection Sides", "Defines whether detection is on one side or both sides of the axis.");
    private static readonly GUIContent _detectionDistanceLabel = new GUIContent("Detection Distance", "The threshold distance for detection.");
    private static readonly GUIContent _targetLabel = new GUIContent("Target", "The object to measure distance from.");
    private static readonly GUIContent _areaTriggerLabel = new GUIContent("Area Trigger", "External trigger used for area-based detection.");
    private static readonly GUIContent _startDetectingTimeLabel = new GUIContent("Start Detecting Time", "Initial time the sensor will need to be active");
    private static readonly GUIContent _detectionConditionLabel = new GUIContent("Detection Condition", "Specifies whether the target object is detected when it is inside or outside the defined magnitude range.");

    private SerializedProperty _distanceType;
    private SerializedProperty _axis;
    private SerializedProperty _partOfAxis;
    private SerializedProperty _detectionSide;
    private SerializedProperty _detectionDistance;
    private SerializedProperty _target;
    private SerializedProperty _areaTrigger;
	private SerializedProperty _startDetectingTime;
	private SerializedProperty _detectionCondition;

	private void OnEnable()
    {
        _distanceType = serializedObject.FindProperty("_distanceType");
        _axis = serializedObject.FindProperty("_axis");
        _partOfAxis = serializedObject.FindProperty("_partOfAxis");
        _detectionSide = serializedObject.FindProperty("_detectionSide");
        _detectionDistance = serializedObject.FindProperty("_detectionDistance");
        _target = serializedObject.FindProperty("_target");
        _areaTrigger = serializedObject.FindProperty("_areaTrigger");
        _startDetectingTime = serializedObject.FindProperty("_startDetectingTime");
		_detectionCondition = serializedObject.FindProperty("_detectionCondition");

	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_distanceType, _distanceTypeLabel);
		EditorGUILayout.PropertyField(_detectionCondition, _detectionConditionLabel);
		EditorGUILayout.PropertyField(_target, _targetLabel);
        EditorGUILayout.PropertyField(_startDetectingTime, _startDetectingTimeLabel);
     
        switch (_distanceType.intValue)
        {
            case (int)TypeOfDistance.SingleAxis:
				EditorGUILayout.PropertyField(_detectionDistance, _detectionDistanceLabel);
				EditorGUILayout.PropertyField(_axis, _axisLabel);
				EditorGUILayout.PropertyField(_detectionSide, _detectionSideLabel);
				if ((DetectionSide)_detectionSide.intValue != DetectionSide.Both)
				{
					// Aquí aplicamos la lógica para restringir las opciones del enum PartOfAxis
					string[] options;
					int[] values = new int[] { 0, 1 }; ;

					if (_axis.intValue == 0) // Si Axis es 1, permitimos Up (0) y Down (1)
					{
						options = new string[] { "Up", "Down" };
						
					}
					else // Si Axis es 0, permitimos Right (2) y Left (3)
					{
						options = new string[] { "Left", "Right" };
					}

					// Obtener el índice actual basado en el valor almacenado
					int currentIndex = System.Array.IndexOf(values, _partOfAxis.intValue);
					if (currentIndex == -1) currentIndex = 0; // En caso de valor inválido, seleccionar el primero disponible

					// Dibujar el popup personalizado
					int selectedIndex = EditorGUILayout.Popup(_partOfAxisLabel, currentIndex, options);

					// Asignar el nuevo valor seleccionado
					_partOfAxis.intValue = values[selectedIndex];
				}
				break;
            case (int)TypeOfDistance.Magnitude:
				EditorGUILayout.PropertyField(_detectionDistance, _detectionDistanceLabel);
				break;
            case (int)TypeOfDistance.Area:
				EditorGUILayout.PropertyField(_areaTrigger, _areaTriggerLabel);
				break;

        }
       serializedObject.ApplyModifiedProperties();
    }
}
