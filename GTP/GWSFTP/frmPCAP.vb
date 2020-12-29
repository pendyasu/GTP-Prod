Imports System.IO
Imports Chilkat

Public Class frmPCAP

    Private _FileList As List(Of String)

    Private Sub Button_SelectFile_Click(sender As System.Object, e As System.EventArgs) Handles Button_SelectFile.Click
        Dim repeatFile As Boolean = False
        Dim selectedUploadFiles() As String
        Dim myFiles1 As New OpenFileDialog
        'Try
        myFiles1.Filter = "Select Files to upload  (*.sqz)|*.sqz|(*.sqz)|*.sqz"
        'myFiles1.DefaultExt = "csv"
        myFiles1.Multiselect = True
        myFiles1.Title = "Select File "
        myFiles1.RestoreDirectory = True 'mod 7/6/07

        If (myFiles1.ShowDialog() = DialogResult.OK) Then
            Dim FolderName As String = myFiles1.FileNames(0).Substring(0, myFiles1.FileNames(0).LastIndexOf("\"))
            If Not Common.isFolderWritable(FolderName) Then
                MessageBox.Show("Files cannot be processed from a read-only location")
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

            selectedUploadFiles = myFiles1.FileNames
            Array.Sort(selectedUploadFiles)
        End If
        'Catch ee As Exception

        'End Try
        Dim i As Integer
        Dim FileInfo As FileInfo

        If Not IsNothing(selectedUploadFiles) Then
            For i = 0 To selectedUploadFiles.Length - 1
                If selectedUploadFiles(i).ToLower.IndexOf(".sqz") > 0 Then

                    Dim lvi As New ListViewItem
                    ' First Column can be the listview item's Text  
                    lvi.Text = selectedUploadFiles(i)
                    ' Second Column is the first sub item  
                    ' Add the ListViewItem to the ListView  
                    ListView_FileList.Items.Add(lvi)
                    FileInfo = New FileInfo(selectedUploadFiles(i))
                    lvi.SubItems.Add(FileInfo.Length)

                End If
            Next
        End If

    End Sub


    Private Sub ProcessPCAPFiles()


        Dim success As Boolean
        Dim zip As New Chilkat.Zip
        Dim unlocked As Boolean
        unlocked = zip.UnlockComponent("cqcmgfZIP_CVvkamt08Rwl")
        If (Not unlocked) Then
            'MsgBox(zip.LastErrorText)
            Return
        End If
        Dim fileToProcess As String

        For Each fileToProcess In _FileList
            Dim FileName As String = ""

            UpdateProgress_Sub(ListView_FileList, fileToProcess, Color.Black)

            If fileToProcess.ToLower.Contains(".sqz") Then
                If File.Exists(fileToProcess) Then



                    Dim isRemove As Boolean = False
                    success = zip.OpenZip(fileToProcess)

                    '2016-08-19-08-42-53-0000-7086-1873-0070-B.sqz
                    Dim isPCAP As Boolean = False
                    If fileToProcess.ToLower.EndsWith(".sqz") And success Then
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
                        Dim tmpFolderName As String = fileToProcess + "_PCAP"
                        Dim unzipCount As Integer
                        Directory.CreateDirectory(tmpFolderName)

                        unzipCount = zip.Unzip(tmpFolderName)
                        zip.CloseZip()
                        zip.Dispose()
                        'Rename file
                        Dim tmpFileName As String = fileToProcess.ToString().Substring(0, fileToProcess.ToString().LastIndexOf("."))


                        FileSystem.Rename(fileToProcess, tmpFileName + ".pcapzip")


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

                        Common.ZipFolder(fileToProcess, tmpFolderName)
                        Directory.Delete(tmpFolderName, True)

                    End If


                End If

            End If

            UpdateListView_Sub(ListView_FileList, fileToProcess, Color.Green)
        Next

        MessageBox.Show("Done")

    End Sub

    Private Sub Button_ProcessPCAP_Click(sender As System.Object, e As System.EventArgs) Handles Button_ProcessPCAP.Click

        _FileList = New List(Of String)

        For Each listitem In ListView_FileList.Items
            _FileList.Add(listitem.Text)
        Next
        BackgroundWorker1.RunWorkerAsync()

    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        ProcessPCAPFiles()
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
                item.SubItems.RemoveAt(2)
                item.SubItems.Add("Done")
                ListView_FileList.Refresh()

            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub UpdateProgress_Sub(ByVal [ListView_FileList1] As ListView, ByVal [FileName] As String, ByVal [Color] As Color)
        Try
            If ListView_FileList1.InvokeRequired Then
                Dim myDelegate As New UpdateListView_Delegate(AddressOf UpdateProgress_Sub)
                Me.Invoke(myDelegate, New Object() {[ListView_FileList1], [FileName], [Color]})
            Else
                Dim item As New ListViewItem
                item = ListView_FileList.FindItemWithText(FileName)
                item.ForeColor = [Color]
                item.SubItems.Add("Processing")
                ListView_FileList.Refresh()

            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub Button_Select_Folder_Click(sender As System.Object, e As System.EventArgs) Handles Button_Select_Folder.Click


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

        If folder_path <> "" Then
            Dim Subfolders() As String = Common.getAllFolders(folder_path)

            Dim count As Integer = -1
            Dim di As New IO.DirectoryInfo(folder_path)
            Dim diar1 As IO.FileInfo() = di.GetFiles()
            Dim dra As IO.FileInfo

            For Each dra In diar1
                Dim fileType As String = ".sqz"

                If dra.Name.ToLower.EndsWith(".sqz") Then
                    Dim lvi As New ListViewItem
                    lvi.Text = dra.FullName
                    ListView_FileList.Items.Add(lvi)
                    lvi.SubItems.Add(dra.Length)
                End If

            Next


            For Each f In Subfolders
                di = New IO.DirectoryInfo(f)
                diar1 = di.GetFiles()
                For Each dra In diar1

                    If dra.Name.ToLower.EndsWith(".sqz") Then
                        Dim lvi As New ListViewItem
                        lvi.Text = dra.FullName
                        ListView_FileList.Items.Add(lvi)
                        lvi.SubItems.Add(dra.Length)
                    End If

                Next
            Next

        End If
    End Sub
End Class