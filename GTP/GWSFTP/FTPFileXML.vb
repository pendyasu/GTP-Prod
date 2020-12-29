Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization


Public Class FTPFileXML

    Public XMLFileName As String
    Public FileGID As Guid
    Public id_server_file As Integer
    Public id_server_transaction As Integer
    Public id_client_transaction As Integer
    Public file_name As String
    Public date_created As DateTime
    Public date_finished As DateTime
    Public is_finished As Boolean
    Public file_status As Integer
    'Public time_to_PRO As DateTime
    'Public time_Unzipped As DateTime
    Public status_flag As Integer
    Public file_name_only As String
    Public dbname As String
    Public bytestrnsfrd As String
    Public uploadtime As String
    Public isScanner As Boolean
    Public isAsideOrphan As Boolean

    Public Function InsertFile() As Boolean

        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(XMLFileName)
        Dim FTPElement As XmlElement
        Dim FileNode As XmlNode
        Dim RootElement As XmlElement
        RootElement = xmlDoc.DocumentElement

        FileNode = xmlDoc.SelectSingleNode("tblFTPTransaction/tblFTPFile[@file_name_only='" + file_name_only + "']")
        If (IsNothing(FileNode)) Then
            FTPElement = xmlDoc.CreateElement("tblFTPFile")
            FTPElement.SetAttribute("FileGID", FileGID.ToString())
            FTPElement.SetAttribute("id_server_file", id_server_file.ToString())
            FTPElement.SetAttribute("id_server_transaction", id_server_transaction.ToString())
            FTPElement.SetAttribute("id_client_transaction", id_client_transaction)
            FTPElement.SetAttribute("file_name", file_name)
            If date_created <> Nothing Then
                FTPElement.SetAttribute("date_created", Common.getDateString(date_created))
            Else
                FTPElement.SetAttribute("date_created", date_created.ToString())
            End If

            If date_finished <> Nothing Then
                FTPElement.SetAttribute("date_finished", Common.getDateString(date_finished))
            Else
                FTPElement.SetAttribute("date_finished", date_finished.ToString())
            End If


            FTPElement.SetAttribute("is_finished", is_finished.ToString())
            FTPElement.SetAttribute("file_status", file_status.ToString())
            FTPElement.SetAttribute("status_flag", status_flag)
            FTPElement.SetAttribute("file_name_only", file_name_only)
            FTPElement.SetAttribute("dbname", dbname)
            FTPElement.SetAttribute("bytestrnsfrd", bytestrnsfrd)
            FTPElement.SetAttribute("uploadtime", uploadtime)
            FTPElement.SetAttribute("isScanner", isScanner)
            FTPElement.SetAttribute("isAsideOrphan", isAsideOrphan)
            FTPElement.SetAttribute("BSideDeviceType", "")

            RootElement.AppendChild(FTPElement)
            xmlDoc.Save(XMLFileName)

        End If


        Return True
    End Function


    Public Function UpdateFile() As Boolean
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(XMLFileName)
        Dim FTPElement As XmlNode

        FTPElement = xmlDoc.SelectSingleNode("tblFTPTransaction/tblFTPFile[@FileGID='" + FileGID.ToString() + "']")
        If Not IsNothing(FTPElement) Then
            FTPElement.Attributes("id_server_file").Value = id_server_file.ToString()
            FTPElement.Attributes("id_server_transaction").Value = id_server_transaction.ToString()
            FTPElement.Attributes("id_client_transaction").Value = id_client_transaction.ToString()
            FTPElement.Attributes("file_name").Value = file_name.ToString()
            FTPElement.Attributes("date_finished").Value = date_finished.ToString()
            FTPElement.Attributes("is_finished").Value = is_finished.ToString()
            FTPElement.Attributes("file_status").Value = file_status.ToString()
            FTPElement.Attributes("status_flag").Value = status_flag.ToString()
            FTPElement.Attributes("file_name_only").Value = file_name_only
            FTPElement.Attributes("dbname").Value = dbname.ToString()
            FTPElement.Attributes("uploadtime").Value = uploadtime
            xmlDoc.Save(XMLFileName)
        End If


        Return True
    End Function


End Class
