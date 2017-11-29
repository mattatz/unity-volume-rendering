using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace VolumeRendering
{

    public class VolumeAssetBuilder : EditorWindow {

        [MenuItem("Window/VolumeAssetBuilder")]
        static void Init()
        {
            var window = EditorWindow.GetWindow(typeof(VolumeAssetBuilder));
            window.Show();
        }

        string inputPath, outputPath;
        int width = 256, height = 256, depth = 256;
        Object asset;

        void OnEnable()
        {
            inputPath = "Assets/MRI.256x256x256.raw";
            outputPath = "Assets/MRI.asset";
        }

        void OnGUI()
        {
            const float headerSize = 120f;
            
            using(new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Input pvm raw file path", GUILayout.Width(headerSize));
                asset = EditorGUILayout.ObjectField(asset, typeof(Object), true);
                inputPath = AssetDatabase.GetAssetPath(asset);
            }

            using(new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Width", GUILayout.Width(headerSize));
                width = EditorGUILayout.IntField(width);
            }

            using(new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Height", GUILayout.Width(headerSize));
                height = EditorGUILayout.IntField(height);
            }

            using(new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Depth", GUILayout.Width(headerSize));
                depth = EditorGUILayout.IntField(depth);
            }

            using(new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Output path", GUILayout.Width(headerSize));
                outputPath = EditorGUILayout.TextField(outputPath);
            }

            if(GUILayout.Button("Build"))
            {
                Build(inputPath, outputPath, width, height, depth);
            }
        }

        void Build(
            string inputPath, 
            string outputPath, 
            int width,
            int height,
            int depth 
        )
        {
            if (!File.Exists(inputPath))
            {
                Debug.LogWarning(inputPath + " is not exist.");
                return;
            }

            var volume = Build(inputPath, width, height, depth);
            AssetDatabase.CreateAsset(volume, outputPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static Texture3D Build(string path, int width, int height, int depth)
        {
            var max = width * height * depth;
            var tex  = new Texture3D(width, height, depth, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.anisoLevel = 0;

            using(var stream = new FileStream(path, FileMode.Open))
            {
                var len = stream.Length;
                if(len != max)
                {
                    Debug.LogWarning(path + " doesn't have required resolution");
                }

                int i = 0;
                Color[] colors = new Color[max];
                float inv = 1f / 255.0f;
                for(i = 0; i < stream.Length; i++)
                {
                    if(i == max)
                    {
                        break;
                    }
                    int v = stream.ReadByte();
                    float f = v * inv;
                    colors[i] = new Color(f, f, f, f);
                }
                tex.SetPixels(colors);
                tex.Apply();
            }
            return tex;
        }

    }

}


