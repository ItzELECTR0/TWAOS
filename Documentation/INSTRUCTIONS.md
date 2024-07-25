<div align="left">

## HOW TO BUILD FROM SOURCE

<div>
  
1 - Clone the project.
<br>2 - Install all [dependencies](DEPENDENCIES.md) required
<br>3 - Add the project to Unity Hub to open it
<br>4 - Once inside the project, you might want to disable the steam DRM
<br>4.1 - To disable the DRM, first go to File > Build Settings to see all of the active scenes
<br>4.2 - Go inside the scenes present in the build settings, and find their "Game Manager" at the top of the hierarchy
<br>4.3 - With the Game Manager selected, go to the inspector and find "Steam Manager" as a component
<br>4.4 - Disable or Remove the Steam Manager to remove the DRM
<br>5 - Once you have disabled the DRM, go back to the build settings, and press the build button
<br>6 - You will be asked to select a folder where the game will be built, choose an empty one prefferably
<br>7 - Now just wait

</div>

### Disclaimer: Building will take a really long time, since the game's shader variants aren't included, and will have to be built from scratch

</div>
