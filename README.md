# Archipelago
Archipelago is a tile based mobile strategy game where players compete for territory with the goal of reaching a predetermined sum of money more quickly than their opponents.
Players can build worker units to capture new tiles and build new buildings, which help both to defend their territory as well as produce more money.
In addition players can also train warriors to battle the opponent head on to gain their territory and buildings.

^^^^^ One paragraph to describe your project. Your description should include the project concept andkey features implemented. ^^^^^

## Libraries
Provide a list of **ALL** the libraries you used for your project. Example:

**google-gson:** Gson is a Java library that can be used to convert Java Objects into their JSON representation. It can also be used to convert a JSON string to an equivalent Java object. Source [here](https://github.com/google/gson)

## Installation Notes
Installation instructions for markers.

## Code Examples
You will encounter roadblocks and problems while developing your project. Share 2-3 'problems' that your team solved while developing your project. Write a few sentences that describe your solution and provide a code snippet/block that shows your solution. Example:

**Problem 1: We needed a method to calculate a Fibonacci sequence**

A short description.
```
// The method we implemented that solved our problem
public static int fibonacci(int fibIndex) {
    if (memoized.containsKey(fibIndex)) {
        return memoized.get(fibIndex);
    } else {
        int answer = fibonacci(fibIndex - 1) + fibonacci(fibIndex - 2);
        memoized.put(fibIndex, answer);
        return answer;
    }
}

// Source: Wikipedia Java [1]
```

## Feature Section
List all the main features of your application with a brief description of each feature.

## Final Project Status
Write a description of the final status of the project. Did you achieve all your goals? What would be the next step for this project (if it were to continue)?

#### Minimum Functionality
- Feature 1 name (Completed)
- Feature 2 name (Partially Completed)
- Feature 3 (Not Implemented)

#### Expected Functionality
- Feature 1 name (Completed)
- Feature 2 name (Partially Completed)
- Feature 3 (Not Implemented)

#### Bonus Functionality
- Feature 1 name (Completed)
- Feature 2 name (Partially Completed)
- Feature 3 (Not Implemented)

## Sources
Use IEEE citation style.
What to include in your project sources:
- Stock images
- Design guides
- Programming tutorials
- Research material
- Android libraries
- Everything listed on the Dalhousie [*Plagiarism and Cheating*](https://www.dal.ca/dept/university_secretariat/academic-integrity/plagiarism-cheating.html)
- Remember AC/DC *Always Cite / Don't Cheat* (see Lecture 0 for more info)

[1] "Java (programming language)", En.wikipedia.org, 2018. [Online]. Available: https://en.wikipedia.org/wiki/Java_(programming_language).

------ PRIOR README BELOW ------

CSCI4176 Group3
Minimum Functionality
At a minimum we shall deliver:
A tiled map with three segments, one for each player and a neutral zone in the middle
One (1) resource: money
Three (3) units: one (1) for building, one (1) for gathering resources, and one (1) for defending 
One (1) offensive action to capture an enemy tile
A single resource collection point in the neutral zone
Two (2) buildings, one(1) which can defend a tile and one (1) that can increase the income from a tile
One (1) trap a player can use defensively without the other player being aware of it
A stand in A.I. which acts as an opponent but performs no actions 
Basic Sound effects

Expected Functionality 
In addition to minimum functionality we expect to deliver:
Multiple resources with money being the primary resource and others being used to allow access to higher tier units
Multiple tiers of units that function as better versions of the basic units but require higher tier resources
Multiple resource collection points around the map of varied value.
Multiple offensive actions for the player to use against their opponent
Multiple traps a player can use defensively without the other player being aware of it

Bonus Expectation
Lobby based network multiplayer (no matchmaking or ranking)
Random map generation
Highly polished Visuals  
Team or Free-For-All game mode
More units, maps, tools, features, etc.
Player Progress Tracking
Local Multiplayer (2 Players 1 screen)
