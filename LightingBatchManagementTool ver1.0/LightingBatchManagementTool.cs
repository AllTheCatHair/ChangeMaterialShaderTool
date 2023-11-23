using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 灯光批量管理工具
/// @全是猫毛
/// Github https://github.com/AllTheCatHair/ChangeMaterialShaderTool
/// </summary>
public class LightingBatchManagementTool : EditorWindow
{
    private string[] allLightType;
    private bool lightsFoldout = true;
    private Vector2 scrollposition = Vector2.zero;
    List<GameObject> allobjs = new List<GameObject>();
    GameObject[] allGameObjects;
    GameObject[] allLightGameOjects;
    List<bool> allLightsInfo = new List<bool>();
    List<Light> allLights = new List<Light>();
    [MenuItem("工具_Tool/灯光批量管理 LightingBatchManagementTool V1.0")]


    public static void ShowWindow()
    {
        GetWindow<LightingBatchManagementTool>("LightTool");
    }

    private void OnGUI()
    {
        allLightType = new string[] {"Spot","Directional","Point"};
        if (GUILayout.Button("重新开始/Restart"))
        {
            Restart();
        }

        if (GUILayout.Button("读取所有灯光/Load all lights"))
        {
            allGameObjects = EnumerateAllObjects();
            allLightGameOjects = FindLightObjects(allGameObjects);
            foreach (GameObject obj in allLightGameOjects)
            {
                allLights.Add(obj.GetComponent<Light>());
                allLightsInfo.Add(false);
            }
        }
        scrollposition = EditorGUILayout.BeginScrollView(scrollposition);

        if (allLightGameOjects != null && allLightGameOjects.Length > 0)
        {
            lightsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(lightsFoldout, "场景内所有的灯光/All the lights in the scene");
            if (lightsFoldout)
            {
                for (int i = 0; i < allLightGameOjects.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (!allLightsInfo[i])
                    {
                        if (GUILayout.Button("展开/Unfold", GUILayout.Width(80)))
                        {
                            allLightsInfo[i] = true;
                        }
                        allLights[i] = EditorGUILayout.ObjectField(allLights[i], typeof(Light), false) as Light;
                        allLights[i].intensity = EditorGUILayout.FloatField("光照强度/Intensity", allLights[i].intensity);
                        GUILayout.EndHorizontal();

                    }
                    else
                    {
                        if (GUILayout.Button("折叠/Fold",GUILayout.Width(80)))
                        {
                            allLightsInfo[i] = false;
                        }
                        allLights[i] = EditorGUILayout.ObjectField(allLights[i], typeof(Light), false) as Light;

                        allLights[i].intensity = EditorGUILayout.FloatField("光照强度/Intensity", allLights[i].intensity);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        allLights[i].color = EditorGUILayout.ColorField("颜色/Color", allLights[i].color);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        allLights[i].range = EditorGUILayout.FloatField("范围/Range", allLights[i].range);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("类型/Type");
                        allLights[i].type = (LightType)EditorGUILayout.Popup((byte)allLights[i].type,allLightType, GUILayout.Width(160));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("模式/Mode");
                        Debug.Log(allLights[i].lightmapBakeType);
                        allLights[i].lightmapBakeType = (LightmapBakeType)EditorGUILayout.EnumPopup(allLights[i].lightmapBakeType, GUILayout.Width(160));
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        EditorGUILayout.EndScrollView();
    }
    private void Restart()
    {
        lightsFoldout = true;
        scrollposition = Vector2.zero;
        allobjs = new List<GameObject>();
        allGameObjects = null;
        allLightGameOjects = null;
        allLightsInfo = new List<bool>();
        allLights = new List<Light>();
    }
    private GameObject[] EnumerateAllObjects()
    {
        GameObject[] output = GameObject.FindObjectsOfType<GameObject>();
        Debug.Log(output.Length);
        return output;
    }
    private GameObject[] FindLightObjects(GameObject[] objs)
    {
        List<GameObject> output = new List<GameObject>();
        Debug.Log(objs.Length);
        foreach (GameObject obj in objs)
        {
            Light light = obj.GetComponent<Light>();
            if (light != null)
            {
                output.Add(obj);
            }
        }
        return output.ToArray();

    }
}
