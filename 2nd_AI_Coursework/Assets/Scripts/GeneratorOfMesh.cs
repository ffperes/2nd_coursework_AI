using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorOfMesh : MonoBehaviour {

	public MeshFilter walls;
	public SquareGrid squareGrid;
	List<Vector3> vertices;
	List<int> triangles;
	// It will be passed a key, as an integer that will be the vertex index at the struct Triangle
	// the value thar will be returned by that key will be a list of all triangles that vertex
	// is part of
	Dictionary<int, List<Triangle>> triDic = new Dictionary<int, List<Triangle>>();
	List<List<int>> outline = new List<List<int>> ();
	HashSet<int> verticesAlreadyChecked = new HashSet<int> ();

	public void GenerateMesh(int[,] map, float squareSize){
		outline.Clear (); // to reset this variable everytime a new mesh is generated - when user clicks the mouse
		verticesAlreadyChecked.Clear (); // to reset this variable everytime a new mesh is generated - when user clicks the mouse
		triDic.Clear(); // to reset this variable everytime a new mesh is generated - when user clicks the mouse

		squareGrid = new SquareGrid (map, squareSize);


		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		for (int x = 0; x < squareGrid.squares.GetLength (0); x++) {
			for (int z = 0; z < squareGrid.squares.GetLength (1); z++) {
				BreakingSquareInTriangules (squareGrid.squares [x, z]);
			}
		}

		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.RecalculateNormals ();

		ExtrudeMeshForWalls ();
	}

	void ExtrudeMeshForWalls(){
		MeshOutline ();

		List<int> triWalls = new List<int> ();
		Mesh meshWall = new Mesh ();
		float heightOfWall = 8;
		List<Vector3> verticeOfWall = new List<Vector3> ();

		foreach (List<int> outlines in outline) {
			for (int i = 0; i < outlines.Count - 1; i++) {
				int startVertex = verticeOfWall.Count;
				verticeOfWall.Add(vertices[outlines[i]]);
				verticeOfWall.Add(vertices[outlines[i+1]]);
				verticeOfWall.Add(vertices[outlines[i]] - Vector3.up * heightOfWall);
				verticeOfWall.Add(vertices[outlines[i+1]] - Vector3.up * heightOfWall);

				triWalls.Add (startVertex + 0);
				triWalls.Add (startVertex + 2);
				triWalls.Add (startVertex + 3);

				triWalls.Add (startVertex + 3);
				triWalls.Add (startVertex + 1);
				triWalls.Add (startVertex + 0);
			}
		}
		meshWall.vertices = verticeOfWall.ToArray ();
		meshWall.triangles = triWalls.ToArray ();
		walls.mesh = meshWall;

	}

	// Those vertex will hold the vertex index of a triangle
	struct Triangle {
		public int vertexOne, vertexTwo, vertexThree;
		int[] v; // for vertices

		public Triangle( int _vertexOne, int _vertexTwo, int _vertexThree){
			vertexOne = _vertexOne;
			vertexTwo = _vertexTwo;
			vertexThree = _vertexThree;

			v = new int[3];
			v[0] = _vertexOne;
			v[1] = _vertexTwo;
			v[2] = _vertexThree;
		}

		public int this[int i]{
			get{ 
				return v [i];
			}
		}

		public bool Contains(int i){
			return i == vertexOne || i == vertexTwo || i == vertexThree;
		}
	}

	// The system to do that is based on the 16 different possibilities of combinations
	// that a square (4 corners) and 4 node between the corners can have
	void BreakingSquareInTriangules(SquareSetup _square){
		// 
		switch (_square.config) {
		case 0:
			break; // in this case there is no mesh
		
		// 1 pseudo-node
		case 1: 
			CreateMeshFromNodes (_square.west, _square.south, _square.southwestCorner);
			break;
		case 2:
			CreateMeshFromNodes (_square.southeastCorner, _square.south, _square.east);
			break;
		case 4: 
			CreateMeshFromNodes (_square.northeastCorner, _square.east, _square.north);
			break;
		case 8: 
			CreateMeshFromNodes (_square.northwestCorner, _square.north, _square.west);
			break;
		
		// 2 pseudo=nodes
		case 3: 
			CreateMeshFromNodes (_square.east, _square.southeastCorner, _square.southwestCorner, _square.west);
			break;
		case 6:
			CreateMeshFromNodes (_square.north, _square.northeastCorner, _square.southeastCorner, _square.south);
			break;
		case 9: 
			CreateMeshFromNodes (_square.northwestCorner, _square.north, _square.south, _square.southwestCorner);
			break;
		case 12: 
			CreateMeshFromNodes (_square.northwestCorner, _square.northeastCorner, _square.east, _square.west);
			break;
		case 5: // diagonal
			CreateMeshFromNodes (_square.north, _square.northeastCorner, _square.east, _square.south, _square.southwestCorner, _square.west);
			break;
		case 10: // diagonal
			CreateMeshFromNodes (_square.northwestCorner, _square.north, _square.east, _square.southeastCorner, _square.south, _square.west);
			break;
		
		// 3 pseudo=node
		case 7:
			CreateMeshFromNodes (_square.north, _square.northeastCorner, _square.southeastCorner, _square.southwestCorner, _square.west);
			break;
		case 11:
			CreateMeshFromNodes (_square.northwestCorner, _square.north, _square.east, _square.southeastCorner, _square.southwestCorner);
			break;
		case 13:
			CreateMeshFromNodes (_square.northwestCorner, _square.northeastCorner, _square.east, _square.south, _square.southwestCorner);
			break;
		case 14:
			CreateMeshFromNodes (_square.northwestCorner, _square.northeastCorner, _square.southeastCorner, _square.south, _square.west);
			break;

		// 4 pseudo-nodes
		case 15:
			CreateMeshFromNodes (_square.northwestCorner, _square.northeastCorner, _square.southeastCorner, _square.southwestCorner);
			verticesAlreadyChecked.Add (_square.northwestCorner.vertexIndex);
			verticesAlreadyChecked.Add (_square.northeastCorner.vertexIndex);
			verticesAlreadyChecked.Add (_square.southeastCorner.vertexIndex);
			verticesAlreadyChecked.Add (_square.southwestCorner.vertexIndex);
			break;
		}
	}

	// The PARAMS keyword allow to specify a method parameter that takes an argument where the number of
	// arguments is variable
	// it will take the nodes and corner of the square, triangulating the points and then create triangule meshes
	// out of then
	void CreateMeshFromNodes(params Node[] _points){
		AssignVertices (_points);
		if (_points.Length >= 3) {
			CreateTriangles (_points [0], _points [1], _points [2]);
		}
		if (_points.Length >= 4) {
			CreateTriangles (_points [0], _points [2], _points [3]);
		}
		if (_points.Length >= 5) {
			CreateTriangles (_points [0], _points [3], _points [4]);
		}
		if (_points.Length >= 6) {
			CreateTriangles (_points [0], _points [4], _points [5]);
		}
	}

	void AssignVertices(Node[] _points){
		for (int i = 0; i < _points.Length; i++) {
			if (_points [i].vertexIndex == -1) {
				_points [i].vertexIndex = vertices.Count; // will get the number of elements contained in the list
				vertices.Add(_points[i].position); // will add the vertice to the array of nodes
			}
		}
	}

	// This method add 3 etices to form a triangle
	void CreateTriangles(Node one, Node two, Node three){
		triangles.Add (one.vertexIndex);
		triangles.Add (two.vertexIndex);
		triangles.Add (three.vertexIndex);

		Triangle tri = new Triangle (one.vertexIndex, two.vertexIndex, three.vertexIndex);
		AddTriDictionary (tri.vertexOne, tri);
		AddTriDictionary (tri.vertexTwo, tri);
		AddTriDictionary(tri.vertexThree, tri);
	}

	// This method will add the triangle create in the method above and store it at the list in
	// the dictionary
	// if the dictionary already contained that triangle key, it will simply add the current triangle 
	// to the list, otherwise, it will create a new list of triangles
	void AddTriDictionary(int vertexKey, Triangle tri){
		if (triDic.ContainsKey (vertexKey)) {
			triDic [vertexKey].Add (tri);
		} else {
			List<Triangle> triList = new List<Triangle> ();
			triList.Add (tri);
			triDic.Add (vertexKey, triList);
		}
	}

	// It will go through every single vertex in the level, it will check if it is an outline vertex
	// and if IS, it will follow that outline, all the way around, until it finds itself again and will add
	// that to the outline list
	void MeshOutline(){
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!verticesAlreadyChecked.Contains (vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex (vertexIndex);
				if (newOutlineVertex != -1) {
					verticesAlreadyChecked.Add (vertexIndex);
					List<int> newOutline = new List<int> ();
					outline.Add (newOutline);
					FollowOutline (newOutlineVertex, outline.Count - 1);
					outline [outline.Count - 1].Add (vertexIndex);
				}
			}
		}
	}

	void FollowOutline (int _vertexIndex, int _outlineIndex){
		outline [_outlineIndex].Add (_vertexIndex);
		verticesAlreadyChecked.Add (_vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex (_vertexIndex);
		if (nextVertexIndex != -1) {
			FollowOutline (nextVertexIndex, _outlineIndex);
		}
	}

	// This method will take 2 vertices and check if they belong to more than one triangle
	// If yes, they are NOT an outline edge, if they belong to just ONE triangule, then they
	// are an outline edge by definition
	bool OutlineEdge (int vertexOne, int vertexTwo){
		int counter = 0; // to track the number of triangles shared by the current vertex
		List<Triangle> trianglesThatContainsVertexOne = triDic [vertexOne];

		for (int i = 0; i < trianglesThatContainsVertexOne.Count; i++) {// count thats the number of values in the list
			if(trianglesThatContainsVertexOne[i].Contains(vertexTwo)){
				counter++;
				if(counter > 1){
					break;
				}
			}
		}
		return counter == 1;
	}

	// To get a list of all triangles that contain the vertexOne parameter
	int GetConnectedOutlineVertex(int _vertexOne){
		List<Triangle> triThatContainsVertexOne = triDic [_vertexOne];

		for (int i = 0; i < triThatContainsVertexOne.Count; i++) {
			Triangle tri = triThatContainsVertexOne [i];

			for (int j = 0; j < 3; j++) {
				int vertex = tri [j];
				if (vertex != _vertexOne && !verticesAlreadyChecked.Contains(vertex)) {
					if (OutlineEdge (_vertexOne, vertex)) {
						return vertex;
					}
				}
			}
		}
		return -1;
	}
		
	// The only objective of this class is to store position values
	public class Node{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node (Vector3 _position){
			position = _position;
		}
	}

	// The mathmatecis of this class was obtained at
	// http://unity3d.com/learn/tutorials/modules/advanced/scripting/procedural-cave-generation-pt2?playlist=17153
	// Creates an 2 dimensional array of squares (x and z axix in unity3D)
	// It takes each pseudo-node in the level, including its position and check which ones are used or empty
	// then in the final nested FOR loop creates squares and determine how they are connected with each other,
	// if they are connected at all
	public class SquareGrid{
		public SquareSetup[, ] squares;

		public SquareGrid(int[,] level, float squareSize){
			int nodeCountX = level.GetLength(0);
			int nodeCountZ = level.GetLength(1);
			float levelWidth = nodeCountX * squareSize;
			float levelHeight = nodeCountZ * squareSize;

			NodeController[,] nodeController = new NodeController[nodeCountX, nodeCountZ];
			for(int x = 0; x < nodeCountX; x++){
				for (int z = 0; z < nodeCountZ; z++){
					Vector3 position = new Vector3(-levelWidth/2 + x * squareSize + squareSize/2, 0, -levelHeight/2 + z * squareSize + squareSize/2);
					nodeController[x, z] = new NodeController(position, level[x, z] == 1, squareSize);
				}
			}

			squares = new SquareSetup[nodeCountX - 1, nodeCountZ -1];
			for(int x = 0; x < nodeCountX - 1; x++){
				for (int z = 0; z < nodeCountZ - 1; z++){
					squares[x, z] = new SquareSetup(nodeController[x, z + 1],
													nodeController[x + 1, z + 1],
													nodeController[x + 1, z],
													nodeController[x, z]);
												
				}
			}
		}
	}

	// This class setup how a individual squares in the script will behave
	// All 4 corners of the square (northwestCorner, northeastCorner, southeastCorner, southwestCorner)
	// will act as a pseudo-node and they are aware of the pseudo-node at its right and above it
	// in terms of grid position
	// the  nodes north, east, south, west are located in between each pseudo-node and are crucial to
	// break the square into triangules in order to creat meches
	public class SquareSetup{
		public NodeController northwestCorner, northeastCorner, southeastCorner, southwestCorner;
		public Node  north, east, south, west;
		public int config;

		public SquareSetup (NodeController _northwestCorner, NodeController _northeastCorner, 
							NodeController _southeastCorner, NodeController _southwestCorner){
			northwestCorner = _northwestCorner;
			northeastCorner = _northeastCorner;
			southeastCorner = _southeastCorner;
			southwestCorner = _southwestCorner;

			north = northwestCorner.nodeRight;
			east = southeastCorner.nodeAbove;
			south = southwestCorner.nodeRight;
			west = southwestCorner.nodeAbove;

			//
			if(northwestCorner.nodeActive){
				config += 8;
			}
			if(northeastCorner.nodeActive){
				config += 4;
			}
			if(southeastCorner.nodeActive){
				config += 2;
			}
			if(southwestCorner.nodeActive){
				config += 1;
			}
		}
	}

	// This class will inherit position from the base class
	public class NodeController : Node{
		// If is active, then is a used tile, if not, is an empty tile
		public bool nodeActive;
		public Node nodeAbove, nodeRight;

		// The position will be set from the base constructor or Node class constructor
		public NodeController(Vector3 _pos, bool _nodeActive, float _sizeOfTheSquare) : base (_pos){
			nodeActive = _nodeActive;
			nodeAbove = new Node(position + Vector3.forward * _sizeOfTheSquare/2f);
			nodeRight = new Node(position + Vector3.right * _sizeOfTheSquare/2f);
		} 
	}

	// for debug and visualization
	// It will draw the grid of the level with squares and show how they are connected
	// if they are connected at all
	// The system to do that is based on the 16 different possibilities of combinations
	// that a square (4 corners) and 4 node between the corners can have
	void OnDrawGizmos(){
		/*
		if (squareGrid != null) {
			for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
				for (int z = 0; z < squareGrid.squares.GetLength(1); z++) {
					Gizmos.color = (squareGrid.squares [x, z].northwestCorner.nodeActive) ? Color.blue : Color.yellow;
					Gizmos.DrawCube (squareGrid.squares [x, z].northwestCorner.position, Vector3.one * .4f);

					Gizmos.color = (squareGrid.squares [x, z].northeastCorner.nodeActive) ? Color.blue : Color.yellow;
					Gizmos.DrawCube (squareGrid.squares [x, z].northeastCorner.position, Vector3.one * .4f);

					Gizmos.color = (squareGrid.squares [x, z].southeastCorner.nodeActive) ? Color.blue : Color.yellow;
					Gizmos.DrawCube (squareGrid.squares [x, z].southeastCorner.position, Vector3.one * .4f);

					Gizmos.color = (squareGrid.squares [x, z].southwestCorner.nodeActive) ? Color.blue : Color.yellow;
					Gizmos.DrawCube (squareGrid.squares [x, z].southwestCorner.position, Vector3.one * .4f);

					Gizmos.color = Color.grey;
					Gizmos.DrawCube (squareGrid.squares [x, z].north.position, Vector3.one * .15f);
					Gizmos.DrawCube (squareGrid.squares [x, z].east.position, Vector3.one * .15f);
					Gizmos.DrawCube (squareGrid.squares [x, z].south.position, Vector3.one * .15f);
					Gizmos.DrawCube (squareGrid.squares [x, z].west.position, Vector3.one * .15f);
				}
			}
		}
		*/
	}
}
