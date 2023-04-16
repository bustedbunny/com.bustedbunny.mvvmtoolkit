using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public class EmbeddedView : BaseView
    {
        [FormerlySerializedAs("containerName")]
        [SerializeField, Tooltip("ContainerName will be used to query proper container in parent's RootVisualElement")]
        private string _containerName;

        [FormerlySerializedAs("parent")] [SerializeField]
        private BaseView _parent;

        public const string EmbeddedRootUssClassName = "mvvmtk-root-view-embedded";

        protected override VisualElement Instantiate()
        {
            var root = InstantiateAsset();
            root.AddToClassList(EmbeddedRootUssClassName);
            return root;
        }

        public override VisualElement ResolveParent()
        {
            var container = _parent.RootVisualElement.Q(_containerName);
            if (container is null)
            {
                throw new(
                    $"No VisualElement with name: {_containerName} was found in hierarchy instantiated from {_parent.name}.");
            }

            return container;
        }
    }
}