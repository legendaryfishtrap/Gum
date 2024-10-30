using Gum.Wireframe;
using MonoGameGum.Forms.Controls;

namespace GumFormsSample.CustomRuntimes;

public class CustomScrollViewer : InteractiveGue
{
    public CustomScrollViewer(bool fullInstantiation = true, bool tryCreateFormsObject = true) : base() { }

    public ScrollViewer FormsControl => FormsControlAsObject as ScrollViewer;

    public override void AfterFullCreation()
    {
        base.AfterFullCreation();

        if (FormsControlAsObject == null)
        {
            FormsControlAsObject = new ScrollViewer(this);
        }
    }
}



