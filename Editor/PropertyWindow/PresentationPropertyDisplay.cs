using System.Collections.Generic;
using MVPToolkit.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVPToolkit.Editor.PropertyWindow
{
    public class PresentationPropertyDisplay : EditorWindow
    {
        private const string BrowserTitle = "Presentation Property Browser";

        [MenuItem("Tools/" + BrowserTitle)]
        public static void OpenWindow()
        {
            var window = GetWindow<PresentationPropertyDisplay>();
            window.titleContent = new GUIContent(BrowserTitle);
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(
                new Label("Types inherited from SystemBase are still valid, but hidden from this table"));
            CreateTreeView();
        }

        private void CreateTreeView()
        {
            var types = ReflectionUtility.GetPresentations();

            var source = new List<TreeViewItemData<string>>();

            var ind = 0;

            foreach (var type in types)
            {
                var validProperties = PropertyAnalyzer.GetValidProperties(type);

                var propertiesData = new List<TreeViewItemData<string>>();
                foreach (var validProperty in validProperties)
                {
                    propertiesData.Add(new TreeViewItemData<string>(ind++, validProperty.Name));
                }

                source.Add(new TreeViewItemData<string>(ind++, type.Name, propertiesData));
            }

            var treeView = new TreeView
            {
                makeItem = () => new Label(),
            };
            treeView.bindItem = (element, i) => { ((Label)element).text = treeView.GetItemDataForIndex<string>(i); };
            treeView.SetRootItems(source);

            rootVisualElement.Add(treeView);
        }
    }
}