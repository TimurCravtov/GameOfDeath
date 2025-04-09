# **Game of Death: A Magical Twist on Conway’s Game of Life**

> ✨ **Project Completed**  
> This project was developed for the **Multimedia Technologies** course as part of the **FAF 23x Laboratory 2 Game Jam**.  
> Explore a magical cellular world inspired by Harry Potter, where life, death, and transformation are driven by house loyalty, zone effects, and legendary characters.

---

## **📌 Overview**

**Game of Death** is a fantasy twist on **Conway’s Game of Life**, implemented in **Unity (C#)**. Inspired by the Harry Potter universe, this simulation models dynamic interactions between Hogwarts houses, featuring magical zones, character transformations, and house rivalries in a 2D grid-based evolution system.

---

## **🧠 Theme & Concept**

Every cell belongs to a Hogwarts house or legendary figure. Evolution follows magical rules, influenced by special zones:

- 🧙 **Dumbledore** appears in Gryffindor-rich areas and may revive neighbors
- 🐍 **Voldemort** grows in Slytherin territory and converts other houses
- Zones like **Slytherin Dormitory** or **Dumbledore’s Office** apply special mechanics that alter survival, transformation, or reproduction

---

## **⚙️ Simulation Rules**

### 📦 **Cell Types**
- **Hogwarts Houses**: Gryffindor, Slytherin, Hufflepuff, Ravenclaw
- **Special Characters**: Dumbledore, Voldemort
- **Dead Cells**: Empty tiles

---

### 🔁 **Basic Rules**
- **Overcrowding / Underpopulation**: Any cell with **<2 or >5** neighbors dies
- **Standard Reproduction**: Dead cells with **exactly 3** neighbors are reborn as the **most common** nearby house

---

### 🧭 **Special Zones**

#### 🐍 Slytherin Dormitory
- Gryffindors **die** inside
- Dumbledore cannot survive
- Voldemort is empowered and **converts** neighbors
- Reproduction requires **exactly 4** neighbors

#### 🧙‍♂️ Dumbledore’s Office
- Slytherins **die** inside
- Dumbledore and other houses behave normally
- Voldemort transforms nearby cells
- Reproduction requires only **2** neighbors

---

### 🏰 **House-Specific Rules**
- Houses survive with **2–3 neighbors of the same house**
- **Diversity kills**: Cells die if surrounded by **4+ different house types**
- **Rivalries**:
  - Gryffindors and Slytherins **destroy each other** if surrounded by **4+ enemies**
  - Hufflepuff dies with **2+ Slytherin neighbors**

---

### ⭐ **Special Characters**

#### Dumbledore
- Dies if near **any Voldemort** or **>5 Slytherins**
- Can revive dead neighbors with **50% chance**, as random house cells

#### Voldemort
- Dies if surrounded by **3+ Dumbledores**
- Converts all adjacent **non-Slytherin** cells into Voldemorts

---

## **🎮 Features**

- 🧩 Grid-based evolution system with rich house logic
- 🧠 Zone-aware behavior modification system
- 🎨 Custom art, map tiles, and UI buttons
- 🎵 Harry Potter-inspired background music
- 🎛️ Timer, start/stop controls, and dynamic interactions

---

## **👥 Development Team**

| Name                     | Contributions                                                                 |
|--------------------------|------------------------------------------------------------------------------|
| **Alexandru Rudoi**      | Engineered cell prefab system, enums, behavior structures, and prefab binding |
| **Artur Țugui**          | Created pixel-to-grid zone system, implemented start/stop timer, added background music |
| **Nicolae Marga**        | Implemented core simulation loop and most gameplay rules |
| **Timur Cravțov**        | Created visual assets, map tiles, and designed the grid layout |
| **Vladimir Vitcovschii** | Developed character control logic, added UI buttons, and designed the title screen |

---

## **🧱 Technologies**

- **Engine**: Unity
- **Language**: C#
- **Graphics**: Custom-designed sprites, house icons, UI
- **Audio**: Miragine War - Fight Theme + ambient sounds
- **Logic**: Conway-style cellular automaton with magical extension rules

---

## **🗂️ File Structure**

From your Unity project:
```
Assets/
├── Resources/
│   ├── Backgrounds/
│   ├── Buttons/
│   ├── Cells/
│   └── Prefabs/
├── Scenes/
├── Scripts/
├── Settings/
├── TextMesh Pro/
```

- **Resources/**: Visual assets, cell textures, UI buttons
- **Prefabs/**: Dumbledore, Voldemort, and house-specific cell prefabs
- **Scripts/**: All gameplay, zone, and simulation logic
- **Scenes/**: Title screen and main simulation scene

---

## **🚀 How to Run**

1. Open the project in **Unity 2022.3 or later**
2. Open the title scene (`Scenes/`)
3. Click **Play** in the Unity Editor
4. Use the **buttons** to control simulation start/stop
5. Watch magical chaos unfold between Hogwarts houses 🧙‍♂️⚡

---

## **🎓 License & Acknowledgments**

- This project was created for academic purposes under the **Multimedia Technologies** course.
- All visuals and logic were created in-house.

---

Thanks for playing **Game of Death** — may the best house survive!
