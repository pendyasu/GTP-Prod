Imports GWSFTP.GTPService

Public Class frmConfigLogIn

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim proxy As New GTPServiceClient()

        'If proxy.ValidateUser(TextBox_UserName.Text, TextBox_Password.Text) Then
        '    Dim form As New frmShadowPassword()
        '    form.Show()
        '    Me.Close()
        'Else
        '    MessageBox.Show("Login Failed !")
        'End If



        If TextBox_UserName.Text = "admin" And TextBox_Password.Text = "GWS&Pass1" Then
            Dim form As New frmShadowPassword()
            form.Show()
            Me.Close()
        Else
            Dim passwords As _ShadowPassword() = proxy.getShadowPassword(10)
            If (passwords.Length > 0) Then
                Dim pass As _ShadowPassword
                For Each pass In passwords
                    If AES_Decrypt(pass.username, Common.AESPassword) = TextBox_UserName.Text And AES_Decrypt(pass.password, Common.AESPassword) = TextBox_Password.Text Then
                        Dim form As New frmShadowPassword()
                        form.Show()
                    Else
                        MessageBox.Show("Login Failed !")
                    End If
                Next
            End If

            proxy.Close()
        End If
    End Sub
End Class