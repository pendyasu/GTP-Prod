<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPCAP
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
        Me.ListView_FileList = New System.Windows.Forms.ListView()
        Me.FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Size = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Button_SelectFile = New System.Windows.Forms.Button()
        Me.Button_ProcessPCAP = New System.Windows.Forms.Button()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.Button_Select_Folder = New System.Windows.Forms.Button()
        Me.Status = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'ListView_FileList
        '
        Me.ListView_FileList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.FileName, Me.Size, Me.Status})
        Me.ListView_FileList.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView_FileList.FullRowSelect = True
        Me.ListView_FileList.Location = New System.Drawing.Point(12, 66)
        Me.ListView_FileList.Name = "ListView_FileList"
        Me.ListView_FileList.Size = New System.Drawing.Size(635, 351)
        Me.ListView_FileList.TabIndex = 5
        Me.ListView_FileList.UseCompatibleStateImageBehavior = False
        Me.ListView_FileList.View = System.Windows.Forms.View.Details
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
        'Button_SelectFile
        '
        Me.Button_SelectFile.Location = New System.Drawing.Point(12, 27)
        Me.Button_SelectFile.Name = "Button_SelectFile"
        Me.Button_SelectFile.Size = New System.Drawing.Size(75, 23)
        Me.Button_SelectFile.TabIndex = 6
        Me.Button_SelectFile.Text = "Select Files"
        Me.Button_SelectFile.UseVisualStyleBackColor = True
        '
        'Button_ProcessPCAP
        '
        Me.Button_ProcessPCAP.Location = New System.Drawing.Point(238, 441)
        Me.Button_ProcessPCAP.Name = "Button_ProcessPCAP"
        Me.Button_ProcessPCAP.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Button_ProcessPCAP.Size = New System.Drawing.Size(130, 23)
        Me.Button_ProcessPCAP.TabIndex = 7
        Me.Button_ProcessPCAP.Text = "Create PCAPZIP Files"
        Me.Button_ProcessPCAP.UseVisualStyleBackColor = True
        '
        'BackgroundWorker1
        '
        '
        'Button_Select_Folder
        '
        Me.Button_Select_Folder.Location = New System.Drawing.Point(116, 27)
        Me.Button_Select_Folder.Name = "Button_Select_Folder"
        Me.Button_Select_Folder.Size = New System.Drawing.Size(150, 23)
        Me.Button_Select_Folder.TabIndex = 8
        Me.Button_Select_Folder.Text = "Select Folder (SQZ only)"
        Me.Button_Select_Folder.UseVisualStyleBackColor = True
        '
        'Status
        '
        Me.Status.Text = "Status"
        Me.Status.Width = 79
        '
        'frmPCAP
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(666, 476)
        Me.Controls.Add(Me.Button_Select_Folder)
        Me.Controls.Add(Me.Button_ProcessPCAP)
        Me.Controls.Add(Me.Button_SelectFile)
        Me.Controls.Add(Me.ListView_FileList)
        Me.Name = "frmPCAP"
        Me.Text = "Create PCAPZIP Files"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListView_FileList As System.Windows.Forms.ListView
    Friend WithEvents FileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Size As System.Windows.Forms.ColumnHeader
    Friend WithEvents Button_SelectFile As System.Windows.Forms.Button
    Friend WithEvents Button_ProcessPCAP As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents Button_Select_Folder As System.Windows.Forms.Button
    Friend WithEvents Status As System.Windows.Forms.ColumnHeader
End Class
