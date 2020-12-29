<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCreateUser
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
        Me.TextBox_PassWord = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBox_UserName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_FTPPassWord = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox_FTPUserName = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox_TeamName = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBox_FTPServer = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBox_Email = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TextBox_Phone = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextBox_FirstName = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TextBox_LastName = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TextBox_RePassword = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TextBox_ReFTPPassWord = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'TextBox_PassWord
        '
        Me.TextBox_PassWord.Location = New System.Drawing.Point(152, 80)
        Me.TextBox_PassWord.Name = "TextBox_PassWord"
        Me.TextBox_PassWord.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_PassWord.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_PassWord.TabIndex = 10
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(83, 87)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Password:"
        '
        'TextBox_UserName
        '
        Me.TextBox_UserName.Location = New System.Drawing.Point(152, 36)
        Me.TextBox_UserName.Name = "TextBox_UserName"
        Me.TextBox_UserName.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_UserName.TabIndex = 8
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(81, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Username:"
        '
        'TextBox_FTPPassWord
        '
        Me.TextBox_FTPPassWord.Location = New System.Drawing.Point(151, 194)
        Me.TextBox_FTPPassWord.Name = "TextBox_FTPPassWord"
        Me.TextBox_FTPPassWord.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_FTPPassWord.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_FTPPassWord.TabIndex = 14
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(61, 201)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(79, 13)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "FTP Password:"
        '
        'TextBox_FTPUserName
        '
        Me.TextBox_FTPUserName.Location = New System.Drawing.Point(151, 150)
        Me.TextBox_FTPUserName.Name = "TextBox_FTPUserName"
        Me.TextBox_FTPUserName.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_FTPUserName.TabIndex = 12
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(59, 153)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(81, 13)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "FTP Username:"
        '
        'TextBox_TeamName
        '
        Me.TextBox_TeamName.Location = New System.Drawing.Point(150, 394)
        Me.TextBox_TeamName.Name = "TextBox_TeamName"
        Me.TextBox_TeamName.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_TeamName.TabIndex = 19
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.Red
        Me.Label5.Location = New System.Drawing.Point(72, 401)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(68, 13)
        Me.Label5.TabIndex = 17
        Me.Label5.Text = "Team Name:"
        '
        'TextBox_FTPServer
        '
        Me.TextBox_FTPServer.Location = New System.Drawing.Point(150, 270)
        Me.TextBox_FTPServer.Name = "TextBox_FTPServer"
        Me.TextBox_FTPServer.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_FTPServer.TabIndex = 16
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(76, 273)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(64, 13)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "FTP Server:"
        '
        'TextBox_Email
        '
        Me.TextBox_Email.Location = New System.Drawing.Point(152, 435)
        Me.TextBox_Email.Name = "TextBox_Email"
        Me.TextBox_Email.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_Email.TabIndex = 20
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.ForeColor = System.Drawing.Color.Red
        Me.Label7.Location = New System.Drawing.Point(105, 442)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(35, 13)
        Me.Label7.TabIndex = 19
        Me.Label7.Text = "Email:"
        '
        'TextBox_Phone
        '
        Me.TextBox_Phone.Location = New System.Drawing.Point(150, 476)
        Me.TextBox_Phone.Name = "TextBox_Phone"
        Me.TextBox_Phone.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_Phone.TabIndex = 22
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.ForeColor = System.Drawing.Color.Red
        Me.Label9.Location = New System.Drawing.Point(99, 483)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(41, 13)
        Me.Label9.TabIndex = 21
        Me.Label9.Text = "Phone:"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(211, 523)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 23
        Me.Button1.Text = "Create"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TextBox_FirstName
        '
        Me.TextBox_FirstName.Location = New System.Drawing.Point(147, 356)
        Me.TextBox_FirstName.Name = "TextBox_FirstName"
        Me.TextBox_FirstName.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_FirstName.TabIndex = 18
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ForeColor = System.Drawing.Color.Red
        Me.Label8.Location = New System.Drawing.Point(80, 359)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(60, 13)
        Me.Label8.TabIndex = 26
        Me.Label8.Text = "First Name:"
        '
        'TextBox_LastName
        '
        Me.TextBox_LastName.Location = New System.Drawing.Point(148, 310)
        Me.TextBox_LastName.Name = "TextBox_LastName"
        Me.TextBox_LastName.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_LastName.TabIndex = 17
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ForeColor = System.Drawing.Color.Red
        Me.Label10.Location = New System.Drawing.Point(82, 313)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(58, 13)
        Me.Label10.TabIndex = 24
        Me.Label10.Text = "LastName:"
        '
        'TextBox_RePassword
        '
        Me.TextBox_RePassword.Location = New System.Drawing.Point(152, 116)
        Me.TextBox_RePassword.Name = "TextBox_RePassword"
        Me.TextBox_RePassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_RePassword.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_RePassword.TabIndex = 11
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.ForeColor = System.Drawing.Color.Red
        Me.Label11.Location = New System.Drawing.Point(39, 123)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(100, 13)
        Me.Label11.TabIndex = 27
        Me.Label11.Text = "Re enter Password:"
        '
        'TextBox_ReFTPPassWord
        '
        Me.TextBox_ReFTPPassWord.Location = New System.Drawing.Point(151, 228)
        Me.TextBox_ReFTPPassWord.Name = "TextBox_ReFTPPassWord"
        Me.TextBox_ReFTPPassWord.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBox_ReFTPPassWord.Size = New System.Drawing.Size(238, 20)
        Me.TextBox_ReFTPPassWord.TabIndex = 15
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(17, 235)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(123, 13)
        Me.Label12.TabIndex = 29
        Me.Label12.Text = "Re enter FTP Password:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.ForeColor = System.Drawing.Color.Red
        Me.Label13.Location = New System.Drawing.Point(81, 9)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(156, 13)
        Me.Label13.TabIndex = 31
        Me.Label13.Text = "Red Color: Required fields"
        '
        'CreateUser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(446, 561)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.TextBox_ReFTPPassWord)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.TextBox_RePassword)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.TextBox_FirstName)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.TextBox_LastName)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox_Phone)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.TextBox_Email)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.TextBox_TeamName)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TextBox_FTPServer)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TextBox_FTPPassWord)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBox_FTPUserName)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBox_PassWord)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBox_UserName)
        Me.Controls.Add(Me.Label1)
        Me.Name = "CreateUser"
        Me.Text = "CreateUser"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox_PassWord As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBox_UserName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_FTPPassWord As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox_FTPUserName As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBox_TeamName As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TextBox_FTPServer As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Email As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Phone As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents TextBox_FirstName As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents TextBox_LastName As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TextBox_RePassword As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents TextBox_ReFTPPassWord As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
End Class
