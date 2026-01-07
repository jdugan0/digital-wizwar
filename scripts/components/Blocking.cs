using Godot;

public class Blocking : IComponent
{
    public static readonly int UP = 1000,
        DOWN = 0100,
        LEFT = 0010,
        RIGHT = 0001;

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

    public int direction;
    public bool movementBlocking;
    public bool losBlocking;
}
