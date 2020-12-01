using OpenBots.Core.Script;
using System.Collections.Generic;
using System.Data;

namespace OpenBots.Core.Infrastructure
{
    public interface IfrmWebElementRecorder
    {
        List<ScriptElement> ScriptElements { get; set; }
        DataTable SearchParameters { get; set; }
        string LastItemClicked { get; set; }
        string StartURL { get; set; }
        bool IsRecordingSequence { get; set; }
        bool IsCommandItemSelected { get; set; }
        void CheckBox_StopOnClick(bool check);
    }
}
