
Imports System.Net
Imports System.IO
Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.Net.Mail

Public Class frmUploadHistory
    Friend dbFile As String = AppDomain.CurrentDomain.BaseDirectory & "\Database\FTPDB.mdb"
    Friend cnnstr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbFile & ";Jet OLEDB:Database Password=jignesh0;"

    Private Sub frmUploadHistory_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cnn As OleDbConnection
        cnn = New OleDbConnection
        cnn.ConnectionString = cnnstr
        Dim cmd As OleDbCommand
        Dim dr As OleDbDataReader
        cmd = New OleDbCommand
        cmd.Connection = cnn
        cmd.CommandText = "SELECT * FROM tblFTpServer "
        cnn.Open()
        dr = cmd.ExecuteReader()
        While dr.Read
            TextBox_Server.Text = dr("ftpserver")
            TextBox_UserName.Text = dr("ftpusername")
            Dim ftppass As String = dr("ftppassword")
            TextBox_Password.Text = EncryptionClass.Class1.Decrypt(ftppass)
        End While

        GetMarketList()
    End Sub

    Private Sub GetMarketList()
        ComboBox_Market.Items.Clear()
        Dim username As String = TextBox_UserName.Text.Trim()
        Dim response As String
        response = Common.GetHTTPData(website_root & "GWSFTP/getmarketlist_per_user.aspx?username=" + username, "", "")

        '>aaaaAlan Garmanbbbb<
        Dim regex As New Regex(">aaaa" + "(.*)" + "bbbb<")
        Dim market_list = regex.Matches(response)
        Dim market_name As Match
        For Each market_name In market_list
            Dim aa As String = market_name.Value.Substring(5, market_name.Length - 10)
            ComboBox_Market.Items.Add(aa)
        Next

        ComboBox_Market.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox_Market.AutoCompleteSource = AutoCompleteSource.ListItems
    End Sub

    Private Sub Button_ViewFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_ViewFiles.Click

    End Sub

    Private Sub ComboBox_Market_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox_Market.SelectedIndexChanged

    End Sub
End Class