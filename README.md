# My RL Projects (3D)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Unity Version](https://img.shields.io/badge/Unity-2022.3.22f1-blue)
![ML-Agents](https://img.shields.io/badge/ML--Agents-0.30.0-orange)
![Python Version](https://img.shields.io/badge/python-3.9-blue)
![Status](https://img.shields.io/badge/status-active-brightgreen)

This repository contains a collection of reinforcement learning environments and agents developed for training and testing simple navigation behaviors.

## Projects

### MoveToGoal

In **MoveToGoal**, the agent is tasked with reaching a reward point in the shortest possible time while avoiding any contact with the environment’s borders. The project focuses on optimizing pathfinding behavior under spatial constraints.

**Reward Mechanics**
* Positive Reward:
  * +5: For successfully reaching the target.
* Negative Reward (Penalties):
  * -0.0001: A small penalty applied at each step to encourage faster completion.
  * -0.01: For colliding with a wall (border).

- 📂 [**Go to project folder**](ML-Agents/Examples/MoveToGoal)
<p align="center">
  <img src="ML-Agents/Examples/video_and_graphs/movetogoal_blueboy/movetogoal_blueboy_graph.png" alt="MoveToGoal Eval" width="800"/>
</p>

- 🎥 **Demo:**

<p align="center">
  <img src="ML-Agents/Examples/video_and_graphs/movetogoal_blueboy/movetogoal_blueboy_gif.gif" alt="MoveToGoal Demo" width="600"/>
</p>

## Credits

The following assets are used in this project.

- [Simple Gems and Items Ultimate Animated Customizable Pack](https://assetstore.unity.com/packages/3d/props/simple-gems-and-items-ultimate-animated-customizable-pack-73764#publisher) — Available for free use via the Unity Asset Store (NOT included in this repo).

<!-- Add more assets here in the future -->

---

### SphereHunt
In **SphereHunt**, the agent's objective is to first locate a button and then perform a specific _discrete action_ to press it. Once activated, the goal appears, and the agent must then collect it as quickly as possible. This project emphasizes sequential task completion and efficient navigation within a dynamic environment.

**Reward Mechanics**
* Positive Reward:
  * +2: For successfully pressing the button.
  * +10: For collecting the spawned goal.
* Negative Reward (Penalties):
  * -0.0001: A small penalty applied at each step to encourage faster completion.
  * -0.01: For colliding with a wall (border).

- 📂 [**Go to project folder**](ML-Agents/Examples/SphereHunt)
<p align="center">
  <img src="ML-Agents/Examples/video_and_graphs/spherehunt_yellowboy/spherehunt_yellowboy_graph.png" alt="SphereHunt Eval" width="800"/>
</p>

- 🎥 **Demo:**

<p align="center">
  <img src="ML-Agents/Examples/video_and_graphs/spherehunt_yellowboy/spherehunt_yellowboy_gif.gif" alt="SphereHunt Demo" width="600"/>
</p>
