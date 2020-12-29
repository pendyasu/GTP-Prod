Imports System.Data.SQLite
Imports Chilkat
Imports System.IO
Imports SevenZip

Public Class FileTrimming

    Public TempFolder As String

    Public Structure FileTrimmingInfo
        Public FileName As String
        Public FileType As String
        Public TempFolder As String
        Public isScanner As Integer
        Public isNetworkFile As Boolean
        Public TempFileName As String
        Public isCorrupted As Boolean
        Public BSideFileType As String
        Public isAsideOrphan As Integer
    End Structure

    Public Function processMFFile(ByVal FileName As String, ByVal isOriginal As Boolean) As FileTrimmingInfo
        Dim fileOutputInfo As New FileTrimmingInfo()
        If FileName.StartsWith("\\") Then
            Try
                If Not Directory.Exists(Common.NetworkTempFolder) Then
                    Directory.CreateDirectory(Common.NetworkTempFolder)
                End If
                Dim FileNameOnly As String = FileName.Substring(FileName.LastIndexOf("\") + 1, FileName.Length - FileName.LastIndexOf("\") - 1)
                File.Copy(FileName, Common.NetworkTempFolder + "\" + FileNameOnly, True)
                FileName = Common.NetworkTempFolder + "\" + FileNameOnly
                'fileOutputInfo.FileName = FileName    
                Try                    
                    Dim oFileInfo As New System.IO.FileInfo(FileName)
                    If (oFileInfo.Attributes And System.IO.FileAttributes.ReadOnly) > 0 Then
                        oFileInfo.Attributes = oFileInfo.Attributes Xor System.IO.FileAttributes.ReadOnly
                    End If
                Catch ex As Exception

                End Try
                fileOutputInfo.isNetworkFile = True
            Catch ex As Exception
                fileOutputInfo.FileName = FileName
                fileOutputInfo.FileType = "MF-Original"
                fileOutputInfo.TempFolder = ""

                Return fileOutputInfo
            End Try
            
        End If

        fileOutputInfo.isAsideOrphan = 0

        Try
            fileOutputInfo.isScanner = 0
            Dim con As New SQLiteConnection
            con.ConnectionString = "Data Source =" + FileName + ";Version=3;New=False;Compress=True;"
            Dim cmd As New SQLiteCommand

            'Get the BSideFile Information
            If FileName.ToLower.EndsWith("a.mf") Then
                con.Open()

                cmd = New SQLiteCommand()
                cmd.Connection = con
                'Select * from message where messagetype=1004 and (messagedata like '%Configuration Sync%' or messagedata like '%Config call success%' or messagedata like '%ACK!%')
                cmd.CommandText = "Select count(*) from message where messagetype=1004 and (messagedata like '%Configuration Sync%' or messagedata like '%Config call success%' or messagedata like '%ACK!%' or messagedata like '%Speech Double Ended%') AND (NOT messagedata like '%Speech Double Ended Config%')"
                Dim count11 As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count11 > 0 Then
                    fileOutputInfo.isAsideOrphan = 0
                Else
                    fileOutputInfo.isAsideOrphan = 1
                End If

                cmd.Dispose()

                Dim count2 As Integer = 0
                cmd = New SQLiteCommand()
                cmd.Connection = con
                cmd.CommandText = "select count(*) from message where messagedata like '%Config Call%'"
                count2 = cmd.ExecuteScalar()
                cmd.Dispose()


                Dim count3 As Integer = 0
                cmd = New SQLiteCommand()
                cmd.Connection = con
                cmd.CommandText = "select count(*) from message where messagedata like '%Double Ended Call%'"
                count3 = cmd.ExecuteScalar()
                cmd.Dispose()


                Dim configuration As String = ""
                cmd = New SQLiteCommand()
                cmd.Connection = con
                cmd.CommandText = "select * from PCM;"
                Dim drConfig As SQLiteDataReader
                drConfig = cmd.ExecuteReader()
                While drConfig.HasRows
                    While drConfig.Read
                        configuration = Convert.ToString(drConfig("configuration"))
                    End While

                End While
                drConfig.Close()
                cmd.Dispose()


                cmd = New SQLiteCommand()
                cmd.Connection = con
                cmd.CommandText = "select * from Measurement"
                Dim drHeader As SQLiteDataReader
                drHeader = cmd.ExecuteReader()
                If drHeader.HasRows Then
                    While drHeader.Read
                        Dim header As String = Convert.ToString(drHeader("FileHeader"))

                        Dim TextLines() As String = header.Split(System.Environment.NewLine)
                        Dim line As String

                        For Each line In TextLines
                            If line.Trim = "UniqueDeviceName=" Or line Like "*UniqueDeviceName*PSTN*" Then
                                fileOutputInfo.BSideFileType = "PSTN"
                                Exit For

                            End If
                        Next

                        If Not fileOutputInfo.BSideFileType = "PSTN" Then
                            If _Project.Trim = "UK BM" Then
                                fileOutputInfo.BSideFileType = "Mobile"
                            Else
                                If header.IndexOf("[B-Device]") < 0 Then

                                    If configuration.IndexOf("<UnitType>QP") > 0 And header.IndexOf("Double Ended Call") > 0 And count2 = 0 Then
                                        fileOutputInfo.BSideFileType = "Mobile"
                                    Else
                                        fileOutputInfo.BSideFileType = ""
                                    End If
                                ElseIf header Like "*[B-Device]*UniqueDeviceName=*PSTN*" And header.IndexOf("[B-Device]") >= 0 And header.IndexOf("PSTN") >= 0 Then
                                    fileOutputInfo.BSideFileType = "PSTN"
                                ElseIf header Like "*[B-Device]*UniqueDeviceName=*" And header.IndexOf("[B-Device]") >= 0 Then
                                    fileOutputInfo.BSideFileType = "Mobile"

                                End If

                                If fileOutputInfo.BSideFileType = "" Then
                                    If configuration.IndexOf("<UnitType>QP") > 0 And count3 > 0 And count2 = 0 Then
                                        fileOutputInfo.BSideFileType = "Mobile"
                                    End If


                                End If


                            End If

                        End If

                    End While
                End If
                drHeader.Close()
                cmd.Dispose()
                con.Close()
            End If

            cmd = New SQLiteCommand()
            cmd.Connection = con
            con.Open()
            cmd.CommandText = "SELECT count(*) FROM message WHERE messageType = 12288"
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            cmd.Dispose()
            con.Close()

            If count = 0 Then
                fileOutputInfo.FileName = FileName
                fileOutputInfo.FileType = "MF-Original"
                Return fileOutputInfo
            Else

                cmd = New SQLiteCommand()
                cmd.Connection = con
                con.Open()

                '(Select * from Measurement where FileHeader like '%JobType=19%') OR
                '(Select * from Measurement where FileHeader like '%TaskName%Scanner%')


                cmd.CommandText = "SELECT * FROM Measurement WHERE FileHeader like '%JobType=19%' OR  FileHeader like '%TaskName%Scanner%'"
                Dim dr As SQLiteDataReader
                dr = cmd.ExecuteReader()
                If dr.HasRows Then
                    fileOutputInfo.FileType = "Scanner only"
                Else
                    fileOutputInfo.FileType = "Device File"

                    'While dr.Read()
                    '    Dim enc As New System.Text.ASCIIEncoding
                    '    Dim data As String = enc.GetString(dr("MessageData"))

                    '    If data Like "*JobType=190*" Or data Like "*JobType=""Scanner only""*" Then

                    '        fileOutputInfo.FileType = "Scanner only"
                    '    Else

                    '        fileOutputInfo.FileType = "Device File"

                    '    End If
                    'End While

                End If
                dr.Close()
                cmd.Dispose()
                con.Close()

                If fileOutputInfo.FileType = "Scanner only" Then
                    fileOutputInfo.isScanner = 1
                    con = New SQLiteConnection
                    con.ConnectionString = "Data Source =" + FileName + ";Version=3;New=False;Compress=True;"
                    con.Open()
                    cmd = New SQLiteCommand()
                    cmd.Connection = con
                    cmd.CommandText = "SELECT count(*) FROM Message WHERE MessageType = 10002 or MessageType = 10012 or MessageType = 10105 or MessageType = 10106 or MessageType = 10109"
                    Dim count1 As Integer = Convert.ToInt32(cmd.ExecuteScalar())



                    If count1 = 0 Then
                        fileOutputInfo.FileName = FileName
                        fileOutputInfo.FileType = "MF-Original"
                        fileOutputInfo.TempFolder = ""
                        Return fileOutputInfo
                    End If
                    Dim tmpFileName As String = FileName
                    If isOriginal Then

                        Dim DriveName As String = FileName.Substring(0, 1)
                        If DriveName.StartsWith("\") Then
                            DriveName = "C"
                        End If

                        If Not Directory.Exists(DriveName + ":\TempGTPUnzip\") Then
                            Directory.CreateDirectory(DriveName + ":\TempGTPUnzip\")
                        End If
                        Common.currentTempGTPUnzip = DriveName + ":\TempGTPUnzip\"
                        tmpFileName = DriveName + ":\TempGTPUnzip\" + Common.GetFileNameOnly(FileName)

                        'Directory.CreateDirectory(FileName.Replace(".", "_"))
                        'fileOutputInfo.TempFolder = FileName.Replace(".", "_")
                        'tmpFileName = FileName.Replace(".", "_") + "\" + FileName.Substring(FileName.LastIndexOf("\") + 1)
                        System.IO.File.Copy(FileName, tmpFileName, True)
                    End If

                    con = New SQLiteConnection
                    con.ConnectionString = "Data Source =" + tmpFileName + ";Version=3;New=False;Compress=True;"
                    con.Open()
                    cmd = New SQLiteCommand()
                    cmd.Connection = con
                    cmd.CommandText = "DELETE FROM Message WHERE MessageType = 10012 or MessageType = 10106 or MessageType = 10109"

                    'Dim startTime As DateTime = DateTime.Now
                    cmd.ExecuteNonQuery()

                    'Dim endTime As DateTime = DateTime.Now
                    'Dim totalDeleteTime As Integer = (endTime - startTime).TotalSeconds


                    'cmd = New SQLiteCommand()
                    'cmd.Connection = con

                    'cmd.CommandText = "Select COUNT(*) FROM Message WHERE MessageType LIKE '%10104%' AND MessageData LIKE '%,,,,,,,,%'"
                    'Dim count10104 As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    'cmd.Dispose()
                    'Dim idList As New List(Of String)

                    'Dim startEcIoTime As DateTime = DateTime.Now                    
                    'Dim tmpStartTime As DateTime

                    'If count10104 > 0 Then
                    'cmd = New SQLiteCommand()
                    'cmd.Connection = con

                    'cmd.CommandText = "SELECT ID, MessageData, MessageType FROM Message WHERE MessageType = 10103 or (MessageType = 10104 AND MessageData LIKE '%,,,,,,,,%') ORDER BY ID"
                    'Dim dr1 As SQLiteDataReader = cmd.ExecuteReader()
                    'Dim msgID As String
                    'Dim msgData As Byte()
                    'Dim msgType As String
                    'Dim enc As New System.Text.ASCIIEncoding
                    'Dim isFirst10104 As Boolean
                    'Dim isCurrent10103 As Boolean

                    'tmpStartTime = DateTime.Now
                    'Dim strMsgData As String()

                    'While (dr1.Read())
                    '    msgID = Convert.ToString(dr1("ID"))
                    '    isFirst10104 = False
                    '    msgType = Convert.ToString(dr1("MessageType"))
                    '    msgData = dr1("MessageData")

                    '    If msgType = "10103" Then
                    '        isCurrent10103 = True
                    '    End If
                    '    If msgType = "10104" Then
                    '        If isCurrent10103 Then
                    '            isFirst10104 = True
                    '            isCurrent10103 = False
                    '        Else
                    '            isFirst10104 = False
                    '            'Check the Ec/Io here
                    '            strMsgData = enc.GetString(msgData).Split(",")
                    '            Dim EcIo As String = strMsgData(2)
                    '            Dim tmpDouble As Double
                    '            If Double.TryParse(EcIo, tmpDouble) Then
                    '                If Convert.ToDouble(EcIo) <= -17 Then
                    '                    idList.Add(msgID)

                    '                End If
                    '            End If
                    '        End If
                    '    End If
                    'End While

                    'Dim countList As Integer = 0
                    'Dim strBuilder As New System.Text.StringBuilder()
                    'Dim i As Integer
                    'Dim strIDList As String = ""
                    'Dim t1 As Integer = 1
                    'While countList < idList.Count
                    '    strBuilder.Append(idList(countList))
                    '    countList = countList + 1
                    '    If (countList >= t1 * 1000000) Or countList = idList.Count Then
                    '        strIDList = "(" + strBuilder.ToString() + ")"
                    '        cmd = New SQLiteCommand()
                    '        cmd.Connection = con
                    '        cmd.CommandText = "DELETE FROM Message WHERE ID IN " + strIDList
                    '        cmd.ExecuteNonQuery()
                    '        cmd.Dispose()
                    '        strBuilder = New System.Text.StringBuilder()
                    '        strIDList = ""
                    '        t1 = t1 + 1
                    '    Else
                    '        strBuilder.Append(",")
                    '    End If

                    'End While

                    'End If

                    'Compact database
                    cmd = New SQLiteCommand()
                    cmd.Connection = con
                    cmd.CommandText = "vacuum;"
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()

                    con.Close()
                    con.Dispose()
                    fileOutputInfo.FileName = tmpFileName

                ElseIf fileOutputInfo.FileType = "Device File" Then
                    Dim tmpFileName As String = FileName
                    If isOriginal Then
                        Directory.CreateDirectory(FileName.Replace(".", "_"))
                        fileOutputInfo.TempFolder = FileName.Replace(".", "_")
                        Dim mfFileName As String = FileName.Substring(FileName.LastIndexOf("\") + 1).Replace(".sqz", ".mf")
                        tmpFileName = FileName.Replace(".", "_") + "\" + mfFileName
                        System.IO.File.Copy(FileName, tmpFileName, True)
                    End If

                    con = New SQLiteConnection

                    con.ConnectionString = "Data Source =" + tmpFileName + ";Version=3;New=False;Compress=True;"
                    con.Open()
                    cmd = New SQLiteCommand()
                    cmd.Connection = con
                    cmd.CommandText = "DELETE FROM Message WHERE MessageType >= 10001 AND MessageType <= 19999"
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()

                    'Compact database
                    cmd = New SQLiteCommand()
                    cmd.CommandText = "vacuum;"
                    cmd.Connection = con
                    cmd.ExecuteNonQuery()

                    cmd.Dispose()
                    con.Close()
                    con.Dispose()
                    fileOutputInfo.FileName = tmpFileName
                Else
                    cmd.Dispose()
                    con.Close()
                    con.Dispose()

                    fileOutputInfo.FileName = FileName
                    fileOutputInfo.FileType = "MF-Original"
                    Return fileOutputInfo

                End If

            End If

            Return fileOutputInfo
        Catch ex As Exception
            'Message = "attempt to write a readonly database  attempt to write a readonly database"
            If ex.Message.Contains("attempt to write a readonly database") Or ex.Message.Contains("out of memory") Then
                fileOutputInfo.FileName = FileName
                fileOutputInfo.FileType = "MF-Original"
                Return fileOutputInfo
            Else
                fileOutputInfo.isCorrupted = True
                fileOutputInfo.FileName = Nothing
                Return fileOutputInfo
                'Return Nothing
            End If
        End Try
    End Function

    Public Function processSQZ(ByVal FileName As String) As FileTrimmingInfo
        Dim sqzFileInfo As New FileTrimmingInfo()

        Dim FileNameOnly As String = FileName.Substring(FileName.LastIndexOf("\") + 1, FileName.Length - FileName.LastIndexOf("\") - 1)

        If FileName.StartsWith("\\") Then
            If Not Directory.Exists(Common.NetworkTempFolder) Then
                Directory.CreateDirectory(Common.NetworkTempFolder)
            End If

            File.Copy(FileName, Common.NetworkTempFolder + "\" + FileNameOnly, True)
            FileName = Common.NetworkTempFolder + "\" + FileNameOnly
            sqzFileInfo.isNetworkFile = True
        End If


        Dim zip As New Chilkat.Zip
        Dim unlocked As Boolean
        unlocked = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (Not unlocked) Then            
            Return Nothing
        End If
        Dim success As Boolean
        success = zip.OpenZip(FileName)

        'Dim tmpFolderName As String = FileName.Replace(".", "_")
        Dim DriveName As String = FileName.Substring(0, 1)
        If DriveName.StartsWith("\") Then
            DriveName = "C"
        End If
        Dim tmpFolderName As String = DriveName + ":\TempGTPUnzip\" + FileNameOnly.Replace(".", "_")

        Common.currentTempGTPUnzip = DriveName + ":\TempGTPUnzip\"
        If Not Directory.Exists(DriveName + ":\TempGTPUnzip\") Then
            Directory.CreateDirectory(DriveName + ":\TempGTPUnzip\")
        End If

        Dim isUnzipOK As Boolean = False
        Dim FilePathLength As String = "C:\Users\Tuan\Desktop\BM101618_ERIE__REDRIVE ID28967\BM101618_VOLTE_REDRIVE ID_28967\2018-10-16-08-34-29\CSM0\Verizon VoLTE Tx\".Length
        Try
            SevenZipExtractor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "7z.dll")
            Dim extractor As New SevenZipExtractor(FileName)
            If (extractor.ArchiveFileData.Count > 0) Then

                Directory.CreateDirectory(tmpFolderName)
                sqzFileInfo.TempFolder = tmpFolderName
                extractor.ExtractArchive(tmpFolderName)
                isUnzipOK = True
            End If
        Catch ex As Exception
            If ex.Message <> "The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters." Then
                Return Nothing
            End If
        End Try
        

        If Not isUnzipOK Then
            If success Then
                Dim unzipCount As Integer
                Directory.CreateDirectory(tmpFolderName)
                sqzFileInfo.TempFolder = tmpFolderName
                unzipCount = zip.Unzip(FileName.Replace(".", "_"))

                isUnzipOK = False
                Dim allFiles As String() = Directory.GetFiles(tmpFolderName)
                For Each fileStr In allFiles
                    If fileStr.ToLower().EndsWith(".mf") And fileStr.Contains(FileNameOnly.Split(".")(0)) Then
                        isUnzipOK = True
                    End If
                Next
                zip.CloseZip()
                zip.Dispose()

            Else
                'Try 7zip extractor            

            End If
        End If

        
        If isUnzipOK Then
            sqzFileInfo.TempFolder = tmpFolderName
            Dim mfFileName As String = FileName.Substring(FileName.LastIndexOf("\") + 1).Split(".")(0) + ".mf"
            Dim mfFileInfo As FileTrimmingInfo = processMFFile(tmpFolderName + "\" + mfFileName, False)
            If Not IsNothing(mfFileInfo) Then
                If Not IsNothing(mfFileInfo.FileName) Then
                    sqzFileInfo.isScanner = mfFileInfo.isScanner
                    sqzFileInfo.isAsideOrphan = mfFileInfo.isAsideOrphan
                    sqzFileInfo.BSideFileType = mfFileInfo.BSideFileType
                    If mfFileInfo.FileType <> "MF-Original" And mfFileInfo.FileType <> Nothing Then
                        Dim outputFileName As String = mfFileInfo.FileName.ToLower.Replace(".mf", ".sqz")
                        ZipFolder(outputFileName, tmpFolderName)
                        sqzFileInfo.FileName = outputFileName
                        sqzFileInfo.FileType = "SQZ-Modified - MF " + mfFileInfo.FileType
                    Else
                        sqzFileInfo.FileName = FileName
                        sqzFileInfo.FileType = "SQZ-Original"
                    End If
                Else
                    Return Nothing
                End If
                
            End If
        End If

        Return sqzFileInfo

    End Function

    Private Sub ZipFolder(ByVal outputFileName As String, ByVal folderNameInput As String)
        Dim zip As New Chilkat.Zip()

        Dim success As Boolean
        success = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (success <> True) Then
            Console.WriteLine(zip.LastErrorText)
            Exit Sub
        End If

        success = zip.NewZip(outputFileName)
        If (success <> True) Then
            Console.WriteLine(zip.LastErrorText)
            Exit Sub
        End If

        Dim recurse As Boolean = True
        success = zip.AppendFiles(folderNameInput + "/*", recurse)
        If (success <> True) Then
            Console.WriteLine(zip.LastErrorText)
            Exit Sub
        End If

        success = zip.WriteZipAndClose()
        If (success <> True) Then
            Console.WriteLine(zip.LastErrorText)
            Exit Sub
        End If
        zip.CloseZip()
        zip.Dispose()

    End Sub


End Class
