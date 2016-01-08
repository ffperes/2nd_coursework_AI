using UnityEngine;
using System;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	// The tiles will be stored as 0 and 1
	int [,] level;

	// To determined the percentage of each type of terrain
	// The range of this number will be between 0 and 100 percent
	[Range(0, 100)]
	public int spaceDistributionOfTerrain;

	// To specified the width and height of the level
	// height here is not Y axis, but Z axis instead
	public int width;
	public int height;
	public int levelOfRefinement = 5;

	// The first option will hold a number based on a hash code of a string,
	// and therefore allow me to create the same level if and only if the same
	// seed of hash code is given.
	// The second variable is used to check if the algorithm needs to generates
	// a random seed or if a seed is being given
	public string seed;
	public bool randomSeed;

	void Start(){
		GenerateLevel ();
	}

	void Update(){
		//////////////////////////////////////
		// WARNING FABIO                    //
		// Click the mouse in the game      //
		// window when the scene is running //
		// not at the scene view noob       //
		//////////////////////////////////////
		// To generate a new seed level each time the left buttom mouse is clicked
		if (Input.GetMouseButtonDown (0)) {
			GenerateLevel ();
		}
	}

	void GenerateLevel(){
		// Set the size of the level
		level = new int[width, height];
		RandomGeneratesLevel ();

		for (int i = 0; i < levelOfRefinement; i++) {
			RefineLevel ();
		}

		// Creates a border to my level
		// x2 for both sides
		int border = 1;
		int [,] borderOfTheLevel = new int[width + border * 2, height + border * 2];
		for (int x = 0; x < borderOfTheLevel.GetLength(0); x++) {
			for (int z = 0; z < borderOfTheLevel.GetLength(1); z++) {
				if (x >= border && x < width + border && z >= border && z < height + border) {
					borderOfTheLevel [x, z] = level [x - border, z - border];				
				} else {
					borderOfTheLevel [x, z] = 1;
				}
			}
		}

		// To get the mesh component from the object that the script is attateched to
		GeneratorOfMesh meshGen = GetComponent<GeneratorOfMesh>();
		meshGen.GenerateMesh(borderOfTheLevel, 1);
	}

	void RandomGeneratesLevel(){
		// if seed os true, it will take the time at the beginning of the frame and
		// return this instance of string
		if (randomSeed == true) {
			seed = Time.time.ToString ();
		}

		// It will generate a hash number based on the seed string
		System.Random randomSeedGenerator = new System.Random (seed.GetHashCode ());

		// This ternary decision if statement will populate the array with 0 and 1
		// 0 for an empty block
		// 1 for a used block
		for (int x = 0; x < width; x++){
			for (int z = 0; z < height; z++) {
				// Set the edges if the grid as used block
				if (x == 0 || x == width - 1 || z == 0 || z == height - 1) {
					level [x, z] = 1;
				} else {
					level [x, z] = (randomSeedGenerator.Next (0, 100) < spaceDistributionOfTerrain) ? 1 : 0;
				}
			}	
		}
	}

	//
	void RefineLevel(){
		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {
				int neighbourUsedTiles = GetUsedTilesAround (x, z);
				if (neighbourUsedTiles > 4) {
					level [x, z] = 1;
				} else if (neighbourUsedTiles < 4){
					level [x, z] = 0;
				}
			}
		}
	}

	// Takes the current tile to be checked as arguments and
	// Looping a 3x3 grid looking for all adjacents tiles of the current tile being analised
	int GetUsedTilesAround(int _x, int _z){
		int counter = 0;
		for (int tileAround_X = _x - 1; tileAround_X <= _x + 1; tileAround_X++) {
			for (int tileAround_Z = _z - 1; tileAround_Z <= _z + 1; tileAround_Z++) {
				// To constrain the analise to inside of the level itself, in case of an edge
				// tile been check
				if (tileAround_X >= 0 && tileAround_X < width && tileAround_Z >= 0 && tileAround_Z < height) {
					// Condition to skip the current tile
					if (tileAround_X != _x || tileAround_Z != _z) {
						// The counter will increase if the tile being checked is a used tile
						counter += level [tileAround_X, tileAround_Z];
					}
				} else {
					counter++;
				}
			}
		}
		return counter;
	}

	// This fuction will draw cubes at the scene to check if the above algorithm is working
	// iif the block is empty, it will fulfill its color with yellow
	// if the bloc is being used, its colour will be blue.
	// Also debug and visualization purposes
	void OnDrawGizmos(){
	/*
		if (level != null) {
			for (int x = 0; x < width; x++){
				for (int z = 0; z < height; z++){
					Gizmos.color = (level [x, z] == 1)? Color.blue : Color.yellow;
					Vector3 position = new Vector3 (-width / 2 + x + 0.5f, 0, -height / 2 + z + 0.5f); 
					Gizmos.DrawCube (position, Vector3.one);
				}
			}
		}
		*/
	}

}
