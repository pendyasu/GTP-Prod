Imports EncryptionClass
Imports System.Data.OleDb
Imports GWSFTP.GTPService
Imports System.Net.Mail
Imports System.Net
Imports Chilkat
Imports System.IO
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates

Imports Chilkat.SFtp

Public Structure UserInfo
    Dim username As String
    Dim password As String
    Dim TeamName As String
    Dim LastName As String
    Dim FirstName As String
    Dim isLogged As Boolean
End Structure

Public Class frmLogin
    Private validatedFTP As Boolean = False
    Friend dbUser As String = AppDomain.CurrentDomain.BaseDirectory & "\Database\UserDB.mdb"
    Friend cnnstrUser As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbUser & ";Jet OLEDB:Database Password=jignesh0;"

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            TestFTP()
            Common.ResetVariable()
            REMIS_authentication()
        Catch ex As Exception
            MessageBox.Show("Login failed because the server did not respond. Please try again.")
        End Try
        
    End Sub

    Private Sub TestFTP()
        Dim success As Boolean
        Dim ftp As New Chilkat.Ftp2()

        ftp.Hostname = "upload.mygws.com"
        ftp.Username = TextBox_UserName.Text
        ftp.Password = TextBox_PassWord.Text

        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        success = ftp.Connect()

        
        If (success <> True) Then
            Common._isSFTP = True
        Else
            Common._isSFTP = False
        End If
    End Sub

    Private Sub REMIS_authentication()


        Common.GetGTPConfig()

        Dim LogFileName As String = TextBox_UserName.Text + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" _
            + DateTime.Now.Day.ToString() + "-" _
        + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt"

        Dim LogFolder = AppDomain.CurrentDomain.BaseDirectory & "\logs\"
        If Not Directory.Exists(LogFolder) Then
            Directory.CreateDirectory(LogFolder)
        End If
        LogFileName = LogFolder + LogFileName
        Common.LogFileName = LogFileName

        File.Create(LogFileName).Dispose()

        Common.hasWCFAccess = True

        REMIS_username = TextBox_UserName.Text.Trim
        REMIS_pass = TextBox_PassWord.Text.Trim

        Dim isValidated As Boolean = False

        If Common.isEnableSSL = True Then
            ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf ValidateCertificate)
        End If


        Dim proxy As GTPServiceClient
        proxy = New GTPServiceClient()
        Common.AssignCredentials(proxy)

        Dim currentVersion = New _GTPVersion()

        Try
            currentVersion = proxy.getVersion("", True)
        Catch ex As Exception
            Dim sss As String = ex.ToString()
            Common.AppendTextFile(Common.LogFileName, "", ex)
            Common.UploadFile(LogFileName, "GTPErrorLog")
            Common.hasWCFAccess = False
        End Try


        Try
            If proxy.State = ServiceModel.CommunicationState.Opened Then
                proxy.Close()

            End If
            proxy = New GTPServiceClient()

            proxy.Open()

        Catch ex As Exception
            'MessageBox.Show(ex.ToString())

            If Not CheckConnection("http://www.google.com") Then
                MessageBox.Show("There is no Internet connection. Please check !")

                Me.Close()
                Return
            Else
                Common.AppendTextFile(LogFileName, "Internet Lost", ex)
                Common.UploadFile(LogFileName, "GTPErrorLog")
                'MessageBox.Show("GWS FTP system is currently offline due to a critical failure.  Please try again in 15 minutes.")
                Common.hasWCFAccess = False
                'SendingEmailMessage("Code 1: " + ex.ToString())
            End If
        End Try

        'Authenticate using FTP
        Dim success As Boolean
        Dim ftp As New Chilkat.Ftp2()
        Dim sftp As New Chilkat.SFtp()

        If _isSFTP Then
            Dim port As Integer
            Dim hostname As String
            'hostname = "192.168.1.19"
            success = sftp.UnlockComponent("cqcmgfSSH_Qk2GPCJcjUnW")
            port = 22

            success = sftp.Connect("upload.mygws.com", port)
            success = sftp.AuthenticatePw(TextBox_UserName.Text.Trim, TextBox_PassWord.Text.Trim)

        Else


            ftp.Hostname = "upload.mygws.com"
            ftp.Username = TextBox_UserName.Text
            ftp.Password = TextBox_PassWord.Text

            success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
            success = ftp.Connect()
        End If
        

        validatedFTP = True

        If (success <> True) Then
            MsgBox("Log in Failed")
            If Not Common._isSFTP Then
                Common.AppendTextFile(LogFileName, ftp.LastErrorText, Nothing)
                Common.UploadFile(LogFileName, "GTPErrorLog")
            End If
            
            Return
        Else
            isREMIS_Authenticated = True
            RememberUserCredentials()
            frmMainApp.FTPUploadToolStripMenuItem.Enabled = False
            frmMainApp.CloseToolStripMenuItem.Enabled = False
        End If
        'success = ftp.Connect()
        If (success <> True) Then
            MessageBox.Show("Failed to FTP server. Please check the FTP server name, username and password")
            Return
        End If

        Try
            Dim DateStart As DateTime
            Dim DateEnd As DateTime

            Dim status As _DBStatus() = proxy.GetDBStatus(1)


            If status.Count() > 0 Then

                If IsNothing(status(0).MaintenanceStart) Then
                    DateStart = DateTime.MinValue
                Else
                    DateStart = status(0).MaintenanceStart
                End If

                If IsNothing(status(0).MaintenanceEnd) Then
                    DateEnd = DateTime.MaxValue
                Else
                    DateEnd = status(0).MaintenanceEnd
                End If

                Dim result1 As Integer = DateTime.Compare(DateTime.Now, DateStart)
                Dim result2 As Integer = DateTime.Compare(DateTime.Now, DateEnd)

                If result1 >= 0 And result2 <= 0 Then
                    MessageBox.Show(status(0).status_notify)
                    Me.Close()
                    'Return
                End If
            End If


            status = proxy.GetDBStatus(2)


            If status.Count() > 0 Then


                If IsNothing(status(0).MaintenanceStart) Then
                    DateStart = DateTime.MinValue
                Else
                    DateStart = status(0).MaintenanceStart
                End If

                If IsNothing(status(0).MaintenanceEnd) Then
                    DateEnd = DateTime.MaxValue
                Else
                    DateEnd = status(0).MaintenanceEnd
                End If



                Dim result1 As Integer = DateTime.Compare(DateTime.Now, DateStart)
                Dim result2 As Integer = DateTime.Compare(DateTime.Now, DateEnd)

                If result1 >= 0 And result2 <= 0 Then
                    MessageBox.Show(status(0).status_notify)
                End If
            End If

        Catch ex As Exception

            Common.AppendTextFile(LogFileName, "", ex)
            If Not _isSFTP Then
                Common.UploadFile(LogFileName, "GTPErrorLog")
            End If


            If Not CheckConnection("http://www.google.com") Then
                MessageBox.Show("There is no Internet connection. Please check !")
                Me.Close()
                Return
            Else
                'MessageBox.Show("GWS FTP system is currently offline due to a critical failure.  Please try again in 15 minutes.")
                SendingEmailMessage("Code 2: " + ex.ToString())
                Common.hasWCFAccess = False
                Me.Close()
            End If
        End Try

        'Check(Version)

        If Common.isCheckNewVersion Then
            Dim oldVersion As New _GTPVersion()

            oldVersion = proxy.getVersion(Common.VersionNumber, False)
            If oldVersion.id_version = 0 Then
                oldVersion.doAllowUpload = True
            End If

            currentVersion = New _GTPVersion()
            currentVersion = proxy.getVersion("", True)
            Dim DBversion As String = Common.VersionNumber

            If Not IsDBNull(currentVersion) Then
                DBversion = currentVersion.version
            End If


            If DBversion > Common.VersionNumber Then
                MessageBox.Show("Your current version is " + Common.VersionNumber + ". The latest version is " + DBversion + ". New installation files will be downloaded to C:\GWSFTPInstallation")
                DownloadFile("C:\GWSFTPInstallation\setup.exe", "setup.exe", "CurrentGTPRelease")
                DownloadFile("C:\GWSFTPInstallation\GWSFTP.msi", "GWSFTP.msi", "CurrentGTPRelease")
            End If


            If Not oldVersion.doAllowUpload Then
                Dim msgStr = "The installed GTP is version " + Common.VersionNumber + ". You MUST upgrade to the latest GTP before uploading data."
                MessageBox.Show(msgStr)
                Return
            End If
    

        End If
        
        Try
            isValidated = proxy.ValidateUser(REMIS_username, REMIS_pass)
        Catch ex As Exception
            Common.AppendTextFile(LogFileName, "", ex)
            If Not _isSFTP Then
                Common.UploadFile(LogFileName, "GTPErrorLog")
            End If

        End Try

        If CheckConnection("http://www.google.com") Then
            If validatedFTP Or isValidated Then

                isREMIS_Authenticated = True
                'Remember the password
                RememberUserCredentials()
                frmMainApp.FTPUploadToolStripMenuItem.Enabled = False
                frmMainApp.CloseToolStripMenuItem.Enabled = False

                Dim count As Integer = Common.GetTotalPendingTransactions()
                If count >= 2 Then
                    Dim pendingForm As New frmPendingTransaction()
                    pendingForm.Show()
                Else
                    Dim uploadForm As New frmUpload()
                    uploadForm.Text = "GTP " + Common.VersionNumber
                    uploadForm.Show()
                End If

                Me.Close()
            Else
                isREMIS_Authenticated = False
                frmMainApp.FTPUploadToolStripMenuItem.Enabled = False
                frmMainApp.CloseToolStripMenuItem.Enabled = True
                MessageBox.Show("Log in Failed, Please try again !")
            End If
        Else
            MessageBox.Show("There is no Internet connection. Please check !")
        End If

        proxy.Close()


    End Sub


    Private Sub SendingEmailMessage(ByVal error_message As String)
        'Sending an email to notify admin
        Try
            Dim MyComputerInfo As New HostComputer
            MyComputerInfo.HostName = ""
            MyComputerInfo.Ip_address = ""
            MyComputerInfo.MAC_address = ""
            Common.GetIPAddress(MyComputerInfo.HostName, MyComputerInfo.Ip_address, MyComputerInfo.MAC_address)

            Dim strTo As String = "tuand@gwsolutions.com"
            Dim strFrom As String = "gwsftp@gmail.com"
            Dim strSubject As String = "Reported: GWS FTP system is currently offline due to a critical failure.  Please try again in 15 minutes."
            Dim strContent As String = "Reported at DateTime: " + Date.Now + " Host name:" + MyComputerInfo.HostName + " IP address: " + MyComputerInfo.Ip_address + " Username: " + TextBox_UserName.Text.Trim

            Dim smtpClient As SmtpClient = New SmtpClient()
            Dim basicCredential As NetworkCredential = New NetworkCredential("gwsftp", "gwsftp@123")
            Dim message As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
            Dim fromAddress As System.Net.Mail.MailAddress = New System.Net.Mail.MailAddress(strFrom)

            smtpClient.Host = "smtp.gmail.com"
            smtpClient.Port = 587
            smtpClient.EnableSsl = True
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = basicCredential
            message.From = fromAddress
            message.Subject = strSubject
            message.Body = strContent + error_message
            message.To.Add(strTo)
            'message.To.Add("janardanag@gwsolutions.com")
            'message.To.Add("agarman@gwsolutions.com")
            'message.To.Add("jigart@gwsolutions.com")
            'message.To.Add("wretvica@gwsolutions.com")

            smtpClient.Send(message)
        Catch ex1 As Exception

        End Try
    End Sub

    Private Sub RememberUserCredentials()

        Dim username As String = TextBox_UserName.Text.Trim
        Dim password As String = TextBox_PassWord.Text.Trim

        Dim encrypted_password As String = ""

        If password <> "" Then
            encrypted_password = EncryptionClass.Class1.Encrypt(password)
        End If
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstrUser
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "DELETE FROM tblRememberMe "
        cnn.Open()
        cmd.ExecuteNonQuery()
        If CheckBox_RememberMe.Checked Then
            'Insert new credentials        
            cmd.CommandText = "INSERT INTO tblRememberMe([username],[password]) VALUES ('" & username & "','" & encrypted_password & "')"
            cmd.ExecuteNonQuery()
        End If
        cnn.Close()
    End Sub



    Private Sub TextBox_PassWord_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox_PassWord.KeyPress
        If e.KeyChar = Chr(13) Then
            Button1.PerformClick()
        End If
    End Sub

    Private Sub frmLogin_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            If Directory.Exists("D:\TempGTPUnzip") Then
                Dim fileToDelete As String() = Directory.GetFiles("D:\TempGTPUnzip")
                For Each fileStr In fileToDelete
                    Try
                        File.Delete(fileStr)
                    Catch ex As Exception

                    End Try

                Next

            End If
            If Directory.Exists("C:\TempGTPUnzip") Then
                Dim fileToDelete As String() = Directory.GetFiles("C:\TempGTPUnzip")
                For Each fileStr In fileToDelete
                    Try
                        File.Delete(fileStr)
                    Catch ex As Exception

                    End Try

                Next

            End If
            
        Catch ex As Exception

        End Try
        

        If Directory.Exists(Common.NetworkTempFolder) Then
            Dim tmpFiles() As String = Directory.GetFiles(Common.NetworkTempFolder)
            Dim tmpFile As String
            For Each tmpFile In tmpFiles
                Try
                    File.Delete(tmpFile)
                Catch ex As Exception

                End Try

            Next
        End If
        
        'checkDriveSpace()
        'Check if xml folder is exist
        GetUserName()
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstrUser
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "SELECT * FROM tblRememberMe "
        cnn.Open()
        dr = cmd.ExecuteReader()
        While dr.Read
            TextBox_UserName.Text = dr("username")
            Dim pass As String = dr("password")
            TextBox_PassWord.Text = EncryptionClass.Class1.Decrypt(pass)
            CheckBox_RememberMe.Checked = True
        End While
        dr.Close()
        cnn.Close()
    End Sub

    Private Sub GetUserName()
        For i = 1 To 9
            TextBox_UserName.Items.Add("TEAM0" + i.ToString())
        Next

        For i = 10 To 99
            TextBox_UserName.Items.Add("TEAM" + i.ToString())
        Next

        For i = 100 To 1000
            TextBox_UserName.Items.Add("TEAM" + i.ToString())
        Next

        TextBox_UserName.Items.Add("TEAMREMIS")

        'proxy.Close()
    End Sub

    Private Sub frmLogin_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        If e.KeyChar = Chr(13) Then
            Button1.PerformClick()
        End If
    End Sub

    Private Sub TextBox_UserName_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If e.KeyChar = Chr(13) Then
            Button1.PerformClick()
        End If
    End Sub

    Private Function validateLogin() As Boolean
        Dim ftp As New Chilkat.Ftp2()

        Dim success As Boolean

        ' Any string unlocks the component for the 1st 30-days.
        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            Return False
        End If

        ftp.Hostname = "upload.mygws.com"
        ftp.Username = TextBox_UserName.Text.Trim
        ftp.Password = TextBox_PassWord.Text.Trim

        ' The default data transfer mode is "Active" as opposed to "Passive".
        ftp.Passive = True
        ' Connect and login to the FTP server.
        success = ftp.Connect()
        If (success <> True) Then            
            Return False
        End If
        Return True
    End Function

    Private Sub DownloadFile(ByVal localFilename As String, ByVal remoteFilename As String, ByVal remote_dir As String)
        Dim ftp As New Chilkat.Ftp2()

        Dim success As Boolean

        ' Any string unlocks the component for the 1st 30-days.
        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            MsgBox(ftp.LastErrorText)
            Exit Sub
        End If
        ftp.Hostname = "upload.mygws.com"
        ftp.Username = "TeamREMIS"
        ftp.Password = "$MSR@mat"

        ' The default data transfer mode is "Active" as opposed to "Passive".
        ftp.Passive = True
        ' Connect and login to the FTP server.
        success = ftp.Connect()
        If (success <> True) Then
            MsgBox(ftp.LastErrorText)
            Exit Sub
        End If

        success = ftp.ChangeRemoteDir(remote_dir)
        If (success <> True) Then
            MsgBox(ftp.LastErrorText)
            Exit Sub
        End If
        ' Download a file.
        If Not Directory.Exists("C:\GWSFTPInstallation") Then
            Directory.CreateDirectory("C:\GWSFTPInstallation")
        End If
        success = ftp.GetFile(remoteFilename, localFilename)
        If (success <> True) Then
            MsgBox(ftp.LastErrorText)
            Exit Sub
        End If
        ftp.Disconnect()
    End Sub


   



    Private Function ValidateCertificate(ByVal sender As Object, ByVal cert As X509Certificate, ByVal chain As X509Chain, ByVal sslPolicyErrors As SslPolicyErrors)

        Return True

    End Function

End Class

