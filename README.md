# TicTacToe
# Instalation
- If you want to open the project via Visual Studio, it will throw an error that says the files aren't found. To fix that, just place Data folder from the repository into bin/Debug/net6.0-windows/ (Built app location).
- If you want to play it, just download, unpack the zip in some folder, and start the .exe.  
Make sure the .exe you're launching is in the same folder as Data, or it'll not find the required files. You may create a shortcut for convenience.   
Windows SmartScreen may warn you, but i don't yet know how to fix that.
## Features:  
- Basic game mechanics
- Intuitive GUI
- Game field of customizable size, up to 12x12. Field may not be a square
- Win combinations are generated via field size, diagonals are not included if field is not a square
- Wins are displayed
- Win counting
- Wins are saved into a file
- Wins can be reset with a button
- Upon winning the combination that won is highlighted with red
- Current turn is displayed
- Reset field button
- Two languages (translations), can be easily extended
- Easily extendable and editable translations, stored in .txt's
- The app consists of a single .exe, with Data folder for... data
## Coming in future updates:
- Draw
- More than 2 symbols (players)
- Customizable symbols
- Color schemes
- Export/Import wins
## May come some time in the future:
- AI
- Selectable win combinations
- Non-rectangular maps
