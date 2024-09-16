﻿using System;
using System.Collections.Generic;
using RenderingLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using RenderingLibrary.Math.Geometry;
using Microsoft.Xna.Framework;
using RenderingLibrary.Content;
using ToolsUtilities;
using System.Text;
using System.Net;
using System.Xml.Linq;

#if WEB
using nkast.Wasm.XHR;
using static System.Runtime.InteropServices.JavaScript.JSType;
#endif



#if USE_GUMCOMMON
using MonoGameGum.GueDeriving;
using Gum.Managers;
using GumRuntime;

using MonoGameGum.Renderables;
using Gum.Wireframe;
#endif

namespace RenderingLibrary
{
    public partial class SystemManagers : ISystemManagers
    {
        #region Fields

        int mPrimaryThreadId;

        #endregion

        #region Properties

        public static SystemManagers Default
        {
            get;
            set;
        }

        public Renderer Renderer
        {
            get;
            private set;
        }

        IRenderer ISystemManagers.Renderer => Renderer;

        public SpriteManager SpriteManager
        {
            get;
            private set;
        }

        public ShapeManager ShapeManager
        {
            get;
            private set;
        }

        public TextManager TextManager
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsCurrentThreadPrimary
        {
            get
            {
#if WINDOWS_8 || UWP
                int threadId = Environment.CurrentManagedThreadId;
#else
                int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
                return threadId == mPrimaryThreadId;
            }
        }

        /// <summary>
        /// The font scale value. This can be used to scale all fonts globally, 
        /// generally in response to a font scaling value like the Android font scale setting.
        /// </summary>
        public static float GlobalFontScale { get; set; } = 1.0f;
        public bool EnableTouchEvents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static Dictionary<string, byte[]> StreamByteDictionary { get; private set; } = new Dictionary<string, byte[]>();
        #endregion

        /// <summary>
        /// Performs every-frame activity for all contained systems in the SystemManager.
        /// </summary>
        /// <param name="currentTime">The amount of time that has passed since the game started.</param>
        /// <exception cref="InvalidOperationException">Exception thrown if the SystemManagers hasn't yet been initialized.</exception>
        public void Activity(double currentTime)
        {
#if DEBUG
            if(SpriteManager == null)
            {
                throw new InvalidOperationException("The SpriteManager is null - did you remember to initialize the SystemManagers?");
            }
#endif

            SpriteManager.Activity(currentTime);
        }

        public void Draw()
        {
            Renderer.Draw(this);
        }

        public void Draw(Layer layer)
        {
            Renderer.Draw(this, layer);
        }

        public void Draw(List<Layer> layers)
        {
            Renderer.Draw(this, layers);
        }

        
        public void Initialize(GraphicsDevice graphicsDevice, bool fullInstantiation = false)
        {
#if NET6_0_OR_GREATER
            var usesTitleContainer = System.OperatingSystem.IsAndroid() || 
                System.OperatingSystem.IsIOS() ||
                System.OperatingSystem.IsBrowser();

            if(usesTitleContainer)
            {
                FileManager.CustomGetStreamFromFile = (fileName) =>
                {
                    if(FileManager.IsRelative(fileName) == false)
                    {
                        fileName = FileManager.MakeRelative(fileName, FileManager.ExeLocation, preserveCase:true);
                    }


#if WEB
                    if(fileName.StartsWith("./"))
                    {
                       fileName = fileName.Substring(2);
                    }
#endif

                    if(StreamByteDictionary.ContainsKey(fileName))
                    {
                        var bytes = StreamByteDictionary[fileName];
                        return new MemoryStream(bytes);
                    }

#if WEB



                    XMLHttpRequest request = new XMLHttpRequest();

                    var suffix =
                        "?token=" + DateTime.Now.Ticks;

                    request.Open("GET", fileName + suffix, false);
                    request.OverrideMimeType("text/plain; charset=x-user-defined");
                    request.Send();

                    if (request.Status == 200)
                    {
                        string responseText = request.ResponseText;

                        var bytes =
                            System.Text.Encoding.UTF8.GetBytes(responseText);
                        return new MemoryStream(bytes);

                        //byte[] buffer = new byte[responseText.Length];
                        //for (int i = 0; i < responseText.Length; i++)
                        //    buffer[i] = (byte)(responseText[i] & 0xff);

                        //Stream ms = new MemoryStream(buffer);

                        //return ms;
                    }
                    else
                    {
                        throw new IOException("HTTP request failed. Status:" + request.Status);
                    }


#else
                    var stream = TitleContainer.OpenStream(fileName);
                    return stream;
#endif
                };
            }

#endif

#if WINDOWS_8 || UWP
            mPrimaryThreadId = Environment.CurrentManagedThreadId;
#else
                    mPrimaryThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif

            Renderer = new Renderer();
            Renderer.Initialize(graphicsDevice, this);

            SpriteManager = new SpriteManager();

            ShapeManager = new ShapeManager();

            TextManager = new TextManager();


            SpriteManager.Managers = this;
            ShapeManager.Managers = this;
            TextManager.Managers = this;

            if(fullInstantiation)
            {
#if USE_GUMCOMMON
                LoaderManager.Self.ContentLoader = new ContentLoader();

                var assembly = typeof(SystemManagers).Assembly;
#if KNI
                var bitmapPattern = ToolsUtilities.FileManager.GetStringFromEmbeddedResource(assembly, "KniGum.Font18Arial.fnt");
                using var stream = ToolsUtilities.FileManager.GetStreamFromEmbeddedResource(assembly, "KniGum.Font18Arial_0.png");
#else
                var bitmapPattern = ToolsUtilities.FileManager.GetStringFromEmbeddedResource(assembly, "MonoGameGum.Content.Font18Arial.fnt");
                using var stream = ToolsUtilities.FileManager.GetStreamFromEmbeddedResource(assembly, "MonoGameGum.Content.Font18Arial_0.png");
#endif
                var defaultFontTexture = Texture2D.FromStream(graphicsDevice, stream);
                Text.DefaultBitmapFont = new BitmapFont(defaultFontTexture, bitmapPattern);

                GraphicalUiElement.CanvasWidth = graphicsDevice.Viewport.Width;
                GraphicalUiElement.CanvasHeight = graphicsDevice.Viewport.Height;
                GraphicalUiElement.SetPropertyOnRenderable = CustomSetPropertyOnRenderable.SetPropertyOnRenderable;
                GraphicalUiElement.UpdateFontFromProperties = CustomSetPropertyOnRenderable.UpdateToFontValues;
                GraphicalUiElement.ThrowExceptionsForMissingFiles = CustomSetPropertyOnRenderable.ThrowExceptionsForMissingFiles;

                GraphicalUiElement.AddRenderableToManagers = CustomSetPropertyOnRenderable.AddRenderableToManagers;
                GraphicalUiElement.RemoveRenderableFromManagers = CustomSetPropertyOnRenderable.RemoveRenderableFromManagers;
                Renderer.ApplyCameraZoomOnWorldTranslation = true;

                Renderer.Camera.CameraCenterOnScreen = CameraCenterOnScreen.TopLeft;

                ElementSaveExtensions.CustomCreateGraphicalComponentFunc = RenderableCreator.HandleCreateGraphicalComponent;

                StandardElementsManager.Self.Initialize();

                Text.RenderBoundaryDefault = false;

                ToolsUtilities.FileManager.RelativeDirectory += "Content/";

                RegisterComponentRuntimeInstantiations();

                GraphicalUiElement.MissingFileBehavior = MissingFileBehavior.ThrowException;

#endif
            }
        }

#if USE_GUMCOMMON

        private void RegisterComponentRuntimeInstantiations()
        {
            ElementSaveExtensions.RegisterGueInstantiation(
                "ColoredRectangle",
                () => new ColoredRectangleRuntime());

            ElementSaveExtensions.RegisterGueInstantiation(
                "Container",
                () => new ContainerRuntime());

            ElementSaveExtensions.RegisterGueInstantiation(
                "NineSlice",
                () => new NineSliceRuntime());

            ElementSaveExtensions.RegisterGueInstantiation(
                "Polygon",
                () => new PolygonRuntime());

            ElementSaveExtensions.RegisterGueInstantiation(
                "Rectangle",
                () => new RectangleRuntime());

            ElementSaveExtensions.RegisterGueInstantiation(
                "Sprite",
                () => new SpriteRuntime());

            ElementSaveExtensions.RegisterGueInstantiation(
                "Text",
                () => new TextRuntime());
        }
#endif

        public override string ToString()
        {
            return Name;
        }

        public void InvalidateSurface()
        {
            throw new NotImplementedException();
        }
    }
}
