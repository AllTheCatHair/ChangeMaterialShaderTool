using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

/// <summary>
/// 批量更换材质着色器工具
/// 使用前记得备份
/// 使用前记得备份
/// 使用前记得备份
/// 
/// @@@全是猫毛  
/// Github https://github.com/AllTheCatHair/ChangeMaterialShaderTool
/// </summary>
public class ChangeMaterialShaderTool : EditorWindow
{
    private Vector2 scrollposition = Vector2.zero;
    private List<Material> materials;
    private Dictionary<Shader, List<bool>> shaderPropertyIsUsed = new Dictionary<Shader, List<bool>>();
    private List<Shader> shaders;
    private List<Shader> targetshaders;
    private Dictionary<int, int[]> choosedata = new Dictionary<int, int[]>();


    [MenuItem("工具_Tool/批量更换材质着色器 ChangeMaterialShaderTool V2.7")]
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        //GetWindow<ChangeMaterialShaderTool>("ChangeMaterialShader");
        Rect windowRect = new Rect(100, 100, 500, 800); // x, y, width, height
        ChangeMaterialShaderTool window = (ChangeMaterialShaderTool)EditorWindow.GetWindow(typeof(ChangeMaterialShaderTool), false, "ChangeMaterialShaderTool", true);
        window.position = windowRect;
        window.Show();
    }
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 13;



        scrollposition = EditorGUILayout.BeginScrollView(scrollposition);
        GUILayout.BeginHorizontal();
        GUILayout.Label("注意提前备份/Please backup in advance");
        if (GUILayout.Button("重新开始/Restart", style, GUILayout.Height(25)))
        {
            Clear(true);
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("添加选中的材质/Add The Selected Materials", style, GUILayout.Height(25)) && Selection.objects != null && Selection.objects.Length > 0)
        {
            Refresh();
        }
        if (materials == null || materials.Count == 0)
        {
            EditorGUILayout.EndScrollView();
            return;
        }
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i] = EditorGUILayout.ObjectField(materials[i], typeof(Material), false) as Material;
        }
        if (materials != null && materials.Count > 0)
        {
            if (GUILayout.Button("读取着色器/Load Shader", style, GUILayout.Height(25)))
            {
                Next();
            }
        }


        GUILayout.Label(" ");
        GUILayout.Label("部分shader中需要将Range和Float互相转换");
        GUILayout.BeginHorizontal();
        GUILayout.Label("当前着色器/Current shader");
        GUILayout.Label("目标着色器Target shader");
        GUILayout.EndHorizontal();

        //if (GUILayout.Button("Refresh"))
        //{
        //    Repaint();
        //}
        if (shaders != null && shaders.Count > 0)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                GUILayout.BeginHorizontal();
                shaders[i] = EditorGUILayout.ObjectField(shaders[i], typeof(Shader), false) as Shader;
                targetshaders[i] = EditorGUILayout.ObjectField(targetshaders[i], typeof(Shader), false) as Shader;
                GUILayout.EndHorizontal();

                if (shaders[i] != null && targetshaders[i] != null)
                {
                    if (!choosedata.ContainsKey(i))
                    {
                        choosedata[i] = new int[ShaderUtil.GetPropertyCount(shaders[i])];
                        //for(int j = 0; j < choosedata[i].Length; j++)
                        //{
                        //    choosedata[i][j] = -1;
                        //}
                    }
                    PrintShaderProperties(shaders[i], targetshaders[i], choosedata[i]);
                }
            }
        }
        GUIStyle startbtnstyle = new GUIStyle(GUI.skin.button);
        startbtnstyle.fontSize = 30;
        if (GUILayout.Button("开始更换,请提前备份\nStart. Please backup in advance", startbtnstyle, GUILayout.Height(100)))
        {
            Transition();
            Clear(false);
        }
        EditorGUILayout.EndScrollView();
    }
    private void Clear(bool all)
    {
        if (all)
        {
            materials = new List<Material>();
        }
        shaders = new List<Shader>();
        targetshaders = new List<Shader>();
        choosedata = new Dictionary<int, int[]>();
        shaderPropertyIsUsed = new Dictionary<Shader, List<bool>>();
    }
    private void Transition()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            Shader sha = materials[i].shader;
            Material lastMat = new Material(materials[i]);
            int index = shaders.IndexOf(sha);
            if (targetshaders[index] == null)
            {
                continue;
            }
            materials[i].shader = targetshaders[index];
            int propertycount = ShaderUtil.GetPropertyCount(materials[i].shader);
            //List<string> propertyName = new List<string>();

            for (int j = 0; j < propertycount; j++)
            {

                ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(materials[i].shader, j);
                string propertyName = ShaderUtil.GetPropertyName(materials[i].shader, j);
                switch (propertyType)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        if (Array.IndexOf(choosedata[index], j + 1) != -1 && Array.IndexOf(choosedata[index], j + 1) != 0)
                        {
                            materials[i].SetColor(propertyName, lastMat.GetColor(ShaderUtil.GetPropertyName(lastMat.shader, Array.IndexOf(choosedata[index], j + 1))));
                        }
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        if (Array.IndexOf(choosedata[index], j + 1) != -1 && Array.IndexOf(choosedata[index], j + 1) != 0)
                        {
                            materials[i].SetVector(propertyName, lastMat.GetVector(ShaderUtil.GetPropertyName(lastMat.shader, Array.IndexOf(choosedata[index], j + 1))));
                        }
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                        if (Array.IndexOf(choosedata[index], j + 1) != -1 && Array.IndexOf(choosedata[index], j + 1) != 0)
                        {
                            materials[i].SetFloat(propertyName, lastMat.GetFloat(ShaderUtil.GetPropertyName(lastMat.shader, Array.IndexOf(choosedata[index], j + 1))));
                        }
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                        if (Array.IndexOf(choosedata[index], j + 1) != -1 && Array.IndexOf(choosedata[index], j + 1) != 0)
                        {
                            materials[i].SetFloat(propertyName, lastMat.GetFloat(ShaderUtil.GetPropertyName(lastMat.shader, Array.IndexOf(choosedata[index], j + 1))));
                        }
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        if (Array.IndexOf(choosedata[index], j + 1) != -1 && Array.IndexOf(choosedata[index], j + 1) != 0)
                        {
                            materials[i].SetTexture(propertyName, lastMat.GetTexture(ShaderUtil.GetPropertyName(lastMat.shader, Array.IndexOf(choosedata[index], j + 1))));
                        }

                        break;
                    case ShaderUtil.ShaderPropertyType.Int:
                        break;
                }

            }
        }
    }
    private int[] PrintShaderProperties(Shader shader, Shader targetshader, int[] chooses)
    {
        GUIStyle redStyle = new GUIStyle(EditorStyles.label);
        redStyle.normal.textColor = Color.red;
        redStyle.hover.textColor = Color.red;

        int propertycount = ShaderUtil.GetPropertyCount(shader);
        int targetpropertycount = ShaderUtil.GetPropertyCount(targetshader);

        List<string> options = new List<string>();

        options.Add(new string("未选择"));
        for (int i = 0; i < targetpropertycount; i++)
        {
            options.Add(ShaderUtil.GetPropertyName(targetshader, i) + "  [类型" + ShaderUtil.GetPropertyType(targetshader, i) + "]");
        }

        string[] optionsstrs = options.ToArray();


        for (int i = 0; i < propertycount; i++)
        {
            string propertyname = ShaderUtil.GetPropertyName(shader, i);

            if (!IsPropertyHidden(shader, propertyname))
            {

                if (shaderPropertyIsUsed[shader][i])
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("(含值)" + propertyname + "  [类型" + ShaderUtil.GetPropertyType(shader, i) + "]", redStyle);

                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("           " + propertyname + "  [类型" + ShaderUtil.GetPropertyType(shader, i) + "]");
                }
                chooses[i] = EditorGUILayout.Popup(chooses[i], optionsstrs, GUILayout.Width(300));
                GUILayout.EndHorizontal();

            }


        }
        return chooses;
    }
    private bool IsPropertyHidden(Shader shader, string propertyname)
    {
        int propertyindex = shader.FindPropertyIndex(propertyname);
        return ShaderUtil.IsShaderPropertyHidden(shader, propertyindex);
    }
    private void Next()
    {
        Clear(false);
        shaders = new List<Shader>();
        targetshaders = new List<Shader>();

        foreach (Material material in materials)
        {
            Shader shader = material.shader;
            if (!shaders.Contains(shader))
            {
                shaders.Add(shader);
                targetshaders.Add(null);
                shaderPropertyIsUsed.Add(shader, new List<bool>());
            }
            int propertycount = ShaderUtil.GetPropertyCount(shader);

            for (int i = 0; i < propertycount; i++)
            {
                if (shaderPropertyIsUsed[shader].Count <= i)
                {
                    shaderPropertyIsUsed[shader].Add(false);
                }
                if (PropertyIsUsed(material, i))
                {
                    shaderPropertyIsUsed[shader][i] = true;
                }
            }
        }
    }
    private bool PropertyIsUsed(Material material, int propertyIndex)
    {
        Shader shader = material.shader;
        Material nullMaterial = new Material(shader);
        string propertyname = ShaderUtil.GetPropertyName(shader, propertyIndex);

        ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(shader, propertyIndex);
        switch (propertyType)
        {
            case ShaderUtil.ShaderPropertyType.Color:
                if (material.GetColor(propertyname) != nullMaterial.GetColor(propertyname))
                {
                    return true;
                }
                break;
            case ShaderUtil.ShaderPropertyType.Float:
                if (material.GetFloat(propertyname) != nullMaterial.GetFloat(propertyname))
                {
                    return true;
                }
                break;
            case ShaderUtil.ShaderPropertyType.Vector:
                if (material.GetVector(propertyname) != nullMaterial.GetVector(propertyname))
                {
                    return true;
                }
                break;
            case ShaderUtil.ShaderPropertyType.Int:
                if (material.GetInt(propertyname) != nullMaterial.GetInt(propertyname))
                {
                    return true;
                }
                break;
            case ShaderUtil.ShaderPropertyType.Range:
                if (material.GetFloat(propertyname) != nullMaterial.GetFloat(propertyname))
                {
                    return true;
                }
                break;
            case ShaderUtil.ShaderPropertyType.TexEnv:
                if (material.GetTexture(propertyname) != nullMaterial.GetTexture(propertyname))
                {
                    return true;
                }
                break;
        }
        return false;
    }
    private void Refresh()
    {
        if (materials == null)
        {
            materials = new List<Material>();
        }
        foreach (UnityEngine.Object obj in Selection.objects)
        {
            if (obj is Material && !materials.Contains((Material)obj))
            {
                materials.Add((Material)obj);
            }
        }
    }

}