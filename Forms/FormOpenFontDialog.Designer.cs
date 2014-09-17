namespace MosFontTool
{
    partial class FormOpenFontDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.btnPickFont = new System.Windows.Forms.Button();
            this.txtFontFile = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "Font file:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(474, 312);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(87, 33);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(349, 312);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(87, 33);
            this.buttonOK.TabIndex = 15;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // btnPickFont
            // 
            this.btnPickFont.Location = new System.Drawing.Point(443, 14);
            this.btnPickFont.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnPickFont.Name = "btnPickFont";
            this.btnPickFont.Size = new System.Drawing.Size(87, 33);
            this.btnPickFont.TabIndex = 13;
            this.btnPickFont.Text = "...";
            this.btnPickFont.UseVisualStyleBackColor = true;
            this.btnPickFont.Click += new System.EventHandler(this.btnPickFont_Click);
            // 
            // txtFontFile
            // 
            this.txtFontFile.Location = new System.Drawing.Point(80, 14);
            this.txtFontFile.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtFontFile.Name = "txtFontFile";
            this.txtFontFile.ReadOnly = true;
            this.txtFontFile.Size = new System.Drawing.Size(345, 21);
            this.txtFontFile.TabIndex = 12;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(13, 59);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.groupBox1.Size = new System.Drawing.Size(517, 202);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Font size";
            // 
            // FormOpenFontDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 366);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.btnPickFont);
            this.Controls.Add(this.txtFontFile);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormOpenFontDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormOpenFormDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button btnPickFont;
        private System.Windows.Forms.TextBox txtFontFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}