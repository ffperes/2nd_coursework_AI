In this coursework, I will be using procedural content generation AI technique 
to create a dungeon/cave level using cellular automata and tiles.

The first part of the algorithm randomly generates a basic shape of the level. After
that refines it to a proper level representation. After that, creates meshes and 
other visual content.



To create 3D wall out of the mesh, we need to look at every couple of vertices
(an edge) that belogns to just one triangule in the mesh. If that edge is 
shared by 2 triangules at the same time, than that edge is not one of the outside
edge, and therefore, will not be extrude to create a 3D mesh (the wall). Instead,
if the edge is not share by more than 1 triangule, is safe to say that we can
extrude that edge to crate a 3D mesh out of it.