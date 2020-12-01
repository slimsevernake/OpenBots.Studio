using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using OpenBots.Core.Script;
using OpenBots.Core.Settings;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Studio.Utilities;
using OpenBots.UI.CustomControls.CustomUIControls;
using OpenBots.UI.Forms.Supplement_Forms;
using OpenBots.UI.Supplement_Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.ScriptBuilder_Forms
{
    public partial class frmScriptBuilder : Form
    {
        #region UI Buttons
        #region File Actions Tool Strip and Buttons
        private void uiBtnNew_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void NewFile()
        {
            ScriptFilePath = null;

            string title = $"New Tab {(uiScriptTabControl.TabCount + 1)} *";
            TabPage newTabPage = new TabPage(title)
            {
                Name = title,
                Tag = new ScriptObject(new List<ScriptVariable>(), new List<ScriptElement>()),
                ToolTipText = ""
            };
            uiScriptTabControl.Controls.Add(newTabPage);
            newTabPage.Controls.Add(NewLstScriptActions(title));
            newTabPage.Controls.Add(pnlCommandHelper);

            uiScriptTabControl.SelectedTab = newTabPage;

            _selectedTabScriptActions = (UIListView)uiScriptTabControl.SelectedTab.Controls[0];
            _selectedTabScriptActions.Items.Clear();
            HideSearchInfo();

            _scriptVariables = new List<ScriptVariable>();
            //assign ProjectPath variable
            var projectPathVariable = new ScriptVariable
            {
                VariableName = "ProjectPath",
                VariableValue = "Value Provided at Runtime"
            };
            _scriptVariables.Add(projectPathVariable);
            GenerateRecentFiles();
            newTabPage.Controls[0].Hide();
            pnlCommandHelper.Show();
        }

        private void uiBtnOpen_Click(object sender, EventArgs e)
        {
            //show ofd
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = ScriptProjectPath,
                RestoreDirectory = true,
                Filter = "Json (*.json)|*.json"
            };

            //if user selected file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //open file
                OpenFile(openFileDialog.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show ofd
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = ScriptProjectPath,
                RestoreDirectory = true,
                Filter = "Json (*.json)|*.json"
            };

            //if user selected file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //open file
                OpenFile(openFileDialog.FileName);
            }
        }

        public delegate void OpenFileDelegate(string filepath);
        public void OpenFile(string filePath)
        {
            if (InvokeRequired)
            {
                var d = new OpenFileDelegate(OpenFile);
                Invoke(d, new object[] { filePath });
            }
            else
            {
                try
                {
                    //create or switch to TabPage
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    var foundTab = uiScriptTabControl.TabPages.Cast<TabPage>().Where(t => t.ToolTipText == filePath)
                                                                              .FirstOrDefault();
                    if (foundTab == null)
                    {
                        TabPage newtabPage = new TabPage(fileName)
                        {
                            Name = fileName,
                            ToolTipText = filePath
                        };

                        uiScriptTabControl.TabPages.Add(newtabPage);
                        newtabPage.Controls.Add(NewLstScriptActions(fileName));
                        uiScriptTabControl.SelectedTab = newtabPage;
                    }
                    else
                    {
                        uiScriptTabControl.SelectedTab = foundTab;
                        return;
                    }

                    _selectedTabScriptActions = (UIListView)uiScriptTabControl.SelectedTab.Controls[0];

                    //get deserialized script
                    Script deserializedScript = Script.DeserializeFile(filePath);

                    //reinitialize
                    _selectedTabScriptActions.Items.Clear();
                    _scriptVariables = new List<ScriptVariable>();
                    _scriptElements = new List<ScriptElement>();

                    if (deserializedScript.Commands.Count == 0)
                    {
                        Notify("Error Parsing File: Commands not found!", Color.Red);
                    }

                    //update file path and reflect in title bar
                    ScriptFilePath = filePath;

                    string scriptFileName = Path.GetFileNameWithoutExtension(ScriptFilePath);
                    _selectedTabScriptActions.Name = $"{scriptFileName}ScriptActions";

                    //assign variables
                    _scriptVariables.AddRange(deserializedScript.Variables);
                    _scriptElements.AddRange(deserializedScript.Elements);
                    uiScriptTabControl.SelectedTab.Tag = new ScriptObject(_scriptVariables, _scriptElements );                  

                    //populate commands
                    PopulateExecutionCommands(deserializedScript.Commands);

                    FileInfo scriptFileInfo = new FileInfo(_scriptFilePath);
                    uiScriptTabControl.SelectedTab.Text = scriptFileInfo.Name.Replace(".json", "");

                    //notify
                    Notify("Script Loaded Successfully!", Color.White);
                }
                catch (Exception ex)
                {
                    //signal an error has happened
                    Notify("An Error Occured: " + ex.Message, Color.Red);
                }
            }           
        }

        private void uiBtnSave_Click(object sender, EventArgs e)
        {
            //clear selected items
            ClearSelectedListViewItems();
            SaveToFile(false);
        }

        private void uiBtnSaveAs_Click(object sender, EventArgs e)
        {
            //clear selected items
            ClearSelectedListViewItems();
            SaveToFile(true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear selected items
            ClearSelectedListViewItems();
            SaveToFile(false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear selected items
            ClearSelectedListViewItems();
            SaveToFile(true);
        }

        private void SaveToFile(bool saveAs)
        {
            if (_selectedTabScriptActions.Items.Count == 0)
            {
                Notify("You must have at least 1 automation command to save.", Color.Yellow);
                return;
            }

            int beginLoopValidationCount = 0;
            int beginIfValidationCount = 0;
            int tryCatchValidationCount = 0;
            int retryValidationCount = 0;
            int beginSwitchValidationCount = 0;

            foreach (ListViewItem item in _selectedTabScriptActions.Items)
            {
                if(item.Tag.GetType().Name == "BrokenCodeCommentCommand")
                {
                    Notify("Please verify that all broken code has been removed or replaced.", Color.Yellow);
                    return;
                }
                else if ((item.Tag.GetType().Name == "LoopCollectionCommand") || (item.Tag.GetType().Name == "LoopContinuouslyCommand") ||
                    (item.Tag.GetType().Name == "LoopNumberOfTimesCommand") || (item.Tag.GetType().Name == "BeginLoopCommand") ||
                    (item.Tag.GetType().Name == "BeginMultiLoopCommand"))
                {
                    beginLoopValidationCount++;
                }
                else if (item.Tag.GetType().Name == "EndLoopCommand")
                {
                    beginLoopValidationCount--;
                }
                else if ((item.Tag.GetType().Name == "BeginIfCommand") || (item.Tag.GetType().Name == "BeginMultiIfCommand"))
                {
                    beginIfValidationCount++;
                }
                else if (item.Tag.GetType().Name == "EndIfCommand")
                {
                    beginIfValidationCount--;
                }
                else if (item.Tag.GetType().Name == "BeginTryCommand")
                {
                    tryCatchValidationCount++;
                }
                else if (item.Tag.GetType().Name == "EndTryCommand")
                {
                    tryCatchValidationCount--;
                }
                else if (item.Tag.GetType().Name == "BeginRetryCommand")
                {
                    retryValidationCount++;
                }
                else if (item.Tag.GetType().Name == "EndRetryCommand")
                {
                    retryValidationCount--;
                }
                else if(item.Tag.GetType().Name == "BeginSwitchCommand")
                {
                    beginSwitchValidationCount++;
                }
                else if (item.Tag.GetType().Name == "EndSwitchCommand")
                {
                    beginSwitchValidationCount--;
                }

                //end loop was found first
                if (beginLoopValidationCount < 0)
                {
                    Notify("Please verify the ordering of your loops.", Color.Yellow);
                    return;
                }

                //end if was found first
                if (beginIfValidationCount < 0)
                {
                    Notify("Please verify the ordering of your ifs.", Color.Yellow);
                    return;
                }

                if (tryCatchValidationCount < 0)
                {
                    Notify("Please verify the ordering of your try/catch blocks.", Color.Yellow);
                    return;
                }

                if (retryValidationCount < 0)
                {
                    Notify("Please verify the ordering of your retry blocks.", Color.Yellow);
                    return;
                }

                if (beginSwitchValidationCount < 0)
                {
                    Notify("Please verify the ordering of your switch/case blocks.", Color.Yellow);
                    return;
                }
            }

            //extras were found
            if (beginLoopValidationCount != 0)
            {
                Notify("Please verify the ordering of your loops.", Color.Yellow);
                return;
            }

            //extras were found
            if (beginIfValidationCount != 0)
            {
                Notify("Please verify the ordering of your ifs.", Color.Yellow);
                return;
            }

            if (tryCatchValidationCount != 0)
            {
                Notify("Please verify the ordering of your try/catch blocks.", Color.Yellow);
                return;
            }

            if (retryValidationCount != 0)
            {
                Notify("Please verify the ordering of your retry blocks.", Color.Yellow);
                return;
            }

            if (beginSwitchValidationCount != 0)
            {
                Notify("Please verify the ordering of your switch/case blocks.", Color.Yellow);
                return;
            }

            //define default output path
            if (string.IsNullOrEmpty(ScriptFilePath) || (saveAs))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = ScriptProjectPath,
                    RestoreDirectory = true,
                    Filter = "Json (*.json)|*.json"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                if (!saveFileDialog.FileName.ToString().Contains(ScriptProjectPath))
                {
                    Notify("An Error Occured: Attempted to save script outside of project directory", Color.Red);
                    return;
                }

                ScriptFilePath = saveFileDialog.FileName;
                string scriptFileName = Path.GetFileNameWithoutExtension(ScriptFilePath);
                if (uiScriptTabControl.SelectedTab.Text != scriptFileName)
                    UpdateTabPage(uiScriptTabControl.SelectedTab, ScriptFilePath);
            }

            //serialize script
            try
            {
                var exportedScript = Script.SerializeScript(_selectedTabScriptActions.Items, _scriptVariables, _scriptElements, ScriptFilePath);
                uiScriptTabControl.SelectedTab.Text = uiScriptTabControl.SelectedTab.Text.Replace(" *", "");
                //show success dialog
                Notify("File has been saved successfully!", Color.White);

                try
                {
                    ScriptProject.SaveProject(ScriptFilePath);
                }
                catch (Exception ex)
                {
                    Notify(ex.Message, Color.Red);
                }              
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void SaveAllFiles()
        {
            TabPage currentTab = uiScriptTabControl.SelectedTab;
            foreach (TabPage openTab in uiScriptTabControl.TabPages)
            {
                if (openTab.Text.Contains(" *"))
                {
                    uiScriptTabControl.SelectedTab = openTab;
                    //clear selected items
                    ClearSelectedListViewItems();
                    SaveToFile(false); // Save & Run!
                }
            }
            uiScriptTabControl.SelectedTab = currentTab;
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllFiles();
        }

        private void uiBtnSaveAll_Click(object sender, EventArgs e)
        {
            SaveAllFiles();
        }

        private void ClearSelectedListViewItems()
        {
            _selectedTabScriptActions.SelectedItems.Clear();
            _selectedIndex = -1;
            _selectedTabScriptActions.Invalidate();
        }

        private void publishProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllFiles();
            frmPublishProject publishProject = new frmPublishProject(ScriptProjectPath, ScriptProject);
            publishProject.ShowDialog();

            if (publishProject.DialogResult == DialogResult.OK)
                Notify(publishProject.NotificationMessage, Color.White);
        }

        private void uiBtnPublishProject_Click(object sender, EventArgs e)
        {
            publishProjectToolStripMenuItem_Click(sender, e);
        }

        private void uiBtnImport_Click(object sender, EventArgs e)
        {
            BeginImportProcess();
        }

        private void importFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginImportProcess();
        }

        private void BeginImportProcess()
        {
            //show ofd
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Folders.GetFolder(FolderType.ScriptsFolder),
                RestoreDirectory = true,
                Filter = "Json (*.json)|*.json"
            };

            //if user selected file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //import
                Cursor.Current = Cursors.WaitCursor;
                Import(openFileDialog.FileName);
                Cursor.Current = Cursors.Default;
            }
        }

        private void Import(string filePath)
        {
            try
            {
                //deserialize file
                Script deserializedScript = Script.DeserializeFile(filePath);

                if (deserializedScript.Commands.Count == 0)
                {
                    Notify("Error Parsing File: Commands not found!", Color.Red);
                }

                //variables for comments
                var fileName = new FileInfo(filePath).Name;
                var dateTimeNow = DateTime.Now.ToString();

                //comment
                dynamic addCodeCommentCommand = TypeMethods.CreateTypeInstance(AppDomain.CurrentDomain, "AddCodeCommentCommand");
                addCodeCommentCommand.v_Comment = "Imported From " + fileName + " @ " + dateTimeNow;
                _selectedTabScriptActions.Items.Add(CreateScriptCommandListViewItem(addCodeCommentCommand));

                //import
                PopulateExecutionCommands(deserializedScript.Commands);
                foreach (ScriptVariable var in deserializedScript.Variables)
                {
                    if (_scriptVariables.Find(alreadyExists => alreadyExists.VariableName == var.VariableName) == null)
                    {
                        _scriptVariables.Add(var);
                    }
                }

                foreach (ScriptElement elem in deserializedScript.Elements)
                {
                    if (_scriptElements.Find(alreadyExists => alreadyExists.ElementName == elem.ElementName) == null)
                    {
                        _scriptElements.Add(elem);
                    }
                }

                //comment
                dynamic codeCommentCommand = TypeMethods.CreateTypeInstance(AppDomain.CurrentDomain, "AddCodeCommentCommand");
                codeCommentCommand.v_Comment = "End Import From " + fileName + " @ " + dateTimeNow;
                _selectedTabScriptActions.Items.Add(CreateScriptCommandListViewItem(codeCommentCommand));

                //format listview
                //notify
                Notify("Script Imported Successfully!", Color.White);
            }
            catch (Exception ex)
            {
                //signal an error has happened
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        public void PopulateExecutionCommands(List<ScriptAction> commandDetails)
        {

            foreach (ScriptAction item in commandDetails)
            {
                if (item.ScriptCommand != null)
                    _selectedTabScriptActions.Items.Add(CreateScriptCommandListViewItem(item.ScriptCommand));
                else
                {
                    dynamic brokenCodeCommentCommand = TypeMethods.CreateTypeInstance(AppDomain.CurrentDomain, "BrokenCodeCommentCommand");
                    brokenCodeCommentCommand.v_Comment = item.SerializationError;
                    _selectedTabScriptActions.Items.Add(CreateScriptCommandListViewItem(brokenCodeCommentCommand));
                }
                if (item.AdditionalScriptCommands?.Count > 0)
                    PopulateExecutionCommands(item.AdditionalScriptCommands);
            }

            if (pnlCommandHelper.Visible)
            {
                uiScriptTabControl.SelectedTab.Controls.Remove(pnlCommandHelper);
                uiScriptTabControl.SelectedTab.Controls[0].Show();
            }
            else if (!uiScriptTabControl.SelectedTab.Controls[0].Visible)
                uiScriptTabControl.SelectedTab.Controls[0].Show();
        }

        #region Restart And Close Buttons
        private void restartApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
       
        private void uiBtnRestart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void closeApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void uiBtnClose_Click(object sender, EventArgs e)
        {
            if (_isSequence)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            Application.Exit();
        }
        #endregion
        #endregion

        #region Options Tool Strip and Buttons
        private void variablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenVariableManager();
        }

        private void uiBtnAddVariable_Click(object sender, EventArgs e)
        {
            OpenVariableManager();
        }

        private void OpenVariableManager()
        {
            frmScriptVariables scriptVariableEditor = new frmScriptVariables
            {
                ScriptName = uiScriptTabControl.SelectedTab.Name,
                ScriptVariables = _scriptVariables
            };

            if (scriptVariableEditor.ShowDialog() == DialogResult.OK)
            {
                _scriptVariables = scriptVariableEditor.ScriptVariables;
                if (!uiScriptTabControl.SelectedTab.Text.Contains(" *"))
                    uiScriptTabControl.SelectedTab.Text += " *";
            }
        }

        private void elementManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenElementManager();
        }

        private void uiBtnAddElement_Click(object sender, EventArgs e)
        {
            OpenElementManager();
        }

        private void OpenElementManager()
        {
            frmScriptElements scriptElementEditor = new frmScriptElements
            {
                ScriptName = uiScriptTabControl.SelectedTab.Name,
                ScriptElements = _scriptElements
            };

            if (scriptElementEditor.ShowDialog() == DialogResult.OK)
            {
                CreateUndoSnapshot();
                _scriptElements = scriptElementEditor.ScriptElements;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSettingsManager();
        }

        private void uiBtnSettings_Click(object sender, EventArgs e)
        {
            OpenSettingsManager();
        }

        private void OpenSettingsManager()
        {
            //show settings dialog
            frmSettings newSettings = new frmSettings(this);
            newSettings.ShowDialog();

            //reload app settings
            _appSettings = new ApplicationSettings();
            _appSettings = _appSettings.GetOrCreateApplicationSettings();
        }

        private void showSearchBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //set to empty
            tsSearchResult.Text = "";
            tsSearchBox.Text = "";

            //show or hide
            tsSearchBox.Visible = !tsSearchBox.Visible;
            tsSearchButton.Visible = !tsSearchButton.Visible;
            tsSearchResult.Visible = !tsSearchResult.Visible;

            //update verbiage
            if (tsSearchBox.Visible)
            {
                showSearchBarToolStripMenuItem.Text = "Hide Search Bar";
            }
            else
            {
                showSearchBarToolStripMenuItem.Text = "Show Search Bar";
            }
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uiBtnClearAll_Click(sender, e);
        }

        private void uiBtnClearAll_Click(object sender, EventArgs e)
        {
            CreateUndoSnapshot();
            HideSearchInfo();
            _selectedTabScriptActions.Items.Clear();
        }

        private void aboutOpenBotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frmAboutForm = new frmAbout();
            frmAboutForm.Show();
        }
        #endregion

        #region Script Events Tool Strip and Buttons
        

        private void scheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmScheduleManagement scheduleManager = new frmScheduleManagement();
            scheduleManager.Show();
        }

        private void uiBtnScheduleManagement_Click(object sender, EventArgs e)
        {
            frmScheduleManagement scheduleManager = new frmScheduleManagement();
            scheduleManager.Show();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsScriptRunning)
                return;

            saveToolStripMenuItem_Click(null, null);
            _isDebugMode = true;
            RunScript();
        }

        private void uiBtnDebugScript_Click(object sender, EventArgs e)
        {
            debugToolStripMenuItem_Click(sender, e);
        }

        private void RunScript()
        {
            if (_selectedTabScriptActions.Items.Count == 0)
            {
                Notify("You must first build the script by adding commands!", Color.Yellow);
                return;
            }

            if (ScriptFilePath == null)
            {
                Notify("You must first save your script before you can run it!", Color.Yellow);
                return;
            }

            SaveAllFiles();

            Notify("Running Script..", Color.White);

            if (CurrentEngine != null)
                ((Form)CurrentEngine).Close();

            //initialize Logger
            switch (_appSettings.EngineSettings.LoggingSinkType)
            {
                case SinkType.File:
                    if (string.IsNullOrEmpty(_appSettings.EngineSettings.LoggingValue1.Trim()))
                        _appSettings.EngineSettings.LoggingValue1 = Path.Combine(Folders.GetFolder(FolderType.LogFolder), "OpenBots Engine Logs.txt");

                    EngineLogger = new Logging().CreateFileLogger(_appSettings.EngineSettings.LoggingValue1, Serilog.RollingInterval.Day,
                        _appSettings.EngineSettings.MinLogLevel);
                    break;
                case SinkType.HTTP:
                    EngineLogger = new Logging().CreateHTTPLogger(ScriptProject.ProjectName, _appSettings.EngineSettings.LoggingValue1, _appSettings.EngineSettings.MinLogLevel);
                    break;
                case SinkType.SignalR:
                    string[] groupNames = _appSettings.EngineSettings.LoggingValue3.Split(',').Select(x => x.Trim()).ToArray();
                    string[] userIDs = _appSettings.EngineSettings.LoggingValue4.Split(',').Select(x => x.Trim()).ToArray();

                    EngineLogger = new Logging().CreateSignalRLogger(ScriptProject.ProjectName, _appSettings.EngineSettings.LoggingValue1, _appSettings.EngineSettings.LoggingValue2,
                        groupNames, userIDs, _appSettings.EngineSettings.MinLogLevel);
                    break;
            }

            //initialize Engine
            CurrentEngine = new frmScriptEngine(ScriptFilePath, ScriptProjectPath, this, EngineLogger, null, null, null, false, _isDebugMode);

            //executionManager = new ScriptExectionManager();
            //executionManager.CurrentlyExecuting = true;
            //executionManager.ScriptName = new System.IO.FileInfo(ScriptFilePath).Name;

            CurrentEngine.CallBackForm = this;
            ((frmScriptEngine)CurrentEngine).Show();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsScriptRunning)
                return;

            saveToolStripMenuItem_Click(null, null);
            _isDebugMode = false;
            RunScript();
        }

        private void uiBtnRunScript_Click(object sender, EventArgs e)
        {
            runToolStripMenuItem_Click(sender, e);
        }

        private void breakpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRemoveBreakpoint();
        }

        private void uiBtnBreakpoint_Click(object sender, EventArgs e)
        {
            AddRemoveBreakpoint();
        }
        #endregion

        #region Recorder Buttons
        private void elementRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmWebElementRecorder elementRecorder = new frmWebElementRecorder(HTMLElementRecorderURL)
            {
                CallBackForm = this,
                IsRecordingSequence = true,
                ScriptElements = _scriptElements
            };
            elementRecorder.chkStopOnClick.Visible = false;
            elementRecorder.IsCommandItemSelected = _selectedTabScriptActions.SelectedItems.Count > 0;

            CreateUndoSnapshot();

            elementRecorder.ShowDialog();

            HTMLElementRecorderURL = elementRecorder.StartURL;
            _scriptElements = elementRecorder.ScriptElements;           
        }

        private void uiBtnRecordElementSequence_Click(object sender, EventArgs e)
        {
            elementRecorderToolStripMenuItem_Click(sender, e);
        }

        private void uiRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecordSequence();
        }

        private void uiBtnRecordUISequence_Click(object sender, EventArgs e)
        {
            RecordSequence();
        }

        private void RecordSequence()
        {
            Hide();
            frmScreenRecorder sequenceRecorder = new frmScreenRecorder
            {
                CallBackForm = this,
                IsCommandItemSelected = _selectedTabScriptActions.SelectedItems.Count > 0
            };

            sequenceRecorder.ShowDialog();
            uiScriptTabControl.SelectedTab.Controls.Remove(pnlCommandHelper);
            uiScriptTabControl.SelectedTab.Controls[0].Show();

            Show();
            BringToFront();
        }

        private void uiAdvancedRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();

            frmAdvancedUIElementRecorder appElementRecorder = new frmAdvancedUIElementRecorder
            {
                CallBackForm = this,
                IsRecordingSequence = true
            };
            appElementRecorder.chkStopOnClick.Visible = false;
            appElementRecorder.IsCommandItemSelected = _selectedTabScriptActions.SelectedItems.Count > 0;

            CreateUndoSnapshot();

            appElementRecorder.ShowDialog();

            Show();
            BringToFront();
        }

        private void uiBtnRecordAdvancedUISequence_Click(object sender, EventArgs e)
        {
            uiAdvancedRecorderToolStripMenuItem_Click(sender, e);
        }

        private void uiBtnSaveSequence_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void uiBtnRenameSequence_Click(object sender, EventArgs e)
        {
            frmInputBox renameSequence = new frmInputBox("New Sequence Name", "Rename Sequence");
            renameSequence.txtInput.Text = Text;
            renameSequence.ShowDialog();

            if (renameSequence.DialogResult == DialogResult.OK)
                Text = renameSequence.txtInput.Text;
        }

        private void shortcutMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShortcutMenu shortcutMenuForm = new frmShortcutMenu();
            shortcutMenuForm.Show();
        }

        private void openShortcutMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shortcutMenuToolStripMenuItem_Click(sender, e);
        }
        #endregion
        #endregion
    }
}
