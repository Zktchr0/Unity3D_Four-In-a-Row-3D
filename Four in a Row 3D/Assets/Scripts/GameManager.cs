using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject piece;        // The game pieces prefab
    public static int size = 4;     // Size of the (cubic) board
    public float dropDelay = 0.2f;  // So that pieces won't tumble into each other

    public Text messageText;        // Used for game over messages

    public Color playerOneColor;    
    public Color playerTwoColor;
    private Color currentColor;     // For keeping track and finding the winner


    private GameObject pole;                                // The pole - will be searched by name ("pole" + coordinates)
    private Vector3 pieceVecOffset;                         // The offset between piece and pole
    private int[,] piecesOnPole = new int[size,size];       // Counting the pieces on each pole (limited to 4)
    private int[,,] gameBoard = new int[size, size, size];  // 3D array for keeping track of the whole board

    private int sum;            // Sum of the number of pieces, to be able to tell if the board is full
    private int horizontal;     // x input
    private int vertical;       // z input
    private float nextDrop;     // time between dropping pieces
    private bool won;           
    private bool restart;

    void Start () {
        pieceVecOffset = new Vector3(0.08f, 1.8f, -0.3f);

        // initializing the arrays with zeros
        for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    piecesOnPole[i,j] = 0;
                }
            }

        for (int xAxis = 0; xAxis < size; xAxis++)
        {
            for (int yAxis = 0; yAxis < size; yAxis++)
            {
                for (int zAxis = 0; zAxis < size; zAxis++)
                {
                    gameBoard[xAxis, yAxis, zAxis] = 0;
                }
            }
        }
        
        messageText.text = string.Empty;
        won = false;
        sum = 0;
    }

    void Update() {
        // First - Marking the pole we're on (and updating it).

        GameObject pole = GameObject.Find("Pole" + horizontal + vertical);

        pole.GetComponent<PoleScript>().Mark();

        // Getting new coordinates and keeping them in range
        int newHorizontal = horizontal + (int)(Input.GetAxisRaw("Horizontal"));
        int newVertical = vertical + (int)(Input.GetAxisRaw("Vertical"));
        if (newHorizontal < 0)
            newHorizontal = 0;
        if (newVertical < 0)
            newVertical = 0;
        if (newHorizontal > 3)
            newHorizontal = 3;
        if (newVertical > 3)
            newVertical = 3;

        // Unmarking the pole, if we left it
        if (newHorizontal != horizontal || newVertical != vertical)
        {
            pole.GetComponent<PoleScript>().Unmark();
            horizontal = newHorizontal;
            vertical = newVertical;
        }

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
            nextDrop = Time.time + dropDelay;       // Otherwise it will just "flow" out and make a mess
            piecesOnPole[horizontal, vertical]++;
            sum++;

            // creating and picking the color of the piece + updating the board (player one = 1, player two = -1)

            GameObject newPiece = Instantiate(piece, pole.GetComponent<Transform>().position + pieceVecOffset, Quaternion.Euler(90f, 0f, 0f));
            if (currentColor != playerOneColor)
            {
                newPiece.GetComponent<Renderer>().material.color = playerOneColor;
                currentColor = playerOneColor;
                gameBoard[horizontal, piecesOnPole[horizontal, vertical] - 1, vertical] = 1;
            }
            else
            {
                newPiece.GetComponent<Renderer>().material.color = playerTwoColor;
                currentColor = playerTwoColor;
                gameBoard[horizontal, piecesOnPole[horizontal, vertical] - 1, vertical] = -1;
            }

            won = (IsWinning());
        }

        // Game end + message + restart enabled

        if (won)
        {
            restart = true;
            if (currentColor == playerOneColor)
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

    // This should be done much better but it could've been worse (only a few cases are checked). I think that, at least, it's quite clear this way...

    private bool IsWinning()
    {
        int thisX = horizontal;
        int thisY = piecesOnPole[horizontal, vertical] - 1;
        int thisZ = vertical;

        // checking current straight lines
        if ((gameBoard[thisX, thisY, 0] != 0 &&
            gameBoard[thisX, thisY, 0] == gameBoard[thisX, thisY, 1] &&
            gameBoard[thisX, thisY, 1] == gameBoard[thisX, thisY, 2] &&
            gameBoard[thisX, thisY, 2] == gameBoard[thisX, thisY, 3]) ||
            (gameBoard[thisX, 0, thisZ] != 0 &&
            gameBoard[thisX, 0, thisZ] == gameBoard[thisX, 1, thisZ] &&
            gameBoard[thisX, 1, thisZ] == gameBoard[thisX, 2, thisZ] &&
            gameBoard[thisX, 2, thisZ] == gameBoard[thisX, 3, thisZ]) ||
            (gameBoard[0, thisY, thisZ] != 0 &&
            gameBoard[0, thisY, thisZ] == gameBoard[1, thisY, thisZ] &&
            gameBoard[1, thisY, thisZ] == gameBoard[2, thisY, thisZ] &&
            gameBoard[2, thisY, thisZ] == gameBoard[3, thisY, thisZ]) ||
        // checking diagonals on each layer the piece is on
            (gameBoard[thisX, 0, 0] != 0 &&
            gameBoard[thisX, 0, 0] == gameBoard[thisX, 1, 1] &&
            gameBoard[thisX, 1, 1] == gameBoard[thisX, 2, 2] &&
            gameBoard[thisX, 2, 2] == gameBoard[thisX, 3, 3]) ||
            (gameBoard[thisX, 3, 0] != 0 &&
            gameBoard[thisX, 3, 0] == gameBoard[thisX, 2, 1] &&
            gameBoard[thisX, 2, 1] == gameBoard[thisX, 1, 2] &&
            gameBoard[thisX, 1, 2] == gameBoard[thisX, 0, 3]) ||
            (gameBoard[0, thisY, 0] != 0 &&
            gameBoard[0, thisY, 0] == gameBoard[1, thisY, 1] &&
            gameBoard[1, thisY, 1] == gameBoard[2, thisY, 2] &&
            gameBoard[2, thisY, 2] == gameBoard[3, thisY, 3]) ||
            (gameBoard[0, thisY, 3] != 0 &&
            gameBoard[0, thisY, 3] == gameBoard[1, thisY, 2] &&
            gameBoard[1, thisY, 2] == gameBoard[2, thisY, 1] &&
            gameBoard[2, thisY, 1] == gameBoard[3, thisY, 0]) ||
            (gameBoard[0, 0, thisZ] != 0 &&
            gameBoard[0, 0, thisZ] == gameBoard[1, 1, thisZ] &&
            gameBoard[1, 1, thisZ] == gameBoard[2, 2, thisZ] &&
            gameBoard[2, 2, thisZ] == gameBoard[3, 3, thisZ]) ||
            (gameBoard[0, 3, thisZ] != 0 &&
            gameBoard[0, 3, thisZ] == gameBoard[1, 2, thisZ] &&
            gameBoard[1, 2, thisZ] == gameBoard[2, 1, thisZ] &&
            gameBoard[2, 1, thisZ] == gameBoard[3, 0, thisZ]) ||
        // checking the all-layer-crossing diagonals
            (gameBoard[0, 0, 0] != 0 && 
            gameBoard[0, 0, 0] == gameBoard[1, 1, 1] &&
            gameBoard[1, 1, 1] == gameBoard[2, 2, 2] &&
            gameBoard[2, 2, 2] == gameBoard[3, 3, 3]) ||
            (gameBoard[0, 0, 3] != 0 &&
            gameBoard[0, 0, 3] == gameBoard[1, 1, 2] &&
            gameBoard[1, 1, 2] == gameBoard[2, 2, 1] &&
            gameBoard[2, 2, 1] == gameBoard[3, 3, 0]) ||
            (gameBoard[0, 3, 3] != 0 &&
            gameBoard[0, 3, 3] == gameBoard[1, 2, 2] &&
            gameBoard[1, 2, 2] == gameBoard[2, 1, 1] &&
            gameBoard[2, 1, 1] == gameBoard[3, 0, 0]) ||
            (gameBoard[0, 3, 0] != 0 &&
            gameBoard[0, 3, 0] == gameBoard[1, 2, 1] &&
            gameBoard[1, 2, 1] == gameBoard[2, 1, 2] &&
            gameBoard[2, 1, 2] == gameBoard[3, 0, 3]) ||
            (gameBoard[3, 3, 0] != 0 &&
            gameBoard[3, 3, 0] == gameBoard[2, 2, 1] &&
            gameBoard[2, 2, 1] == gameBoard[1, 1, 2] &&
            gameBoard[1, 1, 2] == gameBoard[0, 0, 3]) ||
            (gameBoard[3, 0, 0] != 0 &&
            gameBoard[3, 0, 0] == gameBoard[2, 1, 1] &&
            gameBoard[2, 1, 1] == gameBoard[1, 2, 2] &&
            gameBoard[1, 2, 2] == gameBoard[0, 3, 3])
            )
            return true;
        return false;
    }

}
