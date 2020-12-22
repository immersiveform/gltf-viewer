using System.Threading.Tasks;
using UnityEditor;
#if ENABLE_WINMD_SUPPORT
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
#endif

namespace GltfViewer.Scripts
{
    public static class OpenFileHelper 
    {
        internal static async Task<string> SelectGLTFFileAsync()
        {
            TaskCompletionSource<string> pickCompleted = new TaskCompletionSource<string>();
            string filePath = string.Empty;
        
#if ENABLE_WINMD_SUPPORT
            UnityEngine.WSA.Application.InvokeOnUIThread(
                async () =>
                {
                    FileOpenPicker picker = new FileOpenPicker();
                    picker.SuggestedStartLocation = PickerLocationId.Objects3D;
                    picker.FileTypeFilter.Add(".glb");
                    picker.FileTypeFilter.Add(".gltf");
                    //picker.FileTypeFilter.Add("*");
                    picker.ViewMode = PickerViewMode.Thumbnail; //PickerViewMode.List;
                    picker.CommitButtonText = "Select file";

                    var file = await picker.PickSingleFileAsync();
                    if (file != null)
                    {
                        filePath = file.Path;
                    }
                    pickCompleted.SetResult(filePath);
                    await pickCompleted.Task;
                },
                true
            );
#else
            filePath = EditorUtility.OpenFilePanelWithFilters(
                title: "Load file",
                directory: string.Empty,
                filters: new[] {"Gltf files", "glb,gltf"});

            pickCompleted.SetResult(result: filePath);
            await pickCompleted.Task;    
#endif
            return pickCompleted.Task.Result;
        }
    }
}
