using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Life))]
public class LifeEditor : Editor
{
	SerializedProperty _initialLife;
	SerializedProperty _maxLife;
	SerializedProperty _entityType;
	SerializedProperty _textName;
	SerializedProperty _lifeText;

	private static readonly GUIContent _initialLifeLabel = new GUIContent("Initial Life", "Amount of life the entity will have since it's creation.");
	private static readonly GUIContent _maxLifeLabel = new GUIContent("Max Life", "Maximun amount of life the entity will have.");
	private static readonly GUIContent _entityTypeLabel = new GUIContent("Entity Type", "Whether the entity is an Enemy or the Player.");
	private static readonly GUIContent _textNameLabel = new GUIContent("Text Name", "Text that will be shown before the life amount.");
	private static readonly GUIContent _lifeTextLabel = new GUIContent("Life Text", "Reference to the text that will show the life amount.");

	private void OnEnable()
	{
		_initialLife = serializedObject.FindProperty("_initialLife");
        _maxLife = serializedObject.FindProperty("_maxLife");
		_entityType = serializedObject.FindProperty("_entityType");
		_textName = serializedObject.FindProperty("_textName");
		_lifeText = serializedObject.FindProperty("_lifeText");
	}
	public override void OnInspectorGUI()
    {
      
        serializedObject.Update();
		EditorGUILayout.LabelField("Life Settings", EditorStyles.boldLabel);
		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField(_initialLife, _initialLifeLabel);
		EditorGUILayout.PropertyField(_maxLife, _maxLifeLabel);
		EditorGUILayout.PropertyField(_entityType, _entityTypeLabel);
		EditorGUI.indentLevel--;


		if (_entityType.intValue == 1)
        {
            EditorGUILayout.LabelField("UI Settings", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_textName, _textNameLabel);
			EditorGUILayout.PropertyField(_lifeText, _lifeTextLabel);
			EditorGUI.indentLevel--;
		}
		serializedObject.ApplyModifiedProperties();
	}
}
