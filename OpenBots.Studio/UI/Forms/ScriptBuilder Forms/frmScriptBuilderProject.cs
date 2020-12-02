using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OpenBots.Core.Command;
using OpenBots.Core.Gallery;
using OpenBots.Core.Project;
using OpenBots.Core.Script;
using OpenBots.Nuget;
using OpenBots.Studio.Utilities;
using OpenBots.UI.CustomControls.CustomUIControls;
using OpenBots.UI.Forms.Supplement_Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VBFileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace OpenBots.UI.Forms.ScriptBuilder_Forms
{
    public partial class frmScriptBuilder : Form
    {

        #region Project Tool Strip, Buttons and Pane
        private async void addProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await AddProject();
        }

        private async void uiBtnProject_Click(object sender, EventArgs e)
        {
            await AddProject();
        }

        public async Task<DialogResult> AddProject()
        {
            tvProject.Nodes.Clear();
            var projectBuilder = new frmProjectBuilder();
            projectBuilder.ShowDialog();

            //Close OpenBots if add project form is closed at startup
            if (projectBuilder.DialogResult == DialogResult.Cancel && ScriptProject == null)
            {
                Application.Exit();
                return DialogResult.Abort;
            }

            //Create new OpenBots project
            else if (projectBuilder.Action == frmProjectBuilder.ProjectAction.CreateProject)
            {
                DialogResult result = CheckForUnsavedScripts();
                if (result == DialogResult.Cancel)
                    return DialogResult.Cancel;
              
                uiScriptTabControl.TabPages.Clear();
                ScriptProjectPath = projectBuilder.NewProjectPath;

                //Create new project
                ScriptProject = new Project(projectBuilder.NewProjectName);
                string configPath = Path.Combine(ScriptProjectPath, "project.config");

                //create config file
                File.WriteAllText(configPath, JsonConvert.SerializeObject(ScriptProject));

                var assemblyList = await NugetPackageManager.LoadProjectAssemblies(configPath);
                _builder = AppDomainSetupManager.LoadBuilder(assemblyList);
                _container = _builder.Build();
                         
                string mainScriptPath = Path.Combine(ScriptProjectPath, "Main.json");
                string mainScriptName = Path.GetFileNameWithoutExtension(mainScriptPath);
                UIListView mainScriptActions = NewLstScriptActions(mainScriptName);
                List<ScriptVariable> mainScriptVariables = new List<ScriptVariable>();
                List<ScriptElement> mainScriptElements = new List<ScriptElement>();

                try
                {
                    dynamic helloWorldCommand = TypeMethods.CreateTypeInstance(_container, "ShowMessageCommand");
                    helloWorldCommand.v_Message = "Hello World";
                    mainScriptActions.Items.Insert(0, CreateScriptCommandListViewItem(helloWorldCommand));
                }
                catch (Exception)
                {
                    var brokenHelloWorldCommand = new BrokenCodeCommentCommand();
                    brokenHelloWorldCommand.v_Comment = "Hello World";
                    mainScriptActions.Items.Insert(0, CreateScriptCommandListViewItem(brokenHelloWorldCommand));
                }
                
                //Begin saving as main.xml
                ClearSelectedListViewItems();

                try
                {                    
                    //Serialize main script
                    var mainScript = Script.SerializeScript(mainScriptActions.Items, mainScriptVariables, mainScriptElements,
                                                            mainScriptPath);
                    
                    _mainFileName = ScriptProject.Main;
                   
                    OpenFile(mainScriptPath);
                    ScriptFilePath = mainScriptPath;

                    //Show success dialog
                    Notify("Project has been created successfully!", Color.White);
                }
                catch (Exception ex)
                {
                    Notify("An Error Occured: " + ex.Message, Color.Red);
                }
            }

            //Open existing OpenBots project
            else if (projectBuilder.Action == frmProjectBuilder.ProjectAction.OpenProject)
            {
                DialogResult result = CheckForUnsavedScripts();
                if (result == DialogResult.Cancel)
                    return DialogResult.Cancel;

                try
                {
                    //Open project
                    ScriptProject = Project.OpenProject(projectBuilder.ExistingConfigPath);

                    var assemblyList = await NugetPackageManager.LoadProjectAssemblies(projectBuilder.ExistingConfigPath);
                    _builder = AppDomainSetupManager.LoadBuilder(assemblyList);
                    _container = _builder.Build();

                    _mainFileName = ScriptProject.Main;

                    string mainFilePath = Directory.GetFiles(projectBuilder.ExistingProjectPath, _mainFileName, SearchOption.AllDirectories).FirstOrDefault();
                    if (mainFilePath == null)
                        throw new Exception("Main script not found");

                    ScriptProjectPath = projectBuilder.ExistingProjectPath;
                    uiScriptTabControl.TabPages.Clear();

                    //Open Main
                    OpenFile(mainFilePath);
                    //show success dialog
                    Notify("Project has been opened successfully!", Color.White);
                }
                catch (Exception ex)
                {
                    //show fail dialog
                    Notify("An Error Occured: " + ex.Message, Color.Red);
                    //Try adding project again
                    await AddProject();
                    return DialogResult.None;
                }
            }

            DirectoryInfo projectDirectoryInfo = new DirectoryInfo(ScriptProjectPath);
            TreeNode projectNode = new TreeNode(projectDirectoryInfo.Name);
            projectNode.Text = projectDirectoryInfo.Name;
            projectNode.Tag = projectDirectoryInfo.FullName;
            projectNode.Nodes.Add("Empty");
            projectNode.ContextMenuStrip = cmsProjectMainFolderActions;          
            tvProject.Nodes.Add(projectNode);
            projectNode.Expand();
            LoadCommands();
            return DialogResult.OK;
        }

        private void LoadChildren(TreeNode parentNode, string directory)
        {
            DirectoryInfo parentDirectoryInfo = new DirectoryInfo(directory);
            try
            {
                foreach (DirectoryInfo childDirectoryInfo in parentDirectoryInfo.GetDirectories())
                {
                    if (childDirectoryInfo.Attributes != FileAttributes.Hidden)
                        NewNode(parentNode, childDirectoryInfo.FullName, "folder");
                }
                foreach (FileInfo fileInfo in parentDirectoryInfo.GetFiles())
                {
                    if (fileInfo.Attributes != FileAttributes.Hidden)
                        NewNode(parentNode, fileInfo.FullName, "file");
                }
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void NewNode(TreeNode parentNode, string childPath, string type)
        {
            if (type == "folder")
            {
                DirectoryInfo childDirectoryInfo = new DirectoryInfo(childPath);

                TreeNode innerFolderNode = new TreeNode(childDirectoryInfo.Name);
                innerFolderNode.Name = childDirectoryInfo.Name;
                innerFolderNode.Text = childDirectoryInfo.Name;
                innerFolderNode.Tag = childDirectoryInfo.FullName;
                innerFolderNode.Nodes.Add("Empty");
                innerFolderNode.ContextMenuStrip = cmsProjectFolderActions;
                innerFolderNode.ImageIndex = 0; //folder icon
                innerFolderNode.SelectedImageIndex = 0;
                parentNode.Nodes.Add(innerFolderNode);
            }
            else if (type == "file")
            {
                FileInfo childFileInfo = new FileInfo(childPath);

                TreeNode fileNode = new TreeNode(childFileInfo.Name);
                fileNode.Name = childFileInfo.Name;
                fileNode.Text = childFileInfo.Name;
                fileNode.Tag = childFileInfo.FullName;
                
                if (fileNode.Name != "project.config")
                    fileNode.ContextMenuStrip = cmsProjectFileActions;

                if (fileNode.Tag.ToString().ToLower().Contains(".json"))
                {
                    fileNode.ImageIndex = 1; //script file icon
                    fileNode.SelectedImageIndex = 1;
                }
                else if (fileNode.Tag.ToString().ToLower().Contains(".xlsx") ||
                         fileNode.Tag.ToString().ToLower().Contains(".csv"))
                {
                    fileNode.ImageIndex = 3; //excel file icon
                    fileNode.SelectedImageIndex = 3;
                }
                else if (fileNode.Tag.ToString().ToLower().Contains(".docx"))
                {
                    fileNode.ImageIndex = 4; //word file icon
                    fileNode.SelectedImageIndex = 4;
                }
                else if (fileNode.Tag.ToString().ToLower().Contains(".pdf"))
                {
                    fileNode.ImageIndex = 5; //pdf file icon
                    fileNode.SelectedImageIndex = 5;
                }
                else
                {
                    fileNode.ImageIndex = 2; //default file icon
                    fileNode.SelectedImageIndex = 2;
                }

                parentNode.Nodes.Add(fileNode);
            }
        }
        #endregion

        #region Project TreeView Events
        private void tvProject_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (Directory.Exists(e.Node.Tag.ToString()))
            {
                e.Node.Nodes.Clear();
                LoadChildren(e.Node, e.Node.Tag.ToString());
            }
            else
                e.Cancel = true;
        }

        private void tvProject_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (IsScriptRunning)
                return;

            if (e == null || e.Button == MouseButtons.Left)
            {
                try
                {
                    string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                    string currentOpenScriptFilePath = _scriptFilePath;

                    if (File.Exists(selectedNodePath) && selectedNodePath.ToLower().Contains(".json"))
                        OpenFile(selectedNodePath);
                    else if (File.Exists(selectedNodePath))
                        Process.Start(selectedNodePath);
                }
                catch (Exception ex)
                {
                    Notify("An Error Occured: " + ex.Message, Color.Red);
                }
            }
        }

        private void tvProject_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvProject.SelectedNode = e.Node;
        }

        private void tvProject_KeyDown(object sender, KeyEventArgs e)
        {
            string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
            bool isFolder;
            if (Directory.Exists(selectedNodePath))
                isFolder = true;
            else
                isFolder = false;
            if (e.KeyCode == Keys.Delete && isFolder)
                tsmiDeleteFolder_Click(sender, e);
            else if (e.KeyCode == Keys.Delete && !isFolder)
                tsmiDeleteFile_Click(sender, e);
            else if (e.KeyCode == Keys.Enter && !isFolder)
                tvProject_DoubleClick(sender, null);
            else if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                    tsmiCopyFolder_Click(sender, e);
                if (e.KeyCode == Keys.V)
                    tsmiPasteFolder_Click(sender, e);
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        #endregion

        #region Project Folder Context Menu Strip
        private void tsmiCopyFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                Clipboard.SetData(DataFormats.Text, selectedNodePath);
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void tsmiDeleteFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                string selectedNodeName = tvProject.SelectedNode.Text.ToString();
                if (selectedNodeName != ScriptProject.ProjectName)
                {
                    DialogResult result = MessageBox.Show($"Are you sure you would like to delete {selectedNodeName}?",
                                                 $"Delete {selectedNodeName}", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        if (Directory.Exists(selectedNodePath))
                        {
                            Directory.Delete(selectedNodePath, true);
                            tvProject.Nodes.Remove(tvProject.SelectedNode);
                        }
                        else
                            throw new FileNotFoundException();
                    }
                }
                else
                {
                    throw new Exception($"Cannot delete {selectedNodeName}");
                }
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void tsmiNewFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string newName = "";
                var newNameForm = new frmInputBox("Enter the name of the new folder", "New Folder");
                newNameForm.txtInput.Text = tvProject.SelectedNode.Name;
                newNameForm.ShowDialog();

                if (newNameForm.DialogResult == DialogResult.OK)
                    newName = newNameForm.txtInput.Text;
                else if (newNameForm.DialogResult == DialogResult.Cancel)
                    return;

                if (newName.EndsWith(".json"))
                    throw new Exception("Invalid folder name");

                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                string newFolderPath = Path.Combine(selectedNodePath, newName);

                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                    DirectoryInfo newDirectoryInfo = new DirectoryInfo(newFolderPath);
                    NewNode(tvProject.SelectedNode, newFolderPath, "folder");
                }
                else
                {
                    int count = 1;
                    string newerFolderPath = newFolderPath;
                    while (Directory.Exists(newerFolderPath))
                    {
                        newerFolderPath = $"{newFolderPath} ({count})";
                        count += 1;
                    }
                    Directory.CreateDirectory(newerFolderPath);
                    DirectoryInfo newDirectoryInfo = new DirectoryInfo(newerFolderPath);

                    NewNode(tvProject.SelectedNode, newerFolderPath, "folder");
                }
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void tsmiPasteFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                string copiedNodePath = Clipboard.GetData(DataFormats.Text).ToString();

                if (Directory.Exists(copiedNodePath))
                {
                    DirectoryInfo copiedNodeDirectoryInfo = new DirectoryInfo(copiedNodePath);

                    if (Directory.Exists(Path.Combine(selectedNodePath, copiedNodeDirectoryInfo.Name)))
                        throw new Exception("A directory with this name already exists in this location");

                    else if (copiedNodePath == ScriptProjectPath)
                        throw new Exception("The project directory cannot be copied or moved");

                    else
                    {
                        VBFileSystem.CopyDirectory(copiedNodePath, Path.Combine(selectedNodePath, copiedNodeDirectoryInfo.Name));
                        NewNode(tvProject.SelectedNode, copiedNodePath, "folder");
                    }
                }
                else if (File.Exists(copiedNodePath))
                {
                    FileInfo copiedNodeFileInfo = new FileInfo(copiedNodePath);

                    if (File.Exists(Path.Combine(selectedNodePath, copiedNodeFileInfo.Name)))
                        throw new Exception("A file with this name already exists in this location");

                    else if (copiedNodeFileInfo.Name == "project.config")
                        throw new Exception("This file cannot be copied or moved");

                    else
                    {
                        File.Copy(copiedNodePath, Path.Combine(selectedNodePath, copiedNodeFileInfo.Name));
                        NewNode(tvProject.SelectedNode, copiedNodePath, "file");
                    }
                }
                else
                    throw new Exception("Attempted to paste something that isn't a file or folder");

            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void tsmiRenameFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                if (selectedNodePath != ScriptProjectPath)
                {
                    DirectoryInfo selectedNodeDirectoryInfo = new DirectoryInfo(selectedNodePath);

                    string newName = "";
                    var newNameForm = new frmInputBox("Enter the new name of the folder", "Rename Folder");
                    newNameForm.txtInput.Text = tvProject.SelectedNode.Name;
                    newNameForm.ShowDialog();

                    if (newNameForm.DialogResult == DialogResult.OK)
                        newName = newNameForm.txtInput.Text;
                    else if (newNameForm.DialogResult == DialogResult.Cancel)
                        return;

                    string newPath = Path.Combine(selectedNodeDirectoryInfo.Parent.FullName, newName);
                    bool isInvalidProjectName = new[] { @"/", @"\" }.Any(c => newName.Contains(c));

                    if (isInvalidProjectName)
                        throw new Exception("Illegal characters in path");

                    if (Directory.Exists(newPath))
                        throw new Exception("A folder with this name already exists");

                    FileSystem.Rename(selectedNodePath, newPath);
                    tvProject.SelectedNode.Name = newName;
                    tvProject.SelectedNode.Text = newName;
                    tvProject.SelectedNode.Tag = newPath;
                }
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }

        }
        private void tsmiNewScriptFile_Click(object sender, EventArgs e)
        {
            try
            {               
                string newName = "";
                var newNameForm = new frmInputBox("Enter the name of the new file without extension", "New File");
                newNameForm.txtInput.Text = tvProject.SelectedNode.Name;
                newNameForm.ShowDialog();

                if (newNameForm.DialogResult == DialogResult.OK)
                    newName = newNameForm.txtInput.Text;
                else if (newNameForm.DialogResult == DialogResult.Cancel)
                    return;

                if (newName.EndsWith(".json"))
                    throw new Exception("Invalid file name");

                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                string newFilePath = Path.Combine(selectedNodePath, newName + ".json");
                UIListView newScriptActions = NewLstScriptActions();
                List<ScriptVariable> newScripVariables = new List<ScriptVariable>();
                List<ScriptElement> newScriptElements = new List<ScriptElement>();
                dynamic helloWorldCommand = TypeMethods.CreateTypeInstance(_container, "ShowMessageCommand");
                helloWorldCommand.v_Message = "Hello World";
                newScriptActions.Items.Insert(0, CreateScriptCommandListViewItem(helloWorldCommand));

                if (!File.Exists(newFilePath))
                {
                    Script.SerializeScript(newScriptActions.Items, newScripVariables, newScriptElements, newFilePath);
                    NewNode(tvProject.SelectedNode, newFilePath, "file");
                    OpenFile(newFilePath);
                }
                else
                {
                    int count = 1;
                    string newerFilePath = newFilePath;
                    while (File.Exists(newerFilePath))
                    {
                        string newDirectoryPath = Path.GetDirectoryName(newFilePath);
                        string newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFilePath);
                        newerFilePath = Path.Combine(newDirectoryPath, $"{newFileNameWithoutExtension} ({count}).json");
                        count += 1;
                    }
                    Script.SerializeScript(newScriptActions.Items, newScripVariables, newScriptElements, newerFilePath);
                    NewNode(tvProject.SelectedNode, newerFilePath, "file");
                    OpenFile(newerFilePath);
                }

            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }
        #endregion

        #region Project File Context Menu Strip
        private void tsmiCopyFile_Click(object sender, EventArgs e)
        {
            tsmiCopyFolder_Click(sender, e);
        }

        private void tsmiDeleteFile_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                string selectedNodeName = tvProject.SelectedNode.Text.ToString();
                if (selectedNodeName != "project.config")
                {
                    var result = MessageBox.Show($"Are you sure you would like to delete {selectedNodeName}?",
                                             $"Delete {selectedNodeName}", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(selectedNodePath))
                        {
                            string selectedFileName = Path.GetFileNameWithoutExtension(selectedNodePath);
                            File.Delete(selectedNodePath);
                            tvProject.Nodes.Remove(tvProject.SelectedNode);
                            var foundTab = uiScriptTabControl.TabPages.Cast<TabPage>()
                                                                      .Where(t => t.ToolTipText == selectedNodePath)
                                                                      .FirstOrDefault();
                            if (foundTab != null)
                                uiScriptTabControl.TabPages.Remove(foundTab);
                        }
                        else
                            throw new FileNotFoundException();
                    }
                }
                else
                    throw new Exception($"Cannot delete {selectedNodeName}");
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }

        private void tsmiRenameFile_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedNodePath = tvProject.SelectedNode.Tag.ToString();
                string selectedNodeName = tvProject.SelectedNode.Text.ToString();
                string selectedNodeNameWithoutExtension = Path.GetFileNameWithoutExtension(selectedNodeName);
                string selectedNodeFileExtension = Path.GetExtension(selectedNodePath);

                if (selectedNodeName != "project.config")
                {
                    FileInfo selectedNodeDirectoryInfo = new FileInfo(selectedNodePath);

                    string newNameWithoutExtension = "";
                    var newNameForm = new frmInputBox("Enter the new name of the file without extension", "Rename File");
                    newNameForm.txtInput.Text = Path.GetFileNameWithoutExtension(selectedNodeDirectoryInfo.Name);
                    newNameForm.ShowDialog();

                    if (newNameForm.DialogResult == DialogResult.OK)
                        newNameWithoutExtension = newNameForm.txtInput.Text;
                    else if (newNameForm.DialogResult == DialogResult.Cancel)
                        return;

                    string newName = newNameWithoutExtension + selectedNodeFileExtension;
                    string newPath = Path.Combine(selectedNodeDirectoryInfo.DirectoryName, newName);

                    bool isInvalidProjectName = new[] { @"/", @"\" }.Any(c => newNameWithoutExtension.Contains(c));
                    if (isInvalidProjectName)
                        throw new Exception("Illegal characters in path");

                    if (File.Exists(newPath))
                        throw new Exception("A file with this name already exists");

                    var foundTab = uiScriptTabControl.TabPages.Cast<TabPage>().Where(t => t.ToolTipText == selectedNodePath)
                                                                          .FirstOrDefault();

                    if (foundTab != null)
                    {
                        DialogResult result = CheckForUnsavedScript(foundTab);
                        if (result == DialogResult.Cancel)
                            return;

                        uiScriptTabControl.TabPages.Remove(foundTab);
                    }

                    FileSystem.Rename(selectedNodePath, newPath);

                    if (selectedNodeName == _mainFileName)
                    {
                        string newMainName = Path.GetFileName(newPath);
                        _mainFileName = newMainName;
                        ScriptProject.Main = newMainName;
                        ScriptProject.SaveProject(newPath);
                    }

                    tvProject.SelectedNode.Name = newName;
                    tvProject.SelectedNode.Text = newName;
                    tvProject.SelectedNode.Tag = newPath;                   
                }
            }
            catch (Exception ex)
            {
                Notify("An Error Occured: " + ex.Message, Color.Red);
            }
        }
        #endregion

        #region Project Pane Buttons
        private void uiBtnRefresh_Click(object sender, EventArgs e)
        {
            tvProject.CollapseAll();
            tvProject.TopNode.Expand();
        }

        private void uiBtnExpand_Click(object sender, EventArgs e)
        {
            tvProject.ExpandAll();
        }

        private void uiBtnCollapse_Click(object sender, EventArgs e)
        {
            tvProject.CollapseAll();
        }

        private void uiBtnOpenDirectory_Click(object sender, EventArgs e)
        {
            Process.Start(ScriptProjectPath);
        }
        #endregion
    }
}
