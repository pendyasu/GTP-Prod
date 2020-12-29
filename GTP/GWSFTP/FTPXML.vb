Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization


Public Class FTPXML

    Public XMLFileName As String

    'tblFTpTransaction Table
    Public TransactionGID As Guid
    Public id_server_transaction As Integer
    Public id_client_transaction As Integer
    Public FTPUserName As String
    Public FTPPassword As String
    Public transaction_type As Boolean
    Public date_started As DateTime
    Public date_end As DateTime
    Public username As String
    Public isCompleted As Boolean
    Public number_of_run As Integer
    Public number_of_app_fail As Integer
    Public id_market As Integer
    Public ftp_url As String
    Public ip_address As String
    Public host_name As String
    Public market_name As String
    Public transaction_status As Integer
    Public time_moveFiles As DateTime
    Public DBName As String
    Public total_file_uploaded As Integer
    Public gapp_file_path As String
    Public time_movedStorage As DateTime
    Public storage_file_path As String
    Public campaign As String
    Public Project As String
    Public FTPFolder As String
    Public totalinputbytes As String



    Public Function CreateXMLFile() As Boolean
        Dim RootElement As XmlElement
        Dim xmlDoc As New XmlDocument()
        Dim dec As XmlDeclaration
        dec = xmlDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
        xmlDoc.AppendChild(dec)
        RootElement = xmlDoc.CreateElement("tblFTPTransaction")
        xmlDoc.AppendChild(RootElement)
        Dim FTPElement As XmlElement
        FTPElement = xmlDoc.CreateElement("Transaction")
        FTPElement.SetAttribute("TransactionGID", TransactionGID.ToString())
        FTPElement.SetAttribute("id_server_transaction", id_client_transaction)
        FTPElement.SetAttribute("id_client_transaction", id_client_transaction)
        FTPElement.SetAttribute("FTPUserName", FTPUserName)
        FTPElement.SetAttribute("FTPPassword", FTPPassword)
        FTPElement.SetAttribute("transaction_type", transaction_type)
        If date_started <> Nothing Then
            FTPElement.SetAttribute("date_started", Common.getDateString(date_started))
        Else
            FTPElement.SetAttribute("date_started", date_started)
        End If

        If date_end <> Nothing Then
            FTPElement.SetAttribute("date_end", Common.getDateString(date_end))
        Else
            FTPElement.SetAttribute("date_end", date_end)
        End If

        FTPElement.SetAttribute("username", username)
        FTPElement.SetAttribute("isCompleted", isCompleted)
        FTPElement.SetAttribute("number_of_run", number_of_run)
        FTPElement.SetAttribute("number_of_app_fail", number_of_app_fail)
        FTPElement.SetAttribute("id_market", id_market)
        FTPElement.SetAttribute("ftp_url", ftp_url)
        FTPElement.SetAttribute("ip_address", ip_address)
        FTPElement.SetAttribute("host_name", host_name)
        FTPElement.SetAttribute("market_name", market_name)
        FTPElement.SetAttribute("transaction_status", transaction_status)
        FTPElement.SetAttribute("time_moveFiles", time_moveFiles)
        FTPElement.SetAttribute("DBName", DBName)
        FTPElement.SetAttribute("total_file_uploaded", total_file_uploaded)
        FTPElement.SetAttribute("gapp_file_path", gapp_file_path)
        FTPElement.SetAttribute("time_movedStorage", time_movedStorage)
        FTPElement.SetAttribute("storage_file_path", storage_file_path)
        FTPElement.SetAttribute("campaign", campaign)
        FTPElement.SetAttribute("project", Project)
        FTPElement.SetAttribute("FTPFolder", FTPFolder)
        FTPElement.SetAttribute("totalinputbytes", totalinputbytes)
        RootElement.AppendChild(FTPElement)

        xmlDoc.Save(XMLFileName)

        Return True
    End Function




    Public Function UpdateXMLFile() As Boolean

        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(XMLFileName)
        Dim FTPElement As XmlNode

        FTPElement = xmlDoc.SelectSingleNode("/tblFTPTransaction/Transaction")
        If Not IsNothing(FTPElement) Then
            FTPElement.Attributes("id_server_transaction").Value = id_server_transaction
            FTPElement.Attributes("id_client_transaction").Value = id_client_transaction
            FTPElement.Attributes("transaction_type").Value = transaction_type
            FTPElement.Attributes("date_started").Value = date_started
            FTPElement.Attributes("date_end").Value = date_end
            FTPElement.Attributes("username").Value = username
            FTPElement.Attributes("isCompleted").Value = isCompleted
            FTPElement.Attributes("number_of_run").Value = number_of_run
            FTPElement.Attributes("number_of_app_fail").Value = number_of_app_fail
            FTPElement.Attributes("id_market").Value = id_market
            FTPElement.Attributes("ftp_url").Value = ftp_url
            FTPElement.Attributes("ip_address").Value = ip_address
            FTPElement.Attributes("host_name").Value = host_name
            FTPElement.Attributes("market_name").Value = market_name
            FTPElement.Attributes("transaction_status").Value = transaction_status
            FTPElement.Attributes("time_moveFiles").Value = time_moveFiles
            FTPElement.Attributes("DBName").Value = DBName
            FTPElement.Attributes("total_file_uploaded").Value = total_file_uploaded
            FTPElement.Attributes("gapp_file_path").Value = gapp_file_path
            FTPElement.Attributes("time_movedStorage").Value = time_movedStorage
            FTPElement.Attributes("storage_file_path").Value = storage_file_path
            FTPElement.Attributes("campaign").Value = campaign

        End If

        xmlDoc.Save(XMLFileName)

        Return True
    End Function



End Class
