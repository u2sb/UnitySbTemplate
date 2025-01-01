using System;
using System.Collections.Generic;
using Sb.UnityKit.SbGameObject;
using UnityEditor;
using UnityEngine;

namespace Sb.UnityKit.Editor.SbGameObject;

public class SbGameObjectFinder : EditorWindow
{
  private readonly List<GameObject> _matchingObjects = new();
  private string _searchKeyword = "";
  private bool _showFullPath;
  private bool _useFuzzySearch;

  private void OnGUI()
  {
    GUILayout.Label("SbGameObject 查找", EditorStyles.boldLabel);

    _searchKeyword = EditorGUILayout.TextField("关键词", _searchKeyword);
    _useFuzzySearch = EditorGUILayout.Toggle("模糊搜索", _useFuzzySearch);
    _showFullPath = EditorGUILayout.Toggle("显示完整路径", _showFullPath);

    if (GUILayout.Button("查找")) FindMatchingObjects();

    GUILayout.Label("查找结果:", EditorStyles.boldLabel);

    var leftAlignedStyle = new GUIStyle(GUI.skin.button)
    {
      alignment = TextAnchor.MiddleLeft
    };

    foreach (var obj in _matchingObjects)
    {
      var displayName = _showFullPath ? GetFullPath(obj) : obj.name;
      if (GUILayout.Button(displayName, leftAlignedStyle))
        SelectObjectInScene(obj);
    }
  }

  [MenuItem("U2SB/SbGameObject/Find SbGameObject")]
  public static void ShowWindow()
  {
    GetWindow<SbGameObjectFinder>("Find SbGameObject");
  }

  private void FindMatchingObjects()
  {
    _matchingObjects.Clear();

    foreach (var htGameObject in FindObjectsByType<BaseSbGameObject>(FindObjectsInactive.Include,
               FindObjectsSortMode.InstanceID))
      if (htGameObject.Keys is { Length: > 0 })
      {
        if (_useFuzzySearch)
        {
          if (Array.Exists(htGameObject.Keys, key => key.Contains(_searchKeyword)))
            _matchingObjects.Add(htGameObject.gameObject);
        }
        else
        {
          if (Array.Exists(htGameObject.Keys, key => key == _searchKeyword))
            _matchingObjects.Add(htGameObject.gameObject);
        }
      }
  }

  private static void SelectObjectInScene(GameObject obj)
  {
    Selection.activeObject = obj;
    EditorGUIUtility.PingObject(obj);
  }

  private static string GetFullPath(GameObject obj)
  {
    var path = obj.name;
    var current = obj.transform;
    while (current.parent != null)
    {
      current = current.parent;
      path = current.name + "/" + path;
    }

    return path;
  }
}