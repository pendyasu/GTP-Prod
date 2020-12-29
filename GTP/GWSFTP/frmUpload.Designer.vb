<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUpload
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUpload))
        Me.TextBox_Password = New System.Windows.Forms.TextBox()
        Me.CheckBox_SaveCredential = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ComboBox_Market = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ComboBox_Drive = New System.Windows.Forms.ComboBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.Label_Info = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CheckBox_Duplicate = New System.Windows.Forms.CheckBox()
        Me.TextBox_UserName = New System.Windows.Forms.ComboBox()
        Me.TextBox_Server = New System.Windows.Forms.ComboBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.CheckBox_NewMarket = New System.Windows.Forms.CheckBox()
        Me.Button_NewMarket = New System.Windows.Forms.Button()
        Me.ComboBox_PendingMarket = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBox_Project = New System.Windows.Forms.ComboBox()
        Me.Label_id_local_transaction = New System.Windows.Forms.Label()
        Me.FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Size = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ListView_FileList = New System.Windows.Forms.ListView()
        Me.Button_SelectFolder = New System.Windows.Forms.Button()
        Me.Label_TotalFile = New System.Windows.Forms.Label()
        Me.Timer_TotalFile = New System.Windows.Forms.Timer(Me.components)
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ComboBox_FileType = New System.Windows.Forms.ComboBox()
        Me.ProgressBar_Upload = New System.Windows.Forms.ProgressBar()
        Me.Label_Progress = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label_TotalFailed = New System.Windows.Forms.Label()
        Me.Timer_Upload = New System.Windows.Forms.Timer(Me.components)
        Me.Label_Version = New System.Windows.Forms.Label()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.TextBox_FTPSessionLog = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox_Action = New System.Windows.Forms.GroupBox()
        Me.RadioButton_Close = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Delete = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Continue = New System.Windows.Forms.RadioButton()
        Me.ListView_OtherFiles = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ToolTip_Delete = New System.Windows.Forms.ToolTip(Me.components)
        Me.ToolTip_CLose = New System.Windows.Forms.ToolTip(Me.components)
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Label_CurrentFile = New System.Windows.Forms.Label()
        Me.ProgressBar_FileProgress = New System.Windows.Forms.ProgressBar()
        Me.Label_UploadSpeed = New System.Windows.Forms.Label()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.Label_CurrentFile1 = New System.Windows.Forms.Label()
        Me.ProgressBar_FileProgress1 = New System.Windows.Forms.ProgressBar()
        Me.Label_UploadSpeed1 = New System.Windows.Forms.Label()
        Me.Button_CloseTransaction = New System.Windows.Forms.Button()
        Me.Label_InternetStatus = New System.Windows.Forms.Label()
        Me.Button_CheckInternet = New System.Windows.Forms.Button()
        Me.Button_Upload = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button_DeleteUploadedFiles = New System.Windows.Forms.Button()
        Me.BackgroundWorker2 = New System.ComponentModel.BackgroundWorker()
        Me.CheckBox_Mode = New System.Windows.Forms.CheckBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage_UnzipFailed = New System.Windows.Forms.TabPage()
        Me.Label_OtherFiles = New System.Windows.Forms.Label()
        Me.TabPage_DuplicateFile = New System.Windows.Forms.TabPage()
        Me.ListView_Duplicate = New System.Windows.Forms.ListView()
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage_TrimmingFailed = New System.Windows.Forms.TabPage()
        Me.ListView_FileTrimmingFailed = New System.Windows.Forms.ListView()
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage_Dups = New System.Windows.Forms.TabPage()
        Me.ListView_Dups = New System.Windows.Forms.ListView()
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage_MissingFiles = New System.Windows.Forms.TabPage()
        Me.ListView_MissingFile = New System.Windows.Forms.ListView()
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label_Mode = New System.Windows.Forms.Label()
        Me.Timer_UploadPcap = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox_PCAP_Options = New System.Windows.Forms.GroupBox()
        Me.RadioButton_SQZ = New System.Windows.Forms.RadioButton()
        Me.RadioButton_SQZ_PCAP = New System.Windows.Forms.RadioButton()
        Me.BackgroundWorker_Unzip = New System.ComponentModel.BackgroundWorker()
        Me.CheckBox_EnableSFTP = New System.Windows.Forms.CheckBox()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.GroupBox_Action.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage_UnzipFailed.SuspendLayout()
        Me.TabPage_DuplicateFile.SuspendLayout()
        Me.TabPage_TrimmingFailed.SuspendLayout()
        Me.TabPage_Dups.SuspendLayout()
        Me.TabPage_MissingFiles.SuspendLayout()
        Me.GroupBox_PCAP_Options.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBox_Password
        '
        Me.TextBox_Password.Location = New System.Drawing.Point(512, 10)
        Me.TextBox_Password.Name = "TextBox_Password"
        Me.TextBox_Password.Size = New System.Drawing.Size(148, 20)
        Me.TextBox_Password.TabIndex = 10
        Me.TextBox_Password.UseSystemPasswordChar = True
        '
        'CheckBox_SaveCredential
        '
        Me.CheckBox_SaveCredential.AutoSize = True
        Me.CheckBox_SaveCredential.Checked = True
        Me.CheckBox_SaveCredential.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox_SaveCredential.Location = New System.Drawing.Point(731, 5)
        Me.CheckBox_SaveCredential.Name = "CheckBox_SaveCredential"
        Me.CheckBox_SaveCredential.Size = New System.Drawing.Size(154, 17)
        Me.CheckBox_SaveCredential.TabIndex = 11
        Me.CheckBox_SaveCredential.Text = "Remember FTP credentials"
        Me.CheckBox_SaveCredential.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(450, 15)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Market:"
        '
        'ComboBox_Market
        '
        Me.ComboBox_Market.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBox_Market.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBox_Market.FormattingEnabled = True
        Me.ComboBox_Market.Location = New System.Drawing.Point(512, 11)
        Me.ComboBox_Market.Name = "ComboBox_Market"
        Me.ComboBox_Market.Size = New System.Drawing.Size(281, 21)
        Me.ComboBox_Market.TabIndex = 14
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(259, 12)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(57, 13)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Campaign:"
        '
        'ComboBox_Drive
        '
        Me.ComboBox_Drive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_Drive.FormattingEnabled = True
        Me.ComboBox_Drive.Location = New System.Drawing.Point(323, 10)
        Me.ComboBox_Drive.Name = "ComboBox_Drive"
        Me.ComboBox_Drive.Size = New System.Drawing.Size(121, 21)
        Me.ComboBox_Drive.TabIndex = 16
        '
        'Timer1
        '
        Me.Timer1.Interval = 6000
        '
        'Timer2
        '
        Me.Timer2.Interval = 2000
        '
        'Label_Info
        '
        Me.Label_Info.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Info.ForeColor = System.Drawing.Color.Blue
        Me.Label_Info.Location = New System.Drawing.Point(10, 2)
        Me.Label_Info.Name = "Label_Info"
        Me.Label_Info.Size = New System.Drawing.Size(373, 15)
        Me.Label_Info.TabIndex = 18
        Me.Label_Info.Text = "Upload data to FTP server"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.CheckBox_Duplicate)
        Me.Panel2.Controls.Add(Me.TextBox_UserName)
        Me.Panel2.Controls.Add(Me.TextBox_Server)
        Me.Panel2.Controls.Add(Me.TextBox_Password)
        Me.Panel2.Controls.Add(Me.CheckBox_SaveCredential)
        Me.Panel2.Location = New System.Drawing.Point(11, 22)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(955, 47)
        Me.Panel2.TabIndex = 23
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(450, 11)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(56, 13)
        Me.Label7.TabIndex = 55
        Me.Label7.Text = "Password:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(259, 11)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(58, 13)
        Me.Label6.TabIndex = 54
        Me.Label6.Text = "Username:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 13)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 53
        Me.Label3.Text = "Server:"
        '
        'CheckBox_Duplicate
        '
        Me.CheckBox_Duplicate.AutoSize = True
        Me.CheckBox_Duplicate.Checked = True
        Me.CheckBox_Duplicate.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox_Duplicate.Location = New System.Drawing.Point(731, 26)
        Me.CheckBox_Duplicate.Name = "CheckBox_Duplicate"
        Me.CheckBox_Duplicate.Size = New System.Drawing.Size(182, 17)
        Me.CheckBox_Duplicate.TabIndex = 52
        Me.CheckBox_Duplicate.Text = "Exclude files previously uploaded"
        Me.CheckBox_Duplicate.UseVisualStyleBackColor = True
        '
        'TextBox_UserName
        '
        Me.TextBox_UserName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TextBox_UserName.FormattingEnabled = True
        Me.TextBox_UserName.Location = New System.Drawing.Point(323, 11)
        Me.TextBox_UserName.Name = "TextBox_UserName"
        Me.TextBox_UserName.Size = New System.Drawing.Size(121, 21)
        Me.TextBox_UserName.TabIndex = 16
        '
        'TextBox_Server
        '
        Me.TextBox_Server.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TextBox_Server.FormattingEnabled = True
        Me.TextBox_Server.Items.AddRange(New Object() {"upload.mygws.com", "download.mygws.com"})
        Me.TextBox_Server.Location = New System.Drawing.Point(57, 11)
        Me.TextBox_Server.Name = "TextBox_Server"
        Me.TextBox_Server.Size = New System.Drawing.Size(196, 21)
        Me.TextBox_Server.TabIndex = 15
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel3.Controls.Add(Me.CheckBox_NewMarket)
        Me.Panel3.Controls.Add(Me.Button_NewMarket)
        Me.Panel3.Controls.Add(Me.ComboBox_PendingMarket)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.ComboBox_Project)
        Me.Panel3.Controls.Add(Me.Label_id_local_transaction)
        Me.Panel3.Controls.Add(Me.ComboBox_Market)
        Me.Panel3.Controls.Add(Me.Label4)
        Me.Panel3.Controls.Add(Me.Label5)
        Me.Panel3.Controls.Add(Me.ComboBox_Drive)
        Me.Panel3.Location = New System.Drawing.Point(11, 72)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(955, 40)
        Me.Panel3.TabIndex = 24
        '
        'CheckBox_NewMarket
        '
        Me.CheckBox_NewMarket.AutoSize = True
        Me.CheckBox_NewMarket.Enabled = False
        Me.CheckBox_NewMarket.Location = New System.Drawing.Point(791, 3)
        Me.CheckBox_NewMarket.Name = "CheckBox_NewMarket"
        Me.CheckBox_NewMarket.Size = New System.Drawing.Size(157, 17)
        Me.CheckBox_NewMarket.TabIndex = 20
        Me.CheckBox_NewMarket.Text = "Enter New or Select Market"
        Me.CheckBox_NewMarket.UseVisualStyleBackColor = True
        '
        'Button_NewMarket
        '
        Me.Button_NewMarket.Location = New System.Drawing.Point(815, 10)
        Me.Button_NewMarket.Name = "Button_NewMarket"
        Me.Button_NewMarket.Size = New System.Drawing.Size(126, 23)
        Me.Button_NewMarket.TabIndex = 22
        Me.Button_NewMarket.Text = "Enter New Market"
        Me.Button_NewMarket.UseVisualStyleBackColor = True
        Me.Button_NewMarket.Visible = False
        '
        'ComboBox_PendingMarket
        '
        Me.ComboBox_PendingMarket.FormattingEnabled = True
        Me.ComboBox_PendingMarket.Location = New System.Drawing.Point(511, 10)
        Me.ComboBox_PendingMarket.Name = "ComboBox_PendingMarket"
        Me.ComboBox_PendingMarket.Size = New System.Drawing.Size(282, 21)
        Me.ComboBox_PendingMarket.TabIndex = 23
        Me.ComboBox_PendingMarket.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(43, 13)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Project:"
        '
        'ComboBox_Project
        '
        Me.ComboBox_Project.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_Project.FormattingEnabled = True
        Me.ComboBox_Project.Location = New System.Drawing.Point(57, 10)
        Me.ComboBox_Project.Name = "ComboBox_Project"
        Me.ComboBox_Project.Size = New System.Drawing.Size(196, 21)
        Me.ComboBox_Project.TabIndex = 19
        '
        'Label_id_local_transaction
        '
        Me.Label_id_local_transaction.AutoSize = True
        Me.Label_id_local_transaction.Location = New System.Drawing.Point(886, 12)
        Me.Label_id_local_transaction.Name = "Label_id_local_transaction"
        Me.Label_id_local_transaction.Size = New System.Drawing.Size(39, 13)
        Me.Label_id_local_transaction.TabIndex = 17
        Me.Label_id_local_transaction.Text = "Label6"
        Me.Label_id_local_transaction.Visible = False
        '
        'FileName
        '
        Me.FileName.Text = "FileName"
        Me.FileName.Width = 465
        '
        'Size
        '
        Me.Size.Text = "Size"
        Me.Size.Width = 74
        '
        'ListView_FileList
        '
        Me.ListView_FileList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.FileName, Me.Size})
        Me.ListView_FileList.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_FileList.FullRowSelect = True
        Me.ListView_FileList.Location = New System.Drawing.Point(13, 233)
        Me.ListView_FileList.Name = "ListView_FileList"
        Me.ListView_FileList.Size = New System.Drawing.Size(551, 143)
        Me.ListView_FileList.TabIndex = 4
        Me.ListView_FileList.UseCompatibleStateImageBehavior = False
        Me.ListView_FileList.View = System.Windows.Forms.View.Details
        '
        'Button_SelectFolder
        '
        Me.Button_SelectFolder.Location = New System.Drawing.Point(220, 6)
        Me.Button_SelectFolder.Name = "Button_SelectFolder"
        Me.Button_SelectFolder.Size = New System.Drawing.Size(122, 30)
        Me.Button_SelectFolder.TabIndex = 2
        Me.Button_SelectFolder.Text = "Select Upload Files"
        Me.Button_SelectFolder.UseVisualStyleBackColor = True
        '
        'Label_TotalFile
        '
        Me.Label_TotalFile.AutoSize = True
        Me.Label_TotalFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_TotalFile.ForeColor = System.Drawing.Color.Blue
        Me.Label_TotalFile.Location = New System.Drawing.Point(0, 3)
        Me.Label_TotalFile.Name = "Label_TotalFile"
        Me.Label_TotalFile.Size = New System.Drawing.Size(74, 13)
        Me.Label_TotalFile.TabIndex = 26
        Me.Label_TotalFile.Text = "Total File(s)"
        '
        'Timer_TotalFile
        '
        Me.Timer_TotalFile.Interval = 2000
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(397, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(95, 30)
        Me.Button1.TabIndex = 27
        Me.Button1.Text = "Select Folder"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ComboBox_FileType
        '
        Me.ComboBox_FileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_FileType.FormattingEnabled = True
        Me.ComboBox_FileType.Items.AddRange(New Object() {".sqc/.sqz/.mf only", ".sqc/.sqz/.mf/images only", ".sqz/.mf only", ".sqz only", ".mf only", ".log only", ".wnd only", ".wnu only", ".wnl only", ".trp only", ".txt only", ".nmf/.gpx only", ".pcapzip only", ".sqc only", "All image/video types", "All file types"})
        Me.ComboBox_FileType.Location = New System.Drawing.Point(498, 6)
        Me.ComboBox_FileType.Name = "ComboBox_FileType"
        Me.ComboBox_FileType.Size = New System.Drawing.Size(163, 21)
        Me.ComboBox_FileType.TabIndex = 28
        '
        'ProgressBar_Upload
        '
        Me.ProgressBar_Upload.Location = New System.Drawing.Point(753, 3)
        Me.ProgressBar_Upload.Name = "ProgressBar_Upload"
        Me.ProgressBar_Upload.Size = New System.Drawing.Size(193, 20)
        Me.ProgressBar_Upload.TabIndex = 29
        Me.ProgressBar_Upload.Visible = False
        '
        'Label_Progress
        '
        Me.Label_Progress.AutoSize = True
        Me.Label_Progress.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Progress.ForeColor = System.Drawing.Color.Blue
        Me.Label_Progress.Location = New System.Drawing.Point(665, 6)
        Me.Label_Progress.MaximumSize = New System.Drawing.Size(100, 0)
        Me.Label_Progress.Name = "Label_Progress"
        Me.Label_Progress.Size = New System.Drawing.Size(78, 26)
        Me.Label_Progress.TabIndex = 30
        Me.Label_Progress.Text = "Transaction Progress"
        Me.Label_Progress.Visible = False
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.Button_SelectFolder)
        Me.Panel4.Controls.Add(Me.Button1)
        Me.Panel4.Controls.Add(Me.ComboBox_FileType)
        Me.Panel4.Controls.Add(Me.Label_TotalFile)
        Me.Panel4.Controls.Add(Me.Label_TotalFailed)
        Me.Panel4.Controls.Add(Me.Label_Progress)
        Me.Panel4.Controls.Add(Me.ProgressBar_Upload)
        Me.Panel4.Location = New System.Drawing.Point(12, 118)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(954, 40)
        Me.Panel4.TabIndex = 31
        '
        'Label_TotalFailed
        '
        Me.Label_TotalFailed.AutoSize = True
        Me.Label_TotalFailed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_TotalFailed.ForeColor = System.Drawing.Color.Red
        Me.Label_TotalFailed.Location = New System.Drawing.Point(0, 20)
        Me.Label_TotalFailed.Name = "Label_TotalFailed"
        Me.Label_TotalFailed.Size = New System.Drawing.Size(74, 13)
        Me.Label_TotalFailed.TabIndex = 36
        Me.Label_TotalFailed.Text = "Total Failed"
        '
        'Timer_Upload
        '
        Me.Timer_Upload.Interval = 500
        '
        'Label_Version
        '
        Me.Label_Version.AutoSize = True
        Me.Label_Version.Location = New System.Drawing.Point(763, 6)
        Me.Label_Version.Name = "Label_Version"
        Me.Label_Version.Size = New System.Drawing.Size(0, 13)
        Me.Label_Version.TabIndex = 33
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        '
        'TextBox_FTPSessionLog
        '
        Me.TextBox_FTPSessionLog.BackColor = System.Drawing.Color.White
        Me.TextBox_FTPSessionLog.Location = New System.Drawing.Point(570, 252)
        Me.TextBox_FTPSessionLog.Multiline = True
        Me.TextBox_FTPSessionLog.Name = "TextBox_FTPSessionLog"
        Me.TextBox_FTPSessionLog.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox_FTPSessionLog.Size = New System.Drawing.Size(396, 168)
        Me.TextBox_FTPSessionLog.TabIndex = 34
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Blue
        Me.Label1.Location = New System.Drawing.Point(570, 233)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(94, 16)
        Me.Label1.TabIndex = 35
        Me.Label1.Text = "Session Log"
        '
        'GroupBox_Action
        '
        Me.GroupBox_Action.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox_Action.Controls.Add(Me.RadioButton_Close)
        Me.GroupBox_Action.Controls.Add(Me.RadioButton_Delete)
        Me.GroupBox_Action.Controls.Add(Me.RadioButton_Continue)
        Me.GroupBox_Action.Enabled = False
        Me.GroupBox_Action.Location = New System.Drawing.Point(573, 482)
        Me.GroupBox_Action.Name = "GroupBox_Action"
        Me.GroupBox_Action.Size = New System.Drawing.Size(393, 53)
        Me.GroupBox_Action.TabIndex = 37
        Me.GroupBox_Action.TabStop = False
        Me.GroupBox_Action.Text = "Select an Action"
        '
        'RadioButton_Close
        '
        Me.RadioButton_Close.AutoSize = True
        Me.RadioButton_Close.Location = New System.Drawing.Point(251, 23)
        Me.RadioButton_Close.Name = "RadioButton_Close"
        Me.RadioButton_Close.Size = New System.Drawing.Size(110, 17)
        Me.RadioButton_Close.TabIndex = 2
        Me.RadioButton_Close.TabStop = True
        Me.RadioButton_Close.Text = "Close Transaction"
        Me.RadioButton_Close.UseVisualStyleBackColor = True
        '
        'RadioButton_Delete
        '
        Me.RadioButton_Delete.AutoSize = True
        Me.RadioButton_Delete.Location = New System.Drawing.Point(120, 23)
        Me.RadioButton_Delete.Name = "RadioButton_Delete"
        Me.RadioButton_Delete.Size = New System.Drawing.Size(115, 17)
        Me.RadioButton_Delete.TabIndex = 1
        Me.RadioButton_Delete.TabStop = True
        Me.RadioButton_Delete.Text = "Delete Transaction"
        Me.RadioButton_Delete.UseVisualStyleBackColor = True
        '
        'RadioButton_Continue
        '
        Me.RadioButton_Continue.AutoSize = True
        Me.RadioButton_Continue.Location = New System.Drawing.Point(6, 23)
        Me.RadioButton_Continue.Name = "RadioButton_Continue"
        Me.RadioButton_Continue.Size = New System.Drawing.Size(104, 17)
        Me.RadioButton_Continue.TabIndex = 0
        Me.RadioButton_Continue.TabStop = True
        Me.RadioButton_Continue.Text = "Continue Upload"
        Me.RadioButton_Continue.UseVisualStyleBackColor = True
        '
        'ListView_OtherFiles
        '
        Me.ListView_OtherFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.ListView_OtherFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_OtherFiles.FullRowSelect = True
        Me.ListView_OtherFiles.Location = New System.Drawing.Point(7, 20)
        Me.ListView_OtherFiles.Name = "ListView_OtherFiles"
        Me.ListView_OtherFiles.Size = New System.Drawing.Size(530, 106)
        Me.ListView_OtherFiles.TabIndex = 38
        Me.ListView_OtherFiles.UseCompatibleStateImageBehavior = False
        Me.ListView_OtherFiles.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "FileName"
        Me.ColumnHeader1.Width = 465
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Size"
        Me.ColumnHeader2.Width = 74
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.White
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.Label_CurrentFile)
        Me.Panel5.Controls.Add(Me.ProgressBar_FileProgress)
        Me.Panel5.Controls.Add(Me.Label_UploadSpeed)
        Me.Panel5.Location = New System.Drawing.Point(13, 165)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(953, 28)
        Me.Panel5.TabIndex = 40
        '
        'Label_CurrentFile
        '
        Me.Label_CurrentFile.AutoSize = True
        Me.Label_CurrentFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_CurrentFile.ForeColor = System.Drawing.Color.Green
        Me.Label_CurrentFile.Location = New System.Drawing.Point(289, 5)
        Me.Label_CurrentFile.Name = "Label_CurrentFile"
        Me.Label_CurrentFile.Size = New System.Drawing.Size(66, 13)
        Me.Label_CurrentFile.TabIndex = 47
        Me.Label_CurrentFile.Text = "File progress"
        '
        'ProgressBar_FileProgress
        '
        Me.ProgressBar_FileProgress.BackColor = System.Drawing.Color.White
        Me.ProgressBar_FileProgress.ForeColor = System.Drawing.Color.Green
        Me.ProgressBar_FileProgress.Location = New System.Drawing.Point(752, 2)
        Me.ProgressBar_FileProgress.Name = "ProgressBar_FileProgress"
        Me.ProgressBar_FileProgress.Size = New System.Drawing.Size(193, 20)
        Me.ProgressBar_FileProgress.TabIndex = 46
        '
        'Label_UploadSpeed
        '
        Me.Label_UploadSpeed.AutoSize = True
        Me.Label_UploadSpeed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_UploadSpeed.ForeColor = System.Drawing.Color.Green
        Me.Label_UploadSpeed.Location = New System.Drawing.Point(3, 5)
        Me.Label_UploadSpeed.Name = "Label_UploadSpeed"
        Me.Label_UploadSpeed.Size = New System.Drawing.Size(104, 13)
        Me.Label_UploadSpeed.TabIndex = 45
        Me.Label_UploadSpeed.Text = "Label_UploadSpeed"
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.White
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.Label_CurrentFile1)
        Me.Panel6.Controls.Add(Me.ProgressBar_FileProgress1)
        Me.Panel6.Controls.Add(Me.Label_UploadSpeed1)
        Me.Panel6.Location = New System.Drawing.Point(13, 199)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(953, 28)
        Me.Panel6.TabIndex = 42
        '
        'Label_CurrentFile1
        '
        Me.Label_CurrentFile1.AutoSize = True
        Me.Label_CurrentFile1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_CurrentFile1.ForeColor = System.Drawing.Color.Green
        Me.Label_CurrentFile1.Location = New System.Drawing.Point(289, 4)
        Me.Label_CurrentFile1.Name = "Label_CurrentFile1"
        Me.Label_CurrentFile1.Size = New System.Drawing.Size(66, 13)
        Me.Label_CurrentFile1.TabIndex = 47
        Me.Label_CurrentFile1.Text = "File progress"
        '
        'ProgressBar_FileProgress1
        '
        Me.ProgressBar_FileProgress1.BackColor = System.Drawing.Color.White
        Me.ProgressBar_FileProgress1.ForeColor = System.Drawing.Color.Green
        Me.ProgressBar_FileProgress1.Location = New System.Drawing.Point(752, 3)
        Me.ProgressBar_FileProgress1.Name = "ProgressBar_FileProgress1"
        Me.ProgressBar_FileProgress1.Size = New System.Drawing.Size(193, 20)
        Me.ProgressBar_FileProgress1.TabIndex = 46
        '
        'Label_UploadSpeed1
        '
        Me.Label_UploadSpeed1.AutoSize = True
        Me.Label_UploadSpeed1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_UploadSpeed1.ForeColor = System.Drawing.Color.Green
        Me.Label_UploadSpeed1.Location = New System.Drawing.Point(3, 5)
        Me.Label_UploadSpeed1.Name = "Label_UploadSpeed1"
        Me.Label_UploadSpeed1.Size = New System.Drawing.Size(110, 13)
        Me.Label_UploadSpeed1.TabIndex = 45
        Me.Label_UploadSpeed1.Text = "Label_UploadSpeed1"
        '
        'Button_CloseTransaction
        '
        Me.Button_CloseTransaction.BackColor = System.Drawing.SystemColors.Control
        Me.Button_CloseTransaction.Location = New System.Drawing.Point(478, 545)
        Me.Button_CloseTransaction.Name = "Button_CloseTransaction"
        Me.Button_CloseTransaction.Size = New System.Drawing.Size(105, 34)
        Me.Button_CloseTransaction.TabIndex = 48
        Me.Button_CloseTransaction.Text = "Close Transaction"
        Me.Button_CloseTransaction.UseVisualStyleBackColor = False
        Me.Button_CloseTransaction.Visible = False
        '
        'Label_InternetStatus
        '
        Me.Label_InternetStatus.AutoSize = True
        Me.Label_InternetStatus.Location = New System.Drawing.Point(786, 552)
        Me.Label_InternetStatus.Name = "Label_InternetStatus"
        Me.Label_InternetStatus.Size = New System.Drawing.Size(117, 13)
        Me.Label_InternetStatus.TabIndex = 47
        Me.Label_InternetStatus.Text = "No Internet Connection"
        Me.Label_InternetStatus.Visible = False
        '
        'Button_CheckInternet
        '
        Me.Button_CheckInternet.BackColor = System.Drawing.SystemColors.Control
        Me.Button_CheckInternet.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button_CheckInternet.Location = New System.Drawing.Point(636, 544)
        Me.Button_CheckInternet.Name = "Button_CheckInternet"
        Me.Button_CheckInternet.Size = New System.Drawing.Size(105, 34)
        Me.Button_CheckInternet.TabIndex = 46
        Me.Button_CheckInternet.Text = "Check Internet"
        Me.Button_CheckInternet.UseVisualStyleBackColor = False
        Me.Button_CheckInternet.Visible = False
        '
        'Button_Upload
        '
        Me.Button_Upload.BackColor = System.Drawing.SystemColors.Control
        Me.Button_Upload.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button_Upload.Location = New System.Drawing.Point(10, 543)
        Me.Button_Upload.Name = "Button_Upload"
        Me.Button_Upload.Size = New System.Drawing.Size(113, 34)
        Me.Button_Upload.TabIndex = 43
        Me.Button_Upload.Text = "Upload"
        Me.Button_Upload.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.SystemColors.Control
        Me.Button2.Location = New System.Drawing.Point(159, 544)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(126, 33)
        Me.Button2.TabIndex = 44
        Me.Button2.Text = "Remove Selected File"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button_DeleteUploadedFiles
        '
        Me.Button_DeleteUploadedFiles.BackColor = System.Drawing.SystemColors.Control
        Me.Button_DeleteUploadedFiles.Enabled = False
        Me.Button_DeleteUploadedFiles.Location = New System.Drawing.Point(318, 544)
        Me.Button_DeleteUploadedFiles.Name = "Button_DeleteUploadedFiles"
        Me.Button_DeleteUploadedFiles.Size = New System.Drawing.Size(125, 35)
        Me.Button_DeleteUploadedFiles.TabIndex = 45
        Me.Button_DeleteUploadedFiles.Text = "Delete selected files"
        Me.Button_DeleteUploadedFiles.UseVisualStyleBackColor = False
        '
        'BackgroundWorker2
        '
        Me.BackgroundWorker2.WorkerReportsProgress = True
        Me.BackgroundWorker2.WorkerSupportsCancellation = True
        '
        'CheckBox_Mode
        '
        Me.CheckBox_Mode.AutoSize = True
        Me.CheckBox_Mode.Location = New System.Drawing.Point(702, 2)
        Me.CheckBox_Mode.Name = "CheckBox_Mode"
        Me.CheckBox_Mode.Size = New System.Drawing.Size(114, 17)
        Me.CheckBox_Mode.TabIndex = 50
        Me.CheckBox_Mode.Text = "Enable XML Mode"
        Me.CheckBox_Mode.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage_UnzipFailed)
        Me.TabControl1.Controls.Add(Me.TabPage_DuplicateFile)
        Me.TabControl1.Controls.Add(Me.TabPage_TrimmingFailed)
        Me.TabControl1.Controls.Add(Me.TabPage_Dups)
        Me.TabControl1.Controls.Add(Me.TabPage_MissingFiles)
        Me.TabControl1.Location = New System.Drawing.Point(13, 382)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(551, 157)
        Me.TabControl1.TabIndex = 51
        '
        'TabPage_UnzipFailed
        '
        Me.TabPage_UnzipFailed.Controls.Add(Me.Label_OtherFiles)
        Me.TabPage_UnzipFailed.Controls.Add(Me.ListView_OtherFiles)
        Me.TabPage_UnzipFailed.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_UnzipFailed.Name = "TabPage_UnzipFailed"
        Me.TabPage_UnzipFailed.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_UnzipFailed.Size = New System.Drawing.Size(543, 131)
        Me.TabPage_UnzipFailed.TabIndex = 0
        Me.TabPage_UnzipFailed.Text = "Unzip Failed"
        Me.TabPage_UnzipFailed.UseVisualStyleBackColor = True
        '
        'Label_OtherFiles
        '
        Me.Label_OtherFiles.AutoSize = True
        Me.Label_OtherFiles.Location = New System.Drawing.Point(9, 3)
        Me.Label_OtherFiles.Name = "Label_OtherFiles"
        Me.Label_OtherFiles.Size = New System.Drawing.Size(39, 13)
        Me.Label_OtherFiles.TabIndex = 52
        Me.Label_OtherFiles.Text = "Label3"
        '
        'TabPage_DuplicateFile
        '
        Me.TabPage_DuplicateFile.Controls.Add(Me.ListView_Duplicate)
        Me.TabPage_DuplicateFile.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_DuplicateFile.Name = "TabPage_DuplicateFile"
        Me.TabPage_DuplicateFile.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_DuplicateFile.Size = New System.Drawing.Size(543, 131)
        Me.TabPage_DuplicateFile.TabIndex = 1
        Me.TabPage_DuplicateFile.Text = "Files Previously Uploaded"
        Me.TabPage_DuplicateFile.UseVisualStyleBackColor = True
        '
        'ListView_Duplicate
        '
        Me.ListView_Duplicate.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader3, Me.ColumnHeader4})
        Me.ListView_Duplicate.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_Duplicate.FullRowSelect = True
        Me.ListView_Duplicate.Location = New System.Drawing.Point(6, 6)
        Me.ListView_Duplicate.Name = "ListView_Duplicate"
        Me.ListView_Duplicate.Size = New System.Drawing.Size(530, 106)
        Me.ListView_Duplicate.TabIndex = 39
        Me.ListView_Duplicate.UseCompatibleStateImageBehavior = False
        Me.ListView_Duplicate.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "FileName"
        Me.ColumnHeader3.Width = 465
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Size"
        Me.ColumnHeader4.Width = 74
        '
        'TabPage_TrimmingFailed
        '
        Me.TabPage_TrimmingFailed.Controls.Add(Me.ListView_FileTrimmingFailed)
        Me.TabPage_TrimmingFailed.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_TrimmingFailed.Name = "TabPage_TrimmingFailed"
        Me.TabPage_TrimmingFailed.Size = New System.Drawing.Size(543, 131)
        Me.TabPage_TrimmingFailed.TabIndex = 2
        Me.TabPage_TrimmingFailed.Text = "Corrupted Files"
        Me.TabPage_TrimmingFailed.UseVisualStyleBackColor = True
        '
        'ListView_FileTrimmingFailed
        '
        Me.ListView_FileTrimmingFailed.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader5, Me.ColumnHeader6})
        Me.ListView_FileTrimmingFailed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_FileTrimmingFailed.FullRowSelect = True
        Me.ListView_FileTrimmingFailed.Location = New System.Drawing.Point(6, 12)
        Me.ListView_FileTrimmingFailed.Name = "ListView_FileTrimmingFailed"
        Me.ListView_FileTrimmingFailed.Size = New System.Drawing.Size(530, 106)
        Me.ListView_FileTrimmingFailed.TabIndex = 40
        Me.ListView_FileTrimmingFailed.UseCompatibleStateImageBehavior = False
        Me.ListView_FileTrimmingFailed.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "FileName"
        Me.ColumnHeader5.Width = 465
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Size"
        Me.ColumnHeader6.Width = 74
        '
        'TabPage_Dups
        '
        Me.TabPage_Dups.Controls.Add(Me.ListView_Dups)
        Me.TabPage_Dups.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Dups.Name = "TabPage_Dups"
        Me.TabPage_Dups.Size = New System.Drawing.Size(543, 131)
        Me.TabPage_Dups.TabIndex = 3
        Me.TabPage_Dups.Text = "Duplicates"
        Me.TabPage_Dups.UseVisualStyleBackColor = True
        '
        'ListView_Dups
        '
        Me.ListView_Dups.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8})
        Me.ListView_Dups.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_Dups.FullRowSelect = True
        Me.ListView_Dups.Location = New System.Drawing.Point(6, 12)
        Me.ListView_Dups.Name = "ListView_Dups"
        Me.ListView_Dups.Size = New System.Drawing.Size(530, 106)
        Me.ListView_Dups.TabIndex = 40
        Me.ListView_Dups.UseCompatibleStateImageBehavior = False
        Me.ListView_Dups.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "FileName"
        Me.ColumnHeader7.Width = 465
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "Size"
        Me.ColumnHeader8.Width = 74
        '
        'TabPage_MissingFiles
        '
        Me.TabPage_MissingFiles.Controls.Add(Me.ListView_MissingFile)
        Me.TabPage_MissingFiles.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_MissingFiles.Name = "TabPage_MissingFiles"
        Me.TabPage_MissingFiles.Size = New System.Drawing.Size(543, 131)
        Me.TabPage_MissingFiles.TabIndex = 4
        Me.TabPage_MissingFiles.Text = "Missing Files"
        Me.TabPage_MissingFiles.UseVisualStyleBackColor = True
        '
        'ListView_MissingFile
        '
        Me.ListView_MissingFile.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader9, Me.ColumnHeader10})
        Me.ListView_MissingFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_MissingFile.FullRowSelect = True
        Me.ListView_MissingFile.Location = New System.Drawing.Point(6, 12)
        Me.ListView_MissingFile.Name = "ListView_MissingFile"
        Me.ListView_MissingFile.Size = New System.Drawing.Size(530, 106)
        Me.ListView_MissingFile.TabIndex = 41
        Me.ListView_MissingFile.UseCompatibleStateImageBehavior = False
        Me.ListView_MissingFile.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "FileName"
        Me.ColumnHeader9.Width = 465
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Size"
        Me.ColumnHeader10.Width = 74
        '
        'Label_Mode
        '
        Me.Label_Mode.AutoSize = True
        Me.Label_Mode.ForeColor = System.Drawing.Color.Red
        Me.Label_Mode.Location = New System.Drawing.Point(822, 4)
        Me.Label_Mode.Name = "Label_Mode"
        Me.Label_Mode.Size = New System.Drawing.Size(34, 13)
        Me.Label_Mode.TabIndex = 49
        Me.Label_Mode.Text = "Mode"
        '
        'Timer_UploadPcap
        '
        Me.Timer_UploadPcap.Interval = 1000
        '
        'GroupBox_PCAP_Options
        '
        Me.GroupBox_PCAP_Options.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox_PCAP_Options.Controls.Add(Me.RadioButton_SQZ)
        Me.GroupBox_PCAP_Options.Controls.Add(Me.RadioButton_SQZ_PCAP)
        Me.GroupBox_PCAP_Options.Location = New System.Drawing.Point(573, 426)
        Me.GroupBox_PCAP_Options.Name = "GroupBox_PCAP_Options"
        Me.GroupBox_PCAP_Options.Size = New System.Drawing.Size(393, 53)
        Me.GroupBox_PCAP_Options.TabIndex = 52
        Me.GroupBox_PCAP_Options.TabStop = False
        Me.GroupBox_PCAP_Options.Text = "Select a PCAP Upload Option (only applicable when SQZ contains PCAP data)"
        '
        'RadioButton_SQZ
        '
        Me.RadioButton_SQZ.AutoSize = True
        Me.RadioButton_SQZ.Location = New System.Drawing.Point(212, 24)
        Me.RadioButton_SQZ.Name = "RadioButton_SQZ"
        Me.RadioButton_SQZ.Size = New System.Drawing.Size(112, 17)
        Me.RadioButton_SQZ.TabIndex = 1
        Me.RadioButton_SQZ.Text = "Only SQZ/MF files"
        Me.RadioButton_SQZ.UseVisualStyleBackColor = True
        '
        'RadioButton_SQZ_PCAP
        '
        Me.RadioButton_SQZ_PCAP.AutoSize = True
        Me.RadioButton_SQZ_PCAP.Checked = True
        Me.RadioButton_SQZ_PCAP.Location = New System.Drawing.Point(6, 23)
        Me.RadioButton_SQZ_PCAP.Name = "RadioButton_SQZ_PCAP"
        Me.RadioButton_SQZ_PCAP.Size = New System.Drawing.Size(160, 17)
        Me.RadioButton_SQZ_PCAP.TabIndex = 0
        Me.RadioButton_SQZ_PCAP.TabStop = True
        Me.RadioButton_SQZ_PCAP.Text = "SQZ/MF + PCAPZIP files     "
        Me.RadioButton_SQZ_PCAP.UseVisualStyleBackColor = True
        '
        'BackgroundWorker_Unzip
        '
        '
        'CheckBox_EnableSFTP
        '
        Me.CheckBox_EnableSFTP.AutoSize = True
        Me.CheckBox_EnableSFTP.Location = New System.Drawing.Point(559, 2)
        Me.CheckBox_EnableSFTP.Name = "CheckBox_EnableSFTP"
        Me.CheckBox_EnableSFTP.Size = New System.Drawing.Size(89, 17)
        Me.CheckBox_EnableSFTP.TabIndex = 54
        Me.CheckBox_EnableSFTP.Text = "Enable SFTP"
        Me.CheckBox_EnableSFTP.UseVisualStyleBackColor = True
        '
        'frmUpload
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(970, 586)
        Me.Controls.Add(Me.CheckBox_EnableSFTP)
        Me.Controls.Add(Me.GroupBox_PCAP_Options)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.ListView_FileList)
        Me.Controls.Add(Me.TextBox_FTPSessionLog)
        Me.Controls.Add(Me.CheckBox_Mode)
        Me.Controls.Add(Me.Label_Mode)
        Me.Controls.Add(Me.Button_CloseTransaction)
        Me.Controls.Add(Me.Label_InternetStatus)
        Me.Controls.Add(Me.Button_CheckInternet)
        Me.Controls.Add(Me.Button_Upload)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button_DeleteUploadedFiles)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.GroupBox_Action)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label_Version)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Label_Info)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmUpload"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.GroupBox_Action.ResumeLayout(False)
        Me.GroupBox_Action.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage_UnzipFailed.ResumeLayout(False)
        Me.TabPage_UnzipFailed.PerformLayout()
        Me.TabPage_DuplicateFile.ResumeLayout(False)
        Me.TabPage_TrimmingFailed.ResumeLayout(False)
        Me.TabPage_Dups.ResumeLayout(False)
        Me.TabPage_MissingFiles.ResumeLayout(False)
        Me.GroupBox_PCAP_Options.ResumeLayout(False)
        Me.GroupBox_PCAP_Options.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox_Password As System.Windows.Forms.TextBox
    Friend WithEvents CheckBox_SaveCredential As System.Windows.Forms.CheckBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ComboBox_Drive As System.Windows.Forms.ComboBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents Label_Info As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents ComboBox_Market As System.Windows.Forms.ComboBox
    Friend WithEvents FileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Size As System.Windows.Forms.ColumnHeader
    Friend WithEvents ListView_FileList As System.Windows.Forms.ListView
    Friend WithEvents Button_SelectFolder As System.Windows.Forms.Button
    Friend WithEvents Button_NewMarket As System.Windows.Forms.Button
    Friend WithEvents ComboBox_PendingMarket As System.Windows.Forms.ComboBox
    Friend WithEvents Label_TotalFile As System.Windows.Forms.Label
    Friend WithEvents Timer_TotalFile As System.Windows.Forms.Timer
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ComboBox_FileType As System.Windows.Forms.ComboBox
    Friend WithEvents Label_id_local_transaction As System.Windows.Forms.Label
    Friend WithEvents ProgressBar_Upload As System.Windows.Forms.ProgressBar
    Friend WithEvents Label_Progress As System.Windows.Forms.Label
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Timer_Upload As System.Windows.Forms.Timer
    Friend WithEvents Label_Version As System.Windows.Forms.Label
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents TextBox_FTPSessionLog As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label_TotalFailed As System.Windows.Forms.Label
    Friend WithEvents TextBox_Server As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox_UserName As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox_Action As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButton_Continue As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_Close As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_Delete As System.Windows.Forms.RadioButton
    Friend WithEvents ListView_OtherFiles As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolTip_Delete As System.Windows.Forms.ToolTip
    Friend WithEvents ToolTip_CLose As System.Windows.Forms.ToolTip
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Label_CurrentFile As System.Windows.Forms.Label
    Friend WithEvents ProgressBar_FileProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents Label_UploadSpeed As System.Windows.Forms.Label
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Label_CurrentFile1 As System.Windows.Forms.Label
    Friend WithEvents ProgressBar_FileProgress1 As System.Windows.Forms.ProgressBar
    Friend WithEvents Label_UploadSpeed1 As System.Windows.Forms.Label
    Friend WithEvents Button_CloseTransaction As System.Windows.Forms.Button
    Friend WithEvents Label_InternetStatus As System.Windows.Forms.Label
    Friend WithEvents Button_CheckInternet As System.Windows.Forms.Button
    Friend WithEvents Button_Upload As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button_DeleteUploadedFiles As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker2 As System.ComponentModel.BackgroundWorker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ComboBox_Project As System.Windows.Forms.ComboBox
    Friend WithEvents CheckBox_Mode As System.Windows.Forms.CheckBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage_UnzipFailed As System.Windows.Forms.TabPage
    Friend WithEvents TabPage_DuplicateFile As System.Windows.Forms.TabPage
    Friend WithEvents ListView_Duplicate As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label_OtherFiles As System.Windows.Forms.Label
    Friend WithEvents CheckBox_Duplicate As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label_Mode As System.Windows.Forms.Label
    Friend WithEvents CheckBox_NewMarket As System.Windows.Forms.CheckBox
    Friend WithEvents TabPage_TrimmingFailed As System.Windows.Forms.TabPage
    Friend WithEvents ListView_FileTrimmingFailed As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TabPage_Dups As System.Windows.Forms.TabPage
    Friend WithEvents ListView_Dups As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Timer_UploadPcap As System.Windows.Forms.Timer
    Friend WithEvents GroupBox_PCAP_Options As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButton_SQZ As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_SQZ_PCAP As System.Windows.Forms.RadioButton
    Friend WithEvents BackgroundWorker_Unzip As System.ComponentModel.BackgroundWorker
    Friend WithEvents CheckBox_EnableSFTP As System.Windows.Forms.CheckBox
    Friend WithEvents TabPage_MissingFiles As System.Windows.Forms.TabPage
    Friend WithEvents ListView_MissingFile As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader9 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader10 As System.Windows.Forms.ColumnHeader

End Class
