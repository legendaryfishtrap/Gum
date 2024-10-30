using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumFormsSample.CustomRuntimes;
using GumRuntime;
using Microsoft.Xna.Framework;
using MonoGameGum.Forms;
using MonoGameGum.Forms.Controls;
using MonoGameGum.Forms.DefaultFromFileVisuals;
using MonoGameGum.Forms.DefaultVisuals;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsUtilities;

namespace GumFormsSample.Screens;

internal class FromFileDemoScreen
{
    GraphicalUiElement _root;
    public void Initialize(ref GraphicalUiElement root)
    {

        var gumProject = GumProjectSave.Load("FormsGumProject/GumProject.gumx");
        ObjectFinder.Self.GumProjectSave = gumProject;
        gumProject.Initialize();
        FormsUtilities.RegisterFromFileFormRuntimeDefaults();

        FileManager.RelativeDirectory = "Content/FormsGumProject/";

        // This assumes that your project has at least 1 screen
        ElementSaveExtensions.RegisterGueInstantiationType(
            "Controls/ScrollViewer",
            typeof(CustomScrollViewer)
        );

        _root = gumProject.Screens.Find(item => item.Name == "DemoScreenGum").ToGraphicalUiElement(
            SystemManagers.Default, addToManagers: true);
        root = _root;

        PopulateListBox();

        PopulateComboBox();

        InitializeRadioButtons();

        PopulateScrollViewer();
    }

    private void PopulateScrollViewer()
    {
        var scrollViewer = (CustomScrollViewer)_root.GetGraphicalUiElementByName("ScrollViewerInstance");
        var scrollViewerForms = scrollViewer.FormsControlAsObject as ScrollViewer;

        scrollViewerForms.InnerPanel.Children.Clear();
        scrollViewerForms.InnerPanel.ChildrenLayout = ChildrenLayout.LeftToRightStack;

        var random = new System.Random();
        for (int i = 0; i < 30; i++)
        {
            var innerRectangle = new ColoredRectangleRuntime();
            innerRectangle.X = 0;
            innerRectangle.Y = 0;
            innerRectangle.Width = 50;
            innerRectangle.Height = 50;
            innerRectangle.Color = new Color(random.Next(255), random.Next(255), random.Next(255));

            scrollViewerForms.InnerPanel.Children.Add(innerRectangle);
        }
    }

    private void PopulateComboBox()
    {
        var comboBox = (InteractiveGue)_root.GetGraphicalUiElementByName("ComboBoxInstance");
        var comboBoxForms = comboBox.FormsControlAsObject as ComboBox;

        comboBoxForms.Items.Add("Easy");
        comboBoxForms.Items.Add("Medium");
        comboBoxForms.Items.Add("Hard");
        comboBoxForms.Items.Add("Impossible");
    }

    private void InitializeRadioButtons()
    {
        var radioButton = (InteractiveGue)_root.GetGraphicalUiElementByName("RadioButtonInstance");
        var radioButtonForms = radioButton.FormsControlAsObject as RadioButton;
        radioButtonForms.IsChecked = true;
    }

    private void PopulateListBox()
    {
        var listBoxVisual = (InteractiveGue)_root.GetGraphicalUiElementByName("ResolutionBox");
        var listBox = listBoxVisual.FormsControlAsObject as ListBox;

        listBox.Items.Add("400x300");
        listBox.Items.Add("600x800");
        listBox.Items.Add("1024x768");
        listBox.Items.Add("1280x720");
        listBox.Items.Add("1920x1080");
        listBox.Items.Add("2560x1440");
        listBox.Items.Add("3840x2160");
        listBox.Items.Add("7680x4320");
    }


}
