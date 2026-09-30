#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace MyBox.Internal
{
    public static class TemplatesCreator
    {
        private const string ComponentTemplate =
            "using UnityEngine;\n" +
            "\n" +
            "public class ##ClassName## : MonoBehaviour\n" +
            "{\n" +
            "}\n";

        //[MenuItem("Assets/Create/Mono Component", false, -99)]
        public static void CreateComponentMenuItem()
        {
            CreateFromTemplate(ComponentTemplate, "NewComponent.cs");
        }

        private static void CreateFromTemplate(string template, string defaultName)
        {
            var processor = ScriptableObject.CreateInstance<TemplateProcessor>();
            processor.Template = template;
            var icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

#if UNITY_6000_3_OR_NEWER
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(default(EntityId), processor, defaultName, icon, "");
#else
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, processor, defaultName, icon, "");
#endif
        }

#if UNITY_6000_3_OR_NEWER
        private class TemplateProcessor : AssetCreationEndAction
        {
            public string Template;

            public override void Action(EntityId entityId, string pathName, string resourceFile)
            {
                Write(Template, pathName);
            }
        }
#else
		private class TemplateProcessor : EndNameEditAction
		{
			public string Template;

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				Write(Template, pathName);
			}
		}
#endif

        private static void Write(string template, string pathName)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            File.WriteAllText(Path.GetFullPath(pathName), template.Replace("##ClassName##", className));
            AssetDatabase.ImportAsset(pathName);
            ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath<Object>(pathName));
        }
    }
}
#endif