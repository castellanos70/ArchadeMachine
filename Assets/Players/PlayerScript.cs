﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{

    public int playerNumber;
    public TextMesh levelMovesTextMesh;
    public int speedMax;
    public int speedMin;
    public int acceleration;

    private int movesRemaining;
    private bool moving = false;
    private float speedX = 0;
    private float speedZ = 0;
    //private int boardWidth, boardHeight;
	//private int prevX = 0;
	//private int prevZ = 0;

    private Cell[,] grid;
    private CameraScript.Element myPlayer;
    private CameraScript cameraScript;

    private KeyCode[] keycode = new KeyCode[4];
    private HashSet<GameObject> entities;


    public void setMaxLevelMoves(int moveCount)
    {
        movesRemaining = moveCount;
    }

    public void setBoard(CameraScript cameraScript, Cell[,] myGrid)
    {
        this.cameraScript = cameraScript;
        grid = myGrid;
        //boardWidth = grid.GetLength(0);
        //boardHeight = grid.GetLength(1);
    }

	public void setPosition(int x, int z){
		transform.Translate(new Vector3(x, 1, z));
		//prevX = x;
		//prevZ = z;
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
			myPlayer = CameraScript.Element.PLAYER1;
        }
        else
        {
            keycode[0] = KeyCode.UpArrow;
            keycode[1] = KeyCode.RightArrow;
            keycode[2] = KeyCode.DownArrow;
            keycode[3] = KeyCode.LeftArrow;
			myPlayer = CameraScript.Element.PLAYER2;
        }
    }
		
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        int x0; // current position (rounded down)
        int z0;

        x0 = (int)(transform.position.x);
        z0 = (int)(transform.position.z);

        if (moving)
        {
            float x = transform.position.x + speedX * Time.deltaTime;
            float z = transform.position.z + speedZ * Time.deltaTime;

            bool hit = checkMove(x0, z0);

            // These extra conditions stop the players from jerking against the wall
            if (hit && ((speedX >= 0 || Math.Abs(x - x0) < 0.2) && (speedZ >= 0 || Math.Abs(z - z0) < 0.2)))
            {
                x = x0; z = z0;
                speedX = 0;
                speedZ = 0;
                moving = false;
            }

            else if (Input.GetKey(keycode[0]) && speedX == 0)
            {
                speedZ += acceleration * Time.deltaTime;
				speedZ = toSpeedBounds(speedZ);
            }

            else if (Input.GetKey(keycode[2]) && speedX == 0)
            {
                speedZ -= acceleration * Time.deltaTime;
				speedZ = toSpeedBounds(speedZ);
            }

            else if (Input.GetKey(keycode[1]) && speedZ == 0)
            {
                speedX += acceleration * Time.deltaTime;
				speedX = toSpeedBounds(speedX);
            }

            else if (Input.GetKey(keycode[3]) && speedZ == 0)
            {
                speedX -= acceleration * Time.deltaTime;
				speedX = toSpeedBounds(speedX);
            }

            if (grid[(int)x, (int)z].getEnvironment() == CameraScript.Element.GOAL)
            {
                cameraScript.setGameState(CameraScript.GameState.WON);
            }

            transform.position = new Vector3(x, 1, z);
        }

        else
        {
            if (movesRemaining > 0)
            {
                int x1 = x0;
                int z1 = z0;
                if (Input.GetKey(keycode[0]))
                {
                    speedZ = acceleration * Time.deltaTime;
                    speedX = 0;
                    z1 += 1;
                    moving = true;
                }
                else if (Input.GetKey(keycode[2]))
                {
                    speedZ = -acceleration * Time.deltaTime;
                    speedX = 0;
                    z1 -= 1;
                    moving = true;
                }
                else if (Input.GetKey(keycode[1]))
                {
                    speedZ = 0;
                    speedX = acceleration * Time.deltaTime;
                    x1 += 1;
                    moving = true;
                }
                else if (Input.GetKey(keycode[3]))
                {
                    speedZ = 0;
                    speedX = -acceleration * Time.deltaTime;
                    x1 -= 1;
                    moving = true;
                }


                if (moving)
                {
                    
                    bool hit = checkSpot(x1, z1);
                    if (hit)
                    {
                        speedX = 0;
                        speedZ = 0;
                        moving = false;
                    }
                    else
                    {
                        movesRemaining--;
                        levelMovesTextMesh.text = movesRemaining.ToString();
                    }
                }
                //else grid[prevX, prevZ].setEntity(myPlayer, true);
            }
        }
    }

    public void setEntities(HashSet<GameObject> entities)
    {
        this.entities = entities;
    }

   public int getDirectionX()
    {
        return floatToUnit(speedX);
    }

    public int getDirectionZ()
    {
        return floatToUnit(speedZ);
    }

    private bool checkMove(int x0, int z0)
    {
        int x1 = x0;
        int z1 = z0;

        /*if (speedX > 0) x1 = (int)(x + 1);
        else if (speedX < 0) x1 = (int)(x);
        else if (speedZ > 0) z1 = (int)(z + 1);
        else if (speedZ < 0) z1 = (int)(z);*/

        if (speedX > 0) x1 = x0 + 1;
        else if (speedX < 0) x1 = x0 - 1;
        else if (speedZ > 0) z1 = z0 + 1;
        else if (speedZ < 0) z1 = z0 - 1;

        //Debug.Log(Time.time + ": [" + x0 + "," + z0 + "]===> "+ " grid[" +x1+","+z1+"]="+grid[x1,z1]);

        /*if (grid[x1, z1].getEnvironment() == CameraScript.Element.FLOOR) return false;
		if (grid[x1, z1].getEnvironment() == CameraScript.Element.GOAL) return false;
        if (grid[x1, z1].getEntity() == myPlayer) return false;*/

        if (grid[x1, z1].getEntity() == CameraScript.Element.CRATE)
        {
            if (speedX > 15 || speedX < -15 || speedZ > 15 || speedZ < -15)
            {
                return false;
            }
            else return true;
        }

        else if ((grid[x1, z1].getEntity() == CameraScript.Element.NOTHING
            || grid[x1, z1].getEntity() == myPlayer)
            && grid[x1, z1].getEnvironment() != CameraScript.Element.WALL) return false;

        return true;
    }
		
    private bool checkSpot(int x1, int z1)
    {
        /*if (grid[x1, z1].getEnvironment() == CameraScript.Element.FLOOR) return false;
        if (grid[x1, z1].getEnvironment() == CameraScript.Element.GOAL) return false;
        if (grid[x1, z1].getEntity() == myPlayer) return false;*/

        if (grid[x1, z1].getEntity() == CameraScript.Element.CRATE)
        {
            if (speedX > 15 || speedX < -15 || speedZ > 15 || speedZ < -15)
            {
                return false;
            }
            else return true;
        }

        else if ((grid[x1, z1].getEntity() == CameraScript.Element.NOTHING
            || grid[x1, z1].getEntity() == myPlayer)
            && grid[x1, z1].getEnvironment() != CameraScript.Element.WALL) return false;

        return true;
    }

	private float toSpeedBounds(float n){
		if (n > speedMax) n = speedMax;
		else if (n < -speedMax) n = -speedMax;
		else if (n > 0 && n < speedMin) n = speedMin;
		else if (n < 0 && n > -speedMin) n = -speedMin;

		return n;
	}

    private int intToUnit(int n)
    {
        if (n < 0) return -1;
        else if (n > 0) return 1;
        return 0;
    }

    private int floatToUnit(float n)
    {
        if (n < 0) return -1;
        else if (n > 0) return 1;
        return 0;
    }
}
