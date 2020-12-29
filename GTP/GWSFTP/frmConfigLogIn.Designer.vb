<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfigLogIn
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_UserName = New System.Windows.Forms.TextBox()
        Me.TextBox_Password = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(27, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "UserName"
        '
        'TextBox_UserName
        '
        Me.TextBox_UserName.Location = New System.Drawing.Point(86, 12)
        Me.TextBox_UserName.Name = "TextBox_UserName"
        Me.TextBox_UserName.Size = New System.Drawing.Size(134, 20)
        Me.TextBox_UserName.TabIndex = 1
        '
        'TextBox_Password
        '
        Me.TextBox_Password.Location = New System.Drawing.Point(86, 52)
        Me.TextBox_Password.Name = "TextBox_Password"
        Me.TextBox_Password.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_Password.Size = New System.Drawing.Size(134, 20)
        Me.TextBox_Password.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(27, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Password"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(111, 93)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Log In"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmConfigLogIn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(259, 148)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox_Password)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBox_UserName)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmConfigLogIn"
        Me.Text = "frmConfigLogIn"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_UserName As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Password As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
