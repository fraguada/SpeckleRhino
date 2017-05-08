namespace SpeckleRhino
{
  partial class SpeckleRhinoPanelControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
      this.toolStripContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStripContainer
      // 
      this.toolStripContainer.BottomToolStripPanelVisible = false;
      // 
      // toolStripContainer.ContentPanel
      // 
      this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(300, 288);
      this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.toolStripContainer.LeftToolStripPanelVisible = false;
      this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
      this.toolStripContainer.Name = "toolStripContainer";
      this.toolStripContainer.RightToolStripPanelVisible = false;
      this.toolStripContainer.Size = new System.Drawing.Size(300, 288);
      this.toolStripContainer.TabIndex = 0;
      this.toolStripContainer.Text = "toolStripContainer1";
      this.toolStripContainer.TopToolStripPanelVisible = false;
      // 
      // SpeckleRhinoPanelControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.toolStripContainer);
      this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.Name = "SpeckleRhinoPanelControl";
      this.Size = new System.Drawing.Size(300, 288);
      this.toolStripContainer.ResumeLayout(false);
      this.toolStripContainer.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolStripContainer toolStripContainer;
  }
}
