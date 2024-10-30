﻿using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumFormsSample.CustomRuntimes;
using GumRuntime;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.Forms;
using MonoGameGum.Forms.Controls;
using MonoGameGum.Forms.DefaultFromFileVisuals;
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
    ToastRuntime _popup;

    public void Initialize(ref GraphicalUiElement root)
    {

        var gumProject = GumProjectSave.Load("FormsGumProject/GumProject.gumx");
        ObjectFinder.Self.GumProjectSave = gumProject;
        gumProject.Initialize();
        FormsUtilities.RegisterFromFileFormRuntimeDefaults();

        FileManager.RelativeDirectory = "Content/FormsGumProject/";

        ElementSaveExtensions.RegisterGueInstantiationType(
            "Controls/Toast",
            typeof(ToastRuntime)
        );

        // This assumes that your project has at least 1 screen

        _root = gumProject.Screens.Find(item => item.Name == "DemoScreenGum").ToGraphicalUiElement(
            SystemManagers.Default, addToManagers: true);
        root = _root;

        PopulateListBox();

        PopulateComboBox();

        InitializeRadioButtons();

        var interactable = (InteractiveGue)_root.GetGraphicalUiElementByName("PopupContainer");
        interactable.RollOff += RemovePopup;
        interactable.RollOn += AddPopup;
    }

    private void AddPopup(object sender, EventArgs e)
    {
        _popup = new ToastRuntime(true, false);
        var state = Mouse.GetState();
        _popup.X = state.X + 16;
        _popup.Y = state.Y + 16;

        FrameworkElement.ModalRoot.Children.Add( _popup );
    }

    private void RemovePopup(object sender, EventArgs e)
    {
        _popup.RemoveFromManagers();
        _popup.Parent = null;
        _popup = null;
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
