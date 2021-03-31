using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public float timer;
    public Text timerText;

    public int score;
    public int maxScore = 1000;
    public Text scoreText;
    public Text maxScoreText;

    // 1 = easy, 2 = medium, 3 = hard;
    public int difficulty;
    public Text difficultyText;
    public Text nextDifficultyText;

    public AudioClip matchSound;

    public DraggedTile draggingTile;
    public Sprite[] matchSprites;
    public BoardSlot[,] boardSlots = new BoardSlot[9, 9];

    public GameObject boardSlotPrefab;
    public bool created = false;
    public bool won = false;
    public bool timeOut = false;

    public GameObject backgroundObject;
    public GameObject gameOverObject;
    public GameObject gameWinObject;

    public void Start()
    {
        GenerateBoard();
        CheckBoardMatches();
    }

    private void Update()
    {
        if(created)
            timer -= Time.deltaTime;
        timerText.text = ((int)timer).ToString();

        if(timer <= 0)
        {
            // Game Over
            timeOut = true;
            gameOverObject.SetActive(true);
            gameObject.SetActive(false);
            backgroundObject.SetActive(false);
            resetGame();
        }

        if(score >= maxScore)
        {
            // Game Win
            won = true;
            backgroundObject.SetActive(false);
            gameWinObject.SetActive(true); 
            resetGame();
        }
    }

    public void resetGame()
    {
        foreach(BoardSlot bs in boardSlots)
        {
            Destroy(bs.gameObject);
        }
        created = false;
        difficulty = 1;
        difficultyText.text = "Easy";
        nextDifficultyText.text = "Medium";
        maxScore = 4500;
        score = 0;
        scoreText.text = score.ToString();
        timer = 180.0f;
        timerText.text = timer.ToString();
    }

    public void changeDifficulty()
    {
        switch(difficulty)
        {
            case 1:
                difficulty = 2;
                difficultyText.text = "Medium";
                nextDifficultyText.text = "Hard";
                maxScore = 8000;
                timer = 120.0f;
                break;
            case 2:
                difficulty = 3;
                difficultyText.text = "Hard";
                nextDifficultyText.text = "Easy";
                maxScore = 15000;
                timer = 60.0f;
                break;
            case 3:
                difficulty = 1;
                difficultyText.text = "Easy";
                nextDifficultyText.text = "Medium";
                maxScore = 4500;
                timer = 180.0f;
                break;
            default:
                difficulty = 1;
                difficultyText.text = "Easy";
                nextDifficultyText.text = "Medium";
                maxScore = 4500;
                timer = 180.0f;
                break;
        }
        maxScoreText.text = maxScore.ToString();
    }

    public void GenerateBoard()
    {

        switch (difficulty)
        {
            case 1:
                timer = 180.0f;
                break;
            case 2:
                timer = 120.0f;
                break;
            case 3:
                timer = 60.0f;
                break;
            default:
                timer = 180.0f;
                break;
        }
        GameObject spawnedBoardSlot;

        for (int r = 0; r < 9; r++)
        {
            for(int c = 0; c < 9; c++)
            {
                if (!created)
                {
                    spawnedBoardSlot = Instantiate(boardSlotPrefab, transform);
                    boardSlots[r, c] = spawnedBoardSlot.GetComponent<BoardSlot>();
                }

                boardSlots[r, c].row = r;
                boardSlots[r, c].col = c;
                boardSlots[r, c].draggingTile = draggingTile;
                boardSlots[r, c].boardAttachedTo = this;
                int randomNum;

                switch (difficulty)
                {
                    case 1:
                        randomNum = Random.Range(0, (int)matchType.COUNT - 4);
                        break;

                    case 2:
                        randomNum = Random.Range(0, (int)matchType.COUNT - 3);
                        if (Random.Range(0, 30) == 5) // 5 Bomb, 6 Block
                            randomNum = 5;
                        break;

                    case 3:
                        randomNum = Random.Range(0, (int)matchType.COUNT - 2);
                        if (Random.Range(0, 20) == 5) // 5 Bomb, 6 Block
                            randomNum = 5; 
                        if (Random.Range(0, 20) == 5) // 5 Bomb, 6 Block
                            randomNum = 6;
                        break;

                    default:
                        randomNum = Random.Range(0, (int)matchType.COUNT - 4);
                        break;
                }

                boardSlots[r, c].updateMatchObjectInSlot(matchSprites[randomNum], (matchType)randomNum);
            }
        }
        created = true;
    }

    public void checkBoardMatch(int row, int col)
    {
        Debug.Log("Checking Board At: Row: " + row + ", Col: " + col);
        // Up Down Check
        // Match 5 check
        if (!(row - 2 < 0 || row + 2 > 8))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 1, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 1, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 2, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 2, col].matchObjectInSlot.tileMatchType)
            {

                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row + 1, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row - 1, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row + 2, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row - 2, col].updateMatchObjectInSlot(null, matchType.empty);

                for (int i = row; i > 2; i--)
                {
                    // "Drop" Tiles Down
                    boardSlots[i - 3, col].swapBoardSlots(boardSlots[i - 2, col]);
                    boardSlots[i - 2, col].swapBoardSlots(boardSlots[i - 1, col]);
                    boardSlots[i - 1, col].swapBoardSlots(boardSlots[i, col]);
                    boardSlots[i, col].swapBoardSlots(boardSlots[i + 1, col]);
                    boardSlots[i + 1, col].swapBoardSlots(boardSlots[i + 2, col]);
                }

                DropTile(col);

                if (coconut)
                    score += 2000;
                else
                    score += 400;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }
        // Match 4 Check

        if (!((row - 2 < 0 || row + 1 > 8) || (row - 1 < 0 || row + 2 > 8)))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 1, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 1, col].matchObjectInSlot.tileMatchType
            && (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 2, col].matchObjectInSlot.tileMatchType
            || boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 2, col].matchObjectInSlot.tileMatchType))
            {

                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row + 1, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row - 1, col].updateMatchObjectInSlot(null, matchType.empty);

                if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 2, col].matchObjectInSlot.tileMatchType)
                    boardSlots[row + 2, col].updateMatchObjectInSlot(null, matchType.empty);
                else
                    boardSlots[row - 2, col].updateMatchObjectInSlot(null, matchType.empty);


                if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 2, col].matchObjectInSlot.tileMatchType)
                {
                    for (int i = row; i > 1; i--)
                    {
                        // "Drop" Tiles Down
                        boardSlots[i - 2, col].swapBoardSlots(boardSlots[i - 1, col]);
                        boardSlots[i - 1, col].swapBoardSlots(boardSlots[i, col]);
                        boardSlots[i, col].swapBoardSlots(boardSlots[i + 1, col]);
                        boardSlots[i + 1, col].swapBoardSlots(boardSlots[i + 2, col]);
                    }
                }
                else
                {
                    for (int i = row; i > 2; i--)
                    {
                        // "Drop" Tiles Down
                        boardSlots[i - 3, col].swapBoardSlots(boardSlots[i - 2, col]);
                        boardSlots[i - 2, col].swapBoardSlots(boardSlots[i - 1, col]);
                        boardSlots[i - 1, col].swapBoardSlots(boardSlots[i, col]);
                        boardSlots[i, col].swapBoardSlots(boardSlots[i + 1, col]);
                    }
                }
                DropTile(col);
                if (coconut)
                    score += 2000;
                else
                    score += 275;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }

        if (!(row - 1 < 0 || row + 1 > 8))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 1, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 1, col].matchObjectInSlot.tileMatchType)
            {
                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row + 1, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row - 1, col].updateMatchObjectInSlot(null, matchType.empty);

                for (int i = row; i > 1; i--)
                {
                    boardSlots[i - 2, col].swapBoardSlots(boardSlots[i - 1, col]);
                    boardSlots[i - 1, col].swapBoardSlots(boardSlots[i, col]);
                    boardSlots[i, col].swapBoardSlots(boardSlots[i + 1, col]);
                }

                DropTile(col);
                if (coconut)
                    score += 2000;
                else
                    score += 125;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }

        if (!(row + 2 > 8))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 1, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row + 2, col].matchObjectInSlot.tileMatchType)
            {
                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row + 1, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row + 2, col].updateMatchObjectInSlot(null, matchType.empty);

                for (int i = row; i > 0; i--)
                {
                    boardSlots[i - 1, col].swapBoardSlots(boardSlots[i, col]);
                    boardSlots[i, col].swapBoardSlots(boardSlots[i + 1, col]);
                    boardSlots[i + 1, col].swapBoardSlots(boardSlots[i + 2, col]);
                }

                DropTile(col);
                if (coconut)
                    score += 2000;
                else
                    score += 125;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }

        if (!(row - 2 < 0))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 1, col].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row - 2, col].matchObjectInSlot.tileMatchType)
            {
                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row - 1, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row - 2, col].updateMatchObjectInSlot(null, matchType.empty);

                for (int i = row; i > 2; i--)
                {
                    boardSlots[i - 3, col].swapBoardSlots(boardSlots[i - 2, col]);
                    boardSlots[i - 2, col].swapBoardSlots(boardSlots[i - 1, col]);
                    boardSlots[i - 1, col].swapBoardSlots(boardSlots[i, col]);
                }

                DropTile(col);
                if (coconut)
                    score += 2000;
                else
                    score += 125;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }
        // Left Right Check
        // Match 5 check

        if (!(col - 2 < 0 || col + 2 > 8))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 1].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 1].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 2].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 2].matchObjectInSlot.tileMatchType)
            {

                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col + 1].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col - 1].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col + 2].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col - 2].updateMatchObjectInSlot(null, matchType.empty);

                if (row == 0)
                {

                }
                else
                {
                    for (int i = row; i > 0; i--)
                    {
                        boardSlots[i, col].swapBoardSlots(boardSlots[i - 1, col]);
                        boardSlots[i, col + 1].swapBoardSlots(boardSlots[i - 1, col + 1]);
                        boardSlots[i, col - 1].swapBoardSlots(boardSlots[i - 1, col - 1]);
                        boardSlots[i, col + 2].swapBoardSlots(boardSlots[i - 1, col + 2]);
                        boardSlots[i, col - 2].swapBoardSlots(boardSlots[i - 1, col - 2]);
                    }
                }

                DropTile(col);
                DropTile(col + 1);
                DropTile(col - 1);
                DropTile(col + 2);
                DropTile(col - 2);
                if (coconut)
                    score += 2000;
                else
                    score += 400;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }
        // Match 4 check
        if (!((col - 2 < 0 || col + 1 > 8) || (col - 1 < 0 || col + 2 > 8)))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 1].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 1].matchObjectInSlot.tileMatchType
            && (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 2].matchObjectInSlot.tileMatchType
            || boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 2].matchObjectInSlot.tileMatchType))
            {

                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col + 1].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col - 1].updateMatchObjectInSlot(null, matchType.empty);

                if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 2].matchObjectInSlot.tileMatchType)
                    boardSlots[row, col + 2].updateMatchObjectInSlot(null, matchType.empty);
                else
                    boardSlots[row, col - 2].updateMatchObjectInSlot(null, matchType.empty);

                if (row == 0)
                {

                }
                else
                {
                    for (int i = row; i > 0; i--)
                    {
                        boardSlots[i, col].swapBoardSlots(boardSlots[i - 1, col]);
                        boardSlots[i, col + 1].swapBoardSlots(boardSlots[i - 1, col + 1]);
                        boardSlots[i, col - 1].swapBoardSlots(boardSlots[i - 1, col - 1]);

                        if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 2].matchObjectInSlot.tileMatchType)
                            boardSlots[i, col + 2].swapBoardSlots(boardSlots[i - 1, col + 2]);
                        else
                            boardSlots[i, col - 2].swapBoardSlots(boardSlots[i - 1, col - 2]);
                    }
                }

                DropTile(col);
                DropTile(col + 1);
                DropTile(col - 1);

                if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 2].matchObjectInSlot.tileMatchType)
                    DropTile(col + 2);
                else
                    DropTile(col - 2);

                if (coconut)
                    score += 2000;
                else
                    score += 275;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }

        if (!(col - 1 < 0 || col + 1 > 8))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 1].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 1].matchObjectInSlot.tileMatchType)
            {
                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col + 1].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col - 1].updateMatchObjectInSlot(null, matchType.empty);

                if (row == 0)
                {

                }
                else
                {
                    for (int i = row; i > 0; i--)
                    {
                        boardSlots[i, col].swapBoardSlots(boardSlots[i - 1, col]);
                        boardSlots[i, col + 1].swapBoardSlots(boardSlots[i - 1, col + 1]);
                        boardSlots[i, col - 1].swapBoardSlots(boardSlots[i - 1, col - 1]);
                    }
                }

                DropTile(col);
                DropTile(col + 1);
                DropTile(col - 1);
                if (coconut)
                    score += 2000;
                else
                    score += 125;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }

        if (!(col + 2 > 8))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 1].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col + 2].matchObjectInSlot.tileMatchType)
            {
                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col + 1].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col + 2].updateMatchObjectInSlot(null, matchType.empty);

                for (int i = row; i > 0; i--)
                {
                    boardSlots[i, col].swapBoardSlots(boardSlots[i - 1, col]);
                    boardSlots[i, col + 1].swapBoardSlots(boardSlots[i - 1, col + 1]);
                    boardSlots[i, col + 2].swapBoardSlots(boardSlots[i - 1, col + 2]);
                }

                DropTile(col);
                DropTile(col + 1);
                DropTile(col + 2);
                if (coconut)
                    score += 2000;
                else
                    score += 125;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }

        if (!(col - 2 < 0))
        {
            if (boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 1].matchObjectInSlot.tileMatchType
            && boardSlots[row, col].matchObjectInSlot.tileMatchType == boardSlots[row, col - 2].matchObjectInSlot.tileMatchType)
            {
                bool coconut = boardSlots[row, col].matchObjectInSlot.tileMatchType == matchType.bomb;
                boardSlots[row, col].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col - 1].updateMatchObjectInSlot(null, matchType.empty);
                boardSlots[row, col - 2].updateMatchObjectInSlot(null, matchType.empty);

                for (int i = row; i > 2; i--)
                {
                    boardSlots[i, col].swapBoardSlots(boardSlots[i - 1, col]);
                    boardSlots[i, col - 1].swapBoardSlots(boardSlots[i - 1, col - 1]);
                    boardSlots[i, col - 2].swapBoardSlots(boardSlots[i - 1, col - 2]);
                }

                DropTile(col);
                DropTile(col - 1);
                DropTile(col - 2);
                if (coconut)
                    score += 2000;
                else
                    score += 125;
                scoreText.text = score.ToString();
                GetComponent<AudioSource>().PlayOneShot(matchSound);
            }
        }
    }

    public void CheckBoardMatches()
    {
        int count = 0;
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (c == 0)
                {

                }
                else if (c == 8)
                {

                }
                else
                {
                    if (boardSlots[r, c].matchObjectInSlot.tileMatchType == boardSlots[r, c + 1].matchObjectInSlot.tileMatchType
                    && boardSlots[r, c].matchObjectInSlot.tileMatchType == boardSlots[r, c - 1].matchObjectInSlot.tileMatchType)
                    {
                        count++;
                        // 3 Match
                        boardSlots[r, c].updateMatchObjectInSlot(null, matchType.empty);
                        boardSlots[r, c + 1].updateMatchObjectInSlot(null, matchType.empty);
                        boardSlots[r, c - 1].updateMatchObjectInSlot(null, matchType.empty);

                        Debug.Log("Found 3 Match at: " + r + ", " + c);
                        if(r == 0)
                        {

                        }
                        else
                        {
                            for(int i = r; i > 0; i--)
                            {
                                boardSlots[i, c].swapBoardSlots(boardSlots[i - 1, c]);
                                boardSlots[i, c + 1].swapBoardSlots(boardSlots[i - 1, c + 1]);
                                boardSlots[i, c - 1].swapBoardSlots(boardSlots[i - 1, c - 1]);
                            }
                        }

                        // After match is found, and tiles get updated, move tiles down
                        DropTile(c);
                        DropTile(c + 1);
                        DropTile(c - 1);

                        CheckBoardMatches();
                    }
                }
            }
        }
        Debug.Log("Found " + count + " 3 Matches");
    }

    private void DropTile(int col)
    {
        int rowsToFill = 0;
        bool finished = false;
        do
        {
            int randomNum;

            switch (difficulty)
            {
                case 1:
                    randomNum = Random.Range(0, (int)matchType.COUNT - 4);
                    break;

                case 2:
                    randomNum = Random.Range(0, (int)matchType.COUNT - 3);
                    if (Random.Range(0, 30) == 5) // 5 Bomb, 6 Block
                        randomNum = 5;
                    break;

                case 3:
                    randomNum = Random.Range(0, (int)matchType.COUNT - 2);
                    if (Random.Range(0, 20) == 5) // 5 Bomb, 6 Block
                        randomNum = 5;
                    if (Random.Range(0, 20) == 5) // 5 Bomb, 6 Block
                        randomNum = 6;
                    break;

                default:
                    randomNum = Random.Range(0, (int)matchType.COUNT - 4);
                    break;
            }

            boardSlots[rowsToFill++, col].updateMatchObjectInSlot(matchSprites[randomNum], (matchType)randomNum);
            Debug.Log(rowsToFill);
            if(rowsToFill > 8)
                finished = true;
            else if (boardSlots[rowsToFill, col].matchObjectInSlot.tileMatchType == matchType.empty)
            {
                Debug.Log("Filled, Row: " + (rowsToFill - 1) + ", Col: " + col);
                boardSlots[rowsToFill, col].swapBoardSlots(boardSlots[rowsToFill - 1, col]);
            }
            else
                finished = true;

        } while (!finished);
        CheckBoardMatches();
    }
}
