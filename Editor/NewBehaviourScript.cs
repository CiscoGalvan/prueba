using UnityEditor;
using UnityEngine;
using System.IO;

public class EnemyBehaviorToolInstaller
{
	private const string PackagePath = "Packages/EnemyBehaviorTool";
	private const string TargetPath = "Assets/EnemyBehaviorTool";

	[MenuItem("Tools/Enemy Behavior Tool/Instalar Contenido")]
	public static void InstallContent()
	{
		if (!Directory.Exists(PackagePath))
		{
			Debug.LogWarning("No se encontró la carpeta de contenido en el paquete.");
			return;
		}

		if (Directory.Exists(TargetPath))
		{
			if (!EditorUtility.DisplayDialog("Sobrescribir contenido",
				"Ya existe una carpeta en 'Assets/EnemyBehaviorTool'. ¿Quieres sobrescribirla?",
				"Sí", "No"))
			{
				return;
			}
			Directory.Delete(TargetPath, true);
		}

		FileUtil.CopyFileOrDirectory(PackagePath, TargetPath);
		AssetDatabase.Refresh();
		Debug.Log("Contenido copiado a: " + TargetPath);
	}
}
