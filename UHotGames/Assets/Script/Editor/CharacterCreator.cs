using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterCreator : EditorWindow
{
	[MenuItem("MyTools/生成角色")]
	public static void ShowWindow()
	{
		EditorWindow thisWindow = EditorWindow.GetWindow(typeof(CharacterCreator));
		thisWindow.titleContent = new GUIContent("生成角色");
	}
	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	Vector2 scrollPos;
	Texture2D selectedSprite;
	void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		selectedSprite = EditorGUILayout.ObjectField(selectedSprite, typeof(Texture2D)) as Texture2D;
		if (selectedSprite != null)
		{
			if (GUILayout.Button("Create", GUILayout.Width(120)))
			{
				var sps = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(selectedSprite));
				var all = new List<Sprite>();
				foreach (var sp in sps)
				{
					if (sp.name.StartsWith(selectedSprite.name + "_"))
					{
						all.Add(sp as Sprite);
					}
				}
				for (var i = 0; i < 4; i++)
				{
					var gobj = new GameObject();
					
				}
				Debug.Log($"all sprites count {all.Count}");
			}
		}
		EditorGUILayout.EndScrollView();
	}
}
