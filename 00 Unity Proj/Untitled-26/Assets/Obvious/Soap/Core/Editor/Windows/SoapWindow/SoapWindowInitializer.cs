using UnityEditor;

namespace Obvious.Soap.Editor
{
    [InitializeOnLoad]
    public class SoapWindowInitializer
    {
        static SoapWindowInitializer()
        {
            var hasShownWindow = SoapEditorUtils.HasShownWindow;
            if (hasShownWindow)
                return;
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        private static void OnEditorApplicationUpdate()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;
            SoapWindow.Open();
            SoapEditorUtils.HasShownWindow = true;
        }
    }
}