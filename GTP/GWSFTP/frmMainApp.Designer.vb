<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMainApp
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMainApp))
        Me.MainMenu = New System.Windows.Forms.MenuStrip()
        Me.HomeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoginToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FTPToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FTPUploadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PCAPUtilityToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdminToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnableXMLModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DisableXMLModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TestFileTrimmingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShadowPasswordCOngifToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainMenu
        '
        Me.MainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HomeToolStripMenuItem, Me.FTPToolStripMenuItem, Me.AdminToolStripMenuItem})
        Me.MainMenu.Location = New System.Drawing.Point(0, 0)
        Me.MainMenu.Name = "MainMenu"
        Me.MainMenu.Size = New System.Drawing.Size(959, 34)
        Me.MainMenu.TabIndex = 0
        Me.MainMenu.Text = "MenuStrip1"
        '
        'HomeToolStripMenuItem
        '
        Me.HomeToolStripMenuItem.AutoSize = False
        Me.HomeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CloseToolStripMenuItem, Me.LoginToolStripMenuItem})
        Me.HomeToolStripMenuItem.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HomeToolStripMenuItem.Image = CType(resources.GetObject("HomeToolStripMenuItem.Image"), System.Drawing.Image)
        Me.HomeToolStripMenuItem.Name = "HomeToolStripMenuItem"
        Me.HomeToolStripMenuItem.Size = New System.Drawing.Size(68, 30)
        Me.HomeToolStripMenuItem.Text = "Home"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(115, 24)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'LoginToolStripMenuItem
        '
        Me.LoginToolStripMenuItem.Name = "LoginToolStripMenuItem"
        Me.LoginToolStripMenuItem.Size = New System.Drawing.Size(115, 24)
        Me.LoginToolStripMenuItem.Text = "Login"
        '
        'FTPToolStripMenuItem
        '
        Me.FTPToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FTPUploadToolStripMenuItem, Me.PCAPUtilityToolStripMenuItem})
        Me.FTPToolStripMenuItem.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FTPToolStripMenuItem.Name = "FTPToolStripMenuItem"
        Me.FTPToolStripMenuItem.Size = New System.Drawing.Size(47, 30)
        Me.FTPToolStripMenuItem.Text = "GTP"
        '
        'FTPUploadToolStripMenuItem
        '
        Me.FTPUploadToolStripMenuItem.Enabled = False
        Me.FTPUploadToolStripMenuItem.Name = "FTPUploadToolStripMenuItem"
        Me.FTPUploadToolStripMenuItem.Size = New System.Drawing.Size(214, 24)
        Me.FTPUploadToolStripMenuItem.Text = "FTP Upload"
        '
        'PCAPUtilityToolStripMenuItem
        '
        Me.PCAPUtilityToolStripMenuItem.Name = "PCAPUtilityToolStripMenuItem"
        Me.PCAPUtilityToolStripMenuItem.Size = New System.Drawing.Size(214, 24)
        Me.PCAPUtilityToolStripMenuItem.Text = "Create PCAPZIP Files"
        '
        'AdminToolStripMenuItem
        '
        Me.AdminToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EnableXMLModeToolStripMenuItem, Me.DisableXMLModeToolStripMenuItem, Me.TestFileTrimmingToolStripMenuItem, Me.ShadowPasswordCOngifToolStripMenuItem})
        Me.AdminToolStripMenuItem.Font = New System.Drawing.Font("Segoe UI", 11.25!)
        Me.AdminToolStripMenuItem.Name = "AdminToolStripMenuItem"
        Me.AdminToolStripMenuItem.Size = New System.Drawing.Size(65, 30)
        Me.AdminToolStripMenuItem.Text = "Admin"
        '
        'EnableXMLModeToolStripMenuItem
        '
        Me.EnableXMLModeToolStripMenuItem.Name = "EnableXMLModeToolStripMenuItem"
        Me.EnableXMLModeToolStripMenuItem.Size = New System.Drawing.Size(244, 24)
        Me.EnableXMLModeToolStripMenuItem.Text = "Enable XML Mode"
        '
        'DisableXMLModeToolStripMenuItem
        '
        Me.DisableXMLModeToolStripMenuItem.Name = "DisableXMLModeToolStripMenuItem"
        Me.DisableXMLModeToolStripMenuItem.Size = New System.Drawing.Size(244, 24)
        Me.DisableXMLModeToolStripMenuItem.Text = "Disable XML Mode"
        '
        'TestFileTrimmingToolStripMenuItem
        '
        Me.TestFileTrimmingToolStripMenuItem.Name = "TestFileTrimmingToolStripMenuItem"
        Me.TestFileTrimmingToolStripMenuItem.Size = New System.Drawing.Size(244, 24)
        Me.TestFileTrimmingToolStripMenuItem.Text = "Test File Trimming"
        '
        'ShadowPasswordCOngifToolStripMenuItem
        '
        Me.ShadowPasswordCOngifToolStripMenuItem.Name = "ShadowPasswordCOngifToolStripMenuItem"
        Me.ShadowPasswordCOngifToolStripMenuItem.Size = New System.Drawing.Size(244, 24)
        Me.ShadowPasswordCOngifToolStripMenuItem.Text = "Shadow Password Config"
        '
        'frmMainApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(959, 642)
        Me.Controls.Add(Me.MainMenu)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MainMenu
        Me.Name = "frmMainApp"
        Me.Text = "GTP Client"
        Me.MainMenu.ResumeLayout(False)
        Me.MainMenu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MainMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents HomeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoginToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FTPToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FTPUploadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        CloseAllForms()
        Dim myform As New frmLogin
        myform.MdiParent = Me
        myform.StartPosition = FormStartPosition.CenterScreen
        myform.Show()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Friend WithEvents AdminToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EnableXMLModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DisableXMLModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TestFileTrimmingToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShadowPasswordCOngifToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PCAPUtilityToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
