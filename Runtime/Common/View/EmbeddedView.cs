using UnityEngine;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public class EmbeddedView : BaseView
    {
        [SerializeField, Tooltip("ContainerName will be used to query proper container in parent's RootVisualElement")]
        private string containerName;

        [SerializeField] private BaseView parent;

        public const string EmbeddedRootUssClassName = "mvvmtk-root-view-embedded";

        protected override VisualElement Instantiate()
        {
            var root = Asset.Instantiate();
            root.AddToClassList(EmbeddedRootUssClassName);
            root.pickingMode = PickingMode.Ignore;
            root.style.flexGrow = 1f;
            return root;
        }

        public override VisualElement ResolveParent()
        {
            var container = parent.RootVisualElement.Q(containerName);
            if (container is null)
            {
                throw new(
                    $"No VisualElement with name: {containerName} was found in hierarchy instantiated from {parent.name}.");
            }

            return container;
        }
    }
}