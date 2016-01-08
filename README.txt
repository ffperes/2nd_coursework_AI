The repository files of this coursework can be found at:
https://github.com/ffperes/2nd_coursework_AI/

Link to my video:
https://youtu.be/UfaAERQGLrE
-----------------------------------------------------------------------------------------------------------------------------------------
How to use the scene
1-	Keep both scene view and game view windows displayed at the screen
2-	Run the Final Scene using Unity3D.
3-	Left click the mouse in the game window and observe the results in the scene window
4-	Better observation is obtained if in the scene window the ‘Y’ axis view is selected (it should be by default)
5-	During run time, values such as, width, height and level of refinement can be changed, but is advised to not increase the value of refinement above 50%.

-----------------------------------------------------------------------------------------------------------------------------------------

Procedural Content Generation

For this coursework, I will be using procedural content generation AI technique to create a dungeon level using cellular automata and tiles.
Procedural Content Generation is the programmatic generation of game content using a random or pseudo-random process that results in an unpredictable range of possible game play spaces.
Cellular automata is a discrete model in computing theory. It consists of a regular grid of cells (or tiles), each in one of a finite number of states, such as 0 and 1. The grid must be in any finite number of dimensions.
Among the different types of cellular automata types, such as iterative arrays, tessellation automata, for this work, I have used the Moore neighbourhood cellular automata approach (See other references).
The concept of randomness was a key part for understanding how procedural content generation works, once in its core, it is implicit that from a few parameters, a large number of possible types of content can be generated (see other references for more).
The first step of this work was to produce a grid of tiles and be able to divide the tiles in two different types, called in the comments of the scripts by USED tile and EMPTY tile (starting the cellular automata algorithm). After the creation of those different tiles, display them was the main objective.
  
Then was implemented a seed system based on unity tutorial for procedural content generation to create different levels. Also was introduced a user interaction. During game play, every time the user click with his left mouse on the game window, if a seed is given and the random seed option is ticked, a new level will be generated.
  
The second step was to turn the entire edge of the grid as a USED tile, or in code, 1.
  
A refinement algorithm was introduced next, where actually the grid start to looks like a level. In fact, the inspiration for that was the Diablo3 level generation.
 
The next step was to actually convert the grid into NODE, where I could manipulate values such position in the world and relatively to each other. This effect can be observed if uncomment the function OnDrawGizmos() at the GeneratorOfMesh script.
 
The creation of a mesh out of the USED tiles (blue area in the picture above) was created. Once the mesh was created, a function to check where the outlines of the mesh where, and with that information, the possibility to extrude walls from the outline mesh.
To create 3D wall out of the mesh, we need to look at every couple of vertices (an edge) that belongs to just one triangle in the mesh. If that edge is shared by 2 triangles at the same time, than that edge is not one of the outside edge, and therefore, will not be extrude to create a 3D mesh (the wall). Instead, if the edge is not share by more than 1 triangle, is safe to say that we can extrude that edge to create a 3D mesh out of it. By definition of outline edge is when two vertices which share exactly one triangle.
To conclude, the most complicated part of the coursework actually was not the cellular automata process, but to create meshed out of a grid and then 3D meshes. It seems to be very computing intensive to generate the level, but once it is generated, it is extremely useful and light comparing to other methods of creation of content.


------------------------------------------------------------------------------------------------------------------------------------------
REFERENCES
The main reference for this work was the unity tutorial procedural caves generation (especially about the triangulation of squares and the programming logic behind the meshes, where part of the mesh code was copied from).
http://unity3d.com/learn/tutorials/modules/advanced/scripting/procedural-cave-generation-pt4?playlist=17153
Other references
https://en.wikipedia.org/wiki/Cellular_automaton
https://en.wikipedia.org/wiki/Moore_neighborhood
https://en.wikipedia.org/wiki/File:Moore_neighborhood_with_cardinal_directions.svg

http://pcg.wikidot.com/pcg-algorithm:map-generation
http://pcg.wikidot.com/pcg-algorithm:cellular-automata
http://pcg.wikidot.com/pcg-algorithm:dungeon-generation
http://pcg.wikidot.com/category-pcg-algorithms

