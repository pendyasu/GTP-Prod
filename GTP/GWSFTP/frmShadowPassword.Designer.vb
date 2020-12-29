<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShadowPassword
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
        Me.TextBox_ShadowPassword = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBox_ShadowPassword1 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.CheckBox_Active = New System.Windows.Forms.CheckBox()
        Me.Button_Update = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(57, 79)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Shadow UserName"
        '
        'TextBox_UserName
        '
        Me.TextBox_UserName.Location = New System.Drawing.Point(178, 76)
        Me.TextBox_UserName.Name = "TextBox_UserName"
        Me.TextBox_UserName.Size = New System.Drawing.Size(168, 20)
        Me.TextBox_UserName.TabIndex = 1
        '
        'TextBox_ShadowPassword
        '
        Me.TextBox_ShadowPassword.Location = New System.Drawing.Point(178, 116)
        Me.TextBox_ShadowPassword.Name = "TextBox_ShadowPassword"
        Me.TextBox_ShadowPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_ShadowPassword.Size = New System.Drawing.Size(168, 20)
        Me.TextBox_ShadowPassword.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(57, 119)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(95, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Shadow Password"
        '
        'TextBox_ShadowPassword1
        '
        Me.TextBox_ShadowPassword1.Location = New System.Drawing.Point(178, 151)
        Me.TextBox_ShadowPassword1.Name = "TextBox_ShadowPassword1"
        Me.TextBox_ShadowPassword1.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_ShadowPassword1.Size = New System.Drawing.Size(168, 20)
        Me.TextBox_ShadowPassword1.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(27, 154)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(125, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Shadow Password Again"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(177, 244)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "Add"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'CheckBox_Active
        '
        Me.CheckBox_Active.AutoSize = True
        Me.CheckBox_Active.Location = New System.Drawing.Point(178, 211)
        Me.CheckBox_Active.Name = "CheckBox_Active"
        Me.CheckBox_Active.Size = New System.Drawing.Size(67, 17)
        Me.CheckBox_Active.TabIndex = 7
        Me.CheckBox_Active.Text = "Is Active"
        Me.CheckBox_Active.UseVisualStyleBackColor = True
        '
        'Button_Update
        '
        Me.Button_Update.Location = New System.Drawing.Point(270, 244)
        Me.Button_Update.Name = "Button_Update"
        Me.Button_Update.Size = New System.Drawing.Size(75, 23)
        Me.Button_Update.TabIndex = 8
        Me.Button_Update.Text = "Update"
        Me.Button_Update.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"Stand Alone", "Append"})
        Me.ComboBox1.Location = New System.Drawing.Point(177, 177)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(121, 21)
        Me.ComboBox1.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(111, 180)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(31, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Type"
        '
        'frmShadowPassword
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(395, 289)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.ComboBox1)
        Me.Controls.Add(Me.Button_Update)
        Me.Controls.Add(Me.CheckBox_Active)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox_ShadowPassword1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBox_ShadowPassword)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBox_UserName)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmShadowPassword"
        Me.Text = "frmShadowPassword"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_UserName As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_ShadowPassword As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBox_ShadowPassword1 As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents CheckBox_Active As System.Windows.Forms.CheckBox
    Friend WithEvents Button_Update As System.Windows.Forms.Button
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
End Class
