
Imports System.Net
Imports System.IO
Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.Net.Mail
Imports System.Threading
Imports GWSFTP.GTPService
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Collections.Concurrent
Imports Chilkat
Imports Ionic
Imports System.Xml
Imports System.Xml.Serialization
Imports SevenZip
Imports System.Security

Structure FileUploadInfo
    Dim FileName As String
    Dim Status As Integer
    Dim TextColor As String
    Dim isUploading As Boolean
End Structure


Public Class frmUpload



    Private currentListViewItem As List(Of String)
    Private reuploadFileList As List(Of String)


    Private isSelectFile As Boolean
    Private isPcapFileExist As Boolean
    Private tmpSelectedUploadFiles As String()
    Private selectedUploadFiles As New List(Of String)
    Private _uploadedFiles As New List(Of String)
    Private selectedUploadFilesExist As New List(Of String)
    'Private _folderListToDelete As New List(Of String)

    Private pcapFileList As List(Of String)
    Private isUploadPcap As Boolean
    Private _isRunUploadPcap As Boolean
    Private formLoad As Boolean = True
    Private Match As Boolean
    Private isValid As Boolean

    Friend dbFile As String = AppDomain.CurrentDomain.BaseDirectory & "\Database\FTPDB.mdb"
    Friend cnnstr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbFile & ";Jet OLEDB:Database Password=jignesh0;"

    Private currentMarket = ""

    Private DeleteButton_ToolTip As ToolTip
    Private ToolTip_CloseButton As ToolTip


    'Private _FileUpload1 As FileUploadInfo()
    'Private _FileUpload2 As FileUploadInfo()
    Private _FileUpload As FileUploadInfo()
    Private _AllFileUpload As FileUploadInfo()

    Private _isFinished1 As Boolean = False
    Private _isFinished2 As Boolean = False

    Private isProccessed As Boolean
    Private transaction_status_server As Integer
    Private id_transaction_reupload As Integer
    Private id_server_transaction_reupload As Integer
    Private id_transaction_finished As Integer
    Private id_server_transaction_finished As Integer
    Private FilestoUpload() As FileUploadInfo
    Private isFileInfoCompleted As Boolean = False
    Private total_file_to_uplload As Integer

    Private total_uploaded_file As Integer = 0

    Private ReuploadProcess As Integer
    Private uploadStatus As Integer
    'Private FiletoUpload As String
    Private FTPURL As String
    Dim TransactionGID As Guid
    Private id_FTP_Transaction As String
    Private id_FTP_Client As String
    Private GWSFTP As GWSFTP
    Private count_uploaded_files As Integer
    Private FileList As String()
    Private FileList1 As String()
    Private FileList2 As String()
    Private isFileList1Ending As Boolean

    Private FileRemaining_Resume As String()

    Private total_file_failed As Integer
    Private isSucessUploaded As Boolean
    Private count_uploaded As Integer

    Private is_network_lost As Boolean
    Private clients As _Client()

    Private allFile As List(Of String)

    Private failedTrimmingFiles As New List(Of String)
    Private lstClient As New List(Of _Client)
    Private teamProjects As New List(Of _TeamProjects)

    Private Sub DeleteTmpFolder()
        Dim i As Integer
        If Not IsNothing(allFile) Then
            For i = 0 To allFile.Count - 1
                Try
                    Dim fileName As String = allFile(i)
                    Dim tmpFolder As String = fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + fileName.Substring(fileName.LastIndexOf(".") + 1)
                    If Directory.Exists(tmpFolder) Then
                        Try
                            Directory.Delete(tmpFolder, True)
                        Catch ex As Exception
                            Dim ss As String = ""
                        End Try
                    End If
                Catch ex As Exception

                End Try

            Next
        End If
    End Sub

    Private Sub GetClient()
        If Common.hasWCFAccess And Not Common.forceXMLMode Then
            Dim proxy As New GTPServiceClient()
            Dim clientList As String()
            proxy.Open()
            clientList = proxy.GetClientForTeam(Common.REMIS_username)
            If clientList.Length > 0 Then
                For Each client In clientList
                    ComboBox_Project.Items.Add(client)
                Next
            End If
            clients = proxy.GetClient()
            Dim c As New _Client
            If clientList.Length = 0 Then
                For Each c In clients
                    ComboBox_Project.Items.Add(c.ClientName)
                Next
            End If
            proxy.Close()
        Else
            Dim xmlDoc = New XmlDocument()
            xmlDoc.Load("project.xml")
            EncryptDecrypt.Program.Decrypt(xmlDoc, "GWSsol")
            Dim ClientNodes As XmlNodeList
            ClientNodes = xmlDoc.SelectNodes("/ProjectRoot/TeamProject")
            Dim node As XmlNode
            If ClientNodes.Count > 0 Then
                For Each node In ClientNodes
                    Dim teamProject As New _TeamProjects
                    teamProject.UserName = node.Attributes("userName").Value
                    teamProject.ClientName = node.Attributes("clientName").Value
                    teamProjects.Add(teamProject)
                Next
                Dim clientNames = teamProjects.Where(Function(p) p.UserName = Common.REMIS_username).Select(Function(p) p)
                If clientNames.Count > 0 Then
                    For Each clientName In clientNames
                        ComboBox_Project.Items.Add(clientName.ClientName)
                    Next
                Else
                    Dim clientXmlDoc = New XmlDocument()
                    clientXmlDoc.Load("client.xml")
                    EncryptDecrypt.Program.Decrypt(clientXmlDoc, "GWSsol")
                    Dim client As XmlNodeList
                    client = clientXmlDoc.SelectNodes("/ClientRoot/Client")
                    Dim clientnode As XmlNode
                    If client.Count > 0 Then
                        For Each clientnode In client
                            Dim clientName As New _Client
                            clientName.ClientName = clientnode.Attributes("Name").Value
                            clientName.Campaigns = clientnode.Attributes("Campaigns").Value
                            lstClient.Add(clientName)
                        Next
                        Dim allClientNames = lstClient.Select(Function(p) p.ClientName).Distinct()
                        For Each clientName In allClientNames
                            ComboBox_Project.Items.Add(clientName)
                        Next
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub CheckDuplicateLocal_Background()

        Dim count1 As Integer = 0
        Dim listitem As ListViewItem
        Dim FileInfo As FileInfo

        Dim hash As HashSet(Of String) = New HashSet(Of String)()

        For Each listitem In ListView_FileList.Items
            If listitem.Text.Trim.ToLower.EndsWith(".mf") Or listitem.Text.Trim.ToLower.EndsWith(".sqz") Then
                Dim tmp3 = listitem.Text.Substring(listitem.Text.LastIndexOf("\") + 1)
                Dim tmp2 As String = tmp3.Substring(0, tmp3.LastIndexOf("."))
                If Not hash.Contains(tmp2.ToLower()) Then
                    hash.Add(tmp2.ToLower())
                Else
                    count1 = count1 + 1
                    Dim lvi As New ListViewItem
                    ' First Column can be the listview item's Text  
                    lvi.Text = listitem.Text
                    lvi.ForeColor = Color.Brown
                    ListView_Dups.Items.Add(lvi)
                    TabControl1.SelectedIndex = 3
                    FileInfo = New FileInfo(listitem.Text)
                    lvi.SubItems.Add(FileInfo.Length)
                    listitem.Remove()
                End If
            End If
        Next

        If count1 > 0 Then
            TabControl1.SelectedIndex = 3
        End If
    End Sub



    Private Sub CheckDuplicateLocal(ByVal fileList As List(Of String))

        Dim count1 As Integer = 0

        Dim FileInfo As FileInfo

        Dim hash As HashSet(Of String) = New HashSet(Of String)()
        Dim tmpRemoveList As New List(Of String)

        Dim uniqueFileList As New List(Of String)

        Dim hash_file As HashSet(Of String) = New HashSet(Of String)()

        'uniqueFileList = _uploadedFiles

        For Each fileStr In fileList
            If Not uniqueFileList.Contains(fileStr) Then
                uniqueFileList.Add(fileStr)
            Else
                Dim lvi As New ListViewItem
                lvi.Text = fileStr
                lvi.ForeColor = Color.Brown

                FileInfo = New FileInfo(fileStr)
                lvi.SubItems.Add(FileInfo.Length)
                AddListViewItem1(ListView_Dups, lvi)
                SelectTabIndex_Sub(TabControl1, 3)
            End If

        Next

        selectedUploadFiles = uniqueFileList
        Dim hash_uploaded As New HashSet(Of String)

        For Each fileStr In _uploadedFiles
            Dim tmp3 = fileStr.Substring(fileStr.LastIndexOf("\") + 1)
            Dim tmp2 As String = tmp3.Substring(0, tmp3.LastIndexOf("."))

            tmp3 = tmp3.Split(".")(0)
            tmp2 = tmp2.Split(".")(0)

            If Not hash_uploaded.Contains(tmp2.ToLower()) Then
                hash_uploaded.Add(tmp2.ToLower())
            End If
        Next
        currentListViewItem = New List(Of String)
        GetListViewItem(ListView_FileList)

        Dim hashMap As New Hashtable()
        For Each fileStr In uniqueFileList
            If fileStr.Trim.ToLower.EndsWith(".mf") Or fileStr.Trim.ToLower.EndsWith(".sqz") Then

                If currentListViewItem.Contains(fileStr) Then
                    Dim tmp3 = fileStr.Substring(fileStr.LastIndexOf("\") + 1)
                    Dim tmp2 As String = tmp3.Substring(0, tmp3.LastIndexOf("."))

                    tmp3 = tmp3.Split(".")(0)
                    tmp2 = tmp2.Split(".")(0)

                    If Not hashMap.ContainsKey(tmp2.ToLower()) Then
                        Dim tmpfileList As New List(Of String)
                        tmpfileList.Add(fileStr)
                        hashMap.Add(tmp2.ToLower(), tmpfileList)
                    Else
                        hashMap.Item(tmp2.ToLower()).Add(fileStr)
                    End If
                End If

            End If
        Next

        For Each key In hashMap.Keys
            If hashMap.Item(key).Count > 1 Then
                Dim tmp_count As Integer = hashMap.Item(key).Count
                Dim current_count As Integer = 0
                For Each tmpStr1 In hashMap.Item(key)

                    current_count = current_count + 1

                    If Not _uploadedFiles.Contains(tmpStr1) Then

                        Dim lvi As New ListViewItem
                        ' First Column can be the listview item's Text  
                        lvi.Text = tmpStr1
                        lvi.ForeColor = Color.Brown
                        'ListView_Dups.Items.Add(lvi)
                        'TabControl1.SelectedIndex = 3
                        FileInfo = New FileInfo(tmpStr1)
                        lvi.SubItems.Add(FileInfo.Length)
                        'listitem.Remove()
                        RemoveListViewItem(ListView_FileList, tmpStr1)

                        tmpRemoveList.Add(tmpStr1)

                        'ListView_Duplicate.Items.Add(lvi)
                        AddListViewItem1(ListView_Dups, lvi)
                        SelectTabIndex_Sub(TabControl1, 3)

                    End If

                    If current_count = tmp_count - 1 Then
                        Exit For
                    End If
                Next
            End If
        Next


        Dim tmpStr As String
        For Each tmpStr In tmpRemoveList
            selectedUploadFiles.Remove(tmpStr)

        Next


        'If count1 > 0 Then
        '    TabControl1.SelectedIndex = 3
        'End If

    End Sub

    Private Sub CheckDuplicate(ByVal fileList As List(Of String))
        Dim proxy As New GTPServiceClient()
        Dim isExist As Boolean
        Dim count As Integer = 0
        Dim listitem As ListViewItem
        Dim FileInfo As FileInfo

        For Each fileStr In fileList
            If File.Exists(fileStr) Then
                Dim FileName As String = fileStr.Substring(fileStr.LastIndexOf("\") + 1, fileStr.Length - fileStr.LastIndexOf("\") - 1)

                Dim imageStr As String = "*.jpg,*.jpeg,*.jpe,*.png,*.bmp,*.gif,*.tif,*.mpo,*.heic,*.heif,*.heiv,*.avi,*.flv,*.wmv,*.mov,*.mp4"
                Dim ext As String = Common.GetFileExtension(FileName)
                If Not imageStr.Contains("*." + ext.ToLower()) Then
                    isExist = proxy.isFileExist(FileName)
                    If isExist Then
                        count += 1
                        Dim lvi As New ListViewItem
                        ' First Column can be the listview item's Text  
                        lvi.Text = fileStr
                        lvi.ForeColor = Color.Brown

                        FileInfo = New FileInfo(fileStr)
                        lvi.SubItems.Add(FileInfo.Length)

                        RemoveListViewItem(ListView_FileList, fileStr)
                        'ListView_Duplicate.Items.Add(lvi)
                        AddListViewItem1(ListView_Duplicate, lvi)
                        SelectTabIndex_Sub(TabControl1, 1)
                        'TabControl1.SelectedIndex = 1
                    End If
                End If

            End If

        Next

        proxy.Close()
    End Sub

    Private Sub addTrimmingFailed(ByVal fileNameStr As String)
        selectTabControlIndex(TabControl1, 2)
        If File.Exists(fileNameStr) Then
            Dim FileName As String = fileNameStr.Substring(fileNameStr.LastIndexOf("\") + 1, fileNameStr.Length - fileNameStr.LastIndexOf("\") - 1)
            AddListViewItem(ListView_FileTrimmingFailed, fileNameStr)
            RemoveListViewItem(ListView_FileList, fileNameStr)
            'Update status to local DB
        End If
    End Sub

    Private Sub addTrimmingFailed()



        Dim i As Integer
        For i = 0 To failedTrimmingFiles.Count - 1
            Dim FileInfo As FileInfo
            If File.Exists(failedTrimmingFiles(i)) Then
                Dim FileName As String = failedTrimmingFiles(i).Substring(failedTrimmingFiles(i).LastIndexOf("\") + 1, failedTrimmingFiles(i).Length - failedTrimmingFiles(i).LastIndexOf("\") - 1)
                AddListViewItem(ListView_FileTrimmingFailed, failedTrimmingFiles(i))
                selectTabControlIndex(TabControl1, 2)
            End If
        Next
    End Sub

    Private Sub ProcessPCAPFiles()

        pcapFileList = New List(Of String)

        Dim success As Boolean
        Dim zip As New Chilkat.Zip
        Dim unlocked As Boolean
        unlocked = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (Not unlocked) Then
            'MsgBox(zip.LastErrorText)
            Return
        End If

        For Each listitem In ListView_FileList.Items
            Dim FileName As String = ""


            If listitem.Text.ToLower.Contains(".sqz") Then
                If File.Exists(listitem.Text) Then

                    Dim isRemove As Boolean = False
                    success = zip.OpenZip(listitem.Text)

                    '2016-08-19-08-42-53-0000-7086-1873-0070-B.sqz
                    Dim isPCAP As Boolean = False
                    If listitem.Text.ToLower.EndsWith(".sqz") And success Then
                        For i = 0 To zip.NumEntries - 1
                            Dim entry As ZipEntry = zip.GetEntryByIndex(i)
                            If Not IsNothing(entry) Then
                                If entry.FileName.ToLower.EndsWith(".pcap") Then
                                    isPCAP = True
                                    FileName = entry.FileName
                                    Exit For
                                End If

                            End If
                        Next
                    End If

                    If isPCAP Then
                        Dim tmpFolderName As String = listitem.Text + "_PCAP"
                        Dim unzipCount As Integer
                        Directory.CreateDirectory(tmpFolderName)

                        unzipCount = zip.Unzip(tmpFolderName)
                        zip.CloseZip()
                        zip.Dispose()
                        'Rename file
                        Dim tmpFileName As String = listitem.Text.ToString().Substring(0, listitem.Text.ToString().LastIndexOf("."))


                        FileSystem.Rename(listitem.Text, tmpFileName + ".pcapzip")


                        Dim Subfolders() As String = Common.getAllFolders(tmpFolderName)

                        Dim di As IO.DirectoryInfo

                        Dim diar1 As IO.FileInfo()

                        Dim dra As IO.FileInfo

                        For Each f In Subfolders
                            di = New IO.DirectoryInfo(f)
                            diar1 = di.GetFiles()
                            For Each dra In diar1
                                If dra.FullName.EndsWith(".pcap") Then
                                    File.Delete(dra.FullName)
                                End If

                            Next
                        Next

                        Common.ZipFolder(listitem.Text, tmpFolderName)
                        Directory.Delete(tmpFolderName, True)
                        pcapFileList.Add(tmpFileName + ".pcapzip")
                    End If


                End If

            End If
        Next
    End Sub


    Private Function CheckPCAP(ByVal FileName As String) As Boolean
        Dim zip As New Chilkat.Zip
        Dim unlocked As Boolean
        unlocked = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (Not unlocked) Then
            'MsgBox(zip.LastErrorText)
            Return True
        End If
        Dim success As Boolean
        success = zip.OpenZip(FileName)
        If success Then
            For i = 0 To zip.NumEntries - 1
                Dim entry As ZipEntry = zip.GetEntryByIndex(i)
                If Not IsNothing(entry) Then
                    If entry.FileName.ToLower.EndsWith(".pcap") Then
                        Return True
                    End If
                End If
            Next
        End If
        Return False
    End Function


    Private Function UnzipTest() As Boolean

        Label_Info.Text = "Performing the unzip test. PLEASE WAIT … "
        Label_Info.Update()
        Dim TmpPath As String = "C:\GWSFTPUnzipTest"
        'C:\GWSFTPUnzipTest
        Dim zip As New Chilkat.Zip
        Dim unlocked As Boolean
        unlocked = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (Not unlocked) Then
            'MsgBox(zip.LastErrorText)
            Return True
        End If
        Dim success As Boolean

        Dim i As Integer
        Dim count As Integer = 0
        Dim listitem As ListViewItem
        Dim FileInfo As FileInfo

        For Each listitem In ListView_FileList.Items
            If listitem.Text.ToLower.Contains(".sqz") Then
                If File.Exists(listitem.Text) Then

                    Dim isRemove As Boolean = False
                    success = zip.OpenZip(listitem.Text)

                    '2016-08-19-08-42-53-0000-7086-1873-0070-B.sqz

                    If listitem.Text.ToLower.EndsWith(".sqz") And success Then

                        For i = 0 To zip.NumEntries - 1
                            Dim entry As ZipEntry = zip.GetEntryByIndex(i)
                            If Not IsNothing(entry) Then

                                If entry.FileName.ToLower.EndsWith(".mf") Then

                                    Dim tmp1 As String = entry.FileName.ToLower.Substring(0, entry.FileName.LastIndexOf("."))
                                    Dim tmp3 = listitem.Text.Substring(listitem.Text.LastIndexOf("\") + 1)

                                    Dim tmp2 As String = tmp3.Substring(0, tmp3.LastIndexOf("."))

                                    If Not tmp2.ToLower = tmp1.ToLower Then
                                        count += 1
                                        Dim lvi As New ListViewItem
                                        ' First Column can be the listview item's Text  
                                        lvi.Text = listitem.Text
                                        lvi.ForeColor = Color.Brown
                                        ListView_OtherFiles.Items.Add(lvi)
                                        TabControl1.SelectedIndex = 0
                                        FileInfo = New FileInfo(listitem.Text)
                                        lvi.SubItems.Add(FileInfo.Length)
                                        listitem.Remove()
                                        isRemove = True


                                    End If

                                    Exit For

                                End If

                            End If
                        Next
                    End If

                    If (Not success) And (Not isRemove) Then
                        Try
                            'Check if 7zip
                            SevenZipExtractor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "7z.dll")
                            Dim extractor As New SevenZipExtractor(listitem.Text)
                            If (extractor.ArchiveFileData.Count <= 0) Then
                                count += 1
                                Dim lvi As New ListViewItem
                                ' First Column can be the listview item's Text  
                                lvi.Text = listitem.Text
                                lvi.ForeColor = Color.Brown
                                ListView_OtherFiles.Items.Add(lvi)
                                TabControl1.SelectedIndex = 0
                                FileInfo = New FileInfo(listitem.Text)
                                lvi.SubItems.Add(FileInfo.Length)
                                listitem.Remove()
                            End If
                        Catch
                            count += 1
                            Dim lvi As New ListViewItem
                            ' First Column can be the listview item's Text  
                            lvi.Text = listitem.Text
                            lvi.ForeColor = Color.Brown
                            ListView_OtherFiles.Items.Add(lvi)
                            TabControl1.SelectedIndex = 0
                            FileInfo = New FileInfo(listitem.Text)
                            lvi.SubItems.Add(FileInfo.Length)
                            listitem.Remove()
                        End Try
                    End If
                End If

            End If
        Next

        If count > 0 Then
            TabControl1.SelectedIndex = 0
            Label_OtherFiles.Text = "Unzip Failed"
            Dim Button = MessageBox.Show("There are a total of " + count.ToString() + " file(s) that failed to unzip. They will be removed from the upload list. Would you like to continue uploading? ", "Message", MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

            Label_TotalFile.Text = "Total File(s): " + ListView_FileList.Items.Count.ToString

            If Button = Windows.Forms.DialogResult.Yes Then
                Return True
            Else
                Return False
            End If
        Else
            Label_OtherFiles.Text = "Files removed from the local path"
        End If

        Label_Info.Text = "Preparing for upload. PLEASE WAIT…"
        Label_Info.Update()
        Thread.Sleep(1000)

        Return True

    End Function

    Private Sub CreateMarketXML()

        ComboBox_Market.Items.Clear()
        Dim proxy1 As GTPServiceClient
        proxy1 = New GTPServiceClient()
        Dim Markets As _MarketName()
        Dim i, j As Integer
        Dim AllMarkets As New List(Of _MarketName)
        Dim RootElement As XmlElement
        Dim xmlDoc As New XmlDocument()
        Common.MarketXMLFile = AppDomain.CurrentDomain.BaseDirectory + "market.xml"
        Dim dec As XmlDeclaration
        dec = xmlDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
        xmlDoc.AppendChild(dec)
        RootElement = xmlDoc.CreateElement("MarketRoot")
        xmlDoc.AppendChild(RootElement)
        clients = proxy1.GetClient()
        Dim marketStatus = clients.OrderBy(Function(x) x.client_market_value).Select(Function(p) p.client_market_value).Distinct()
        For Each j In marketStatus
            If j <> -1 Then
                Markets = proxy1.GetMarket(j)
                For i = 0 To Markets.Count - 1
                    AllMarkets.Add(Markets(i))
                Next
            End If
            'For j = 2 To marketStatus.Count
        Next
        Dim marketElement As XmlElement
        If AllMarkets.Count() > 0 Then
            For i = 0 To AllMarkets.Count - 1
                ComboBox_Market.Items.Add(AllMarkets(i).name_market.Replace(",", ""))
                marketElement = xmlDoc.CreateElement("Market")
                marketElement.SetAttribute("Name", AllMarkets(i).name_market.Replace(",", ""))
                marketElement.SetAttribute("market_status", AllMarkets(i).market_status)
                marketElement.SetAttribute("id_market", AllMarkets(i).id_market.ToString())
                RootElement.AppendChild(marketElement)
            Next
        End If
        ComboBox_Market.Items.Clear()
        EncryptDecrypt.Program.Encrypt(xmlDoc, "MarketRoot", "GWSsol").Save(Common.MarketXMLFile)
        'xmlDoc.Save(Common.MarketXMLFile)
        proxy1.Close()
    End Sub

    Private Function isEnoughDriveSpace(ByVal drive As String) As Boolean
        Try
            Dim cdrive As System.IO.DriveInfo
            cdrive = My.Computer.FileSystem.GetDriveInfo(drive + ":\")
            If cdrive.IsReady Then
                Dim total_free_space As Double = Math.Round(cdrive.TotalFreeSpace / 1024 / 1024 / 1024, 2)

                If total_free_space < 1.5 Then
                    MessageBox.Show("Please clean up before upload. Total space for drive " + drive + " need at least 1.5G for data trimming. Currently there is only " + total_free_space.ToString() + "GB")
                    Return False
                End If

            End If

        Catch ex As Exception

        End Try
        Return True
    End Function


    Private Sub Button_Upload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Upload.Click


        'Directory.CreateDirectory("c:\GTPTest\STTicket\Scnr file status_flag 9\kamsas7\k78\RuS TSMW (101771)\2019-10-15-16-11-38\CSM0\Scanner RuS TSMW (101763)\")
        ReDim tmpSelectedUploadFiles(0)


        _isRunUploadPcap = False

        Dim uploadFiles As New List(Of String)
        Dim item As ListViewItem
        For Each item In ListView_FileList.Items
            uploadFiles.Add(item.Text)
        Next
        For Each item In ListView_Duplicate.Items
            uploadFiles.Add(item.Text)
        Next
        _Project = ComboBox_Project.Text
        Common._missingList = New HashSet(Of String)
        ListView_MissingFile.Items.Clear()

        If _Project = "AT&T BM" Then
            Dim fileGroup As New List(Of _FileGroupInfo)()
            fileGroup = Common.GetFileGroupInfo(uploadFiles)
            CheckFileGroupInfo(fileGroup)
            If Common._missingList.Count > 0 Then

                For Each itemStr In _missingList
                    ListView_MissingFile.Items.Add(itemStr)
                Next
                TabControl1.SelectedIndex = 4

                Dim message, title, defaultValue As String
                defaultValue = ""
                Dim myValue As Object
                ' Set prompt.
                message = "Expected filenames are missing. Type ""I agree"" and upload will proceed."
                ' Set title.
                title = "Confirm Upload"


                ' Display message, title, and default value.
                myValue = InputBox(message, title, defaultValue)

                If myValue.ToString.ToLower().Trim() <> "i agree" And myValue.ToString.ToLower().Trim() <> "" Then
                    MessageBox.Show("You must type ""I agree"" or modify file selection.")
                    Return
                ElseIf myValue.ToString.ToLower().Trim() = "" Then
                    Return
                End If

            Else
                ListView_Duplicate.Focus()
            End If
        End If

        'Return

        If (CheckBox_Duplicate.Checked) Then
            Dim file1 As System.IO.StreamWriter
            file1 = My.Computer.FileSystem.OpenTextFileWriter(Common.rememberCheckBox, False)
            file1.WriteLine("1")
            file1.Close()
        Else
            Dim file1 As System.IO.StreamWriter
            file1 = My.Computer.FileSystem.OpenTextFileWriter(Common.rememberCheckBox, False)
            file1.WriteLine("0")
            file1.Close()
        End If


        If ListView_FileList.Items.Count = 0 Then
            MessageBox.Show("Please select file to upload")
            Return
        End If

        Dim drive As String = ListView_FileList.Items(0).Text.Substring(0, 1)
        If Not isEnoughDriveSpace(drive) Then
            Return
        End If

        Common.isShowCompleteMessage = False

        If TextBox_UserName.Text.Trim = "" Then
            MessageBox.Show("Please enter username and password")
            Return
        End If


        Common.CountUploaded = 0

        'Me.ControlBox = False        
        GroupBox_Action.Visible = False
        'TextBox_FTPSessionLog.Height = 283
        Dim success As Boolean
        Dim ftp As New Chilkat.Ftp2()

        ftp.Hostname = TextBox_Server.Text.Trim
        ftp.Username = TextBox_UserName.Text
        ftp.Password = TextBox_Password.Text

        Common.FTPServerName = TextBox_Server.Text.Trim
        Common.FTPUserName = TextBox_UserName.Text
        Common.FTPPass = TextBox_Password.Text

        'Get shadow credentials
        'Dim shadowCredential = Common.GetShadowCredential()

        'If (Not IsNothing(shadowCredential)) And Common.enableShadowPassword Then
        '    If shadowCredential.type = 1 Then
        '        Common.FTPUserName = shadowCredential.username
        '        Common.FTPPass = shadowCredential.password
        '    Else
        '        Common.FTPUserName += shadowCredential.username
        '        Common.FTPPass += shadowCredential.password
        '    End If

        'End If

        'success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
        'If (success <> True) Then
        '    MsgBox(ftp.LastErrorText)
        '    Return
        'End If
        'success = ftp.Connect()
        'If (success <> True) Then
        '    MessageBox.Show("Failed to connect to FTP server. Please check the FTP server name, username and password")
        '    Return
        'End If

        If Not Directory.Exists("C:\GWSFTPlogs") Then
            Directory.CreateDirectory("C:\GWSFTPlogs")
        End If

        'Get DB status
        Dim proxy As GTPServiceClient
        If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
            proxy = New GTPServiceClient()
            Common.AssignCredentials(proxy)
            Try
                Dim status As _DBStatus() = proxy.GetDBStatus(1)
                If status.Count() > 0 Then
                    MessageBox.Show(status(0).status_notify)
                    Me.Close()
                    Return

                End If

            Catch ex As Exception
                If Not CheckConnection("http://www.google.com") Then
                    MessageBox.Show("No Internet connection. Please check !")
                Else
                    MessageBox.Show("GWS FTP system is currently offline due to a critical failure.  Please try again in 15 minutes.")
                End If

                Me.Close()
                Return

            End Try

            proxy.Close()

        End If

        FTPSessionLog = ""

        Dim market_name As String = ComboBox_Market.Text
        If (CheckBox_NewMarket.Checked) Then
            market_name = ComboBox_PendingMarket.Text
        End If

        If ComboBox_Market.Text = "" And ComboBox_PendingMarket.Text = "" Then
            If Button_Upload.Text <> "Re-Upload" Then
                MessageBox.Show("Market name cannot be empty !")
                Return
            End If
        End If

        If ComboBox_Drive.Text = "" Then
            If Button_Upload.Text <> "Re-Upload" Then
                MessageBox.Show("No Campaign Selected…")
                Return
            End If
        End If

        count_uploaded = 0

        If ListView_FileList.Items.Count = 0 Then
            Return
        End If

        Label_TotalFile.Text = "Total File(s): " + ListView_FileList.Items.Count.ToString

        isUploadPcap = False

        If Button_Upload.Text <> "Re-Upload" Then
            Dim Button = MessageBox.Show("Please confirm market name: " + market_name + "  and campaign: " + ComboBox_Drive.Text, "Message", MessageBoxButtons.YesNoCancel,
                                 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

            If Button = Windows.Forms.DialogResult.Yes Then
                If UnzipTest() Then
                    Button_Upload.Enabled = False

                    If IsNothing(pcapFileList) And isPcapFileExist Then
                        RadioButton_SQZ.Enabled = True
                        RadioButton_SQZ_PCAP.Enabled = True
                        GroupBox_PCAP_Options.Enabled = True
                        ProcessPCAPFiles()
                    End If

                    If Not IsNothing(pcapFileList) Then
                        If pcapFileList.Count > 0 Then

                            Dim pcap_option As String = "SQZ/MF + PCAPZIP files" + Environment.NewLine + "(All SQZ and MF files will be uploaded first, then all PCAPZIP files will be uploaded afterwards.)"

                            If RadioButton_SQZ.Checked Then
                                pcap_option = "Only SQZ/MF files" + Environment.NewLine + "(No PCAPZIP files will be uploaded.  To upload any PCAPZIP files, you will have to select these at a later time.)"
                            End If

                            Dim ButtonPCAP = MessageBox.Show("1. At least one selected SQZ file contains PCAP files." + Environment.NewLine + " (Any such SQZ file will be renamed as PCAPZIP in your folder, and a new SQZ will be created that does not contain any PCAP files.)" + Environment.NewLine + Environment.NewLine + "2. You selected PCAP upload option: " + pcap_option + Environment.NewLine + Environment.NewLine + "3. Do you want to continue ? ", "Message", MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

                            If ButtonPCAP = Windows.Forms.DialogResult.No Then
                                Button_Upload.Enabled = True
                                Return
                            End If

                        End If
                    End If
                    Button_Upload_Do(0)

                    'Adding unzip files failed to the database.  SP'
                    If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                        For Each listitem In ListView_OtherFiles.Items
                            Dim fileName As String = Path.GetFileName(listitem.text)
                            Using proxy1 As New GTPServiceClient()
                                Try
                                    Dim fileNameWithoutExt As String = Path.GetFileNameWithoutExtension(fileName)
                                    Dim extension As String = Path.GetExtension(fileName)
                                    Dim res As String = proxy1.AddFTPFileFailed(id_FTP_Transaction, fileName, fileNameWithoutExt, extension)
                                Catch ex As Exception
                                    If Not CheckConnection("http://www.google.com") Then
                                        MessageBox.Show("No Internet connection. Please check !")
                                    Else
                                        MessageBox.Show("GWS FTP system is currently offline due to a critical failure.  Please try again in 15 minutes.")
                                    End If
                                End Try
                            End Using
                        Next
                    End If

                    If Not IsNothing(pcapFileList) Then
                        If pcapFileList.Count > 0 Then
                            isUploadPcap = True
                        End If

                    End If

                Else
                    Return
                End If
            ElseIf Button = Windows.Forms.DialogResult.Cancel Then
                ListView_FileList.Items.Clear()
                ListView_OtherFiles.Items.Clear()
                ListView_MissingFile.Items.Clear()
                ComboBox_Market.SelectedIndex = -1
                Label_TotalFile.Text = "Total File(s): "
                Button_SelectFolder.Text = "Select Upload Files"
                Return
            Else
                Label_TotalFile.Text = "Total File(s): "
                Return
            End If
        Else
            Button_Upload.Enabled = False
            Button_Upload_Do(0)
        End If

        Try
            total_file_failed = GetTotalFailed()
            If total_file_failed > 0 Then
                Label_TotalFailed.Text = "Total file(s) failed Upload:" + total_file_failed.ToString()
            Else
                Label_TotalFailed.Text = ""
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Function GetTotalUploaded() As Integer
        Dim total_uploaded As Integer = 0
        For i = 0 To ListView_FileList.Items.Count - 1
            If ListView_FileList.Items.Item(i).ForeColor = Color.Green Then
                total_uploaded += 1
            End If
        Next
        Return total_uploaded
    End Function

    Private Function GetTotalFailed() As Integer
        Dim total_failed As Integer = 0
        For i = 0 To ListView_FileList.Items.Count - 1
            If ListView_FileList.Items.Item(i).ForeColor = Color.Red Then
                total_failed += 1
            End If
        Next
        Return total_failed
    End Function


    Private Sub Button_Upload_Do(ByVal process As Integer)

        _isTransactionStart = True
        Dim FileInfo As FileInfo
        If isUploadPcap Then
            ListView_FileList.Items.Clear()

            For Each pcapFile In pcapFileList

                Dim lvi As New ListViewItem
                ' First Column can be the listview item's Text  
                lvi.Text = pcapFile
                ' Second Column is the first sub item  
                ' Add the ListViewItem to the ListView  
                ListView_FileList.Items.Add(lvi)
                FileInfo = New FileInfo(pcapFile)
                lvi.SubItems.Add(FileInfo.Length)

            Next


        End If

        If ListView_FileList.Items.Count = 0 Then
            MessageBox.Show("The file list is empty")
            Return
        End If

        allFile = New List(Of String)
        For i = 0 To ListView_FileList.Items.Count - 1
            allFile.Add(ListView_FileList.Items.Item(i).Text)
        Next
        Dim proxy As New GTPServiceClient()

        'Check if FTP folder exist, if not create a new folder
        CheckFTPFolderExist()


        total_file_failed = GetTotalFailed()
        Label_TotalFailed.Text = "Total file(s) failed Upload: " + total_file_failed.ToString()


        count_uploaded = GetTotalUploaded()
        total_uploaded_file = count_uploaded
        Label_Info.Text = "Uploading files ... "



        Label_Progress.Visible = True
        ProgressBar_Upload.Visible = True


        Dim id_server_transaction As String = ""
        Button_SelectFolder.Enabled = False
        Button1.Enabled = False
        frmMainApp.CloseToolStripMenuItem.Enabled = False


        Button2.Enabled = False
        Dim cmd As OleDbCommand
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr

        If transaction_status_server >= 3 Then
            'Update local transaction to completed                        
            cmd = New OleDbCommand
            cmd.CommandText = "UPDATE tblTransactionInfo SET [isCompleted] = True, [date_end] = Date() WHERE id_transaction = " + Label_id_local_transaction.Text
            cmd.Connection = cnn
            If cnn.State <> ConnectionState.Open Then
                cnn.Open()
            End If

            cmd.ExecuteNonQuery()
            cnn.Close()
        End If


        RememberFTPCredentials()

        'Get the FileList()
        Dim count_file1 As Integer = -1
        Dim count_file2 As Integer = -1
        Dim count_file3 As Integer = -1


        Dim TotalFile As Integer = ListView_FileList.Items.Count
        Dim FileUploaded() As String

        Dim Total_Remaining As Integer = 0
        For i = 0 To TotalFile - 1
            If ListView_FileList.Items.Item(i).ForeColor <> Color.Green Then
                Total_Remaining += 1
            End If
        Next


        ReDim Preserve _FileUpload(TotalFile - 1)
        If isUploadPcap Then
            ReDim _FileUpload(TotalFile - 1)
        End If

        For i = 0 To TotalFile - 1

            _FileUpload(i).FileName = ListView_FileList.Items.Item(i).Text
            _FileUpload(i).isUploading = False
            If ListView_FileList.Items.Item(i).ForeColor = Color.Green Then
                _FileUpload(i).Status = 1
            End If

            If ListView_FileList.Items.Item(i).ForeColor <> Color.Green Then
                count_file3 += 1
                ReDim Preserve FileList(count_file3)
                FileList(count_file3) = ListView_FileList.Items.Item(i).Text
            Else
                ReDim Preserve FileUploaded(count_uploaded)
                FileUploaded(count_uploaded) = ListView_FileList.Items.Item(i).Text
            End If


        Next

        Dim id_market As Integer = 0
        Dim username As String = TextBox_UserName.Text.Trim
        Dim file_info As String = ""

        'Insert transaction info to Access database

        Dim id_transaction As Integer = 0
        Dim Market As String = ""
        If ComboBox_PendingMarket.Text = "" Then
            If ComboBox_Market.SelectedIndex < 0 Then
                If Button_Upload.Text <> "Re-Upload" Then
                    MessageBox.Show("Market name is not on the list. Please select market from the list or click ""Enter New Market"" check box to add")
                    Return
                End If
            End If

        End If

        If CheckBox_NewMarket.Checked Then
            Market = ComboBox_PendingMarket.Text.Replace(",", "")
            id_market = ComboBox_PendingMarket.SelectedValue
        Else
            Market = ComboBox_Market.Text.Replace(",", "")
            id_market = ComboBox_Market.SelectedValue
        End If

        If (Market.Trim = "") Then
            Market = ComboBox_Market.Text.Replace(",", "")
        End If


        Market.Replace(" ", "")
        Dim DestinationFolder As String = ""


        Dim GWSUploadTransaction As GWSTransaction

        GWSUploadTransaction = New GWSTransaction
        GWSUploadTransaction.username = username
        GWSUploadTransaction.id_market = id_market
        GWSUploadTransaction.transaction_type = True
        GWSUploadTransaction.cnn = cnn
        GWSUploadTransaction.ftp_url = "ftp://" & TextBox_Server.Text.Trim + "/" + DestinationFolder
        GWSUploadTransaction.market_name = Market
        GWSUploadTransaction.Campaign = ComboBox_Drive.Text
        GWSUploadTransaction.Project = ComboBox_Project.Text

        Dim MyComputerInfo As New HostComputer
        MyComputerInfo.HostName = ""
        MyComputerInfo.Ip_address = ""
        MyComputerInfo.MAC_address = ""
        Common.GetIPAddress(MyComputerInfo.HostName, MyComputerInfo.Ip_address, MyComputerInfo.MAC_address)

        Dim transaction_info As String = ""


        If Button_Upload.Text <> "Re-Upload" Then

            Dim request_url As String
            Dim response As String
            Dim DBName As String
            Dim count_file As Integer = 0
            Dim count_error As Integer = 0
            Dim isInserted As Boolean = False

            Dim totalinputbytes As Double = 0

            Try

                id_server_transaction = 0
                id_transaction = 0
                id_transaction = GWSUploadTransaction.CreateTransaction(FileList)
                Common.currentIDLocalTransaction = id_transaction.ToString()
                Label_id_local_transaction.Text = id_transaction
                id_transaction_reupload = id_transaction
                'Create Transaction Info for sending to Web server
                'transaction_info = id_transaction.ToString + "@" + GWSUploadTransaction.transaction_type.ToString + "@" + username + "@" + id_market.ToString + "@" + GWSUploadTransaction.ftp_url + "@" + MyComputerInfo.HostName + "@" + MyComputerInfo.Ip_address + "@" + Market
                'request_url = website_root & "GWSFTP/create_transaction.aspx?transaction_info=" & transaction_info


                DBName = "T" + username.Substring(4, username.Length - 4) + "_" + Market.Replace(" ", "_")

                For i = 0 To FileList.Length - 1
                    Dim fileIn As New FileInfo(FileList(i))
                    totalinputbytes = totalinputbytes + fileIn.Length
                Next

                totalinputbytes = Math.Round(totalinputbytes)

                If Common.hasWCFAccess And Not Common.forceXMLMode Then

                    Try

                        id_server_transaction = proxy.tblFTPTrasaction(0, 0, Convert.ToInt32(id_transaction), 1, DateTime.Now, username, 0, 1, 0, id_market, GWSUploadTransaction.ftp_url, MyComputerInfo.Ip_address, MyComputerInfo.HostName + "|Ver:" + VersionNumber, Market, 0, DBName, 0, ComboBox_Drive.Text.Trim, ComboBox_Project.Text, totalinputbytes)
                        If Not IsNothing(id_server_transaction) Then
                            If id_server_transaction <= 0 Then
                                Common.hasWCFAccess = False
                                Common.forceXMLMode = True
                                CheckBox_Mode.Checked = True
                                GoTo LabelXMLMode

                            End If
                        Else
                            Common.hasWCFAccess = False
                            Common.forceXMLMode = True
                            CheckBox_Mode.Checked = True
                            GoTo LabelXMLMode
                        End If

                        'Incorrect syntax near 'Alene'

                    Catch ex As Exception
                        Common.AppendTextFile(Common.LogFileName, "id_server_transaction = proxy.tblFTPTrasaction(0, 0, Convert.ToInt32(id_transaction), 1, DateTime.Now, username, 0, 1, 0, id_market, GWSUploadTransaction.ftp_url, MyComputerInfo.Ip_address, MyComputerInfo.HostName ... ", ex)
                        Common.UploadFile(Common.LogFileName, "GTPErrorLog")
                        Common.hasWCFAccess = False
                        Common.forceXMLMode = True
                        CheckBox_Mode.Checked = True
                        GoTo LabelXMLMode

                    End Try

                    id_server_transaction_reupload = id_server_transaction

                    Dim fs As FileStream
                    Try
                        log_file = "C:\GWSFTPlogs\" + username + "_" + id_server_transaction.ToString() + "_" + Market.Replace(" ", "_").Replace("'", "") + ".txt"
                        If Not File.Exists(log_file) Then
                            fs = File.Create(log_file)
                            fs.Close()
                        End If

                    Catch ex As Exception
                        log_file = "C:\GWSFTPlogs\" + username + "_" + id_server_transaction.ToString() + ".txt"
                        If Not File.Exists(log_file) Then
                            fs = File.Create(log_file)
                            fs.Close()
                        End If
                    End Try

                    'Update id_server_transaction to local tblTransaciton
                    GWSUploadTransaction.Update_Id_server_Transaction(Convert.ToInt32(id_transaction), Convert.ToInt32(id_server_transaction))

                    'C:\Tuan\DataUpload\2011-10-15-20-00-53-0021-0025-0006-0021-B.sqz
                    If id_server_transaction = "0" Then
                        MessageBox.Show("id_server_transaction is 0. Please try to upload again.")
                        Return
                    End If

                    For i = 0 To FileList.Length - 1
                        Dim file_name_only As String = ""
                        Try
                            file_name_only = FileList(i).Substring(FileList(i).LastIndexOf("\") + 1, FileList(i).Length - FileList(i).LastIndexOf("\") - 1)
                        Catch ex As Exception

                        End Try
                        Dim fileIn As New FileInfo(FileList(i))

                        isInserted = proxy.tblFTPFile(id_server_transaction, id_transaction, FileList(i), DateTime.Now, False, 0, file_name_only, fileIn.Length)
                        If Not isInserted Then


                            If Not CheckConnection("http://www.google.com") Then
                                Label_Info.Text = "Internet connection is lost. Check connection and press ""Upload"" button again"
                                MessageBox.Show("Transaction information is not complete! Try upload again")
                            Else
                                MessageBox.Show("Transaction information is not complete! Try upload again")
                            End If

                        End If

                    Next

                    If (id_server_transaction = 0) Then
                        If id_transaction <> 0 Then
                            GWSUploadTransaction.DeleteTransactionInfo(id_transaction)
                            If Not CheckConnection("http://www.google.com") Then
                                Label_Info.Text = "Internet connection is lost. Check connection and press Upload button again"
                            Else
                                MessageBox.Show("Transaction information is not complete! Try upload again")
                            End If
                        End If

                        Return
                    End If

                End If

            Catch ex As Exception

                Common.AppendTextFile(Common.LogFileName, "", ex)
                Common.UploadFile(Common.LogFileName, "GTPErrorLog")
                'MessageBox.Show(ex.ToString())
                If Not CheckConnection("http://www.google.com") Then
                    Label_Info.Text = "Internet connection is lost. Check connection and press Upload button again"
                    MessageBox.Show("Transaction information is not complete! Try upload again")
                    Return
                Else

                End If

                If id_transaction <> 0 Then
                    Common.SendEmailNotification("Reported: Transaction could not be created on the GAPP server due to web service error ", ex.ToString())
                    'MessageBox.Show("Transaction could not be created on the GAPP server due to web service error.  Please close GTP, re-open, and re-try data upload")

                    Common.hasWCFAccess = False
                    Common.forceXMLMode = True
                    CheckBox_Mode.Checked = True

                    'GWSUploadTransaction.DeleteTransactionInfo(id_transaction)
                    'Return

                End If
                MessageBox.Show("Error occurred! Transaction has been cancelled")
                'Return
            End Try

LabelXMLMode:


            If Not Common.hasWCFAccess Or Common.forceXMLMode Then


                'Create XMl File
                Dim xmlObj = New FTPXML()
                xmlObj.campaign = ComboBox_Drive.Text
                xmlObj.TransactionGID = Guid.NewGuid()
                TransactionGID = xmlObj.TransactionGID
                xmlObj.FTPUserName = TextBox_UserName.Text.Trim()
                'xmlObj.FTPPassword = Common.EncryptString(TextBox_Password.Text.Trim())

                xmlObj.FTPPassword = Common.AES_Encrypt(TextBox_Password.Text.Trim(), "TeamREMIS$MSR@mat")

                xmlObj.date_started = Common.GetCurrentEasternTime()
                xmlObj.DBName = DBName
                xmlObj.ftp_url = GWSUploadTransaction.ftp_url
                xmlObj.host_name = MyComputerInfo.HostName + "|Ver:" + VersionNumber
                xmlObj.id_client_transaction = Convert.ToInt32(id_transaction)
                xmlObj.ip_address = MyComputerInfo.Ip_address
                xmlObj.isCompleted = False
                xmlObj.market_name = Market
                xmlObj.transaction_status = 0
                xmlObj.transaction_type = 0
                xmlObj.username = username
                xmlObj.Project = ComboBox_Project.Text
                xmlObj.totalinputbytes = totalinputbytes
                xmlObj.id_market = id_market

                xmlObj.XMLFileName = AppDomain.CurrentDomain.BaseDirectory + "xml\" + DBName + "_" + xmlObj.TransactionGID.ToString() + ".xml"

                DestinationFolder = xmlObj.TransactionGID.ToString() + "-" + Market.Replace(",", " ")
                Common.RemoteFTPFolder = DestinationFolder

                xmlObj.FTPFolder = DestinationFolder

                If Not Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "xml\") Then
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "xml\")
                End If
                Common.currentXMLFile = xmlObj.XMLFileName
                xmlObj.CreateXMLFile()


                For i = 0 To FileList.Length - 1
                    Dim fileObj As New FTPFileXML()
                    Dim file_name_only As String = ""
                    Try
                        file_name_only = FileList(i).Substring(FileList(i).LastIndexOf("\") + 1, FileList(i).Length - FileList(i).LastIndexOf("\") - 1)
                    Catch ex As Exception

                    End Try

                    Dim fileIn As New FileInfo(FileList(i))


                    fileObj.XMLFileName = xmlObj.XMLFileName
                    fileObj.file_name = FileList(i)
                    fileObj.file_name_only = file_name_only
                    fileObj.file_status = 0
                    fileObj.FileGID = Guid.NewGuid()
                    fileObj.is_finished = False
                    fileObj.date_created = Common.GetCurrentEasternTime()
                    fileObj.file_status = 0
                    fileObj.id_client_transaction = Convert.ToInt32(id_transaction)
                    fileObj.bytestrnsfrd = fileIn.Length
                    fileObj.isScanner = False
                    fileObj.isAsideOrphan = False
                    fileObj.InsertFile()
                Next

                'Upload File to FTP server

                Common.XMLFileName = xmlObj.XMLFileName

                File.Copy(xmlObj.XMLFileName, xmlObj.XMLFileName.Replace(".xml", "_backup.xml"), True)

                UploadFile(xmlObj.XMLFileName, Common.currentXMLFolder)
                'Update to local database
                GWSUploadTransaction.UpdateXMLMode(id_transaction, xmlObj.TransactionGID.ToString())
            End If

            id_transaction_finished = id_transaction
            id_server_transaction_finished = id_server_transaction

            'Insert File Info to server tblFileInfo            

            If count_error = 0 Then
                isFileInfoCompleted = True
            Else
                isFileInfoCompleted = False
            End If

            If Common.hasWCFAccess And Not Common.forceXMLMode Then
                DestinationFolder = id_server_transaction + "-" + Market.Replace(",", " ")
                Common.RemoteFTPFolder = DestinationFolder
            End If
        Else
            id_transaction = id_transaction_reupload
            id_server_transaction = id_server_transaction_reupload

            If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then

                InsertFileServer(id_transaction, id_server_transaction)
                Label_id_local_transaction.Text = id_transaction
                DestinationFolder = id_server_transaction_reupload.ToString + "-" + Market.Replace(",", " ")
                Common.RemoteFTPFolder = DestinationFolder
                log_file = "C:\GWSFTPlogs\" + username + "_" + id_server_transaction.ToString() + "_" + Market.Replace(" ", "_").Replace("'", "") + ".txt"
                If Button_Upload.Text = "Re-Upload" Then
                    'Update upload counter
                    'Dim update_counter_uri As String
                    Dim tt As Boolean
                    If is_network_lost Then
                        tt = proxy.updateTotalFailed(id_server_transaction, 0, 1)
                    Else
                        tt = proxy.updateTotalFailed(id_server_transaction, 1, 0)

                    End If
                End If

            Else
                DestinationFolder = TransactionGID.ToString() + "-" + Market.Replace(",", " ")
                Common.RemoteFTPFolder = DestinationFolder
                InsertFileServer(id_transaction, id_server_transaction)
                'Update XML file if there are more added file
            End If


        End If

        'Upload FileList() to FTP server
        Dim isExist As Boolean

        GWSFTP = New GWSFTP


        'GWSFTP.ftpusername = TextBox_UserName.Text.Trim + "%ase9"
        'GWSFTP.ftppassword = TextBox_Password.Text.Trim + "%ase9"

        GWSFTP.ftpusername = TextBox_UserName.Text.Trim
        GWSFTP.ftppassword = TextBox_Password.Text.Trim



        GWSFTP.ftphost = "ftp://" & TextBox_Server.Text.Trim + "/" + TextBox_UserName.Text.Trim + "_curUpload"

        GWSFTP.cnn = cnn
        ProgressBar_Upload.Minimum = 0
        ProgressBar_Upload.Maximum = 100
        ProgressBar_Upload.Step = 1
        If Not _isSFTP Then
            isExist = GWSFTP.CheckFolder(DestinationFolder)
            If Not isExist Then
                GWSFTP.GWSCreateFolder(DestinationFolder)
            End If
        Else
            If Button_Upload.Text <> "Re-Upload" Then
                Dim sFTPClass As New GWSSFTP()
                sFTPClass.CreateDirectory(TextBox_UserName.Text.Trim + "_curUpload/" + DestinationFolder)
            End If
        End If



        If Button_Upload.Text <> "Re-Upload" Then


            ProgressBar_Upload.Maximum = 100
            ProgressBar_Upload.PerformStep()

        Else

            count_uploaded = GetTotalUploaded()
            total_uploaded_file = count_uploaded

            Label_TotalFile.Text = (count_uploaded).ToString() + " file(s) uploaded"
            Try
                total_file_failed = GetTotalFailed()
                Label_TotalFailed.Text = "Total file(s) failed Upload: " + total_file_failed.ToString()

            Catch ex As Exception

            End Try

        End If


        Dim uploadCode As Integer = 0
        Dim upload_file_info As String = ""
        Dim count11 As Integer = 0
        count_uploaded_files = 0



        Dim count1 As Integer = -1
        Dim count2 As Integer = -1

        isFileList1Ending = False

        FTPURL = GWSFTP.ftphost + "/" + DestinationFolder
        If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then


            Dim tmpFolderName As String = GetFTPFolderNameXML()

            If tmpFolderName = "" Then
                FTPURL = GWSFTP.ftphost + "/" + Common.RemoteFTPFolder
            Else
                FTPURL = GWSFTP.ftphost + "/" + tmpFolderName
            End If


            If Button_Upload.Text = "Re-Upload" Then
                'Read XML File to get the Transaction Information
                Dim FTPElement As XmlNode
                Dim xmlDoc As New XmlDocument
                xmlDoc.Load(Common.XMLFileName)
                FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
                If is_network_lost Then
                    FTPElement.Attributes("number_of_run").Value = (1 + Convert.ToInt32(FTPElement.Attributes("number_of_run").Value)).ToString()
                Else
                    FTPElement.Attributes("number_of_app_fail").Value = (1 + Convert.ToInt32(FTPElement.Attributes("number_of_app_fail").Value)).ToString()
                End If

                xmlDoc.Save(Common.XMLFileName)

                For i = 0 To FileList.Length - 1
                    Dim fileObj As New FTPFileXML()
                    Dim file_name_only As String = ""
                    Try
                        file_name_only = FileList(i).Substring(FileList(i).LastIndexOf("\") + 1, FileList(i).Length - FileList(i).LastIndexOf("\") - 1)
                    Catch ex As Exception

                    End Try

                    Dim fileIn As New FileInfo(FileList(i))

                    fileObj.XMLFileName = Common.XMLFileName
                    fileObj.file_name = FileList(i)
                    fileObj.file_name_only = file_name_only
                    fileObj.file_status = 0
                    fileObj.FileGID = Guid.NewGuid()
                    fileObj.is_finished = False
                    fileObj.date_created = Common.GetCurrentEasternTime()
                    fileObj.file_status = 0
                    fileObj.id_client_transaction = Convert.ToInt32(id_transaction)
                    fileObj.bytestrnsfrd = fileIn.Length

                    fileObj.InsertFile()
                Next



                File.Copy(Common.currentXMLFile, Common.currentXMLFile.Replace(".xml", "_backup.xml"), True)
                UploadFile(Common.currentXMLFile, Common.currentXMLFolder)

            End If
        End If

        id_FTP_Transaction = id_server_transaction
        id_FTP_Client = id_transaction
        If id_transaction = 0 Then
            id_FTP_Client = id_transaction_reupload
        End If



        If Not BackgroundWorker1.IsBusy Then
            BackgroundWorker1.RunWorkerAsync()
        End If

        If Not BackgroundWorker2.IsBusy Then
            BackgroundWorker2.RunWorkerAsync()

        End If
        proxy.Close()

    End Sub


    Private Sub UploadFile(ByRef uploadCode As Integer, ByVal FileName As String, ByVal GWSFTP As GWSFTP, ByVal ftphost As String, ByVal id_transaction As String)
        Dim uploadRate As Integer = 0
    End Sub


    Private Sub DownloadFile()
        Dim myFtpWebRequest As FtpWebRequest
        Dim myFtpWebResponse As FtpWebResponse
        Dim myStreamWriter As StreamWriter
        Dim strURL As String = ""
        Dim strFTPUsername As String = "tuand"
        Dim strFTPPassword As String = "gws123"
        Dim strFTPURL As String = "192.168.1.117/"
        Dim strFTPFolder As String = ""
        Dim strFileName As String = "GWS.dll"
        strURL = "ftp://" & strFTPURL & strFTPFolder & strFileName
        myFtpWebRequest = DirectCast(WebRequest.Create(strURL), FtpWebRequest)
        myFtpWebRequest.KeepAlive = False
        myFtpWebRequest.Timeout = 20000
        myFtpWebRequest.UsePassive = True
        myFtpWebRequest.UseBinary = True
        myFtpWebRequest.Credentials = New NetworkCredential(strFTPUsername, strFTPPassword)
        myFtpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile
        myFtpWebResponse = myFtpWebRequest.GetResponse()
        Dim response As Stream = myFtpWebResponse.GetResponseStream
        Dim streamReader As New StreamReader(response)
        Dim streamWriter As New StreamWriter("D:\FTPUpload\GWS.dll")
        streamWriter.Write(streamReader.ReadToEnd)
        streamWriter.Flush()
    End Sub



    Private Sub Button_SelectFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_SelectFolder.Click
        'Directory.CreateDirectory("C:\Team Software\! Tests\GTP\181022 Long Test Path\SQC 256\")
        ListView_MissingFile.Items.Clear()

        _uploadedFiles = New List(Of String)

        For Each item In ListView_FileList.Items
            If item.ForeColor = Color.Green Then
                _uploadedFiles.Add(item.Text)
            End If
        Next
        selectedUploadFiles = New List(Of String)
        isSelectFile = True
        Dim repeatFile As Boolean = False
        'Dim selectedUploadFiles As New List(Of String)
        Dim myFiles1 As New OpenFileDialog
        'Try
        myFiles1.Filter = "Select Files to upload  (*.sqz,*.mf,*.log,*.wnd,*.wnu,*.wnl,*.trp,*.txt,*.nmf/*.gpx,*.pcapzip,*.sqc,*.jpg,*.jpeg,*.jpe,*.png,*.bmp,*.gif,*.tif,*.mpo,*.heic,*.heif,*.heiv,*.avi,*.flv,*.wmv,*.mov,*.mp4)" &
                                                  "|*.sqz;*.mf;*.log;*.wnd;*.wnu;*.wnl;*.trp;*.txt;*.nmf;*.gpx;*.pcapzip;*.sqc;*.jpg;*.jpeg;*.jpe,;*.png;*.bmp;*.gif;*.tif;*.mpo;*.heic;*.heif;*.heiv;*.avi;*.flv;*.wmv;*.mov;*.mp4" &
                                                  "|(*.sqz)|*.sqz|(*.mf)|*.mf|(*.log)|*.log|(*.wnd)|*.wnd|(*.wnu)|*.wnu|(*.wnl)|*.wnl|(*.trp)|*.trp|(*.txt)|*.txt|(*.nmf/*.gpx)|*.nmf;*.gpx?|(*.pcapzip)|*.pcapzip|(*.sqc)|*.sqc" &
                                                  "|(*.jpg)|*.jpg|(*.jpeg)|*.jpeg|(*.jpe)|*.jpe|(*.png)|*.png|(*.bmp)|*.bmp|(*.gif)|*.gif|(*.tif)|*.tif|(*.mpo)|*.mpo|(*.heic)|*.heic|(*.heif)|*.heif|(*.heiv)|*.heiv|(*.avi)|*.avi|(*.flv)|*.flv|(*.wmv)|*.wmv|(*.mov)|*.mov|(*.mp4)|*.mp4"
        'myFiles1.DefaultExt = "csv"
        'b.	JPG, JPEG, JPE, PNG, BMP, GIF, TIF, TIFF, MPO, HEIC, HEIF, HEIV, AVI, FLV, WMV, MOV, MP4
        myFiles1.Multiselect = True
        myFiles1.Title = "Select File to upload "
        myFiles1.RestoreDirectory = True 'mod 7/6/07

        If (myFiles1.ShowDialog() = DialogResult.OK) Then
            Dim FolderName As String = myFiles1.FileNames(0).Substring(0, myFiles1.FileNames(0).LastIndexOf("\"))
            If Not Common.isFolderWritable(FolderName) Then
                MessageBox.Show("Files cannot be uploaded from a read-only location")
                Return
            End If

            Dim tmpFileName As String
            For Each tmpFileName In myFiles1.FileNames
                'Check if File is readonly then change it
                Try
                    Dim oFileInfo As New System.IO.FileInfo(tmpFileName)
                    If (oFileInfo.Attributes And System.IO.FileAttributes.ReadOnly) > 0 Then
                        oFileInfo.Attributes = oFileInfo.Attributes Xor System.IO.FileAttributes.ReadOnly
                    End If
                Catch ex As Exception

                End Try
            Next
            Array.Sort(myFiles1.FileNames)
            Dim i As Integer

            'selectedUploadFiles = New List(Of String)

            selectedUploadFilesExist = New List(Of String)
            Dim FileInfo As FileInfo


            selectedUploadFiles.AddRange(myFiles1.FileNames.ToList())

            If ListView_FileList.Items.Count > 0 Then
                Dim lvi As New ListViewItem
                For Each lvi In ListView_FileList.Items
                    selectedUploadFilesExist.Add(lvi.Text)
                Next
            Else

            End If



        End If
        'Catch ee As Exception

        'End Try

        If selectedUploadFiles.Count = 0 Then
            Return
        End If

        For i = 0 To selectedUploadFiles.Count - 1
            ReDim Preserve tmpSelectedUploadFiles(i)
            tmpSelectedUploadFiles(i) = selectedUploadFiles(i)
        Next

        Dim totalSize As Double = 0
        Dim isSQCExist As Boolean = False
        For i = 0 To tmpSelectedUploadFiles.Count - 1
            If tmpSelectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Then
                Dim tmpFileINfor As New FileInfo(tmpSelectedUploadFiles(i))
                totalSize = totalSize + tmpFileINfor.Length / 1024 / 1024 / 1024
            End If
        Next

        totalSize = totalSize + 1
        'totalSize = 611

        For i = 0 To tmpSelectedUploadFiles.Count - 1
            If tmpSelectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Then

                Dim DirName As String = tmpSelectedUploadFiles(i).Substring(0, 1) + ":\"

                Dim driveInfos As DriveInfo() = System.IO.DriveInfo.GetDrives()

                For Each driveInfo In driveInfos
                    If driveInfo.Name = DirName Then
                        Dim freeSpace As Double = driveInfo.AvailableFreeSpace / 1024 / 1024 / 1024
                        ' freeSpace = 648.668
                        If freeSpace < totalSize Then
                            MessageBox.Show("This application will automatically unzip all SQZ files from the selected SQC files. This process requires " + Math.Round(totalSize, 1).ToString() + " GB free space.This system only has " + Math.Round(freeSpace, 1).ToString() + " GB free space available. Please create free space before continuing.")
                            Return
                        End If
                    End If

                Next
            End If
        Next

        BackgroundWorker_Unzip.RunWorkerAsync()

    End Sub

    Private Sub CheckBox_isAnonymous_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_SaveCredential.CheckedChanged

    End Sub

    Private Sub CreateTransInfo(ByVal transaction_type As Boolean, ByVal id_market As Integer, ByVal username As String, ByVal FileList() As String, ByVal ftp_url As String)
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr

        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "INSERT INTO tblTransactionInfo(transaction_type,username,id_market,ftp_url) values (" & transaction_type & ",'" & username & "'," & id_market & ")"
        cnn.Open()
        cmd.ExecuteNonQuery()
        cmd.Dispose()

        'Get id_transaction
        cmd.CommandText = "SELECT TOP 1 id_transaction from tblTransactionInfo ORDER BY id_transaction DESC"
        cmd.Connection = cnn
        Dim dr As OleDbDataReader
        dr = cmd.ExecuteReader()
        Dim id_transaction As Integer
        If dr.HasRows Then
            While dr.Read()
                id_transaction = Convert.ToInt32(dr("id_transaction"))
            End While
            dr.Close()
            cmd.Dispose()
        End If

        'Insert file list to tblFileInfo
        For i = 0 To FileList.Length - 1
            cmd.CommandText = "INSERT INTO tblFileInfo(id_transaction,file_name) values (" & id_transaction.ToString() & ",'" & FileList(i) & "')"
            cmd.Connection = cnn
            cmd.ExecuteNonQuery()
            cmd.Dispose()
        Next

        cnn.Close()
    End Sub


    Private Sub Populate_MarketList()
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "SELECT id_market,name_market FROM tblMarket ORDER BY name_market "
        cnn.Open()
        dr = cmd.ExecuteReader()



        While (dr.Read())
            ComboBox_Market.Items.Add(dr("name_market"))
        End While

        dr.Close()
        cmd.Dispose()
        cnn.Close()


        ComboBox_Market.AutoCompleteMode = AutoCompleteMode.SuggestAppend

        ComboBox_Market.AutoCompleteSource = AutoCompleteSource.ListItems

    End Sub

    Private Sub RememberFTPCredentials()
        Dim ftpserver As String = TextBox_Server.Text.Trim
        Dim ftpusername As String = TextBox_UserName.Text.Trim
        Dim ftppassword As String = TextBox_Password.Text.Trim

        Dim encrypted_ftppassword As String = ""

        If ftppassword <> "" Then
            encrypted_ftppassword = EncryptionClass.Class1.Encrypt(ftppassword)
        End If
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "DELETE FROM tblFTpServer "
        cnn.Open()
        cmd.ExecuteNonQuery()
        If CheckBox_SaveCredential.Checked Then
            'Insert new credentials        
            cmd.CommandText = "INSERT INTO tblFTPServer([ftpserver],[ftpusername],[ftppassword]) VALUES ('" & ftpserver & "','" & ftpusername & "','" & encrypted_ftppassword & "')"
            cmd.ExecuteNonQuery()
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Label_SQC.Text = ""

        If Common.hasWCFAccess And Not Common.forceXMLMode Then
            CreateMarketXML()
            CreateClientXML()
            CreateCampaignXML()
            CreateProjectXML()
        End If

        Common.GetFileGroupConfig()


        CheckBox_EnableSFTP.Visible = False

        If Common.REMIS_username.ToUpper = "TEAMREMIS" Then
            CheckBox_EnableSFTP.Visible = True
        End If

        RadioButton_SQZ.Enabled = False
        RadioButton_SQZ_PCAP.Enabled = False
        GroupBox_PCAP_Options.Enabled = False

        CheckBox_NewMarket.Visible = False
        Label_OtherFiles.Text = ""
        Label_TotalFailed.Text = ""
        Label_CurrentFile.Text = ""
        Label_CurrentFile1.Text = ""
        Label_UploadSpeed.Text = ""
        Label_UploadSpeed1.Text = ""

        'TextBox_FTPSessionLog.Height = 283
        Button_DeleteUploadedFiles.Visible = False
        GetClient()
        If (Common.currentIDLocalTransaction = "") Then
            Dim count As Integer = Common.GetTotalPendingTransactions()
            If count >= 2 Then
                Dim pendingForm As New frmPendingTransaction()
                pendingForm.Show()
                Me.Close()
            End If
        End If

        If Common.REMIS_username.ToUpper() = "TeamREMIS".ToUpper() Then
            CheckBox_Mode.Visible = True
        Else
            CheckBox_Mode.Visible = False
        End If

        If Common.forceXMLMode Then
            Label_Mode.Visible = False
            Label_Mode.Visible = True
            Label_Mode.Text = "XML Mode is enable"
        Else
            Label_Mode.Visible = False
        End If

        If Not Common.hasWCFAccess Then
            CheckBox_Duplicate.Checked = False
            CheckBox_Duplicate.Enabled = False
            CheckBox_Mode.Checked = True
        Else
            CheckBox_Mode.Checked = False
            'CheckBox_Duplicate.Checked = True
            CheckBox_Duplicate.Enabled = True
        End If
        ComboBox_Market.Enabled = False
        'DeleteButton_ToolTip = New ToolTip
        'DeleteButton_ToolTip.SetToolTip(Button_DeleteUploadedFiles, "Please select files to delete. If you want to delete the whole transaction, select all the files ")
        ToolTip_Delete.SetToolTip(Button_DeleteUploadedFiles, "Please select files to delete. If you want to delete the whole transaction, select all the files ")
        ToolTip_CLose.SetToolTip(Button_CloseTransaction, "Close the transaction")


        'ToolTip_CloseButton = New ToolTip
        'ToolTip_CloseButton.SetToolTip(Button_CloseTransaction, "Close the transaction")
        GetUserName()

        ComboBox_FileType.SelectedText = ".sqz, .mf, .log, .wnd, .txt and .wnu"
        ComboBox_FileType.SelectedIndex = 0
        total_file_failed = 0
        'Label_Version.Text = "Version:" + VersionNumber
        ComboBox_FileType.SelectedItem = ".sqz and .mf and .log"
        'GetMarketList(2)
        GetCampaign()

        TextBox_UserName.Text = Common.REMIS_username
        TextBox_Password.Text = Common.REMIS_pass

        If TextBox_Server.Text = "" Then
            TextBox_Server.Text = "upload.mygws.com"
        End If
        Timer1.Enabled = False
        Timer2.Enabled = True

        'Dim link As LinkLabel.Link = New LinkLabel.Link()
        'link.LinkData = "http://upload.mygws.com/GTP"
        'LinkLabel_ReportIssue.Links.Add(link)

    End Sub


    Private Sub GetUserName()


        For i = 1 To 9
            TextBox_UserName.Items.Add("TEAM0" + i.ToString())
        Next

        For i = 10 To 1000
            TextBox_UserName.Items.Add("TEAM" + i.ToString())
        Next

        TextBox_UserName.Items.Add("TEAMREMIS")


        'Dim proxy As GTPServiceClient
        'proxy = New GTPServiceClient()
        'Dim i As Integer
        'Dim USerList As _AspNetUser()
        'USerList = proxy.GetUserAccount(1)
        'If (USerList.Count > 0) Then
        '    For i = 0 To USerList.Count - 1
        '        TextBox_UserName.Items.Add(USerList(i).username.ToUpper())
        '    Next
        'End If

        'For i = 44 To 99
        '    TextBox_UserName.Items.Add("TEAM" + i.ToString())
        'Next


        'proxy.Close()
    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Button_DeleteUploadedFiles.Visible = True
        Button_DeleteUploadedFiles.Enabled = True
        Button_CheckInternet.Visible = True
        Label_InternetStatus.Visible = True
        ResumeUpload("Timer1")
    End Sub

    Private Sub saveFolderToDelete()

        Try
            Dim GWSFTP As New GWSFTP
            Dim cnn As OleDbConnection
            cnn = New OleDbConnection
            cnn.ConnectionString = cnnstr
            cnn.Open()
            Dim cmd As New OleDbCommand
            cmd.Connection = cnn
            Dim folderToDelete As String = ""
            For Each tmpStr In _folderListToDelete
                folderToDelete += tmpStr + ";"
            Next
            cmd.CommandText = "UPDATE tblTransactionInfo SET folderToDelete = '" + folderToDelete + "' WHERE id_transaction = " + Common.currentIDLocalTransaction

            cmd.ExecuteNonQuery()
            cmd.Dispose()
            cnn.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub


    Private Sub InsertFileServer(ByVal id_transaction As String, ByVal id_server_transaction As String)
        Dim item As ListViewItem

        Dim proxy As GTPServiceClient = Nothing

        If Common.hasWCFAccess And Not Common.forceXMLMode Then
            proxy = New GTPServiceClient()
        End If


        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr
        cnn.Open()

        For Each item In ListView_FileList.Items
            If item.ForeColor <> Color.Green Then
                If Not reuploadFileList.Contains(item.Text) Then
                    Try
                        Dim file_name_only As String = ""
                        Try
                            file_name_only = item.Text.Substring(item.Text.LastIndexOf("\") + 1, item.Text.Length - item.Text.LastIndexOf("\") - 1)
                        Catch ex As Exception

                        End Try

                        Dim fileIn As New FileInfo(item.Text)

                        If Common.hasWCFAccess And Not Common.forceXMLMode Then
                            proxy.tblFTPFile(id_server_transaction, id_transaction, item.Text, DateTime.Now, False, 0, file_name_only, fileIn.Length)
                        End If

                        Dim cmd As OleDbCommand
                        cmd = New OleDbCommand

                        cmd.CommandText = "INSERT INTO tblFileInfo(id_transaction,file_name) values (" & id_transaction.ToString() & ",'" & item.Text.Replace("'", "''") & "')"
                        cmd.Connection = cnn
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()

                    Catch ex As Exception

                    End Try

                End If
            End If
        Next

        cnn.Close()

        If Common.hasWCFAccess And Not Common.forceXMLMode Then
            proxy.Close()
        End If
    End Sub



    Private Sub ResumeUpload(ByVal TimerName As String)
        reuploadFileList = New List(Of String)
        Common.FTPUserName = TextBox_UserName.Text.Trim()
        Common.FTPPass = TextBox_Password.Text.Trim()

        ComboBox_Market.Enabled = True

        Dim totalPending As Integer = 0
        Dim removedCount As Integer = 0
        Dim proxy As GTPServiceClient
        proxy = New GTPServiceClient()
        Common.AssignCredentials(proxy)
        Dim request_url As String = ""
        Dim FileList() As String
        Dim file_info As String = ""
        Dim response As String


        'Label_TotalFile.Text = "Total files: " & ListView_FileList.Items.Count.ToString

        Dim GWSFTP As New GWSFTP
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr

        If GWSFTP.CheckConnection("http://www.google.com") Then

            'GWSFTP.ftpusername = TextBox_UserName.Text.Trim + "%ase9"
            'GWSFTP.ftppassword = TextBox_Password.Text.Trim + "%ase9"
            If Label_Info.Text.Contains("Internet connection has been lost") Then
                Label_Info.Text = "Internet has re-connected. Upload process will resume automatically !"
                Button_CheckInternet.Visible = False
                Label_InternetStatus.Visible = False
                Label_InternetStatus.Text = "No Internet Connection"
                Button_DeleteUploadedFiles.Enabled = False
                If TimerName = "Timer1" Then
                    Timer1.Enabled = False
                End If
                Button_Upload.Text = "Re-Upload"
            End If


            GWSFTP.ftpusername = TextBox_UserName.Text.Trim
            GWSFTP.ftppassword = TextBox_Password.Text.Trim
            GWSFTP.ftphost = "ftp://" & TextBox_Server.Text.Trim + "/" + TextBox_UserName.Text.Trim + "_curUpload"
            GWSFTP.cnn = cnn
            'GWSFTP.GWSResumeUpload(cnn)

            Dim cmd As OleDbCommand
            Dim dr As OleDbDataReader
            Dim id_transaction As Integer = 0
            Dim ftp_url As String = ""
            Dim i As Integer



            cmd = New OleDbCommand
            cmd.Connection = cnn

            Dim FileRemaining() As String
            Dim FileUploaded() As String
            Dim FileFailed() As String

            If cnn.State <> ConnectionState.Open Then
                cnn.Open()
            End If
            If (Common.currentIDLocalTransaction = "") Then
                cmd.CommandText = "SELECT TOP 1 * FROM tblTransactionInfo WHERE status_transaction<> 2 and isCompleted=0 "
            Else
                cmd.CommandText = "SELECT * FROM tblTransactionInfo WHERE status_transaction<> 2 and isCompleted=0 and id_transaction = " + Common.currentIDLocalTransaction
            End If




            is_network_lost = False
            dr = cmd.ExecuteReader()

            If dr.HasRows Then
                While dr.Read()

                    If File.Exists(Common.rememberCheckBox) Then
                        Dim fileReader As System.IO.StreamReader
                        fileReader =
                        My.Computer.FileSystem.OpenTextFileReader(Common.rememberCheckBox)
                        Dim stringReader As String
                        stringReader = fileReader.ReadLine()
                        If stringReader = "0" Then
                            CheckBox_Duplicate.Checked = False
                        Else
                            CheckBox_Duplicate.Checked = True
                        End If
                        fileReader.Close()
                    End If

                    id_transaction = Convert.ToInt32(dr("id_transaction"))

                    'Get PSC tmp folder to delete
                    Try
                        If Not IsNothing(dr("folderToDelete")) Then
                            Dim folderToDeleteStr = Convert.ToString(dr("folderToDelete"))
                            _folderListToDelete = New List(Of String)
                            Dim strArr As String() = folderToDeleteStr.Split(";")
                            For Each tmpFolder In strArr
                                If tmpFolder.Trim <> "" Then
                                    _folderListToDelete.Add(tmpFolder)
                                End If
                            Next
                        End If

                    Catch ex As Exception
                        _folderListToDelete = New List(Of String)
                    End Try


                    Common.currentIDLocalTransaction = id_transaction.ToString()

                    Label_id_local_transaction.Text = id_transaction.ToString()
                    If Not IsDBNull(dr("id_server_transaction")) Then
                        id_server_transaction_reupload = Convert.ToInt32(dr("id_server_transaction"))

                    End If


                    Common.isXMLMode = Convert.ToBoolean(dr("isXMLMode"))
                    If Common.isXMLMode Then

                        If Not IsNothing(dr("TransactionGID")) Then
                            TransactionGID = Guid.Parse(dr("TransactionGID"))
                        End If
                        Common.forceXMLMode = True
                        Dim tmpMarket = Convert.ToString(dr("market_name"))
                        Dim tmpDBName As String = "T" + TextBox_UserName.Text.Substring(4, TextBox_UserName.Text.Length - 4) + "_" + tmpMarket.Replace(" ", "_")

                        CheckBox_Mode.Checked = True
                        Common.currentXMLFile = AppDomain.CurrentDomain.BaseDirectory + "xml\" + tmpDBName + "_" + TransactionGID.ToString() + ".xml"
                        XMLFileName = Common.currentXMLFile

                    Else
                        CheckBox_Mode.Checked = False
                    End If
                    is_network_lost = Convert.ToBoolean(dr("is_network_lost"))
                    If Not Common.isXMLMode Then
                        ftp_url = GWSFTP.ftphost + "/" + id_server_transaction_reupload.ToString + "-" + Convert.ToString(dr("market_name"))
                    Else
                        ftp_url = GWSFTP.ftphost + "/" + TransactionGID.ToString() + "-" + Convert.ToString(dr("market_name"))
                    End If


                    If TimerName <> "Timer1" Then

                        ComboBox_Drive.DropDownStyle = ComboBoxStyle.DropDown
                        ComboBox_Market.DropDownStyle = ComboBoxStyle.DropDown
                        ComboBox_Project.DropDownStyle = ComboBoxStyle.DropDown
                        ComboBox_Drive.Text = Convert.ToString(dr("Campaign"))
                        ComboBox_Project.Text = Convert.ToString(dr("Project"))
                        Dim tmpMarket As String = Convert.ToString(dr("market_name"))
                        ComboBox_Market.Text = tmpMarket
                        currentMarket = ComboBox_Market.Text
                        ComboBox_Drive.Enabled = False
                        ComboBox_Market.Enabled = False
                        ComboBox_Project.Enabled = False
                    End If
                End While
                dr.Close()


                If id_transaction <> 0 Then
                    'Update is_network_lost to False
                    cmd = New OleDbCommand
                    cmd.Connection = cnn
                    If cnn.State <> ConnectionState.Open Then
                        cnn.Open()
                    End If
                    cmd.CommandText = "UPDATE tblTransactionInfo SET [is_network_lost] = FALSE WHERE id_transaction=" & id_transaction
                    cmd.ExecuteNonQuery()

                End If
            Else

                Dim tmpFileList As New List(Of String)
                Dim itemList As New List(Of ListViewItem)
                For Each item In ListView_FileList.Items
                    tmpFileList.Add(item.Text)
                Next

                If File.Exists(Common.rememberCheckBox) Then
                    Dim file1 As System.IO.StreamWriter
                    file1 = My.Computer.FileSystem.OpenTextFileWriter(Common.rememberCheckBox, False)
                    file1.WriteLine("1")
                    file1.Close()
                    If CheckBox_Duplicate.Checked Then

                        If (Common.hasWCFAccess) Then

                            CheckDuplicate(tmpFileList)
                        End If

                    End If
                    CheckDuplicateLocal(tmpFileList)
                End If
            End If



            If Not Common.isXMLMode Then
                If ListView_FileList.Items.Count > 0 Then
                    'Re insert File Info if not completed
                    Dim total_file_server = 0
                    total_file_server = proxy.countFile(id_server_transaction_reupload, "all")
                    If total_file_server < ListView_FileList.Items.Count Then

                        If Not isFileInfoCompleted Then
                            Dim count_file1 As Integer = -1

                            Dim TotalFile As Integer = ListView_FileList.Items.Count
                            For i = 0 To TotalFile - 1
                                count_file1 += 1
                                ReDim Preserve FileList(count_file1)
                                FileList(count_file1) = ListView_FileList.Items.Item(i).Text
                            Next

                            Dim count_file As Integer = 0
                            Dim count_error As Integer = 0

                            'Delete FileInfo in tblFTPFile first
                            Dim isSuccess = False
                            isSuccess = proxy.deletetblFTPFile(id_server_transaction_reupload)

                            If isSuccess Then
                                For i = 0 To FileList.Length - 1
                                    count_file += 1
                                    Dim file_name_only As String = ""
                                    Try
                                        file_name_only = FileList(i).Substring(FileList(i).LastIndexOf("\") + 1, FileList(i).Length - FileList(i).LastIndexOf("\") - 1)
                                    Catch ex As Exception

                                    End Try

                                    Dim fileIn As New FileInfo(FileList(i))

                                    isSuccess = proxy.tblFTPFile(id_server_transaction_reupload, id_transaction_reupload, FileList(i), DateTime.Now, False, 0, file_name_only, fileIn.Length)

                                    'If response = "error" Then
                                    If isSuccess Then
                                        count_error += 1
                                    End If
                                    file_info = ""
                                    'End If
                                Next
                                If count_error = 0 Then
                                    isFileInfoCompleted = True
                                Else
                                    isFileInfoCompleted = False
                                End If
                            End If
                        End If
                    End If
                End If

                If id_transaction = 0 Then
                    Exit Sub
                End If

                'Check if transaction_status is>3 on server

                Dim trans As _FTPTransaction = proxy.GetTransaction(id_server_transaction_reupload)
                ComboBox_Project.Enabled = True
                If Not IsNothing(trans) Then
                    Common.isResumeUpload = True
                    transaction_status_server = trans.transaction_status
                    ComboBox_Project.Text = trans.Project
                End If

                If transaction_status_server >= 3 Then

                End If
                ComboBox_Project.Enabled = False
            End If


            'Get remaining files
            cmd = New OleDbCommand()
            cmd.Connection = cnn
            cmd.CommandText = "SELECT * FROM tblFileInfo WHERE [file_status]<>2 AND [id_transaction] =" & id_transaction & " ORDER BY is_finished DESC, id_file "

            'cnn.Open()

            dr = cmd.ExecuteReader()
            Dim count As Integer = -1
            Dim count1 As Integer = -1
            Dim count_failed As Integer = -1

            Dim uploadStatus As Integer = 0
            If dr.HasRows Then
                id_transaction_reupload = id_transaction
                If id_transaction_reupload.ToString <> "" Then
                    Button_Upload.Text = "Re-Upload"
                End If
                While dr.Read()
                    If Convert.ToBoolean(dr("is_delete_disconnected")) = True Then




                        proxy.UpdatetbFTPFile(id_server_transaction_reupload, Convert.ToString(dr("file_name")), 2)

                        'Delete from FTP server
                        GWSFTP.ftphost = "ftp://" & TextBox_Server.Text.Trim + "/" + TextBox_UserName.Text.Trim + "_curUpload"
                        GWSFTP.GWSDeleteOneFile(Convert.ToString(dr("file_name")), GWSFTP.ftphost + "/" + id_server_transaction_reupload.ToString + "-" + ComboBox_Market.Text.Replace(",", ""))
                    Else
                        If Convert.ToBoolean(dr("is_finished")) = False Then

                            reuploadFileList.Add(Convert.ToString(dr("file_name")))

                            If Convert.ToInt32(dr("file_status")) = -1 Then
                                count_failed += 1
                                ReDim Preserve FileFailed(count_failed)
                                FileFailed(count_failed) = dr("file_name")
                            Else
                                count += 1
                                ReDim Preserve FileRemaining(count)
                                FileRemaining(count) = dr("file_name")
                            End If

                        Else
                            count1 += 1
                            ReDim Preserve FileUploaded(count1)
                            FileUploaded(count1) = dr("file_name")

                        End If
                    End If
                End While

                Dim total_uploaded = 0

                If TimerName = "Timer2" Then

                    Dim FileInfo As FileInfo
                    'Display file on the ListVew

                    If transaction_status_server = 0 Then
                        If Not IsNothing(FileUploaded) Then
                            For i = 0 To FileUploaded.Length - 1
                                Dim lvi As New ListViewItem
                                ' First Column can be the listview item's Text  
                                lvi.Text = FileUploaded(i)
                                lvi.ForeColor = Color.Green
                                total_uploaded += 1
                                ListView_FileList.Items.Add(lvi)

                                If File.Exists(FileUploaded(i)) Then
                                    FileInfo = New FileInfo(FileUploaded(i))
                                    lvi.SubItems.Add(FileInfo.Length)
                                End If
                            Next


                            count_uploaded_files = total_uploaded
                        End If
                    End If

                    ListView_OtherFiles.Items.Clear()


                    If Not IsNothing(FileRemaining) Then
                        If FileRemaining.Length > 0 Then
                            For i = 0 To FileRemaining.Length - 1
                                Dim lvi As New ListViewItem
                                lvi.Text = FileRemaining(i)
                                FileInfo = New FileInfo(FileRemaining(i))

                                If File.Exists(FileRemaining(i)) Then
                                    ListView_FileList.Items.Add(lvi)
                                    lvi.SubItems.Add(FileInfo.Length)
                                Else
                                    ListView_OtherFiles.Items.Add(FileRemaining(i))
                                    TabControl1.SelectedIndex = 0
                                    Label_OtherFiles.Text = "Removed Files"
                                    removedCount += 1

                                    If Not Common.isXMLMode Then
                                        Dim status As Integer = proxy.GetFileStatus(id_server_transaction_reupload, FileRemaining(i))
                                        If Not (status = 1 Or status >= 3) Then
                                            proxy.UpdatetbFTPFile(id_server_transaction_reupload, FileRemaining(i), -5)
                                        End If
                                    End If

                                End If
                            Next
                        End If
                    End If


                    If Not IsNothing(FileFailed) Then
                        If FileFailed.Length > 0 Then
                            For i = 0 To FileFailed.Length - 1
                                Dim lvi As New ListViewItem
                                lvi.Text = FileFailed(i)
                                lvi.ForeColor = Color.Red

                                If File.Exists(FileFailed(i)) Then
                                    ListView_FileList.Items.Add(lvi)
                                    FileInfo = New FileInfo(FileFailed(i))
                                    lvi.SubItems.Add(FileInfo.Length)
                                Else
                                    ListView_OtherFiles.Items.Add(lvi)
                                    TabControl1.SelectedIndex = 0
                                    removedCount += 1
                                    If Not Common.isXMLMode Then

                                        Dim status As Integer = proxy.GetFileStatus(id_server_transaction_reupload, FileFailed(i))
                                        If Not (status = 1 Or (status >= 3)) Then
                                            proxy.UpdatetbFTPFile(id_server_transaction_reupload, FileFailed(i), -5)
                                        End If

                                    End If

                                End If

                            Next
                        End If
                    End If


                    ProgressBar_Upload.Visible = True
                    ProgressBar_Upload.Minimum = 0
                    ProgressBar_Upload.Step = 1
                    ProgressBar_Upload.Maximum = 100

                    Label_TotalFile.Text = (total_uploaded).ToString() + " file(s) uploaded"

                    If Not IsNothing(FileRemaining) Then
                        totalPending = totalPending + FileRemaining.Count()
                    End If
                    If Not IsNothing(FileFailed) Then
                        totalPending = totalPending + FileFailed.Count()
                    End If
                    If removedCount = totalPending Then
                        MessageBox.Show("All remaining files are removed. The transaction will be closed")
                        Button_Upload.Visible = False
                        Button_DeleteUploadedFiles.Visible = False
                        Button2.Visible = False
                        Button_CloseTransaction.Visible = True

                    Else
                        MessageBox.Show(" There are pending files that need to be re-uploaded. Please select action: Continue, Delete or Close Transaction")

                        Button_Upload.Visible = False
                        Button_DeleteUploadedFiles.Visible = False
                        Button2.Visible = False
                        Button_CloseTransaction.Visible = False

                    End If

                End If
                If TimerName = "Timer2" Then
                    GroupBox_Action.Visible = True
                    GroupBox_Action.Enabled = True
                    'TextBox_FTPSessionLog.Height = 225
                End If

                If transaction_status_server = 0 Then
                    If TimerName = "Timer2" Then

                        Button_Upload.Text = "Re-Upload"

                        RadioButton_Continue.Enabled = True
                        RadioButton_Delete.Enabled = True
                        RadioButton_Close.Enabled = True

                    End If

                Else
                    If TimerName = "Timer2" Then

                        RadioButton_Continue.Enabled = True
                        RadioButton_Delete.Enabled = False
                        RadioButton_Close.Enabled = True
                    End If


                End If

                If Button_Upload.Text <> "Re-Upload" Then
                    CheckBox_Duplicate.Checked = True
                Else
                    If File.Exists(Common.rememberCheckBox) Then
                        Dim fileReader As System.IO.StreamReader
                        fileReader =
                        My.Computer.FileSystem.OpenTextFileReader(Common.rememberCheckBox)
                        Dim stringReader As String
                        stringReader = fileReader.ReadLine()
                        If stringReader = "0" Then
                            CheckBox_Duplicate.Checked = False
                        Else
                            CheckBox_Duplicate.Checked = True
                        End If
                        fileReader.Close()
                        formLoad = False
                    End If
                End If



                Dim count11 As Integer = 0

                If TimerName <> "Timer2" Then

                    Dim iii = id_server_transaction_reupload
                    Dim jjj = id_transaction_reupload


                    Button_Upload_Do(0)
                    Exit Sub
                End If
            End If
            cmd.Dispose()
        End If
        id_server_transaction_finished = id_server_transaction_reupload
        id_transaction_finished = id_transaction_reupload
        Label_id_local_transaction.Text = id_transaction_finished

        proxy.Close()
        cnn.Close()

        If removedCount = totalPending And removedCount > 0 Then
            CloseTransaction()
        End If
        'Button_DeleteUploadedFiles.Visible = True
        'Button2.Visible = False
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        'When form is loaded
        Timer2.Enabled = False
        ResumeUpload("Timer2")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        ListView_MissingFile.Items.Clear()

        Dim item As ListViewItem
        item = New ListViewItem

        For Each item In ListView_FileList.SelectedItems

            ListView_FileList.Items.Remove(item)

        Next

        Label_TotalFile.Text = "Total file is " + ListView_FileList.Items.Count.ToString
        total_file_to_uplload = ListView_FileList.Items.Count

    End Sub


    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim HostName As String = ""
    '    Dim Ipaddress As String = ""
    '    Dim Macaddress As String = ""
    '    Common.GetIPAddress(HostName, Ipaddress, Macaddress)
    'End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_DeleteUploadedFiles.Click

        Dim isDeleteTransaction As Boolean = False
        Dim item As ListViewItem
        item = New ListViewItem


        If ListView_FileList.SelectedItems.Count = ListView_FileList.Items.Count Then
            isDeleteTransaction = True
        End If

        Dim Button = MessageBox.Show("Are you sure you want to delete ?", "Message", MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

        Dim GWSFTP As GWSFTP
        Dim id_server_transaction As String
        Dim FolderName As String = ""
        Dim id_local_transaction As String
        Dim transaction_status_xml As String = "0"



        Dim market_name As String = ComboBox_Market.Text
        If (CheckBox_NewMarket.Checked) Then
            market_name = ComboBox_PendingMarket.Text
        End If


        If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then

            Try
                FolderName = GetFTPFolderNameXML()
            Catch ex As Exception
                MessageBox.Show("Error, could not delete the file(s) !")
                Return
            End Try
        End If


        id_local_transaction = Label_id_local_transaction.Text


        Dim proxy As GTPServiceClient
        If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then

        Else
            proxy = New GTPServiceClient()
            Common.AssignCredentials(proxy)
            id_server_transaction = id_server_transaction_finished.ToString
            FolderName = id_server_transaction.ToString() + "-" + market_name.Replace(",", "")
        End If



        If Button = Windows.Forms.DialogResult.Yes Then

            'Label_Info.Text = "The transaction deletion is in process. PLEASE WAIT … "
            Dim cnn As OleDbConnection
            cnn = New OleDbConnection
            cnn.ConnectionString = cnnstr

            Dim isConnectionOn As Boolean = True
            If Not CheckConnection("http://www.google.com") Then
                isConnectionOn = False
            End If



            GWSFTP = New GWSFTP
            GWSFTP.ftpusername = TextBox_UserName.Text.Trim
            GWSFTP.ftppassword = TextBox_Password.Text.Trim



            If ListView_FileList.SelectedItems.Count = ListView_FileList.Items.Count Then
                isDeleteTransaction = True
            End If



            'Check if transaction_status on server

            Dim trans As _FTPTransaction

            If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                trans = proxy.GetTransaction(id_server_transaction)
            End If


            If isDeleteTransaction Then
                If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                    proxy.updateTotalUploaded(id_server_transaction, 0)
                    proxy.deleteTransaction(id_server_transaction)

                Else
                    'Update delete transaction


                End If
            End If

            Dim isDeleteAble As Boolean = False

            Dim lastFile As String = ""

            'Update xml file in XML mode
            Dim FTPElement As XmlNode
            Dim xmlDoc As New XmlDocument

            If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then

                xmlDoc.Load(XMLFileName)
                FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
                If Not IsNothing(FTPElement) Then
                    If isDeleteTransaction Then
                        FTPElement.Attributes("transaction_status").Value = "2"

                        'GetTotalUploaded()
                    End If
                    'FTPElement.Attributes("total_file_uploaded").Value = GetTotalUploaded().ToString()
                End If

                Dim FileNodes As XmlNodeList
                Dim node As XmlNode
                FileNodes = xmlDoc.SelectNodes("/tblFTPTransaction/tblFTPFile")

                If Not IsNothing(FileNodes) Then
                    For Each node In FileNodes
                        For Each item In ListView_FileList.SelectedItems
                            If item.Text.Contains(node.Attributes("file_name_only").Value) Then
                                node.Attributes("file_status").Value = "2"
                            End If
                        Next
                    Next
                End If

            End If


            If Not IsNothing(trans) Or (Common.forceXMLMode Or Common.isXMLMode) Then

                If Not IsNothing(trans) Then
                    If (trans.transaction_status = 0 Or trans.transaction_status = 1) Then
                        isDeleteAble = True
                    End If
                End If

                If isDeleteAble Or (transaction_status_xml = "1" Or transaction_status_xml = "0") Then

                    For Each item In ListView_FileList.SelectedItems
                        Dim cmd As OleDbCommand
                        cmd = New OleDbCommand
                        'Try
                        cnn.Open()
                        cmd.Connection = cnn

                        If isConnectionOn Then
                            cmd.CommandText = "UPDATE tblFileInfo SET file_status = 2 WHERE id_transaction = " + id_local_transaction + " AND file_name='" + item.Text.Replace("'", "''") + "'"
                        Else
                            cmd.CommandText = "UPDATE tblFileInfo SET file_status = 2, is_delete_disconnected =1 WHERE id_transaction = " + id_local_transaction + " AND file_name='" + item.Text.Replace("'", "''") + "'"
                        End If

                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                        cnn.Close()

                        'Delete from server

                        'Check if connection is on
                        If isConnectionOn Then

                            If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                                If Not (Common.isXMLMode Or Common.forceXMLMode) Then
                                    proxy.UpdatetbFTPFile(id_server_transaction, item.Text, Convert.ToInt16(2))
                                End If

                            End If
                            GWSFTP.ftphost = "ftp://" & TextBox_Server.Text.Trim + "/" + TextBox_UserName.Text.Trim + "_curUpload"

                            If Not Common.CheckFTPDirectory(FolderName) Then
                                FolderName = GetFTPFolderNameXML()
                            End If
                            GWSFTP.GWSDeleteOneFile(item.Text, GWSFTP.ftphost + "/" + FolderName)


                            If Not isDeleteTransaction Then
                                UpdateTotalFile()
                                '                            lastFile = item.Text
                                ListView_FileList.Items.Remove(item)

                            End If

                        End If
                    Next


                    If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then
                        FTPElement.Attributes("total_file_uploaded").Value = GetTotalUploaded().ToString()
                        xmlDoc.Save(XMLFileName)
                        'Upload file to server
                        File.Copy(XMLFileName, XMLFileName.Replace(".xml", "_backup.xml"), True)
                        UploadFile(XMLFileName, Common.currentXMLFolder)
                    End If

                    'Update total uploaded files
                    If isDeleteTransaction Then
                        'ListView_FileList.Items.Add(lastFile)

                        Dim GWSTransaction As GWSTransaction
                        GWSTransaction = New GWSTransaction
                        GWSTransaction.cnn = cnn
                        GWSTransaction.DeleteTransactionInfo(id_local_transaction)

                        If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then

                            If Not Common.CheckFTPDirectory(FolderName) Then
                                FolderName = GetFTPFolderNameXML()
                            End If

                            Dim isNotEmpty As Boolean = True
                            Dim count As Integer = 0
                            While isNotEmpty
                                count += 1
                                If count > 10 Then
                                    Exit While
                                End If

                            End While
                        Else
                            Dim isNotEmpty As Boolean = True
                            Dim count As Integer = 0
                            While isNotEmpty
                                count += 1
                                If count > 10 Then
                                    Exit While
                                End If
                            End While
                        End If

                        ListView_FileList.Items.Clear()
                        MessageBox.Show("Transaction has been successfully deleted")
                        MessageBox.Show("Upload form will be closed. Please reopen it if starting a new upload.")
                        Me.Close()
                    Else

                        If Not (Common.isXMLMode Or Common.forceXMLMode) Then
                            If (Common.hasWCFAccess) Then
                                'proxy.updateTotalUploaded(id_server_transaction, GetTotalUploaded())
                            End If

                        End If

                    End If
                End If

            End If

            If isDeleteTransaction Then
                Dim GWSTransaction As GWSTransaction
                GWSTransaction = New GWSTransaction
                GWSTransaction.cnn = cnn

                GWSTransaction.DeleteTransactionInfo(id_local_transaction)

                ListView_FileList.Items.Clear()

                Me.Close()
                Return
            Else
                If Common.hasWCFAccess Then
                    Try
                        If Convert.ToInt32(id_server_transaction) > 0 Then
                            'proxy.updateTotalUploaded(id_server_transaction, GetTotalUploaded())

                        End If
                    Catch ex As Exception

                    End Try


                End If
                MessageBox.Show("Files have been deleted")
                Label_Info.Text = "Files have been deleted"

            End If


            If isDeleteAble Or (transaction_status_xml = "1" Or transaction_status_xml = "0") Then
                For Each item In ListView_FileList.SelectedItems
                    ListView_FileList.Items.Remove(item)
                    Dim cmd As OleDbCommand
                    cmd = New OleDbCommand
                    cnn.Open()
                    cmd.Connection = cnn
                    If isConnectionOn Then
                        cmd.CommandText = "UPDATE tblFileInfo SET file_status = 2 WHERE id_transaction = " + id_local_transaction + " AND file_name='" + item.Text + "'"
                    Else
                        cmd.CommandText = "UPDATE tblFileInfo SET file_status = 2, is_delete_disconnected =1 WHERE id_transaction = " + id_local_transaction + " AND file_name='" + item.Text + "'"
                    End If
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                    cnn.Close()
                    UpdateTotalFile()
                Next



                If isDeleteTransaction Then
                    Dim GWSTransaction As GWSTransaction
                    GWSTransaction = New GWSTransaction
                    GWSTransaction.cnn = cnn
                    GWSTransaction.DeleteTransactionInfo(id_local_transaction)
                    MessageBox.Show("Transaction has been successfully deleted")
                    MessageBox.Show("Upload form will be close. Please re-open it if you want to start new upload")
                    Me.Close()
                End If
            End If

        Else
            Return
        End If
        If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
            proxy.Close()
        End If

        Label_TotalFile.Text = GetTotalUploaded().ToString() + " file(s) uploaded of total " + ListView_FileList.Items.Count.ToString() + " file(s)"


    End Sub


    Private Function GetFTPFolderNameXML() As String
        Dim FolderName As String = ""
        'TREMIS_Benton_Harbor_MI_5f479722-128f-42a2-839a-2548f7fc65c4.xml
        'TREMIS_Benton_Harbor_MI_5f479722-128f-42a2-839a-2548f7fc65c4.xml
        Dim DBName As String = "T" + TextBox_UserName.Text.Substring(4, TextBox_UserName.Text.ToString().Length - 4) + "_" + ComboBox_Market.Text.Trim().Replace(" ", "_")
        Dim XMLFileName As String = DBName + "_" + TransactionGID.ToString() + ".xml"
        Dim isFileExist As Boolean
        Dim localFileName As String = AppDomain.CurrentDomain.BaseDirectory + "xml\" + XMLFileName
        isFileExist = Common.DownloadFile(localFileName, XMLFileName, Common.currentXMLFolder)
        Try
            If isFileExist Then
                'Read XML File to get the Transaction Information
                Dim FTPElement As XmlNode
                Dim xmlDoc As New XmlDocument
                xmlDoc.Load(localFileName)
                FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
                FolderName = FTPElement.Attributes("FTPFolder").Value
                File.Copy(localFileName, localFileName.Replace(".xml", "_backup.xml"), True)
            End If
        Catch ex As Exception
            If File.Exists(localFileName.Replace(".xml", "_backup.xml")) Then
                'Read XML File to get the Transaction Information
                Dim FTPElement As XmlNode
                Dim xmlDoc As New XmlDocument
                xmlDoc.Load(localFileName.Replace(".xml", "_backup.xml"))
                FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
                FolderName = FTPElement.Attributes("FTPFolder").Value
                File.Copy(localFileName.Replace(".xml", "_backup.xml"), localFileName, True)
            End If
        End Try

        Return FolderName

    End Function

    Private Sub createXMLFile()

    End Sub

    Private Sub GetMarketList(ByVal market_status)
        'ComboBox_Market.Items.Clear()

        If Common.hasWCFAccess And Not Common.forceXMLMode Then

            Dim proxy1 As GTPServiceClient
            proxy1 = New GTPServiceClient()

            Dim Markets As _MarketName()
            Dim i As Integer

            Markets = proxy1.GetMarket(market_status)


            ComboBox_Market.DataSource = Markets
            ComboBox_Market.DisplayMember = "name_market"
            ComboBox_Market.ValueMember = "id_market"

            If currentMarket <> "" Then
                ComboBox_Market.SelectedIndex = ComboBox_Market.FindString(currentMarket)
            Else
                ComboBox_Market.SelectedIndex = -1
            End If
            'If Markets.Count() > 0 Then
            '    For i = 0 To Markets.Count - 1
            '        ComboBox_Market.Items.Add(Markets(i).name_market.Replace(",", ""))
            '    Next

            'End If
            proxy1.Close()
        Else
            Dim xmlDoc = New XmlDocument()
            xmlDoc.Load("market.xml")
            EncryptDecrypt.Program.Decrypt(xmlDoc, "GWSsol")
            Dim MarketNodes As XmlNodeList
            MarketNodes = xmlDoc.SelectNodes("/MarketRoot/Market")
            Dim node As XmlNode
            Dim Markets As New List(Of _MarketName)

            If MarketNodes.Count > 0 Then
                For Each node In MarketNodes

                    Dim market_status_str As String = node.Attributes("market_status").Value
                    If market_status.ToString() = market_status_str Then
                        Dim m As New _MarketName
                        m.name_market = node.Attributes("Name").Value
                        m.id_market = Convert.ToInt32(node.Attributes("id_market").Value)
                        m.market_status = Convert.ToInt32(node.Attributes("market_status").Value)
                        Markets.Add(m)
                    End If


                    'Dim market_status_str As String = node.Attributes("market_status").Value
                    'If market_status.ToString() = market_status_str Then
                    '    ComboBox_Market.Items.Add(node.Attributes("Name").Value)
                    'End If
                Next

                If Markets.Count > 0 Then
                    ComboBox_Market.DataSource = Markets
                    ComboBox_Market.DisplayMember = "name_market"
                    ComboBox_Market.ValueMember = "id_market"

                    If currentMarket <> "" Then
                        ComboBox_Market.SelectedIndex = ComboBox_Market.FindString(currentMarket)
                    End If

                End If

            End If
        End If
    End Sub

    Private Sub GetAllMarket()
        ComboBox_Market.Items.Clear()
        Dim proxy1 As GTPServiceClient
        proxy1 = New GTPServiceClient()

        Dim Markets As _MarketName()
        Dim i As Integer

        Markets = proxy1.GetMarket(2)

        If Markets.Count() > 0 Then
            For i = 0 To Markets.Count - 1
                ComboBox_Market.Items.Add(Markets(i).name_market.Replace(",", ""))
            Next

        End If

        Markets = proxy1.GetMarket(3)

        If Markets.Count() > 0 Then
            For i = 0 To Markets.Count - 1
                ComboBox_Market.Items.Add(Markets(i).name_market.Replace(",", ""))
            Next

        End If
        proxy1.Close()

    End Sub

    Private Sub GetCampaign()

        If Common.hasWCFAccess And (Not Common.isXMLMode) Then
            Dim i As Integer
            Dim proxy As GTPServiceClient
            proxy = New GTPServiceClient()
            Common.AssignCredentials(proxy)
            Dim CampaignList As _Campaign() = proxy.GetCampaign()
            If CampaignList.Count > 0 Then
                For i = 0 To CampaignList.Count - 1
                    ComboBox_Drive.Items.Add(CampaignList(i).campaign_name)
                Next

            End If
            proxy.Close()

        Else
            Dim CurrentYear As Integer = DateTime.Now.Year - 2000

            ComboBox_Drive.Items.Add((CurrentYear - 1).ToString() + "D1")
            ComboBox_Drive.Items.Add((CurrentYear - 1).ToString() + "D2")
            ComboBox_Drive.Items.Add((CurrentYear - 1).ToString() + "D3")
            ComboBox_Drive.Items.Add((CurrentYear - 1).ToString() + "D4")


            ComboBox_Drive.Items.Add(CurrentYear.ToString() + "D1")
            ComboBox_Drive.Items.Add(CurrentYear.ToString() + "D2")
            ComboBox_Drive.Items.Add(CurrentYear.ToString() + "D3")
            ComboBox_Drive.Items.Add(CurrentYear.ToString() + "D4")

            ComboBox_Drive.Items.Add((CurrentYear + 1).ToString() + "D1")
            ComboBox_Drive.Items.Add((CurrentYear + 1).ToString() + "D2")
            ComboBox_Drive.Items.Add((CurrentYear + 1).ToString() + "D3")
            ComboBox_Drive.Items.Add((CurrentYear + 1).ToString() + "D4")

        End If

    End Sub


    Private Sub EnableNewMarket()
        If CheckBox_NewMarket.Checked Then

            Button_NewMarket.Visible = True
            'Get Pending market list
            'ComboBox_Market.Items.Clear()
            ComboBox_PendingMarket.Visible = True
            ComboBox_Market.Enabled = False


            If Common.hasWCFAccess Then
                Dim proxy As GTPServiceClient
                proxy = New GTPServiceClient()
                Common.AssignCredentials(proxy)
                Dim Markets As _MarketName()
                Dim i As Integer


                Markets = proxy.GetMarket(1)
                ComboBox_PendingMarket.DataSource = Markets
                ComboBox_PendingMarket.DisplayMember = "name_market"
                ComboBox_PendingMarket.ValueMember = "id_market"
                'If Markets.Count() > 0 Then
                '    For i = 0 To Markets.Count - 1
                '        ComboBox_PendingMarket.Items.Add(Markets(i).name_market)
                '    Next

                'End If
                proxy.Close()

                ComboBox_PendingMarket.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                ComboBox_PendingMarket.AutoCompleteSource = AutoCompleteSource.ListItems

            End If

        Else
            ComboBox_PendingMarket.Visible = False
            Button_NewMarket.Visible = False
            ComboBox_Market.Enabled = True
        End If
    End Sub

    Private Sub CheckBox_NewMarket_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_NewMarket.CheckedChanged
        EnableNewMarket()
    End Sub


    Private Sub Button_NewMarket_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_NewMarket.Click
        If ComboBox_PendingMarket.Text = "" Then
            MessageBox.Show("Market Name cannot be empty !")
            Return
        End If

        'MessageBox.Show("NO GSM Scanners will be created for any NEW MARKET NAME entered manually")

        If Not Common.hasWCFAccess Then
            Return
        End If
        Dim i As Integer
        Dim market As _MarketName
        i = 0


        For Each market In ComboBox_PendingMarket.Items

            If market.name_market = ComboBox_PendingMarket.Text Then

                ComboBox_PendingMarket.SelectedIndex = i
                Button_NewMarket.Enabled = False
                MessageBox.Show("Market is already in the list. !")
                Return
            Else
                Button_NewMarket.Enabled = True
            End If
            i = i + 1
        Next


        'For i = 0 To ComboBox_Market.Items.Count - 1
        '    Dim City As String = ComboBox_Market.Items.Item(i).ToString().Split(" ")(0)


        '    If (ComboBox_Market.Items.Item(i) = ComboBox_PendingMarket.Text) Then
        '        MessageBox.Show("Market is already in the list.")
        '        ComboBox_Market.Enabled = True
        '        ComboBox_Market.SelectedIndex = ComboBox_Market.Items.IndexOf(ComboBox_PendingMarket.Text)
        '        Return
        '    ElseIf (ComboBox_Market.Items.Item(i).ToString.ToLower.Contains(ComboBox_PendingMarket.Text.ToLower)) Then
        '        Dim Button = MessageBox.Show("Market may be already in the list.(" + ComboBox_Market.Items.Item(i).ToString + "). Continue ?", "Message", MessageBoxButtons.YesNo, _
        '                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        '        If Button = Windows.Forms.DialogResult.No Then
        '            Return
        '        End If
        '    ElseIf (ComboBox_PendingMarket.Text.ToLower.Contains(City.ToLower)) Then
        '        Dim Button = MessageBox.Show("Market may be already in the list.(" + ComboBox_Market.Items.Item(i).ToString + "). Continue ?", "Message", MessageBoxButtons.YesNo, _
        '                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        '        If Button = Windows.Forms.DialogResult.No Then
        '            Return
        '        End If
        '    End If
        'Next

        'Button_NewMarket.Enabled = False

        Dim response As String

        Dim name_market As String = ComboBox_PendingMarket.Text
        Dim proxy As GTPServiceClient
        proxy = New GTPServiceClient()
        Common.AssignCredentials(proxy)
        If name_market <> "" Then
            proxy.AddMarket(name_market)
        End If


        'Send email to administrator for notification
        Try
            Dim strTo As String = "tuand@gwsolutions.com"
            Dim strFrom As String = "gwsftp@gmail.com"
            Dim strSubject As String = "New market:" + name_market + " has been added to the databasse from " + TextBox_UserName.Text
            Dim strContent As String = "New market:" + name_market + " has been added to the databasse from " + TextBox_UserName.Text

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
            message.Body = strContent
            message.To.Add(strTo)
            message.To.Add("tuand@gwsolutions.com")
            smtpClient.Send(message)
        Catch ex As Exception

        End Try

        'ComboBox_PendingMarket.Items.Clear()
        Dim Markets As _MarketName()
        Markets = proxy.GetMarket(1)

        ComboBox_PendingMarket.DataSource = Markets
        ComboBox_PendingMarket.DisplayMember = "name_market"
        ComboBox_PendingMarket.ValueMember = "id_market"

        ComboBox_PendingMarket.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox_PendingMarket.AutoCompleteSource = AutoCompleteSource.ListItems
        Button_NewMarket.Enabled = True
        proxy.Close()

    End Sub

    Private Sub Timer_TotalFile_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer_TotalFile.Tick
        If total_uploaded_file > 0 Then
            Label_TotalFile.Text = count_uploaded.ToString + " of total " + total_file_to_uplload.ToString + " files uploaded "
        End If
    End Sub

    Private Sub Button1_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ListView_MissingFile.Items.Clear()
        isSelectFile = False
        selectedUploadFilesExist = New List(Of String)

        For Each item In ListView_FileList.Items
            selectedUploadFilesExist.Add(item.Text)
        Next

        '.sqc/.sqz/.mf/images only
        'All image/video types
        'All file types


        RadioButton_SQZ.Enabled = False
        RadioButton_SQZ_PCAP.Enabled = False
        GroupBox_PCAP_Options.Enabled = False

        Dim FolderDiaglog As FolderBrowserDialog

        FolderDiaglog = New FolderBrowserDialog()
        FolderDiaglog.ShowNewFolderButton = False

        FolderDiaglog.ShowDialog()

        Dim folder_path As String = FolderDiaglog.SelectedPath

        If folder_path.IndexOf("'") >= 0 Or folder_path.IndexOf("`") >= 0 Then
            MessageBox.Show(" The input path has special chracter. Please remove the special char from the input path and restart the upload process.")
            Return
        End If

        If Not Common.isFolderWritable(folder_path) Then
            MessageBox.Show("Files cannot be uploaded from a read-only location")
            Return
        End If

        Dim imageStr As String = "*.jpg,*.jpeg,*.jpe,*.png,*.bmp,*.gif,*.tif,*.mpo,*.heic,*.heif,*.heiv,*.avi,*.flv,*.wmv,*.mov,*.mp4"

        If folder_path <> "" Then
            Dim Subfolders() As String = Common.getAllFolders(folder_path)
            Dim FileList() As String
            Dim count As Integer = -1
            Dim di As New IO.DirectoryInfo(folder_path)
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo


            Dim sqcFileList As New List(Of String)


            For Each dra In diar1

                If selectedUploadFilesExist.Contains(dra.FullName) Then
                    Continue For
                End If
                Dim fileType As String = ComboBox_FileType.SelectedItem

                If Not CheckFileExist(dra.FullName) Then

                    If ComboBox_FileType.SelectedItem = ".sqz only" Or ComboBox_FileType.SelectedItem = ".sqc/.sqz/.mf only" Or ComboBox_FileType.SelectedItem = ".sqc/.sqz/.mf/images only" Or ComboBox_FileType.SelectedItem = "All file types" Then
                        If dra.FullName.ToLower.EndsWith(".sqz") And Not isPcapFileExist Then
                            Dim FileInfo As FileInfo
                            FileInfo = New FileInfo(dra.FullName)

                            If FileInfo.Length > 30 * 1024 * 1024 Then
                                If CheckPCAP(dra.FullName) Then
                                    isPcapFileExist = True
                                    GroupBox_PCAP_Options.Enabled = True
                                    RadioButton_SQZ.Enabled = True
                                    RadioButton_SQZ_PCAP.Enabled = True
                                End If
                            End If
                        End If
                    End If


                    If ComboBox_FileType.SelectedItem = ".sqz/.mf only" Then
                        If dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If

                    End If
                    '.sqc/.sqz/.mf/images only

                    If ComboBox_FileType.SelectedItem = ".sqc/.sqz/.mf only" Then
                        If dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Or dra.Name.ToLower.EndsWith(".sqc") Then

                            If dra.Name.ToLower.EndsWith(".sqc") Then
                                sqcFileList.Add(dra.FullName)

                            Else

                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If

                        End If

                    End If

                    Dim fileExt As String = Common.GetFileExtension(dra.Name.ToLower())

                    If ComboBox_FileType.SelectedItem = ".sqc/.sqz/.mf/images only" Then
                        If dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Or dra.Name.ToLower.EndsWith(".sqc") Or imageStr.IndexOf("*." + fileExt) >= 0 Then

                            If dra.Name.ToLower.EndsWith(".sqc") Then
                                sqcFileList.Add(dra.FullName)

                            Else

                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If

                        End If

                    End If



                    If ComboBox_FileType.SelectedItem = "All file types" Then


                        If imageStr.IndexOf("*." + fileExt) >= 0 Or dra.Name.ToLower.EndsWith(".pcapzip") Or dra.Name.ToLower.EndsWith(".gpx") Or dra.Name.ToLower.EndsWith(".nmf") Or dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Or dra.Name.ToLower.EndsWith(".txt") Or dra.Name.ToLower.EndsWith(".log") Or dra.Name.ToLower.EndsWith(".wnd") Or dra.Name.ToLower.EndsWith(".wnu") Or dra.Name.ToLower.EndsWith(".wnl") Or dra.Name.ToLower.EndsWith(".trp") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = "All image/video types" Then
                        If imageStr.IndexOf("*." + fileExt) >= 0 Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".sqz only" Then
                        If dra.Name.ToLower.EndsWith(".sqz") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".sqc only" Then
                        If dra.Name.ToLower.EndsWith(".sqc") Then
                            sqcFileList.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".log only" Then
                        If dra.Name.ToLower.EndsWith(".log") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".wnd only" Then
                        If dra.Name.ToLower.EndsWith(".wnd") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".wnu only" Then
                        If dra.Name.ToLower.EndsWith(".wnu") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".wnl only" Then
                        If dra.Name.ToLower.EndsWith(".wnl") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".trp only " Then
                        If dra.Name.ToLower.EndsWith(".trp") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".txt only" Then
                        If dra.Name.ToLower.EndsWith(".txt") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".mf only" Then
                        If dra.Name.ToLower.EndsWith(".mf") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".nmf/.gpx only" Then
                        If dra.Name.ToLower.EndsWith(".nmf") Or dra.Name.ToLower.EndsWith(".gpx") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".sqc only" Then
                        If dra.Name.ToLower.EndsWith(".sqc") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    ElseIf ComboBox_FileType.SelectedItem = ".pcapzip only" Then
                        If dra.Name.ToLower.EndsWith(".pcapzip") Then
                            Dim lvi As New ListViewItem
                            lvi.Text = dra.FullName
                            ListView_FileList.Items.Add(lvi)
                            lvi.SubItems.Add(dra.Length)
                            selectedUploadFilesExist.Add(dra.FullName)
                        End If
                    End If

                Else
                    Dim lvi As New ListViewItem
                    lvi.Text = dra.FullName
                    lvi.ForeColor = Color.Brown
                    lvi.SubItems.Add(dra.Length)
                    ListView_Dups.Items.Add(lvi)
                    TabControl1.SelectedIndex = 3
                End If
            Next


            For Each f In Subfolders
                di = New IO.DirectoryInfo(f)
                diar1 = di.GetFiles()
                For Each dra In diar1
                    Dim fileExt As String = Common.GetFileExtension(dra.Name.ToLower())

                    If selectedUploadFilesExist.Contains(dra.FullName) Then
                        Continue For
                    End If

                    If Not CheckFileExist(dra.FullName) Then
                        If dra.FullName.ToLower.EndsWith(".sqz") And Not isPcapFileExist Then
                            Dim FileInfo As FileInfo
                            FileInfo = New FileInfo(dra.FullName)

                            If FileInfo.Length > 30 * 1024 * 1024 Then
                                If CheckPCAP(dra.FullName) Then
                                    isPcapFileExist = True
                                    RadioButton_SQZ.Enabled = True
                                    RadioButton_SQZ_PCAP.Enabled = True
                                    GroupBox_PCAP_Options.Enabled = True
                                End If
                            End If
                        End If

                        If ComboBox_FileType.SelectedItem = ".sqc/.sqz/.mf only" Then
                            If dra.Name.ToLower.EndsWith(".sqc") Then
                                sqcFileList.Add(dra.FullName)
                            End If
                            If dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If

                        End If

                        If ComboBox_FileType.SelectedItem = ".sqc/.sqz/.mf/images only" Then

                            If dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Or dra.Name.ToLower.EndsWith(".sqc") Or imageStr.IndexOf("*." + fileExt) >= 0 Then

                                If dra.Name.ToLower.EndsWith(".sqc") Then
                                    sqcFileList.Add(dra.FullName)

                                Else

                                    Dim lvi As New ListViewItem
                                    lvi.Text = dra.FullName
                                    ListView_FileList.Items.Add(lvi)
                                    lvi.SubItems.Add(dra.Length)
                                    selectedUploadFilesExist.Add(dra.FullName)
                                End If

                            End If

                        End If

                        If ComboBox_FileType.SelectedItem = ".sqz/.mf only" Then
                            If dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If

                        End If


                        If ComboBox_FileType.SelectedItem = "All file types" Then
                            If imageStr.IndexOf("*." + fileExt) >= 0 Or dra.Name.ToLower.EndsWith(".pcapzip") Or dra.Name.ToLower.EndsWith(".gpx") Or dra.Name.ToLower.EndsWith(".sqc") Or dra.Name.ToLower.EndsWith(".nmf") Or dra.Name.ToLower.EndsWith(".mf") Or dra.Name.ToLower.EndsWith(".sqz") Or dra.Name.ToLower.EndsWith(".txt") Or dra.Name.ToLower.EndsWith(".log") Or dra.Name.ToLower.EndsWith(".wnd") Or dra.Name.ToLower.EndsWith(".wnu") Or dra.Name.ToLower.EndsWith(".wnl") Or dra.Name.ToLower.EndsWith(".trp") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = "All image/video types" Then
                            If imageStr.IndexOf("*." + fileExt) >= 0 Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".sqz only" Then
                            If dra.Name.ToLower.EndsWith(".sqz") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".sqc only" Then
                            If dra.Name.ToLower.EndsWith(".sqc") Then
                                sqcFileList.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".log only" Then
                            If dra.Name.ToLower.EndsWith(".log") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".wnd only" Then
                            If dra.Name.ToLower.EndsWith(".wnd") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".wnu only" Then
                            If dra.Name.ToLower.EndsWith(".wnu") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".wnl only" Then
                            If dra.Name.ToLower.EndsWith(".wnl") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".trp only " Then
                            If dra.Name.ToLower.EndsWith(".trp") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".txt only" Then
                            If dra.Name.ToLower.EndsWith(".txt") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".mf only" Then
                            If dra.Name.ToLower.EndsWith(".mf") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".pcapzip only" Then
                            If dra.Name.ToLower.EndsWith(".pcapzip") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        ElseIf ComboBox_FileType.SelectedItem = ".nmf/.gpx only" Then
                            If dra.Name.ToLower.EndsWith(".nmf") Or dra.Name.ToLower.EndsWith(".gpx") Then
                                Dim lvi As New ListViewItem
                                lvi.Text = dra.FullName
                                ListView_FileList.Items.Add(lvi)
                                lvi.SubItems.Add(dra.Length)
                                selectedUploadFilesExist.Add(dra.FullName)
                            End If
                        End If
                    Else

                        Dim lvi As New ListViewItem
                        lvi.Text = dra.FullName
                        lvi.ForeColor = Color.Brown
                        lvi.SubItems.Add(dra.Length)
                        ListView_Dups.Items.Add(lvi)
                        TabControl1.SelectedIndex = 3
                    End If
                    '.sqc/.sqz/.mf only


                Next
            Next




            If sqcFileList.Count = 0 Then

                If CheckBox_Duplicate.Checked Then

                    If (Common.hasWCFAccess) Then

                        CheckDuplicate(selectedUploadFilesExist)
                    End If
                End If

                CheckDuplicateLocal(selectedUploadFilesExist)
                'CheckDuplicateLocal_Background()

                UpdateControl_Sub(Label_TotalFile, "Total file(s): " + ListView_FileList.Items.Count.ToString)
                total_file_to_uplload = ListView_FileList.Items.Count
                total_file_to_uplload = selectedUploadFiles.Count

                Return
            End If


            selectedUploadFiles.AddRange(sqcFileList)

            Dim i As Integer

            For i = 0 To selectedUploadFiles.Count - 1
                ReDim Preserve tmpSelectedUploadFiles(i)
                tmpSelectedUploadFiles(i) = selectedUploadFiles(i)
            Next

            Dim totalSize As Double = 0

            For i = 0 To tmpSelectedUploadFiles.Count - 1
                If tmpSelectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Then
                    Dim tmpFileINfor As New FileInfo(tmpSelectedUploadFiles(i))
                    totalSize = totalSize + tmpFileINfor.Length / 1024 / 1024 / 1024
                End If
            Next

            totalSize = totalSize + 1
            'totalSize = 611

            For i = 0 To tmpSelectedUploadFiles.Count - 1
                If tmpSelectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Then

                    Dim DirName As String = tmpSelectedUploadFiles(i).Substring(0, 1) + ":\"

                    Dim driveInfos As DriveInfo() = System.IO.DriveInfo.GetDrives()

                    For Each driveInfo In driveInfos
                        If driveInfo.Name = DirName Then
                            Dim freeSpace As Double = driveInfo.AvailableFreeSpace / 1024 / 1024 / 1024
                            ' freeSpace = 648.668
                            If freeSpace < totalSize Then
                                MessageBox.Show("This application will automatically unzip all SQZ files from the selected SQC files. This process requires " + Math.Round(totalSize, 1).ToString() + " GB free space.This system only has " + Math.Round(freeSpace, 1).ToString() + " GB free space available. Please create free space before continuing.")
                                Return
                            End If
                        End If

                    Next
                End If
            Next

            BackgroundWorker_Unzip.RunWorkerAsync()

        End If
    End Sub


    Private Sub RemoveDupliateFile()
        Dim s1 As String = ""
        Dim s2 As String = ""
        Dim dra As IO.FileInfo

        Dim toDelete As Boolean()
        ReDim toDelete(ListView_FileList.Items.Count - 1)

        For i = 0 To ListView_FileList.Items.Count - 1
            toDelete(i) = False
        Next


        Dim FileList() As String
        ReDim FileList(ListView_FileList.Items.Count - 1)
        For i = 0 To ListView_FileList.Items.Count - 1
            FileList(i) = ListView_FileList.Items.Item(i).Text
        Next

        For i = 0 To ListView_FileList.Items.Count - 2
            s1 = ListView_FileList.Items.Item(i).Text
            s1 = s1.Substring(s1.LastIndexOf("\") + 1, s1.Length - s1.LastIndexOf("\") - 1)
            For j = i + 1 To ListView_FileList.Items.Count - 1
                s2 = ListView_FileList.Items.Item(j).Text
                s2 = s2.Substring(s2.LastIndexOf("\") + 1, s2.Length - s2.LastIndexOf("\") - 1)

                If s1 = s2 Then
                    toDelete(j) = True
                End If

            Next
        Next

        For Each item In ListView_FileList.Items
            ListView_FileList.Items.Remove(item)
        Next

        For i = 0 To FileList.Length - 1

            If Not toDelete(i) Then
                dra = My.Computer.FileSystem.GetFileInfo(FileList(i))
                Dim lvi As New ListViewItem
                lvi.Text = dra.FullName
                ListView_FileList.Items.Add(lvi)
                lvi.SubItems.Add(dra.Length)

            End If
        Next


    End Sub


    Private Function CheckFileExist(ByVal FileName As String) As Boolean
        'C:\TempGTPSQC\04ed7af6-d11f-40a2-98bf-9b5a89e178cc\CSM0-2019-04-10-07-20-27-0000-0011-0000-0000_sqc\CSM0\ATT Data LTE\2019-04-10-07-20-37-0000-0011-0000-0102-S.sqz
        Dim isExist As Boolean = False
        Dim i As Integer
        For i = 0 To selectedUploadFiles.Count - 1
            If FileName <> selectedUploadFiles(i) Then
                Dim s1 As String = FileName.Substring(FileName.LastIndexOf("\") + 1, FileName.Length - FileName.LastIndexOf("\") - 1)
                Dim s2 As String = selectedUploadFiles(i)
                s2 = s2.Substring(s2.LastIndexOf("\") + 1, s2.Length - s2.LastIndexOf("\") - 1)
                '2018-04-10-10-51-52-0000-0000-7689-2122-S.sqz
                If s2.Split(".")(0) = s1.Split(".")(0) Then
                    isExist = True
                End If
            End If

        Next
        Return isExist
    End Function

    Private Sub UpdateTotalFile()

        Dim count_uploaded_file As Integer = 0
        Dim k As Integer
        For k = 0 To ListView_FileList.Items.Count - 1
            If ListView_FileList.Items(k).ForeColor = Color.Green Then
                count_uploaded_file += 1
            End If
        Next

        Label_TotalFile.Text = count_uploaded_file.ToString + " file(s) uploaded of total " + ListView_FileList.Items.Count.ToString + " file(s) "
        'Application.DoEvents()
        'Thread.Sleep(1000)

    End Sub

    Private Sub Upload_MultiThread()
        'uploadCode = GWSFTP.GWSUploadOneFile(FileList(i), GWSFTP.ftphost + "/" + DestinationFolder, id_transaction)
        'mainThread.Suspend()
        'uploadCode_multithread = GWSFTP_Multithread.GWSUploadOneFile(File_multithread, ftp_url_multithread, id_transaction_multithread)
        'mainThread.Resume()
    End Sub

    Private Sub LinkLabel_ReportIssue_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        Process.Start(Convert.ToString(e.Link.LinkData))

    End Sub

    Private Sub Timer_Upload_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer_Upload.Tick

        'Timer_Upload.Interval = 600000000
        Button_Upload.Text = "Re-Upload"
        Timer_Upload.Stop()

        Button_Upload_Do(ReuploadProcess)
        'Timer_Upload.Enabled = False
    End Sub

    Private Sub CheckOrphan(ByVal TeamName As String, ByVal DateList As DateTime())
        'Dim proxy As GTPServiceClient
        'proxy = New GTPServiceClient()
        'Dim currentBFiles As _CurrentBSide()
        'currentBFiles = proxy.GetCurrentBSide(TeamName, DateList)
        'proxy.Close()

    End Sub


    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If id_FTP_Transaction.ToString = "0" And Not (Not Common.hasWCFAccess Or Common.forceXMLMode) Then
            Return
        End If
        Dim FiletoUpload As String
        _isFinished1 = False

        id_client_public = id_FTP_Client

        Dim proxy As GTPServiceClient
        If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
            proxy = New GTPServiceClient()
            Common.AssignCredentials(proxy)
        End If


        Dim cmd As OleDbCommand
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr

        Dim i As Integer
        Dim ii As Integer
        Dim isUploaded As Boolean
        Dim isUploading As Boolean

        Dim isFileExist As Boolean

        Dim uploadRate As Integer = 0
        'C:\GTPTest\2012-03-19-09-23-21-0004-0023-0003-0004-A.sqz
        For i = 0 To _FileUpload.Length - 1

            If BackgroundWorker1.CancellationPending Then
                e.Cancel = True
                Common.isWorker1Stop = True
                Return
            End If


            isUploaded = False
            isUploading = False
            isFileExist = True


            If _FileUpload(i).Status = 1 Then
                isUploaded = True
            End If

            If _FileUpload(i).Status = 2 Then
                isUploading = True
            End If

            If Not File.Exists(_FileUpload(i).FileName) Then

                If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                    Dim status As Integer = proxy.GetFileStatus(id_FTP_Transaction, _FileUpload(i).FileName)
                    If Not ((status = 1 Or status >= 3)) Then
                        If _FileUpload(i).FileName <> Nothing Then
                            proxy.UpdatetbFTPFile(id_FTP_Transaction, _FileUpload(i).FileName, -5)
                        End If

                    End If

                End If

                isFileExist = False
                _FileUpload(i).Status = -5
                RemoveListViewItem(ListView_FileList, _FileUpload(i).FileName)
                AddListViewItem(ListView_OtherFiles, _FileUpload(i).FileName)
            Else
                isFileExist = True
            End If


            If (Not isUploaded) And (Not isUploading) And isFileExist Then
                If _FileUpload(i).Status = 0 Then
                    _FileUpload(i).Status = 2
                    isUploading = True
                    FiletoUpload = _FileUpload(i).FileName

                    'uploadStatus = GWSFTP.GWSUploadOneFile(uploadRate, FiletoUpload, FTPURL, id_FTP_Transaction)
                    Dim FileInfo = New FileInfo(FiletoUpload)
                    UpdateControl_Sub(Label_CurrentFile, "File " + FileInfo.Name + " progress :")
                    If (id_FTP_Transaction = "") Then
                        id_FTP_Transaction = -1
                    End If
                    'uploadStatus = GWSUploadAsyn(FiletoUpload, FTPURL, Convert.ToInt32(id_FTP_Transaction), FileInfo.Length, 1)
                    Dim uploadFileResult As UploadResult = GWSUploadAsyn(FiletoUpload, FTPURL, Convert.ToInt32(id_FTP_Transaction), FileInfo.Length, 1)
                    uploadStatus = uploadFileResult.uploadStatus
                    If IsNothing(uploadFileResult.fileTrimmingResult.FileName) And (FiletoUpload.ToLower.EndsWith(".mf") Or FiletoUpload.ToLower.EndsWith(".sqz")) Then
                        _FileUpload(i).Status = 9 'Unzip Failed                    
                        uploadFileResult.fileTrimmingResult.isScanner = False
                    End If

                    If Not IsNothing(uploadFileResult.fileTrimmingResult) And (FiletoUpload.ToLower.EndsWith("a.mf") Or FiletoUpload.ToLower.EndsWith("a.sqz")) Then

                        Dim BSideDevice As String = ""

                        If uploadFileResult.fileTrimmingResult.BSideFileType = "Mobile" Then
                            BSideDevice = "Mobile"
                        End If
                        Dim proxyBSide As New GTPServiceClient()
                        Dim queryString As String = "UPDATE tblFTPFile SET BsideDeviceType ='" + BSideDevice + "', isAsideOrphan = " + uploadFileResult.fileTrimmingResult.isAsideOrphan.ToString() + " WHERE id_server_transaction = " + id_FTP_Transaction + " AND file_name = '" + FiletoUpload + "'"
                        proxyBSide.ExecuteQuery(queryString)

                        proxyBSide.Close()

                    End If

                    UpdateTextBoxLog_Sub(TextBox_FTPSessionLog, FTPSessionLog)





                    Try
                        If File.Exists(log_file) Then
                            Using sw As StreamWriter = New StreamWriter(log_file)
                                sw.Write(FTPSessionLog)
                            End Using
                        End If

                    Catch ex As Exception

                    End Try

                    Dim count11 As Integer
                    Dim uploadCode As Integer = uploadStatus

                    If uploadCode = 0 Or uploadCode = -1 Then
                        If _FileUpload(i).Status <> 9 Then
                            _FileUpload(i).Status = 1
                        End If
                        count_uploaded_files += 1
                        total_uploaded_file += 1
                        count_uploaded += 1

                        If Not Common.hasWCFAccess Or Common.forceXMLMode Then
                            'Update XML File
                            Try

                                Dim xmlDOC As New XmlDocument
                                xmlDOC.Load(Common.currentXMLFile)
                                Dim node As XmlNode
                                node = xmlDOC.SelectSingleNode("/tblFTPTransaction/tblFTPFile[@file_name='" + FiletoUpload + "']")
                                If Not IsNothing(node) Then
                                    node.Attributes("is_finished").Value = "True"
                                    node.Attributes("date_finished").Value = Common.getDateString(Common.GetCurrentEasternTime())
                                    'node.Attributes("date_finished").Value = Common.getDateString(DateTime.Now)
                                    If _FileUpload(i).Status = 9 Then
                                        node.Attributes("file_status").Value = "1"
                                        node.Attributes("status_flag").Value = "9"
                                    Else
                                        node.Attributes("file_status").Value = _FileUpload(i).Status.ToString()
                                    End If

                                    node.Attributes("isScanner").Value = uploadFileResult.fileTrimmingResult.isScanner.ToString()

                                    node.Attributes("isAsideOrphan").Value = uploadFileResult.fileTrimmingResult.isAsideOrphan.ToString()

                                    If Not IsNothing(uploadFileResult.fileTrimmingResult) And (FiletoUpload.ToLower.EndsWith("a.mf") Or FiletoUpload.ToLower.EndsWith("a.sqz")) Then

                                        If uploadFileResult.fileTrimmingResult.BSideFileType = "Mobile" Then
                                            node.Attributes("BSideDeviceType").Value = "Mobile"
                                        Else
                                            node.Attributes("BSideDeviceType").Value = ""
                                        End If
                                    End If


                                End If

                                If uploadCode = 0 Then
                                    Dim tt As Integer = (Common.FileUploadEndTime - Common.FileUploadStartTime).TotalMilliseconds
                                    node.Attributes("uploadtime").Value = tt.ToString()
                                End If


                                Dim TransNode As XmlNode
                                TransNode = xmlDOC.SelectSingleNode("/tblFTPTransaction/Transaction")
                                TransNode.Attributes("total_file_uploaded").Value = total_uploaded_file


                                xmlDOC.Save(Common.currentXMLFile)
                                UploadFile(Common.currentXMLFile, Common.currentXMLFolder)
                                File.Copy(Common.currentXMLFile, Common.currentXMLFile.Replace(".xml", "_backup.xml"), True)

                            Catch ex As Exception

                            End Try

                        End If



                        'File uploaded sucessful

                        Dim file_uploaded_server = 0
                        Dim status As Integer = 1
                        Dim total_file = ListView_FileList.Items.Count

                        If IsNothing(uploadFileResult.fileTrimmingResult.FileName) And (FiletoUpload.ToLower.EndsWith(".mf") Or FiletoUpload.ToLower.EndsWith(".sqz")) Then
                            status = 9
                        End If

                        Dim isSuccess As Boolean

                        If Common.hasWCFAccess And Not Common.forceXMLMode And Not Common.isXMLMode Then
                            If uploadCode = 0 Or uploadCode = -1 Then
                                isSuccess = proxy.UpdatetbFTPFile(Convert.ToInt32(id_FTP_Transaction), FiletoUpload, status)

                                'Update isScanner status to server
                                If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                                    Dim proxyGTP As New GTPServiceClient()
                                    proxyGTP.Open()
                                    proxyGTP.UpdateFileScannerStatus(Convert.ToInt32(id_FTP_Transaction), FiletoUpload, uploadFileResult.fileTrimmingResult.isScanner)
                                    proxyGTP.Close()
                                Else
                                    'Update using GTP XML mode
                                End If

                                Dim tt As Double = (Common.FileUploadEndTime - Common.FileUploadStartTime).TotalMilliseconds
                                proxy.ExecuteQuery("UPDATE tblFTPFile  SET uploadtime = " + tt.ToString() + " WHERE id_server_transaction=" + id_FTP_Transaction.ToString() + " AND file_name='" + FiletoUpload.Replace("'", "''") + "'")
                                file_uploaded_server = proxy.countFile(id_FTP_Transaction, "finished")
                            End If
                            If file_uploaded_server < total_file Then
                                BackgroundWorker1.ReportProgress(CInt((file_uploaded_server) / total_file * 100))
                            End If
                            'proxy.updateTotalUploaded(id_FTP_Transaction, file_uploaded_server)
                            If (ListView_FileList.Items.Count > 0) And (file_uploaded_server <= ListView_FileList.Items.Count) Then
                                If file_uploaded_server < ListView_FileList.Items.Count Then
                                    UpdateControl_Sub(Label_TotalFile, (file_uploaded_server).ToString() + " file(s) uploaded of total " + ListView_FileList.Items.Count.ToString() + " file(s) ")
                                End If
                            End If


                        Else
                            Try
                                If total_uploaded_file < total_file Then
                                    BackgroundWorker1.ReportProgress(CInt((total_uploaded_file) / total_file * 100))
                                End If
                            Catch ex As Exception

                            End Try

                            If (ListView_FileList.Items.Count > 0) And (total_uploaded_file <= ListView_FileList.Items.Count) Then
                                If total_uploaded_file < ListView_FileList.Items.Count Then
                                    UpdateControl_Sub(Label_TotalFile, (total_uploaded_file).ToString() + " file(s) uploaded of total " + ListView_FileList.Items.Count.ToString() + " file(s) ")
                                End If
                            End If
                        End If


                        If (Not (Common.hasWCFAccess And Not Common.forceXMLMode And Not Common.isXMLMode)) Or isSuccess Then
                            UpdateListView_Sub(ListView_FileList, FiletoUpload, Color.Green)
                            'Update tblFileInfo
                            cmd = New OleDbCommand
                            cmd.Connection = cnn
                            If cnn.State <> ConnectionState.Open Then
                                cnn.Open()
                            End If
                            cmd.CommandText = "UPDATE tblFileInfo SET [is_finished] = TRUE, [date_finished]=Date() WHERE id_transaction=" & id_FTP_Client & " AND file_name ='" & FiletoUpload.Replace("'", "''") & "'"
                            cmd.ExecuteNonQuery()
                            cmd.Dispose()
                            cnn.Close()
                        End If



                    ElseIf uploadCode = 1 Then
                        _FileUpload(i).Status = 0
                        UpdateControl_Sub(Label_Info, "Internet connection has been lost. Upload process will resume automatically upon gaining internet connection!")

                        If BackgroundWorker1.IsBusy Then
                            'If it supports cancellation, Cancel It
                            If BackgroundWorker1.WorkerSupportsCancellation Then
                                ' Tell the Background Worker to stop working.
                                BackgroundWorker1.CancelAsync()
                            End If
                        End If

                        'Timer1.Enabled = True
                        Me.Invoke(New MethodInvoker(AddressOf Me.Timer1.Start))

                    ElseIf uploadCode = 2 Then

                        Try
                            'Try to delete file    
                            System.Threading.Thread.Sleep(1500)
                            Dim remote_dir = FTPURL.Substring(FTPURL.LastIndexOf("/") + 1, FTPURL.Length - FTPURL.LastIndexOf("/") - 1)
                            remote_dir = Common.FTPUserName + "_curUpload/" + remote_dir
                            Common.DeleteFileonFTP(FiletoUpload, remote_dir)
                        Catch ex As Exception

                        End Try
                        _FileUpload(i).Status = 0
                        'Unknow error or timeout - need to re-upload file
                        UpdateListView_Sub(ListView_FileList, FiletoUpload, Color.Red)
                        UpdateControl_Sub(Label_Info, "File: " + FiletoUpload + " was failed to upload.")
                        cmd = New OleDbCommand
                        cmd.Connection = cnn
                        If cnn.State <> ConnectionState.Open Then
                            cnn.Open()
                        End If
                        cmd.CommandText = "UPDATE tblFileInfo SET file_status=-1  WHERE id_transaction=" & id_FTP_Client & " AND file_name ='" & FiletoUpload.Replace("'", "''") & "'"
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                        cnn.Close()

                        'Update server
                        If Common.hasWCFAccess And Not Common.forceXMLMode Then
                            'Update server
                            proxy.UpdatetbFTPFile(Convert.ToInt32(id_FTP_Transaction), FiletoUpload, -1)

                        End If

                        total_file_failed += 1

                        'Update total file uploaded on server
                        If Common.hasWCFAccess And Not Common.forceXMLMode Then
                            Try
                                Dim file_uploaded_server = proxy.countFile(id_FTP_Transaction, "finished")
                                'proxy.updateTotalUploaded(id_FTP_Transaction, file_uploaded_server)
                            Catch ex As Exception

                            End Try
                        End If
                    End If

                    If uploadCode = 1 Then
                        cmd = New OleDbCommand
                        cmd.Connection = cnn
                        If cnn.State <> ConnectionState.Open Then
                            cnn.Open()
                        End If
                        cmd.CommandText = "UPDATE tblTransactionInfo SET [is_network_lost] = TRUE WHERE id_transaction=" & id_FTP_Client
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                        cnn.Close()
                        Exit Sub
                    End If

                End If
            End If
        Next

        Dim _TotalUploaded As Integer = 0
        Dim _TotalRemoved As Integer = 0

        For i = 0 To _FileUpload.Length - 1
            If _FileUpload(i).Status = 1 Or _FileUpload(i).Status = 9 Then
                _TotalUploaded += 1
            End If
            If _FileUpload(i).Status < 0 Then
                _TotalRemoved += 1
            End If
        Next


        If _TotalUploaded = _FileUpload.Length - _TotalRemoved Then


            If id_FTP_Transaction.Trim() <> "" Then
                id_server_transaction_finished = id_FTP_Transaction
            End If
            id_transaction_finished = id_FTP_Client
            cmd = New OleDbCommand
            cmd.CommandText = "UPDATE tblTransactionInfo SET [isCompleted] = True, [date_end] = Date() WHERE id_transaction = " + id_FTP_Client.ToString
            cmd.Connection = cnn
            If cnn.State <> ConnectionState.Open Then
                cnn.Open()
            End If

            cmd.ExecuteNonQuery()
            'Update to server that the transaction has been completed
            Dim isCompleted As Boolean = True

            Try
                Me.Invoke(New MethodInvoker(AddressOf Me.Timer_Upload.Stop))

            Catch ex As Exception
                Try
                    Timer_Upload.Stop()
                Catch ex1 As Exception

                End Try
            End Try

            If Common.hasWCFAccess And Not Common.forceXMLMode Then

                proxy.completeTransaction(id_FTP_Transaction)

            End If


            If isCompleted Then
                If Not Common.hasWCFAccess Or Common.forceXMLMode Then
                    Dim xmlDoc As New XmlDocument()
                    xmlDoc.Load(Common.currentXMLFile)

                    Dim node As XmlNode
                    node = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
                    If Not IsNothing(node) Then
                        'node.Attributes("date_end").Value = DateTime.Now.ToString()
                        node.Attributes("date_end").Value = Common.getDateString(Common.GetCurrentEasternTime())
                        node.Attributes("isCompleted").Value = "True"
                        node.Attributes("transaction_status").Value = "1"

                    End If
                    'Update File status in case some file not updated
                    For i = 0 To _FileUpload.Length - 1
                        If _FileUpload(i).Status = 1 Or _FileUpload(i).Status = 9 Then
                            Dim FileNode As XmlNode
                            FileNode = xmlDoc.SelectSingleNode("/tblFTPTransaction/tblFTPFile[@file_name='" + _FileUpload(i).FileName + "']")

                            If Not IsNothing(FileNode) Then
                                If FileNode.Attributes("is_finished").Value <> "True" Then
                                    FileNode.Attributes("is_finished").Value = "True"
                                    'FileNode.Attributes("date_finished").Value = DateTime.Now.ToString()
                                    FileNode.Attributes("date_finished").Value = Common.getDateString(Common.GetCurrentEasternTime())
                                    FileNode.Attributes("file_status").Value = _FileUpload(i).Status.ToString()
                                End If
                            End If

                        End If

                    Next

                    xmlDoc.Save(Common.currentXMLFile)
                    File.Copy(Common.currentXMLFile, Common.currentXMLFile.Replace(".xml", "_backup.xml"), True)
                    UploadFile(Common.currentXMLFile, Common.currentXMLFolder)
                End If

                'Me.ControlBox = True
                'SQZ complete.  Starting PCAPZIP

                If isUploadPcap And Not _isRunUploadPcap And RadioButton_SQZ_PCAP.Checked Then
                    UpdateControl_Sub(Label_Info, "SQZ complete.  Starting PCAPZIP")
                    UpdateControl_Sub(Label_TotalFile, "SQZ complete.  Starting PCAPZIP")
                Else
                    UpdateControl_Sub(Label_Info, "All files have been uploaded")
                    UpdateControl_Sub(Label_TotalFile, "All files have been uploaded")
                End If


                addTrimmingFailed()

                UpdateControl_Sub(Label_TotalFailed, "")
                EnableButton_Sub(Button_DeleteUploadedFiles, True)
                If cnn.State <> ConnectionState.Closed Then
                    cnn.Close()
                End If
                'Upload log file to FTP server
                If Common.hasWCFAccess And Not Common.forceXMLMode Then
                    Try
                        UploadFile(log_file, "GTPLogs")
                    Catch ex As Exception

                    End Try
                End If
                If isUploadPcap And Not _isRunUploadPcap And RadioButton_SQZ_PCAP.Checked Then
                    BackgroundWorker1.ReportProgress(CInt(50))
                Else
                    BackgroundWorker1.ReportProgress(CInt(100))
                End If

                If Not Common.isShowCompleteMessage Then
                    Common.isShowCompleteMessage = True
                    DeleteTmpFolder()
                    Dim msgStr As String = "Files have been uploaded. "
                    If ListView_OtherFiles.Items.Count > 0 Then
                        msgStr = msgStr + "Note that " + ListView_OtherFiles.Items.Count.ToString() + " file(s) failed the unzip check and still need to be uploaded to FTP. "
                    End If
                    If ListView_FileTrimmingFailed.Items.Count > 0 Then
                        msgStr = msgStr + "Note that " + ListView_FileTrimmingFailed.Items.Count.ToString() + " of the uploaded file(s) were corrupted. "
                    End If
                    If ListView_OtherFiles.Items.Count > 0 Or ListView_FileTrimmingFailed.Items.Count > 0 Then
                        MessageBox.Show(msgStr)
                    Else
                        If isUploadPcap And Not _isRunUploadPcap And RadioButton_SQZ_PCAP.Checked Then
                            Common.isShowCompleteMessage = False
                            Me.Invoke(New MethodInvoker(AddressOf Me.Timer_UploadPcap.Start))
                            _isRunUploadPcap = True
                        Else
                            Try
                                If _folderListToDelete.Count > 0 Then
                                    For i = 0 To _folderListToDelete.Count - 1
                                        Try
                                            Directory.Delete(_folderListToDelete(i), True)
                                        Catch ex As Exception

                                        End Try


                                    Next
                                    '_folderListToDelete = New List(Of String)
                                End If
                            Catch ex As Exception

                            End Try

                            MessageBox.Show("All files have been uploaded")

                            Threading.Thread.Sleep(2000)

                            If Directory.Exists(Common.currentTempGTPUnzip) Then
                                Dim fileToDelete As String() = Directory.GetFiles(Common.currentTempGTPUnzip)
                                For Each fileStr In fileToDelete
                                    Try
                                        File.Delete(fileStr)
                                    Catch ex As Exception

                                    End Try

                                Next
                            End If

                        End If

                    End If
                Else

                    If Directory.Exists(Common.currentTempGTPUnzip) Then
                        Dim fileToDelete As String() = Directory.GetFiles(Common.currentTempGTPUnzip)
                        For Each fileStr In fileToDelete
                            File.Delete(fileStr)
                        Next
                    End If

                End If

            End If
        Else

            Dim isReupload = False
            For i = 0 To _FileUpload.Length - 1
                If _FileUpload(i).Status = 0 Then
                    isReupload = True
                End If
            Next



            If isReupload Then

                UpdateControl_Sub(Label_Info, "Some files have failed to upload. GTP is trying to re-upload")
                id_transaction_reupload = id_FTP_Client
                If id_FTP_Transaction.ToString().Trim() = "" Then
                    'id_server_transaction_reupload = id
                Else
                    id_server_transaction_reupload = id_FTP_Transaction
                End If

                ReuploadProcess = 1
                Me.Invoke(New MethodInvoker(AddressOf Me.Timer_Upload.Start))

            End If

        End If

        If Common.hasWCFAccess And Not Common.forceXMLMode Then
            proxy.Close()
        End If

    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        'ProgressBar_FileProgress.Value = e.ProgressPercentage
        Try
            ProgressBar_Upload.Value = e.ProgressPercentage
        Catch ex As Exception

        End Try

    End Sub



    Delegate Sub EnableTimer_Delegate(ByVal [Timer1] As Timer, ByVal Status As Boolean)

    Private Sub EnableTimer_Sub(ByVal [Timer1] As Timer, ByVal Status As Boolean)

    End Sub

    Delegate Sub UpdateControl_Delegate(ByVal [Label_TotalFile] As Control, ByVal [text] As String)

    Private Sub UpdateControl_Sub(ByVal [Label_TotalFile1] As Control, ByVal [text] As String)
        If [Label_TotalFile1].InvokeRequired Then
            Dim myDelegate As New UpdateControl_Delegate(AddressOf UpdateControl_Sub)
            Me.Invoke(myDelegate, New Object() {[Label_TotalFile1], [text]})
        Else
            [Label_TotalFile1].Text = [text]
        End If
    End Sub

    Delegate Sub UpdateTextBoxLog_Delegate(ByVal [TextBox_FTPSessionLog] As TextBox, ByVal [text] As String)

    Private Sub UpdateTextBoxLog_Sub(ByVal [TextBox_FTPSessionLog] As TextBox, ByVal [text] As String)
        If [TextBox_FTPSessionLog].InvokeRequired Then
            Dim myDelegate As New UpdateTextBoxLog_Delegate(AddressOf UpdateTextBoxLog_Sub)
            Me.Invoke(myDelegate, New Object() {[TextBox_FTPSessionLog], [text]})
        Else
            [TextBox_FTPSessionLog].Text = [text]
        End If
    End Sub


    Delegate Sub UpdateListView_Delegate(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String, ByVal [Color] As Color)
    Private Sub UpdateListView_Sub(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String, ByVal [Color] As Color)
        Try
            If ListView_FileList1.InvokeRequired Then
                Dim myDelegate As New UpdateListView_Delegate(AddressOf UpdateListView_Sub)
                Me.Invoke(myDelegate, New Object() {[ListView_FileList1], [FileName], [Color]})
            Else
                Dim item As New ListViewItem
                item = ListView_FileList.FindItemWithText(FileName)
                item.ForeColor = [Color]
                ListView_FileList.Refresh()

            End If
        Catch ex As Exception

        End Try
    End Sub


    Delegate Sub RemoveListViewItem_Delegate(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String)

    Private Sub RemoveListViewItem(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String)
        Try
            If ListView_FileList1.InvokeRequired Then
                Dim myDelegate As New RemoveListViewItem_Delegate(AddressOf RemoveListViewItem)
                Me.Invoke(myDelegate, New Object() {[ListView_FileList1], [FileName]})
            Else
                Dim item As New ListViewItem
                item = ListView_FileList1.FindItemWithText(FileName)
                item.Remove()
                ListView_FileList1.Refresh()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Delegate Sub GetListViewItem_Delegate(ByVal [ListView_FileList1] As ListView)

    Private Sub GetListViewItem(ByVal [ListView_FileList1] As ListView)
        Try
            If ListView_FileList1.InvokeRequired Then
                Dim myDelegate As New GetListViewItem_Delegate(AddressOf GetListViewItem)
                Me.Invoke(myDelegate, New Object() {[ListView_FileList1]})
            Else
                Dim item As ListViewItem

                For Each item In ListView_FileList1.Items
                    currentListViewItem.Add(item.Text)
                Next

            End If
        Catch ex As Exception
            Dim tt As String = ""

        End Try
    End Sub

    Delegate Sub AddListViewItem_Delegate(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String)

    Private Sub AddListViewItem(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String)
        Try
            If ListView_FileList1.InvokeRequired Then
                Dim myDelegate As New AddListViewItem_Delegate(AddressOf AddListViewItem)
                Me.Invoke(myDelegate, New Object() {[ListView_FileList1], [FileName]})
            Else
                ListView_FileList1.Items.Add([FileName])
                ListView_FileList1.Refresh()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Delegate Sub AddListViewItem1_Delegate(ByVal [ListView_FileList1] As ListView, ByVal [item] As ListViewItem)

    Private Sub AddListViewItem1(ByVal [ListView_FileList1] As ListView, ByVal [item] As ListViewItem)
        Try
            If ListView_FileList1.InvokeRequired Then
                Dim myDelegate As New AddListViewItem1_Delegate(AddressOf AddListViewItem1)
                Me.Invoke(myDelegate, New Object() {[ListView_FileList1], [item]})
            Else
                ListView_FileList1.Items.Add(item)
                ListView_FileList1.Refresh()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Delegate Sub selectTabControlIndex_Delegate(ByVal tabControl As TabControl, ByVal index As Integer)

    Private Sub selectTabControlIndex(ByVal tabControl As TabControl, ByVal index As Integer)
        Try
            If tabControl.InvokeRequired Then
                Dim myDelegate As New selectTabControlIndex_Delegate(AddressOf selectTabControlIndex)
                Me.Invoke(myDelegate, New Object() {tabControl, index})
            Else
                tabControl.SelectedIndex = index
            End If
        Catch ex As Exception

        End Try
    End Sub



    Delegate Sub EnableButton_Delegate(ByVal [button] As Button, ByVal [status] As Boolean)

    Private Sub EnableButton_Sub(ByVal [button] As Button, ByVal status As Boolean)
        If [button].InvokeRequired Then
            Dim mydelegate As New EnableButton_Delegate(AddressOf EnableButton_Sub)
            Me.Invoke(mydelegate, New Object() {[button], [status]})
        Else
            button.Enabled = [status]
            button.Visible = [status]
        End If
    End Sub

    Delegate Sub EnableRadioButton_Delegate(ByVal [checkbox] As RadioButton, ByVal [status] As Boolean)

    Private Sub EnableRadioButton_Sub(ByVal [checkbox] As RadioButton, ByVal status As Boolean)
        If [checkbox].InvokeRequired Then
            Dim mydelegate As New EnableRadioButton_Delegate(AddressOf EnableRadioButton_Sub)
            Me.Invoke(mydelegate, New Object() {[checkbox], [status]})
        Else
            checkbox.Enabled = [status]
            checkbox.Visible = [status]
        End If
    End Sub


    Delegate Sub EnableGroupBox_Delegate(ByVal [checkbox] As GroupBox, ByVal [status] As Boolean)

    Private Sub EnableGroupBox_Sub(ByVal [checkbox] As GroupBox, ByVal status As Boolean)
        If [checkbox].InvokeRequired Then
            Dim mydelegate As New EnableGroupBox_Delegate(AddressOf EnableGroupBox_Sub)
            Me.Invoke(mydelegate, New Object() {[checkbox], [status]})
        Else
            checkbox.Enabled = [status]
            checkbox.Visible = [status]
        End If
    End Sub

    Delegate Sub SelectTabIndex_Delegate(ByVal [tab_control] As TabControl, ByVal [index] As Integer)

    Private Sub SelectTabIndex_Sub(ByVal [tab_control] As TabControl, ByVal [index] As Integer)
        If [tab_control].InvokeRequired Then
            Dim mydelegate As New SelectTabIndex_Delegate(AddressOf SelectTabIndex_Sub)
            Me.Invoke(mydelegate, New Object() {[tab_control], [index]})
        Else
            tab_control.SelectedIndex = index
        End If
    End Sub

    Delegate Sub UpdateProgressBar_Delegate(ByVal [ProgressBar] As ProgressBar, ByVal [percent] As Double)
    Private Sub UpdateProgressBar_Sub(ByVal [ProgressBar] As ProgressBar, ByVal [percent] As Double)
        Try
            If [ProgressBar].InvokeRequired Then
                Dim mydelegate As New UpdateProgressBar_Delegate(AddressOf UpdateProgressBar_Sub)
                Me.Invoke(mydelegate, New Object() {[ProgressBar], [percent]})
            Else
                [ProgressBar].Value = [percent]
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If uploadStatus = 1 Then
            'Timer1.Enabled = True
            'ResumeUpload("Timer1")
        End If
    End Sub

    Private Sub Timer_CheckConnection_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If Not CheckConnection() Then
        '    Timer1.Enabled = True
        '    ResumeUpload("Timer1")
        'Else
        '    Timer1.Enabled = False
        '    ResumeUpload("Timer1")
        'End If
    End Sub

    Private Sub BackgroundWorker_Resume_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)

    End Sub

    Private Sub BackgroundWorker_Resume_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs)

    End Sub

    Private Sub CheckFTPFolderExist()
        If Not _isSFTP Then
            'Check if GWSFTP is exist, if not create a new folder
            Dim success As Boolean
            Dim ftp As New Chilkat.Ftp2()
            ftp.Hostname = "upload.mygws.com"
            ftp.Username = TextBox_UserName.Text.Trim
            ftp.Password = TextBox_Password.Text.Trim
            success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
            If (success <> True) Then
                MessageBox.Show(ftp.LastErrorText)
            End If
            success = ftp.Connect()
            If (success <> True) Then
                MessageBox.Show(ftp.LastErrorText)
            End If
            Dim dirExists As Boolean
            dirExists = ftp.ChangeRemoteDir("/" + TextBox_UserName.Text.Trim + "_curUpload")
            If (dirExists <> True) Then
                'Create new directory
                success = ftp.CreateRemoteDir("/" + TextBox_UserName.Text.Trim + "_curUpload")
                If Not success Then
                    MessageBox.Show(ftp.LastErrorText)
                End If
            End If
            ftp.Disconnect()
        End If

    End Sub

    Private Sub UploadFile(ByVal FiletoUpload As String, ByVal remote_dir As String)

        If _isSFTP Then
            Dim classSFTP As New GWSSFTP()
            classSFTP.UploadFileGeneral(FiletoUpload, remote_dir)
        Else
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
        End If
    End Sub

    'Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    UploadFile("C:\GWSFTPLogs\log1.txt", "GTPLogs")
    'End Sub

    Private Sub Button3_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs)
        GetUserName()
    End Sub

    Private Sub Button_CheckInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_CheckInternet.Click
        If Common.CheckConnection("http://www.google.com") Then
            Label_InternetStatus.Text = "Internet is on. Upload will resume shortly"
        Else
            Label_InternetStatus.Text = "No Internet Connection"
        End If
    End Sub

    Private Sub Button_CloseTransaction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_CloseTransaction.Click
        CloseTransaction()
    End Sub

    Private Sub CloseTransaction()
        Dim id_local_transaction As String
        If Not Common.isXMLMode Then
            id_local_transaction = Label_id_local_transaction.Text
            Dim id_server_transaction As String = id_server_transaction_finished.ToString
            Dim proxy As New GTPServiceClient()
            Dim trans As _FTPTransaction
            trans = proxy.GetTransaction(id_server_transaction)

            Dim cnn As OleDbConnection
            cnn = New OleDbConnection()
            cnn.ConnectionString = cnnstr
            Dim cmd As OleDbCommand
            cmd = New OleDbCommand
            cnn.Open()

            Dim success As Boolean
            If trans.transaction_status = 0 Then
                Dim total_uploaded As Integer
                total_uploaded = proxy.countFile(id_server_transaction, "finished")
                success = proxy.ExecuteQuery("UPDATE tblFTpTransaction SET isCompleted=1, date_end = getdate(), transaction_status=1, total_file_uploaded=" + total_uploaded.ToString() + " WHERE id_server_transaction=" + id_server_transaction)
            ElseIf trans.transaction_status > 2 Then


            End If

            'Update local database            
            cmd.CommandText = "UPDATE tblTransactionInfo SET [isCompleted] = True, [date_end] = Date() WHERE id_transaction = " + id_local_transaction.ToString
            cmd.Connection = cnn

            cmd.ExecuteNonQuery()
            cnn.Close()
            proxy.Close()
        Else
            'Update xml file
            Try
                If Not Common.hasWCFAccess Or (Common.forceXMLMode Or Common.isXMLMode) Then
                    Dim FTPElement As XmlNode
                    Dim xmlDoc As New XmlDocument
                    xmlDoc.Load(XMLFileName)


                    FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")

                    If Not IsNothing(FTPElement) Then
                        FTPElement.Attributes("transaction_status").Value = "1"
                        'FTPElement.Attributes("date_end").Value = DateTime.Now.ToString()
                        FTPElement.Attributes("date_end").Value = Common.getDateString(Common.GetCurrentEasternTime())
                        FTPElement.Attributes("isCompleted").Value = "True"
                    End If
                    xmlDoc.Save(XMLFileName)
                    'Upload file to server
                    File.Copy(XMLFileName, XMLFileName.Replace(".xml", "_backup.xml"), True)
                    UploadFile(XMLFileName, Common.currentXMLFolder)
                End If
            Catch ex As Exception

            End Try

            Dim cnn As OleDbConnection
            cnn = New OleDbConnection()
            cnn.ConnectionString = cnnstr
            Dim cmd As OleDbCommand
            cmd = New OleDbCommand
            cnn.Open()

            'Update local database            
            cmd.CommandText = "UPDATE tblTransactionInfo SET [isCompleted] = True, [date_end] = Date() WHERE TransactionGID = '" + TransactionGID.ToString() + "'"
            cmd.Connection = cnn

            cmd.ExecuteNonQuery()
            cnn.Close()
        End If



        MessageBox.Show("Transaction has been closed")
        Me.Close()

        'Dim uploadForm As New frmUpload()
        'uploadForm.MdiParent = frmMainApp

        'uploadForm.Show()

    End Sub

    Private Sub RadioButton_Continue_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_Continue.CheckedChanged
        If transaction_status_server = 0 Then
            Button_Upload.Visible = True
            Button_DeleteUploadedFiles.Visible = True
            Button_DeleteUploadedFiles.Enabled = True
            Button2.Visible = False
            Button_CloseTransaction.Visible = False

        Else
            Button_Upload.Visible = True
            Button_DeleteUploadedFiles.Visible = False
            Button_DeleteUploadedFiles.Enabled = True
            Button2.Visible = True
            Button_CloseTransaction.Visible = False


        End If

        ToolTip_Delete.SetToolTip(Button_DeleteUploadedFiles, "Please select files to delete. If you want to delete the whole transaction, select all the files ")
        ToolTip_CLose.SetToolTip(Button_CloseTransaction, "Close the transaction")

        For Each lvi In ListView_FileList.Items
            lvi.Selected = False
        Next
        ListView_FileList.Enabled = True



    End Sub

    Private Sub RadioButton_Delete_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_Delete.CheckedChanged
        Button_Upload.Visible = False
        Button_DeleteUploadedFiles.Visible = True
        Button_DeleteUploadedFiles.Enabled = True
        Button2.Visible = False
        Button_CloseTransaction.Visible = False

        ToolTip_Delete.SetToolTip(Button_DeleteUploadedFiles, "Please select files to delete. If you want to delete the whole transaction, select all the files ")
        ToolTip_CLose.SetToolTip(Button_CloseTransaction, "Close the transaction")
        Dim lvi As ListViewItem

        If RadioButton_Delete.Checked Then
            Button_DeleteUploadedFiles.Text = "Delete All Files"
        Else
            Button_DeleteUploadedFiles.Text = "Delete Selected Files"
        End If

        For Each lvi In ListView_FileList.Items
            lvi.Selected = True
        Next
        ListView_FileList.Enabled = False

    End Sub

    Private Sub RadioButton_Close_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_Close.CheckedChanged
        Button_Upload.Visible = False
        Button_DeleteUploadedFiles.Visible = False
        Button_DeleteUploadedFiles.Enabled = True
        Button2.Visible = False
        Button_CloseTransaction.Visible = True

        ToolTip_Delete.SetToolTip(Button_DeleteUploadedFiles, "Please select files to delete. If you want to delete the whole transaction, select all the files ")
        ToolTip_CLose.SetToolTip(Button_CloseTransaction, "Close the transaction")

        For Each lvi In ListView_FileList.Items
            lvi.Selected = False
        Next
        ListView_FileList.Enabled = True

    End Sub

    Private Function getFileTrimmingInfo(ByVal FileName As String) As FileTrimming.FileTrimmingInfo
        Dim fileTrimmingObj As New FileTrimming()
        Dim fileInfoObj As New FileTrimming.FileTrimmingInfo()
        If FileName.Trim().ToLower.EndsWith(".sqz") Then
            fileInfoObj = fileTrimmingObj.processSQZ(FileName.Trim())
        ElseIf FileName.Trim().ToLower.EndsWith(".mf") Then
            fileInfoObj = fileTrimmingObj.processMFFile(FileName.Trim(), True)
        End If
        Return fileInfoObj
    End Function

    Private Function GWSUploadAsyn(ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer, ByVal LocalFileSize As Double, ByVal process As Integer) As UploadResult
        Dim uploadFileResult As New UploadResult()
        Common.FileUploadStartTime = DateTime.Now

        Dim fileTrimming As FileTrimming.FileTrimmingInfo = Nothing
        If FiletoUpload.ToLower().EndsWith(".sqz") Or FiletoUpload.ToLower().EndsWith(".mf") Then
            fileTrimming = getFileTrimmingInfo(FiletoUpload)
            uploadFileResult.fileTrimmingResult = fileTrimming
            If Not IsNothing(fileTrimming.FileName) And fileTrimming.isCorrupted = False Then
                FiletoUpload = fileTrimming.FileName
            Else
                addTrimmingFailed(FiletoUpload)
                'Return uploadFileResult
            End If
        End If

        Dim proxy As New GTPServiceClient
        Dim success As Boolean
        Dim finishedCode As Integer = 2
        Dim ftp As New Chilkat.Ftp2()

        Dim fileSize As Long
        Dim fileSize_Local As Long
        Dim fileSize_Server As Long
        Dim FileUploadInfo As FileInfo
        FileUploadInfo = New FileInfo(FiletoUpload)
        fileSize_Local = FileUploadInfo.Length

        'Check if the file is on server and status in database
        If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
            Dim file_status As Integer = 0
            file_status = proxy.GetFileStatus(id_transaction, FiletoUpload)
            If file_status = 1 Then
                'Check file exist on server
                fileSize = ftp.GetSizeByName64(FiletoUpload)
                If (fileSize > 0) Then
                    If FileUploadInfo.Length = fileSize Then
                        finishedCode = -1
                        proxy.Close()
                        uploadFileResult.uploadStatus = finishedCode
                        Return uploadFileResult
                    End If
                End If
            End If
            proxy.Close()
        End If


        If Common._isSFTP Then
            Dim sFTPClass As New GWSSFTP()
            sFTPClass.UploadFile(FiletoUpload, ftp_url, id_transaction, LocalFileSize, LocalFileSize)
            finishedCode = 0
        Else

            ftp.Hostname = Common.FTPServerName
            ftp.Username = Common.FTPUserName
            ftp.Password = Common.FTPPass

            success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
            If (success <> True) Then

                uploadFileResult.uploadStatus = 1
                Return uploadFileResult
            End If
            ftp.KeepSessionLog = True

            ftp.Passive = True

            Try
                Dim FileName As String
                FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
                'Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)

                Dim remote_dir = ftp_url.Substring(ftp_url.LastIndexOf("/") + 1, ftp_url.Length - ftp_url.LastIndexOf("/") - 1)
                remote_dir = Common.FTPUserName + "_curUpload/" + remote_dir

                success = ftp.Connect()
                If (success <> True) Then
                    Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                    If Not isConnected Then
                        finishedCode = 1
                    Else
                        finishedCode = 2
                    End If
                    uploadFileResult.uploadStatus = finishedCode
                    Return uploadFileResult
                End If

                success = ftp.ChangeRemoteDir(remote_dir)
                If (success <> True) Then
                    Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                    If Not isConnected Then
                        finishedCode = 1
                    Else
                        finishedCode = 2
                    End If
                    uploadFileResult.uploadStatus = finishedCode
                    Return uploadFileResult
                End If



                ftp.ConnectTimeout = 60 * 1000 'Time out in second
                ftp.ReadTimeout = 60 * 3000 'Time out in second

                success = ftp.AsyncPutFileStart(FiletoUpload, FileName)


                'Dim aa As Boolean
                'aa = ftp.AsyncFinished

                If (Not success) Then
                    'MessageBox.Show(ftp.LastErrorText)
                    uploadFileResult.uploadStatus = 1
                    Return uploadFileResult
                End If

                ' This is where your application might do other interesting tasks...
                ' This example will loop and update the current upload rate and bytes
                ' transferred.
                While Not ftp.AsyncFinished
                    System.Threading.Thread.Sleep(100)
                    ' Handle application events so our user interface remains responsive.
                    Application.DoEvents()
                    ' Update label controls with the current data rate and # bytes downloaded.

                    If process = 1 Then
                        UpdateControl_Sub(Label_UploadSpeed, "Upload Speed (KB/Second): " + Convert.ToString(Math.Round(ftp.UploadRate / 1024, 2)))
                        UpdateProgressBar_Sub(ProgressBar_FileProgress, (ftp.AsyncBytesSent / LocalFileSize) * 100)
                    ElseIf process = 2 Then
                        UpdateControl_Sub(Label_UploadSpeed1, "Upload Speed (KB/Second): " + Convert.ToString(Math.Round(ftp.UploadRate / 1024, 2)))
                        UpdateProgressBar_Sub(ProgressBar_FileProgress1, (ftp.AsyncBytesSent / LocalFileSize) * 100)
                    End If

                    If Not ftp.IsConnected Then
                        If Not CheckConnection("http://www.google.com") Then
                            finishedCode = 1
                            uploadFileResult.uploadStatus = finishedCode
                            Return uploadFileResult
                        End If
                    End If

                End While

                If Not ftp.IsConnected Then
                    If Not CheckConnection("http://www.google.com") Then
                        finishedCode = 1
                        uploadFileResult.uploadStatus = finishedCode
                        Return uploadFileResult
                    End If

                End If

                ' The upload is finished, now check the success:
                If Not ftp.AsyncSuccess Then
                    If Not CheckConnection("http://www.google.com") Then
                        finishedCode = 1
                        uploadFileResult.uploadStatus = finishedCode
                        Return uploadFileResult
                    End If
                Else
                    fileSize_Server = ftp.GetSizeByName64(FileName)
                    If fileSize_Local <> fileSize_Server Then
                        finishedCode = 2
                    Else
                        finishedCode = 0
                    End If
                End If
                FTPSessionLog += ftp.SessionLog


                If (success <> True) Then
                    Try
                        If CheckConnection("http://www.google.com") Then
                            finishedCode = GWSUploadOneFileNETAsyn(FiletoUpload, ftp_url, id_transaction)
                        Else

                            finishedCode = 1
                            uploadFileResult.uploadStatus = finishedCode
                            Return uploadFileResult
                        End If

                    Catch ex As Exception

                    End Try

                    If finishedCode <> 0 Then
                        Try
                            Dim cnn As OleDbConnection
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
                            uploadFileResult.uploadStatus = finishedCode
                            Return uploadFileResult
                        Else
                            finishedCode = 2
                        End If
                    Else
                        'Compare Filesize

                        FileUploadInfo = New FileInfo(FiletoUpload)
                        fileSize_Local = FileUploadInfo.Length
                        fileSize_Server = ftp.GetSizeByName64(FileName)

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
        End If



        Common.FileUploadEndTime = DateTime.Now

        Try
            If Not IsNothing(fileTrimming) Then
                If Not IsNothing(fileTrimming.TempFolder) Then
                    If fileTrimming.TempFolder.Trim <> "" Then
                        Directory.Delete(fileTrimming.TempFolder, True)
                    End If
                End If
                If fileTrimming.isNetworkFile Then
                    File.Delete(fileTrimming.FileName)
                End If
            End If
        Catch ex As Exception
            Dim fileName As String = AppDomain.CurrentDomain.BaseDirectory + "DirToDelete.txt"
            If Not File.Exists(fileName) Then
                File.CreateText(fileName).Close()
            End If
            Using sw As StreamWriter = File.AppendText(fileName)
                sw.WriteLine(fileTrimming.TempFolder)
            End Using
        End Try

        uploadFileResult.uploadStatus = finishedCode
        Return uploadFileResult

    End Function



    Private Function GWSUploadAsyn2(ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer, ByVal LocalFileSize As Double, ByVal process As Integer) As UploadResult
        Dim uploadFileResult As New UploadResult()

        Common.FileUploadStartTime1 = DateTime.Now

        Dim fileTrimming As FileTrimming.FileTrimmingInfo = Nothing
        If FiletoUpload.ToLower().EndsWith(".sqz") Or FiletoUpload.ToLower().EndsWith(".mf") Then
            fileTrimming = getFileTrimmingInfo(FiletoUpload)
            uploadFileResult.fileTrimmingResult = fileTrimming
            If Not IsNothing(fileTrimming.FileName) And fileTrimming.isCorrupted = False Then
                FiletoUpload = fileTrimming.FileName
            Else
                addTrimmingFailed(FiletoUpload)
                'Return uploadFileResult
            End If
        End If

        Dim proxy As New GTPServiceClient
        Dim success As Boolean
        Dim finishedCode As Integer = 2
        Dim ftp As New Chilkat.Ftp2()

        ftp.Hostname = Common.FTPServerName
        ftp.Username = Common.FTPUserName
        ftp.Password = Common.FTPPass

        Dim fileSize As Long
        Dim fileSize_Local As Long
        Dim fileSize_Server As Long
        Dim FileUploadInfo As FileInfo
        FileUploadInfo = New FileInfo(FiletoUpload)
        fileSize_Local = FileUploadInfo.Length

        'Check if the file is on server and status in database
        If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
            Dim file_status As Integer = 0
            file_status = proxy.GetFileStatus(id_transaction, FiletoUpload)
            If file_status = 1 Then
                'Check file exist on server
                fileSize = ftp.GetSizeByName64(FiletoUpload)
                If (fileSize > 0) Then
                    If FileUploadInfo.Length = fileSize Then
                        finishedCode = -1
                        proxy.Close()
                        uploadFileResult.uploadStatus = finishedCode
                        Return uploadFileResult
                    End If
                End If
            End If
            proxy.Close()
        End If


        If Common._isSFTP Then
            Dim sFTPClass As New GWSSFTP()
            sFTPClass.UploadFile(FiletoUpload, ftp_url, id_transaction, LocalFileSize, LocalFileSize)
            finishedCode = 0
        Else

            ftp.Hostname = Common.FTPServerName
            ftp.Username = Common.FTPUserName
            ftp.Password = Common.FTPPass

            success = ftp.UnlockComponent("cqcmgfFTP_bmMUBvCckRnr")
            If (success <> True) Then

                uploadFileResult.uploadStatus = 1
                Return uploadFileResult
            End If
            ftp.KeepSessionLog = True

            ftp.Passive = True

            Try
                Dim FileName As String
                FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
                'Dim bFile() As Byte = System.IO.File.ReadAllBytes(FiletoUpload)

                Dim remote_dir = ftp_url.Substring(ftp_url.LastIndexOf("/") + 1, ftp_url.Length - ftp_url.LastIndexOf("/") - 1)
                remote_dir = Common.FTPUserName + "_curUpload/" + remote_dir

                success = ftp.Connect()
                If (success <> True) Then
                    Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                    If Not isConnected Then
                        finishedCode = 1
                    Else
                        finishedCode = 2
                    End If
                    uploadFileResult.uploadStatus = finishedCode
                    Return uploadFileResult
                End If

                success = ftp.ChangeRemoteDir(remote_dir)
                If (success <> True) Then
                    Dim isConnected As Boolean = CheckConnection("http://www.google.com")
                    If Not isConnected Then
                        finishedCode = 1
                    Else
                        finishedCode = 2
                    End If
                    uploadFileResult.uploadStatus = finishedCode
                    Return uploadFileResult
                End If



                ftp.ConnectTimeout = 60 * 1000 'Time out in second
                ftp.ReadTimeout = 60 * 3000 'Time out in second

                success = ftp.AsyncPutFileStart(FiletoUpload, FileName)


                'Dim aa As Boolean
                'aa = ftp.AsyncFinished

                If (Not success) Then
                    'MessageBox.Show(ftp.LastErrorText)
                    uploadFileResult.uploadStatus = 1
                    Return uploadFileResult
                End If

                ' This is where your application might do other interesting tasks...
                ' This example will loop and update the current upload rate and bytes
                ' transferred.
                While Not ftp.AsyncFinished
                    System.Threading.Thread.Sleep(100)
                    ' Handle application events so our user interface remains responsive.
                    Application.DoEvents()
                    ' Update label controls with the current data rate and # bytes downloaded.

                    If process = 1 Then
                        UpdateControl_Sub(Label_UploadSpeed, "Upload Speed (KB/Second): " + Convert.ToString(Math.Round(ftp.UploadRate / 1024, 2)))
                        UpdateProgressBar_Sub(ProgressBar_FileProgress, (ftp.AsyncBytesSent / LocalFileSize) * 100)
                    ElseIf process = 2 Then
                        UpdateControl_Sub(Label_UploadSpeed1, "Upload Speed (KB/Second): " + Convert.ToString(Math.Round(ftp.UploadRate / 1024, 2)))
                        UpdateProgressBar_Sub(ProgressBar_FileProgress1, (ftp.AsyncBytesSent / LocalFileSize) * 100)
                    End If

                    If Not ftp.IsConnected Then
                        If Not CheckConnection("http://www.google.com") Then
                            finishedCode = 1
                            uploadFileResult.uploadStatus = finishedCode
                            Return uploadFileResult
                        End If
                    End If

                End While

                If Not ftp.IsConnected Then
                    If Not CheckConnection("http://www.google.com") Then
                        finishedCode = 1
                        uploadFileResult.uploadStatus = finishedCode
                        Return uploadFileResult
                    End If

                End If

                ' The upload is finished, now check the success:
                If Not ftp.AsyncSuccess Then
                    If Not CheckConnection("http://www.google.com") Then
                        finishedCode = 1
                        uploadFileResult.uploadStatus = finishedCode
                        Return uploadFileResult
                    End If
                Else
                    fileSize_Server = ftp.GetSizeByName64(FileName)
                    If fileSize_Local <> fileSize_Server Then
                        finishedCode = 2
                    Else
                        finishedCode = 0
                    End If
                End If
                FTPSessionLog += ftp.SessionLog


                If (success <> True) Then
                    Try
                        If CheckConnection("http://www.google.com") Then
                            finishedCode = GWSUploadOneFileNETAsyn(FiletoUpload, ftp_url, id_transaction)
                        Else

                            finishedCode = 1
                            uploadFileResult.uploadStatus = finishedCode
                            Return uploadFileResult
                        End If

                    Catch ex As Exception

                    End Try

                    If finishedCode <> 0 Then
                        Try
                            Dim cnn As OleDbConnection
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
                            uploadFileResult.uploadStatus = finishedCode
                            Return uploadFileResult
                        Else
                            finishedCode = 2
                        End If
                    Else
                        'Compare Filesize

                        FileUploadInfo = New FileInfo(FiletoUpload)
                        fileSize_Local = FileUploadInfo.Length
                        fileSize_Server = ftp.GetSizeByName64(FileName)

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
        End If


        Common.FileUploadEndTime1 = DateTime.Now
        Try
            If Not IsNothing(fileTrimming) Then
                If Not IsNothing(fileTrimming.TempFolder) Then
                    If fileTrimming.TempFolder.Trim <> "" Then
                        Directory.Delete(fileTrimming.TempFolder, True)
                    End If
                End If
                If fileTrimming.isNetworkFile Then
                    File.Delete(fileTrimming.FileName)
                End If
            End If
        Catch ex As Exception
            Dim fileName As String = AppDomain.CurrentDomain.BaseDirectory + "DirToDelete.txt"
            If Not File.Exists(fileName) Then
                File.CreateText(fileName).Close()
            End If
            Using sw As StreamWriter = File.AppendText(fileName)
                sw.WriteLine(fileTrimming.TempFolder)
            End Using
        End Try


        uploadFileResult.uploadStatus = finishedCode
        Return uploadFileResult
    End Function


    Public Function GWSUploadOneFileNETAsyn(ByVal FiletoUpload As String, ByVal ftp_url As String, ByVal id_transaction As Integer) As Integer
        Dim finishedCode As Integer = 0
        Dim FileName As String
        Dim clsRequest As System.Net.FtpWebRequest
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        Try
            FileName = FiletoUpload.Substring(FiletoUpload.LastIndexOf("\") + 1, FiletoUpload.Length - FiletoUpload.LastIndexOf("\") - 1)
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileName), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(Common.FTPUserName, Common.FTPPass)
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


    Private Sub BackgroundWorker2_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        If id_FTP_Transaction.ToString = "0" And Not (Not Common.hasWCFAccess Or Common.forceXMLMode) Then
            Return
        End If
        Dim FiletoUpload As String


        id_client_public = id_FTP_Client

        Dim proxy As GTPServiceClient
        If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
            proxy = New GTPServiceClient()
            Common.AssignCredentials(proxy)
        End If


        Dim cmd As OleDbCommand
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr

        Dim i As Integer
        Dim ii As Integer
        Dim isUploaded As Boolean
        Dim isUploading As Boolean

        Dim isFileExist As Boolean

        Dim uploadRate As Integer = 0

        For i = 0 To _FileUpload.Length - 1

            If BackgroundWorker2.CancellationPending Then
                e.Cancel = True
                Common.isWorker2Stop = True
                If Common.isWorker1Stop Then

                End If
                Return
            End If


            isUploaded = False
            isUploading = False
            isFileExist = True



            If _FileUpload(i).Status = 1 Then
                isUploaded = True
            End If

            If _FileUpload(i).Status = 2 Then
                isUploading = True
            End If
            If Not File.Exists(_FileUpload(i).FileName) Then

                If Common.hasWCFAccess And Not (Common.forceXMLMode Or Common.isXMLMode) Then
                    Dim status As Integer = proxy.GetFileStatus(id_FTP_Transaction, _FileUpload(i).FileName)
                    If Not (status = 1 Or status >= 3) Then
                        If _FileUpload(i).FileName <> Nothing Then
                            proxy.UpdatetbFTPFile(id_FTP_Transaction, _FileUpload(i).FileName, -5)
                        End If

                    End If

                End If

                isFileExist = False
                _FileUpload(i).Status = -5
                RemoveListViewItem(ListView_FileList, _FileUpload(i).FileName)
                AddListViewItem(ListView_OtherFiles, _FileUpload(i).FileName)
            Else
                isFileExist = True
            End If


            If (Not isUploaded) And (Not isUploading) And isFileExist Then
                If _FileUpload(i).Status = 0 Then
                    _FileUpload(i).Status = 2
                    isUploading = True
                    FiletoUpload = _FileUpload(i).FileName

                    'uploadStatus = GWSFTP.GWSUploadOneFile(uploadRate, FiletoUpload, FTPURL, id_FTP_Transaction)
                    Dim FileInfo = New FileInfo(FiletoUpload)
                    UpdateControl_Sub(Label_CurrentFile1, "File " + FileInfo.Name + " progress :")
                    If (id_FTP_Transaction = "") Then
                        id_FTP_Transaction = -1
                    End If
                    Dim uploadFileResult As UploadResult = GWSUploadAsyn2(FiletoUpload, FTPURL, Convert.ToInt32(id_FTP_Transaction), FileInfo.Length, 2)
                    uploadStatus = uploadFileResult.uploadStatus
                    If IsNothing(uploadFileResult.fileTrimmingResult.FileName) And (FiletoUpload.ToLower.EndsWith(".mf") Or FiletoUpload.ToLower.EndsWith(".sqz")) Then
                        _FileUpload(i).Status = 9 'Unzip Failed
                        uploadFileResult.fileTrimmingResult.isScanner = False
                        'proxy.UpdatetbFTPFile(id_FTP_Transaction, _FileUpload(i).FileName, -10)
                        'Continue For                        
                    End If

                    If Not IsNothing(uploadFileResult.fileTrimmingResult) And (FiletoUpload.ToLower.EndsWith("a.mf") Or FiletoUpload.ToLower.EndsWith("a.sqz")) Then
                        Dim BSideDevice As String = ""

                        If uploadFileResult.fileTrimmingResult.BSideFileType = "Mobile" Then
                            BSideDevice = "Mobile"
                        End If
                        Dim proxyBSide As New GTPServiceClient()
                        Dim queryString As String = "UPDATE tblFTPFile SET BsideDeviceType ='" + BSideDevice + "', isAsideOrphan = " + uploadFileResult.fileTrimmingResult.isAsideOrphan.ToString() + " WHERE id_server_transaction = " + id_FTP_Transaction + " AND file_name = '" + FiletoUpload + "'"
                        proxyBSide.ExecuteQuery(queryString)

                        proxyBSide.Close()
                    End If

                    UpdateTextBoxLog_Sub(TextBox_FTPSessionLog, FTPSessionLog)


                    Try
                        If File.Exists(log_file) Then
                            Using sw As StreamWriter = New StreamWriter(log_file)
                                sw.Write(FTPSessionLog)
                            End Using
                        End If

                    Catch ex As Exception

                    End Try

                    Dim count11 As Integer
                    Dim uploadCode As Integer = uploadStatus

                    If uploadCode = 0 Or uploadCode = -1 Then
                        If _FileUpload(i).Status <> 9 Then
                            _FileUpload(i).Status = 1
                        End If
                        count_uploaded_files += 1
                        total_uploaded_file += 1
                        count_uploaded += 1


                        If Not Common.hasWCFAccess Or Common.forceXMLMode Then
                            'Update XML File
                            Try

                                Dim xmlDOC As New XmlDocument
                                xmlDOC.Load(Common.currentXMLFile)
                                Dim node As XmlNode
                                node = xmlDOC.SelectSingleNode("/tblFTPTransaction/tblFTPFile[@file_name='" + FiletoUpload + "']")
                                If Not IsNothing(node) Then
                                    node.Attributes("is_finished").Value = "True"
                                    'node.Attributes("date_finished").Value = DateTime.Now.ToString()
                                    node.Attributes("date_finished").Value = Common.getDateString(Common.GetCurrentEasternTime())
                                    If _FileUpload(i).Status = 9 Then
                                        node.Attributes("file_status").Value = "1"
                                        node.Attributes("status_flag").Value = "9"
                                    Else
                                        node.Attributes("file_status").Value = _FileUpload(i).Status.ToString()
                                    End If

                                    If Not IsNothing(uploadFileResult.fileTrimmingResult) And (FiletoUpload.ToLower.EndsWith("a.mf") Or FiletoUpload.ToLower.EndsWith("a.sqz")) Then

                                        If uploadFileResult.fileTrimmingResult.BSideFileType = "Mobile" Then
                                            node.Attributes("BSideDeviceType").Value = "Mobile"
                                        Else
                                            node.Attributes("BSideDeviceType").Value = ""
                                        End If
                                    End If

                                    node.Attributes("isScanner").Value = uploadFileResult.fileTrimmingResult.isScanner.ToString()
                                    node.Attributes("isAsideOrphan").Value = uploadFileResult.fileTrimmingResult.isAsideOrphan.ToString()

                                End If

                                If uploadCode = 0 Then
                                    Dim tt As Integer = (Common.FileUploadEndTime1 - Common.FileUploadStartTime1).TotalMilliseconds
                                    node.Attributes("uploadtime").Value = tt.ToString()
                                End If


                                Dim TransNode As XmlNode
                                TransNode = xmlDOC.SelectSingleNode("/tblFTPTransaction/Transaction")
                                TransNode.Attributes("total_file_uploaded").Value = total_uploaded_file


                                xmlDOC.Save(Common.currentXMLFile)
                                File.Copy(Common.currentXMLFile, Common.currentXMLFile.Replace(".xml", "_backup.xml"), True)
                                UploadFile(Common.currentXMLFile, Common.currentXMLFolder)

                            Catch ex As Exception

                            End Try

                        End If

                        'File uploaded sucessful

                        Dim file_uploaded_server = 0
                        Dim status As Integer = 1
                        Dim total_file = ListView_FileList.Items.Count

                        If IsNothing(uploadFileResult.fileTrimmingResult.FileName) And (FiletoUpload.ToLower.EndsWith(".mf") Or FiletoUpload.ToLower.EndsWith(".sqz")) Then
                            status = 9
                        End If

                        Dim isSuccess As Boolean

                        If Common.hasWCFAccess And Not Common.forceXMLMode And Not Common.isXMLMode Then
                            If uploadCode = 0 Or uploadCode = -1 Then
                                isSuccess = proxy.UpdatetbFTPFile(Convert.ToInt32(id_FTP_Transaction), FiletoUpload, status)

                                'Update isScanner status to server
                                Dim proxyGTP As New GTPServiceClient()
                                proxyGTP.Open()
                                proxyGTP.UpdateFileScannerStatus(Convert.ToInt32(id_FTP_Transaction), FiletoUpload, uploadFileResult.fileTrimmingResult.isScanner)
                                proxyGTP.Close()


                                Dim tt As Double = (Common.FileUploadEndTime1 - Common.FileUploadStartTime1).TotalMilliseconds
                                proxy.ExecuteQuery("UPDATE tblFTPFile  SET uploadtime = " + tt.ToString() + " WHERE id_server_transaction=" + id_FTP_Transaction.ToString() + " AND file_name='" + FiletoUpload + "'")
                                file_uploaded_server = proxy.countFile(id_FTP_Transaction, "finished")
                            End If
                            If file_uploaded_server < total_file Then
                                BackgroundWorker2.ReportProgress(CInt((file_uploaded_server) / total_file * 100))
                            End If
                            'proxy.updateTotalUploaded(id_FTP_Transaction, file_uploaded_server)
                            If (ListView_FileList.Items.Count > 0) And (file_uploaded_server <= ListView_FileList.Items.Count) Then
                                If file_uploaded_server < ListView_FileList.Items.Count Then
                                    UpdateControl_Sub(Label_TotalFile, (file_uploaded_server).ToString() + " file(s) uploaded of total " + ListView_FileList.Items.Count.ToString() + " file(s) ")
                                End If
                            End If


                        Else
                            Try
                                If total_uploaded_file < total_file Then
                                    BackgroundWorker2.ReportProgress(CInt((total_uploaded_file) / total_file * 100))
                                End If
                            Catch ex As Exception

                            End Try

                            If (ListView_FileList.Items.Count > 0) And (total_uploaded_file <= ListView_FileList.Items.Count) Then
                                If total_uploaded_file < ListView_FileList.Items.Count Then
                                    UpdateControl_Sub(Label_TotalFile, (total_uploaded_file).ToString() + " file(s) uploaded of total " + ListView_FileList.Items.Count.ToString() + " file(s) ")
                                End If
                            End If


                        End If

                        If (Not (Common.hasWCFAccess And Not Common.forceXMLMode And Not Common.isXMLMode)) Or isSuccess Then
                            UpdateListView_Sub(ListView_FileList, FiletoUpload, Color.Green)
                            'Update tblFileInfo
                            cmd = New OleDbCommand
                            cmd.Connection = cnn
                            If cnn.State <> ConnectionState.Open Then
                                cnn.Open()
                            End If
                            cmd.CommandText = "UPDATE tblFileInfo SET [is_finished] = TRUE, [date_finished]=Date() WHERE id_transaction=" & id_FTP_Client & " AND file_name ='" & FiletoUpload & "'"
                            cmd.ExecuteNonQuery()
                            cmd.Dispose()
                            cnn.Close()
                        End If
                    ElseIf uploadCode = 1 Then
                        _FileUpload(i).Status = 0
                        UpdateControl_Sub(Label_Info, "Internet connection has been lost. Upload transaction will be resumed when Internet is available ")

                        If BackgroundWorker1.IsBusy Then
                            'If it supports cancellation, Cancel It
                            If BackgroundWorker1.WorkerSupportsCancellation Then
                                ' Tell the Background Worker to stop working.
                                BackgroundWorker1.CancelAsync()
                            End If
                        End If

                        'Timer1.Enabled = True
                        Me.Invoke(New MethodInvoker(AddressOf Me.Timer1.Start))

                    ElseIf uploadCode = 2 Then
                        _FileUpload(i).Status = 0
                        Try
                            'Try to delete file    
                            System.Threading.Thread.Sleep(1500)
                            Dim remote_dir = FTPURL.Substring(FTPURL.LastIndexOf("/") + 1, FTPURL.Length - FTPURL.LastIndexOf("/") - 1)
                            remote_dir = Common.FTPUserName + "_curUpload/" + remote_dir
                            Common.DeleteFileonFTP(FiletoUpload, remote_dir)
                        Catch ex As Exception

                        End Try

                        'Unknow error or timeout - need to re-upload file
                        UpdateListView_Sub(ListView_FileList, FiletoUpload, Color.Red)
                        UpdateControl_Sub(Label_Info, "File: " + FiletoUpload + " was failed to upload.")
                        cmd = New OleDbCommand
                        cmd.Connection = cnn
                        If cnn.State <> ConnectionState.Open Then
                            cnn.Open()
                        End If
                        cmd.CommandText = "UPDATE tblFileInfo SET file_status=-1  WHERE id_transaction=" & id_FTP_Client & " AND file_name ='" & FiletoUpload.Replace("'", "''") & "'"
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                        cnn.Close()

                        'Update server
                        If Common.hasWCFAccess And Not Common.forceXMLMode Then
                            'Update server
                            proxy.UpdatetbFTPFile(Convert.ToInt32(id_FTP_Transaction), FiletoUpload, -1)

                        End If

                        total_file_failed += 1

                        'Update total file uploaded on server
                        If Common.hasWCFAccess And Not Common.forceXMLMode Then
                            Try
                                Dim file_uploaded_server = proxy.countFile(id_FTP_Transaction, "finished")
                            Catch ex As Exception

                            End Try

                        End If



                    End If

                    If uploadCode = 1 Then
                        cmd = New OleDbCommand
                        cmd.Connection = cnn
                        If cnn.State <> ConnectionState.Open Then
                            cnn.Open()
                        End If
                        cmd.CommandText = "UPDATE tblTransactionInfo SET [is_network_lost] = TRUE WHERE id_transaction=" & id_FTP_Client
                        cmd.ExecuteNonQuery()
                        cmd.Dispose()
                        cnn.Close()
                        Exit Sub
                    End If

                End If
            End If
        Next

        Dim _TotalUploaded As Integer = 0
        Dim _TotalRemoved As Integer = 0

        For i = 0 To _FileUpload.Length - 1
            If _FileUpload(i).Status = 1 Or _FileUpload(i).Status = 9 Then
                _TotalUploaded += 1
            End If
            If _FileUpload(i).Status < 0 Then
                _TotalRemoved += 1
            End If
        Next


        If _TotalUploaded = _FileUpload.Length - _TotalRemoved Then
            If id_FTP_Transaction.Trim() <> "" Then
                id_server_transaction_finished = id_FTP_Transaction
            End If

            id_transaction_finished = id_FTP_Client
            cmd = New OleDbCommand
            cmd.CommandText = "UPDATE tblTransactionInfo SET [isCompleted] = True, [date_end] = Date() WHERE id_transaction = " + id_FTP_Client.ToString
            cmd.Connection = cnn
            If cnn.State <> ConnectionState.Open Then
                cnn.Open()
            End If

            cmd.ExecuteNonQuery()
            'Update to server that the transaction has been completed
            Dim isCompleted As Boolean = True

            Try
                Me.Invoke(New MethodInvoker(AddressOf Me.Timer_Upload.Stop))

            Catch ex As Exception
                Try
                    Timer_Upload.Stop()
                Catch ex1 As Exception

                End Try
            End Try

            If Common.hasWCFAccess And Not Common.forceXMLMode Then

                proxy.completeTransaction(id_FTP_Transaction)

            End If


            If isCompleted Then


                If Not Common.hasWCFAccess Or Common.forceXMLMode Then
                    Dim xmlDoc As New XmlDocument()
                    xmlDoc.Load(Common.currentXMLFile)

                    Dim node As XmlNode
                    node = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
                    If Not IsNothing(node) Then
                        'node.Attributes("date_end").Value = DateTime.Now.ToString()
                        node.Attributes("date_end").Value = Common.getDateString(Common.GetCurrentEasternTime())
                        node.Attributes("isCompleted").Value = "True"
                        node.Attributes("transaction_status").Value = "1"

                    End If
                    'Update File status in case some file not updated
                    For i = 0 To _FileUpload.Length - 1
                        If _FileUpload(i).Status = 1 Or _FileUpload(i).Status = 9 Then
                            Dim FileNode As XmlNode
                            FileNode = xmlDoc.SelectSingleNode("/tblFTPTransaction/tblFTPFile[@file_name='" + _FileUpload(i).FileName + "']")

                            If Not IsNothing(FileNode) Then
                                If FileNode.Attributes("is_finished").Value <> "True" Then
                                    FileNode.Attributes("is_finished").Value = "True"
                                    'FileNode.Attributes("date_finished").Value = DateTime.Now.ToString()
                                    FileNode.Attributes("date_finished").Value = Common.getDateString(Common.GetCurrentEasternTime())
                                    FileNode.Attributes("file_status").Value = _FileUpload(i).Status.ToString()
                                End If
                            End If

                        End If

                    Next

                    xmlDoc.Save(Common.currentXMLFile)
                    File.Copy(Common.currentXMLFile, Common.currentXMLFile.Replace(".xml", "_backup.xml"), True)
                    UploadFile(Common.currentXMLFile, Common.currentXMLFolder)
                End If


                'Me.ControlBox = True
                If isUploadPcap And Not _isRunUploadPcap And RadioButton_SQZ_PCAP.Checked Then
                    UpdateControl_Sub(Label_Info, "SQZ complete.  Starting PCAPZIP")
                    UpdateControl_Sub(Label_TotalFile, "SQZ complete.  Starting PCAPZIP")
                Else
                    UpdateControl_Sub(Label_Info, "All files have been uploaded")
                    UpdateControl_Sub(Label_TotalFile, "All files have been uploaded")
                End If

                UpdateControl_Sub(Label_TotalFailed, "")

                'addTrimmingFailed()

                EnableButton_Sub(Button_DeleteUploadedFiles, True)
                If cnn.State <> ConnectionState.Closed Then
                    cnn.Close()
                End If
                'Upload log file to FTP server
                If Common.hasWCFAccess And Not Common.forceXMLMode Then
                    Try
                        UploadFile(log_file, "GTPLogs")
                    Catch ex As Exception

                    End Try
                End If


                If isUploadPcap And Not _isRunUploadPcap And RadioButton_SQZ_PCAP.Checked Then
                    BackgroundWorker2.ReportProgress(CInt(50))
                Else
                    BackgroundWorker2.ReportProgress(CInt(100))
                End If

                If Not Common.isShowCompleteMessage Then
                    Common.isShowCompleteMessage = True
                    DeleteTmpFolder()
                    Dim msgStr As String = "Files have been uploaded. "
                    If ListView_OtherFiles.Items.Count > 0 Then
                        msgStr = msgStr + "Note that " + ListView_OtherFiles.Items.Count.ToString() + " file(s) failed the unzip check and still need to be uploaded to FTP. "
                    End If
                    If ListView_FileTrimmingFailed.Items.Count > 0 Then
                        'TabControl1.SelectedIndex = 2
                        msgStr = msgStr + "Note that " + ListView_FileTrimmingFailed.Items.Count.ToString() + " of the uploaded file(s) were corrupted. "
                    End If
                    If ListView_OtherFiles.Items.Count > 0 Or ListView_FileTrimmingFailed.Items.Count > 0 Then
                        MessageBox.Show(msgStr)
                    Else
                        If isUploadPcap And Not _isRunUploadPcap And RadioButton_SQZ_PCAP.Checked Then
                            Common.isShowCompleteMessage = False
                            Me.Invoke(New MethodInvoker(AddressOf Me.Timer_UploadPcap.Start))
                            _isRunUploadPcap = True
                        Else
                            Try
                                If _folderListToDelete.Count > 0 Then
                                    For i = 0 To _folderListToDelete.Count - 1
                                        Try
                                            Directory.Delete(_folderListToDelete(i), True)
                                        Catch ex As Exception

                                        End Try

                                    Next
                                    '_folderListToDelete = New List(Of String)
                                End If
                            Catch ex As Exception

                            End Try

                            MessageBox.Show("All files have been uploaded")

                            Threading.Thread.Sleep(2000)

                            If Directory.Exists(Common.currentTempGTPUnzip) Then
                                Dim fileToDelete As String() = Directory.GetFiles(Common.currentTempGTPUnzip)
                                For Each fileStr In fileToDelete
                                    File.Delete(fileStr)
                                Next
                            End If
                        End If

                    End If
                Else
                    If Directory.Exists(Common.currentTempGTPUnzip) Then
                        Dim fileToDelete As String() = Directory.GetFiles(Common.currentTempGTPUnzip)
                        For Each fileStr In fileToDelete
                            Try
                                File.Delete(fileStr)
                            Catch ex As Exception

                            End Try

                        Next
                    End If
                End If

            End If
        Else

            Dim isReupload = False
            For i = 0 To _FileUpload.Length - 1
                If _FileUpload(i).Status = 0 Then
                    isReupload = True
                End If
            Next


            If isReupload Then
                UpdateControl_Sub(Label_Info, "Some files have failed to upload. GTP is trying to re-upload")
                id_transaction_reupload = id_FTP_Client
                If id_FTP_Transaction.ToString().Trim() = "" Then
                    'id_server_transaction_reupload = id
                Else
                    id_server_transaction_reupload = id_FTP_Transaction
                End If

                ReuploadProcess = 1
                Me.Invoke(New MethodInvoker(AddressOf Me.Timer_Upload.Start))
            End If

        End If

        If Common.hasWCFAccess And Not Common.forceXMLMode Then
            proxy.Close()
        End If



    End Sub

    Private Sub BackgroundWorker2_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged
        'ProgressBar_FileProgress.Value = e.ProgressPercentage
        Try
            ProgressBar_Upload.Value = e.ProgressPercentage
        Catch ex As Exception

        End Try
    End Sub

    Private Sub frmUpload_FormClosed(sender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed

        Try
            Dim fileToDelete As String() = Directory.GetFiles(Common.currentTempGTPUnzip)
            For Each fileStr In fileToDelete
                Try
                    File.Delete(fileStr)
                Catch ex As Exception

                End Try

            Next
        Catch ex As Exception

        End Try

        frmMainApp.FTPUploadToolStripMenuItem.Enabled = True
        frmMainApp.CloseToolStripMenuItem.Enabled = True
        Common.isResumeUpload = False
        If BackgroundWorker1.WorkerSupportsCancellation Then
            BackgroundWorker1.CancelAsync()
        End If

        If BackgroundWorker2.WorkerSupportsCancellation Then
            BackgroundWorker2.CancelAsync()
        End If

        BackgroundWorker1.Dispose()
        BackgroundWorker2.Dispose()
        Try
            System.IO.Directory.Delete(Common.NetworkTempFolder, True)
        Catch ex As Exception

        End Try

        If Not _isTransactionStart Then
            Try
                If _folderListToDelete.Count > 0 Then
                    For i = 0 To _folderListToDelete.Count - 1
                        Try
                            Directory.Delete(_folderListToDelete(i), True)
                        Catch ex As Exception

                        End Try


                    Next
                    '_folderListToDelete = New List(Of String)
                End If
            Catch ex As Exception

            End Try
        Else
            saveFolderToDelete()
        End If


    End Sub

    Private Sub ComboBox_Project_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox_Project.SelectedIndexChanged
        'ComboBox_Market.Items.Clear()
        ComboBox_Drive.Items.Clear()
        If currentMarket <> "" Then
            Return
        End If
        If Common.isResumeUpload = False Then
            If ComboBox_Project.Text = "" Then
                ComboBox_Market.Enabled = False
            Else
                'Dim strClient As String = ComboBox_Project.Text
                Dim client_market_value As Integer = -1
                Dim i As Integer
                If Common.hasWCFAccess And Not Common.forceXMLMode Then
                    For i = 0 To clients.Length - 1
                        If clients(i).ClientName = ComboBox_Project.Text Then
                            client_market_value = clients(i).client_market_value

                            Dim clientIds = clients.Where(Function(p) p.ClientName = clients(i).ClientName).Select(Function(p) p.Campaigns)
                            For Each clientId In clientIds
                                If Not String.IsNullOrEmpty(clientId) Then
                                    Dim arrClientIds() As String = clientId.Split(";")
                                    For Each client In arrClientIds
                                        If Not String.IsNullOrEmpty(client) Then
                                            ComboBox_Drive.Items.Add(client)
                                        End If
                                    Next
                                End If
                            Next

                        End If
                    Next
                Else
                    Dim xmlDoc = New XmlDocument()
                    xmlDoc.Load("client.xml")
                    EncryptDecrypt.Program.Decrypt(xmlDoc, "GWSsol")
                    Dim ClientNodes As XmlNodeList
                    ClientNodes = xmlDoc.SelectNodes("/ClientRoot/Client")
                    Dim node As XmlNode
                    If ClientNodes.Count > 0 Then
                        For Each node In ClientNodes
                            If ComboBox_Project.Text.Trim() = node.Attributes("Name").Value.Trim() Then
                                client_market_value = Convert.ToInt32(node.Attributes("market_status").Value)
                                Dim lstcampaigns = lstClient.Where(Function(p) p.ClientName = ComboBox_Project.Text.Trim()).Select(Function(p) p.Campaigns)
                                For Each campaigns In lstcampaigns
                                    If Not String.IsNullOrEmpty(campaigns) Then
                                        Dim arrCampaigns() As String = campaigns.Split(";")
                                        For Each campaign In arrCampaigns
                                            If Not String.IsNullOrEmpty(campaign) Then
                                                ComboBox_Drive.Items.Add(campaign)
                                            End If
                                        Next
                                    End If
                                Next

                            End If
                        Next
                    End If
                End If
                If client_market_value <> -1 Then
                    GetMarketList(client_market_value)
                    CheckBox_NewMarket.Checked = False
                Else
                    EnableNewMarket()
                    CheckBox_NewMarket.Checked = True
                    ComboBox_Market.SelectedIndex = -1
                    ComboBox_Market.Enabled = False
                End If
            End If
        End If
        If currentMarket <> "" Then
            ComboBox_Market.SelectedIndex = ComboBox_Market.FindString(currentMarket)
        End If
    End Sub

    Private Sub Button3_Click_4(sender As System.Object, e As System.EventArgs)
        Dim FileName As String = ListView_FileList.Items(0).Text
        'Dim RemoteDir = "/" + TextBox_UserName.Text.Trim() + "_curUpload/" + Common.RemoteFTPFolder
        Dim RemoteDir = "/TeamREMIS_curUpload/" + Common.RemoteFTPFolder
        Common.DeleteFileonFTP(FileName, RemoteDir)
    End Sub

    Private Sub CheckBox_Mode_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox_Mode.CheckedChanged
        If CheckBox_Mode.Checked Then
            Common.forceXMLMode = True

        Else
            Common.forceXMLMode = False
        End If
    End Sub


    Private Sub frmUpload_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        BackgroundWorker1.CancelAsync()
        BackgroundWorker2.CancelAsync()
        BackgroundWorker1.Dispose()
        BackgroundWorker2.Dispose()
    End Sub

    Private Sub Button_Close_Click(sender As System.Object, e As System.EventArgs)
        If BackgroundWorker1.WorkerSupportsCancellation Then
            BackgroundWorker1.CancelAsync()
        End If

        If BackgroundWorker2.WorkerSupportsCancellation Then
            BackgroundWorker2.CancelAsync()
        End If


        BackgroundWorker1.Dispose()
        BackgroundWorker2.Dispose()

    End Sub


    Public Function FTPRenameFileNet(ByVal FileToRename As String, ByVal NewName As String, ByVal ftp_url As String) As Boolean
        Dim finishedCode As Integer = 0
        Try
            Dim clsRequest As System.Net.FtpWebRequest
            clsRequest = DirectCast(System.Net.WebRequest.Create(ftp_url + "/" & FileToRename), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential("TeamREMIS", "$MSR@mat")
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.Rename
            clsRequest.RenameTo = NewName
            clsRequest.GetResponse()
            finishedCode = 0
        Catch ex As Exception
            Dim ss As String = ""

        End Try

        Return finishedCode

    End Function




    Private Sub CreateClientXML()

        Dim RootElement As XmlElement
        Dim xmlDoc As New XmlDocument()
        Common.MarketXMLFile = AppDomain.CurrentDomain.BaseDirectory + "client.xml"
        Dim dec As XmlDeclaration
        dec = xmlDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
        xmlDoc.AppendChild(dec)
        RootElement = xmlDoc.CreateElement("ClientRoot")

        xmlDoc.AppendChild(RootElement)

        Dim proxy As New GTPServiceClient()

        Dim clientList As String()
        Dim c As New _Client

        proxy.Open()
        'clientList = proxy.GetClientForTeam(Common.REMIS_username)
        'If clientList.Length > 0 Then
        '    For Each c In clientList
        '        'ComboBox_Project.Items.Add(client)
        '        Dim marketElement As XmlElement


        '        marketElement = xmlDoc.CreateElement("Client")
        '        marketElement.SetAttribute("Name", c.ClientName)
        '        marketElement.SetAttribute("market_status", c.client_market_value)
        '        RootElement.AppendChild(marketElement)
        '    Next

        'End If
        clients = proxy.GetClient()
        For Each c In clients
            'ComboBox_Project.Items.Add(c.ClientName)
            Dim marketElement As XmlElement
            marketElement = xmlDoc.CreateElement("Client")
            marketElement.SetAttribute("Name", c.ClientName)
            marketElement.SetAttribute("market_status", c.client_market_value)
            marketElement.SetAttribute("Campaigns", c.Campaigns)
            RootElement.AppendChild(marketElement)
        Next
        proxy.Close()
        EncryptDecrypt.Program.Encrypt(xmlDoc, "ClientRoot", "GWSsol").Save(Common.MarketXMLFile)
        ' xmlDoc.Save(Common.MarketXMLFile)


    End Sub


    Private Sub Button3_Click_10(sender As System.Object, e As System.EventArgs)
        CreateClientXML()
        CreateMarketXML()
    End Sub


    Private Sub ComboBox_Market_Enter(sender As System.Object, e As System.EventArgs) Handles ComboBox_Market.Enter
        ComboBox_Market.DroppedDown = True
    End Sub


    Private Sub ComboBox_Market_MouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles ComboBox_Market.MouseDown
        'ComboBox_Market.DroppedDown = True
    End Sub

    Private Sub ComboBox_Market_MouseLeave(sender As System.Object, e As System.EventArgs)



    End Sub

    Private Sub ComboBox_Market_Leave(sender As System.Object, e As System.EventArgs) Handles ComboBox_Market.Leave
        Dim market As String = ComboBox_Market.Text
        If market <> "" Then
            isValid = False
            Dim i As Integer
            For Each item In ComboBox_Market.Items
                Dim tmp As _MarketName = item

                If tmp.name_market = market Then

                    isValid = True
                    ComboBox_Market.SelectedValue = tmp.id_market
                    Return
                End If
            Next

            If Not isValid Then
                MessageBox.Show("Market Name is not in the list. Please select again ! ")
                ComboBox_Market.Text = ""
            End If
        End If

    End Sub

    Private Sub Button3_Click_8(sender As System.Object, e As System.EventArgs)

        SevenZipExtractor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "7z.dll")
        Dim fileName As String = "C:\Tuan\DataUpload\Team23.zip"
        Dim extractor As New SevenZipExtractor(fileName)

        Dim count As Integer = extractor.ArchiveFileData.Count
        If (count > 0) Then
            MessageBox.Show(count.ToString())
        End If
        'extractor.ExtractArchive("C:\Tuan\DataUpload\Team23_1")




    End Sub


    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs)
        SevenZipCompressor.SetLibraryPath(AppDomain.CurrentDomain.BaseDirectory + "7z.dll")
        'C:\Program Files\7-Zip
        'SevenZipCompressor.SetLibraryPath("C:\Program Files\7-Zip\7z.dll")
        Dim cmp As New SevenZipCompressor()
        'Dim dir As String = "C:\GTPTest\aa"
        Dim dir As String = "C:\Tuan\DataUpload\2011-11-01-12-28-05-0021-0025-0006-0021-B.mf"

        'Dim fileCompress As String = "C:\GTPTest\aa.zip"
        Dim fileCompress As String = "C:\Tuan\DataUpload\2011-11-01-12-28-05-0021-0025-0006-0021-B.sqz"
        cmp.CompressDirectory(dir, fileCompress)
    End Sub

    Private Sub TextBox_UserName_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles TextBox_UserName.SelectedIndexChanged
        TextBox_Password.Text = ""
    End Sub

    Private Sub Button3_Click_9(sender As System.Object, e As System.EventArgs)
        Dim ftpRequest As FtpWebRequest
        Dim ftpResponse As FtpWebResponse
        ftpRequest = FtpWebRequest.Create(New Uri("ftp://upload.mygws.com"))
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory
        ftpRequest.Proxy = Nothing
        ftpRequest.UseBinary = True
        ftpRequest.Credentials = New NetworkCredential("TeamREMIS", "$MSR@mat111")
        Try
            ftpResponse = ftpRequest.GetResponse()
        Catch ex As Exception

        End Try

        If ftpResponse.StatusCode = FtpStatusCode.OpeningData Then
            Dim tt As String = ""
        End If

    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs)
    End Sub


    Private Sub CheckBox_Duplicate_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox_Duplicate.CheckedChanged
        If Not File.Exists(Common.rememberCheckBox) Then
            File.Create(Common.rememberCheckBox).Close()
        End If

        Dim tmpFileList As New List(Of String)
        Dim itemList As New List(Of ListViewItem)
        For Each item In ListView_FileList.Items
            If Not item.ForeColor = Color.Green Then
                tmpFileList.Add(item.Text)
            End If

        Next
        selectedUploadFiles = New List(Of String)
        ListView_Dups.Items.Clear()

        If ListView_FileList.Items.Count > 0 Or ListView_Duplicate.Items.Count > 0 Then
            If (CheckBox_Duplicate.Checked) Then
                If CheckBox_Duplicate.Checked Then

                    If (Common.hasWCFAccess) Then

                        CheckDuplicate(tmpFileList)
                    End If

                End If
            Else
                'Move Files Back to the Upload List

                Dim uniqueFileList As New List(Of String)

                Dim hash_file As HashSet(Of String) = New HashSet(Of String)()

                Dim FileInfo As FileInfo


                For Each listitem In ListView_Duplicate.Items
                    If Not uniqueFileList.Contains(listitem.Text) Then
                        uniqueFileList.Add(listitem.Text)
                    Else
                        Dim lvi As New ListViewItem
                        lvi.Text = listitem.Text
                        lvi.ForeColor = Color.Brown
                        FileInfo = New FileInfo(listitem.Text)
                        lvi.SubItems.Add(FileInfo.Length)
                        AddListViewItem1(ListView_Dups, lvi)
                        SelectTabIndex_Sub(TabControl1, 3)
                    End If
                    listitem.Remove()
                Next
                Dim fileStr As String

                For Each fileStr In uniqueFileList

                    If File.Exists(fileStr) Then
                        Dim FileName As String = fileStr.Substring(fileStr.LastIndexOf("\") + 1, fileStr.Length - fileStr.LastIndexOf("\") - 1)
                        Dim lvi As New ListViewItem
                        ' First Column can be the listview item's Text  
                        lvi.Text = fileStr
                        lvi.ForeColor = Color.Black
                        ListView_FileList.Items.Add(lvi)
                        FileInfo = New FileInfo(fileStr)
                        lvi.SubItems.Add(FileInfo.Length)

                    End If

                Next
            End If

            For Each item In ListView_FileList.Items
                selectedUploadFiles.Add(item.Text)
            Next

            CheckDuplicateLocal(selectedUploadFiles)

            Label_TotalFile.Text = "Total file(s): " + ListView_FileList.Items.Count.ToString
            total_file_to_uplload = ListView_FileList.Items.Count
            ListView_FileList.Refresh()
            ListView_FileList.EndUpdate()

            ListView_Dups.Refresh()
            ListView_Dups.EndUpdate()

        End If

    End Sub


    Private Sub ComboBox_Market_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles ComboBox_Market.KeyDown
        ComboBox_Market.DroppedDown = False
    End Sub

    Private Sub ComboBox_PendingMarket_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles ComboBox_PendingMarket.KeyDown
        ComboBox_Market.DroppedDown = False
        ComboBox_PendingMarket.DroppedDown = False
    End Sub

    Private Sub ComboBox_PendingMarket_KeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles ComboBox_PendingMarket.KeyUp
        Button_NewMarket.Enabled = True
    End Sub

    'Private Sub Button3_Click_3(sender As System.Object, e As System.EventArgs)
    '    Dim fileSec = System.IO.File.GetAccessControl("C:\Temp", System.Security.AccessControl.AccessControlSections.Access)
    '    Dim accessRules = fileSec.GetAccessRules(True, True, GetType(System.Security.Principal.NTAccount))
    '    For Each rule As System.Security.AccessControl.FileSystemAccessRule In accessRules
    '        Console.WriteLine(rule.IdentityReference.Value)
    '        Console.WriteLine(rule.AccessControlType.ToString())
    '        Console.WriteLine(rule.FileSystemRights.ToString())
    '    Next
    'End Sub

    'Private Sub Button3_Click_6(sender As System.Object, e As System.EventArgs)
    '    Dim date1 As DateTime = Common.GetCurrentEasternTime()
    '    Dim tt = date1


    'End Sub

    Private Sub Timer_UploadPcap_Tick(sender As System.Object, e As System.EventArgs) Handles Timer_UploadPcap.Tick
        If RadioButton_SQZ_PCAP.Checked Then
            Button_Upload_Do(0)
            isUploadPcap = False
            Timer_UploadPcap.Stop()
        Else
            isUploadPcap = False
            Timer_UploadPcap.Stop()
        End If

    End Sub


    Private Sub BackgroundWorker_Unzip_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker_Unzip.DoWork
        'Dim selectedUploadFiles As New List(Of String)

        Dim count_sqc As Integer = 0

        For i = 0 To tmpSelectedUploadFiles.Count - 1
            If tmpSelectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Then
                Dim tmpFileINfor As New FileInfo(tmpSelectedUploadFiles(i))
                'totalSize = totalSize + tmpFileINfor.Length / 1024 / 1024 / 1024
                count_sqc = count_sqc + 1
            End If
        Next
        Dim current_sqc As Integer = 0

        Dim FileInfo As FileInfo
        For Each sqzFile In tmpSelectedUploadFiles

            Dim imageStr As String = "*.jpg,*.jpeg,*.jpe,*.png,*.bmp,*.gif,*.tif,*.mpo,*.heic,*.heif,*.heiv,*.avi,*.flv,*.wmv,*.mov,*.mp4"
            Dim fileExt As String = Common.GetFileExtension(sqzFile)
            If imageStr.IndexOf("*." + fileExt.ToLower) >= 0 Then
                Dim lvi As New ListViewItem
                lvi.Text = sqzFile

                AddListViewItem1(ListView_FileList, lvi)
            End If
        Next
        Dim sqzHash As New HashSet(Of String)
        For Each sqzFile In tmpSelectedUploadFiles
            If sqzFile.ToLower.EndsWith(".sqz") Then
                Dim fileNameOnly As String = Common.GetFileNameOnly(sqzFile)
                If Not sqzHash.Contains(fileNameOnly) Then
                    sqzHash.Add(fileNameOnly)
                End If
            End If
        Next

        Dim mfHash As New HashSet(Of String)
        For Each mfFile In tmpSelectedUploadFiles
            If mfFile.ToLower.EndsWith(".mf") Then
                Dim fileNameOnly As String = Common.GetFileNameOnly(mfFile)
                If Not mfHash.Contains(fileNameOnly) Then
                    mfHash.Add(fileNameOnly)
                End If
            End If
        Next

        Dim isErr As Boolean = False

        Dim errMsg As String = "The following file paths are too long for some Windows functions and cannot be uploaded. If these are SQC files, unzip them to SQZ files. If these are any other file type, move them to a shorter path"
        errMsg = errMsg + Environment.NewLine



        For i = 0 To tmpSelectedUploadFiles.Count - 1
            If tmpSelectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Then
                current_sqc = current_sqc + 1
                'Label_Info.Text = "Please wait. Unzipping SQC " + current_sqc.ToString() + " of " + count_sqc.ToString()
                UpdateControl_Sub(Label_Info, "Please wait. Unzipping SQC " + current_sqc.ToString() + " of " + count_sqc.ToString())
                Dim fileSQZList As New List(Of String)
                Dim DriveName As String = tmpSelectedUploadFiles(i).Substring(0, 1)
                If DriveName.StartsWith("\") Then
                    DriveName = "C"
                End If
                Dim DirName As String = DriveName + ":\TempGTPSQC\" + System.Guid.NewGuid().ToString() + "\"
                Dim sqcDirPath As String = Common.UnzipFile(DirName, tmpSelectedUploadFiles(i))
                If sqcDirPath.Trim <> "" Then
                    fileSQZList = Common.GetAllFileInFolder(sqcDirPath, ".sqz")


                    For Each sqzFile In fileSQZList
                        Dim fileNameOnly As String = Common.GetFileNameOnly(sqzFile)
                        If Not sqzHash.Contains(fileNameOnly) Then

                            Try
                                FileInfo = New FileInfo(sqzFile)
                                sqzHash.Add(fileNameOnly)
                                selectedUploadFiles.Add(sqzFile)
                            Catch ex As Exception
                                'The following path+filename lengths are too long for some Windows functions.  You must select files from shorter path names.
                                'The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.
                                If ex.Message.Contains("The specified path, file name, or both are too long") Then
                                    isErr = True
                                    errMsg = errMsg + tmpSelectedUploadFiles(i) + Environment.NewLine
                                    'MessageBox.Show("The following unzipped " + tmpSelectedUploadFiles(i) + " lengths are too long for some Windows functions (" + sqzFile + ").  You must select files from shorter path names.")
                                    'ListView_OtherFiles.Items.Add(tmpSelectedUploadFiles(i))
                                    Dim lvi As New ListViewItem
                                    lvi.Text = tmpSelectedUploadFiles(i)
                                    lvi.ForeColor = Color.Brown
                                    AddListViewItem1(ListView_OtherFiles, lvi)
                                    SelectTabIndex_Sub(TabControl1, 0)
                                    'Return
                                End If
                                'File.Copy(sqzFile, DirName + fileNameOnly)
                                'sqzHash.Add(fileNameOnly)
                                'selectedUploadFiles.Add(DirName + fileNameOnly)
                            End Try

                        End If
                    Next

                    'selectedUploadFiles.AddRange(fileSQZList)

                    fileSQZList = Common.GetAllFileInFolder(sqcDirPath, ".mf")
                    For Each mfFile In fileSQZList
                        Dim fileNameOnly As String = Common.GetFileNameOnly(mfFile)
                        If Not sqzHash.Contains(fileNameOnly) Then
                            mfHash.Add(fileNameOnly)
                            selectedUploadFiles.Add(mfFile)
                        End If
                    Next

                    'selectedUploadFiles.AddRange(fileSQZList)

                    _folderListToDelete.Add(DirName)
                Else
                    Dim lvi As New ListViewItem
                    ' First Column can be the listview item's Text  
                    lvi.Text = tmpSelectedUploadFiles(i)
                    lvi.ForeColor = Color.Brown
                    'ListView_FileTrimmingFailed.Items.Add(lvi)
                    AddListViewItem1(ListView_FileTrimmingFailed, lvi)
                    'TabControl1.SelectedIndex = 2
                    SelectTabIndex_Sub(TabControl1, 2)

                End If

                selectedUploadFiles.Remove(tmpSelectedUploadFiles(i))

            End If
        Next


        If isErr Then
            MessageBox.Show(errMsg)
        End If

        UpdateControl_Sub(Label_Info, "Upload data to FTP server")

        Dim isEmpty As Boolean = False

        If ListView_FileList.Items.Count = 0 Then
            isEmpty = True
        End If
        Dim hashListView As New HashSet(Of String)

        If Not IsNothing(selectedUploadFiles) Then
            For i = 0 To selectedUploadFiles.Count - 1
                If selectedUploadFiles(i).ToLower.IndexOf(".sqc") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".pcapzip") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".nmf") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".gpx") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".sqz") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".mf") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".log") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".wnu") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".wnd") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".wnl") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".trp") > 0 Or selectedUploadFiles(i).ToLower.IndexOf(".txt") > 0 Then

                    If selectedUploadFiles(i).IndexOf("'") >= 0 Or selectedUploadFiles(i).IndexOf("`") >= 0 Then
                        MessageBox.Show(" The input path has special chracter. Please remove the special char from the input path and restart the upload process.")
                        Return
                    Else
                        If (Not CheckFileExist(selectedUploadFiles(i)) Or Not hashListView.Contains(selectedUploadFiles(i))) Or isSelectFile Then
                            Dim lvi As New ListViewItem
                            ' First Column can be the listview item's Text  
                            lvi.Text = selectedUploadFiles(i)
                            ' Second Column is the first sub item  
                            ' Add the ListViewItem to the ListView  
                            'ListView_FileList.Items.Add(lvi)
                            Try
                                FileInfo = New FileInfo(selectedUploadFiles(i))
                                lvi.SubItems.Add(FileInfo.Length)
                            Catch ex As Exception

                            End Try


                            If Not selectedUploadFilesExist.Contains(lvi.Text) Then
                                AddListViewItem1(ListView_FileList, lvi)
                                hashListView.Add(selectedUploadFiles(i))
                            End If


                            If selectedUploadFiles(i).ToLower.EndsWith(".sqz") And FileInfo.Length > 30 * 1024 * 1024 And Not isPcapFileExist Then
                                If CheckPCAP(selectedUploadFiles(i)) Then
                                    isPcapFileExist = True
                                    EnableRadioButton_Sub(RadioButton_SQZ, True)
                                    EnableGroupBox_Sub(GroupBox_PCAP_Options, True)
                                    EnableRadioButton_Sub(RadioButton_SQZ_PCAP, True)

                                    'RadioButton_SQZ.Enabled = True
                                    'GroupBox_PCAP_Options.Enabled = True
                                    'RadioButton_SQZ_PCAP.Enabled = True
                                End If
                            End If

                        End If
                    End If

                Else

                    'MessageBox.Show("File" + selectedUploadFiles(i) + " is not one of following type: MF, SQZ, LOG, TRP, TXT, WND, WNU, WNL, .NMF, .GPX., .SQC It will NOT be added to Upload File list")
                End If

            Next
            'Button_SelectFolder.Text = "Add More Upload Files"
            UpdateControl_Sub(Button_SelectFolder, "Add More Upload Files")
        End If
        selectedUploadFiles.AddRange(selectedUploadFilesExist)
        If CheckBox_Duplicate.Checked Then

            If (Common.hasWCFAccess) Then

                CheckDuplicate(selectedUploadFiles)
            End If
        End If

        CheckDuplicateLocal(selectedUploadFiles)
        'CheckDuplicateLocal_Background()

        UpdateControl_Sub(Label_TotalFile, "Total file(s): " + ListView_FileList.Items.Count.ToString)
        total_file_to_uplload = ListView_FileList.Items.Count
    End Sub
    Private Sub ComboBox_Drive_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles ComboBox_Drive.KeyDown
        ComboBox_Drive.DroppedDown = False
    End Sub
    Private Sub ComboBox_PendingMarket_Leave(sender As System.Object, e As System.EventArgs) Handles ComboBox_PendingMarket.Leave
        If ComboBox_PendingMarket.Text.Trim.Length > 50 Then
            MessageBox.Show("Market name must be less than or equal to 50 characters in length")
            ComboBox_PendingMarket.Text = ComboBox_PendingMarket.Text.Substring(0, 50)
            Return
        End If
    End Sub

    Private Sub CheckBox_EnableSFTP_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox_EnableSFTP.CheckedChanged
        If CheckBox_EnableSFTP.Checked Then
            Common._isSFTP = True
        Else
            Common._isSFTP = False
        End If
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs)
        'CreateMarketXML()
        CreateClientXML()
    End Sub
    Private Sub CreateCampaignXML()
        Dim campaigns As _Campaign()
        Dim RootElement As XmlElement
        Dim xmlDoc As New XmlDocument()
        Common.MarketXMLFile = AppDomain.CurrentDomain.BaseDirectory + "Campaign.xml"
        Dim dec As XmlDeclaration
        dec = xmlDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
        xmlDoc.AppendChild(dec)
        RootElement = xmlDoc.CreateElement("ClientRoot")
        xmlDoc.AppendChild(RootElement)
        Using proxy As New GTPServiceClient()
            campaigns = proxy.GetCampaign()
            For Each campaign In campaigns
                Dim campaignElement As XmlElement
                campaignElement = xmlDoc.CreateElement("Campaign")
                campaignElement.SetAttribute("Idcampaign", campaign.id_Campaign)
                campaignElement.SetAttribute("campaign_name", campaign.campaign_name)
                RootElement.AppendChild(campaignElement)
            Next
        End Using
        EncryptDecrypt.Program.Encrypt(xmlDoc, "ClientRoot", "GWSsol").Save(Common.MarketXMLFile)
    End Sub

    Private Sub CreateProjectXML()
        Dim teamProjects As _TeamProjects()
        Dim RootElement As XmlElement
        Dim xmlDoc As New XmlDocument()
        Common.MarketXMLFile = AppDomain.CurrentDomain.BaseDirectory + "project.xml"
        Dim dec As XmlDeclaration
        dec = xmlDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
        xmlDoc.AppendChild(dec)
        RootElement = xmlDoc.CreateElement("ProjectRoot")
        xmlDoc.AppendChild(RootElement)
        Using proxy As New GTPServiceClient()
            teamProjects = proxy.GetTeamProjects()
            For Each teamProject In teamProjects
                Dim projectElement As XmlElement
                projectElement = xmlDoc.CreateElement("TeamProject")
                projectElement.SetAttribute("userName", teamProject.UserName)
                projectElement.SetAttribute("clientName", teamProject.ClientName)
                RootElement.AppendChild(projectElement)
            Next
        End Using
        EncryptDecrypt.Program.Encrypt(xmlDoc, "ProjectRoot", "GWSsol").Save(Common.MarketXMLFile)
    End Sub
End Class