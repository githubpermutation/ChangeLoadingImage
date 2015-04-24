using System;
using ICities;

namespace ChangeLoadingImage
{
    public class Loader : LoadingExtensionBase
    {
        public override void OnCreated (ILoading loading)
        {
            Redirector r = new Redirector();
            r.Initialize ();      
        }

        public override void OnLevelUnloading ()
        {
            Redirector r = new Redirector();
            r.Initialize (); 
        }
    }
}