using System.Data;

namespace OpenBots.Core.Infrastructure
{
    public interface IfrmAdvancedUIElementRecorder
    {
        DataTable SearchParameters { get; set; }
        string LastItemClicked { get; set; }
        bool IsRecordingSequence { get; set; }
        bool IsCommandItemSelected { get; set; }
        void SetWindowTitle(string title);
        string GetWindowTitle();
    }
}
