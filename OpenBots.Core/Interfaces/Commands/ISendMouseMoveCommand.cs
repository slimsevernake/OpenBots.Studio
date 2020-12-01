namespace OpenBots.Core.Infrastructure
{
    public interface ISendMouseMoveCommand
    {
        string v_XMousePosition { get; set; }
        string v_YMousePosition { get; set; }
        string v_MouseClick { get; set; }
    }
}
