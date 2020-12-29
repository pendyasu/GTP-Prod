Imports System.Data.OleDb
Imports EncryptionClass

Public Class frmCreateUser

    Friend dbUser As String = "c:\Users\tuand\Documents\Visual Studio 2010\Projects\GWSFTP\GWSFTP\Database\UserDB.mdb"
    Friend cnnstrUser As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbUser & ";Jet OLEDB:Database Password=jignesh0;"

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim username As String = TextBox_UserName.Text.Trim
        Dim password As String = TextBox_PassWord.Text.Trim
        Dim re_password As String = TextBox_RePassword.Text.Trim

        Dim FTPUserName As String = TextBox_FTPUserName.Text.Trim
        Dim FTPPassWord As String = TextBox_FTPPassWord.Text.Trim
        Dim re_FTPPassWord As String = TextBox_ReFTPPassWord.Text.Trim


        Dim FTPServer As String = TextBox_FTPServer.Text.Trim
        Dim teamname As String = TextBox_TeamName.Text.Trim
        Dim LastName = TextBox_LastName.Text.Trim
        Dim FirstName = TextBox_FirstName.Text.Trim

        Dim email As String = TextBox_Email.Text.Trim
        Dim phone As String = TextBox_Phone.Text

        If username = "" Or password = "" Or teamname = "" Or LastName = "" Or FirstName = "" Or email = "" Or phone = "" Then
            Return
        Else

            If password <> re_password Then
                MessageBox.Show("Password and Re enter Password are not matched")
                Return
            End If

            If FTPPassWord <> re_FTPPassWord Then
                MessageBox.Show("FTP Password and Re enter FTP Password are not matched")
                Return
            End If

            'Encrypting passwords using Advanced Encryption Standard (AES) algorithm
            Dim encrypted_FTPPassWord As String = ""
            Dim encrypted_password = EncryptionClass.Class1.Encrypt(password)
            If FTPUserName <> "" Then
                encrypted_FTPPassWord = EncryptionClass.Class1.Encrypt(FTPPassWord)
            End If


            'Insert User information to USERDB.mdb databases
            Dim cnn As OleDbConnection


            cnn = New OleDbConnection
            cnn.ConnectionString = cnnstrUser


            Dim cmd As OleDbCommand
            cmd = New OleDbCommand
            cmd.Connection = cnn
            cmd.CommandText = "INSERT INTO tblUser([username],[password],[ftpusername],[ftppass],[ftpserver],[teamname],[lastname],[firstname],[email],[phone])" & _
                " VALUES ('" & username & "','" & encrypted_password & "','" & FTPUserName & "','" & encrypted_FTPPassWord & "','" & FTPServer & "','" & teamname & "','" & LastName & "','" & FirstName & "','" & email & "','" & phone & "' ) "
            cnn.Open()
            cmd.ExecuteNonQuery()
            cmd.Dispose()


        End If


    End Sub
End Class