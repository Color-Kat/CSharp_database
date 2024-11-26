namespace MyDatabase
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            uploadCSVButton = new Button();
            mergeTablesButton = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 20);
            label1.Name = "label1";
            label1.Size = new Size(360, 32);
            label1.TabIndex = 0;
            label1.Text = "Upload CSV File to the Database";
            // 
            // uploadCSVButton
            // 
            uploadCSVButton.Location = new Point(22, 68);
            uploadCSVButton.Name = "uploadCSVButton";
            uploadCSVButton.Size = new Size(150, 46);
            uploadCSVButton.TabIndex = 1;
            uploadCSVButton.Text = "Select File";
            uploadCSVButton.UseVisualStyleBackColor = true;
            uploadCSVButton.Click += uploadCSVButton_Click;
            // 
            // mergeTablesButton
            // 
            mergeTablesButton.Location = new Point(22, 209);
            mergeTablesButton.Name = "mergeTablesButton";
            mergeTablesButton.Size = new Size(150, 46);
            mergeTablesButton.TabIndex = 2;
            mergeTablesButton.Text = "Start Merging";
            mergeTablesButton.UseVisualStyleBackColor = true;
            mergeTablesButton.Click += mergeTablesButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 164);
            label2.Name = "label2";
            label2.Size = new Size(362, 32);
            label2.TabIndex = 3;
            label2.Text = "Merge two tables in DB into one";
            label2.Click += label2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(mergeTablesButton);
            Controls.Add(uploadCSVButton);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button uploadCSVButton;
        private Button mergeTablesButton;
        private Label label2;
    }
}
