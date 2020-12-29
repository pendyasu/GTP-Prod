Imports System.IO
Imports System.Data.OleDb

Public Class GWSTransaction

    Public ftp_url As String    
    Public cnn As OleDbConnection
    Public transaction_type As Boolean
    Public username As String
    Public id_market As Integer
    Public id_transaction As Integer
    Public market_name As String
    Public Campaign As String
    Public Project As String


    Public Sub UpdateXMLMode(ByVal id_transaction As Integer, ByVal TransactionGID As String)
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "UPDATE tblTransactionInfo SET [isXMLMode] = 1, [TransactionGID]='" + TransactionGID + "' WHERE id_transaction = " + id_transaction.ToString()
        cnn.Open()
        cmd.ExecuteNonQuery()
        cmd.Dispose()
        cnn.Close()

    End Sub

    Public Function CreateTransaction(ByVal _FileList() As String) As Integer

        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        'market_name = market_name.Replace("'", "")
        cmd.CommandText = "INSERT INTO tblTransactionInfo(transaction_type,username,id_market,ftp_url,market_name,campaign,Project) values (" & transaction_type & ",'" & username & "'," & id_market & ",'" & ftp_url & "','" & market_name.Replace("'", "''") & "','" & Campaign & "','" & Project.Replace("'", "''") & "')"
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
        For i = 0 To _FileList.Length - 1
            Try
                cmd.CommandText = "INSERT INTO tblFileInfo(id_transaction,file_name) values (" & id_transaction.ToString() & ",'" & _FileList(i).Replace("'", "''") & "')"
                cmd.Connection = cnn
                cmd.ExecuteNonQuery()
                ' cmd.Dispose()
            Catch ex As Exception
            Finally
                cmd.Dispose()

            End Try
          
        Next
        cnn.Close()
        Return id_transaction
    End Function

    Public Sub UpdateFileStatus(ByVal file_name As String)
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "UPDATE tblFileInfo SET [is_finished] = True, [date_finished]=Date() WHERE [id_transaction] = " & id_transaction & " AND [file_name] = '" & file_name.Replace("'", "''") & "'"
        cnn.Open()
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Public Sub Update_Id_server_Transaction(ByVal id_local_transaction As Integer, ByVal id_server_transaction As Integer)
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "UPDATE tblTransactionInfo SET [id_server_transaction] = " + id_server_transaction.ToString + " WHERE [id_transaction] = " & id_local_transaction.ToString
        cnn.Open()
        cmd.ExecuteNonQuery()
        cmd.Dispose()
        cnn.Close()
    End Sub

    Public Function DeleteTransactionInfo(ByVal id_server_transaction As String)
        Dim cmd As OleDbCommand
        cmd = New OleDbCommand
        Try
            cnn.Open()
            cmd.Connection = cnn
            cmd.CommandText = "UPDATE tblFileInfo SET file_status =2 WHERE id_transaction = " + id_server_transaction
            cmd.ExecuteNonQuery()
            cmd.CommandText = "UPDATE tblTransactionInfo SET status_transaction=2 WHERE id_transaction = " + id_server_transaction
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            cnn.Close()

            'Delete transaction on server
            'Dim delete_url = website_root & "GWSFTP/delete_transaction.aspx?id_server_transaction=" + id_server_transaction
            'Common.GetHTTPData(delete_url, "", "")
            'MessageBox.Show("Transaction has been successfully deleted")
        Catch ex As Exception
            MessageBox.Show("Error ! Transaction has not been deleted. Try again")
        End Try

        Return 0
    End Function

End Class
