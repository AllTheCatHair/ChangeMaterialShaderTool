using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// �ƹ�����������
/// @ȫ��èë
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
    [MenuItem("����_Tool/�ƹ��������� LightingBatchManagementTool V1.0")]


    public static void ShowWindow()
    {
        GetWindow<LightingBatchManagementTool>("LightTool");
    }

    private void OnGUI()
    {
        allLightType = new string[] {"Spot","Directional","Point"};
        if (GUILayout.Button("���¿�ʼ/Restart"))
        {
            Restart();
        }

        if (GUILayout.Button("��ȡ���еƹ�/Load all lights"))
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
            lightsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(lightsFoldout, "���������еĵƹ�/All the lights in the scene");
            if (lightsFoldout)
            {
                for (int i = 0; i < allLightGameOjects.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (!allLightsInfo[i])
                    {
                        if (GUILayout.Button("չ��/Unfold", GUILayout.Width(80)))
                        {
                            allLightsInfo[i] = true;
                        }
                        allLights[i] = EditorGUILayout.ObjectField(allLights[i], typeof(Light), false) as Light;
                        allLights[i].intensity = EditorGUILayout.FloatField("����ǿ��/Intensity", allLights[i].intensity);
                        GUILayout.EndHorizontal();

                    }
                    else
                    {
                        if (GUILayout.Button("�۵�/Fold",GUILayout.Width(80)))
                        {
                            allLightsInfo[i] = false;
                        }
                        allLights[i] = EditorGUILayout.ObjectField(allLights[i], typeof(Light), false) as Light;

                        allLights[i].intensity = EditorGUILayout.FloatField("����ǿ��/Intensity", allLights[i].intensity);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        allLights[i].color = EditorGUILayout.ColorField("��ɫ/Color", allLights[i].color);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        allLights[i].range = EditorGUILayout.FloatField("��Χ/Range", allLights[i].range);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("����/Type");
                        allLights[i].type = (LightType)EditorGUILayout.Popup((byte)allLights[i].type,allLightType, GUILayout.Width(160));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ģʽ/Mode");
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
