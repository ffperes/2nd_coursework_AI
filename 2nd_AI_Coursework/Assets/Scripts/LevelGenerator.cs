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

	void GenerateLevel(){
		// Set the size of the level
		level = new int[width, height];
		RandomGeneratesLevel ();
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

	// This fuction will draw cubes at the scene to check if the above algorithm is working
	// iif the block is empty, it will fulfill its color with yellow
	// if the bloc is being used, its colour will be blue.
	void OnDrawGizmos(){
		if (level != null) {
			for (int x = 0; x < width; x++){
				for (int z = 0; z < height; z++){
					Gizmos.color = (level [x, z] == 1)? Color.blue : Color.yellow;
					Vector3 position = new Vector3 (-width / 2 + x + 0.5f, 0, -height / 2 + z + 0.5f); 
					Gizmos.DrawCube (position, Vector3.one);
				}
			}
		}
	}

}
