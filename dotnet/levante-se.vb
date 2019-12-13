Imports System
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text


Module Program
  Public todo1(20) As String
  Public filled1bis As Integer = 0
  Public todo3 As String = Nothing
  Dim todayurl = Nothing

  Sub Main(args As String())
      ' User language = Galician
      LoadAData
      If DateTime.Today.DayOfWeek = DayOfWeek.Saturday Then
          Dim stoerungen2 = janein1("¿Tráfico pesado na estrada")
          If  stoerungen2 Then
              todo3 = "Descubra a causa da estrada transitada"
          Else
        Console.WriteLine("nada")
          End If
      End If
      Console.WriteLine(VbLf)
      Planungsfragen
      Array.Resize( todo1,filled1bis+1 )
      left = todo1
      r = wForM.TodoPart2
      If Not istHeizperiode() Then 
          r = Array.FindAll( wForM.TodoPart2, Function(p As String ) IsNothing(p) OrElse Not p.StartsWith("Winter:") )
      End If
      dozweispaltigchecklist( todayurl )
      ' dozweispaltigchecklist( todo1, wForM.TodoPart2 )
      Console.WriteLine("Prema unha tecla para rematar o programa.")
      Console.ReadKey()
  End Sub
    
  Dim dataForM as DataForMorningP
  Dim wForM    as WorteForMorningP


  Private Sub LoadAData
    Dim fn = "jsonxmlm/data1_for_morning_galician.xml"
    Try
      For Teste = 1 To 5
        If not File.Exists(fn) Then fn = "../" & fn
      Next
      Dim stm As FileStream = New FileStream(fn, FileMode.Open)
      Dim xmlSer As XmlSerializer = New XmlSerializer(GetType(DataForMorningP))
      dataForM = CType(xmlSer.Deserialize(stm), DataForMorningP)
      stm.Close()    ' Testprint waere:   Console.WriteLine(dataForM.DayOfLastWalkOrJogging)
      Dim cd = new StringBuilder( Directory.GetCurrentDirectory() )
      Console.Write( cd )
      fn = "jsonxmlm/words_for_morning_galician.xml"
      For Teste = 1 To 5
        If not File.Exists(fn) Then 
          fn = "../" & fn
          Dim lin as Integer =   cd.ToString().LastIndexOf("\")
          If lin = -1 Then lin = cd.ToString().LastIndexOf("/")
          If Teste = 1 AndAlso lin > 4 Then
            cd.Length = lin
            Console.Write( VbCr & cd.ToString() )
          Else
            Console.Write("/..")
          End If
        End if        
      Next
      stm = New FileStream(fn, FileMode.Open)
      xmlSer = New XmlSerializer(GetType(WorteForMorningP))
      wForM = CType(xmlSer.Deserialize(stm), WorteForMorningP)
      stm.Close()    ' Testprint waere:   Console.WriteLine(wForM.VersionOfStruct)
      Console.WriteLine( "/jsonxmlm/*.xml" & " cargado" )
      If dataForM.WhereSundayToSaturday Is Nothing Then Console.WriteLine("Aviso:Non está presente WhereSundayToSaturday en data1_for_morning_galician.xml") 
      If wForM.URL_ofDay Is Nothing Then Console.WriteLine("Aviso: Non está presente URL_ofDay en data1_for_morning_galician.xml?")
      If dataForM.FruehsportModeArray Is Nothing Then Console.WriteLine("Aviso: Non está presente FruehsportModeArray en data1_for_morning_galician.xml?")
    Catch eee As Exception  
      errorAbbruch(fn & "   Erro:", eee)
    End Try 
  End Sub

  
  Function istHeizperiode()
    Dim mon = DateTime.Now.Month
    Dim day = DateTime.Now.Day
    If dataForM.WinterHeizAbMMDD < dataForM.WinterHeizBisMDD Then
      If dataForM.WinterHeizAbMMDD/100 < mon AndAlso mon < dataForM.WinterHeizBisMDD/100 Then Return true
      If dataForM.WinterHeizAbMMDD/100 = mon Then Return day > dataForM.WinterHeizAbMMDD mod 100
      If dataForM.WinterHeizBisMDD/100  = mon Then Return day < dataForM.WinterHeizBisMDD mod 100
      Return False
    Else
      If dataForM.WinterHeizAbMMDD/100 > mon OrElse mon < dataForM.WinterHeizBisMDD/100 Then Return true
      If dataForM.WinterHeizAbMMDD/100 = mon Then Return day > dataForM.WinterHeizAbMMDD mod 100
      If dataForM.WinterHeizBisMDD/100  = mon Then Return day < dataForM.WinterHeizBisMDD mod 100
      Return False
    End If
  End Function
  
  
  Sub Planungsfragen
    ' Setzt todo1, filled1bis, todayurl
    Dim day146097 As Integer = DateDiff(DateInterval.Day, GlobalConstants.base_sunday , Date.Now)
    day146097 = day146097 mod 146097
    Dim fs as FruehsportMode
    fs = BewegungsVorfestleg( day146097 , dataForM )
    ' FruehsportMode auf Keiner und FastImmer reduzieren
    if FruehsportMode.Wetterfrage = fs AndAlso janein1("¿Chove ou está baixo os -5 graos") then
      fs = FruehsportMode.Keiner
    Elseif FruehsportMode.Wetterfrage = fs then
      fs = FruehsportMode.FastImmer
    End if
    Console.WriteLine(VbLf)
    If DateTime.Now.Hour >= 7 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Monday Then
      filled1bis = filled1bis + 1
      todo1(filled1bis) = "Se é necesario, un lavaplatos"
    End If
    if fs = FruehsportMode.FastImmer Then
      Console.WriteLine("-----> " & "Hoxe mañá exercicio, logo")  
    Else
      filled1bis = filled1bis + 1
      todo1(filled1bis) = "Lavarse"
    End If
    If todo3 IsNot Nothing Then 
      filled1bis = filled1bis + 1
      todo1(filled1bis) = todo3
    End If
    If dataForM.LowCarbTeil.Length() > 0 Then
      Dim ur as String = dataForM.LowCarbTeil( day146097 mod dataForM.LowCarbTeil.Length() )
      filled1bis = filled1bis + 1
      todo1(filled1bis) = If( len(ur)<23, "Se é necesario " & ur, ur )     
    End If
    If fs <> FruehsportMode.FastImmer Then
      If DateTime.Today.DayOfWeek < dataForM.ZeitknapperAbTag OrElse DateTime.Today.DayOfWeek > dataForM.ZeitknapperBisTag Then
        filled1bis = filled1bis + 2
        todo1(filled1bis-1) = "Cociña o almorzo"
        todo1(filled1bis) = "Verifique a limpeza da roupa"
      End If
      filled1bis = filled1bis + 1
      todo1(filled1bis) = If( dataForM.LowCarbTeil.Length() > 0 , "Almorzo principal" , "Almorzo" )
    Else
      filled1bis = filled1bis + 1
      todo1(filled1bis) = If( day146097 And 1 > 0 , "Mini cuncha de torta" , "Pon o bollo de leite no peto" )
      ' Wenn man vor dem Frühstück Sport macht, sollte man trotzdem minimal Kohlenhydrate essen, da der Körper sonst Eiweiss verbrennt
    End If
    Dim work1 as LocationOfDayWork
    if dataForM.WhereSundayToSaturday.Length() <> 7 Then errorAbbruch( "WhereSundayToSaturday.Length()", Nothing )
    work1 = dataForM.WhereSundayToSaturday( day146097 mod 7 )
    Console.WriteLine( "-----> " & work1.ToString() ) 
    Console.WriteLine(VbLf)
    If wForM.URL_ofDay.Length() > 0 Then
      Dim url as String = wForM.URL_ofDay( day146097 mod wForM.URL_ofDay.Length() )
      Console.WriteLine( "-----> " & url ) 
      filled1bis = filled1bis + 1
      todo1(filled1bis) = If( len(url)<27, url, "Ver o URL anterior (U)")     
      todayurl = url
    End if  
    If DateTime.Now.Hour < 7 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Monday Then
      filled1bis = filled1bis + 1
      todo1(filled1bis) = "Acenda o lavaplatos se é necesario"
    End If
    if fs = FruehsportMode.FastImmer Then
      filled1bis = filled1bis + 1
      todo1(filled1bis) = "Poña roupa deportiva ao aire libre"     
    ElseIf work1 = LocationOfDayWork.EinkaufenUndHaushalt OrElse work1 = LocationOfDayWork.Externarbeit OrElse 
           work1=LocationOfDayWork.Fruehschicht OrElse work1=LocationOfDayWork.ExterneWeiterbildung Then
      filled1bis = filled1bis + 1
      todo1(filled1bis) = "Desgaste roupa para exteriores" 
    End If
  End Sub
 
 
  Function BewegungsVorfestleg( day146097 As Integer, regelstruct As DataForMorningP ) As FruehsportMode
    Dim laenge = regelstruct.FruehsportModeArray.Length()
    if 0=laenge Then Return FruehsportMode.FastImmer
    Dim fs as FruehsportMode
    fs = regelstruct.FruehsportModeArray( day146097 mod laenge )
    ' FruehsportMode auf Keiner, Wetterfrage und FastImmer reduzieren
    If FruehsportMode.Sommerhalbjahr = fs AndAlso DateTime.Today.Month >= 5 AndAlso DateTime.Today.Month <= 10 Then
      fs = FruehsportMode.FastImmer
    Elseif FruehsportMode.Sommerhalbjahr = fs then
      fs = FruehsportMode.Keiner
    Elseif FruehsportMode.WetterAbApril = fs AndAlso DateTime.Today.Month >= 4 then
      fs = FruehsportMode.Wetterfrage
    Elseif FruehsportMode.WetterAbApril = fs then
      fs = FruehsportMode.Keiner
    End If
    Return fs
  End Function

  Dim left() as String
  Dim r() as String

 
  Function janein1(text As String)
    Dim line As String
    Do
      Console.Write("{0} ? (s=y=j/n) ", text)
      line = Console.ReadKey(true).KeyChar 
    Loop Until "jnJNsSyY".Contains(line)
    If line.ToUpper() = "J"  OrElse line.ToUpper() = "S"  OrElse line.ToUpper() = "Y" then 
      Console.Write(" Si ")
    Else 
      Console.Write(" Non ")
    End If
    Console.WriteLine(" ")
    Return ( line.ToUpper() = "J" OrElse line.ToUpper() = "S"  OrElse line.ToUpper() = "Y"  )
  End Function
  
  
  Private Sub dozweispaltigchecklist( url As String )
  ' Private Sub dozweispaltigchecklist(left() As String,r() As String)
    Console.WriteLine( VbCrLf & " Responda de que lado se procesou (L ou R); e = l , d = r."& vbCrlf)
    If Not IsNothing(url) Then
      Console.WriteLine( "Entre U pode introducirse para chamar á URL."& vbCrlf)
    End If
    Dim lindex as Integer = 1
    Dim rindex as Integer = 1
    Dim line As String
    Console.WriteLine("Hai {0} + {1} elementos a procesar." & VbCrLf & VbCrLf & VbCrLf  & VbCrLf & VbCrLf & VbCrLf, left.Length-1, r.Length-1)
    Dim spaceline = new String( " "c, 78)
    Do While lindex < left.Length OrElse rindex < r.Length
     Console.Write(vbcr &  spaceline )
     Do
      If lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>73 then
        Console.Write(vbcr &  "{0}, {1} ? ", left(lindex).SubString(0,Math.Max(3,73-len(r(rindex)))) ,r(rindex))
      Elseif lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))>31 then
        Console.Write(vbcr &  "{0}, {1} ? ", left(lindex),r(rindex))
      ElseIf lindex < left.length AndAlso rindex < r.length then
        Console.Write(vbcr &  "{0},    {1} ?  (E/D) ", left(lindex),r(rindex))
      Elseif lindex < left.length  AndAlso len(left(lindex))>39  then
        Console.Write(vbcr &  "{0}, - ? (E) ", left(lindex))
      Elseif lindex < left.length then
        Console.Write(vbcr &  "{0},  logo prema E = L ? ", left(lindex))
      Else
        Console.Write(vbcr &  "Pulse R ou D despois do {0} ? ",  r(rindex))
      End if
      line = Console.ReadKey(true).KeyChar 
     Loop Until "eEdDlLrRuU".Contains(line)
     If line.ToUpper() = "R" OrElse line.ToUpper() = "D" then 
      rindex  = rindex  + 1
     Elseif line.ToUpper() = "U" then 
       Try
         If Not IsNothing(url) Then
           dim psi as new ProcessStartInfo(url)
           psi.UseShellExecute = true
           System.Diagnostics.Process.Start( psi )
         End If
       Catch ee As Exception
         errorAbbruch( url  , ee )
       End Try
     Else 
      lindex  = lindex  + 1
     End If
    Loop
    Console.WriteLine( VbCrLf & VbCrLf & "Fertig." & VbCrLf )
  End Sub


  Sub errorAbbruch( text1 as String, text2 as Exception )
      Console.WriteLine(text1)    
      if text2 IsNot Nothing Then Console.WriteLine(text2)
      Console.WriteLine("Prema unha tecla para rematar o programa.")
      Console.ReadKey()
      Environment.Exit(-1)
  End Sub
End Module



Public Module Interf
  Public Enum LocationOfDayWork
    Heimarbeit = 0            '
    HaushaltUndHeimarbeit = 1 '
    EinkaufenUndHaushalt = 2  '
    Externarbeit = 3          '
    Frei = 4                  '
    Fruehschicht = 5          '
    Lernen = 6                '
    ExterneWeiterbildung = 7  '
  End Enum
  
  
  Public Enum FruehsportMode
    Keiner
    Sommerhalbjahr
    Wetterfrage
    WetterAbApril
    FastImmer
  End Enum

  Public Structure DataForMorningP
    Public WhereSundayToSaturday() As LocationOfDayWork REM Anzahl 7 kann ich hier nicht festlegen.
    Public LowCarbTeil()  As String
    Public ZeitknapperAbTag As Integer
    Public ZeitknapperBisTag As Integer
    Public DayOfLastWalkOrJogging As Integer
    Public FruehsportModeArray() As FruehsportMode
    Public WinterHeizAbMMDD  As Integer
    Public WinterHeizBisMDD  As Integer
  End Structure

  Public Structure WorteForMorningP
    Public VersionOfStruct As Decimal
    Public TodoPart2() as String
    Public URL_ofDay() As String
  End Structure
End Module

Public Module GlobalConstants
    Public ReadOnly base_sunday As Date = New Date(2018, 9, 2) ' Muss ein Sonntag sein = Tag 0. Die Zshl der Tage ab dem 31.12.1899 ist durch 63 teilbar.
End Module