namespace ExternalGlasses
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.OverlayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OverlayButton
            // 
            this.OverlayButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverlayButton.Location = new System.Drawing.Point(12, 12);
            this.OverlayButton.Name = "OverlayButton";
            this.OverlayButton.Size = new System.Drawing.Size(206, 84);
            this.OverlayButton.TabIndex = 0;
            this.OverlayButton.Text = "Open";
            this.OverlayButton.UseVisualStyleBackColor = true;
            this.OverlayButton.Click += new System.EventHandler(this.OverlayButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 108);
            this.Controls.Add(this.OverlayButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "External Glasses";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OverlayButton;
    }
}