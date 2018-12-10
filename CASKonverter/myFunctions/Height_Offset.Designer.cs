namespace CASKonverter.myFunctions
{
    partial class Height_Offset
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
            this.tb_HeightOffset = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Offset:";
            // 
            // tb_HeightOffset
            // 
            this.tb_HeightOffset.Location = new System.Drawing.Point(58, 13);
            this.tb_HeightOffset.Name = "tb_HeightOffset";
            this.tb_HeightOffset.Size = new System.Drawing.Size(90, 20);
            this.tb_HeightOffset.TabIndex = 1;
            this.tb_HeightOffset.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_HeightOffset_KeyPress);
            // 
            // Height_Offset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 52);
            this.ControlBox = false;
            this.Controls.Add(this.tb_HeightOffset);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(220, 90);
            this.MinimumSize = new System.Drawing.Size(220, 90);
            this.Name = "Height_Offset";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Height_Offset";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_HeightOffset;
    }
}