#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;
using System;
using System.Reflection;

/// <summary>
/// Generates icon images for items.
/// </summary>
public class ItemIconGenerator : EditorWindow
{
    private const int Resolution = 500;

    private Camera _cam;
    private string _filename;

    [MenuItem("Debug Tools/Item Icon Generator")]
    private static void Init()
    {
        ItemIconGenerator window = GetWindow<ItemIconGenerator>("Icon Generator");
        window.minSize = window.maxSize = new Vector2(520, 650);
        window.Show();
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        UpdateCamera();

        Rect rect = EditorGUILayout.GetControlRect(false, Resolution);
        EditorGUI.DrawTextureTransparent(rect, _cam.targetTexture, ScaleMode.ScaleToFit);

        // Hack to get unity's project window path
        // From https://discussions.unity.com/t/how-to-get-path-from-the-current-opened-folder-in-the-project-window-in-unity-editor/226209/2
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string pathToCurrentFolder = obj.ToString();

        _filename = EditorGUILayout.TextField("Filename", _filename);

        if (string.IsNullOrWhiteSpace(_filename))
        {
            EditorGUILayout.HelpBox("Enter filename to continue", MessageType.Info);
            return;
        }

        if (!GUILayout.Button($"Save: {pathToCurrentFolder}/{_filename}.png"))
            return;

        RenderTexture prevRt = RenderTexture.active;
        RenderTexture.active = _cam.targetTexture;

        Texture2D captured = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false);
        Rect rectReadPicture = new Rect(0, 0, Resolution, Resolution);
        captured.ReadPixels(rectReadPicture, 0, 0);

        Texture2D savedAsset = AttachmentIconUtils.SaveIcon(captured, pathToCurrentFolder, _filename);

        EditorGUIUtility.PingObject(savedAsset);
        RenderTexture.active = prevRt;
    }

    private void UpdateCamera()
    {
        if (_cam == null)
        {
            FindOrCreateCam();
            EditorGUIUtility.PingObject(_cam.gameObject);
        }

        // Synchronize the custom camera with the scene view camera
        if (SceneView.lastActiveSceneView != null)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            _cam.transform.position = sceneCamera.transform.position;
            _cam.transform.rotation = sceneCamera.transform.rotation;
        }

        _cam.Render();
    }


    private void FindOrCreateCam()
    {
        const string camName = "Icon Render Camera";

        GameObject prevCam = GameObject.Find(camName);

        if (prevCam != null && prevCam.TryGetComponent(out _cam))
            return;

        GameObject camGo = new GameObject(camName);

        _cam = camGo.AddComponent<Camera>();
        _cam.nearClipPlane = 0.01f;
        _cam.farClipPlane = 100;
        _cam.targetTexture = new RenderTexture(Resolution, Resolution, 0);

        HDAdditionalCameraData camData = _cam.gameObject.AddComponent<HDAdditionalCameraData>();
        camData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
        camData.backgroundColorHDR = new Color(1f, 1f, 1f, 0f);
        camData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;

        EditorUtility.SetDirty(_cam);
    }
}

#endif
