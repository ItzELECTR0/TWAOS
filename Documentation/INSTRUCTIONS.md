<div align="left">

## HOW TO GET THE PROJECT LOCALLY

You might consider going to Code > Download ZIP
<br>However, a better way is to clone the project
<br>There are multiple ways of doing this, either through the GitHub Desktop app or CLI

### HOW TO CLONE WITH GITHUB DESKTOP

- 1 - Install all [dependencies](DEPENDENCIES.md) required
- 2 - Open the GitHub Desktop app, and log in
- 3 - Select the clone a project option, and go to the URL tab
- 3.1 - In the URL tab type ItzELECTR0/TWAOS in the URL field
- 3.2 - Select a path where to clone the project
- 4 - Select clone and wait (Will take quite a while)

### HOW TO CLONE WITH CLI

- 1 - Install all [dependencies](DEPENDENCIES.md) required
- 2 - Open a termial of your choosing
- 3 - Navigate into your desired path to clone the project with

      cd "D:\path\for\project"
  - 3.1 - Letter doesn't have to be D, you can use any drive.
- 4 - Type or paste in this command

      git clone https://github.com/ItzELECTR0/TWAOS.git
- 5 - If you are asked to input your email and password, input your email, but not your password, enter your github token instead
- 6 - The project should now be cloning, if not, just repeat the command at step 4

### HOW TO CREATE A GITHUB TOKEN FOR CLONING

- 1 - To create a token, go back to the github website and go to your settings
- 2 - Scroll to the bottom and access Developer Settings
- 3 - Expand Personal access tokens and select Tokens (classic)
- 4 - Expand Generate new token and select Generate new token (classic)
- 5 - Select No expiration (Or any desired expiration time)
- 6 - Scroll to the bottom and select Generate token
- 7 - Copy the created token, and use it in the password field in the terminal

## HOW TO BUILD FROM SOURCE
  
- 1 - Install all [dependencies](DEPENDENCIES.md) required
- 2 - Clone the project
- 3 - Add the project to Unity Hub to open it
- 4 - Once inside the project, to build and play you are required to disable the Steam DRM, even if you own a copy on steam
- 5 - Once you have disabled the DRM, go back to the build settings, and press the build button
- 6 - You will be asked to select a folder where the game will be built, choose an empty one prefferably
- 7 - Now just wait

#### Disclaimers:
 - Project can only be opened and built on Windows 10/11 (Attempts on Arch Linux have always failed me)
 - Building will take a really long time, since the game's shader variants aren't included, and will have to be built from scratch

## HOW DISABLE/REMOVE STEAM DRM

### Medthod 1 - RECOMMENDED
- 1 - Open Edit > Project Settings
- 2 - Go to the "Player" options
- 3 - Expand "Other Settings" and scroll down
- 4 - Inside of "Scripting Define Symbols" find "ELECTRIS_ENABLE_STEAMWORKS"
- 5 - Select the field by clicking the two lines by it's side
- 6 - Remove the entry by clicking the minus sign at the bottom of the list

### Method 2 - DEPRECATED (Still works, but not recommended)
- 1 - To disable the DRM, first go to File > Build Settings to see all of the active scenes
- 2 - Go inside the scenes present in the build settings, and find their "Game Manager" at the top of the hierarchy
- 3 - With the Game Manager selected, go to the inspector and find "Steam Manager" as a component
- 4 - Disable or Remove the Steam Manager to remove the DRM

#### Disclaimer:
 - Disabling Steam DRM will remove any online functionality from the game, and there is no way to avoid this

</div>
