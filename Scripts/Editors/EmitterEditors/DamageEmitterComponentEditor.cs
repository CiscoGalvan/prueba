using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DamageEmitter))]
public class DamageEmitterComponentEditor : Editor
{
	private SerializedProperty _damageType;
	private SerializedProperty _destroyAfterDoingDamage;
	private SerializedProperty _instaKill;
	private SerializedProperty _amountOfDamage;
	private SerializedProperty _damageCooldown;
	private SerializedProperty _residualDamageAmount;
	private SerializedProperty _numOfDamageApplication;
	private SerializedProperty _activeFromStart;

	private static readonly GUIContent _amountOfDamageLabel = new GUIContent("Damage Amount", "Amount of damage the enemy will deal the player.");
	private static readonly GUIContent _activeFromStartLabel = new GUIContent("Active From Start", "If true, the Damage Emitter won't need to be included in any State in order to activate itself, otherwise if the DamageEmitter is not included in any State and it's not Active From Start it won't deal any damage.");
	
	private static readonly GUIContent _destroyAfterDoingDamageLabel = new GUIContent("Destroy After Doing Damage", "Will the object destroy after doing damage?");
	private static readonly GUIContent _damageCooldownLabel = new GUIContent("Damage Cooldown", "Amount of seconds it will take the player to receive damage again.");
	private static readonly GUIContent _damageTypeLabel = new GUIContent("Damage Type", "How will the damage be dealt?\n" +
		"Instant: The damage will be applied instantly.\n" +
		"Persistent: The damage will be applied while the enemy is in contact with the player.\n" +
		"Residual: There will be a part of the damage applied instantly and another part will be in delivered \"applications\".");
	#region Instant Damage Labels
	private static readonly GUIContent _instaKillLabel = new GUIContent("Instant Kill", "Will the damage eliminate the player ignoring it's current life?");
	#endregion
	#region Residual Damage Labels
	private static readonly GUIContent _residualDamageLabel = new GUIContent("Residual Damage Amount", "Damage taken by the player after the initial collision");
	private static readonly GUIContent _numberOfTicks = new GUIContent("Number Of Applications", "Times the residual damage will be applied");
	private static readonly GUIContent _instantDamageAmount = new GUIContent("Instant Damage Amount", "Amount of damage the enemy will deal the player when they collide");
	#endregion

	private bool _showDamageInfo = true;

	private void OnEnable()
	{
		_damageType = serializedObject.FindProperty("_damageType");
		_destroyAfterDoingDamage = serializedObject.FindProperty("_destroyAfterDoingDamage");
		_instaKill = serializedObject.FindProperty("_instaKill");
		_amountOfDamage = serializedObject.FindProperty("_amountOfDamage");
		_damageCooldown = serializedObject.FindProperty("_damageCooldown");
		_residualDamageAmount = serializedObject.FindProperty("_residualDamageAmount");
		_numOfDamageApplication = serializedObject.FindProperty("_numOfDamageApplication");
		_activeFromStart = serializedObject.FindProperty("_activeFromStart");
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.HelpBox("In case the DamageEmitter is included in the initial state, this checkbox will be automaticly set to true.", MessageType.Info);
		EditorGUILayout.PropertyField(_activeFromStart, _activeFromStartLabel);
		EditorGUILayout.PropertyField(_damageType, _damageTypeLabel);
		EditorGUI.indentLevel++;
		_showDamageInfo = EditorGUILayout.Foldout(_showDamageInfo, "Damage Info", true);
		if (_showDamageInfo)
		{
			EditorGUI.indentLevel++;
			switch (_damageType.intValue)
			{
				case 0:	// Instant
                    EditorGUILayout.PropertyField(_destroyAfterDoingDamage, _destroyAfterDoingDamageLabel);
                    EditorGUILayout.PropertyField(_instaKill, _instaKillLabel);
					if (!_instaKill.boolValue)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(_amountOfDamage, _amountOfDamageLabel);
						EditorGUI.indentLevel--;
					}
					break;
				case 1: //Persistent
					EditorGUILayout.PropertyField(_amountOfDamage, _amountOfDamageLabel);
					EditorGUILayout.PropertyField(_damageCooldown, _damageCooldownLabel);
					break;
				case 2: //Residual
                    EditorGUILayout.PropertyField(_destroyAfterDoingDamage, _destroyAfterDoingDamageLabel);
					EditorGUILayout.PropertyField(_amountOfDamage, _instantDamageAmount);
					EditorGUILayout.PropertyField(_residualDamageAmount, _residualDamageLabel);
					EditorGUILayout.PropertyField(_damageCooldown, _damageCooldownLabel);
					EditorGUILayout.PropertyField(_numOfDamageApplication, _numberOfTicks);
					break;
			}
			EditorGUI.indentLevel--; 
		}
		serializedObject.ApplyModifiedProperties();

	}
}
