// .. Store all game tags in a public class to be accessible anywhere.
public class Tags
{
    public const string player = "Player";
    public const string node = "Node";
    public const string food = "Food";
    public const string wall = "Wall";
    public const string fruit = "Fruit";
    public const string bomb = "Bomb";
    public const string pickupsParent = "PickupsParent";
}

// .. Store all the constant dimensions that we use in the game
public class Metrics
{
    public const float SNACK_NODE = .2f;
    public const float FRUIT = .2f;
    public const float OBSTACLE = .2f;
}

// .. Store game borders values
public class WorldBorders
{
    public const float LeftBorder = -4.4f;
    public const float RightBorder = 4.4f;
    public const float TopBorder = 2.4f;
    public const float BottomBorder = -2.4f;
}

// .. Store all directions player can have
public enum PlayerDirection
{
    Left = 0,
    Up = 1,
    Right = 2,
    Down = 3,
    Count = 4,
}

// .. Store all different types of pickups
public enum PickupType
{
    Fruit,
    Bomb,
}