using System;
using System.IO;
using GLTFast;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Rendering;
using UnityEngine;

namespace GltfViewer.Scripts
{
    public class ModelController : MonoBehaviour
    {
        private GltfAsset gltfAsset;
        
#pragma warning disable 0649
        [SerializeField] private Material mrtkStandardMaterial;
#pragma warning restore 0649

        private void Awake()
        {
            Debug.Assert(mrtkStandardMaterial);
            gltfAsset = gameObject.GetComponent<GltfAsset>();
            gltfAsset.onLoadComplete += OnLoadComplete;
        }

        private void OnLoadComplete(GltfAssetBase gltfAsset, bool success)
        {
            if (success)
            {
                Renderer[] renderers = gltfAsset.gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    ReplaceMaterialToMrtk(renderer);
                }
                Debug.Log("Gltf model was loaded");
            }
        }

        public void SetPath(string filePath)
        {
            gltfAsset.url = filePath;
            RenameGameObjectToFileName(filePath);
        }

        public void SetTransform(Transform newTransform)
        {
            transform.position = newTransform.position;
            transform.rotation = newTransform.rotation;
        }

        private void ReplaceMaterialToMrtk(Renderer renderer)
        {
            //add MaterialInstance to each renderer that will automatically destroy material instances
            //when their host objects are destroyed.
            Material[] materials = renderer.gameObject.EnsureComponent<MaterialInstance>().Materials;
            for (int i = 0; i < materials.Length; i++)
            {
                //it is very important to check Albedo Assigned at Runtime property in MrtkStandard material
                //or textures won't be visible at runtime 
                Texture orgTexture = materials[i].mainTexture;
                Color orgColor = materials[i].color;
                 
                materials[i] = new Material(mrtkStandardMaterial)
                {
                    mainTexture = orgTexture, 
                    color = orgColor
                };
            }
            renderer.materials = materials; //change back the materials
        }

        private void RenameGameObjectToFileName(string fullPath)
        {
            Uri uri = new Uri(fullPath);
            string fileName = Path.GetFileName(uri.LocalPath);
            if (!string.IsNullOrEmpty(fileName))
            {
                gameObject.name = fileName;
            }
        }
    }
}
