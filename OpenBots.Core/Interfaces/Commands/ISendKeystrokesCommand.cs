namespace OpenBots.Core.Infrastructure
{
    public interface ISendKeystrokesCommand
    {
        string v_WindowName { get; set; }
        string v_TextToSend { get; set; }
        string v_EncryptionOption { get; set; }
    }
}
