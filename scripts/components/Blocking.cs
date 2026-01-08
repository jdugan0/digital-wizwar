using Godot;

public class Blocking : IComponent
{
    public static readonly int UP = 0b1000,
        DOWN = 0b0100,
        LEFT = 0b0010,
        RIGHT = 0b0001;

    public static int VecToDir(Vector2I dir)
    {
        if (dir == Vector2I.Up)
        {
            return UP;
        }
        if (dir == Vector2I.Down)
        {
            return DOWN;
        }
        if (dir == Vector2I.Left)
        {
            return LEFT;
        }
        if (dir == Vector2I.Right)
        {
            return RIGHT;
        }
        return 0;
    }

    public static int RotateDir(int dirMask, int quarterTurns)
    {
        quarterTurns = ((quarterTurns % 4) + 4) % 4;

        for (int i = 0; i < quarterTurns; i++)
        {
            dirMask = Rotate90CCW(dirMask);
        }

        return dirMask;
    }

    private static int Rotate90CCW(int dirMask)
    {
        int result = 0;

        if ((dirMask & RIGHT) != 0)
            result |= UP;
        if ((dirMask & UP) != 0)
            result |= LEFT;
        if ((dirMask & LEFT) != 0)
            result |= DOWN;
        if ((dirMask & DOWN) != 0)
            result |= RIGHT;

        return result;
    }

    public int direction;
    public bool movementBlocking;
    public bool losBlocking;
}
