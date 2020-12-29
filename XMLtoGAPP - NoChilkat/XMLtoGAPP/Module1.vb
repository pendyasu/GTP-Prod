Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
'Imports Chilkat

Module Module1
    Private FTPUserName As String
    Private FTPPassword As String
    Private connectionString As String = "Data Source=GAPP-FTP;Initial Catalog=GAPP;user id=sa;password=gtp6B--ds$U; MultipleActiveResultSets=True"
    Private ftpConnection As String = "ftp://upload.mygws.com/GTPXMLFiles"
    Private ftpConnectionProcessed As String = "ftp://upload.mygws.com/GTPXMLFiles-Processed"
    Private FTPRootUrl As String = "ftp://upload.mygws.com/"
    Private xmlFolder As String = ""
    Private LogFileName As String

    Sub Main()



        Dim ClientType As String = ""

        Dim TransactionGID As Guid
        Dim id_server_transaction As Integer
        Dim id_client_transaction As Integer
        Dim transaction_type As Boolean
        Dim date_started As DateTime
        Dim date_end As String
        Dim username As String
        Dim isCompleted As Boolean
        Dim isProcessed As Boolean
        Dim number_of_run As Integer
        Dim number_of_app_fail As Integer
        Dim id_market As Integer
        Dim ftp_url As String
        Dim ip_address As String
        Dim host_name As String
        Dim market_name As String = ""
        Dim transaction_status As Integer
        Dim DBName As String
        Dim total_file_uploaded As Integer
        Dim campaign As String
        Dim Project As String
        Dim FTPFolder As String = ""
        Dim totalinputbytes As String
        Dim totalbytesuploaded As Double

        Dim isScanner As Boolean
        Dim isAsideOrphan As Boolean
        Dim BSideDeviceType As String





        'tblFTP File variable
        Dim XMLFileName As String
        Dim FileGID As Guid
        Dim id_server_file As Integer
        Dim file_name As String
        Dim date_created As DateTime
        Dim date_finished As DateTime
        Dim is_finished As Boolean
        Dim file_status As Integer
        Dim status_flag As Integer
        Dim file_name_only As String
        Dim bytestrnsfrd As String
        Dim uploadtime As String


        ReadXMLConfigFile()

        checkXMLFormat()

      

        LogFileName = "XML_TO_SQL_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" _
          + DateTime.Now.Day.ToString() + "-" _
          + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt"

        Dim LogFolder = AppDomain.CurrentDomain.BaseDirectory & "\logs\"
        If Not Directory.Exists(LogFolder) Then
            Directory.CreateDirectory(LogFolder)
        End If

        LogFileName = LogFolder + LogFileName




        Dim xmlFiles As String() = Directory.GetFiles(xmlFolder)
        'Dim xmlFiles As String() = Directory.GetFiles(xmlFolder)
        Dim file As String

        For Each file In xmlFiles
         
            Try

                isCompleted = False
                isProcessed = False
                Dim FTPElement As XmlNode
                Dim FileInfomation As New System.IO.FileInfo(file)
                Dim FileName As String = FileInfomation.Name
                XMLFileName = FileName

                If file.IndexOf("Processed") < 0 Then
                    'If file.IndexOf("T41_Abilene_TX_d75be799-0224-431d-8c9a-7990ab93d08f.xml") >= 0 Then
                    '    Dim tt As String = ""
                    'End If
                    Dim GappCon As SqlConnection
                    Dim cmd As SqlCommand
                    GappCon = New SqlConnection

                    'GappCon.ConnectionString = connectionString
                    GappCon.ConnectionString = connectionString


                    GappCon.Open()

                    Dim xmlDoc As New XmlDocument

                    Try
                        xmlDoc.Load(file)
                    Catch ex As Exception
                        If Not System.IO.File.Exists(LogFileName) Then
                            System.IO.File.Create(LogFileName).Dispose()
                        Else
                            AppendTextFile(LogFileName, ex.ToString())
                            'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                        End If
                        GoTo Label1
                    End Try



                    FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")

                    If Not IsNothing(FTPElement) Then
                        Try
                            ClientType = FTPElement.Attributes("GTPType").Value
                        Catch ex As Exception

                        End Try

                        TransactionGID = Guid.Parse(FTPElement.Attributes("TransactionGID").Value)


                        FTPUserName = FTPElement.Attributes("FTPUserName").Value
                        If ClientType <> "Android" Then
                            FTPPassword = AES_Decrypt(FTPElement.Attributes("FTPPassword").Value.ToString(), "TeamREMIS$MSR@mat")
                            id_server_transaction = FTPElement.Attributes("id_server_transaction").Value
                            id_client_transaction = FTPElement.Attributes("id_client_transaction").Value
                            transaction_type = FTPElement.Attributes("transaction_type").Value
                            number_of_run = FTPElement.Attributes("number_of_run").Value
                            number_of_app_fail = FTPElement.Attributes("number_of_app_fail").Value
                            total_file_uploaded = FTPElement.Attributes("total_file_uploaded").Value
                            id_client_transaction = FTPElement.Attributes("id_client_transaction").Value
                        Else
                            FTPPassword = FTPElement.Attributes("FTPPassword").Value.ToString()
                            id_client_transaction = 0
                            number_of_run = 0
                            number_of_app_fail = 0
                            total_file_uploaded = 0
                            id_client_transaction = 0
                        End If



                        date_started = FTPElement.Attributes("date_started").Value
                        date_end = FTPElement.Attributes("date_end").Value
                        username = FTPElement.Attributes("username").Value

                        If FTPElement.Attributes("isCompleted").Value.ToLower = "True".ToLower Then
                            isCompleted = True
                        Else
                            isCompleted = False
                        End If


                        ftp_url = FTPElement.Attributes("ftp_url").Value
                        ip_address = FTPElement.Attributes("ip_address").Value
                        host_name = FTPElement.Attributes("host_name").Value
                        market_name = FTPElement.Attributes("market_name").Value
                        transaction_status = FTPElement.Attributes("transaction_status").Value
                        DBName = FTPElement.Attributes("DBName").Value

                        campaign = FTPElement.Attributes("campaign").Value
                        username = FTPUserName


                        Project = FTPElement.Attributes("project").Value

                        Project = Project.Replace("%26", "&")
                        Project = Project.Replace("+", " ")

                        FTPFolder = FTPElement.Attributes("FTPFolder").Value

                        If FTPElement.Attributes("totalinputbytes").Value.Trim <> "" Then
                            totalinputbytes = FTPElement.Attributes("totalinputbytes").Value
                        Else
                            totalinputbytes = 0
                        End If


                        If FTPElement.Attributes("id_market").Value.Trim = "" Then
                            id_market = GetMarketID(market_name, Project)
                        Else
                            id_market = FTPElement.Attributes("id_market").Value
                            If id_market = 0 Then
                                id_market = GetMarketID(market_name, Project)
                            End If
                        End If


                        cmd = New SqlCommand
                        cmd.Connection = GappCon

                        cmd.CommandText = "SELECT * FROM tblFTPTransaction WHERE TransactionGID =  '" & TransactionGID.ToString() & "'"
                        Dim dr As SqlDataReader

                        dr = cmd.ExecuteReader()
                        'Update tblFTPTransaction SET isCompleted='False', date_end= 1/1/0001 12:00:00 AM,transaction_status=0, total_file_uploaded =0 where TransactionGID=1779c679-d159-4ac5-a0df-e291e9613243
                        If dr.HasRows Then

                            While dr.Read
                                id_server_transaction = Convert.ToInt32(dr("id_server_transaction"))



                                If Convert.ToInt32(dr("transaction_status")) >= 3 Then
                                    'Modify the transaction status in xml file
                                    transaction_status = Convert.ToInt32(dr("transaction_status"))
                                    FTPElement.Attributes("transaction_status").Value = transaction_status.ToString()

                                    isProcessed = True

                                End If
                            End While

                            'isProcessed = False 'Forced
                            If isProcessed Then
                                GoTo Label2
                            End If
                            'Update tblFTPTransaction


                            cmd.CommandText = "Update tblFTPTransaction SET number_of_run = @number_of_run , number_of_app_fail=@number_of_app_fail, " _
                                & " isCompleted=@isCompleted, date_end= @date_end,transaction_status=@transaction_status, total_file_uploaded =@total_file_uploaded " _
                                & " where TransactionGID=@TransactionGID "



                            If (date_end.ToString().IndexOf("1900") > 0) Or (date_end.ToString() = "12:00:00 AM") Then
                                cmd.Parameters.AddWithValue("@date_end", DBNull.Value)
                            Else
                                cmd.Parameters.AddWithValue("@date_end", date_end)
                            End If



                            cmd.Parameters.AddWithValue("@isCompleted", isCompleted)
                            cmd.Parameters.AddWithValue("@number_of_run", number_of_run)
                            cmd.Parameters.AddWithValue("@number_of_app_fail", number_of_app_fail)
                            cmd.Parameters.AddWithValue("@transaction_status", transaction_status)
                            cmd.Parameters.AddWithValue("@total_file_uploaded", total_file_uploaded)
                            cmd.Parameters.AddWithValue("@TransactionGID", TransactionGID.ToString())
                            cmd.Parameters.AddWithValue("@id_market", id_market.ToString())
                        Else


                            cmd.CommandText = "INSERT INTO tblFTPTransaction(id_client_transaction,transaction_type,date_started,date_end,username,isCompleted,number_of_run," _
                                & "number_of_app_fail,id_market,ftp_url,ip_address,host_name,market_name,transaction_status,DBName,total_file_uploaded,campaign, TransactionGID, Project, FTPFolder, totalinputbytes, isXMLMode) " _
                                & " VALUES (@id_client_transaction,@transaction_type,@date_started,@date_end,@username,@isCompleted,@number_of_run," _
                                & "@number_of_app_fail,@id_market,@ftp_url,@ip_address,@host_name,@market_name,@transaction_status,@DBName,@total_file_uploaded,@campaign, @TransactionGID, @Project, @FTPFolder, @totalinputbytes, @isXMLMode) "

                            cmd.Parameters.AddWithValue("@id_client_transaction", id_client_transaction)
                            cmd.Parameters.AddWithValue("@transaction_type", transaction_type)
                            cmd.Parameters.AddWithValue("@date_started", date_started)
                            If (date_end.ToString().IndexOf("1900") > 0) Or (date_end.ToString() = "12:00:00 AM") Then
                                cmd.Parameters.AddWithValue("@date_end", DBNull.Value)
                            Else
                                cmd.Parameters.AddWithValue("@date_end", date_end)
                            End If

                            cmd.Parameters.AddWithValue("@username", username)
                            cmd.Parameters.AddWithValue("@isCompleted", isCompleted)
                            cmd.Parameters.AddWithValue("@number_of_run", number_of_run)
                            cmd.Parameters.AddWithValue("@number_of_app_fail", number_of_app_fail)
                            cmd.Parameters.AddWithValue("@id_market", id_market)

                            cmd.Parameters.AddWithValue("@ftp_url", ftp_url)
                            cmd.Parameters.AddWithValue("@ip_address", ip_address)
                            cmd.Parameters.AddWithValue("@host_name", host_name)
                            cmd.Parameters.AddWithValue("@market_name", market_name)

                            cmd.Parameters.AddWithValue("@transaction_status", transaction_status)
                            cmd.Parameters.AddWithValue("@DBName", DBName)
                            cmd.Parameters.AddWithValue("@total_file_uploaded", total_file_uploaded)
                            cmd.Parameters.AddWithValue("@campaign", campaign)
                            cmd.Parameters.AddWithValue("@TransactionGID", TransactionGID)
                            cmd.Parameters.AddWithValue("@Project", Project)
                            cmd.Parameters.AddWithValue("@FTPFolder", FTPFolder)
                            cmd.Parameters.AddWithValue("@totalinputbytes", totalinputbytes)
                            cmd.Parameters.AddWithValue("@isXMLMode", 1)

                        End If

                        dr.Close()
                        dr.Dispose()
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()

                    End If

                    'Change FTP Folder format

                    cmd = New SqlCommand()
                    cmd.Connection = GappCon
                    cmd.CommandText = "SELECT * FROM tblFTPTransaction WHERE TransactionGID='" + TransactionGID.ToString() + "'"


                    Dim dr1 As SqlDataReader
                    dr1 = cmd.ExecuteReader()

                    While dr1.Read()
                        id_server_transaction = Convert.ToInt32(dr1("id_server_transaction"))
                        transaction_status = Convert.ToInt32(dr1("transaction_status"))
                        FTPFolder = Convert.ToString(dr1("FTPFolder"))
                        FTPElement.Attributes("id_server_transaction").Value = id_server_transaction
                        FTPElement.Attributes("transaction_status").Value = transaction_status
                    End While

                    dr1.Close()
                    cmd.Dispose()


                    'Update/Insert Files
                    Dim FileNodes As XmlNodeList
                    Dim node As XmlNode
                    FileNodes = xmlDoc.SelectNodes("/tblFTPTransaction/tblFTPFile")

                    'transaction_status = 0 'Forced

                    If transaction_status < 3 Then

                        If Not IsNothing(FileNodes) Then
                            For Each node In FileNodes

                                Try

                                    Dim aa As String = node.Attributes("FileGID").Value
                                    FileGID = Guid.Parse(node.Attributes("FileGID").Value)
                                    If (node.Attributes("id_server_file").Value.Trim <> "") Then
                                        id_server_file = node.Attributes("id_server_file").Value
                                    Else
                                        id_server_file = 0
                                    End If


                                    file_name = node.Attributes("file_name").Value
                                    date_created = DateTime.Parse(node.Attributes("date_created").Value)
                                    date_finished = DateTime.Parse(node.Attributes("date_finished").Value)
                                    is_finished = Convert.ToBoolean(node.Attributes("is_finished").Value)
                                    file_status = Convert.ToInt32(node.Attributes("file_status").Value)

                                    file_name_only = node.Attributes("file_name_only").Value
                                    bytestrnsfrd = node.Attributes("bytestrnsfrd").Value

                                    isScanner = GetBooleanValue(node.Attributes("isScanner").Value)
                                    isAsideOrphan = GetBooleanValue(node.Attributes("isAsideOrphan").Value)

                                    BSideDeviceType = node.Attributes("BSideDeviceType").Value

                                    If bytestrnsfrd.Trim = "" Then
                                        bytestrnsfrd = -1
                                    End If
                                    uploadtime = node.Attributes("uploadtime").Value
                                    If uploadtime.Trim() = "" Then
                                        uploadtime = "-1"
                                    End If

                                    Dim Date_Collected As String = ""
                                    Dim tmp As String

                                    If file_name_only = "" Then
                                        file_name_only = file_name.Substring(file_name.LastIndexOf("/") + 1, file_name.Length - file_name.LastIndexOf("/") - 1)
                                    End If
                                    tmp = file_name_only.Substring(0, 10)


                                    Dim tmpDate As DateTime

                                    If DateTime.TryParse(tmp, tmpDate) Then
                                        Date_Collected = Convert.ToString(Convert.ToDateTime(tmp))
                                    End If


                                    cmd = New SqlCommand()
                                    cmd.Connection = GappCon
                                    cmd.CommandText = "SELECT * FROM tblFTPFile WHERE FileGID ='" & FileGID.ToString() & "'"
                                    Dim dr As SqlDataReader
                                    dr = cmd.ExecuteReader()
                                    If dr.HasRows Then

                                        While dr.Read()


                                            file_status = Convert.ToInt32(dr("file_status"))
                                            If file_status = 0 Then
                                                file_status = Convert.ToInt32(node.Attributes("file_status").Value)
                                            End If

                                            'Check on FTP server
                                            If file_status = 0 Then
                                                Try

                                                    Dim fileSizeUploaded As Long = GetFileSizeNET(file_name_only, FTPRootUrl + FTPUserName + "_curUpload/" + FTPFolder, FTPUserName, FTPPassword)

                                                    If fileSizeUploaded = bytestrnsfrd Then
                                                        If file_status = 0 Then
                                                            file_status = 1
                                                        End If
                                                    End If
                                                Catch ex As Exception
                                                    AppendTextFile(LogFileName, ex.ToString())
                                                End Try
                                            End If


                                        End While


                                        cmd.CommandText = "UPDATE tblFTPFile SET isScanner=@isScanner, isAsideOrphan=@isAsideOrphan, BSideDeviceType=@BSideDeviceType,  file_status=@file_status,date_finished=@date_finished, is_finished=@is_finished , uploadtime=@uploadtime WHERE FileGID=@FileGID "
                                        If file_status = 1 Then
                                            cmd.Parameters.AddWithValue("@is_finished", True)
                                        Else
                                            cmd.Parameters.AddWithValue("@is_finished", is_finished)
                                        End If

                                        cmd.Parameters.AddWithValue("@file_status", file_status)
                                        If date_finished = "1/1/0001 12:00:00 AM" And file_status = 0 Then
                                            cmd.Parameters.AddWithValue("@date_finished", DBNull.Value)
                                        ElseIf file_status = 1 Then
                                            'cmd.Parameters.AddWithValue("@date_finished", DateTime.Now.ToString())
                                            cmd.Parameters.AddWithValue("@date_finished", date_finished)

                                        Else
                                            cmd.Parameters.AddWithValue("@date_finished", date_finished)
                                        End If


                                        cmd.Parameters.AddWithValue("@uploadtime", uploadtime)
                                        cmd.Parameters.AddWithValue("@FileGID", FileGID)

                                        cmd.Parameters.AddWithValue("@isScanner", isScanner)
                                        cmd.Parameters.AddWithValue("@isAsideOrphan", isAsideOrphan)
                                        cmd.Parameters.AddWithValue("@BSideDeviceType", BSideDeviceType)


                                    Else
                                        'Insert File to Database

                                        cmd.CommandText = "INSERT INTO tblFTPFile(id_server_transaction, id_client_transaction, file_name,date_created,date_finished,is_finished, file_status,file_name_only," _
                                     & " FileGID, Date_Collected, TransactionGID,bytestrnsfrd,uploadtime, status_flag, isScanner,isAsideOrphan, BSideDeviceType, FileNameNoExtension) VALUES (@id_server_transaction,@id_client_transaction," _
                                     & " @file_name,@date_created,@date_finished,@is_finished, @file_status, " _
                                     & " @file_name_only,@FileGID,@Date_Collected,@TransactionGID,@bytestrnsfrd,@uploadtime, @status_flag, @isScanner ,@isAsideOrphan, @BSideDeviceType, @FileNameNoExtension)"


                                        If file_status = 9 Then
                                            cmd.Parameters.AddWithValue("@status_flag", 9)
                                            cmd.Parameters.AddWithValue("@file_status", 1)
                                        Else
                                            cmd.Parameters.AddWithValue("@status_flag", DBNull.Value)
                                            cmd.Parameters.AddWithValue("@file_status", file_status)
                                        End If

                                        If date_finished = "1/1/0001 12:00:00 AM" Then
                                            cmd.Parameters.AddWithValue("@date_finished", DBNull.Value)
                                        Else
                                            cmd.Parameters.AddWithValue("@date_finished", date_finished)
                                        End If
                                        cmd.Parameters.AddWithValue("@is_finished", is_finished)
                                        cmd.Parameters.AddWithValue("@uploadtime", uploadtime)
                                        cmd.Parameters.AddWithValue("@FileGID", FileGID)

                                        cmd.Parameters.AddWithValue("@id_server_transaction", id_server_transaction)
                                        cmd.Parameters.AddWithValue("@id_client_transaction", id_client_transaction)
                                        cmd.Parameters.AddWithValue("@file_name", file_name)
                                        cmd.Parameters.AddWithValue("@date_created", date_created)
                                        cmd.Parameters.AddWithValue("@file_name_only", file_name_only)
                                        cmd.Parameters.AddWithValue("@Date_Collected", Date_Collected)
                                        cmd.Parameters.AddWithValue("@TransactionGID", TransactionGID)
                                        cmd.Parameters.AddWithValue("@bytestrnsfrd", bytestrnsfrd)

                                        cmd.Parameters.AddWithValue("@isScanner", isScanner)
                                        cmd.Parameters.AddWithValue("@isAsideOrphan", isAsideOrphan)
                                        cmd.Parameters.AddWithValue("@BSideDeviceType", BSideDeviceType)
                                        cmd.Parameters.AddWithValue("@FileNameNoExtension", Path.GetFileNameWithoutExtension(file_name))

                                    End If

                                    dr.Close()
                                    dr.Dispose()
                                    ''1/1/0001 12:00:00 AM'
                                    If file_status < 3 Or file_status = 9 Then
                                        cmd.ExecuteNonQuery()
                                        cmd.Dispose()

                                    End If

                                Catch ex As Exception
                                    If Not System.IO.File.Exists(LogFileName) Then
                                        System.IO.File.Create(LogFileName).Dispose()
                                    Else
                                        AppendTextFile(LogFileName, ex.ToString())
                                        'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                                    End If
                                End Try
                            Next


                            'Update total_file_uploaded

                            'update tblFTPTransaction set total_file_uploaded=(select count(*) from tblFTPFile where file_status=1 and TransactionGID='14C99A58-3CB8-4E20-9B48-481027BD0E76')
                            'where TransactionGID='14C99A58-3CB8-4E20-9B48-481027BD0E76'
                            cmd = New SqlCommand()
                            cmd.CommandText = "UPDATE tblFTPTransaction set total_file_uploaded=(select count(*) from tblFTPFile where (file_status=1 OR file_status>=3) and TransactionGID = '" + TransactionGID.ToString() + "') WHERE TransactionGID = '" + TransactionGID.ToString() + "'"
                            cmd.Connection = GappCon

                            cmd.ExecuteNonQuery()
                            cmd.Dispose()



Label2:

                            Try
                                If isCompleted Or transaction_status = 2 Then

                                    'Update totalbytesuploaded
                                    totalbytesuploaded = 0

                                    Dim dr2 As SqlDataReader

                                    Dim cmd2 As New SqlCommand()
                                    cmd2.Connection = GappCon
                                    cmd2.CommandText = "SELECT * FROM tblFTPFile where file_status =1 AND TransactionGID = '" + TransactionGID.ToString() + "'"
                                    dr2 = cmd2.ExecuteReader()
                                    While dr2.Read()

                                        If Convert.ToInt32(dr2("bytestrnsfrd")) > 0 Then
                                            Try
                                                totalbytesuploaded += Convert.ToDouble(dr2("bytestrnsfrd"))
                                            Catch ex As Exception

                                            End Try

                                        End If
                                    End While


                                    dr2.Close()
                                    dr2.Dispose()
                                    cmd2.Dispose()
                                    'Rename FTP Folder

                                    'RenameFTPDir(FTPFolder, id_server_transaction.ToString() + "-" + market_name)
                                    'dr1.Close()

                                    Try
                                        Dim finishedCode As Integer = FTPRenameFileNet(FTPFolder, id_server_transaction.ToString() + "-" + market_name, FTPRootUrl + FTPUserName + "_curUpload", FTPUserName, FTPPassword)

                                        If finishedCode = 0 Then
                                            cmd = New SqlCommand()
                                            cmd.Connection = GappCon
                                            cmd.CommandText = "UPDATE tblFTPTransaction SET FTPFolder='" + id_server_transaction.ToString() + "-" + market_name + "' WHERE TransactionGID = '" + TransactionGID.ToString() + "'"
                                            cmd.ExecuteNonQuery()
                                            cmd.Dispose()



                                            FTPElement.Attributes("FTPFolder").Value = id_server_transaction.ToString() + "-" + market_name

                                            Try
                                                If Not IsNothing(totalbytesuploaded) Then
                                                    cmd = New SqlCommand()
                                                    cmd.Connection = GappCon
                                                    cmd.CommandText = "UPDATE tblFTPTransaction SET totalbytesuploaded = " + totalbytesuploaded.ToString() + " WHERE TransactionGID = '" + TransactionGID.ToString() + "'"
                                                    cmd.ExecuteNonQuery()

                                                    cmd.Dispose()
                                                End If
                                            Catch ex As Exception

                                            End Try


                                        End If
                                    Catch ex As Exception
                                        If Not System.IO.File.Exists(LogFileName) Then
                                            System.IO.File.Create(LogFileName).Dispose()
                                        End If
                                        Try
                                            AppendTextFile(LogFileName, ex.ToString())
                                            'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                                        Catch ex1 As Exception

                                        End Try
                                    End Try
                                End If
                            Catch ex As Exception
                                If Not System.IO.File.Exists(LogFileName) Then
                                    System.IO.File.Create(LogFileName).Dispose()
                                End If

                                Try
                                    AppendTextFile(LogFileName, ex.ToString())
                                    'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                                Catch ex1 As Exception

                                End Try
                            End Try


                        End If


                    End If



                    Try
                        If isProcessed Then
                            FTPRenameFileNet(FileName, FileName.Replace(".xml", "_Processed.xml"), ftpConnection, "TeamREMIS", "$MSR@mat")
                            Try
                                GWSUploadOneFileNET(AppDomain.CurrentDomain.BaseDirectory + "\xml\" + FileName.Replace(".xml", "_Processed.xml"), ftpConnectionProcessed, "TeamREMIS", "$MSR@mat")
                            Catch

                            End Try


                        Else
                            'Save .xml fle
                            If Not Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\xml\") Then
                                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\xml\")

                            End If
                            xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "\xml\" + XMLFileName)

                            'GWSUploadOneFileNET(AppDomain.CurrentDomain.BaseDirectory + "\xml\" + XMLFileName, "ftp://upload.mygws.com/GTPXMLFiles", "TeamREMIS", "$MSR@mat")
                        End If
                    Catch ex As Exception
                        If Not System.IO.File.Exists(LogFileName) Then
                            System.IO.File.Create(LogFileName).Dispose()
                        Else

                        End If

                        AppendTextFile(LogFileName, ex.ToString())
                        'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                    End Try

                    GappCon.Close()


                Else
                    'Move Processed file to different folder



                End If
            Catch ex As Exception
                If Not System.IO.File.Exists(LogFileName) Then
                    System.IO.File.Create(LogFileName).Dispose()
                End If
                Try
                    AppendTextFile(LogFileName, ex.ToString())
                    'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                Catch ex1 As Exception

                End Try
            End Try
Label1:
        Next
    End Sub

    Private Sub checkXMLFormat()

        Dim xmlFiles As String() = Directory.GetFiles(xmlFolder)
        If Not Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\tmp\") Then
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\tmp\")

        End If
        Dim file As String

        For Each file In xmlFiles

            Dim fileInfo As New FileInfo(file)
            If file.ToLower.IndexOf("_processed.") < 0 And fileInfo.Length > 0 Then
                Dim fileNameOnly As String = file.Substring(file.LastIndexOf("\") + 1, file.Length - file.LastIndexOf("\") - 1)

                Dim tmpFileName = AppDomain.CurrentDomain.BaseDirectory + "\tmp\" + fileNameOnly

                If Not System.IO.File.Exists(tmpFileName) Then
                    System.IO.File.CreateText(tmpFileName).Close()
                Else
                    Dim tmpFileInfo As New FileInfo(tmpFileName)
                    tmpFileInfo.Delete()
                    System.IO.File.CreateText(tmpFileName).Close()
                End If

                Dim sw As StreamWriter = New StreamWriter(tmpFileName)

                Dim isMissing As Boolean = False


                Dim textLine As String = ""
                Dim objReader As New StreamReader(file)
                Do While objReader.Peek() <> -1
                    Dim isWrite As Boolean = True

                    textLine = objReader.ReadLine()
                    If (Not textLine.Trim.EndsWith("/>")) And (Not textLine.StartsWith("<?xml version=")) And textLine.IndexOf("tblFTPTransaction") < 0 Then
                        If Not textLine.EndsWith("""") Then
                            textLine = textLine + """"
                        End If
                        '<tblFTPFile FileGID="877b5099-2cbb-4cf6-9923-3e58fd9a8b9f" id_server_file="0" id_server_transaction="0" id_client_transaction="244" 
                        'file_name="C:\Users\GWS\Desktop\FONDDULAC_WI_083115\2015-08-31 18-05-59\2015-08-31-18-05-59-0221-0020-0003-0221-B.sqz" date_created="8/31/2015 6:47:59 PM" 
                        'date_finished="1/1/0001 12:00:00 AM" is_finished="False" file_status="0" status_flag="0" file_name_only="2015-08-31-18-05-59-0221-0020-0003-0221-B.sqz" 
                        'dbname="" bytestrnsfrd="7463082" uploadtime="" />
                        If Not textLine.IndexOf("id_server_file") > 0 Then
                            textLine = textLine + "id_server_file=""""0"""""
                        End If
                        If Not textLine.IndexOf("id_server_transaction") > 0 Then
                            textLine = textLine + "id_server_transaction=""""0"""""
                        End If
                        If Not textLine.IndexOf("id_server_file") > 0 Then
                            textLine = textLine + "id_server_transaction=""""0"""""
                        End If
                        If textLine.IndexOf("file_name") < 0 Then
                            isWrite = False
                        End If
                        If Not textLine.IndexOf("date_created") > 0 Then
                            textLine = textLine + "date_created=" + DateTime.Now.ToString()
                        End If

                        If Not textLine.IndexOf("date_finished") > 0 Then
                            textLine = textLine + "date_finished=" + DateTime.Now.ToString()
                        End If

                        If Not textLine.IndexOf("file_status") > 0 Then
                            textLine = textLine + "file_status=""""0"""""
                        End If

                        If Not textLine.IndexOf("file_name_only") > 0 Then
                            isWrite = False
                        End If

                        If Not textLine.IndexOf("dbname") > 0 Then
                            textLine = textLine + "dbname="""""""""
                        End If

                        If Not textLine.IndexOf("dbname") > 0 Then
                            textLine = textLine + "dbname="""""""""
                        End If
                        If Not textLine.IndexOf("bytestrnsfrd") > 0 Then
                            textLine = textLine + "bytestrnsfrd=""""0"""""
                        End If
                        If Not textLine.IndexOf("uploadtime") > 0 Then
                            textLine = textLine + "uploadtime="""""""""
                        End If

                        textLine = textLine + "/>"
                        isMissing = True
                    End If
                    If isWrite Then
                        sw.WriteLine(textLine)
                    End If

                Loop

                If Not textLine.Trim() = "</tblFTPTransaction>" Then
                    sw.WriteLine("</tblFTPTransaction>")
                    isMissing = True
                End If
                objReader.Close()
                objReader.Dispose()
                sw.Close()

                If Not isMissing Then
                    Dim tmpFileInfo As New FileInfo(tmpFileName)
                    tmpFileInfo.Delete()
                Else
                    'Upload File to FTP server T20_Fond_du_Lac_WI_0017bb00-6cc8-4baf-9aa2-3fa610ed9c3b.xml
                    'My.Computer.FileSystem.RenameFile(tmpFileName, fileNameOnly.Replace(".xml", ".tmp"))
                    deleteFileOnFTP(ftpConnection, fileNameOnly, "TeamREMIS", "$MSR@mat")
                    'DeleteFileonFTP(fileNameOnly, "GTPXMLFiles")
                    GWSUploadOneFileNET(tmpFileName, ftpConnection, "TeamREMIS", "$MSR@mat")
                End If
            End If


        Next
    End Sub


    Private Function AppendTextFile(ByVal FileName As String, ByVal strText As String) As Boolean
        Dim objWriter As New StreamWriter(FileName, True)
        objWriter.WriteLine(DateTime.Now.ToString() + " --- " + strText)
        objWriter.Close()
        objWriter.Dispose()
        Return True
    End Function


    Private Function GetMarketID(ByVal market_name As String, ByVal Project As String) As Integer
        'AT%26T+BM
        Dim id_market As Integer = 0
        Dim status_market As Integer = 0
        Project = Project.Replace("%26", "&")
        Project = Project.Replace("+", " ")

        If Project = "AT&T BM" Then
            status_market = 2
        ElseIf Project = "AT&T E911" Then
            status_market = 3
        ElseIf Project = "AT&T MRAB" Then
            status_market = 7
        ElseIf Project = "BELL BM" Then
            status_market = 4
        ElseIf Project = "TIGO BM" Then
            status_market = 5
        ElseIf Project = "UAE BM" Then
            status_market = 5
        ElseIf Project = "Netherlands" Then
            status_market = 8
        End If

        market_name = market_name.Substring(0, market_name.LastIndexOf(" ")) + ", " + market_name.Substring(market_name.LastIndexOf(" ") + 1, market_name.Length - 1 - market_name.LastIndexOf(" "))

        Dim con As New SqlConnection
        con.ConnectionString = connectionString
        con.Open()
        Dim cmd As New SqlCommand()
        cmd.Connection = con
        cmd.CommandText = "SELECT * FROM tblMarket where name_market='" + market_name + "' AND market_status=" + status_market.ToString()
        Dim dr As SqlDataReader

        dr = cmd.ExecuteReader()

        While dr.Read()
            id_market = Convert.ToInt32(dr("id_market"))
        End While
        cmd.Dispose()
        con.Close()
        Return id_market

    End Function


    'Public Function DeleteFileonFTP(ByVal FiletoDelete As String, ByVal RemoteDir As String) As Boolean
    '    Try
    '        FiletoDelete = "T28_Salt_Lake_City_UT_cdc24ace-b7fe-42a8-93e1-49c7b9ae26c0_Processed.xml"
    '        Dim success As Boolean
    '        Dim FileName As String = FiletoDelete.Substring(FiletoDelete.LastIndexOf("\") + 1, FiletoDelete.Length - FiletoDelete.LastIndexOf("\") - 1)
    '        Dim finishedCode As Integer = 0
    '        Dim ftp As New Chilkat.Ftp2()
    '        ftp.Hostname = "upload.mygws.com"
    '        ftp.Username = "TeamREMIS"
    '        ftp.Password = "$MSR@mat"
    '        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
    '        If (success <> True) Then
    '            Return False
    '        End If
    '        success = ftp.Connect()
    '        If (success <> True) Then
    '            Return False
    '        End If

    '        success = ftp.ChangeRemoteDir(RemoteDir + "/")
    '        If (success <> True) Then
    '            MsgBox(ftp.LastErrorText)
    '            Return False
    '        End If

    '        Dim fileSize As Long
    '        fileSize = ftp.GetSizeByName(FiletoDelete)
    '        If (fileSize > 0) Then
    '            success = ftp.DeleteRemoteFile(FiletoDelete)
    '            If (success <> True) Then
    '                Dim tt As String = ftp.LastErrorText
    '                Return False
    '            End If
    '        End If

    '        ftp.Disconnect()
    '        Return success
    '    Catch ex As Exception
    '        Return False
    '    End Try

    'End Function



    Private Function deleteFileOnFTP(ByVal ftp_url As String, ByVal FileName As String, ByVal Username As String, ByVal Password As String) As Boolean
        Try
            'FileName = "T20_Janesville-Beloit_WI_9a847fcb-5bc5-465d-a63a-dd4ce75738a8_Processed.xml"
            Dim clsRequest As System.Net.FtpWebRequest
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.DeleteFile
            clsRequest.Credentials = New System.Net.NetworkCredential(Username, Password)
            'clsRequest.UseBinary = False
            clsRequest.UsePassive = True
            Dim response As FtpWebResponse = CType(clsRequest.GetResponse(), FtpWebResponse)
            response = clsRequest.GetResponse()
            response.Close()
        Catch ex As Exception
            Dim tt As String = ""

        End Try




        Return True

    End Function

    Public Function GWSUploadOneFileNET(ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal Username As String, ByVal Password As String) As Integer
        Dim finishedCode As Integer = 0
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest

        Try
            FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(Username, Password)
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

        End Try

        Return finishedCode

    End Function

    Public Function GetFileSizeNET(ByVal FileName As String, ByVal RemoteDir As String, ByVal FTPUserName As String, ByVal FTPPassword As String) As Long


        Dim clsRequest As System.Net.FtpWebRequest
        clsRequest = DirectCast(System.Net.WebRequest.Create(RemoteDir + "/" & FileName), System.Net.FtpWebRequest)
        clsRequest.Credentials = New System.Net.NetworkCredential(FTPUserName, FTPPassword)
        clsRequest.Method = WebRequestMethods.Ftp.GetFileSize
        clsRequest.UseBinary = True
        Dim size As Long = clsRequest.GetResponse().ContentLength
        Return size

    End Function

    Public Function FTPRenameFileNet(ByVal FileToRename As String, ByVal NewName As String, ByVal ftp_url As String, ByVal Username As String, ByVal Password As String) As Integer
        Dim finishedCode As Integer = 0

        If FileToRename.ToLower() <> NewName.ToLower() Then
            Try
                Dim clsRequest As System.Net.FtpWebRequest
                clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileToRename), System.Net.FtpWebRequest)
                clsRequest.Credentials = New System.Net.NetworkCredential(Username, Password)
                clsRequest.Method = System.Net.WebRequestMethods.Ftp.Rename
                clsRequest.RenameTo = NewName
                clsRequest.GetResponse()
                finishedCode = 0
            Catch ex As Exception
                finishedCode = 1
                If Not System.IO.File.Exists(LogFileName) Then
                    System.IO.File.Create(LogFileName).Dispose()
                Else

                End If

                AppendTextFile(LogFileName, ex.ToString())
                'GWSUploadOneFileNET(LogFileName, "ftp://upload.mygws.com/GTPErrorLog", "TeamREMIS", "$MSR@mat")
                Return finishedCode
            End Try
        End If

        Return finishedCode

    End Function

    Private Function GetBooleanValue(ByVal input As String) As Boolean
        Dim tmpInt As Integer
        Dim tmpBool As Boolean
        Dim ouput As Boolean

        If Integer.TryParse(input, tmpInt) Then
            ouput = Convert.ToInt32(input)
        Else
            If Boolean.TryParse(input, tmpBool) Then
                ouput = Convert.ToBoolean(input)
            End If
        End If

        If ouput = False Then
            ouput = 0
        Else
            ouput = 1
        End If
        Return ouput
    End Function

    Public Function AES_Decrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim decrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = System.Security.Cryptography.CipherMode.ECB
            Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = Convert.FromBase64String(input)
            decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return decrypted
        Catch ex As Exception
            Return ""
        End Try
    End Function

   

#Region "XML Cofig Reading"
    ''' <summary>
    ''' Reads the records from the Config file
    ''' </summary>
    ''' <returns>True => if successfully processed</returns>
    ''' <remarks></remarks>
    Private Function ReadXMLConfigFile() As Boolean
        Dim xmlDocument As XmlDocument
        Dim xmlNode As XmlNode
        Try
            xmlDocument = New XmlDocument
            xmlDocument.Load("XML_TO_SQL_config.xml")


            xmlNode = xmlDocument.SelectSingleNode("/ROOT/ASideConfigSettings/ServerSettings/DBConnection")
            connectionString = xmlNode.InnerText

            xmlNode = xmlDocument.SelectSingleNode("/ROOT/ASideConfigSettings/FolderSettings/XMLFolder")
            xmlFolder = xmlNode.InnerText
            Try
                xmlNode = xmlDocument.SelectSingleNode("/ROOT/ASideConfigSettings/ServerSettings/FTPConnection")
                ftpConnection = xmlNode.InnerText

                xmlNode = xmlDocument.SelectSingleNode("/ROOT/ASideConfigSettings/ServerSettings/FTPConnection-Processed")
                ftpConnectionProcessed = xmlNode.InnerText
            Catch ex As Exception

            End Try
            

            xmlNode = xmlDocument.SelectSingleNode("/ROOT/ASideConfigSettings/ServerSettings/FTPRootUrl")
            FTPRootUrl = xmlNode.InnerText


            Return True
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
            'Console.Read()
            Return False
        Finally
            xmlDocument = Nothing
            xmlNode = Nothing
        End Try
    End Function
#End Region



End Module
