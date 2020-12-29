
Imports System.Net
Imports System.IO
Imports System.Data.OleDb
Imports GWSFTP.GTPService
Imports Chilkat



Public Class GWSFTP
    Public ftphost As String
    Public ftpusername As String
    Public ftppassword As String
    Public cnn As OleDbConnection
    Private dbFile As String = AppDomain.CurrentDomain.BaseDirectory & "\Database\FTPDB.mdb"
    Private cnnstr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbFile & ";Jet OLEDB:Database Password=jignesh0;"



    Public Sub ReNameFileFTP(ByVal OldFile As String, ByVal NewFile As String)
        Dim ftp As New Chilkat.Ftp2()

        Dim success As Boolean

        '  Any string unlocks the component for the 1st 30-days.
        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        ftp.Hostname = "upload.mygws.com"
        ftp.Username = "TeamREMIS"
        ftp.Password = "$MSR@mat"

        '  Connect and login to the FTP server.
        success = ftp.Connect()
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        '  Set the current remote directory to where the file
        '  is located:
        success = ftp.ChangeRemoteDir("/GTPXMLFiles")
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        '  Rename the remote file (or directory)
        Dim existingFilename As String
        existingFilename = OldFile
        Dim newFilename As String
        newFilename = NewFile
        success = ftp.RenameRemoteFile(existingFilename, newFilename)
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        ftp.Disconnect()
    End Sub


    Public Sub RenameFTPDir(ByVal OldDir As String, ByVal NewDir As String)
        Dim ftp As New Chilkat.Ftp2()

        Dim success As Boolean

        '  Any string unlocks the component for the 1st 30-days.
        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        ftp.Hostname = "upload.mygws.com"
        ftp.Username = "TeamREMIS"
        ftp.Password = "$MSR@mat"

        '  Connect and login to the FTP server.
        success = ftp.Connect()
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        '  Set the current remote directory to where the file
        '  is located:
        success = ftp.ChangeRemoteDir("/TeamREMIS_curUpload")
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        '  Rename the remote file (or directory)


        success = ftp.RenameRemoteFile(OldDir, NewDir)
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Exit Sub
        End If


        ftp.Disconnect()
    End Sub



    Public Sub GWSUpload(ByVal _FileList() As String, ByVal ftp_url As String, ByVal id_transaction As Integer)
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest
        Dim i As Integer
        For i = 0 To _FileList.Length - 1
            'Try
            FileName = _FileList(i).Substring(_FileList(i).LastIndexOf("\"), _FileList(i).Length - _FileList(i).LastIndexOf("\"))
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ftpusername, ftppassword)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(_FileList(i))
            Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()
            clsStream.Write(bFile, 0, bFile.Length)
            clsStream.Close()
            clsStream.Dispose()

            'Update tblFileInfo
            Dim cmd As OleDbCommand
            cmd = New OleDbCommand
            cnn.ConnectionString = cnnstr
            cmd.Connection = cnn
            cnn.Open()
            cmd.CommandText = "UPDATE tblFileInfo SET [is_finished] = TRUE, [date_finished]=Date()  WHERE id_transaction=" & id_transaction & " AND file_name ='" & _FileList(i) & "'"
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            cnn.Close()

            'Catch ex As Exception

            'End Try
        Next
    End Sub

    Public Function GWSUploadOneFile(ByRef uploadRate As Integer, ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer) As Integer

        Dim proxy As New GTPServiceClient

        Dim success As Boolean

        Dim finishedCode As Integer = 0
        Dim ftp As New Chilkat.Ftp2()

        ftp.Hostname = "upload.mygws.com"
        ftp.Username = ftpusername
        ftp.Password = ftppassword

        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            MsgBox(ftp.LastErrorText)
            Return 1
        End If
        ftp.KeepSessionLog = True

        ftp.Passive = True

        Try
            Dim FileName As String
            FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)

            Dim remote_dir = ftp_url.Substring(ftp_url.LastIndexOf("/") + 1, ftp_url.Length - ftp_url.LastIndexOf("/") - 1)
            remote_dir = ftpusername + "_curUpload/" + remote_dir

            success = ftp.Connect()
            If (success <> True) Then
                Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                If Not isConnected Then
                    finishedCode = 1
                Else
                    finishedCode = 2
                End If
                Return finishedCode
            End If


            success = ftp.ChangeRemoteDir(remote_dir)
            If (success <> True) Then
                Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                If Not isConnected Then
                    finishedCode = 1
                Else
                    finishedCode = 2
                End If
                Return finishedCode
            End If


            'Check if the file is on server and status in database
            Dim file_status As Integer = 0

            file_status = proxy.GetFileStatus(id_transaction, FiletoUpload)
            If file_status = 1 Then
                'Check file exist on server
                Dim fileSize As Integer = ftp.GetSizeByName(FiletoUpload)
                If (fileSize > 0) Then
                    Dim FIleUploadInfo = New FileInfo(FiletoUpload)
                    If FIleUploadInfo.Length = fileSize Then
                        finishedCode = -1
                        proxy.Close()
                        Return finishedCode
                    End If
                End If
            End If

            proxy.Close()


            ftp.ConnectTimeout = 60 * 1000 'Time out in second
            ftp.ReadTimeout = 60 * 3000 'Time out in second
            success = ftp.PutFile(FiletoUpload, FileName)
            FTPSessionLog += ftp.SessionLog


            If (success <> True) Then
                Try
                    If CheckConnection("http://www.google.com") Then
                        finishedCode = GWSUploadOneFileNET(FiletoUpload, ftp_url, id_transaction)
                    Else
                        finishedCode = 1
                        Return finishedCode
                    End If

                Catch ex As Exception

                End Try

                If finishedCode <> 0 Then
                    Try
                        Dim cmd As OleDbCommand
                        cnn = New OleDbConnection
                        cmd = New OleDbCommand
                        cmd.Connection = cnn
                        cnn.ConnectionString = cnnstr
                        cnn.Open()
                        cmd.CommandText = "UPDATE tblFileInfo SET [error_log] = '" + ftp.LastErrorText + "'   WHERE id_transaction=" & id_client_public & " AND file_name ='" & FiletoUpload & "'"
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                        cnn.Close()
                    Catch ex As Exception

                    End Try


                    Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                    If Not isConnected Then
                        finishedCode = 1
                        Return finishedCode
                    Else
                        finishedCode = 2
                    End If
                Else
                    'Compare Filesize
                    Dim fileSize_Local As Integer
                    Dim fileSize_Server As Integer
                    Dim FileUploadInfo As FileInfo
                    FileUploadInfo = New FileInfo(FiletoUpload)
                    fileSize_Local = FileUploadInfo.Length
                    fileSize_Server = ftp.GetSizeByName(FileName)

                    If fileSize_Local <> fileSize_Server Then
                        finishedCode = 2
                    End If
                End If

            End If
            ftp.Disconnect()
        Catch ex As Exception
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            If Not isConnected Then
                finishedCode = 1
            Else
                finishedCode = 2
            End If
        End Try
        Return finishedCode
    End Function



    Public Function GWSUploadOneFileNET(ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer) As Integer
        Dim finishedCode As Integer = 0
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest

        Try
            FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ftpusername, ftppassword)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            clsRequest.UseBinary = True
            clsRequest.KeepAlive = True
            clsRequest.Timeout = 1000 * 60 * 15

            Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)
            Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()
            Dim bbb As Boolean = clsStream.CanTimeout


            clsStream.Write(bFile, 0, bFile.Length)
            clsStream.Close()
            clsStream.Dispose()

            finishedCode = 0
        Catch ex As Exception
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            If Not isConnected Then
                finishedCode = 1
            Else
                finishedCode = 2
            End If

        End Try

        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        Return finishedCode

    End Function



    'Upload big file

    Public Function GWSUploadOneFileNETBig(ByRef uploadRate As Integer, ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer) As Integer
        Dim finishedCode As Integer = 0
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest

        Try
            FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ftpusername, ftppassword)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            clsRequest.KeepAlive = True
            clsRequest.UseBinary = True
            clsRequest.Timeout = 1000 * 60 * 10
            clsRequest.ReadWriteTimeout = 1000 * 60 * 15
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)


            clsRequest.ContentLength = bFile.Length

            Dim buffLength As Integer = 2048
            Dim buff() As Byte
            ReDim buff(buffLength)

            Dim contentLen As Integer
            Dim fileInf As FileInfo = New FileInfo(FiletoUpload)


            Dim fs As FileStream = fileInf.OpenRead()

            Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()

            Dim bbb As Boolean = clsStream.CanTimeout

            contentLen = fs.Read(buff, 0, buffLength)

            While contentLen <> 0
                clsStream.Write(buff, 0, contentLen)
                contentLen = fs.Read(buff, 0, buffLength)
            End While


            'clsStream.Write(bFile, 0, bFile.Length)
            Try
                fs.Close()
                clsStream.Close()
                clsStream.Dispose()

            Catch ex As Exception

            End Try


            finishedCode = 0
        Catch ex As Exception
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            If Not isConnected Then
                finishedCode = 1
            Else
                finishedCode = 2
            End If

        End Try

        If cnn.State = ConnectionState.Open Then
            cnn.Close()
        End If
        Return finishedCode
    End Function

    Public Function FTPRenameFileNet(ByVal FileToRename As String, ByVal NewName As String, ByVal ftp_url As String) As Boolean
        Dim finishedCode As Integer = 0
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest

        Try
            FileName = FileToRename.Substring(FileToRename.LastIndexOf("\") + 1, FileToRename.Length - FileToRename.LastIndexOf("\") - 1)
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ftpusername, ftppassword)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.Rename
            clsRequest.RenameTo = FileName
            clsRequest.GetResponse()
            finishedCode = 0
        Catch ex As Exception
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            If Not isConnected Then
                finishedCode = 1
            Else
                finishedCode = 2
            End If

        End Try

        Return finishedCode

    End Function

    Public Function GWSDeleteOneFile(ByVal FiletoDelete As String, ByVal ftp_url As String)
        Dim finishedCode As Integer = 0
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest

        Try
            FileName = FiletoDelete.Substring(FiletoDelete.LastIndexOf("\") + 1, FiletoDelete.Length - FiletoDelete.LastIndexOf("\") - 1)
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ftpusername, ftppassword)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.DeleteFile            
            clsRequest.GetResponse()
            finishedCode = 0
        Catch ex As Exception
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            If Not isConnected Then
                finishedCode = 1
            Else
                finishedCode = 2
            End If

        End Try

        Return finishedCode

    End Function

    Public Function GWSRemoveDir(ByVal ftp_url As String)
        Dim finishedCode As Integer = 0
        Dim clsRequest As System.Net.FtpWebRequest

        Try
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ftpusername, ftppassword)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.RemoveDirectory
            clsRequest.GetResponse()
            finishedCode = 0
        Catch ex As Exception
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            If Not isConnected Then
                finishedCode = 1
            Else
                finishedCode = 2
            End If

        End Try
        Return finishedCode

    End Function



    Public Sub GWSResumeUpload(ByVal cnn As OleDbConnection)
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        Dim id_transaction As Integer
        Dim ftp_url As String = ""
        Dim i As Integer

        cmd = New OleDbCommand
        cmd.Connection = cnn
        Dim FileRemaining() As String

        cmd.CommandText = "SELECT TOP 1 * FROM tblTransactionInfo WHERE isCompleted=FALSE "

        cnn.Open()
        dr = cmd.ExecuteReader()
        If dr.HasRows Then
            While dr.Read()
                id_transaction = Convert.ToInt32(dr("id_transaction"))
                ftp_url = dr("ftp_url")
            End While
            dr.Close()
        End If

        'Get remaining files
        cmd.CommandText = "SELECT * FROM tblFileInfo WHERE [id_transaction] =" & id_transaction & " AND  is_Finished=FALSE "

        'cnn.Open()
        dr = cmd.ExecuteReader()
        Dim count As Integer = -1
        Dim uploadStatus As Integer = 0
        If dr.HasRows Then
            While dr.Read()
                count += 1
                ReDim Preserve FileRemaining(count)
                FileRemaining(count) = dr("file_name")
            End While
            If Not IsNothing(FileRemaining) Then
                Dim uploadRate As Integer = 0
                For i = 0 To FileRemaining.Length - 1
                    uploadStatus = GWSUploadOneFile(uploadRate, FileRemaining(i), ftp_url, id_transaction)
                    If uploadStatus = 0 Then

                    ElseIf uploadStatus = 1 Then

                    ElseIf uploadStatus = 2 Then

                    End If
                Next
            End If
        End If
        cmd.Dispose()
    End Sub


    Public Sub GWSCreateFolder(ByVal FolderName As String)
        If _isSFTP Then
            Dim sFTPClass As New GWSSFTP()
            sFTPClass.CreateDirectory(FolderName)
        Else
            Dim FTPReq As System.Net.FtpWebRequest
            'FTPReq = DirectCast(FtpWebRequest.Create(New Uri("ftp://192.168.1.117" + "/" + FolderName)), System.Net.FtpWebRequest)
            FTPReq = DirectCast(FtpWebRequest.Create(New Uri(ftphost + "/" + FolderName)), System.Net.FtpWebRequest)
            FTPReq.Method = WebRequestMethods.Ftp.MakeDirectory
            FTPReq.UseBinary = True
            FTPReq.Credentials = New NetworkCredential(ftpusername, ftppassword)
            Dim response As System.Net.FtpWebResponse
            response = FTPReq.GetResponse()
            Dim ftpStream = response.GetResponseStream()
            ftpStream.Close()
            response.Close()
        End If
        

        'Dim ftp As New Chilkat.Ftp2()

        'Dim success As Boolean

        '' Any string unlocks the component for the 1st 30-days.
        'success = ftp.UnlockComponent("Anything for 30-day trial")
        'If (success <> True) Then
        '    MsgBox(ftp.LastErrorText)
        '    Exit Sub
        'End If


        'ftp.Hostname = "upload.mygws.com"
        'ftp.Username = ftpusername
        'ftp.Password = ftppassword

        '' Connect and login to the FTP server.
        'success = ftp.Connect()
        'If (success <> True) Then
        '    MsgBox(ftp.LastErrorText)
        '    Exit Sub
        'End If


        '' Create a new directory on the FTP server:
        'success = ftp.CreateRemoteDir("newDirName")
        'If (success <> True) Then
        '    MsgBox(ftp.LastErrorText)
        '    Exit Sub
        'End If

    End Sub


    'Public Function CheckFolder(ByVal FolderName As String) As Boolean
    '    Dim isExist As Boolean = False
    '    Dim request = DirectCast(WebRequest.Create(ftphost + "/" + FolderName), FtpWebRequest)
    '    request.Credentials = New NetworkCredential(ftpusername, ftppassword)
    '    request.Method = WebRequestMethods.Ftp.ListDirectory
    '    Try
    '        Using response As FtpWebResponse = DirectCast(request.GetResponse(), FtpWebResponse)
    '            If response.StatusCode = 0 Then
    '                isExist = False
    '            Else
    '                isExist = True
    '            End If
    '        End Using

    '    Catch ex As Exception
    '        isExist = False
    '    End Try

    '    Return isExist
    'End Function

    Public Function CheckFolder(ByVal FolderName As String) As Boolean
        If Not _isSFTP Then

            Dim ftp As New Chilkat.Ftp2()

            Dim success As Boolean

            '  Any string unlocks the component for the 1st 30-days.
            success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
            If (success <> True) Then
                MsgBox(ftp.LastErrorText)
                Return False
            End If


            ftp.Hostname = "upload.mygws.com"
            ftp.Username = Common.FTPUserName
            ftp.Password = Common.FTPPass

            Dim currentFolder As String = ftphost.Substring(ftphost.LastIndexOf("/") + 1, ftphost.Length - ftphost.LastIndexOf("/") - 1)
            '  Connect and login to the FTP server.
            success = ftp.Connect()
            If (success <> True) Then
                MsgBox(ftp.LastErrorText)
                Return False
            End If



            Dim dirExists As Boolean = True

            dirExists = ftp.ChangeRemoteDir(currentFolder)
            dirExists = ftp.ChangeRemoteDir(FolderName)


            Return dirExists
        Else
            Return True
        End If


    End Function


    Public Function CheckConnection(ByVal strURL As String) As Boolean
        Dim objUrl As New System.Uri(strURL)
        Dim status As Boolean = False

        ' Setup WebRequest
        Dim objWebReq As System.Net.WebRequest
        objWebReq = System.Net.WebRequest.Create(objUrl)
        Dim objResp As System.Net.WebResponse
        Try
            ' Attempt to get response and return True
            objResp = objWebReq.GetResponse
            objResp.Close()
            objWebReq = Nothing
            status = True
        Catch ex As Exception
            ' Error, exit and return False
            'objResp.Close()

            objWebReq = Nothing
            status = False
        End Try
        Return status
    End Function


End Class
