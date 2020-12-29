Public Class frmMainApp

    Private Sub FTPUploadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FTPUploadToolStripMenuItem.Click
        CloseAllForms()
        Dim myform As New frmUpload
        myform.MdiParent = Me
        myform.Show()
    End Sub

    Friend Sub CloseAllForms()
        If Not Me.MdiChildren Is Nothing Then
            Dim childForm As System.Windows.Forms.Form
            For Each childForm In Me.MdiChildren
                childForm.Close()
            Next
        End If
    End Sub

    Private Sub CreateNewUserToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'CloseAllForms()
        Dim myform As New frmCreateUser
        myform.MdiParent = Me
        myform.Show()

    End Sub

    Private Sub LoginToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoginToolStripMenuItem.Click
        CloseAllForms()
        Dim myform As New frmLogin
        myform.MdiParent = Me        
        myform.StartPosition = FormStartPosition.CenterScreen
        myform.Show()        

    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub FTPUploadHistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        CloseAllForms()
        Dim myform As New frmUploadHistory
        myform.MdiParent = Me

        myform.Show()
    End Sub

    Private Sub frmMainApp_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            System.IO.Directory.Delete(Common.NetworkTempFolder, True)
        Catch ex As Exception

        End Try

        'Try
        '    Dim fileName As String
        '    For Each fileName In System.IO.Directory.GetFiles(Common.NetworkTempFolder)
        '        Try
        '            'System.IO.File.Delete(fileName)
        '            My.Computer.FileSystem.DeleteFile(fileName)
        '        Catch ex As Exception
        '            Dim tt As String = ""
        '        End Try

        '    Next
        'Catch ex As Exception
        '    Dim tt As String = ""
        'End Try
        AdminToolStripMenuItem.Visible = True
    End Sub

    Private Sub EnableXMLModeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles EnableXMLModeToolStripMenuItem.Click
        If Common.REMIS_username <> Nothing Then
            If Common.REMIS_username.ToUpper() = "TeamREMIS".ToUpper() Then
                Common.forceXMLMode = True
            Else
                MessageBox.Show("You are not allowed to use this feature")
            End If
        Else
            MessageBox.Show("You need to login first !")
        End If
        
    End Sub

    Private Sub DisableXMLModeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DisableXMLModeToolStripMenuItem.Click

        If Common.REMIS_username <> Nothing Then
            If Common.REMIS_username.ToUpper() = "TeamREMIS".ToUpper() Then
                Common.forceXMLMode = False
            Else
                MessageBox.Show("You are not allowed to use this feature")
            End If
        Else
            MessageBox.Show("You need to login first !")
        End If

    End Sub

    Private Sub TestFileTrimmingToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles TestFileTrimmingToolStripMenuItem.Click
        Dim form As New frmTest()
        form.Show()
    End Sub

    Private Sub frmMainApp_FormClosed(sender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            Dim startInfo As New ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "DeleteTempFiles.exe")
            startInfo.WindowStyle = ProcessWindowStyle.Minimized
            Process.Start(startInfo)
            startInfo.Arguments = AppDomain.CurrentDomain.BaseDirectory + "DirToDelete.txt"
            Process.Start(startInfo)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ShadowPasswordCOngifToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ShadowPasswordCOngifToolStripMenuItem.Click
        Dim form As New frmConfigLogIn()
        form.Show()
    End Sub

    Private Sub frmMainApp_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            System.IO.Directory.Delete(Common.NetworkTempFolder, True)
        Catch ex As Exception

        End Try
        'Try
        '    Dim fileName As String
        '    For Each fileName In System.IO.Directory.GetFiles(Common.NetworkTempFolder)
        '        Try
        '            System.IO.File.Delete(fileName)
        '        Catch ex As Exception
        '            Dim tt As String = ""
        '        End Try

        '    Next
        'Catch ex As Exception
        '    Dim tt As String = ""
        'End Try
    End Sub

    Private Sub PCAPUtilityToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PCAPUtilityToolStripMenuItem.Click
        Dim formPCAP As New frmPCAP()
        formPCAP.Show()
    End Sub
End Class