<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLogin
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLogin))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBox_PassWord = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.CheckBox_RememberMe = New System.Windows.Forms.CheckBox()
        Me.TextBox_UserName = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(27, 44)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Username:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(27, 92)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Password:"
        '
        'TextBox_PassWord
        '
        Me.TextBox_PassWord.Location = New System.Drawing.Point(101, 85)
        Me.TextBox_PassWord.Name = "TextBox_PassWord"
        Me.TextBox_PassWord.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_PassWord.Size = New System.Drawing.Size(156, 20)
        Me.TextBox_PassWord.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(101, 149)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(156, 37)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Log In"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'CheckBox_RememberMe
        '
        Me.CheckBox_RememberMe.AutoSize = True
        Me.CheckBox_RememberMe.Location = New System.Drawing.Point(101, 114)
        Me.CheckBox_RememberMe.Name = "CheckBox_RememberMe"
        Me.CheckBox_RememberMe.Size = New System.Drawing.Size(94, 17)
        Me.CheckBox_RememberMe.TabIndex = 6
        Me.CheckBox_RememberMe.Text = "Remember me"
        Me.CheckBox_RememberMe.UseVisualStyleBackColor = True
        '
        'TextBox_UserName
        '
        Me.TextBox_UserName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TextBox_UserName.FormattingEnabled = True
        Me.TextBox_UserName.Location = New System.Drawing.Point(101, 44)
        Me.TextBox_UserName.Name = "TextBox_UserName"
        Me.TextBox_UserName.Size = New System.Drawing.Size(156, 21)
        Me.TextBox_UserName.TabIndex = 17
        '
        'frmLogin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(317, 226)
        Me.Controls.Add(Me.TextBox_UserName)
        Me.Controls.Add(Me.CheckBox_RememberMe)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox_PassWord)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmLogin"
        Me.Text = "Log In to GTP"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBox_PassWord As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents CheckBox_RememberMe As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox_UserName As System.Windows.Forms.ComboBox
End Class
