using System;
using UnityEngine;
using ColossalFramework.HTTP;
using ColossalFramework.Packaging;
using System.Reflection;
using ColossalFramework.UI;
using System.Collections.Generic;
using System.IO;
using ColossalFramework.IO;

namespace ChangeLoadingImage
{
    public class Redirector
    {
        #region variables
        RedirectCallsState rcsMySetImage;
        RedirectCallsState rcsMySetText;
        ImageListEntry entry;

        public static bool isSupported {
            get {
                return versionIsSupported ();
            }
        }

        static LoadingAnimation loadingAnimation {
            get {
                return LoadingManager.instance.LoadingAnimationComponent;
            }
        }
        #endregion

        public Redirector ()
        {
        }

        public void Initialize ()
        {
            if(!isSupported)
                return;

            redirectSetImage ();
            redirectSetText ();
        }

        private void Deinitialize ()
        {
            revertSetImage ();
        }
        
        #region redirection
        private void redirectSetImage ()
        {
            rcsMySetImage = RedirectionHelper.RedirectCalls (
                typeof(LoadingAnimation).GetMethod ("SetImage", BindingFlags.Public | BindingFlags.Instance), 
                typeof(Redirector).GetMethod ("mySetImage", BindingFlags.Public | BindingFlags.Instance));
        }

        private void revertSetImage ()
        {
            RedirectionHelper.RevertRedirect (
                typeof(LoadingAnimation).GetMethod ("SetImage", BindingFlags.Public | BindingFlags.Instance), 
                rcsMySetImage);
        }

        private void redirectSetText ()
        {
            rcsMySetText = RedirectionHelper.RedirectCalls (
                typeof(LoadingAnimation).GetMethod ("SetText", BindingFlags.Public | BindingFlags.Instance), 
                typeof(Redirector).GetMethod ("mySetText", BindingFlags.Public | BindingFlags.Instance));
        }

        private void revertSetText ()
        {
            RedirectionHelper.RevertRedirect (
                typeof(LoadingAnimation).GetMethod ("SetText", BindingFlags.Public | BindingFlags.Instance), 
                rcsMySetText);
        }
        #endregion 

        public void mySetImage (Mesh mesh, Material material, float scale, bool showAnimation)
        {                   
            LoadingImageContainer loadingImage = new LoadingImageContainer (mesh, material, scale, showAnimation);
            entry = ImageList.getRandomEntry ();

            if (entry == null) {
                keepDefault (loadingImage);
                return;
            }

            try {
                if (entry.isSaveGame) {
                    handleSaveGame (loadingImage);
                } else if (entry.isHTTP) {
                    handleHTTP (loadingImage);
                } else {
                    handleLocal (loadingImage);
                }
            } catch (Exception ex) {
                Debug.Log (ex.ToString ());
                keepDefault (loadingImage);
            }
        }
        
        public void mySetText (UIFont font, Color color, float size, string title, string text)
        {
            try {
                getTextFromImageEntry (out title, out text);
            } catch (Exception ex) {
                Debug.Log (ex.ToString ());
            }

            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_textMaterial", null);

            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_textMaterial", new Material (font.material));
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_font", font);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_textColor", color);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_textSize", size);

            modifyText(title, text);

            revertSetText ();
        }

        #region change the image

        private void changeImage (LoadingImageContainer img, Texture2D bg)
        {
            Material newMaterial = new Material (img.material);
            if (bg != null) {
                newMaterial.mainTexture = bg;
            }
            img.material = newMaterial;

            reflectImageChanges (img);
            Deinitialize ();
        }
        
        private void reflectImageChanges (LoadingImageContainer img)
        {
            float scaleFactor = getScaleFactor (img.material);
            
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_imageMesh", img.mesh);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_imageMaterial", img.material);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_imageScale", scaleFactor);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_imageShowAnimation", img.showAnimation);
            
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_imageLoaded", true);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_imageAlpha", 1f);
        }
        #endregion

        #region handle specific image sources
        private void handleLocal (LoadingImageContainer img)
        {
            byte[] imgdata = System.IO.File.ReadAllBytes (entry.uri);
            Texture2D bg = new Texture2D (1, 1);
            bg.LoadImage (imgdata);
            
            changeImage (img, bg);
        }

        private void handleSaveGame (LoadingImageContainer img)
        {
            SaveGameMetaData savegameMetaData = getSaveGameMetaData ();

            if (savegameMetaData != null && savegameMetaData.imageRef != null) {
                string extraInfo = SimulationManager.instance.m_metaData.m_currentDateTime.ToShortDateString ();
                entry = new ImageListEntry (entry.uri, savegameMetaData.cityName, "", extraInfo);

                Texture2D savegameimage = savegameMetaData.imageRef.Instantiate<Texture> () as Texture2D;
                savegameimage = Blur.FastBlur (savegameimage, 2, 2);

                changeImage (img, savegameimage);
            } else {
                keepDefault (img);
            }
        }

        private SaveGameMetaData getSaveGameMetaData ()
        {
            SaveGameMetaData savegameMetaData = null;
            if (entry.isLatestSaveGame) {
                savegameMetaData = SaveHelper.GetLatestSaveGame ();
            } else if (entry.isCurrentSaveGame) {
                savegameMetaData = getMetaDataForDateTime (SimulationManager.instance.m_metaData.m_currentDateTime);
            }
            return savegameMetaData;
        }

        private static SaveGameMetaData getMetaDataForDateTime (DateTime needle)
        {
            SaveGameMetaData result = null;
            foreach (Package.Asset current in PackageManager.FilterAssets(new Package.AssetType[]
                                                                          {
                UserAssetType.SaveGameMetaData
            })) {
                if (current != null && current.isEnabled) {
                    SaveGameMetaData saveGameMetaData = current.Instantiate<SaveGameMetaData> ();
                    if (saveGameMetaData != null) {
                        try {
                            Stream s = saveGameMetaData.assetRef.GetStream ();
                            SimulationMetaData mysimmeta = DataSerializer.Deserialize<SimulationMetaData> (s, DataSerializer.Mode.File);
                            if (mysimmeta.m_currentDateTime.Equals (needle)) {
                                return saveGameMetaData;
                            }
                        } catch (Exception ex) {
                            ex.ToString ();
                        }
                    }
                }
            }
            return result;
        }

        private void handleHTTP (LoadingImageContainer img)
        {
            Request imgget = new Request ("get", entry.uri);
            imgget.AddHeader ("Accept-Language", "en");

            imgget.Send (httpCallback (img));
        }

        private Action<Request> httpCallback (LoadingImageContainer img)
        {
            return delegate (Request req) {
                try {
                    if (req.isDone) {
                        if (req.exception != null) {
                            throw req.exception;
                        }
                        
                        if (req.response == null || req.response.status != 200) {
                            throw new Exception ("response not 200 " + req.uri);
                        }
                        
                        Texture2D bg = new Texture2D (1, 1);
                        byte[] imgdata = req.response.bytes;
                        bg.LoadImage (imgdata);
                        
                        if (bg.width < 10 || bg.height < 10) {
                            throw new Exception ("image looks wrong " + req.uri);
                        }
                        
                        changeImage (img, bg);
                    }
                } catch (Exception ex) {
                    ex.ToString ();
                    keepDefault (img);
                }
            };
        }

        private void keepDefault (LoadingImageContainer img)
        {
            string msg;
            if (entry != null && !string.IsNullOrEmpty (entry.uri)) {
                msg = "Sorry, could not load " + entry.uri + ".";
            } else {
                msg = "Sorry, could not load any entry.";
            }
            entry = new ImageListEntry ("", msg, "", "ChangeLoadingImage");
            
            changeImage (img, null);
        }
        #endregion

        #region utilities
        private float getScaleFactor (Material newMaterial)
        {
            float scaleFactor = 1f;
            if (newMaterial == null || newMaterial.mainTexture == null)
                return scaleFactor;
            
            float screenWidth = Screen.currentResolution.width;
            float screenHeight = Screen.currentResolution.height;
            
            int imgWidth = newMaterial.mainTexture.width;
            int imgHeight = newMaterial.mainTexture.height;
            
            float widthFactor = screenWidth / imgWidth;
            float heightFactor = screenHeight / imgHeight;
            
            if (widthFactor > heightFactor) {
                float temp = heightFactor * imgWidth;
                scaleFactor = screenWidth / temp;
            } else {
                float temp = widthFactor * imgHeight;
                scaleFactor = screenHeight / temp;
            }
            
            return scaleFactor;
        }

        private void getTextFromImageEntry (out string title, out string text)
        {
            title = "";
            text = "";
            if (!String.IsNullOrEmpty (entry.title)) {
                title += entry.title;
            }
            if (!String.IsNullOrEmpty (entry.author)) {
                text += "by " + entry.author;
            }
            if (!String.IsNullOrEmpty (entry.extraInfo)) {
                text += " (" + entry.extraInfo + ")";
            }
        }

        private void modifyText (string title, string text)
        {
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_title", title);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_text", text);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_textLoaded", true);
            ReflectionUtils.WritePrivate<LoadingAnimation> (loadingAnimation, "m_textAlpha", 0f);
            
            MethodInfo mi = typeof(LoadingAnimation).GetMethod ("GenerateTextMesh", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke (loadingAnimation, null);
        }

        private static bool versionIsSupported ()
        {
            string version = BuildConfig.applicationVersion;
            
            List<string> supportedVersions = new List<string> ();
            supportedVersions.Add ("1.0.7c");
            
            return supportedVersions.Contains (version);
        }
        #endregion
    }
}
