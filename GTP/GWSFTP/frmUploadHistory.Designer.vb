<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUploadHistory
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
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.ComboBox_Market = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.TextBox_Server = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBox_UserName = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox_Password = New System.Windows.Forms.TextBox()
        Me.CheckBox_SaveCredential = New System.Windows.Forms.CheckBox()
        Me.Label_Info = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Button_ViewFiles = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Panel3.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel3.Controls.Add(Me.Label5)
        Me.Panel3.Controls.Add(Me.ComboBox1)
        Me.Panel3.Controls.Add(Me.Button_ViewFiles)
        Me.Panel3.Controls.Add(Me.ComboBox_Market)
        Me.Panel3.Controls.Add(Me.Label4)
        Me.Panel3.Location = New System.Drawing.Point(12, 71)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(945, 40)
        Me.Panel3.TabIndex = 27
        '
        'ComboBox_Market
        '
        Me.ComboBox_Market.FormattingEnabled = True
        Me.ComboBox_Market.Location = New System.Drawing.Point(69, 10)
        Me.ComboBox_Market.Name = "ComboBox_Market"
        Me.ComboBox_Market.Size = New System.Drawing.Size(308, 21)
        Me.ComboBox_Market.TabIndex = 14
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 13)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Market:"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel2.Controls.Add(Me.TextBox_Server)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.TextBox_UserName)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.TextBox_Password)
        Me.Panel2.Controls.Add(Me.CheckBox_SaveCredential)
        Me.Panel2.Location = New System.Drawing.Point(12, 28)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(945, 40)
        Me.Panel2.TabIndex = 26
        '
        'TextBox_Server
        '
        Me.TextBox_Server.Location = New System.Drawing.Point(56, 9)
        Me.TextBox_Server.Name = "TextBox_Server"
        Me.TextBox_Server.Size = New System.Drawing.Size(182, 20)
        Me.TextBox_Server.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Server:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(250, 12)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Username:"
        '
        'TextBox_UserName
        '
        Me.TextBox_UserName.Location = New System.Drawing.Point(314, 10)
        Me.TextBox_UserName.Name = "TextBox_UserName"
        Me.TextBox_UserName.Size = New System.Drawing.Size(165, 20)
        Me.TextBox_UserName.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(494, 10)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Password:"
        '
        'TextBox_Password
        '
        Me.TextBox_Password.Location = New System.Drawing.Point(569, 9)
        Me.TextBox_Password.Name = "TextBox_Password"
        Me.TextBox_Password.Size = New System.Drawing.Size(148, 20)
        Me.TextBox_Password.TabIndex = 10
        Me.TextBox_Password.UseSystemPasswordChar = True
        '
        'CheckBox_SaveCredential
        '
        Me.CheckBox_SaveCredential.AutoSize = True
        Me.CheckBox_SaveCredential.Checked = True
        Me.CheckBox_SaveCredential.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox_SaveCredential.Location = New System.Drawing.Point(723, 11)
        Me.CheckBox_SaveCredential.Name = "CheckBox_SaveCredential"
        Me.CheckBox_SaveCredential.Size = New System.Drawing.Size(154, 17)
        Me.CheckBox_SaveCredential.TabIndex = 11
        Me.CheckBox_SaveCredential.Text = "Remember FTP credentials"
        Me.CheckBox_SaveCredential.UseVisualStyleBackColor = True
        '
        'Label_Info
        '
        Me.Label_Info.AutoSize = True
        Me.Label_Info.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Info.ForeColor = System.Drawing.Color.Red
        Me.Label_Info.Location = New System.Drawing.Point(10, 9)
        Me.Label_Info.Name = "Label_Info"
        Me.Label_Info.Size = New System.Drawing.Size(147, 16)
        Me.Label_Info.TabIndex = 25
        Me.Label_Info.Text = "GTP Upload History"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 127)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1144, 584)
        Me.TabControl1.TabIndex = 28
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1136, 558)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "On FTP Server"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1136, 558)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "All Upload"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Button_ViewFiles
        '
        Me.Button_ViewFiles.Location = New System.Drawing.Point(681, 13)
        Me.Button_ViewFiles.Name = "Button_ViewFiles"
        Me.Button_ViewFiles.Size = New System.Drawing.Size(134, 23)
        Me.Button_ViewFiles.TabIndex = 17
        Me.Button_ViewFiles.Text = "View Uploaded Files"
        Me.Button_ViewFiles.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(523, 10)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(121, 21)
        Me.ComboBox1.TabIndex = 18
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(418, 13)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(82, 13)
        Me.Label5.TabIndex = 19
        Me.Label5.Text = "Date Uploaded:"
        '
        'frmUploadHistory
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1188, 742)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Label_Info)
        Me.Name = "frmUploadHistory"
        Me.Text = "frmUploadHistory"
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents ComboBox_Market As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents TextBox_Server As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBox_UserName As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Password As System.Windows.Forms.TextBox
    Friend WithEvents CheckBox_SaveCredential As System.Windows.Forms.CheckBox
    Friend WithEvents Label_Info As System.Windows.Forms.Label
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents Button_ViewFiles As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
End Class
