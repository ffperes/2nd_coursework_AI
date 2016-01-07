using UnityEngine;
using System.Collections;

public class GeneratorOfMesh : MonoBehaviour {

	public SquareGrid squareGrid;

	public void GenerateMesh(int[,] map, float squareSize){
		squareGrid = new SquareGrid (map, squareSize);
	}

	// for debug and visualization
	void OnDrawGizmos(){
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
	}

	public class Node{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node (Vector3 _position){
			position = _position;
		}
	}

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

	public class SquareSetup{
		public NodeController northwestCorner, northeastCorner, southeastCorner, southwestCorner;
		public Node  north, east, south, west;
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

		}
	}

	public class NodeController : Node{
		// If is active is a used tile, if not, is an empty tile
		public bool nodeActive;
		public Node nodeAbove, nodeRight;

		// The position will be set from the base constructor or Node class constructor
		public NodeController(Vector3 _pos, bool _nodeActive, float _sizeOfTheSquare) : base (_pos){
			nodeActive = _nodeActive;
			nodeAbove = new Node(position + Vector3.forward * _sizeOfTheSquare/2f);
			nodeRight = new Node(position + Vector3.right * _sizeOfTheSquare/2f);
		} 
	}
}
