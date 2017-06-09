﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int playerNumber;
    public TextMesh levelMovesTextMesh;

    private int movesRemaining;
    private bool moving = false;
    private float speedX = 0;
    private float speedZ = 0;
    private int boardWidth, boardHeight;

    private int[,] grid;
    private int myPlayer;

    private KeyCode[] keycode = new KeyCode[4];


    public void setMaxLevelMoves(int moveCount)
    {
        movesRemaining = moveCount;
    }

    public void setBoard(int[,] myGrid)
    {
        grid = myGrid;
        boardWidth = grid.GetLength(0);
        boardHeight = grid.GetLength(1);
    }



    void Start ()
    {
        levelMovesTextMesh.text = movesRemaining.ToString();

        if (playerNumber == 1)
        {
            keycode[0] = KeyCode.W;
            keycode[1] = KeyCode.D;
            keycode[2] = KeyCode.S;
            keycode[3] = KeyCode.A;
			myPlayer = (int)CameraScript.Element.PLAYER1;
        }
        else
        {
            keycode[0] = KeyCode.UpArrow;
            keycode[1] = KeyCode.RightArrow;
            keycode[2] = KeyCode.DownArrow;
            keycode[3] = KeyCode.LeftArrow;
			myPlayer = (int)CameraScript.Element.PLAYER2;
        }
    }
		
	
	// Update is called once per frame
	void Update ()
    {
        if (moving)
        {
            int x0 = (int)transform.position.x;
            int z0 = (int)transform.position.z;
			grid[x0,z0] = (int)CameraScript.Element.FLOOR;

            float x = transform.position.x + speedX * Time.deltaTime;
            float z = transform.position.z + speedZ * Time.deltaTime;

            int x1 = (int)(x - 0.5f);
            int z1 = (int)(z - 0.5f);
            int x2 = (int)(x + 0.5f);
            int z2 = (int)(z + 0.5f);

            bool hit = false;
            if (speedX > 0)
            {
				if (grid[x2, z0] != (int)CameraScript.Element.FLOOR) hit = true;
            }
            else if (speedX < 0)
            {
				if (grid[x1, z0] != (int)CameraScript.Element.FLOOR) hit = true;
            }
            else if (speedZ > 0)
            {
				if (grid[x0, z2] != (int)CameraScript.Element.FLOOR) hit = true;
            }
            else if (speedZ < 0)
            {
				if (grid[x0, z1] != (int)CameraScript.Element.FLOOR) hit = true;
            }

            if (hit)
            {
                x = x0; z = z0;
                moving = false;
            }

            transform.position = new Vector3(x, 1, z);

            x0 = (int)transform.position.x;
            z0 = (int)transform.position.z;
			grid[x0, z0] = (int)CameraScript.Element.PLAYER2;
        }
        else
        {
            if (movesRemaining > 0)
            {
                if (Input.GetKey(keycode[0]))
                {
                    speedZ = 10;
                    speedX = 0;
                    moving = true;
                }
                else if (Input.GetKey(keycode[2]))
                {
                    speedZ = -10;
                    speedX = 0;
                    moving = true;
                }
                else if (Input.GetKey(keycode[1]))
                {
                    speedZ = 0;
                    speedX = 10;
                    moving = true;
                }
                else if (Input.GetKey(keycode[3]))
                {
                    speedZ = 0;
                    speedX = -10;
                    moving = true;
                }


                if (moving)
                {
                    movesRemaining--;
                    levelMovesTextMesh.text = movesRemaining.ToString();
                }
            }
        }
    }
}
