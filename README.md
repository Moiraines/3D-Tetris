# 🎮 3D Tetris (OpenGL + C#)

This is a complete 3D Tetris game implemented in **C# using OpenTK**, combining Object-Oriented Programming principles with real-time 3D graphics via OpenGL.  
The project was developed as a coursework assignment in **Object-Oriented Programming and Computer Graphics**.

---

## 🧩 Features

- ✅ 3D Tetris gameplay in a 10x20x10 grid
- ✅ Support for multiple shape types (Cube, Sphere)
- ✅ Real-time rendering with OpenGL (via OpenTK)
- ✅ Dynamic camera with orbital and free-fly modes
- ✅ Text rendering using sprite sheet + OpenGL shaders
- ✅ Soft camera shake effect on landing
- ✅ Rotations around X/Y/Z axes (with Shift modifier)
- ✅ JSON Save/Load game state
- ✅ Game statistics using LINQ queries
- ✅ Clean class hierarchy, extensible design

---

## 🧱 Gameplay Overview

- 3D Tetrominoes fall from the top of the field
- Full surfaces (XY layers) are detected and cleared
- Tetrominoes are constructed from individual `Shape` objects (Cube or Sphere)
- Rotations and movements are fully supported in 3D space

---

## 🎯 OOP Concepts Demonstrated

- ✅ **Encapsulation**: Each class manages its own logic and data
- ✅ **Inheritance**: `Shape` → `Cube`, `Sphere`
- ✅ **Polymorphism**: `Render()`, `Resize()`, `ApplyAppearance()` etc.
- ✅ **Abstract classes and Interfaces**:
  - `Shape` (abstract base class)
  - `IVertexDataProvider`, `IStatefulShape`
- ✅ **Events**: `GameEvents.OnTetrominoLanded`
- ✅ **Composition**: `Tetromino` contains 4 `Shape` objects
- ✅ **LINQ**: Used for runtime statistics and visual feedback
- ✅ **Serialization**: Save/load via JSON

---

## 🔧 Technical Stack

| Component | Description |
|----------|-------------|
| **Language** | C# (.NET) |
| **Graphics API** | OpenGL 3.3 |
| **Framework** | OpenTK 4.x |
| **Shaders** | GLSL (Vertex + Fragment) |
| **Textures** | PNG via ImageSharp |
| **Text System** | Bitmap-based with SpriteSheet |
| **Serialization** | JSON (.NET built-in) |

---

## 🎮 Controls

| Key         | Action                       |
|-------------|------------------------------|
| Arrow Keys  | Move tetromino (X/Z plane)   |
| `X`, `Y`, `Z` | Rotate around axis         |
| `Shift` + Axis Key | Rotate in opposite dir |
| `Space`     | Fast fall (gravity)          |
| `F2` / `F3` | Save / Load game state       |
| `F1`        | Print LINQ stats in console  |
| `Enter`     | Restart game if over         |
| `Middle Click` | Toggle orbit mode         |
| `Right Click` + Drag | Free rotate camera  |
| `WASD` / `Shift` / `F` | Free camera movement |

---

## 📂 Project Structure

```
Tetris/
│
├── Shaders/
│   ├── Cube.vert / Cube.frag
│   ├── Sphere.vert / Sphere.frag
│   ├── Background.vert / Background.frag
│   ├── text.vert / text.frag
│
├── res/
│   ├── Textures (.png)
│   ├── Letters (bitmap sprite sheets)
│
├── Tetris/
│   ├── Core Classes: Game, Playground, Tetromino, Camera, Shader
│   ├── Shapes: Shape (abstract), Cube, Sphere
│   ├── Text System: Text, SpriteSheet, Texture
│   ├── Serialization: SerializableShapeData
│   ├── Utilities: GameMath, GameEvents, enums/
```

---

## 🎓 Academic Context

This project was created as part of the **"Object-Oriented Programming and Computer Graphics"** course. It demonstrates:

- Real-world application of OOP in game logic
- GPU programming with GLSL shaders
- Integration between CPU-side logic and GPU-side rendering
- Dynamic scene rendering with user input
- Clean architecture and extensibility

---

## 🛠️ Build Instructions

- Requires **.NET 6.0+**
- Uses **OpenTK 4.x** (NuGet)
- Uses **SixLabors.ImageSharp** for texture loading

```bash
dotnet restore
dotnet build
dotnet run
```

---

## ✅ Save File Example

A saved state is stored in `save.json`, with all visible shapes serialized including:

- Type (`Cube`, `Sphere`)
- Position (`x`, `y`, `z`)
- State (`Solid`, `Empty`)
- Assigned Color

---

## 📸 Screenshots

*(Insert gameplay screenshots here if applicable)*

---

## 🔐 License

This project is open for academic and educational use.  
Feel free to explore, learn, and extend!
