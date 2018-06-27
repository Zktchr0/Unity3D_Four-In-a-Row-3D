using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject piece;        // The game pieces prefab
    public static int size = 4;     // Size of the (cubic) board
    public float dropDelay = 0.25f; // So that pieces won't tumble into each other
    public float moveDelay = 0.3f;  // So that the marker will move one pole at a time

    public Text messageText;        // Used for game over messages

    public Color playerOneColor;    
    public Color playerTwoColor;
    private Color currentColor;     // For keeping track and finding the winner


    private GameObject pole;                                            // The pole - will be searched by name ("pole" + coordinates)
    private Vector3 pieceVecOffset = new Vector3(0.08f, 1.8f, -0.3f);   // The offset between piece and pole
    private int[,] piecesOnPole = new int[size,size];                   // 2D array of 0's Counting the pieces on each pole (limited to 4)
    private int[,,] gameBoard = new int[size, size, size];              // 3D array of 0's for keeping track of the whole board

    private int sum;            // Sum of the number of pieces, to be able to tell if the board is full
    private int horizontal;     // x input
    private int vertical;       // z input
    private float nextDrop;     // time between dropping pieces
    private bool previousFrameMovement = false;
    private bool won = false;   
    private bool restart;

    void Start () {
        currentColor = playerOneColor;
        pieceVecOffset = new Vector3(0.08f, 1.8f, -0.3f);
        messageText.text = string.Empty;
    }

    void Update() {

        Move((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));

        // Enabling the restart option (will be done when the game is ended)
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Main");
            }
        }
    }

    private void LateUpdate()   // For things that happen after the mark is in place and before it moves
    {   
        GameObject pole = GameObject.Find("Pole" + horizontal + vertical);
        
        // Dropping the piece, updating the board, checking for a win, changing the color of the next piece

        if (Input.GetButton("Jump") && Time.time > nextDrop && piecesOnPole[horizontal, vertical] < 4 && !won)
        {
            // changing the marker color

            if (currentColor != playerOneColor)
                currentColor = playerOneColor;
            else
                currentColor = playerTwoColor;

            nextDrop = Time.time + dropDelay;       // Otherwise it will just "flow" out and make a mess
            piecesOnPole[horizontal, vertical]++;
            sum++;

            // creating the piece and setting its color + updating the board (player one = 1, player two = -1)

            GameObject newPiece = Instantiate(piece, pole.GetComponent<Transform>().position + pieceVecOffset, Quaternion.Euler(90f, 0f, 0f));
            if (currentColor != playerOneColor)
            {
                newPiece.GetComponent<Renderer>().material.color = playerOneColor;
                gameBoard[horizontal, piecesOnPole[horizontal, vertical] - 1, vertical] = 1;
            }
            else
            {
                newPiece.GetComponent<Renderer>().material.color = playerTwoColor;
                gameBoard[horizontal, piecesOnPole[horizontal, vertical] - 1, vertical] = -1;
            }

            won = CheckWin(horizontal, piecesOnPole[horizontal, vertical] - 1, vertical);   //using CheckWin with the last move
        }

        // Game end + message + restart enabled

        if (won)
        {
            restart = true;
            if (currentColor != playerOneColor)
            {
                messageText.text = "FOUR IN A ROW!\n PLAYER 1 WINS THE GAME!\n\n\n\npress R to restart";
            }
            else
                messageText.text = "FOUR IN A ROW!\n PLAYER 2 WINS THE GAME!\n\n\n\npress R to restart";
        }
        if (sum == 64 && !won)  // The rare case of a full board with no winner
        {
            restart = true;
            messageText.text = "IT'S A DRAW!\n press R to restart";
        }

    }

    private void Move(int x, int y)
    {
        // First - Marking the pole we're on (and updating it).
        GameObject pole = GameObject.Find("Pole" + horizontal + vertical);

        if (!won)
            pole.GetComponent<PoleScript>().Mark(currentColor);

        if (x != 0||y != 0)
        {
            // moving only if we didn't move on the previous frame
            if (!previousFrameMovement)
            {
                previousFrameMovement = true;

                // Getting new coordinates and keeping them in range
                int newHorizontal = EnforceBound(horizontal + x);
                int newVertical = EnforceBound(vertical + y);

                // Unmarking the pole, if we left it
                if (newHorizontal != horizontal || newVertical != vertical)
                {
                    pole.GetComponent<PoleScript>().Unmark();
                    horizontal = newHorizontal;
                    vertical = newVertical;
                }
            }
        }
        else
            previousFrameMovement = false;
    }

    private int EnforceBound(int axis)
    {
        if (axis < 0)
            return 0;
        else if (axis > 3)
            return 3;
        else return axis;
    }


    // A funtion that recieves a coordinate and checks if it's the winning move

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
    }

}