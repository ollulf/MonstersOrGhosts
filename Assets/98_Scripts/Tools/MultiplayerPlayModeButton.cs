using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace UnityToolbarExtender
{

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();
;
			if (GUILayout.Button(new GUIContent(EditorGUIUtility.FindTexture("d_Animation.LastKey"), "Play from Multiplayer"), ToolbarStyles.commandButtonStyle))
			{
				if (EditorApplication.isPlaying == true)
				{
					EditorApplication.isPlaying = false;
					return;
				}

				EditorApplication.isPlaying = true;
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene("Assets/00_Scenes/MenuScene.unity");
			}
		}
		static class ToolbarStyles
		{
			public static readonly GUIStyle commandButtonStyle;

			static ToolbarStyles()
			{
				commandButtonStyle = new GUIStyle("Command")
				{
					fontSize = 16,
					alignment = TextAnchor.MiddleCenter,
					imagePosition = ImagePosition.ImageAbove,
					fontStyle = FontStyle.Bold
				};
			}
		}
	}
}
#endif