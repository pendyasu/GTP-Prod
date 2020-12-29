
Imports System.Net
Imports System.IO
Imports System.Data.OleDb
Imports System.Web
Imports System.Net.Mail
Imports GWSFTP.GTPService
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Security
Imports System.Security.Cryptography
Imports Chilkat
Imports SevenZip
Imports System.Globalization

'Chilkat Mail: cqcmgfMAILQ_uH6UW4u6pXnH
'Chilkat Zip: cqcmgfZIP_CVvkamt08Rwl
'Chilkat Crypt: cqcmgfCrypt_6nX6s3kKkURG
'Chilkat MHT: cqcmgfMHT_eYHWTAy3ALpU
'Chilkat S/MIME: cqcmgfSMIME_5YquvnPmpJ20
'Chilkat Charset: cqcmgfCharset_Y0h28NZf5WpG
'Chilkat IMAP: cqcmgfIMAPMAILQ_O3rtNGeUFX3G
'Chilkat Bounce: cqcmgfBOUNCE_007NZe3aDE8d
'Chilkat PFX: cqcmgfPFX_caNBrCaP9Soa
'Chilkat FTP2: cqcmgfFTP_bmMUBvCckRnr
'Chilkat HTTP: cqcmgfHttp_vd0XGuhuAUMl
'Chilkat HTML-to-XML: cqcmgfHtmlToXml_zecQBF0rALGZ
'Chilkat XMP: cqcmgfXMP_pg4oGS8W0XnX
'Chilkat RSA: cqcmgfRSA_eVh0BzEJjRnG
'Chilkat DSA: cqcmgfDSA_XPn0mcnxjKnw
'Chilkat SSH/SFTP: cqcmgfSSH_Qk2GPCJcjUnW
'Chilkat Diffie-Hellman: cqcmgfDiffie_FbmF6oLh6V27
'Chilkat Compression: cqcmgfCompress_Snze9p0xnRHj
'Chilkat Socket: cqcmgfSocket_C2OpDJvXnB59
'Chilkat TAR: cqcmgfTarArch_WqLANAPR2Zlg

Module Common

    Public VersionNumber As String = "20.2.1"

    Public currentTempGTPUnzip As String
    Public _isUniqueTimeStampSame As Boolean

    Public _UniqueTimeStamp As Hashtable
    Public _UniqueTimeStamp1 As Hashtable
    Public _missingList As HashSet(Of String)
    Public _missingListNoExt As HashSet(Of String)
    Public _FileTimestampThresh As Integer
    Public _FileGroupInfoList As List(Of _FileGroupInfo)
    Public _FileGroupInfoList_Original As List(Of _FileGroupInfo)
    Private _DeviceIDHashTable As Hashtable
    Public _FileGroupInfoList_Unique As List(Of _FileGroupInfo)
    Public _isSFTP As Boolean
    Public _FileGroup As _FileGroupConfig()
    Public _isTransactionStart As Boolean
    Public _folderListToDelete As New List(Of String)
    Public _Project As String
    Public NetworkTempFolder As String = AppDomain.CurrentDomain.BaseDirectory + "\network"
    Public enableShadowPassword As Boolean = False
    Public AESPassword As String = "SH%$gfs123!kslsgwE43H"
    Public isCheckNewVersion As Boolean = True
    Public isShowCompleteMessage As Boolean = False
    Public currentIDLocalTransaction As String = ""
    Public currentXMLFolder = ""
    Public isWorker1Stop As Boolean
    Public isWorker2Stop As Boolean
    Public LogFileName As String
    Public XMLFileName As String
    Public forceXMLMode As Boolean = False
    Public isXMLMode As Boolean
    Public MarketXMLFile As String = AppDomain.CurrentDomain.BaseDirectory + "market.xml"
    Public rememberCheckBox As String = AppDomain.CurrentDomain.BaseDirectory + "checkbox.txt"
    Public MarketXmlElement As XmlElement
    Public hasWCFAccess As Boolean = False
    Public currentXMLFile As String

    Public FileUploadStartTime As DateTime
    Public FileUploadEndTime As DateTime
    Public FileUploadStartTime1 As DateTime
    Public FileUploadEndTime1 As DateTime
    Public RemoteFTPFolder As String
    Public isEnableSSL As Boolean = True
    Public isResumeUpload As Boolean
    Public CountUploaded As Integer
    Public FTPServerName As String
    Public FTPUserName As String
    Public FTPPass As String
    Public FTPSessionLog As String
    Public id_server_public As String
    Public id_client_public As String

    Friend REMIS_username As String
    Friend REMIS_pass As String
    Friend isREMIS_Authenticated As Boolean
    Friend TeamName As String
    Friend log_file As String
    Friend website_root As String = "http://upload.mygws.com/GWSWebApp/"
    Friend ShadowPassFile As String = AppDomain.CurrentDomain.BaseDirectory + "app.backup.usage.txt"
    Private shadowFileName As String = "app.backup.usage.txt"

    Private FileGroupUniqueTimeStampList As List(Of FileGroupUniqueTimeStamp)
    Friend Structure FileGroupUniqueTimeStamp
        Dim FileGroup As _FileGroupConfig
        Dim UniqueTimeStamp As HashSet(Of String)
    End Structure

    Friend Structure HostComputer
        Dim HostName As String
        Dim MAC_address As String
        Dim Ip_address As String
    End Structure

    Friend Structure UploadResult
        Dim uploadStatus As Integer
        Dim fileTrimmingResult As FileTrimming.FileTrimmingInfo
    End Structure

    Friend Structure _FileGroupInfo
        Public FileName As String
        Public DeviceId As String
        Public FileGroup As String
        Public TimeStamps As String
        Public TimeStampFormat As String
        Public TimeStampDateFormat As DateTime
        Public FileGroupDeviceIdList As String
        Public NumToTriggerMissingAlert As Integer
    End Structure

    Friend Structure _FileTimeStamp
        Public timeStampStr As String
        Public timeStamps As Long
        Public isRemove As Boolean
        Public countTotal As Integer
        Public isRemoveByTotal As Boolean
        Public deviceHashSet As HashSet(Of String)
    End Structure

    Friend Sub ResetVariable()
        isShowCompleteMessage = False
        isWorker1Stop = False
        isWorker2Stop = False
        isResumeUpload = False
        currentIDLocalTransaction = ""
    End Sub

    Public Function GetCurrentEasternTime() As DateTime
        Dim now As DateTime = DateTime.Now
        Return System.TimeZoneInfo.ConvertTime(now, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))
    End Function

    Public Function isFolderWritable(ByVal FolderName As String) As Boolean
        Try
            Using fs As FileStream = File.Create(Path.Combine(FolderName, Path.GetRandomFileName), 1, FileOptions.DeleteOnClose)
                Return True
            End Using
        Catch ex As Exception
            Return False
        End Try

        Return False
    End Function

    Public Function GetFileNameOnly(ByVal fileFullName As String) As String
        Return fileFullName.Substring(fileFullName.LastIndexOf("\") + 1, fileFullName.Length - fileFullName.LastIndexOf("\") - 1)
    End Function

    Public Function GetFileExtension(ByVal fileFullName As String) As String
        Return fileFullName.Substring(fileFullName.LastIndexOf(".") + 1, fileFullName.Length - fileFullName.LastIndexOf(".") - 1)
    End Function


    Public Sub GetFileGroupConfig()
        Dim proxy As New GTPServiceClient()
        _FileGroup = proxy.getFileGroupContract()
        Dim config As New _FileGroupConfig()
        For Each config In _FileGroup
            If config.file_group_name = "FileTimestampThresh" Then
                _FileTimestampThresh = Convert.ToInt32(config.device_id)
            End If
        Next
        proxy.Close()
    End Sub

    Public Function GetFileGroupInfo(ByVal FileList As List(Of String)) As List(Of _FileGroupInfo)
        Dim tmpFile As String
        Dim output As New List(Of _FileGroupInfo)()
        Dim output_uniq As New List(Of _FileGroupInfo)()
        _UniqueTimeStamp = New Hashtable()
        Dim _TimeStampList As New Hashtable()
        Dim _TimeStampList1 As New Hashtable()
        _DeviceIDHashTable = New Hashtable()

        Dim fileListStr As String = ""

        FileGroupUniqueTimeStampList = New List(Of FileGroupUniqueTimeStamp)()

        For Each tmpFile In FileList

            If tmpFile.ToLower().EndsWith(".mf") Or tmpFile.ToLower.EndsWith(".sqz") Then
                Dim tmpInfo As New _FileGroupInfo()

                Dim fileNameOnly = tmpFile.Substring(tmpFile.LastIndexOf("\") + 1)

                tmpInfo.FileName = fileNameOnly
                Dim tmpArr As String() = fileNameOnly.Split("-")
                tmpInfo.DeviceId = tmpArr(tmpArr.Length - 2) + "-" + tmpArr(tmpArr.Length - 1).Split(".")(0).ToUpper()
                tmpInfo.FileGroup = ""
                Dim config As New _FileGroupConfig()
                For Each config In _FileGroup
                    If config.device_id.IndexOf(tmpInfo.DeviceId) >= 0 Then
                        tmpInfo.FileGroup = config.file_group_name
                        tmpInfo.FileGroupDeviceIdList = config.device_id
                        tmpInfo.NumToTriggerMissingAlert = config.NumToTriggerMissingAlert
                    End If
                Next


                'If Not tmpInfo.FileGroup = "FileGroup3" Then
                '    Continue For
                'End If
                fileListStr = fileListStr + tmpInfo.FileName + Environment.NewLine
                tmpInfo.TimeStamps = fileNameOnly.Substring(0, "2018-10-01-09-08-31".Length)
                'If tmpInfo.TimeStamps = "2019-04-08-08-28-42" Then
                '    Dim tt11 As String = ""
                'End If
                'Get the hash table of device id
                If Not _DeviceIDHashTable.ContainsKey(tmpInfo.FileGroup) Then
                    Dim tmpHashSet As New HashSet(Of String)
                    tmpHashSet.Add(tmpInfo.DeviceId)
                    _DeviceIDHashTable.Item(tmpInfo.FileGroup) = tmpHashSet
                Else
                    Dim tmpHashSet As New HashSet(Of String)
                    tmpHashSet = _DeviceIDHashTable.Item(tmpInfo.FileGroup)
                    If Not tmpHashSet.Contains(tmpInfo.DeviceId) Then
                        tmpHashSet.Add(tmpInfo.DeviceId)
                        _DeviceIDHashTable.Item(tmpInfo.FileGroup) = tmpHashSet
                    End If
                End If


                If Not _UniqueTimeStamp.ContainsKey(tmpInfo.FileGroup) Then
                    Dim tmpHashSet As New HashSet(Of String)
                    Dim tmpTimeStamp As New List(Of _FileTimeStamp)

                    Dim tp As New _FileTimeStamp()
                    tp.countTotal = 1
                    tp.isRemove = False
                    tp.timeStampStr = tmpInfo.TimeStamps
                    Dim date1 = DateTime.ParseExact(tp.timeStampStr, "yyyy-MM-dd-HH-mm-ss", Nothing)
                    tp.timeStamps = date1.Ticks
                    tp.deviceHashSet = New HashSet(Of String)
                    tp.deviceHashSet.Add(tmpInfo.DeviceId)

                    tmpTimeStamp.Add(tp)

                    tmpHashSet.Add(tmpInfo.TimeStamps)
                    _UniqueTimeStamp.Add(tmpInfo.FileGroup, tmpHashSet)
                    _TimeStampList.Add(tmpInfo.FileGroup, tmpTimeStamp)
                Else
                    Dim tmpHashSet As New HashSet(Of String)
                    tmpHashSet = _UniqueTimeStamp.Item(tmpInfo.FileGroup)

                    If tmpInfo.FileGroup = "FileGroup2" Then
                        Dim tt4 As String = ""
                    End If
                    If Not tmpHashSet.Contains(tmpInfo.TimeStamps) Then

                        Dim tp As New _FileTimeStamp()
                        tp.deviceHashSet = New HashSet(Of String)
                        tp.deviceHashSet.Add(tmpInfo.DeviceId)
                        tp.countTotal = tp.deviceHashSet.Count
                        tp.isRemove = False
                        tp.timeStampStr = tmpInfo.TimeStamps


                        If tp.countTotal < tmpInfo.NumToTriggerMissingAlert Then
                            tp.isRemoveByTotal = True
                        Else
                            tp.isRemoveByTotal = False
                        End If
                        Dim date1 = DateTime.ParseExact(tp.timeStampStr, "yyyy-MM-dd-HH-mm-ss", Nothing)

                        tp.timeStamps = date1.Ticks

                        Dim isWIthIN As Boolean = False
                        Dim foundTimeStamp As String = ""
                        For Each timeStampTmp In tmpHashSet
                            Dim date2 = DateTime.ParseExact(timeStampTmp, "yyyy-MM-dd-HH-mm-ss", Nothing)
                            If Math.Abs((date1 - date2).TotalSeconds) < _FileTimestampThresh And tp.timeStampStr <> timeStampTmp Then
                                foundTimeStamp = timeStampTmp
                                isWIthIN = True
                            End If
                        Next

                        If Not isWIthIN Then
                            tmpHashSet.Add(tmpInfo.TimeStamps)
                            _UniqueTimeStamp.Item(tmpInfo.FileGroup) = tmpHashSet
                            Dim tmpTimeStamp As New List(Of _FileTimeStamp)

                            tmpTimeStamp = _TimeStampList.Item(tmpInfo.FileGroup)


                            tmpTimeStamp.Add(tp)
                            _TimeStampList.Item(tmpInfo.FileGroup) = tmpTimeStamp
                        Else
                            'Increase countTotal for foundTimeStamp

                            Dim tmpTimeStamp As New List(Of _FileTimeStamp)

                            tmpTimeStamp = _TimeStampList.Item(tmpInfo.FileGroup)
                            Dim tmp2 As New _FileTimeStamp()
                            For Each tmp1 In tmpTimeStamp
                                If tmp1.timeStampStr = foundTimeStamp Then
                                    tmp2 = tmp1
                                    Exit For
                                End If
                            Next

                            tmpTimeStamp.Remove(tmp2)

                            If Not tmp2.deviceHashSet.Contains(tmpInfo.DeviceId) Then
                                tmp2.deviceHashSet.Add(tmpInfo.DeviceId)
                            End If
                            tmp2.countTotal = tmp2.deviceHashSet.Count

                            If tmp2.countTotal < tmpInfo.NumToTriggerMissingAlert Then
                                tmp2.isRemoveByTotal = True
                            Else
                                tmp2.isRemoveByTotal = False
                            End If

                            tmpTimeStamp.Add(tmp2)


                            _TimeStampList.Item(tmpInfo.FileGroup) = tmpTimeStamp


                        End If


                    Else

                        Dim tmpTimeStamp As New List(Of _FileTimeStamp)

                        tmpTimeStamp = _TimeStampList.Item(tmpInfo.FileGroup)
                        Dim tmp2 As New _FileTimeStamp()
                        For Each tmp1 In tmpTimeStamp
                            If tmp1.timeStampStr = tmpInfo.TimeStamps Then
                                tmp2 = tmp1
                                Exit For
                            End If
                        Next

                        tmpTimeStamp.Remove(tmp2)


                        If Not tmp2.deviceHashSet.Contains(tmpInfo.DeviceId) Then
                            tmp2.deviceHashSet.Add(tmpInfo.DeviceId)
                        End If
                        tmp2.countTotal = tmp2.deviceHashSet.Count

                        If tmp2.countTotal < tmpInfo.NumToTriggerMissingAlert Then
                            tmp2.isRemoveByTotal = True
                        Else
                            tmp2.isRemoveByTotal = False
                        End If

                        tmpTimeStamp.Add(tmp2)


                        _TimeStampList.Item(tmpInfo.FileGroup) = tmpTimeStamp

                    End If
                End If
                tmpInfo.TimeStampDateFormat = DateTime.ParseExact(tmpInfo.TimeStamps, "yyyy-MM-dd-HH-mm-ss", Nothing)

                output.Add(tmpInfo)
            End If

        Next
        _FileGroupInfoList = output
        '2018-10-01-09-08-32-0001-0016-0000-0111-A
        '2019-04-08-18-32-54
        '2019-04-08-19-31-28
        Dim key As String
        '_TimeStampList1 = _TimeStampList
        Dim tt As System.Collections.ICollection = _TimeStampList.Keys

        'Group 2 0121-A, 0221-A, 0321-A, 0421-A, 0121-B, 0221-B, 0321-B, 0421-B
        For Each key In tt
            Dim tmpList As New List(Of _FileTimeStamp)
            tmpList = _TimeStampList.Item(key)
            tmpList.Sort(Function(x1, y1) x1.timeStamps.CompareTo(y1.timeStamps))


            Dim tmpList1 As New List(Of _FileTimeStamp)
            Dim date2 As DateTime = DateTime.ParseExact(tmpList(0).timeStampStr, "yyyy-MM-dd-HH-mm-ss", Nothing)
            date2 = date2.AddSeconds(_FileTimestampThresh)
            Dim tmpInt As Long = date2.Ticks
            Dim i As Integer
            Dim _UniqueTimeStamp2 As New Hashtable()

            If Not tmpList(0).isRemoveByTotal Then
                tmpList1.Add(tmpList(0))
            End If


            For i = 1 To tmpList.Count - 1

                If tmpList(i).timeStamps > tmpInt And Not tmpList(i).isRemoveByTotal Then
                    tmpList1.Add(tmpList(i))
                End If
            Next
            _TimeStampList1.Item(key) = tmpList1
        Next

        _TimeStampList = _TimeStampList1



        _UniqueTimeStamp1 = New Hashtable()
        _isUniqueTimeStampSame = True
        For Each key In _UniqueTimeStamp.Keys
            Dim tmpHashSet As New HashSet(Of String)
            'tmpHashSet = _UniqueTimeStamp.Item(key)

            Dim tmpTimeStamp As New List(Of _FileTimeStamp)
            tmpTimeStamp = _TimeStampList.Item(key)
            For Each v In tmpTimeStamp
                tmpHashSet.Add(v.timeStampStr)
                _isUniqueTimeStampSame = False
            Next

            _UniqueTimeStamp1.Add(key, tmpHashSet)
        Next
        'If isEqual() Then
        '    _isUniqueTimeStampSame = True
        'Else
        '    _isUniqueTimeStampSame = False
        'End If

        'If Not _isUniqueTimeStampSame Then
        '    _UniqueTimeStamp1 = _UniqueTimeStamp
        'End If

        _UniqueTimeStamp = _UniqueTimeStamp1
        Return output
    End Function

    Private Function isEqual() As Boolean
        For Each key In _UniqueTimeStamp.Keys
            Dim tmpHashSet As New HashSet(Of String)
            Dim tmpHashSet1 As New HashSet(Of String)
            tmpHashSet = _UniqueTimeStamp.Item(key)
            tmpHashSet1 = _UniqueTimeStamp1.Item(key)
            If tmpHashSet.Count <> tmpHashSet1.Count Then
                Return False
            End If
        Next
        Return True
    End Function


    Private Function GetTotalDeviceInFileGroup(ByVal FileGroupDeviceIdList As String) As Integer
        Return FileGroupDeviceIdList.Split(",").Count
    End Function
    Public Function isIgnoreCondition25(ByVal fileInfo As _FileGroupInfo) As Boolean
        'Dim isMatch1 As Boolean = True
        Dim isMatch2 As Boolean = True

        If Not IsNothing(fileInfo.FileGroupDeviceIdList) Then
            If GetTotalDeviceInFileGroup(fileInfo.FileGroupDeviceIdList) > 2 Then

                Dim tmpInfo As _FileGroupInfo
                Dim count As Integer = 0
                For Each tmpInfo In _FileGroupInfoList
                    If tmpInfo.FileGroup = fileInfo.FileGroup Then
                        If tmpInfo.TimeStamps = fileInfo.TimeStamps Then
                            count = count + 1
                        End If
                    End If
                Next
                If count >= 2 Then
                    Return False
                End If

                For Each tmpInfo In _FileGroupInfoList

                    If tmpInfo.FileName <> fileInfo.FileName Then
                        Dim DiffSecond As Integer = Math.Round(DateDiff(DateInterval.Second, tmpInfo.TimeStampDateFormat, fileInfo.TimeStampDateFormat, 0))
                        If Math.Abs(DiffSecond) < _FileTimestampThresh Then
                            Return False
                        End If
                    End If

                Next

            Else
                Return False
            End If
        End If

        'Remove TimeStamp from list
        Dim tmpHash As New HashSet(Of String)
        tmpHash = _UniqueTimeStamp1.Item(fileInfo.FileGroup)
        tmpHash.Remove(fileInfo.TimeStamps)
        _UniqueTimeStamp1.Item(fileInfo.FileGroup) = tmpHash

        Return True


    End Function

    Private Function CheckCondition4(ByVal fileInfoInput As _FileGroupInfo) As Boolean
        For Each fileGroupKey In _UniqueTimeStamp.Keys
            If fileInfoInput.FileGroup = fileGroupKey And fileGroupKey <> "" Then
                Dim timeStampHashSet As New HashSet(Of String)
                timeStampHashSet = _UniqueTimeStamp.Item(fileGroupKey)
                Dim count As Integer = 0
                For Each tmpTimeStamp In timeStampHashSet
                    Dim isFound As Boolean = False
                    For Each tmpInfo In _FileGroupInfoList

                        If fileInfoInput.FileGroup = tmpInfo.FileGroup Then
                            If fileInfoInput.DeviceId = tmpInfo.DeviceId And tmpInfo.TimeStamps = tmpTimeStamp Then
                                Dim tt1 = DateTime.ParseExact(tmpTimeStamp, "yyyy-MM-dd-HH-mm-ss", Nothing)

                                Dim DiffSecond As Integer = Math.Round(DateDiff(DateInterval.Second, tmpInfo.TimeStampDateFormat, tt1, 0))

                                '#10/25/2018 11:11:49 AM#
                                '#10/25/2018 12:39:38 PM#
                                If Math.Abs(DiffSecond) < _FileTimestampThresh Then
                                    'If one of the file out of _FileTimestampThresh then condition 4 failed. Return False - Skip file
                                    isFound = True

                                End If

                            End If
                        End If

                    Next

                    If isFound Then
                        count = count + 1
                    End If


                Next


            End If


        Next

        Return False
    End Function


    Private Sub CheckCondition5(ByVal fileInfoInput As _FileGroupInfo)
        Dim output As String = ""
        Dim tmpList As New List(Of _FileGroupInfo)
        Dim tmpUniqueTimeStamp As New Hashtable()

        For Each fileGroupKey In _UniqueTimeStamp1.Keys
            'If fileGroupKey = "FileGroup3" Then
            '    Dim ss As String = ""
            'End If
            If fileInfoInput.FileGroup = fileGroupKey And fileGroupKey <> "" Then
                'Dim deviceList = fileInfoInput.FileGroupDeviceIdList.Split(",")
                Dim timeStampHashSet As New HashSet(Of String)
                timeStampHashSet = _UniqueTimeStamp1.Item(fileGroupKey)
                Dim count As Integer = 0

                For Each tmpTimeStamp In timeStampHashSet
                    '2018-10-01-09-08-32
                    'If tmpTimeStamp = "2018-10-01-09-08-32" And fileInfoInput.DeviceId = "0111-A" Then
                    '    Dim tt As String = ""
                    'End If
                    Dim isFound As Boolean = False
                    For Each tmpInfo In _FileGroupInfoList_Original
                        If fileInfoInput.FileGroup = tmpInfo.FileGroup Then
                            Dim date1 As DateTime = DateTime.ParseExact(tmpTimeStamp, "yyyy-MM-dd-HH-mm-ss", Nothing)
                            Dim date2 As DateTime = DateTime.ParseExact(tmpInfo.TimeStamps, "yyyy-MM-dd-HH-mm-ss", Nothing)
                            Dim tmpDateDiff As Boolean = Math.Abs((date1 - date2).TotalSeconds) < _FileTimestampThresh

                            If fileInfoInput.DeviceId = tmpInfo.DeviceId And tmpDateDiff Then
                                isFound = True
                                Exit For
                            End If
                        End If
                    Next
                    If Not isFound Then
                        output = tmpTimeStamp + fileInfoInput.FileName.Substring("2018-10-25-12-39-45".Length)

                        Dim isAdd As Boolean = True
                        Dim fileNoExt As String = output.Substring(0, output.LastIndexOf("."))

                        If Not _missingList.Contains(output) And Not _missingListNoExt.Contains(fileNoExt) Then
                            If _missingList.Count > 0 Then
                                For Each item In _missingList
                                    Dim date1 As DateTime = DateTime.ParseExact(tmpTimeStamp, "yyyy-MM-dd-HH-mm-ss", Nothing)
                                    Dim date2 As DateTime = DateTime.ParseExact(item.Substring(0, "2018-10-01-09-08-32".Length), "yyyy-MM-dd-HH-mm-ss", Nothing)
                                    Dim tmpDateDiff As Boolean = Math.Abs((date1 - date2).TotalSeconds) < _FileTimestampThresh
                                    Dim tt1 As String = output.Substring("2018-10-25-12-39-45".Length, output.LastIndexOf(".") - "2018-10-25-12-39-45".Length)
                                    Dim tt2 As String = item.Substring("2018-10-25-12-39-45".Length, item.LastIndexOf(".") - "2018-10-25-12-39-45".Length)

                                    If (tmpDateDiff And tt1 = tt2) Then
                                        isAdd = False
                                        Exit For
                                    End If

                                Next

                                If isAdd Then
                                    _missingList.Add(output)
                                    _missingListNoExt.Add(fileNoExt)
                                End If
                            Else
                                _missingList.Add(output)
                                _missingListNoExt.Add(fileNoExt)
                            End If

                        End If
                    End If
                Next
            End If
        Next

    End Sub

    '2018-10-25-11-11-49-0000-0000-0000-0111-A.sqz

    '2018-10-25-11-11-52-0001-0000-0000-0421-B.sqz
    '2018-10-25-12-39-45-0000-0000-0000-0202-S.sqz
    '2018-10-25-12-39-46-0000-0000-0000-3030-S.sqz

    '2018-10-25-11-11-55-0000-0000-0000-0202-S.sqz

    Public Sub CheckFileGroupInfo(ByVal fileGroup As List(Of _FileGroupInfo))
        _missingList = New HashSet(Of String)()
        _missingListNoExt = New HashSet(Of String)
        Dim tmpInfo As _FileGroupInfo



        'Clean up input file list based on Unique Time Stamps       
        _FileGroupInfoList_Unique = New List(Of _FileGroupInfo)()

        _FileGroupInfoList_Original = _FileGroupInfoList

        Dim fileHashSet As New HashSet(Of String)

        If Not _isUniqueTimeStampSame Then
            For Each group In _UniqueTimeStamp1.Keys

                For Each tmpInfo In _FileGroupInfoList

                    Dim isAdd As Boolean = True

                    If tmpInfo.FileGroup = group Then
                        Dim date2 As DateTime = DateTime.ParseExact(tmpInfo.TimeStamps, "yyyy-MM-dd-HH-mm-ss", Nothing)
                        Dim tmpInt As Long = date2.Ticks

                        Dim tmpTime As New HashSet(Of String)
                        tmpTime = _UniqueTimeStamp1.Item(group)
                        For Each timeS In tmpTime
                            Dim date1 As DateTime = DateTime.ParseExact(timeS, "yyyy-MM-dd-HH-mm-ss", Nothing)
                            If Math.Abs((date1 - date2).TotalSeconds) < _FileTimestampThresh And tmpInfo.TimeStamps <> timeS Then

                                isAdd = False

                            End If
                        Next

                    End If

                    If isAdd Then
                        If Not fileHashSet.Contains(tmpInfo.FileName) Then
                            fileHashSet.Add(tmpInfo.FileName)
                            _FileGroupInfoList_Unique.Add(tmpInfo)
                        End If

                    End If
                Next
            Next
        Else
            _FileGroupInfoList_Unique = _FileGroupInfoList

        End If


        Dim fileToKeep As New List(Of _FileGroupInfo)

        For Each tmpInfo In _FileGroupInfoList_Unique
            If Not isIgnoreCondition25(tmpInfo) Then
                fileToKeep.Add(tmpInfo)
            End If
        Next
        _FileGroupInfoList_Unique = fileToKeep

        If _FileGroupInfoList_Unique.Count = 0 Then
            Return
        End If

        For Each tmpInfo In _FileGroupInfoList_Original

            If IsNothing(tmpInfo.FileGroup) Or tmpInfo.FileGroup = "" Then
                Continue For
            End If

            If CheckCondition4(tmpInfo) Then
                Continue For
            End If

            CheckCondition5(tmpInfo)

        Next

    End Sub

    Friend Function GetShadowCredential() As _ShadowPassword
        Dim shadownPassword As New _ShadowPassword()
        Try
            Dim proxy As New GTPServiceClient()
            Dim passwords As _ShadowPassword() = proxy.getShadowPassword(1)
            If passwords.Length > 0 Then
                SaveShadowPassword(passwords(0).username, passwords(0).password)
                passwords(0).password = AES_Decrypt(passwords(0).password, Common.AESPassword)
                passwords(0).username = AES_Decrypt(passwords(0).username, Common.AESPassword)
                Return passwords(0)
            End If
            proxy.Close()
        Catch ex As Exception
            'Get File from FTP server
            Try
                DownloadFile(ShadowPassFile, shadowFileName, "Shadow")
            Catch

            End Try

            Try
                ' Open the file using a stream reader.
                Dim count As Integer = 0
                Dim password As New _ShadowPassword()

                Using sr As New StreamReader(ShadowPassFile)
                    Dim line As String
                    line = sr.ReadToEnd()
                    ' Read the stream to a string and write the string to the console.
                    If count = 0 Then
                        password.username = AES_Decrypt(line, Common.AESPassword)
                    End If
                    If count = 1 Then
                        password.password = AES_Decrypt(line, Common.AESPassword)
                    End If
                End Using
            Catch e As Exception
                Console.WriteLine("The file could not be read:")
                Console.WriteLine(e.Message)
            End Try
        End Try

        Return Nothing
    End Function

    Public Function UnzipFile(ByVal DirName As String, ByVal fileName As String) As String
        If Not Directory.Exists(DirName) Then
            Directory.CreateDirectory(DirName)
        End If
        Dim zip As New Chilkat.Zip
        Dim unlocked As Boolean
        unlocked = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (Not unlocked) Then
            Return Nothing
        End If
        Dim success As Boolean
        success = zip.OpenZip(fileName)
        Dim tmpFolderName As String = DirName + Path.GetFileName(fileName).Replace(".", "_")
        'Dim tmpFolderName As String = fileName.Replace(".", "_")
        Dim isUnzipOK As Boolean = False
        If success Then
            Dim unzipCount As Integer
            Directory.CreateDirectory(tmpFolderName)

            unzipCount = zip.Unzip(tmpFolderName)
            zip.CloseZip()
            zip.Dispose()
            isUnzipOK = True
        Else

            Try
                'Try 7zip extractor            
                SevenZipExtractor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "7z.dll")
                Dim extractor As New SevenZipExtractor(fileName)
                If (extractor.ArchiveFileData.Count > 0) Then

                    Directory.CreateDirectory(tmpFolderName)
                    extractor.ExtractArchive(tmpFolderName)

                    isUnzipOK = True
                End If
            Catch ex As Exception
                isUnzipOK = False
            End Try

        End If
        If isUnzipOK Then
            Return tmpFolderName
        Else
            Return ""
        End If
    End Function

    Public Function GetAllFileInFolder(ByVal folderName As String, ByVal fileExt As String) As List(Of String)
        Dim fileList As List(Of String)
        Dim Subfolders() As String = Common.getAllFolders(folderName)
        Dim diar1 As IO.FileInfo()
        Dim dra As IO.FileInfo
        fileList = New List(Of String)

        For Each f In Subfolders
            Dim di = New IO.DirectoryInfo(f)
            diar1 = di.GetFiles()
            For Each dra In diar1
                If dra.Name.ToLower.EndsWith(fileExt) Then
                    fileList.Add(dra.FullName)
                End If
            Next
        Next

        Return fileList
    End Function


    Public Sub ZipFolder(ByVal outputFileName As String, ByVal folderNameInput As String)
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

    Friend Function GetHTTPData(ByVal URL As String, Optional ByRef UserName As String = "", Optional ByRef Password As String = "") As String
        Dim Req As HttpWebRequest
        Dim Response As HttpWebResponse
        Dim tmp As String = ""
        Try
            'create a web request to the URL  
            Req = HttpWebRequest.Create(URL)
            'Set username and password if required  (Network credentials)
            If Len(UserName) > 0 Then
                Req.Credentials = New NetworkCredential(UserName, Password)
            End If
            'get a response from web site  
            Response = Req.GetResponse()
            Dim reader As New StreamReader(Response.GetResponseStream())
            tmp = reader.ReadToEnd()
            Response.Close()
        Catch ex As Exception
            tmp = "error"
        Finally

        End Try
        Return tmp
    End Function


    Public Function GetTotalPendingTransactions() As Integer
        Dim dbFile As String = AppDomain.CurrentDomain.BaseDirectory & "\Database\FTPDB.mdb"
        Dim cnnstr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbFile & ";Jet OLEDB:Database Password=jignesh0;"
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        Dim id_transaction As Integer = 0
        cmd = New OleDbCommand
        cmd.Connection = cnn

        If cnn.State <> ConnectionState.Open Then
            cnn.Open()
        End If

        cmd.CommandText = "SELECT * FROM tblTransactionInfo WHERE status_transaction<> 2 and isCompleted=0 ORDER BY date_started DESC "
        dr = cmd.ExecuteReader()
        Dim count As Integer = 0

        If dr.HasRows Then
            While dr.Read()
                count = count + 1

            End While
        End If

        Return count

    End Function

    Public Function getDateString(ByVal dateInput As DateTime) As String
        Dim output As DateTime = dateInput
        Dim mon As Integer = dateInput.Month

        Dim monStr As String

        If mon < 10 Then
            monStr = "0" + mon.ToString
        Else
            monStr = mon.ToString
        End If

        Dim dayStr As String

        Dim day As Integer = dateInput.Day
        If (day < 10) Then
            dayStr = "0" + day.ToString
        Else
            dayStr = day.ToString
        End If
        Dim hourStr As String
        If dateInput.Hour < 10 Then
            hourStr = "0" + dateInput.Hour.ToString
        Else
            hourStr = dateInput.Hour.ToString
        End If

        Dim minStr As String
        If dateInput.Minute < 10 Then
            minStr = "0" + dateInput.Minute.ToString
        Else
            minStr = dateInput.Minute.ToString
        End If

        Dim secondStr As String
        If dateInput.Second < 10 Then
            secondStr = "0" + dateInput.Second.ToString
        Else
            secondStr = dateInput.Second.ToString
        End If

        Return monStr + "/" + dayStr + "/" + dateInput.Year.ToString + " " + hourStr + ":" + minStr + ":" + secondStr

    End Function

    Friend Sub GetIPAddress(ByRef HostName As String, ByRef Ipaddress As String, ByRef MACaddress As String)
        Try
            HostName = System.Net.Dns.GetHostName()
        Catch ex As Exception

        End Try

        Try
            Dim ip_address() As System.Net.IPAddress

            ip_address = System.Net.Dns.GetHostAddresses(HostName)
            MACaddress = ip_address(0).ToString
            Dim i As Integer
            Ipaddress = ip_address(1).ToString
            For i = 0 To ip_address.Length - 1
                If ip_address(i).ToString.IndexOf(".") > 0 Then
                    Ipaddress = ip_address(i).ToString
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SaveShadowPassword(ByVal shadowUserName As String, ByVal shadowPassword As String)

        If Not File.Exists(ShadowPassFile) Then
            Dim objWriter As New StreamWriter(ShadowPassFile, False)
            objWriter.WriteLine(shadowUserName)
            objWriter.WriteLine(shadowPassword)
            objWriter.Close()
            objWriter.Dispose()
        End If
        UploadFile(ShadowPassFile, "Shadow")

    End Sub

    Public Function AppendTextFile(ByVal FileName As String, ByVal strLog As String, ByVal ex As Exception) As Boolean
        Dim objWriter As New StreamWriter(FileName, True)
        If IsNothing(ex) Then
            objWriter.WriteLine(DateTime.Now.ToString() + " --- " + "Log string:" + strLog)
            objWriter.Close()
            objWriter.Dispose()
            Return True
        End If
        Dim LineOfCode As String = "Line of Code: "

        Dim st As New StackTrace(True)
        st = New StackTrace(ex, True)

        Dim i As Integer
        For i = 0 To st.FrameCount - 1
            LineOfCode = LineOfCode + st.GetFrame(i).GetFileLineNumber().ToString

        Next


        If Not IsNothing(ex) Then
            objWriter.WriteLine(DateTime.Now.ToString() + Environment.NewLine + " --- " + LineOfCode + " Log string :" + strLog + Environment.NewLine + " Message:" + ex.Message + Environment.NewLine + " StackTrace: :" + ex.StackTrace + Environment.NewLine + " Target Site Declaring Type:" + ex.TargetSite.DeclaringType.FullName)
        Else
            objWriter.WriteLine(DateTime.Now.ToString() + " --- " + "Log string:" + strLog)
        End If

        objWriter.Close()
        objWriter.Dispose()
        Return True
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



    Public Function getAllFolders(ByVal directory As String) As String()
        'Create object

        Dim fi As New IO.DirectoryInfo(directory)
        'Array to store paths
        Dim path() As String = {}
        'Loop through subfolders
        For Each subfolder As IO.DirectoryInfo In fi.GetDirectories()
            'Add this folders name
            Array.Resize(path, path.Length + 1)
            path(path.Length - 1) = subfolder.FullName
            'Recall function with each subdirectory
            For Each s As String In getAllFolders(subfolder.FullName)
                Array.Resize(path, path.Length + 1)
                path(path.Length - 1) = s
            Next
        Next
        Return path
    End Function


    Public Sub SendEmailNotification(ByVal strSubject As String, ByVal error_message As String)
        'Sending an email to notify admin
        Try
            Dim MyComputerInfo As New HostComputer
            MyComputerInfo.HostName = ""
            MyComputerInfo.Ip_address = ""
            MyComputerInfo.MAC_address = ""
            Common.GetIPAddress(MyComputerInfo.HostName, MyComputerInfo.Ip_address, MyComputerInfo.MAC_address)

            Dim strTo As String = "tuand@gwsolutions.com"
            Dim strFrom As String = "gwsftp@gmail.com"
            Dim strContent As String = "Reported at DateTime: " + Date.Now + " Host name:" + MyComputerInfo.HostName + " IP address: " + MyComputerInfo.Ip_address + " Username: " + Common.REMIS_username

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
            message.Body = strContent + " " + error_message
            message.To.Add(strTo)

            smtpClient.Send(message)
        Catch ex1 As Exception

        End Try
    End Sub


    Public Sub AssignCredentials(ByRef proxy As GTPServiceClient)
        If Not IsNothing(proxy) Then
            If isEnableSSL Then
                proxy.ClientCredentials.UserName.UserName = Common.REMIS_username
                proxy.ClientCredentials.UserName.Password = Common.REMIS_pass
            End If
        End If

    End Sub


    Public Function DeleteFileonFTP(ByVal FiletoDelete As String, ByVal RemoteDir As String) As Boolean
        Try
            Dim success As Boolean
            Dim FileName As String = FiletoDelete.Substring(FiletoDelete.LastIndexOf("\") + 1, FiletoDelete.Length - FiletoDelete.LastIndexOf("\") - 1)
            Dim finishedCode As Integer = 0
            Dim ftp As New Chilkat.Ftp2()
            ftp.Hostname = "upload.mygws.com"
            ftp.Username = FTPUserName
            ftp.Password = FTPPass
            success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
            If (success <> True) Then
                Return False
            End If
            success = ftp.Connect()
            If (success <> True) Then
                Return False
            End If

            success = ftp.ChangeRemoteDir(RemoteDir + "/")
            If (success <> True) Then
                'MsgBox(ftp.LastErrorText)
                Return False
            End If

            Dim fileSize As Long
            fileSize = ftp.GetSizeByName(FileName)
            If (fileSize > 0) Then
                success = ftp.DeleteRemoteFile(FileName)
                If (success <> True) Then
                    'MsgBox(ftp.LastErrorText)
                    Return False
                End If
            End If

            ftp.Disconnect()
            Return success
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function DecryptString(ByVal encryptText As String) As String
        Dim crypt As New Chilkat.Crypt2()

        Dim success As Boolean
        success = crypt.UnlockComponent("cqcmgfCrypt_6nX6s3kKkURG")
        If (success <> True) Then
            Return ""
        End If


        Dim password As String
        password = "afgl;k342a&@*dsg23424#%&(2r6yu7"

        crypt.CryptAlgorithm = "aes"
        crypt.CipherMode = "cbc"
        crypt.KeyLength = 128

        '  Generate a binary secret key from a password string
        '  of any length.  For 128-bit encryption, GenEncodedSecretKey
        '  generates the MD5 hash of the password and returns it
        '  in the encoded form requested.  The 2nd param can be
        '  "hex", "base64", "url", "quoted-printable", etc.
        Dim hexKey As String
        hexKey = crypt.GenEncodedSecretKey(password, "hex")
        crypt.SetEncodedKey(hexKey, "hex")

        crypt.EncodingMode = "base64"


        '  Decrypt and show the original string:
        Dim decryptedText As String
        decryptedText = crypt.DecryptStringENC(encryptText)



        Return decryptedText
    End Function


    Public Function EncryptString(ByVal inputStr As String) As String

        Dim crypt As New Chilkat.Crypt2()

        Dim success As Boolean

        success = crypt.UnlockComponent("cqcmgfCrypt_6nX6s3kKkURG")
        If (success <> True) Then
            Return ""
        End If


        Dim password As String
        password = "afgl;k342a&@*dsg23424#%&(2r6yu7"

        crypt.CryptAlgorithm = "aes"
        crypt.CipherMode = "cbc"
        crypt.KeyLength = 128

        '  Generate a binary secret key from a password string
        '  of any length.  For 128-bit encryption, GenEncodedSecretKey
        '  generates the MD5 hash of the password and returns it
        '  in the encoded form requested.  The 2nd param can be
        '  "hex", "base64", "url", "quoted-printable", etc.
        Dim hexKey As String
        hexKey = crypt.GenEncodedSecretKey(password, "hex")
        crypt.SetEncodedKey(hexKey, "hex")

        crypt.EncodingMode = "base64"
        Dim text As String
        text = inputStr

        '  Encrypt a string and return the binary encrypted data
        '  in a base-64 encoded string.
        Dim encText As String
        encText = crypt.EncryptStringENC(text)

        Return encText

    End Function


    Public Function CheckFTPDirectory(ByVal remote_dir As String) As Boolean
        Dim ftp As New Chilkat.Ftp2()

        Dim success As Boolean

        ' Any string unlocks the component for the 1st 30-days.
        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            Return False
        End If
        ftp.Hostname = "upload.mygws.com"
        ftp.Username = Common.FTPUserName
        ftp.Password = Common.FTPPass


        If remote_dir = Common.currentXMLFolder Then
            ftp.Username = "TeamREMIS"
            ftp.Password = "$MSR@mat"
        End If


        ftp.Passive = True

        ' Connect and login to the FTP server.

        success = ftp.Connect()
        If (success <> True) Then

            Return False
        End If

        Dim dirExists As Boolean
        dirExists = ftp.ChangeRemoteDir(remote_dir)
        If (dirExists = True) Then
            success = ftp.ChangeRemoteDir("..")
            If (success <> True) Then
                Return False
            End If
        End If

        ftp.Disconnect()
        Return True

    End Function


    Public Sub UploadFile(ByVal FiletoUpload As String, ByVal remote_dir As String)
        Dim success As Boolean
        Dim FileName As String
        FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)

        Dim ftp As New Chilkat.Ftp2()

        ftp.Hostname = "upload.mygws.com"
        ftp.Username = "TeamREMIS"
        ftp.Password = "$MSR@mat"
        ftp.Passive = True

        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Return
        End If


        success = ftp.Connect()
        If (success <> True) Then
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            Return
        End If

        success = ftp.ChangeRemoteDir(remote_dir)
        If (success <> True) Then
            Dim isConnected As Boolean = CheckConnection("http://www.google.com")
            Return
        End If

        ftp.ConnectTimeout = 60 * 1000 'Time out in second
        ftp.ReadTimeout = 60 * 3000 'Time out in second
        success = ftp.PutFile(FiletoUpload, FileName)
        If (success <> True) Then
            'MsgBox(ftp.LastErrorText)
            Return
        End If

    End Sub


    Public Function DownloadFile(ByVal localFilename As String, ByVal remoteFilename As String, ByVal remote_dir As String) As Boolean
        Dim ftp As New Chilkat.Ftp2()

        Dim success As Boolean

        ' Any string unlocks the component for the 1st 30-days.
        success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        If (success <> True) Then
            Return False
        End If
        ftp.Hostname = "upload.mygws.com"
        ftp.Username = Common.FTPUserName
        ftp.Password = Common.FTPPass
        ftp.Passive = True

        If remote_dir = Common.currentXMLFolder Then
            ftp.Username = "TeamREMIS"
            ftp.Password = "$MSR@mat"
        End If
        ' Connect and login to the FTP server.

        success = ftp.Connect()
        If (success <> True) Then

            Return False
        End If

        success = ftp.ChangeRemoteDir(remote_dir)
        If (success <> True) Then
            Return False
        End If
        ' Download a file.
        'd29c02f5-ac8b-439b-81fd-e790fd439135-Rome GA.xml
        'TREMIS_Rome_GA_d29c02f5-ac8b-439b-81fd-e790fd439135.xml
        success = ftp.GetFile(remoteFilename, localFilename)
        If (success <> True) Then

            Return False
        End If

        ftp.Disconnect()
        Return True

    End Function


    Public Function AES_Encrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim encrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = System.Security.Cryptography.CipherMode.ECB
            Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(input)
            encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return encrypted
        Catch ex As Exception
            Return ""

        End Try
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



    Public Sub GetGTPConfig()
        'Try
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\GTPConfig.xml")
        Dim Element As XmlNode
        Element = xmlDoc.SelectSingleNode("/ConfigRoot/XMLFolder")
        If Not IsNothing(Element) Then
            Common.currentXMLFolder = Element.Attributes("Name").Value
            If (Common.currentXMLFolder = "") Then
                MessageBox.Show("Please enter XMlFolder name for GTP XML mode in GTPConfig.xml")
            End If
        End If
        'Catch ex As Exception
        '    MessageBox.Show("Your GTPConfig is missing or error ! Please check before using GTP in XML mode !")
        'End Try

    End Sub



End Module

