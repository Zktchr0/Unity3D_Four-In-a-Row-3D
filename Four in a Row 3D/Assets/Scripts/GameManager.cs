using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject piece;        // The game pieces prefab
    public static int size = 4;     // Size of the (cubic) board
    public float dropDelay = 0.25f; // So that pieces won't tumble into each other
    public float moveDelay = 0.3f;  // So that the marker will move one pole at a time - REMOVE

    public Text messageText;        // Used for game over messages

    public Color playerOneColor;    
    public Color playerTwoColor;

    private GameObject pole;                                            // The pole - will be searched by name ("pole" + coordinates)
    private Vector3 pieceVecOffset = new Vector3(0.08f, 1.8f, -0.3f);   // The offset between piece and pole
    private int[,,] gameBoard = new int[size, size, size];              // 3D array of 0's for keeping track of the whole board

    private int sum;            // Sum of the number of pieces, to be able to tell if the board is full and the current turn (even\odd sum)
    private int horizontal;     // x input
    private int vertical;       // z input
    private float nextDrop;     // time between dropping pieces
    private bool previousFrameMovement = false;
    private bool restart;
    private int[] startCoord = new int[] { 0, 0 };   // the coordinates of the pole we start with

    void Start () {
        pole = Tools.GetPole(startCoord[0], startCoord[1]); // choosing the pole according to the startCoord
        horizontal = startCoord[0];                                     // setting the correct coordinates according to startCoord
        vertical = startCoord[1];
        pole.GetComponent<PoleScript>().Mark(playerOneColor);           // marking the pole
        messageText.text = string.Empty;                                // blanking out the text message (until game over)
    }

    void Update() {

        Move();

        pole = Tools.GetPole(horizontal, vertical);

        // Dropping the piece, updating the board, checking for a win, changing the color of the next piece

        if (Input.GetButton("Jump") &&                                  // pressing spacebar
            Time.time > nextDrop &&                                     // waiting between drops
            Tools.PiecesOnPole(horizontal, vertical, gameBoard) < 4 &&  // checking if the pole is not full
            !Tools.CheckBoardWin(gameBoard))                           // checking if game not finished
        {
            DropPiece();

            // Game end + message + restart enabled
            if (Tools.CheckBoardWin(gameBoard))                         // checking if it's a win
                OnWin();
            else if (sum == 64)                                         // checking if there's a draw
                OnDraw();
        }

        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Main");
            }
        }
    }

    private void Move()
    {
        int xMovement = (int)Input.GetAxisRaw("Horizontal");
        int yMovement = (int)Input.GetAxisRaw("Vertical");
        if (xMovement != 0 || yMovement != 0)
        {
            // moving only if we didn't move on the previous frame
            if (!previousFrameMovement)
            {
                previousFrameMovement = true;

                // Getting new coordinates and keeping them in range
                int newHorizontal = Mathf.Clamp(horizontal + xMovement, 0, size - 1);
                int newVertical = Mathf.Clamp(vertical + yMovement, 0, size - 1);

                // Unmarking the pole, if we left it
                if (newHorizontal != horizontal || newVertical != vertical)
                {
                    pole = Tools.GetPole(horizontal, vertical);
                    pole.GetComponent<PoleScript>().Unmark();

                    GameObject newPole = Tools.GetPole(newHorizontal, newVertical);
                    newPole.GetComponent<PoleScript>().Mark(CurrentColor());

                    horizontal = newHorizontal;
                    vertical = newVertical;
                }
            }
        }
        else
            previousFrameMovement = false;
    }

    // creating the piece and setting its color + updating the board (player one = 1, player two = -1)
    private void DropPiece()
    {
        nextDrop = Time.time + dropDelay;       // Otherwise it will just "flow" out and make a mess
        GameObject newPiece = Instantiate(piece, pole.GetComponent<Transform>().position + pieceVecOffset, Quaternion.Euler(90f, 0f, 0f));
        newPiece.GetComponent<Renderer>().material.color = CurrentColor();
        gameBoard[horizontal, Tools.PiecesOnPole(horizontal, vertical, gameBoard), vertical] = CurrentPlayer();
        sum++;
    }

    Color CurrentColor()
    {
        return sum % 2 == 0 ? playerOneColor : playerTwoColor;
    }

    int CurrentPlayer()
    {
        return (int)(((sum % 2) - 0.5) * -2);   // (1) for player1 (-1) for player2 (on first move sum == 0)
    }

    private void OnWin()
    {
        sum++;              // incrementing the sum so that CurrentPlayer() will keep showing the winner
        restart = true;
        messageText.text = string.Format("FOUR IN A ROW!\n PLAYER {0} WINS THE GAME!\n\n\n\npress R to restart", ((sum) % 2) + 1);
    }
     
    private void OnDraw()
    {
        restart = true;
        messageText.text = "IT'S A DRAW!\n press R to restart";
    }
}

// A funtion that recieves a coordinate and checks if it's the winning move - OLD! the current CheckWin is in Tools (and checks the whole board)
/*
private bool CheckWin(int x, int y, int z)
{
    int[] sums = new int[13];               // since a move is either 1 or -1, we'll sum each line and if we reach 4 or -4 it's a win
    for (int i = 0; i < size; i++)
    {
        int j = size - 1 - i;               // j is for going backwards
        sums[0] += gameBoard[x, y, i];      // checking only relevant straight lines
        sums[1] += gameBoard[x, i, z];
        sums[2] += gameBoard[i, y, z];
        sums[3] += gameBoard[x, i, i];      // checking diagonals on each relevant layer the piece is on
        sums[4] += gameBoard[i, y, i];
        sums[5] += gameBoard[i, i, z];
        sums[6] += gameBoard[x, i, j];
        sums[7] += gameBoard[i, y, j];
        sums[8] += gameBoard[i, j, z];
        sums[9] += gameBoard[i, i, i];      // checking the all-layer-crossing diagonals
        sums[10] += gameBoard[i, i, j];
        sums[11] += gameBoard[i, j, i];
        sums[12] += gameBoard[j, i, i];
    }
    foreach (int s in sums)
    {
        if (s == 4 || s == -4)
            return true;
    }
    return false;
} */