# ChangeLoadingImage
ChangeLoadingImage mod for Cities: Skylines

This mod changes the loading image to a random image from a user-supplied list.

#Usage
##Getting started
Subscribe to the mod via the Steam Workshop (or put it in your local Mods if you are compiling manually) and enable it in the game's content manager.

Once added, it will read (and create if needed) the file ChangeLoadingImageList.txt in your Cities: Skylines user folder:
* Windows: `C:\Users\<username>\AppData\Local\Colossal Order\Cities_Skylines\ModConfig\`
* Mac: `/Users/<username>/Library/Application Support/Colossal Order/Cities_Skylines/ModConfig/`
* Linux: `/home/<username>/.local/share/Colossal Order/Cities_Skylines/ModConfig/`

##Default configuration
The mod ships with a default ChangeLoadingImageList.txt. It contains links to Cities: Skylines screenshots with attributions to serve as an example.

##Custom configuration
ChangeLoadingImageList.txt is a list of image sources, with each line representing one source. Each line must start with a path to either a local image file, a local directory or a URL to an image. For example:
```
C:\Users\MyUserName\Desktop\images\
C:\Users\MyUserName\Desktop\family\cat.jpg
http://i.imgur.com/H2mby53.jpg
```

Each line (except for directories and special sources) can have additional fields separated by a `;`. The fields are: 
* path
* title
* author
* additional information

For example
```
http://i.imgur.com/H2mby53.jpg;Cats on stairs;13ucci;http://redd.it/2ytelo/
```

##Special sources
You can add the line
```
LATESTSAVEGAME
```
to the ChangeLoadingImageList.txt, just like any other source. If the mod chooses this line, it will get your latest savegame and display its preview screenshot. Due to technical limitations (low resolution), this image is blurred, so it might not look great everywhere.

The same works with
```
CURRENTSAVEGAME
```
which will show the preview image from the game you are currently loading. 

##Disabling the mod
You can disable ChangeLoadingImage via the Content Manager.

#Please note
ChangeLoadingImage injects itself into the loading process by redirecting the game's image loading mechanism. This means that it can not and will not change anything about your savegame. However, this also means that the game might expect some value to be there which this mod did not write correctly, which could lead to crashes. I have thoroughly tested every combination of errors I can think of, but do not guarantee this to be an error-free mod.

If your game crashes while loading since you have enabled ChangeLoadingImage, please disable it and leave a comment, ideally with the contents of your ChangeLoadingImageList.txt and what your were trying to load (an editor or a game).

#Known issues
ChangeLoadingImage should fail gracefully when it cannot load an image, but I might not have tested every possible combination.

Some of the default image URLs may not be reachable at any moment. ChangeLoadingImage will inform you if it can't load something.

The background will be black while an image is loaded. There is no preloading, yet.

Please report any issues you find.

#Attributions
The default images are fully sourced in the ChangeLoadingImageList.txt file.

Thanks to the people of `#skylines-modders @ irc.esper.net` for many ideas and solutions.
