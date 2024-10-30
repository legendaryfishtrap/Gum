using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary;
using RenderingLibrary.Graphics;

namespace GumFormsSample.CustomRuntimes;

internal class ToastRuntime : GraphicalUiElement {
    public ToastRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true) : base(new InvisibleRenderable()) {
        if (fullInstantiation)
        {
            var element = ObjectFinder.Self.GetComponent("Controls/Toast");
            element.SetGraphicalUiElement(this, SystemManagers.Default);
        }
    }
}



