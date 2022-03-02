Imports System.Drawing
Imports System.IO

Friend Class ThumbnailImage
    'Implements IDisposable

    Private Structure XYSize
        Friend xLocShift, yLocShift, width, height As Integer

        Friend Sub New(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
            xLocShift = x
            yLocShift = y
            width = w
            height = h
        End Sub
    End Structure

    Private Structure ItemFRMName
        Friend frmName As String
        Friend IsInventory As Boolean
    End Structure

    Friend Shared InventImage As Dictionary(Of String, Image) = New Dictionary(Of String, Image)
    Friend Shared ItemsImage As Dictionary(Of String, Image) = New Dictionary(Of String, Image)
    Friend Shared CrittersImage As Dictionary(Of String, Image) = New Dictionary(Of String, Image)

    Private Shared itemsFidName() As ItemFRMName
    Private Shared crittersFidName() As String

    Friend Shared Sub GetCrittersImages()
        Dim extractFiles As List(Of String) = New List(Of String)

        Dim count As Integer = Critter_LST.Length - 1
        ReDim crittersFidName(count)

        For n As Integer = 0 To count
            GetCritterImage(n, extractFiles)
        Next

        If extractFiles.Count > 0 Then
            Main.PrintLog("Extraction frm files....")
            Progress_Form.ShowProgressBar(extractFiles.Count)

            DatFiles.UnpackedFilesByList(extractFiles, "cache\temp\")

            Main.PrintLog("Done" & vbCrLf & "Converting frm files.")
            Application.DoEvents()

            Directory.CreateDirectory(Cache_Patch & ART_CRITTERS_PATH)

            For Each image As String In extractFiles
                Shell(WorkAppDIR & "\frm2gif.exe -p color.pal .\cache\temp\" & image, AppWinStyle.Hide, True)

                Dim iFile As String = image.Remove(image.Length - 4)
                Dim scrFile As String = Cache_Patch & "\temp\" & iFile & "_sw.gif"
                Dim dstFile As String = Cache_Patch & "\" & iFile & ".gif"

                If File.Exists(scrFile) Then
                    File.Move(scrFile, dstFile)
                Else
                    scrFile = Cache_Patch & "\temp\" & iFile & ".gif"
                    If File.Exists(scrFile) Then
                        File.Move(scrFile, dstFile)
                    End If
                End If
                Progress_Form.ProgressIncrement()
            Next

            On Error Resume Next
            Directory.Delete(Cache_Patch & "\temp\", True)
            Progress_Form.Close()
        End If

    End Sub

    Private Shared Sub GetCritterImage(ByVal pNum As Integer, ByVal extractFiles As List(Of String))

        Dim iName As String = ProFiles.GetCritterFID(pNum)
        If iName Is Nothing Then Exit Sub

        If CrittersImage.ContainsKey(iName) Then GoTo setImage

        Dim pathF As String = ART_CRITTERS_PATH & iName & "aa.frm"
        If Not CheckDirFile(pathF, False) AndAlso Not (File.Exists(Cache_Patch & Path.ChangeExtension(pathF, ".gif"))) Then
            pathF = pathF.Substring(1)
            If extractFiles.Contains(pathF) = False Then
                extractFiles.Add(pathF)
            End If
        End If
setImage:
        crittersFidName(pNum) = iName
    End Sub

    Friend Shared Sub GetItemsImages()
        Dim extractFiles As List(Of String) = New List(Of String)

        Dim count As Integer = Items_LST.Length - 1
        ReDim itemsFidName(count)

        For n As Integer = 0 To count
            GetItemImages(n, extractFiles)
        Next

        If extractFiles.Count > 0 Then
            Main.PrintLog("Extraction frm files....")
            Progress_Form.ShowProgressBar(extractFiles.Count)

            DatFiles.UnpackedFilesByList(extractFiles, "cache\")

            Main.PrintLog("Done" & vbCrLf & "Converting frm files.")
            Application.DoEvents()

            For Each image As String In extractFiles
                Shell(WorkAppDIR & "\frm2gif.exe -p color.pal .\cache\" & image, AppWinStyle.Hide, True)
                Progress_Form.ProgressIncrement()
            Next

            On Error Resume Next
            FileSystem.Kill(WorkAppDIR & "\cache\art\inven\*.frm")
            FileSystem.Kill(WorkAppDIR & "\cache\art\items\*.frm")
            Progress_Form.Close()
        End If

    End Sub

    Private Shared Sub GetItemImages(ByVal pNum As Integer, ByVal extractFiles As List(Of String))
        Dim Inventory As Boolean = True
        Dim iName As String = ProFiles.GetItemInvenFID(pNum, Inventory)
        If iName Is Nothing Then Exit Sub

        Dim pathF As String
        If Inventory Then
            If InventImage.ContainsKey(iName) Then GoTo setImage
            pathF = ART_INVEN & iName
        Else
            If ItemsImage.ContainsKey(iName) Then GoTo setImage
            pathF = ART_ITEMS & iName
        End If

        If Not CheckDirFile(pathF, False) AndAlso Not (File.Exists(Cache_Patch & Path.ChangeExtension(pathF, ".gif"))) Then
            pathF = pathF.Substring(1)
            If extractFiles.Contains(pathF) = False Then extractFiles.Add(pathF)
        End If
setImage:
        itemsFidName(pNum).frmName = iName
        itemsFidName(pNum).IsInventory = Inventory
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    Friend Shared Function GetDrawItemImage(ByVal pNum As Integer) As Image
        Dim img As Image = Nothing

        Dim iName As String = itemsFidName(pNum).frmName
        If iName Is Nothing Then Return img

        Dim pathF As String = Cache_Patch

        If itemsFidName(pNum).IsInventory Then
            If InventImage.TryGetValue(iName, img) = False Then
                Dim name = iName.Remove(iName.Length - 4)
                pathF &= ART_INVEN & name & ".gif"
                If Not File.Exists(pathF) Then
                    DatFiles.ItemFrmToGif("inven\", name)
                End If
                If File.Exists(pathF) Then
                    img = Image.FromFile(pathF)
                Else
                    Main.PrintLog("Error convert: " + pathF)
                    img = My.Resources.RESERVAA
                End If
                InventImage.Add(iName, img)
            End If
        Else
            If ItemsImage.TryGetValue(iName, img) = False Then
                Dim name = iName.Remove(iName.Length - 4)
                pathF &= ART_ITEMS & name & ".gif"
                If Not File.Exists(pathF) Then
                    DatFiles.ItemFrmToGif("items\", name)
                End If
                If File.Exists(pathF) Then
                    img = Image.FromFile(pathF)
                Else
                    Main.PrintLog("Error convert: " + pathF)
                    img = My.Resources.RESERVAA
                End If
                ItemsImage.Add(iName, img)
            End If
        End If
        Return img
    End Function

    Friend Shared Function GetDrawCritterImage(ByVal pNum As Integer) As Image
        Dim img As Image = Nothing
        Dim pathF As String = Cache_Patch

        Dim iName As String = crittersFidName(pNum)
        If iName Is Nothing Then Return img

        If CrittersImage.TryGetValue(iName, img) = False Then
            pathF &= ART_CRITTERS_PATH & iName & "aa.gif"
            If Not File.Exists(pathF) Then
                DatFiles.CritterFrmToGif(iName)
            End If
            If File.Exists(pathF) Then img = Image.FromFile(pathF)
            CrittersImage.Add(iName, img)
        End If

        Return img
    End Function

    Private Shared Function FitImage(ByVal dstWidth As Integer, ByVal dstHeight As Integer, ByVal imgWidth As Integer, ByVal imgHeight As Integer) As XYSize
        Dim xLocShift, yLocShift, w, h As Integer
        w = imgWidth
        h = imgHeight

        If (imgWidth <= dstWidth AndAlso imgHeight <= dstHeight) Then   ' Размер изображение меньше окна
            xLocShift = CInt((dstWidth - imgWidth) / 2)
            yLocShift = CInt((dstHeight - imgHeight) / 2)
        ElseIf (imgWidth / imgHeight > dstWidth / dstHeight) Then       ' Размер изображения больше чем окно
            h = CInt(imgHeight / imgWidth * dstWidth)
            yLocShift = CInt((dstHeight - h) / 2)
            If (yLocShift < 0) Then yLocShift = 0
            w = dstWidth
        Else                                                            ' Размер изображения более узкий чем окно
            w = CInt(imgWidth / imgHeight * dstHeight)
            xLocShift = CInt((dstWidth - w) / 2)
            If (xLocShift < 0) Then xLocShift = 0
            h = dstHeight
        End If

        Return New XYSize(xLocShift, yLocShift, w, h)
    End Function

    Private Shared drawStringFormat As StringFormat = New StringFormat() With {
        .Trimming = StringTrimming.EllipsisCharacter,
        .FormatFlags = StringFormatFlags.NoWrap
    }

    Friend Shared Sub DrawListItem(ByVal e As DrawListViewItemEventArgs, ByVal img As Image, ByVal customText As String, ByVal scrFont As Font, ByVal isEdit As Boolean)

        e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        e.Graphics.CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        e.Graphics.TextRenderingHint = Text.TextRenderingHint.ClearTypeGridFit
        e.Graphics.TextContrast = 0

        'If img Is Nothing Then img = My.Resources.RESERVAA

        ' Draw image
        If img IsNot Nothing Then
            Dim fitImage As XYSize = ThumbnailImage.FitImage(e.Bounds.Width, e.Bounds.Height, img.Width, img.Height)
            e.Graphics.DrawImage(img, e.Bounds.X + fitImage.xLocShift, e.Bounds.Y + fitImage.yLocShift, fitImage.width, fitImage.height)
        End If

        ' Draw text
        Dim font As Font = New Font(scrFont, FontStyle.Bold)
        Dim rectText As Rectangle = New Rectangle(e.Bounds.X + 1, e.Bounds.Bottom - 16, e.Bounds.Width, e.Bounds.Height)
        e.Graphics.DrawString(e.Item.Text, font, Brushes.Black, rectText, drawStringFormat)
        rectText.Offset(-1, 1)
        e.Graphics.DrawString(e.Item.Text, font, Brushes.LawnGreen, rectText, drawStringFormat)

        Dim typeColor As Brush = If((e.State And ListViewItemStates.Focused) <> 0, Brushes.Yellow, Brushes.DarkGray)
        e.Graphics.DrawString(customText, scrFont, typeColor, e.Bounds.Location)

        ' Draw marker
        If isEdit Then e.Graphics.FillEllipse(Brushes.RoyalBlue, e.Bounds.X + e.Bounds.Width - 12, e.Bounds.Y + 5, 6, 6)

        ' Draw box
        Dim rectBox As Rectangle = New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1)
        Dim penColor As Pen = If(e.Item.Focused, Pens.OrangeRed, Pens.DarkGreen)
        e.Graphics.DrawRectangle(penColor, rectBox)

    End Sub

    Friend Shared Sub Dispose()
        For Each item In ThumbnailImage.InventImage
            If item.Value Is Nothing Then Continue For
            item.Value.Dispose()
        Next
        For Each item In ThumbnailImage.ItemsImage
            If item.Value Is Nothing Then Continue For
            item.Value.Dispose()
        Next
        For Each item In ThumbnailImage.CrittersImage
            If item.Value Is Nothing Then Continue For
            item.Value.Dispose()
        Next
        ThumbnailImage.InventImage.Clear()
        ThumbnailImage.ItemsImage.Clear()
        ThumbnailImage.CrittersImage.Clear()
    End Sub

End Class
