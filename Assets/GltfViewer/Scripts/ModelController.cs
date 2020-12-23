using System;
using System.IO;
using GLTFast;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Rendering;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine;
using UnityEngine.Events;

namespace GltfViewer.Scripts
{
    [RequireComponent(typeof(GltfAsset))]
    [RequireComponent(typeof(ObjectManipulator))]
    [RequireComponent(typeof(BoundsControl))]
    public class ModelController : MonoBehaviour
    {
        private GltfAsset gltfAsset;
        private bool isLoaded;
        private ObjectManipulator objectManipulator;
        private BoundsControl boundsControl;
        
#pragma warning disable 0649
        [SerializeField] private Material mrtkStandardMaterial;
#pragma warning restore 0649

        public UnityEvent OnModelLoaded;

        private void Awake()
        {
            Debug.Assert(mrtkStandardMaterial);

            objectManipulator = gameObject.GetComponent<ObjectManipulator>();
            boundsControl = gameObject.GetComponent<BoundsControl>();
            
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
                isLoaded = true;
                OnModelLoaded?.Invoke();
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

        public void AdjustModel(bool enableManipulation)
        {
            if (isLoaded)
            {
                objectManipulator.enabled = enableManipulation;
                boundsControl.enabled = enableManipulation;
            }
        }

        private void ReplaceMaterialToMrtk(Renderer renderer)
        {
            Material pbrMaterial = renderer.material;
            
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            if (pbrMaterial.mainTexture != null)
            {
                propertyBlock.SetTexture("_MainTex", pbrMaterial.mainTexture);
            }
            propertyBlock.SetColor("_Color", pbrMaterial.color);
            renderer.material = mrtkStandardMaterial;
            renderer.SetPropertyBlock(propertyBlock);
            Destroy(pbrMaterial);
            //renderer.materials = newMaterials; //this is needed to overwrite old materials


            // Material[] materialInstances = renderer.gameObject.EnsureComponent<MaterialInstance>().Materials;
            // for (int i = 0; i < pbrMaterials.Length; i++)
            // {
            //     materialInstances[i].mainTexture = pbrMaterials[i].mainTexture;
            //     materialInstances[i].color = pbrMaterials[i].color;
            // }
            // renderer.materials = materialInstances;

            //add MaterialInstance to each renderer that will automatically destroy material instances
            //when their host objects are destroyed.
            // Material[] materials = renderer.gameObject.EnsureComponent<MaterialInstance>().Materials;
            // for (int i = 0; i < materials.Length; i++)
            // {
            //     //it is very important to check Albedo Assigned at Runtime property in MrtkStandard material
            //     //or textures won't be visible at runtime 
            //     Texture orgTexture = materials[i].mainTexture;
            //     Color orgColor = materials[i].color;
            //      
            //     materials[i] = new Material(mrtkStandardMaterial)
            //     {
            //         mainTexture = orgTexture, 
            //         color = orgColor
            //     };
            // }
            // renderer.materials = materials; //change back the materials
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
