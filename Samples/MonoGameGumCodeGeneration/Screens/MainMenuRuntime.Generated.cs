//Code for MainMenu
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using MonoGameGum.GueDeriving;
using MonoGameGumCodeGeneration.Components;
using System.Linq;
namespace MonoGameGumCodeGeneration.Screens
{
    public partial class MainMenuRuntime
    {
        public PopupRuntime PopupInstance { get; protected set; }
        public ComponentWithStatesRuntime ComponentWithStatesInstance { get; protected set; }
        public TextRuntime TextWithLotsOfPropertiesSet { get; protected set; }

        public MainMenuRuntime()
        {


        }
        public override void AfterFullCreation()
        {
            PopupInstance = this.GetGraphicalUiElementByName("PopupInstance") as PopupRuntime;
            ComponentWithStatesInstance = this.GetGraphicalUiElementByName("ComponentWithStatesInstance") as ComponentWithStatesRuntime;
            TextWithLotsOfPropertiesSet = this.GetGraphicalUiElementByName("TextWithLotsOfPropertiesSet") as TextRuntime;

            CustomInitialize();
        }
        //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
        partial void CustomInitialize();
    }
}
