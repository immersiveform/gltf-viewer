using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GltfViewer.Scripts
{
    public class AppManager : MonoBehaviour
    {
        private bool allowManipulation = true;
        
#pragma warning disable 0649
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private GameObject modelSpawner;
        [SerializeField] private List<ModelController> models = new List<ModelController>();
#pragma warning restore 0649

        private void Awake()
        {
            Debug.Assert(modelPrefab);
            Debug.Assert(modelSpawner);
        }

        public async void LoadGltfFile()
        {
            string filePath = await OpenFileHelper.SelectGLTFFileAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                ModelController model = Instantiate(modelPrefab, transform, false).GetComponent<ModelController>();
                model.SetTransform(modelSpawner.transform);
                model.SetPath(filePath);
                models.Add(model);
            }
        }

        public void ClearModels()
        {
            if (models.Count > 0)
            {
                foreach (var modelController in models)
                {
                    Destroy(modelController.gameObject);
                }
                
                models.Clear();
            }
            
            //unload created materials
            Resources.UnloadUnusedAssets();
        }
        
        public void AdjustModels()
        {
            allowManipulation = !allowManipulation;
            
            if (models.Count > 0)
            {
                foreach (var modelController in models)
                {
                    modelController.AdjustModel(allowManipulation);
                }
            }
        }
    }
}