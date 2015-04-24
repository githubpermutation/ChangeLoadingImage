using System;
using System.IO;
using System.Collections.Generic;
using Random = System.Random;
using ColossalFramework.IO;

namespace ChangeLoadingImage
{
    public class ImageList
    {
        private static readonly string filename = "ChangeLoadingImageList.txt";

        private static string pathToImageList {
            get {
                return Path.Combine (pathToModConfig, filename);
            }
        }

        private static string pathToModConfig {
            get {
                return Path.Combine (DataLocation.localApplicationData, "ModConfig");
            }
        }

        public static ImageListEntry getRandomEntry ()
        {
            if (!handleImageListCreation ())
                return null;

            List<ImageListEntry> entries = imageListFromFile (pathToImageList);
            return selectFrom (entries);
        }

        private static bool handleImageListCreation ()
        {
            if (!System.IO.File.Exists (pathToImageList)) {
                return writeDefaultImageList ();
            }

            try {
                if (userListIsDefault (pathToImageList)) {
                    deleteFile (pathToImageList);
                    return writeDefaultImageList ();
                }
                return true;
            } catch (Exception ex) {
                ex.ToString ();
                return false;
            }
        }
        
        private static bool writeDefaultImageList ()
        {
            try {
                if (!Directory.Exists (pathToModConfig))
                    Directory.CreateDirectory (pathToModConfig);

                string toWrite = "";
                foreach (ImageListEntry entry in defaultImageList) {
                    toWrite += entry.asFileEntry;
                }

                File.AppendAllText (pathToImageList, toWrite);

                return true;
            } catch (Exception ex) {
                ex.ToString ();
                return false;
            }
        }

        private static List<ImageListEntry> imageListFromFile (string pathToUserFile)
        {
            List<ImageListEntry> list = new List<ImageListEntry> ();
            string[] fileEntries = System.IO.File.ReadAllLines (pathToImageList);
            
            foreach (string entry in fileEntries) {
                ImageListEntry imagelistentry = ImageListEntry.parse (entry);
                addEntryToList (imagelistentry, list);
            }

            return list;
        }
                
        private static bool userListIsDefault (String pathToUserFile)
        {
            List<ImageListEntry> defaultList = defaultImageList;
            List<ImageListEntry> userList = imageListFromFile (pathToUserFile);

            if (userList.Count > defaultList.Count)
                return false;

            foreach (ImageListEntry userEntry in userList) {
                if (!defaultList.Contains (userEntry)) {
                    return false;
                }
            }
            return true;
        }
        
        private static void deleteFile (string pathToImageList)
        {
            File.Delete (pathToImageList);
        }

        private static void addEntryToList (ImageListEntry imagelistentry, List<ImageListEntry> entries)
        {
            if (imagelistentry == null || !imagelistentry.isValidPath)
                return;
            
            if (imagelistentry.isDirectory) {
                entries.AddRange (getDirectoryEntries (imagelistentry.uri));
            } else {
                if (imagelistentry.isFile) {
                    String title = Path.GetFileNameWithoutExtension (imagelistentry.uri);
                    imagelistentry = new ImageListEntry (imagelistentry.uri, title, imagelistentry.author, imagelistentry.extraInfo);
                }
                entries.Add (imagelistentry);
            }
        }
        
        private static List<ImageListEntry> getDirectoryEntries (string directoryPath)
        {
            List<ImageListEntry> list = new List<ImageListEntry> ();
            DirectoryInfo dir = new DirectoryInfo (directoryPath);
            
            List<String> extensions = new List<string> ();
            extensions.Add ("*.jpg");
            extensions.Add ("*.jpeg");
            extensions.Add ("*.png");
            
            foreach (String ext in extensions) {
                foreach (FileInfo fileinfo in dir.GetFiles(ext)) {
                    String title = Path.GetFileNameWithoutExtension (@fileinfo.FullName);
                    ImageListEntry imagelistentry = new ImageListEntry (@fileinfo.FullName, title, "", "");
                    list.Add (imagelistentry);
                }
            }
            return list;
        }
        
        private static ImageListEntry selectFrom (List<ImageListEntry> entries)
        {
            if (entries.Count == 0)
                return null;

            Random random = new Random ();
            return entries [random.Next (entries.Count)];
        }
        
        private static List<ImageListEntry> defaultImageList {
            get {
                List<ImageListEntry> list = new List<ImageListEntry> ();
            
                list.Add (new ImageListEntry ("http://i.imgur.com/JbWQLJX.jpg", "Backyards", "nlight", "imgur.com/a/DRQTy"));
                list.Add (new ImageListEntry ("http://i.imgur.com/wSqsUeG.jpg", "Highrises", "nlight", "imgur.com/a/DRQTy"));
                list.Add (new ImageListEntry ("http://i.imgur.com/4jZvqLy.jpg", "100,000 strong today", "ossahib", "redd.it/2zcd5v"));
                list.Add (new ImageListEntry ("http://i.imgur.com/Z58lVbd.jpg", "Back Roads", "rik4000", "reddit.it/31xb4i"));
                list.Add (new ImageListEntry ("http://i.imgur.com/ger5HID.jpg", "Green Energy", "rik4000", "reddit.com/user/rik4000"));
                list.Add (new ImageListEntry ("http://i.imgur.com/CDYVa0G.jpg", "First Person", "raiderofawesome", "redd.it/2ypjuu"));
                list.Add (new ImageListEntry ("http://i.imgur.com/ZRYNL6M.jpg", "Urban T-Interchange", "laosimerah", "redd.it/2ytzo2"));
                list.Add (new ImageListEntry ("http://i.imgur.com/9PPtl5M.jpg", "200,000 and space for more", "laosimerah", "redd.it/2zegoe"));
                list.Add (new ImageListEntry ("http://i.imgur.com/TewWRrV.jpg", "Sunken Highways", "laosimerah", "redd.it/30oqmh"));
                list.Add (new ImageListEntry ("http://i.imgur.com/uYWjck2.jpg", "Highway Service Station", "IVIaarten", "redd.it/31kd5h"));
                list.Add (new ImageListEntry ("http://i.imgur.com/RbPjrLZ.jpg", "Low Field of View 1", "Simify", "redd.it/2zan7v"));
                list.Add (new ImageListEntry ("http://i.imgur.com/C1LAmJE.jpg", "Low Field of View 2", "Simify", "redd.it/2zan7v"));
                list.Add (new ImageListEntry ("http://i.imgur.com/KuHLYpC.jpg", "Welcome to Scotland", "Jonny1233", "redd.it/31bcqo"));
                list.Add (new ImageListEntry ("http://i.imgur.com/btkOJuj.jpg", "Main train line to the city", "Jonny1233", "redd.it/31bcqo"));
                list.Add (new ImageListEntry ("http://i.imgur.com/l005ZQa.jpg", "Anastilt", "Jonny1233", "redd.it/31bcqo"));
                list.Add (new ImageListEntry ("http://i.imgur.com/z3KELWU.png", "Little Ditches", "Simify", "redd.it/30z571"));
                list.Add (new ImageListEntry ("http://i.imgur.com/BQpbKP1.jpg", "Small Town 1", "Snakorn", "redd.it/31nyup"));
                list.Add (new ImageListEntry ("http://i.imgur.com/P6CMu1N.jpg", "Small Town 2", "Snakorn", "redd.it/31nyup"));
                list.Add (new ImageListEntry ("http://i.imgur.com/hB846Q5.jpg", "Small Town 3", "Snakorn", "redd.it/31nyup"));
                list.Add (new ImageListEntry ("http://i.imgur.com/Ahv4scy.jpg", "Small Town 4", "Snakorn", "redd.it/31nyup"));
                list.Add (new ImageListEntry ("http://i.imgur.com/QI0qxyb.jpg", "Small Town 5", "Snakorn", "redd.it/31nyup"));
                list.Add (new ImageListEntry ("http://i.imgur.com/oBihvV0.jpg", "Small Gaps", "Bonova", "redd.it/2z78e7"));
                list.Add (new ImageListEntry ("http://i.imgur.com/RxvSCFz.jpg", "Midwesterner", "TheBlakers", "redd.it/31xk40"));
                list.Add (new ImageListEntry ("http://i.imgur.com/EeklbxQ.jpg", "Korenth Valley 1", "OM3N1R", "redd.it/2z390e"));
                list.Add (new ImageListEntry ("http://i.imgur.com/BgsVZEI.jpg", "Korenth Valley 2", "OM3N1R", "redd.it/2z390e"));
                list.Add (new ImageListEntry ("http://i.imgur.com/z3xQGT9.jpg", "The Town", "wrogn", "redd.it/30pthp"));
                list.Add (new ImageListEntry ("http://i.imgur.com/dyavD1s.jpg", "A Neighbourhood", "wrogn", "redd.it/30pthp"));
                list.Add (new ImageListEntry ("http://i.imgur.com/nYeGAmr.jpg", "Farms on the Outskirts", "wrogn", "redd.it/30pthp"));
                list.Add (new ImageListEntry ("http://i.imgur.com/GYzN9fX.jpg", "Asteria capital", "Vicious713", "redd.it/32hpnf"));
                list.Add (new ImageListEntry ("http://i.imgur.com/ZIqLrJM.jpg", "Asteria harbor", "Vicious713", "redd.it/32hpnf"));
                list.Add (new ImageListEntry ("http://i.imgur.com/2QxzO98.jpg", "Overpass", "Vicious713", "redd.it/323fmx"));
                list.Add (new ImageListEntry ("http://i.imgur.com/3w9TSUh.jpg", "Mountaintop Cathedral", "maledin", "redd.it/30bbxf"));
                list.Add (new ImageListEntry ("http://i.imgur.com/khyuJxo.jpg", "Lorentum Airport", "maledin", "redd.it/30bbxf"));
                list.Add (new ImageListEntry ("http://i.imgur.com/C3CxLlx.jpg", "Countdown", "guikolinger", "redd.it/3305ce"));
                list.Add (new ImageListEntry ("http://i.imgur.com/plprqgC.jpg", "Fischer Heights", "mab1981", "redd.it/330lkp"));
                list.Add (new ImageListEntry ("http://i.imgur.com/puJUL4s.jpg", "With decorative trees", "JuicyJuice23", "redd.it/334ap8"));
                list.Add (new ImageListEntry ("http://i.imgur.com/zP5nndL.jpg", "Downtown", "JuicyJuice23", "redd.it/334ap8"));
                list.Add (new ImageListEntry ("http://i.imgur.com/vWPw4aB.jpg", "Manhattan traffic", "zzjeffrey", "redd.it/336uft"));
                list.Add (new ImageListEntry ("http://i.imgur.com/280KFEG.jpg", "A solar tomorrow", "zzjeffrey", "redd.it/336uft"));
                list.Add (new ImageListEntry ("http://i.imgur.com/TwxmtYB.jpg", "Soccer", "zzjeffrey", "redd.it/336uft"));
                list.Add (new ImageListEntry ("http://i.imgur.com/HZ9ZUGF.jpg", "Cargo", "Novus_", "redd.it/3387bn"));
                list.Add (new ImageListEntry ("http://i.imgur.com/a0TeAwC.jpg", "Exports", "Novus_", "redd.it/3387bn"));
                list.Add (new ImageListEntry ("http://i.imgur.com/7J8nNus.jpg", "Canalville", "Starkie", "redd.it/33cp51"));
                list.Add (new ImageListEntry ("http://i.imgur.com/q6zzMGP.jpg", "Mountaineering", "Tassietiger1", "redd.it/33hnrj"));
                list.Add (new ImageListEntry ("http://i.imgur.com/n90hE77.jpg", "Dosera river fork", "BirdsOfHell", "redd.it/33glm6"));
                list.Add (new ImageListEntry ("http://i.imgur.com/rHq5lVQ.jpg", "Dosera forestry", "BirdsOfHell", "redd.it/33glm6"));
                list.Add (new ImageListEntry ("http://i.imgur.com/zoS4RBY.jpg", "Bird's-Eye View", "BirdsOfHell", "redd.it/33glm6"));

                return list;
            }
	    }
    }
}
