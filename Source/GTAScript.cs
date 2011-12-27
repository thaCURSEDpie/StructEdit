//=======================================================================
//
// <copyright file="GTAScript.cs" company="not applicable">
//     Copyright (c) thaCURSEDpie. All rights reserved.
// </copyright>
//
//-----------------------------------------------------------------------
//          File:           GTAScript.cs
//          Version:        Pre-Alpha
//          Part of:        StructEdit mod
//          Author:         thaCURSEDpie
//          Date:           December 2011
//          Description:
//              This file contains the GTAScript class,
//              which inherits GTA.Script and provides the actual
//              script functionality.
//
//=======================================================================

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using GTA;

    /// <summary>
    /// The mod's script.
    /// </summary>
    public unsafe class GTAScript : Script
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Initializes a new instance of the <see cref="GTAScript"/> class.
        /// </summary>
        public GTAScript()
        {
            GUID = new Guid("785C7FF8-2F0A-11E1-BD9F-749C4824019B");

            GlobalVars.Structures = new List<CEditableStruct>();
            GlobalVars.BaseAddress = (int)GetModuleHandle(null);

            this.KeyDown += new GTA.KeyEventHandler(this.GTAScript_KeyDown);
            this.Tick += new EventHandler(this.GTAScript_Tick);


            this.BindScriptCommand("si_setparamvalue", new ScriptCommandDelegate(this.setParamValue_scriptCmd));
            this.BindScriptCommand("si_getparamvalue", new ScriptCommandDelegate(this.getParamValue_scriptCmd));
            this.BindScriptCommand("si_getparamnum", new ScriptCommandDelegate(this.getParamNum_scriptCmd));
            this.BindScriptCommand("si_getelementsnum", new ScriptCommandDelegate(this.getElementsNum_scriptCmd));
            this.BindScriptCommand("si_getparamname", new ScriptCommandDelegate(this.getParamName_scriptCmd));
            this.BindScriptCommand("si_getstructnum", new ScriptCommandDelegate(this.getStructNum_scriptCmd));
            this.BindScriptCommand("si_getstructname", new ScriptCommandDelegate(this.getStructName_scriptCmd));

            this.BindConsoleCommand("si_regen", new ConsoleCommandDelegate(this.regenStructures_console));
            this.BindConsoleCommand("si_editparam", new ConsoleCommandDelegate(this.editStructureParam_console), " - Edits a structure's param value. usage: si_editparam <structure name> <element index> <param name> <new value>");
            this.BindConsoleCommand("si_printparams", new ConsoleCommandDelegate(this.printStructureParams_console), " - Prints a structure's parameters. usage: si_printparams <structure name>");
            this.BindConsoleCommand("si_printvalues", new ConsoleCommandDelegate(this.printStructureValues_console), " - Prints the parameter values for a certain element. usage: si_printvalues <structure name> <element index>");
            this.Interval = 500;

        }

        #region ScriptCmds
        private void getStructName_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            // si_getstructname <sender> <index>
            if (parameters.Count < 2)
            {
                // Error code 1: invalid parameter count.
                SendScriptCommand(sender, "si_getstructname_response", parameters[0], 1);
            }

            int index = parameters.Convert<int>(1);

            if (index < 0 || index >= GlobalVars.Structures.Count)
            {
                // Error code 2: invalid index
                SendScriptCommand(sender, "si_getstructname_response", parameters[0], 2);
            }

            SendScriptCommand(sender, "si_getstructname_response", parameters[0], GlobalVars.Structures[index].Name);
        }

        private void getStructNum_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            SendScriptCommand(sender, "si_getstructnum_response", parameters[0], GlobalVars.Structures.Count);
        }

        private void getParamName_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            
            // si_getparamnum <call identifier> <structure name> <param index>
            if (parameters.Count < 3)
            {
                // Error code -1: invalid parameter count
                SendScriptCommand(sender, "si_getparamname_response", parameters[0], 1);
                return;
            }

            string structName = parameters.Convert<string>(1);

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                // Error code -3: invalid struct name
                SendScriptCommand(sender, "si_getparamname_response", parameters[0], 3);
                return;
            }

            int paramIndex = parameters.Convert<int>(2);
            if (paramIndex < 0 || paramIndex >= tempStruct.NumParams)
            {
                // Error code -4: invalid parameter index
                SendScriptCommand(sender, "si_getparamname_response", parameters[0], 4);
            }
            
            SParameter tempParameter = new SParameter();
            tempStruct.GetGenericParamByIndex(paramIndex, out tempParameter);

            SendScriptCommand(sender, "si_getelementsnum_response", parameters[0], tempParameter.ParamName);
        }

        private void getElementsNum_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            // si_getparamnum <call identifier> <structure name>
            if (parameters.Count < 2)
            {
                // Error code 1: invalid parameter count
                SendScriptCommand(sender, "si_getelementsnum_response", parameters[0], 1);
                return;
            }

            string structName = parameters.Convert<string>(1);

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                // Error code 3: invalid struct name
                SendScriptCommand(sender, "si_getelementsnum_response", parameters[0], 3);
                return;
            }

            SendScriptCommand(sender, "si_getelementsnum_response", parameters[0], tempStruct.NumElements);
        }

        private void getParamNum_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            // si_getparamnum <call identifier> <structure name>
            if (parameters.Count < 2)
            {
                // Error code 1: invalid parameter count
                SendScriptCommand(sender, "si_getparamnum_response", parameters[0], 1);
                return;
            }

            string structName = parameters.Convert<string>(1);
            
            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                // Error code 3: invalid struct name
                SendScriptCommand(sender, "si_getparamnum_response", parameters[0], 3);
                return;
            }

            SendScriptCommand(sender, "si_getparamnum_response", parameters[0], tempStruct.NumParams);
        }

        private void getParamValue_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            // si_getparamvalue <call identifier> <structure name> <element index> <parameter name>
            if (parameters.Count < 4)
            {
                // Error code 1: invalid parameter count
                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 1);
                return;
            }

            string structName = parameters.Convert<string>(1);

            int index = 0;

            if (!int.TryParse(parameters.Convert<string>(2), out index))
            {
                // Error code 2: invalid index
                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 2);
                return;
            }

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                // Error code 3: invalid struct name
                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 3);
                return;
            }

            string paramName = parameters.Convert<string>(3);
            SParameter tempParam = new SParameter();

            if (tempStruct.GetGenericParamByName(paramName, ref tempParam) != 0)
            {
                // Error code 4: invalid param name
                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 4);
                return;
            }

            if (tempParam.Type == typeof(int))
            {
                int retVal = 0;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }
            else if (tempParam.Type == typeof(string))
            {
                string retVal = string.Empty;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }
            else if (tempParam.Type == typeof(char))
            {
                char retVal = char.MinValue;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }
            else if (tempParam.Type == typeof(float))
            {
                float retVal = 0f;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }
            else if (tempParam.Type == typeof(double))
            {
                double retVal = 0f;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }
            else if (tempParam.Type == typeof(short))
            {
                short retVal = 0;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }
            else if (tempParam.Type == typeof(long))
            {
                long retVal = 0;
                tempStruct.GetParamValue(index, paramName, ref retVal);

                SendScriptCommand(sender, "si_getparamvalue_response", parameters[0], 0, retVal);
            }

            return;
        }

        private void setParamValue_scriptCmd(GTA.Script sender, GTA.ObjectCollection parameters)
        {
            if (parameters.Count < 5)
            {
                // Error code 1: invalid parameter count
                SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 1);
                return;
            }

            string structName = parameters.Convert<string>(1);

            int index = 0;

            if (!int.TryParse(parameters.Convert<string>(2), out index))
            {
                // Error code 2: invalid index
                SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 2);
                return;
            }

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                // Error code 3: invalid struct name
                SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 3);
                return;
            }

            string paramName = parameters.Convert<string>(3);
            SParameter tempParam = new SParameter();

            if (tempStruct.GetGenericParamByName(paramName, ref tempParam) != 0)
            {
                // Error code 4: invalid param name
                SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 4);
                return;
            }

            if (tempParam.Type == typeof(int))
            {
                int newVal = parameters.Convert<int>(4);
                int retVal = tempStruct.SetParamValue<int>(index, paramName, newVal);

                if (retVal == -1)
                {
                    // Error code 5: invalid param index number
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                    return;
                }
            }
            else if (tempParam.Type == typeof(string))
            {
                string newVal = parameters.Convert<string>(4);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    // Error code 5: invalid param index number
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                    return;
                }
                else if (retVal == -3)
                {
                    // Error code 6: input string too long
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 6);
                    return;
                }
            }
            else if (tempParam.Type == typeof(char))
            {
                string newValString = parameters.Convert<string>(4);
                char newVal = '\0';
                int retVal = 0;

                if (newValString != string.Empty)
                {
                    newVal = newValString[0];
                    if (retVal == -1)
                    {
                        // Error code 5: invalid param index number
                        SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                        return;
                    }
                }
                else
                {
                    // Error code 6: invalid character entered
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 6);
                    return;
                }
            }
            else if (tempParam.Type == typeof(float))
            {
                float newVal = parameters.Convert<float>(4);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    // Error code 5: invalid param index number
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                    return;
                }
            }
            else if (tempParam.Type == typeof(double))
            {
                double newVal = parameters.Convert<float>(4);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    // Error code 5: invalid param index number
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                    return;
                }
            }
            else if (tempParam.Type == typeof(short))
            {
                short newVal = parameters.Convert<short>(4);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    // Error code 5: invalid param index number
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                    return;
                }
            }
            else if (tempParam.Type == typeof(long))
            {
                long newVal = parameters.Convert<long>(4);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    // Error code 5: invalid param index number
                    SendScriptCommand(sender, "si_setparamvalue_response", parameters[0], 5);
                    return;
                }
            }

            // Success! :D
            SendScriptCommand(sender, "si_setparamvalue_respons", parameters[0], 0);
        }
        #endregion


        /// <summary>
        /// Edits a structure's parameter value, using the console input.
        /// </summary>
        /// <param name="parameters">The console input..</param>
        private void editStructureParam_console(ParameterCollection parameters)
        {
            if (parameters.Count < 4)
            {
                Game.Console.Print("usage: si_editparam <structure name> <element index> <param name> <new value>");
                return;
            }

            string structName = parameters.ToString(0);

            int index = 0;

            if (!int.TryParse(parameters.ToString(1), out index))
            {
                Game.Console.Print("Invalid index specified!");
                return;
            }

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                Game.Console.Print("No struct found with name \"" + structName + "\"!");
                return;
            }

            string paramName = parameters.ToString(2);
            SParameter tempParam = new SParameter();

            if (tempStruct.GetGenericParamByName(paramName, ref tempParam) != 0)
            {
                Game.Console.Print("No parameter found with name \"" + paramName + "\"!");
                return;
            }

            if (tempParam.Type == typeof(int))
            {
                int newVal = parameters.ToInteger(3);
                int retVal = tempStruct.SetParamValue<int>(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else
                {
                    Game.Console.Print("Parameter changed");
                }
            }
            else if (tempParam.Type == typeof(uint))
            {
                uint newVal = uint.Parse(parameters.ToString(3));
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else
                {
                    Game.Console.Print("Parameter changed");
                }
            }
            else if (tempParam.Type == typeof(string))
            {
                string newVal = parameters.ToString(3);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else if (retVal == -3)
                {
                    Game.Console.Print("Input string too long! Max size: " + tempParam.StringSize.ToString() + ", your input: " + newVal.Length);
                    return;
                }
            }
            else if (tempParam.Type == typeof(char))
            {
                string newValString = parameters.ToString(3);
                char newVal = '\0';
                int retVal = 0;

                if (newValString != string.Empty)
                {
                    newVal = newValString[0];
                    if (retVal == -1)
                    {
                        Game.Console.Print("Invalid parameter index number");
                        return;
                    }
                    else
                    {
                        Game.Console.Print("Parameter changed");
                    }
                }
                else
                {
                    Game.Console.Print("Invalid char entered!");
                    return;
                }
            }
            else if (tempParam.Type == typeof(float))
            {
                float newVal = parameters.ToFloat(3);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else
                {
                    Game.Console.Print("Parameter changed");
                }
            }
            else if (tempParam.Type == typeof(double))
            {
                double newVal = parameters.ToFloat(3);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else
                {
                    Game.Console.Print("Parameter changed");
                }
            }
            else if (tempParam.Type == typeof(short))
            {
                short newVal = (short)parameters.ToInteger(3);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else
                {
                    Game.Console.Print("Parameter changed");
                }
            }
            else if (tempParam.Type == typeof(long))
            {
                long newVal = parameters.ToInteger(3);
                int retVal = tempStruct.SetParamValue(index, paramName, newVal);

                if (retVal == -1)
                {
                    Game.Console.Print("Invalid parameter index number");
                    return;
                }
                else
                {
                    Game.Console.Print("Parameter changed");
                }
            }
        }

        /// <summary>
        /// Prints a structure´s parameters to the console.
        /// </summary>
        /// <param name="parameters">The console input.</param>
        private void printStructureParams_console(ParameterCollection parameters)
        {
            if (parameters.Count == 0)
            {
                Game.Console.Print("usage: si_printparams <structure name>");
                return;
            }

            string structName = parameters.ToString(0);

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                Game.Console.Print("No struct found with name \"" + structName + "\"!");
                return;
            }

            Game.Console.Print("Listing parameters for structure \"" + structName + "\":");

            SParameter tempParameter = new SParameter();

            for (int i = 0; i < tempStruct.NumParams; i++)
            {
                tempStruct.GetGenericParamByIndex(i, out tempParameter);
                Game.Console.Print(tempParameter.ParamName.ToString() + "  " + "0x" + String.Format("{0:X}", tempParameter.Offset) + "  " + tempParameter.Type.ToString() + "  " + tempParameter.MinVal.ToString() + "  " + tempParameter.MaxVal.ToString());
            }
        }

        /// <summary>
        /// Prints a structure´s parameter values to the console.
        /// </summary>
        /// <param name="parameters">The console input.</param>
        private void printStructureValues_console(ParameterCollection parameters)
        {
            if (parameters.Count < 2)
            {
                Game.Console.Print("usage: si_printvalues <structure name> <element index>");
                return;
            }

            string structName = parameters.ToString(0);
            int index = 0;

            if (!int.TryParse(parameters.ToString(1), out index))
            {
                Game.Console.Print("Invalid index specified!");
                return;
            }

            CEditableStruct tempStruct = GlobalVars.Structures.Find(x => x.Name == structName);

            if (tempStruct == null)
            {
                Game.Console.Print("No struct found with name \"" + structName + "\"!");
                return;
            }

            SParameter tempParam = new SParameter();

            for (int i = 0; i < tempStruct.NumParams; i++)
            {
                tempStruct.GetGenericParamByIndex(i, out tempParam);
                
                if (tempParam.Type == typeof(float))
                {
                    float value = 0f;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
                else if (tempParam.Type == typeof(uint))
                {
                    uint value = 0;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
                else if (tempParam.Type == typeof(string))
                {
                    string value = string.Empty;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value);
                }
                else if (tempParam.Type == typeof(char))
                {
                    char value = '\0';
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value);
                }
                else if (tempParam.Type == typeof(int))
                {
                    int value = 0;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
                else if (tempParam.Type == typeof(short))
                {
                    short value = 0;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
                else if (tempParam.Type == typeof(double))
                {
                    double value = 0f;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
                else if (tempParam.Type == typeof(long))
                {
                    long value = 0;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
                else if (tempParam.Type == typeof(char[]))
                {
                    int value = 0;
                    tempStruct.GetParamValue(index, i, ref value);
                    Game.Console.Print(tempParam.ParamName + " " + value.ToString());
                }
            }
        }

        /// <summary>
        /// A console intermediate to regenerate the mod´s structures.
        /// </summary>
        /// <param name="parameters">None needed.</param>
        private void regenStructures_console(ParameterCollection parameters)
        {
            this.regenStructures();
        }

        /// <summary>
        /// Functions which initiates the regenerations of the mod´s structures: parsing of input files and creating of new CEditableStructs.
        /// </summary>
        private void regenStructures()
        {
            GlobalVars.Structures.Clear();

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(GlobalParams.InputFilesDirectory);
            System.IO.FileInfo[] fi = di.GetFiles("*" + GlobalParams.InputFilesExtension);

            CEditableStruct tempStructure = new CEditableStruct(string.Empty, 0, 0, 0, 0, new List<SParameter>());
            int errorLine = 0;
            int retVal = 0;

            // Load all files
            for (int i = 0; i < fi.Length; i++)
            {
                retVal = CInputParser.CreateStructure(fi[i].FullName, ref tempStructure, ref errorLine);

                if (retVal != 0)
                {
                    Game.Console.Print("Error loading file \"" + fi[i].Name + "\": error code: " + retVal.ToString() + " on line " + errorLine.ToString());
                }
                else
                {
                    Game.Console.Print("Successfully loaded file \"" + fi[i].Name + "\"!");
                    GlobalVars.Structures.Add(tempStructure);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the BombScript control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GTAScript_Tick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the KeyDown event of the BombScript control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GTA.KeyEventArgs"/> instance containing the event data.</param>
        private void GTAScript_KeyDown(object sender, GTA.KeyEventArgs e)
        {
        }
    }
}
