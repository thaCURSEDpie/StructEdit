﻿//=======================================================================
//
// <copyright file="Gui.cs" company="not applicable">
//     Copyright (c) thaCURSEDpie. All rights reserved.
// </copyright>
//
//-----------------------------------------------------------------------
//          File:           Gui.cs
//          Version:        Pre-Alpha
//          Part of:        StructEdit mod
//          Author:         thaCURSEDpie
//          Date:           December 2011
//          Description:
//              This file contains the StructEditGuiForm class, which
//              inherits from GTA.Forms.Form. This class provides the
//              GUI functionality for the mod.
//
//              This file also contains the StructEditGuiScript class,
//              which inherits from GTA.Script. This class is used to
//              initiate and control the form (opening).
//
//=======================================================================

namespace StructEdit.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GTA;
    using GTA.Forms;

    public class StructEditGuiForm : GTA.Forms.Form
    {


        GTA.Forms.Listbox listBox,
                          listBox_values;

        GTA.Forms.Button nextStructBtn,
                         prevStructBtn,
                         nextElementBtn,
                         prevElementBtn,
                         editValBtn,
                         zapToElementBtn,
                         zapToStructBtn;

        GTA.Forms.Textbox newVal,
                          zapElementTextbox,
                          zapStructTextbox;

        GTA.Forms.Label structNameLbl,
                        elementNumLbl;

        int width, height;
        int currentStructIndex,
            currentElementIndex;

        StructEditGuiScript owner;

        private bool ctrlPressed;
        private string clipboard;

        public StructEditGuiForm(StructEditGuiScript owner, int width, int height)
        {
            this.owner = owner;
            this.ctrlPressed = false;
            this.clipboard = string.Empty;

            this.Opened += new EventHandler(StructEditGuiForm_Opened);
            this.width = width;            
            this.height = height;

            this.currentElementIndex = 0;
            this.currentStructIndex = 0;

            this.Size = new System.Drawing.Size(this.width, this.height);

            this.KeyUp += new KeyEventHandler(StructEditGuiForm_KeyUp);
            this.KeyDown += new KeyEventHandler(StructEditGuiForm_KeyDown);

            this.zapElementTextbox = new Textbox();
            this.zapElementTextbox.Location = new System.Drawing.Point((int)(this.width * 0.48), (int)(this.height * 0.1));
            this.zapElementTextbox.Width = (int)(this.width * 0.10);
            this.zapElementTextbox.Text = "XXX";
            this.Controls.Add(this.zapElementTextbox);
            this.zapElementTextbox.GotFocus += new EventHandler(zapElementTextbox_GotFocus);

            this.zapStructTextbox = new Textbox();
            this.zapStructTextbox.Location = new System.Drawing.Point((int)(this.width * 0.48), (int)(this.height * 0.01));
            this.zapStructTextbox.Width = (int)(this.width * 0.10);
            this.zapStructTextbox.Text = "XXX";
            this.Controls.Add(this.zapStructTextbox);
            this.zapStructTextbox.GotFocus += new EventHandler(zapStructTextbox_GotFocus);

            this.zapToElementBtn = new Button();
            this.zapToElementBtn.Location = new System.Drawing.Point((int)(this.width * 0.6), (int)(this.height * 0.1));
            this.zapToElementBtn.Width = (int)(this.width * 0.2);
            this.zapToElementBtn.Text = "Go to element";
            this.Controls.Add(this.zapToElementBtn);
            this.zapToElementBtn.Click += new MouseEventHandler(zapToElementBtn_Click);
            
            this.zapToStructBtn = new Button();
            this.zapToStructBtn.Location = new System.Drawing.Point((int)(this.width * 0.6), (int)(this.height * 0.01));
            this.zapToStructBtn.Width = (int)(this.width * 0.2);
            this.zapToStructBtn.Text = "Go to struct";
            this.Controls.Add(this.zapToStructBtn);
            this.zapToStructBtn.Click += new MouseEventHandler(zapToStructBtn_Click);

            this.newVal = new Textbox();
            this.newVal.Location = new System.Drawing.Point((int)(this.width * 0.7), (int)(this.height * 0.51));
            this.newVal.Width = (int)(this.width * 0.27);
            this.newVal.Height = (int)(this.height * 0.1);
            this.newVal.Text = "Enter new values here";
            this.Controls.Add(this.newVal);
            this.newVal.GotFocus += new EventHandler(newVal_GotFocus);

            this.editValBtn = new Button();
            this.editValBtn.Location = new System.Drawing.Point((int)(this.width * 0.7), (int)(this.height * 0.62));
            this.editValBtn.Width = (int)(this.width * 0.27);
            this.editValBtn.Text = "Change selected param";
            this.Controls.Add(this.editValBtn);
            this.editValBtn.Click += new MouseEventHandler(editValBtn_Click);

            this.structNameLbl = new Label();
            this.structNameLbl.Location = new System.Drawing.Point((int)(this.width * 0.08), (int)(this.height * 0.01));
            this.structNameLbl.Text = "Current structure: ";
            this.structNameLbl.Width = (int)(this.width * 0.3);
            this.structNameLbl.Height = (int)(this.height * 0.05);
            this.Controls.Add(this.structNameLbl);

            this.elementNumLbl = new Label();
            this.elementNumLbl.Location = new System.Drawing.Point((int)(this.width * 0.08), (int)(this.height * 0.1));
            this.elementNumLbl.Text = "Current element: ";
            this.elementNumLbl.Width = (int)(this.width * 0.3);
            this.elementNumLbl.Height = (int)(this.height * 0.05);
            this.Controls.Add(this.elementNumLbl);

            this.nextStructBtn = new Button();
            this.nextStructBtn.Location = new System.Drawing.Point((int)(this.width * 0.4), (int)(this.height * 0.01));
            this.nextStructBtn.Text = "Next";
            this.nextStructBtn.Width = 40;
            this.Controls.Add(this.nextStructBtn);
            this.nextStructBtn.Click += new MouseEventHandler(nextStructBtn_Click);

            this.prevStructBtn = new Button();
            this.prevStructBtn.Location = new System.Drawing.Point((int)(0), (int)(this.height * 0.01));
            this.prevStructBtn.Text = "Prev";
            this.prevStructBtn.Width = 40;
            this.Controls.Add(this.prevStructBtn);
            this.prevStructBtn.Click += new MouseEventHandler(prevStructBtn_Click);

            this.nextElementBtn = new Button();
            this.nextElementBtn.Location = new System.Drawing.Point((int)(this.width * 0.4), (int)(this.height * 0.1));
            this.nextElementBtn.Text = "Next";
            this.nextElementBtn.Width = 40;
            this.Controls.Add(this.nextElementBtn);
            this.nextElementBtn.Click += new MouseEventHandler(nextElementBtn_Click);

            this.prevElementBtn = new Button();
            this.prevElementBtn.Location = new System.Drawing.Point((int)(0), (int)(this.height * 0.1));
            this.prevElementBtn.Text = "Prev";
            this.prevElementBtn.Width = 40;
            this.Controls.Add(this.prevElementBtn);
            this.prevElementBtn.Click += new MouseEventHandler(prevElementBtn_Click);

            this.listBox = new Listbox();
            this.listBox.Location = new System.Drawing.Point((int)(0), (int)(this.height * 0.2));
            this.listBox.Size = new System.Drawing.Size((int)(this.width * 0.3), (int)(this.height * 0.7));
            this.listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);
            this.Controls.Add(this.listBox);

            this.listBox_values = new Listbox();
            this.listBox_values.Location = new System.Drawing.Point((int)(this.width * 0.3), (int)(this.height * 0.2));
            this.listBox_values.Size = new System.Drawing.Size((int)(this.width * 0.3), (int)(this.height * 0.7));
            this.listBox_values.SelectedIndexChanged += new EventHandler(listBox_values_SelectedIndexChanged);

            this.Controls.Add(this.listBox_values);
        }

        private void updateLabels()
        {
            this.elementNumLbl.Text = "Current element: " + this.currentElementIndex.ToString() + "/" + (GlobalVars.Structures[this.currentStructIndex].NumElements - 1).ToString();
            this.structNameLbl.Text = "Current structure: " + GlobalVars.Structures[this.currentStructIndex].Name + " (" + this.currentStructIndex.ToString() + "/" + (GlobalVars.Structures.Count - 1 ).ToString() + ")";
        }

        void newVal_GotFocus(object sender, EventArgs e)
        {
            this.newVal.Text = string.Empty;
        }

        void zapToElementBtn_Click(object sender, MouseEventArgs e)
        {
            int newIndex = 0;
            if (int.TryParse(this.zapElementTextbox.Text, out newIndex))
            {
                if (newIndex >= 0 && newIndex < GlobalVars.Structures[this.currentStructIndex].NumElements)
                {
                    this.currentElementIndex = newIndex;
                    this.reloadParams();
                    updateLabels();
                }
            }
        }

        void zapToStructBtn_Click(object sender, MouseEventArgs e)
        {
            int newIndex = 0;
            if (int.TryParse(this.zapStructTextbox.Text, out newIndex))
            {
                if (newIndex >= 0 && newIndex < GlobalVars.Structures.Count)
                {
                    this.currentStructIndex = newIndex;
                    this.reloadParams();
                    updateLabels();
                }
            }
        }

        void zapStructTextbox_GotFocus(object sender, EventArgs e)
        {
            this.zapStructTextbox.Text = string.Empty;
        }

        void zapElementTextbox_GotFocus(object sender, EventArgs e)
        {
            this.zapElementTextbox.Text = string.Empty;
        }

        void StructEditGuiForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Forms.Keys.ControlKey ||
                e.Key == System.Windows.Forms.Keys.Control ||
                e.Key == System.Windows.Forms.Keys.LControlKey ||
                e.Key == System.Windows.Forms.Keys.RControlKey)
            {
                this.ctrlPressed = false;
            }
        }

        void StructEditGuiForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Forms.Keys.ControlKey ||
                e.Key == System.Windows.Forms.Keys.Control ||
                e.Key == System.Windows.Forms.Keys.LControlKey ||
                e.Key == System.Windows.Forms.Keys.RControlKey)
            {
                this.ctrlPressed = true;
            }

            // Copy
            if (e.Key == System.Windows.Forms.Keys.C && this.ctrlPressed)
            {
                this.clipboard = this.listBox_values.SelectedItem.DisplayText;
            }

            // Paste
            if (e.Key == System.Windows.Forms.Keys.V && this.ctrlPressed)
            {
                this.newVal.Text += this.clipboard;
            }

            // Delete (clear the input textbox)
            if (e.Key == System.Windows.Forms.Keys.D && this.ctrlPressed)
            {
                this.newVal.Text = string.Empty;
            }
        }

        public Textbox NewValTxtBox
        {
            get
            {
                return this.newVal;
            }
        }

        public Listbox ValuesListbox
        {
            get
            {
                return this.listBox_values;
            }
        }

        void editValBtn_Click(object sender, MouseEventArgs e)
        {
            SParameter tempParam = new SParameter();
            GlobalVars.Structures[this.currentStructIndex].GetGenericParamByIndex(this.listBox.SelectedIndex, out tempParam);

            int selectedIndex = this.listBox_values.SelectedIndex;

            if (tempParam.Type == typeof(int))
            {
                int newVal = 0;
                if (Int32.TryParse(this.newVal.Text, out newVal))
                {                    
                    GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);                
                }
            }
            else if (tempParam.Type == typeof(uint))
            {
                uint newVal = 0;
                if (uint.TryParse(this.newVal.Text, out newVal))
                {
                    GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);
                }
            }
            else if (tempParam.Type == typeof(string))
            {
                GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, this.newVal.Text);
            }
            else if (tempParam.Type == typeof(char))
            {
                char newVal = this.newVal.Text.ToCharArray()[0];
                GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);
            }
            else if (tempParam.Type == typeof(float))
            {
                float newVal = 0;
                if (float.TryParse(this.newVal.Text, out newVal))
                {
                    GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);
                }
            }
            else if (tempParam.Type == typeof(double))
            {
                double newVal = 0;
                if (double.TryParse(this.newVal.Text, out newVal))
                {
                    GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);
                }
            }
            else if (tempParam.Type == typeof(short))
            {
                short newVal = 0;
                if (short.TryParse(this.newVal.Text, out newVal))
                {
                    GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);
                }
            }
            else if (tempParam.Type == typeof(long))
            {
                long newVal = 0;
                if (long.TryParse(this.newVal.Text, out newVal))
                {
                    GlobalVars.Structures[this.currentStructIndex].SetParamValue(this.currentElementIndex, this.listBox.SelectedIndex, newVal);
                }
            }

            this.reloadParams();
            this.listBox_values.SelectedIndex = selectedIndex;
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedIndex != this.listBox_values.SelectedIndex)
            {
                this.listBox_values.SelectedIndex = this.listBox.SelectedIndex;
            }
        }

        void listBox_values_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedIndex != this.listBox_values.SelectedIndex)
            {
                this.listBox.SelectedIndex = this.listBox_values.SelectedIndex;
            }
        }

        private string getParamValue(int index)
        {
            SParameter tempParam = new SParameter();
            CEditableStruct tempStruct = GlobalVars.Structures[this.currentStructIndex];
            tempStruct.GetGenericParamByIndex(index, out tempParam);

            if (tempParam.Type == typeof(float))
            {
                float value = 0f;
                
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }
            else if (tempParam.Type == typeof(string))
            {
                string value = string.Empty;
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value;
            }
            else if (tempParam.Type == typeof(char))
            {
                char value = '\0';
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }
            else if (tempParam.Type == typeof(int))
            {
                int value = 0;
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }
            else if (tempParam.Type == typeof(uint))
            {
                uint value = 0;
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }
            else if (tempParam.Type == typeof(short))
            {
                short value = 0;
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }
            else if (tempParam.Type == typeof(double))
            {
                double value = 0f;
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }
            else if (tempParam.Type == typeof(long))
            {
                long value = 0;
                tempStruct.GetParamValue(this.currentElementIndex, index, ref value);
                return value.ToString();
            }

            return string.Empty;
        }

        void nextElementBtn_Click(object sender, MouseEventArgs e)
        {
            this.currentElementIndex++;

            if (this.currentElementIndex >= GlobalVars.Structures[this.currentStructIndex].NumElements)
            {
                this.currentElementIndex = 0;
            }

            this.reloadParams();
            updateLabels();
        }

        void prevElementBtn_Click(object sender, MouseEventArgs e)
        {
            this.currentElementIndex--;

            if (this.currentElementIndex < 0)
            {
                this.currentElementIndex = GlobalVars.Structures[this.currentStructIndex].NumElements - 1;
            }

            this.reloadParams();
            updateLabels();
        }

        private void reloadParams()
        {
            this.listBox.Items.Clear();
            this.listBox_values.Items.Clear();

            SParameter tempParam = new SParameter();

            // We load in the parameters
            for (int i = 0; i < GlobalVars.Structures[this.currentStructIndex].NumParams; i++)
            {
                GlobalVars.Structures[this.currentStructIndex].GetGenericParamByIndex(i, out tempParam);
                this.listBox.Items.Add(new ListboxItem(), tempParam.ParamName);

                this.listBox_values.Items.Add(new ListboxItem(), this.getParamValue(i));
            }
        }

        void nextStructBtn_Click(object sender, MouseEventArgs e)
        {
            this.currentStructIndex++;
            this.currentElementIndex = 0;

            if (this.currentStructIndex >= GlobalVars.Structures.Count)
            {
                this.currentStructIndex = 0;
            }

            this.reloadParams();
            updateLabels();
        }

        void prevStructBtn_Click(object sender, MouseEventArgs e)
        {
            this.currentStructIndex--;
            this.currentElementIndex = 0;

            if (this.currentStructIndex < 0)
            {
                this.currentStructIndex = GlobalVars.Structures.Count - 1;
            }

            this.reloadParams();
            updateLabels();
        }

        void StructEditGuiForm_Opened(object sender, EventArgs e)
        {
            reloadParams();
            updateLabels();
        }

        
    }

    public class StructEditGuiScript : GTA.Script
    {
        StructEditGuiForm form;

        int height, width;

        public string currentStructName;

        public string[] currentStructParams;

        private string clipboard;

        public StructEditGuiScript()
        {
            this.clipboard = string.Empty;

            this.width = GTA.Game.Resolution.Width;
            this.height = GTA.Game.Resolution.Height;

            BindConsoleCommand("si_gui", new GTA.ConsoleCommandDelegate(openGui_console), "- opens the StructEdit GUI");

            this.form = new StructEditGuiForm(this, this.width, this.height);
        }

        private void openGui_console(ParameterCollection parameters)
        {
            Game.Console.Close();
            this.form.Show();
        }
    }
}
