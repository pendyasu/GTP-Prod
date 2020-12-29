Public Class frmTest

    Private Sub Button_SelectFolder_Click(sender As System.Object, e As System.EventArgs) Handles Button_SelectFolder.Click
        Dim myFiles1 As New OpenFileDialog
        myFiles1.Multiselect = False
        myFiles1.RestoreDirectory = True

        If (myFiles1.ShowDialog() = DialogResult.OK) Then
            Dim FileList As String() = myFiles1.FileNames
            TextBox1.Text = FileList(0)
        End If
    End Sub

 
End Class