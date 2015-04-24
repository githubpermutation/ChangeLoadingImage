using System;
using ICities;
namespace ChangeLoadingImage
{
    public class Mod : IUserMod
    {
        public string Name {
            get {
                return "ChangeLoadingImage";
            }
        }

        public string Description {
            get {
                return "Changes the loading image.";
            }
        }
    }
}