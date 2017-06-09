﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject boardBlock;
    public GameObject player1, player2;
    public Material[] wallMat = new Material[10];
    public Material goalMat;

    private const int boardWidth = 24;
    private const int boardHeight = 13;


    public enum GameState { INTRO, PLAYING, LOST, WON };
    private GameState gameState;

    public enum Element { FLOOR, WALL, GOAL, PLAYER1, PLAYER2, PORTAL1, PORTAL2, PORTAL3 };
    private static Element[] elementValues;
    private GameMap[] gameMaps;
    private GameMap gameMap;
    private CameraScript.Element[,] grid;

    private PlayerScript playerScript1, playerScript2;
    private GameObject goalBlock;

    void Awake()
    {
        playerScript1 = player1.GetComponent<PlayerScript>();
        playerScript2 = player2.GetComponent<PlayerScript>();

        elementValues = (Element[])System.Enum.GetValues(typeof(Element));

        gameMaps = MapLoader.loadAllMaps ();
		gameMap = gameMaps [Random.Range (0, gameMaps.Length)];
        grid = gameMap.grid;


        playerScript1.setMaxLevelMoves(gameMap.moves);
		playerScript2.setMaxLevelMoves(gameMap.moves);

		playerScript1.setBoard(this, grid);
		playerScript2.setBoard(this, grid);

        // Spawn board blocks
		for (int x = 0; x < gameMap.width; x++)
        {
			for (int z = 0; z < gameMap.height; z++)
            {
                GameObject block = Instantiate(boardBlock, new Vector3(x, 0, z), Quaternion.identity);
				if (grid[x, z] == Element.WALL)
                {
                    Renderer renderer = block.GetComponent<Renderer>();
                    renderer.material = wallMat[Random.Range(0, wallMat.Length)];
                    block.transform.Translate(Vector3.up);
                }
				else if (grid[x, z] == Element.PLAYER1)
                {
                    player1.transform.Translate(new Vector3(x, 1, z));
                }
				else if (grid[x, z] == Element.PLAYER2)
                {
                    player2.transform.Translate(new Vector3(x, 1, z));
                }
				else if (grid[x, z] == Element.GOAL)
                {
                    Renderer renderer = block.GetComponent<Renderer>();
                    renderer.material = goalMat;
                    block.transform.Translate(new Vector3(0, 0.01f, 0));
                    goalBlock = block;
                }
            }
        }

        gameState = GameState.PLAYING;
    }

    // Update is called once per frame
    void Update()
    {
        goalBlock.transform.Rotate(Vector3.up * Time.deltaTime*20);
    }


    public void setGameState(GameState state)
    {
        gameState = state;
        if (gameState == GameState.WON)
        {
            Time.timeScale = 0;
        }

    }

    public static Element getElement(int idx)
    {
        return elementValues[idx];
    }
}
