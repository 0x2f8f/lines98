using System;

public delegate void ShowBox(int x, int y, int ball);
public delegate void PlayCut();

public class Lines
{
    public const int SIZE = 9;
    public const int BALLS = 7;
    const int ADD_BALLS = 6;
    const int CUT_BALLS = 5;

    ShowBox showBox;
    PlayCut playCut;
    Random random;

    int[,] map;
    int fromX, fromY;
    private bool[,] walkUsed;
    bool isBallSelected;

    private bool[,] marked;

    public Lines(ShowBox showBox, PlayCut playCut)
    {
        this.showBox = showBox;
        this.playCut = playCut;
        random = new Random();
        map = new int[SIZE, SIZE];
    }

    public void Start()
    {
        ClearMap();
        AddRandomBalls();
        isBallSelected = false;
    }

    private void AddRandomBalls()
    {
        for (int i = 0; i < ADD_BALLS; i++) {
            AddRandomBall();
        }
    }

    private void AddRandomBall()
    {
        int x, y;
        int loop = SIZE * SIZE;
        do {
            x = random.Next(SIZE);
            y = random.Next(SIZE);
            if (--loop <= 0) {
                return;
            }
        } while (map[x, y] > 0);

        int ball = 1 + random.Next(BALLS - 1);
        SetMap(x, y, ball);
    }

    private void ClearMap()
    {
        for (int x = 0; x < SIZE; x++) {
            for (int y = 0; y < SIZE; y++) {
                SetMap(x, y, 0);
            }
        }
    }

    private void SetMap(int x, int y, int ball)
    {
        map[x, y] = ball;
        showBox(x, y, ball);
    }

    public void Click(int x, int y)
    {
        if (map[x, y] > 0) {
            TakeBall(x, y);
        } else {
            MoveBallTo(x, y);
        }
    }

    private void TakeBall(int x, int y)
    {
        fromX = x;
        fromY = y;
        isBallSelected = true;
    }

    private void MoveBallTo(int x, int y)
    {
        if (!CanMove(x, y)) {
            return;
        }

        SetMap(x, y, map[fromX, fromY]);
        SetMap(fromX, fromY, 0);
        isBallSelected = false;

        if (!CutLines()) {
            AddRandomBalls();
            CutLines();
        }
    }
    
    private bool CanMove(int x, int y)
    {
        if (!isBallSelected) {
            return false;
        }

        /*
        if (map[x, y] != 0) {
            return false;
        }
        */

        walkUsed = new bool[SIZE, SIZE];
        Walk(fromX, fromY, true);

        return walkUsed[x, y];
    }

    private void Walk(int x, int y, bool isStart = false)
    {
        if (!isStart) {
            if (!OnMap(x, y)) return;
            if (map[x, y] > 0) return;
            if (walkUsed[x, y]) return;
        }

        walkUsed[x, y] = true;
        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
    }

    private bool CutLines()
    {
        int balls = 0;
        int lineBalls;
        marked = new bool[SIZE, SIZE];

        int[][] checkCoords = {
            new int[] {1, 0},
            new int[] {0, 1},
            new int[] {1, 1},
            new int[] {-1, 1}
        };

        for (int x = 0; x < SIZE; x++) {
            for (int y = 0; y < SIZE; y++) {
                foreach (var checkCoord in checkCoords) {
                    lineBalls = CalcLine(x, y, checkCoord[0], checkCoord[1]);
                    if (lineBalls > 0) {
                        MarkLine(x, y, checkCoord[0], checkCoord[1]);
                        balls += lineBalls;
                    }
                }
            }
        }

        if (balls > 0) {
            CutMarked();
            playCut();

            return true;
        }

        return false;
    }

    private int GetMap(int x, int y)
    {
        if (OnMap(x, y)) {
            //UnityEngine.Debug.Log($"x={x}, y={y}");
            return map[x, y];
        }

        return 0;
    }

    private bool OnMap(int x, int y)
    {
        return x >= 0
            && x < SIZE
            && y >= 0
            && y < SIZE;
    }

    private int CalcLine(int x0, int y0, int x1, int y1)
    {
        int ball = map[x0, y0];
        if (ball == 0) {
            return 0;
        }

        int count = 0;

        //UnityEngine.Debug.Log($"x0={x0}, y0={y0}, x1={x1}, y1={y1}");
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += x1, y += y1) {
            count++;
        }

        return count >= CUT_BALLS ? count : 0;
    }

    private void MarkLine(int x0, int y0, int x1, int y1)
    {
        int ball = map[x0, y0];
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += x1, y += y1) {
            marked[x, y] = true;
        }
    }

    private void CutMarked()
    {
        for (int x = 0; x < SIZE; x++) {
            for (int y = 0; y < SIZE; y++) {
                if (marked[x, y] == true) {
                    SetMap(x, y, 0);
                }
            }
        }
    }
}
