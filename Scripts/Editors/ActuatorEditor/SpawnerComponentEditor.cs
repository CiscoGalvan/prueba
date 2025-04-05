using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnerActuator))]
public class SpawnerComponentEditor : Editor
{
    private GUIContent infiniteEnemiesLabel = new GUIContent("Infinite Enemies", "If true, the spawner will spawn enemies indefinitely. If false, it will spawn a limited number of times.");
    private GUIContent numOfEnemiesToSpawnLabel = new GUIContent("Amount Of Spawn Evenet", "The number of times the enemies are spawned.");
    private GUIContent spawnIntervalLabel = new GUIContent("Spawn Interval", "The time interval (in seconds) between each spawn");
    private GUIContent spawnListLabel = new GUIContent("Spawn List", "List of prefabs to spawn and their respective spawn points");

    private SerializedProperty infiniteEnemiesProp;
    private SerializedProperty numOfEnemiesToSpawnProp;
    private SerializedProperty spawnIntervalProp;
    private SerializedProperty spawnListProp;

    private void OnEnable()
    {
        
        infiniteEnemiesProp = serializedObject.FindProperty("_infiniteEnemies");
        numOfEnemiesToSpawnProp = serializedObject.FindProperty("_numofTimesToSpawn");
        spawnIntervalProp = serializedObject.FindProperty("_spawnInterval");
        spawnListProp = serializedObject.FindProperty("_spawnList");
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.PropertyField(infiniteEnemiesProp, infiniteEnemiesLabel);
        if (!infiniteEnemiesProp.boolValue)
          EditorGUILayout.PropertyField(numOfEnemiesToSpawnProp, numOfEnemiesToSpawnLabel);
        
        EditorGUILayout.PropertyField(spawnIntervalProp, spawnIntervalLabel);
        EditorGUILayout.PropertyField(spawnListProp, spawnListLabel);

        serializedObject.ApplyModifiedProperties();
    }
}