#INTRODUCTION

This is a Unity DEMO with a single scene, containing an agent simulation in a 2D procedurally generated level. 

This DEMO implements:
1) A constructive level generator which generates levels using cellular automata. 
2) A collection of autonomous agents that inhabit the generated level. The agent AI use both steering behaviours and behaviour trees, utilising the https://github.com/sturdyspoon/unity-movement-ai and https://github.com/meniku/NPBehave libraries.


#LEVEL GENERATOR
Assets/Generator/CellularAutomataCaveGenerator.cs is a 2D cave level script using cellular automata, with the following principles:

First, set related settings of the cave map, such as the size and seed of the map and the number of torches and gems.
RandomFillMap randomly generates the map (placing walls and floors).
Call SmoothMap multiple times to smooth the map, determining whether to convert it into a wall or a floor based on the number of surrounding walls for each grid.
GetRegions searches for rooms (called regions) on the map, and then ConnectRegions connects the center points of these rooms, forming a walkable path.

CreateTiles places walls and floors on the map, and PlaceTorchesAndGems places torches and gems.

#AGENTS
In the Assets/Agent directory, AgentSpawner.cs is a script that randomly generates a specified number of three types of agents on the ground. In this directory, there are three folders corresponding to three different agents. 

Taking the Thief folder as an example, the ThiefAI.cs in the folder is a behavior tree script using the NPBehave library, and ThiefArrive.cs, ThiefFle.cse, and ThiefWander.cs are three movement scripts using the Unity-Movement-AI library. 

ThiefAI.cs uses behavior tree to enable the corresponding movement script according to different game states and disable other movement scripts. When the Thief is close enough to the nearest Chief or Troll, it enters the Flee state; when it is close enough to the nearest gem, it enters the Arrive state and goes to the gem's location, and destroys the gem object after colliding with it; when it is not close enough to either, it enters the Wander state. All three states will move while avoiding to collide with walls.

The same applies to Chief and Troll. The Chief will Seek the Thief when it is close enough, otherwise, it will enter the Wander state; the Troll will chase the Thief when it is close enough, otherwise, it will enter the OffsetPursuit state, where it will maintain formation with other Trolls while following the nearest Chief.

# VIDEO PRESENTATION
Video URL：
https://youtu.be/j3qcuO43VXg
(In this video, Thief is green, Chiefs are red, and Trolls are blue.)

Unity Version:
2022.1.17f1
