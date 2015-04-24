using System;
using System.IO;

namespace ChangeLoadingImage
{
    public class ImageListEntry
    {
        public string uri;
        public string title;
        public string author;
        public string extraInfo;

        public static char fieldSeparator {
            get {
                return ';';
            }
        }

        public ImageListEntry (string uri, string title, string author, string extraInfo)
        {
            this.uri = uri;
            this.title = title;
            this.author = author;
            this.extraInfo = extraInfo;
        }
        
        public bool isHTTP {
            get {
                return this.uri.ToLower ().StartsWith ("http:") || this.uri.ToLower ().StartsWith ("https:");
            }
        }

        public bool isFile {
            get {
                return isLocal && !System.IO.Directory.Exists(@uri) && System.IO.File.Exists(@uri);
            }
        }

        public bool isDirectory {
            get {
                if (!isLocal || isFile) 
                    return false;

                FileAttributes attr = System.IO.File.GetAttributes (@uri);
                return (attr & FileAttributes.Directory) == FileAttributes.Directory;
            }
        }

        public bool isSaveGame {
            get {
                return isLatestSaveGame || isCurrentSaveGame;
            }
        }

        public bool isLatestSaveGame {
            get {
                return this.uri.ToLower ().StartsWith ("latestsavegame");
            }
        }

        public bool isCurrentSaveGame {
            get {
                return this.uri.ToLower ().StartsWith ("currentsavegame");
            }
        }

        private bool isLocal {
            get {
                return !isHTTP && !isSaveGame;
            }
        }

        public bool isValidPath {
            get {
                if(!isLocal)
                    return true;

                try {
                    FileInfo fi = new System.IO.FileInfo(@uri);
                    FileAttributes attr = System.IO.File.GetAttributes (@uri);
                } catch (Exception ex) {
                    ex.ToString();
                    return false;
                }
                return true;
            }
        }

        public string asFileEntry {
            get {
                return string.Format ("{0}{1}{2}{1}{3}{1}{4}{1}{5}", uri, fieldSeparator, title, author, extraInfo, Environment.NewLine);
            }
        }

        public override bool Equals (object other)
        {
            if(!(other is ImageListEntry))
                return false;

            return this.asFileEntry.Equals(((ImageListEntry)other).asFileEntry);
        }

        public override int GetHashCode ()
        {
            return asFileEntry.GetHashCode();
        }

        public static ImageListEntry parse (string entry)
        {
            string[] items = entry.Split (ImageListEntry.fieldSeparator);
            if (items.Length == 0 || items [0] == null || String.IsNullOrEmpty (items [0])) {
                return null;
            }
            string uri = items [0];
            
            string title = "";
            if (items.Length > 1) {
                title = items [1];
            }
            
            string author = "";
            if (items.Length > 2) {
                author = items [2];
            }
            
            string extraInfo = "";
            if (items.Length > 3) {
                extraInfo = items [3];
            }
            
            return new ImageListEntry (uri, title, author, extraInfo);
        }
    }
}