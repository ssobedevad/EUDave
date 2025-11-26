# Project Overview
## Strategy Map Game
### Introduction
After graduating university I was excited to have time to spend on creating a game.
I chose to make a strategy game similar to some that I have played previously and spent some time prototyping a framework and planning various features that I would like to implement.
This was a challenging process that required many restarts and lots of learning opportunities along the way that lead to what I have currently.
Due to the challenge of digital art I have used a combination of computer generated graphics, simple icons and some AI generated art as placeholders to save lots of time allowing me to focus on developing and solving problems.
I have experimented with some machine learning in an attempt to both make the game more dynamic and challenging however after some testing the ML algorithm would simply take too long to train before being effective due to the complex nature of the game and the possible actions that can be taken.
However, this does not mean that it cannot be implemented at smaller scale as ML could be used to control aspects such as moving units during combat or managing the economic situation of the nation and will be looked at in more detail in the future.

### The Game

Here is some sample gameplay that I have edited together:

https://github.com/user-attachments/assets/a0882a64-3d6d-41d0-8d26-a73d40463352


### Install

Download zip of https://github.com/ssobedevad/EUDave-Builds

Run EUDave.exe

## Complex Algorithms and Features

### Pathfinding A* Hexagonal Grid:
In order to process the movement of units within the game they have to find the best path to any other point in the game. 
This would need to be calculated very frequently due to the number of units, size of the map and number of CPU's. 
This means that implementing an algorithm such as A* is essential to keep the game running smoothly.
An efficient way to do this is by using a priority queue as it is very fast as it uses heaps to store data and only the highest priority(lowest weight) is required to be accessed at any point.
The basic algorithm is simple but very neat and goes as such:

```
Input start and end points:

Priority Queue (Frontier) containing only the starting position

"Setup information for backtracking and minimizing cost"
Dictionary containing the best direction of flow and cumulative minimum cost (cameFrom,costSoFar)

"Astar pathfinding algorithm"
While Frontier not empty and not reached end position:
  position = Dequeue from Frontier
  newCost = costSoFar[position] + 1
  foreach neighbour of position:
    if costSoFar[neighbour] > newCost or costSoFar[neighbour] not set
      costSoFar[neighbour] = newCost
      Enqueue neighbour to frontier with weight newCost + 'Best Possible Distance To End Position'
      cameFrom[neighbour] = position

if not reached end position then return empty path

"Backtrack to find path"
currentPos = end position
new empty List Positions (Path)
while currentPos not startPos:
  Add currentPos to Path
  currentPos = cameFrom[currentPos]

return Path Reversed
```

Example path avoiding obstacles (Sea):

<img width="688" height="403" alt="image" src="https://github.com/user-attachments/assets/992bdd9f-b016-4684-bd28-77fa3c5e2ccf" />

### Hexagonal Perimeters of complex areas

One interesting problem for a map game is dealing with creating borders and names as a way of visually feeding back to the player about their progress. 
This is very important as there are no set goals there must be a sense of achievement and improvement which can be given with large map names and colors.
To create this effect the first problem to solve is finding all perimeter tiles and then giving them an order in which to draw from which turns out to be rather challenging.

The Simple approach:

If you assume there is only one area and it is a fairly standard shape then you can just work from the top left tile and move clockwise until you return to your starting position. However there are some exceptions to this that are not easily dealt with:

-Multiple disconnected areas 

-Strange shapes where the highest leftmost point is not the most extreme point on the top left of the shape or similar

The Number of Bordering Tiles approach:

If you give each tile a score based on how many neighboring tiles they have that are within the area that you want to create a border around then remove any with 6 or more (cannot be a border as that means that they are completely surrounded) and then start on the top left extreme point (lowest neighbors).
This allows you to also remove any tiles not included in the border and then create another border for the remaining and repeat dealing with the issue of disconnected areas. 

Map with Borders and Names:

<img width="834" height="370" alt="image" src="https://github.com/user-attachments/assets/ae553e79-c03f-4ec8-9488-9f137cf46392" />

The Names have been spaced out over each seperate area based on the difference between the highest,lowest,leftmost and rightmost tiles and sized and rotated to fit the area the name is then centered at the tile with the most neighbours belonging to the same area.

Map With Water and Terrain Shaders:

<img width="1292" height="540" alt="image" src="https://github.com/user-attachments/assets/fe0abba4-4113-4532-985d-51d3e6fd4d66" />

This uses moving Voronoi noise for the water and layered perlin noise for the terrain. 
All written in hlsl.
### CPU controller decision making and coordination

The amount of possible actions that can be taken in this game is very large and there is no specific goal so it is very challenging to create a general CPU that interacts with both the player and the other CPU's in a dynamic way.
This is where a lot of time and research has gone in to researching the implementations of this in other games of the same genre.
It is my understanding that at the moment there is no use of machine learning in any games that I have seen for this and I was intrigued as to how challenging it would be.
Instead to make it clearer I will be implementing my own "Personaties" as a way to influence the weightings for decisions and giving the CPU's certain goals to give them a more engaging feel with the potential of inserting ML to help them achieve this in the future.

### Lots of performance tuning using unity cpu profiler

Some of the biggest performance saving has been on organising the map and how it is diplayed as it is very large and requires continuous updates. 
Most notably I managed an 85% reduction in time for managing the color that each tile on the map shows up as this is something that can change very frequently and is a key part of the visual feedback of the game.
By storing data about the last color in each tile and creating a lighter weight equality for color I was able to use the slower methods for changing tile colors much less frequently.
Also by storing information about neighboring tiles inside the tiles themselves it saves a lot of time that I was previously calculating this every time that it was required even though it remained constant throughout the game.

### Combat and Battle lines

An important factor of a map game is ensuring the combat is fairly predictable while being simple enough to understand and well displayed to the player. 
I have used a two level battle line system to simulate warfare in which units are entered into the line if they could hit something on the opposing side. 
Initially filled by the side with the smallest number of units outwards from the central position. 
The otehr side then fills to a width where all placed units can hit units on the opposing line based on their range.
Leftover units are kept in reserve to replace dead/retreated units.


## References
RedBlobGames Hexagonal Grids https://www.redblobgames.com/grids/hexagons/

RedBlobGames Astar Pathfinding https://www.redblobgames.com/pathfinding/a-star/introduction.html

Perimeter Helper https://dillonshook.com/hex-city-borders/

Land Warfare https://eu4.paradoxwikis.com/Land_warfare

Shaders https://www.ronja-tutorials.com/
