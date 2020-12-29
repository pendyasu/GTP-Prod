Imports Chilkat.SFtp

Public Class GWSSFTP


    Public Function UploadFileGeneral(ByVal FiletoUpload As String, ByVal remote_dir As String) As Boolean
        Dim sftp As New Chilkat.SFtp

        Dim FileName As String
        FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
        'Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)

        '  Any string automatically begins a fully-functional 30-day trial.
        Dim success As Boolean = sftp.UnlockComponent("cqcmgfSSH_Qk2GPCJcjUnW")
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Set some timeouts, in milliseconds:
        sftp.ConnectTimeoutMs = 5000
        sftp.IdleTimeoutMs = 10000

        '  Connect to the SSH server.
        '  The standard SSH port = 22
        '  The hostname may be a hostname or IP address.
        Dim port As Integer
        Dim hostname As String
        'hostname = "192.168.1.19"
        hostname = Common.FTPServerName
        port = 22
        success = sftp.Connect("upload.mygws.com", port)
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Authenticate with the SSH server.  Chilkat SFTP supports
        '  both password-based authenication as well as public-key
        '  authentication.  This example uses password authenication.
        'success = sftp.AuthenticatePw("tester", "password")
        success = sftp.AuthenticatePw(Common.FTPUserName, Common.FTPPass)
        'success = sftp.AuthenticatePw("TeamREMIS", "$MSR@mat")
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  After authenticating, the SFTP subsystem must be initialized:
        success = sftp.InitializeSftp()
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If

        Dim absPath As String
        absPath = sftp.RealPath(".", "")

        '  Open a file for writing on the SSH server.
        '  If the file already exists, it is overwritten.
        '  (Specify "createNew" instead of "createTruncate" to
        '  prevent overwriting existing files.)
        Dim FTPFileName As String = ""

        If remote_dir.EndsWith("/") Then
            FTPFileName = remote_dir + FileName
        Else
            FTPFileName = remote_dir + "/" + FileName
        End If
        Dim handle As String = sftp.OpenFile(FTPFileName, "writeOnly", "createTruncate")
        'If (sftp.LastMethodSuccess <> True) Then
        '    Console.WriteLine(sftp.LastErrorText)
        '    Exit Sub
        'End If


        '  Upload from the local file to the SSH server.
        success = sftp.UploadFile(handle, FiletoUpload)

        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Close the file.
        success = sftp.CloseHandle(handle)
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If
        FTPSessionLog += sftp.LastErrorText
        Return True
    End Function


    Public Function UploadFile(ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer, ByVal LocalFileSize As Double, ByVal process As Integer) As Boolean
        Dim sftp As New Chilkat.SFtp

        Dim FileName As String
        FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
        'Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)

        Dim remote_dir = ftp_url.Substring(ftp_url.LastIndexOf("/") + 1, ftp_url.Length - ftp_url.LastIndexOf("/") - 1)
        remote_dir = Common.FTPUserName + "_curUpload/" + remote_dir


        '  Any string automatically begins a fully-functional 30-day trial.
        Dim success As Boolean = sftp.UnlockComponent("cqcmgfSSH_Qk2GPCJcjUnW")
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Set some timeouts, in milliseconds:
        sftp.ConnectTimeoutMs = 5000
        sftp.IdleTimeoutMs = 10000

        '  Connect to the SSH server.
        '  The standard SSH port = 22
        '  The hostname may be a hostname or IP address.
        Dim port As Integer
        Dim hostname As String
        'hostname = "192.168.1.19"
        hostname = Common.FTPServerName
        port = 22
        success = sftp.Connect("upload.mygws.com", port)
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Authenticate with the SSH server.  Chilkat SFTP supports
        '  both password-based authenication as well as public-key
        '  authentication.  This example uses password authenication.
        'success = sftp.AuthenticatePw("tester", "password")
        success = sftp.AuthenticatePw(Common.FTPUserName, Common.FTPPass)
        'success = sftp.AuthenticatePw("TeamREMIS", "$MSR@mat")
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  After authenticating, the SFTP subsystem must be initialized:
        success = sftp.InitializeSftp()
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If

        Dim absPath As String
        absPath = sftp.RealPath(".", "")

        '  Open a file for writing on the SSH server.
        '  If the file already exists, it is overwritten.
        '  (Specify "createNew" instead of "createTruncate" to
        '  prevent overwriting existing files.)
        Dim FTPFileName As String = ""

        If remote_dir.EndsWith("/") Then
            FTPFileName = remote_dir + FileName
        Else
            FTPFileName = remote_dir + "/" + FileName
        End If
        Dim handle As String = sftp.OpenFile(FTPFileName, "writeOnly", "createTruncate")
        'If (sftp.LastMethodSuccess <> True) Then
        '    Console.WriteLine(sftp.LastErrorText)
        '    Exit Sub
        'End If


        '  Upload from the local file to the SSH server.
        success = sftp.UploadFile(handle, FiletoUpload)

        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Close the file.
        success = sftp.CloseHandle(handle)
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Return False
        End If
        FTPSessionLog += sftp.LastErrorText
        Return True
    End Function

    Public Function CreateDirectory(ByVal DirName As String) As Boolean
        Dim sftp As New Chilkat.SFtp

        '  Any string automatically begins a fully-functional 30-day trial.
        Dim success As Boolean = sftp.UnlockComponent("cqcmgfSSH_Qk2GPCJcjUnW")
        If (success <> True) Then
            'Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Set some timeouts, in milliseconds:
        sftp.ConnectTimeoutMs = 15000
        sftp.IdleTimeoutMs = 15000

        '  Connect to the SSH server.
        '  The standard SSH port = 22
        '  The hostname may be a hostname or IP address.
        Dim port As Integer = 22
        Dim hostname As String
        success = sftp.Connect("upload.mygws.com", port)
        If (success <> True) Then
            'Console.WriteLine(sftp.LastErrorText)
        End If


        '  Authenticate with the SSH server.  Chilkat SFTP supports
        '  both password-based authenication as well as public-key
        '  authentication.  This example uses password authenication.
        'success = sftp.AuthenticatePw("tester", "password")
        success = sftp.AuthenticatePw(Common.FTPUserName, Common.FTPPass)
        If (success <> True) Then
            'Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  After authenticating, the SFTP subsystem must be initialized:
        success = sftp.InitializeSftp()
        If (success <> True) Then
            'Console.WriteLine(sftp.LastErrorText)
            Return False
        End If


        '  Create a new directory:
        success = sftp.CreateDir(DirName)
        If (success <> True) Then
            'Console.WriteLine(sftp.LastErrorText)
            Return False
        End If
        Return True
    End Function

    Public Sub ChangeDir(ByVal DirName)
        Dim sftp As New Chilkat.SFtp

        '  Any string automatically begins a fully-functional 30-day trial.
        Dim success As Boolean = sftp.UnlockComponent("cqcmgfSSH_Qk2GPCJcjUnW")
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Exit Sub
        End If


        '  Set some timeouts, in milliseconds:
        sftp.ConnectTimeoutMs = 5000
        sftp.IdleTimeoutMs = 10000

        '  Connect to the SSH server.
        '  The standard SSH port = 22
        '  The hostname may be a hostname or IP address.
        Dim port As Integer
        Dim hostname As String
        hostname = "192.168.1.19"
        port = 22
        success = sftp.Connect(hostname, port)
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Exit Sub
        End If


        '  Authenticate with the SSH server.  Chilkat SFTP supports
        '  both password-based authenication as well as public-key
        '  authentication.  This example uses password authenication.
        success = sftp.AuthenticatePw("tester", "password")
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Exit Sub
        End If


        '  After authenticating, the SFTP subsystem must be initialized:
        success = sftp.InitializeSftp()
        If (success <> True) Then
            Console.WriteLine(sftp.LastErrorText)
            Exit Sub
        End If


        '  To find the full path of our user account's home directory,
        '  call RealPath like this:
        Dim absPath As String
        absPath = sftp.RealPath(".", "")
        'If (sftp.LastMethodSuccess <> True) Then
        '    Console.WriteLine(sftp.LastErrorText)
        '    Exit Sub
        'Else
        '    Console.WriteLine(absPath)
        'End If
    End Sub
End Class
