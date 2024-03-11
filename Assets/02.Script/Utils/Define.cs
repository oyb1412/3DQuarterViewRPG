public class Define 
{
    public enum Layer
    {
        Monster = 6,
        Ground = 7,
        Block = 9
    }
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }
    public enum MouseEvent
    {
        Press,
        Click,
        PointerDown,
        PointerUp,
    }
    public enum CameraMode
    {
        Quarterview,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }
}
