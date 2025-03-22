# SudokuSolver 🎯

SudokuSolver is an application that creates and solves Sudoku puzzles, developed in **Unity** and **C#**. 🎲🧩

## 🏆 Main Features

- **Unique Solving Techniques**: This application does not rely on brute force to solve Sudoku puzzles. Instead, it uses solving techniques autonomously developed by the developer. 🧠✨
- **Brute Force as Last Resort**: The brute force algorithm is executed only if other techniques are not sufficient for the resolution, and in this case, the user is notified. 🚨
- **User Interface**: While not all images in the game and the logo were created by me, I designed the graphical user interface. 🎨🖌️

## 📝 Development Approach

This project focuses on smart **solving algorithms** 🧠⚡ over rigid OOP principles. The goal was to optimize execution flow 🚀 and ensure a fast and intelligent Sudoku-solving experience.

Rather than strict class structures, the priority was efficiency and performance, making the solver effective. 🎯

## 👨‍💻 Developer

I personally developed the entire code for this application. 💻👨‍💻

## 📥 How to Download and Run the Game

### 📂 1. Clone the Repository
To get a local copy of the project, run the following command:
```sh
git clone https://github.com/michele-bisignano/SudokuSolver.git
```

### 🎮 2. Open the Project in Unity
1. Open **Unity Hub**.
2. Click **Add** and navigate to the folder where you cloned the repository.
3. Select the folder and open the project.

### ▶️ 3. Run the Game
1. Ensure you have all necessary dependencies installed.
2. Press the **Play** button in Unity to start the game.

## 📂 How to Clone Only the Builds Directory

To clone only the `Builds` directory from the repository, you can use the following command:
```sh
git clone --no-checkout https://github.com/michele-bisignano/SudokuSolver.git
cd SudokuSolver
git sparse-checkout init --cone
git sparse-checkout set Builds
git checkout main
```

This will clone only the `Builds` directory into your local repository.

## 📥 How to Download the Builds Directory as a Zip File

If you prefer to download the `Builds` directory as a zip file, you can use the GitHub web interface:

1. Navigate to the [SudokuSolver repository](https://github.com/michele-bisignano/SudokuSolver).
2. Go to the `Builds` directory.
3. Click on the **Code** button (usually green) and select **Download ZIP**.

This will download the `Builds` directory as a zip file to your local machine.

## 📌 Tags

🎲 Unity  
🎲 C#  
🎲 Sudoku  
🎲 Puzzle Game  
🎲 Game Development
