# Conscious - A point-and-click game engine for thought-driven adventure games
This is a small game engine for games using the implemented thought-interaction mechanic. This project is focused on prototyping.

To see the game engine in action with a very rudimentary prototype named "Getting up is hard", see the itch.io-Project site [here](https://waackph.itch.io/getting-up-is-hard). The content tool mentioned below is also available [here](https://waackph.itch.io/conscious-content-tool).

## Idea/Vision of this game engine prototype
In the future, this game enigne might be used to create different games with the implemented game mechanics, most prominently, the thought-driven interaction system and the emotional state system which changes the world around the protagonist. From this mechanics, different story approaches can be taken that frame the mechanics, and the player using them, in different ways.

## Technical background
The game engine is implemented via the Monogame C# framework. To create own games with it, Monogame needs to be installed and the content defined for the game must be also imported via the monogame content pipeline tool MGCB editor. To create content a [content tool](https://github.com/waackph/Game-Content-Tool) was implemented, accompaning the game engine. There, all objects (rooms, items, characters) to interact with, can be created and exported. The exported data structure needs to be placed in the `new_states` folder. The game engine will then create the game from the JSON data structure.
The content tool is a react web application and can be found in this github project.

## Build games from the engine
Linux: `dotnet publish -r linux-x64 -c Release --self-contained --output artifacts/linux`
<br>
Start game in unzipped folder with: ./conscious

Windows: `dotnet publish -r win-x64 -c Release --self-contained --output artifacts/windows`
<br>
Start game by executing the .exe file

To automate build process, see: https://learn-monogame.github.io/how-to/automate-release/


# Things to note about monogame

## Spritefont

A spritefont has a character range. It is necessary to define the needed characters e.g. using a unicode decimal table like [this](https://www.ssec.wisc.edu/~tomw/java/unicode.html) and [character codes](https://en.wikipedia.org/wiki/List_of_Unicode_characters).

## Run game in Dev Mode
- Either use C# Debugger
- Or if this does not work use the command line: `dotnet run --project conscious.csproj`
- Start Game Content Tool via `dotnet mgcb-editor Content/Content.mgcb` to enable shader compilation
