Imports GWSFTP.GTPService

Public Class frmShadowPassword

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim proxy As New GTPServiceClient()
        Dim ShadowUserName As String = TextBox_UserName.Text.Trim()
        Dim ShadowPass As String = TextBox_ShadowPassword.Text.Trim()
        Dim ShadowPass1 As String = TextBox_ShadowPassword1.Text.Trim()

        If ShadowPass = ShadowPass1 Then
            ShadowUserName = AES_Encrypt(ShadowUserName, Common.AESPassword)
            ShadowPass = AES_Encrypt(ShadowPass, Common.AESPassword)
            Dim shadowPassObj As New _ShadowPassword()
            shadowPassObj.username = ShadowUserName

            shadowPassObj.password = ShadowPass
            If CheckBox_Active.Checked Then
                shadowPassObj.isActive = 1
            Else
                shadowPassObj.isActive = 0
            End If

            shadowPassObj.date_created = DateTime.Now
            If ComboBox1.Text = "Stand Alone" Then
                shadowPassObj.type = 1
            Else
                shadowPassObj.type = 2
            End If

            Dim result As String = proxy.ShadowPassword(0, shadowPassObj)
            If result = "1" Then
                MessageBox.Show("Successfully Added Shadow Credential")
            End If
        Else
            MessageBox.Show("Failed Adding Shadow Credential")
        End If
        proxy.Close()

    End Sub

    Private Sub Button_Update_Click(sender As System.Object, e As System.EventArgs) Handles Button_Update.Click
        Dim proxy As New GTPServiceClient()
        Dim ShadowUserName As String = TextBox_UserName.Text.Trim()
        Dim ShadowPass As String = TextBox_ShadowPassword.Text.Trim()
        Dim ShadowPass1 As String = TextBox_ShadowPassword1.Text.Trim()

        If ShadowPass = ShadowPass1 Then
            ShadowUserName = AES_Encrypt(ShadowUserName, Common.AESPassword)
            ShadowPass = AES_Encrypt(ShadowPass, Common.AESPassword)
            Dim shadowPassObj As New _ShadowPassword()
            shadowPassObj.username = ShadowUserName

            shadowPassObj.password = ShadowPass
            If CheckBox_Active.Checked Then
                shadowPassObj.isActive = 1
            Else
                shadowPassObj.isActive = 0
            End If

            shadowPassObj.date_created = DateTime.Now
            If ComboBox1.Text = "Stand Alone" Then
                shadowPassObj.type = 1
            Else
                shadowPassObj.type = 2
            End If
            Dim result As String = proxy.ShadowPassword(1, shadowPassObj)
            If result = "1" Then
                MessageBox.Show("Successfully Update Shadow Credential")
            Else
                MessageBox.Show("Failed Update Shadow Credential")
            End If
        Else
            MessageBox.Show("Passwords are not matched !")
        End If
        proxy.Close()
    End Sub
End Class