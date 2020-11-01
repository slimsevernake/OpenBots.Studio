using OpenBots.Core.Infrastructure;
using OpenBots.Core.Script;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.UI.Forms.Supplement_Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.ScriptBuilder_Forms
{
    public partial class frmScriptBuilder : Form, IfrmScriptBuilder
    {
        public delegate void CreateDebugTabDelegate();
        private void CreateDebugTab()
        {
            if (InvokeRequired)
            {
                var d = new CreateDebugTabDelegate(CreateDebugTab);
                Invoke(d, new object[] { });
            }
            else
            {
                TabPage debugTab = uiPaneTabs.TabPages.Cast<TabPage>().Where(t => t.Name == "DebugVariables")
                                                                              .FirstOrDefault();

                if (debugTab == null)
                {
                    debugTab = new TabPage();
                    debugTab.Name = "DebugVariables";
                    debugTab.Text = "Variables";
                    uiPaneTabs.TabPages.Add(debugTab);
                    uiPaneTabs.SelectedTab = debugTab;
                }
                LoadDebugTab(debugTab);
            }          
        }

        public delegate void LoadDebugTabDelegate(TabPage debugTab);
        private void LoadDebugTab(TabPage debugTab)
        {
            if (InvokeRequired)
            {
                var d = new LoadDebugTabDelegate(LoadDebugTab);
                Invoke(d, new object[] { debugTab });
            }
            else
            {
                DataTable variableValues = new DataTable();
                variableValues.Columns.Add("Name");
                variableValues.Columns.Add("Type");
                variableValues.Columns.Add("Value");
                variableValues.TableName = "VariableValuesDataTable" + DateTime.Now.ToString("MMddyyhhmmss");

                DataGridView variablesGridViewHelper = new DataGridView();
                variablesGridViewHelper.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                variablesGridViewHelper.Dock = DockStyle.Fill;
                variablesGridViewHelper.ColumnHeadersHeight = 30;
                variablesGridViewHelper.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                variablesGridViewHelper.AllowUserToAddRows = false;
                variablesGridViewHelper.AllowUserToDeleteRows = false;
                variablesGridViewHelper.ReadOnly = true;

                if (debugTab.Controls.Count != 0)
                    debugTab.Controls.RemoveAt(0);
                debugTab.Controls.Add(variablesGridViewHelper);

                List<ScriptVariable> engineVariables = ((frmScriptEngine)CurrentEngine).EngineInstance.VariableList;
                foreach (var variable in engineVariables)
                {
                    DataRow[] foundVariables = variableValues.Select("Name = '" + variable.VariableName + "'");
                    if (foundVariables.Length == 0)
                    {
                        string type = "null";
                        if (variable.VariableValue != null)
                            type = variable.VariableValue.GetType().FullName;

                        variableValues.Rows.Add(variable.VariableName, type, StringMethods.ConvertObjectToString(variable.VariableValue));                     
                    }
                }
                variablesGridViewHelper.DataSource = variableValues;
                uiPaneTabs.SelectedTab = debugTab;
            }           
        }

        public delegate void RemoveDebugTabDelegate();
        public void RemoveDebugTab()
        {
            if (InvokeRequired)
            {
                var d = new RemoveDebugTabDelegate(RemoveDebugTab);
                Invoke(d, new object[] { });
            }
            else
            {
                TabPage debugTab = uiPaneTabs.TabPages.Cast<TabPage>().Where(t => t.Name == "DebugVariables")
                                                                              .FirstOrDefault();

                if (debugTab != null)
                    uiPaneTabs.TabPages.Remove(debugTab);
            }
        }

        public delegate DialogResult LoadErrorFormDelegate(string errorMessage);
        public DialogResult LoadErrorForm(string errorMessage)
        {
            if (InvokeRequired)
            {
                var d = new LoadErrorFormDelegate(LoadErrorForm);
                return (DialogResult)Invoke(d, new object[] { errorMessage });
            }
            else
            {
                frmError errorForm = new frmError(errorMessage);
                errorForm.Owner = this;
                errorForm.ShowDialog();
                return errorForm.DialogResult;
            }          
        }
    }
}
