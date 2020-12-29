Imports EncryptionClass
Imports System.Data.OleDb
Imports GWSFTP.GTPService
Imports System.Net.Mail
Imports System.Net
Imports Chilkat
Imports System.IO


Public Class frmPendingTransaction


    Private Sub frmPendingTransaction_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        GetPendingTransactions()

    End Sub


    Private Sub GetPendingTransactions()

        Dim dbFile As String = AppDomain.CurrentDomain.BaseDirectory & "\Database\FTPDB.mdb"
        Dim cnnstr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbFile & ";Jet OLEDB:Database Password=jignesh0;"


        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr

        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        Dim id_transaction As Integer = 0
        Dim ftp_url As String = ""
        Dim i As Integer



        cmd = New OleDbCommand
        cmd.Connection = cnn

      

        If cnn.State <> ConnectionState.Open Then
            cnn.Open()
        End If

        cmd.CommandText = "SELECT * FROM tblTransactionInfo WHERE status_transaction<> 2 and isCompleted=0 ORDER BY date_started "
        dr = cmd.ExecuteReader()
        If dr.HasRows Then
            While dr.Read()
                id_transaction = Convert.ToInt32(dr("id_transaction"))
                Dim transaction_info As String
                transaction_info = id_transaction.ToString() + "-" + Convert.ToString(dr("market_name")) + "-" + Convert.ToString(dr("campaign")) _
                + "-" + Convert.ToString(dr("date_started"))

                ListBox_PendingTrans.Items.Add(transaction_info)

            End While
        End If

    End Sub


    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Dim str As String = ListBox_PendingTrans.SelectedItem.ToString()
        '306-Ardmore OK-12D2-10/22/2013 12:00:00 AM

        Dim strList As String() = str.Split("-")
        Common.currentIDLocalTransaction = strList(0)

        Dim uploadForm As New frmUpload()
        uploadForm.Show()
        Me.Close()

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs)
        Me.Close()
    End Sub
End Class