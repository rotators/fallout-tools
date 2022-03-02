Module Misc

    ''' <summary>
    ''' Удаляет пустые строки в конце массива и лишние пробелы в строке.
    ''' </summary>
    Friend Function ClearEmptyLines(ByRef lst As String()) As String()
        Dim count As Integer = UBound(lst)
        Dim n As Integer
        For n = 0 To count
            lst(n) = lst(n).Trim
        Next
        For n = count To 0 Step -1
            If (lst(n).Length > 0) Then
                Exit For
            End If
        Next
        If (count <> n) Then
            ReDim Preserve lst(n)
        End If
        Return lst
    End Function

    Friend Function SearchLW(ByVal n As Integer, ByVal lstView As ListView, ByVal text As String) As Boolean
        text = text.Trim
        For i = n To lstView.Items.Count - 1
            If lstView.Items(i).Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) <> -1 Then
                lstView.Items.Item(i).Selected = True
                lstView.Items.Item(i).Focused = True
                Return True
            End If
        Next
        Return False
    End Function

    Friend Function SearchLWBack(ByVal n As Integer, ByVal lstView As ListView, ByVal text As String) As Boolean
        text = text.Trim
        For i = n - 1 To 0 Step -1
            If lstView.Items(i).Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) <> -1 Then
                lstView.Items.Item(i).Selected = True
                lstView.Items.Item(i).Focused = True
                Return True
            End If
        Next
        Return False
    End Function

    'Возвращает Имя или Описание предмета из msg файла
    Friend Function GetNameItemMsg(ByVal NameID As Integer, Optional ByVal Desc As Boolean = False) As String
        If Desc Then NameID += 1
        Messages.GetMsgData("pro_item.msg")

        Return Messages.GetNameObject(NameID)
    End Function

End Module
