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

The game itself is not very beginner friendly if you have no experience with similar games in the past.
I do plan to create a tutorial but due to time restrictions there is nothing at the moment.

Here is a quick show of features that I have edited together from ~50 mins of gameplay:

https://github.com/user-attachments/assets/a0882a64-3d6d-41d0-8d26-a73d40463352

The filesize limit is 10MB so it is quite challenging to show more at once.

### Install

The Game is written for unity.

Download a zip file of the project at https://github.com/ssobedevad/EUDave-Builds

Unzip

Run EUDave.exe

Done

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

<img width="420" height="617" alt="image" src="https://github.com/user-attachments/assets/dd172f25-b600-4596-b196-cbefd25310b2" />

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

<img width="1385" height="644" alt="image" src="https://github.com/user-attachments/assets/57b3622b-25a0-4e6f-b709-1b83610be109" />

The Names have been spaced out over each seperate area based on the difference between the highest,lowest,leftmost and rightmost tiles and sized and rotated to fit the area the name is then centered at the tile with the most neighbours belonging to the same area.

Shaders:

This uses moving Voronoi noise for the water and layered perlin noise for the terrain. 
All written in hlsl.

### Non-Player controller decision making and coordination

The amount of possible actions that can be taken in this game is very large and there is no specific goal so it is very challenging to create a general controller that interacts with both the player and the other controllers in a dynamic way.
This is where a lot of time and research has gone in to researching the implementations of this in other games of the same genre.
It is my understanding that at the moment there is no use of machine learning in any games that I have seen for this and I was intrigued as to how challenging it would be.
Due to the time and resources required I have instead used a deterministic algorithm that gives a dynamic feel when combined with a set of generated 'personalities' that can be easily customised.
This gives a similar outcome to machine learning as it takes actions based on specific observations with pre specified weightings.

The most important part of the controller is how it handles its armies as this both regulates the difficulty and increases the engagement with the game as a more strategic enemy causes you to have to create strategies of your own.

The approach I took for this:
```
Locate all enemy armies and store information about them such as their positions, strength and whether they are moving or not.

Merge information about nearby enemies strengths to give the impression of an individual strong enemy.

Sort all own armies into categories based on what action they are currently completing.

Try to merge low strength armies together.

If any battles:
  Try to find sufficient nearby own armies to support
  If successful move all armies to enemy position
  Remove moved armies from free armies

If any enemy armies:
  Try to find a collection of own armies that can reach enemy army with combined strength sufficient to fight enemy
  If successful move all armies to enemy position
  Remove moved armies from free armies

If any free armies:
  Send army to siege enemy unsieged areas that are not under siege or there are not enough units to siege them
```
After many iterations this fairly simple deterministic approach produced excellent results with the correct weightings. 

Diplomatic Decisions:

In order to simulate general diplomacy each action has a score for how much the controller supports it.
If the value is above zero then the controller would accept the action.
A similar approach is used to determine who the controller would declare war on which scores all neighbors and if any neighbour scores above zero then the neighbor with the highest score is declared upon.
With the correct score system this can be heavily influenced by personalities to make for very dynamic games.

### Lots of performance tuning using unity cpu profiler

Some of the biggest performance saving has been on organising the map and how it is diplayed as it is very large and requires continuous updates. 
Most notably I managed an 85% reduction in time for managing the color that each tile on the map shows up as this is something that can change very frequently and is a key part of the visual feedback of the game.
By storing data about the last color in each tile and creating a lighter weight equality for color I was able to use the slower methods for changing tile colors much less frequently.
Also by storing information about neighboring tiles inside the tiles themselves it saves a lot of time that I was previously calculating this every time that it was required even though it remained constant throughout the game.

The game performs very well on my 10 year old machine and I am trying hard to maintain efficiency as I add new features as this is one of the reasons why I decided to create my own game.
Other games of this genre are mostly poorly optimised as there is a shift away from efficient code due to the power of modern machines and I still feel that it is incredibly important to keep as a priority.

### Combat and Battle lines

An important factor of a map game is ensuring the combat is fairly predictable while being simple enough to understand and well displayed to the player. 
I have used a two level battle line system to simulate warfare in which units are entered into the line if they could hit something on the opposing side. 
Initially filled by the side with the smallest number of units outwards from the central position. 
The other side then fills to a width where all placed units can hit units on the opposing line based on their range.
Leftover units are kept in reserve to replace dead/retreated units.

<img width="476" height="397" alt="image" src="https://github.com/user-attachments/assets/060bfaf9-c40e-4cd1-aa7e-7376ed3bfefc" />

At each battle phase both sides attack simultaneously.
A simulated dice is rolled every 6 phases for each side determining the effectiveness of the next 6 phases.
Each unit picks a target depending on its preferences and attacks based on various stats and the current dice roll for that side dealing casualties to the target unit and reducing their morale.
All units lose morale every phase based on the average maximum morale of the opposing side.
Units retreat if their morale drops below zero or if they reach zero strength (1000 casualties).
In the image above the color Red(zero morale) -> Green(maximum morale) and the height of each square represents the remaining strength of that unit.
The symbol on each square shows the unit type.
There are no units in the back row as there are no ranged units in this combat.

## References
RedBlobGames Hexagonal Grids https://www.redblobgames.com/grids/hexagons/

RedBlobGames Astar Pathfinding https://www.redblobgames.com/pathfinding/a-star/introduction.html

Perimeter Helper https://dillonshook.com/hex-city-borders/

Land Warfare https://eu4.paradoxwikis.com/Land_warfare

Shaders https://www.ronja-tutorials.com/
