Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Net
Imports System.Security.Policy
Imports System.Diagnostics
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports System.Configuration
Imports System.Threading
Imports System.Net.Http
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.Win32
Imports System.IO.Compression
Imports System.Xml
Imports System.Reflection

Public Class Form1

    Public SC_Installationpath_LIVE As String 'C:\Program Files\Roberts Space Industries\StarCitizen\LIVE
    Public SC_Installationpath_PTU As String 'C:\Program Files\Roberts Space Industries\StarCitizen\PTU
    Public SC_Installationpath_EPTU As String 'C:\Program Files\Roberts Space Industries\StarCitizen\EPTU
    Public SC_Installationpath_TECH_PREVIEW As String 'C:\Program Files\Roberts Space Industries\StarCitizen\TECH-PREVIEW
    Public SC_Installationpath_HOTFIX As String 'C:\Program Files\Roberts Space Industries\StarCitizen\HOTFIX
    Public LC_Installationpath As String 'C:\Program Files\Roberts Space Industries\RSI Launcher
    Public Verbose As Boolean
    'Public ImagePath = "C:\Users\ggmra\Documents\SC Deutsch Launcher\DeSCync\Resources\"
    Public ImagePath = AppDomain.CurrentDomain.BaseDirectory & "\Resources\"
    Public updateOnly As Boolean = False


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Aufgaben
        '1. Suchen nach Installationen in gängigen Pfaden
        '2. Checken der Pfade zu Global.ini evtl. anlegen der Ordner
        '3. Setzen der Sprachen gemäß Speicher




        'Lade existierende xml
        LoadSettingsFromXml()

        Dim version As Version = Assembly.GetExecutingAssembly().GetName().Version
        Me.Text = "SC Deutsch Launcher (Version " & version.ToString & ")"


        ' Vorbereitungen - setzte Flags und Lade Bilder
        If My.Settings.Verbose = True Then
            Verbose = True
            Logtext.Text = "Logging:" & vbCrLf : Logtext.Refresh()
        Else
            Verbose = False
        End If

        'Update Routine
        Dim CurrentVersion As String = My.Application.Info.Version.ToString()
        Update_available.Image = Image.FromFile(ImagePath & "update.png")
        If CurrentVersionCheck(CurrentVersion) Then
            'Version Current Go on
            Update_available.Visible = False
        Else
            'Version Outdated
            Update_available.Visible = True
        End If

        If Verbose = True Then Logtext.Text = "Suche Installationen ..." & vbCrLf : Logtext.Refresh()

        'Lade Überschrift
        Label1.Image = Image.FromFile(ImagePath & "titel.png")
        'Lade Hintergrundbild
        Me.BackgroundImage = Image.FromFile(ImagePath & "Background.png")
        Me.BackgroundImageLayout = ImageLayout.Tile
        'Lade Icon oben links
        'Icon1.Image = Image.FromFile(ImagePath & "Logo2.png")
        'Icon1.BackgroundImageLayout = ImageLayout.Zoom
        'Lade Discord Icon
        Discord.Image = Image.FromFile(ImagePath & "Discord2.png")
        'Icon1.BackgroundImageLayout = ImageLayout.Zoom
        'Made by the community
        SCCommunity.Image = Image.FromFile(ImagePath & "Made_by_the_Community2.png")
        'Icon1.BackgroundImageLayout = ImageLayout.Zoom

        'Check commandline arguments
        For Each arg As String In My.Application.CommandLineArgs
            Console.WriteLine("Argument: " & arg)
        Next

        Dim args() As String = Environment.GetCommandLineArgs()
        If args.Length > 1 AndAlso args(1).ToLower() = "update" Then
            ' Set updateOnly to True if the argument is "update"
            updateOnly = True
            Console.WriteLine("Update mode enabled.")
        Else
            Console.WriteLine("Normal mode enabled.")
        End If

        'Suche nach dem Installationsordner 
        '---- LIVE ----------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If My.Settings.SC_Installationpath_LIVE <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.SC_Installationpath_LIVE & "\" & "StarCitizen_Launcher.exe") Then
                SC_Installationpath_LIVE = My.Settings.SC_Installationpath_LIVE
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.SC_Installationpath_LIVE = ""
                SC_Installationpath_LIVE = ""
            End If
        End If
        'Falls SC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.SC_Installationpath_LIVE = "" Then
            'Regulärer Pfad
            If File.Exists("C:\Program Files\Roberts Space Industries\StarCitizen\LIVE" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_LIVE = "C:\Program Files\Roberts Space Industries\StarCitizen\LIVE"
                SC_Installationpath_LIVE = My.Settings.SC_Installationpath_LIVE
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\StarCitizen\LIVE" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_LIVE = "C:\Program Files\Roberts Space Industries\StarCitizen\LIVE"
                SC_Installationpath_LIVE = My.Settings.SC_Installationpath_LIVE
            End If
        End If
        'Erweiterte Suche über alle Laufwerke
        If My.Settings.SC_Installationpath_LIVE = "" Then
            My.Settings.SC_Installationpath_LIVE = ReturnInstallationPath("LIVE")
            SC_Installationpath_LIVE = My.Settings.SC_Installationpath_LIVE
        End If
        If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: " & SC_Installationpath_LIVE & vbCrLf : Logtext.Refresh()
        '---- PTU --------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If My.Settings.SC_Installationpath_PTU <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.SC_Installationpath_PTU & "\" & "StarCitizen_Launcher.exe") Then
                SC_Installationpath_PTU = My.Settings.SC_Installationpath_PTU
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.SC_Installationpath_PTU = ""
                SC_Installationpath_PTU = ""
            End If
        End If
        'Falls SC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.SC_Installationpath_PTU = "" Then
            'Regulärer Pfad
            If File.Exists("C:\Program Files\Roberts Space Industries\StarCitizen\PTU" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_PTU = "C:\Program Files\Roberts Space Industries\StarCitizen\PTU"
                SC_Installationpath_PTU = My.Settings.SC_Installationpath_PTU
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\StarCitizen\PTU" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_PTU = "C:\Program Files\Roberts Space Industries\StarCitizen\PTU"
                SC_Installationpath_PTU = My.Settings.SC_Installationpath_PTU
            End If
        End If
        'Erweiterte Suche über alle Laufwerke
        If My.Settings.SC_Installationpath_PTU = "" Then
            My.Settings.SC_Installationpath_PTU = ReturnInstallationPath("PTU")
            SC_Installationpath_PTU = My.Settings.SC_Installationpath_PTU
        End If
        If Verbose = True Then Logtext.Text = Logtext.Text & "PTU: " & SC_Installationpath_PTU & vbCrLf : Logtext.Refresh()
        '---- EPTU ----------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If My.Settings.SC_Installationpath_EPTU <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.SC_Installationpath_EPTU & "\" & "StarCitizen_Launcher.exe") Then
                SC_Installationpath_EPTU = My.Settings.SC_Installationpath_EPTU
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.SC_Installationpath_EPTU = ""
                SC_Installationpath_EPTU = ""
            End If
        End If
        'Falls SC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.SC_Installationpath_EPTU = "" Then
            'Regulärer Pfad
            If File.Exists("C:\Program Files\Roberts Space Industries\StarCitizen\EPTU" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_EPTU = "C:\Program Files\Roberts Space Industries\StarCitizen\EPTU"
                SC_Installationpath_EPTU = My.Settings.SC_Installationpath_EPTU
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\StarCitizen\EPTU" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_EPTU = "C:\Program Files\Roberts Space Industries\StarCitizen\EPTU"
                SC_Installationpath_EPTU = My.Settings.SC_Installationpath_EPTU
            End If
        End If
        'Erweiterte Suche über alle Laufwerke
        If My.Settings.SC_Installationpath_EPTU = "" Then
            My.Settings.SC_Installationpath_EPTU = ReturnInstallationPath("EPTU")
            SC_Installationpath_EPTU = My.Settings.SC_Installationpath_EPTU
        End If
        If Verbose = True Then Logtext.Text = Logtext.Text & "EPTU: " & SC_Installationpath_EPTU & vbCrLf : Logtext.Refresh()
        '---- TECH_PREVIEW ----------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If My.Settings.SC_Installationpath_TECH_PREVIEW <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.SC_Installationpath_TECH_PREVIEW & "\" & "StarCitizen_Launcher.exe") Then
                SC_Installationpath_TECH_PREVIEW = My.Settings.SC_Installationpath_TECH_PREVIEW
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.SC_Installationpath_TECH_PREVIEW = ""
                SC_Installationpath_TECH_PREVIEW = ""
            End If
        End If
        'Falls SC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.SC_Installationpath_TECH_PREVIEW = "" Then
            'Regulärer Pfad
            If File.Exists("C:\Program Files\Roberts Space Industries\StarCitizen\TECH-PREVIEW" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_TECH_PREVIEW = "C:\Program Files\Roberts Space Industries\StarCitizen\TECH-PREVIEW"
                SC_Installationpath_TECH_PREVIEW = My.Settings.SC_Installationpath_TECH_PREVIEW
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\StarCitizen\TECH-PREVIEW" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_TECH_PREVIEW = "C:\Program Files\Roberts Space Industries\StarCitizen\TECH-PREVIEW"
                SC_Installationpath_TECH_PREVIEW = My.Settings.SC_Installationpath_TECH_PREVIEW
            End If
        End If
        'Erweiterte Suche über alle Laufwerke
        If My.Settings.SC_Installationpath_TECH_PREVIEW = "" Then
            My.Settings.SC_Installationpath_TECH_PREVIEW = ReturnInstallationPath("TECH-PREVIEW")
            SC_Installationpath_TECH_PREVIEW = My.Settings.SC_Installationpath_TECH_PREVIEW
        End If
        If Verbose = True Then Logtext.Text = Logtext.Text & "TECH-PREVIEW: " & SC_Installationpath_TECH_PREVIEW & vbCrLf : Logtext.Refresh()
        '---- HOTFIX ----------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If My.Settings.SC_Installationpath_HOTFIX <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.SC_Installationpath_HOTFIX & "\" & "StarCitizen_Launcher.exe") Then
                SC_Installationpath_HOTFIX = My.Settings.SC_Installationpath_HOTFIX
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.SC_Installationpath_HOTFIX = ""
                SC_Installationpath_HOTFIX = ""
            End If
        End If
        'Falls SC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.SC_Installationpath_HOTFIX = "" Then
            'Regulärer Pfad
            If File.Exists("C:\Program Files\Roberts Space Industries\StarCitizen\HOTFIX" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_HOTFIX = "C:\Program Files\Roberts Space Industries\StarCitizen\HOTFIX"
                SC_Installationpath_HOTFIX = My.Settings.SC_Installationpath_HOTFIX
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\StarCitizen\HOTFIX" & "\" & "StarCitizen_Launcher.exe") Then
                My.Settings.SC_Installationpath_HOTFIX = "C:\Program Files\Roberts Space Industries\StarCitizen\HOTFIX"
                SC_Installationpath_HOTFIX = My.Settings.SC_Installationpath_HOTFIX
            End If
        End If
        'Erweiterte Suche über alle Laufwerke
        If My.Settings.SC_Installationpath_HOTFIX = "" Then
            My.Settings.SC_Installationpath_HOTFIX = ReturnInstallationPath("HOTFIX")
            SC_Installationpath_HOTFIX = My.Settings.SC_Installationpath_HOTFIX
        End If
        If Verbose = True Then Logtext.Text = Logtext.Text & "HOTFIX: " & SC_Installationpath_TECH_PREVIEW & vbCrLf : Logtext.Refresh()

        '---- RSI-Launcher ----------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If My.Settings.LC_Installationpath <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.LC_Installationpath & "\" & "RSI Launcher.exe") Then
                LC_Installationpath = My.Settings.LC_Installationpath
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.LC_Installationpath = ""
                LC_Installationpath = ""
            End If
        End If
        'Falls SC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.LC_Installationpath = "" Then
            If File.Exists("C:\Program Files\Roberts Space Industries\RSI Launcher" & "\" & "RSI Launcher.exe") Then
                My.Settings.LC_Installationpath = "C:\Program Files\Roberts Space Industries\RSI Launcher"
                LC_Installationpath = My.Settings.LC_Installationpath
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\RSI Launcher" & "\" & "RSI Launcher.exe") Then
                My.Settings.LC_Installationpath = "C:\Program Files\Roberts Space Industries\RSI Launcher"
                LC_Installationpath = My.Settings.LC_Installationpath
            End If
        End If
        'Erweiterte Suche
        'ReturnLCInstallationPath
        If My.Settings.LC_Installationpath = "" Then
            My.Settings.LC_Installationpath = ReturnLCInstallationPath()
            LC_Installationpath = My.Settings.LC_Installationpath
        End If
        If Verbose = True Then Logtext.Text = Logtext.Text & "Launcher: " & LC_Installationpath & vbCrLf & vbCrLf : Logtext.Refresh()

        'Färbe die entsprechenden Kästchen grün für die Installationen die gefunden wurden
        If SC_Installationpath_LIVE <> "" Then LIVE.Image = Image.FromFile(ImagePath & "LIVE_actv.png") Else LIVE.Image = Image.FromFile(ImagePath & "LIVE_deactv.png") : LIVE.Refresh()
        If SC_Installationpath_PTU <> "" Then PTU.Image = Image.FromFile(ImagePath & "PTU_actv.png") Else PTU.Image = Image.FromFile(ImagePath & "PTU_deactv.png") : PTU.Refresh()
        If SC_Installationpath_EPTU <> "" Then EPTU.Image = Image.FromFile(ImagePath & "EPTU_actv.png") Else EPTU.Image = Image.FromFile(ImagePath & "EPTU_deactv.png") : EPTU.Refresh()
        If SC_Installationpath_TECH_PREVIEW <> "" Then Tech_Preview.Image = Image.FromFile(ImagePath & "TECH_PREVIEW_actv.png") Else Tech_Preview.Image = Image.FromFile(ImagePath & "TECH_PREVIEW_deactv.png") : Tech_Preview.Refresh()
        If SC_Installationpath_HOTFIX <> "" Then HOTFIX.Image = Image.FromFile(ImagePath & "HOTFIX_actv.png") Else HOTFIX.Image = Image.FromFile(ImagePath & "HOTFIX_deactv.png") : HOTFIX.Refresh()
        If LC_Installationpath <> "" Then Label11.Image = Image.FromFile(ImagePath & "Update_und_Launch.png") Else Label11.Image = Image.FromFile(ImagePath & "Update_und_Launch.png") : Label11.Refresh()
        'Wenn nur Update dann ändern des Buttons für Update + Start
        If updateOnly = True And LC_Installationpath <> "" Then Label11.Image = Image.FromFile(ImagePath & "Autostart.png") : Label11.Refresh()

        'Abgelich mit evtl. vorhandener Installation in der die Spracheinstellung evtl. unterschiedlich ist
        If Verbose = True Then Logtext.Text = Logtext.Text & "Suche Spracheinstellungen ...." & vbCrLf : Logtext.Refresh()

        My.Settings.lang_LIVE = "eng"
        If File.Exists(SC_Installationpath_LIVE & "\user.cfg") And SC_Installationpath_LIVE <> "" Then
            'Einlesen der user.cfg
            Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_LIVE & "\user.cfg").ToList()
            ' Suchen nach Einträgen bezüglich g_language
            For i As Integer = 0 To lines.Count - 1
                If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                    My.Settings.lang_LIVE = "de"
                End If
            Next
        End If
        If SC_Installationpath_LIVE = "" Then My.Settings.lang_LIVE = ""
        If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: " & My.Settings.lang_LIVE
        If My.Settings.de_full_flag = True Then
            Logtext.Text = Logtext.Text & " (voll)" & vbCrLf : Logtext.Refresh()
        Else
            Logtext.Text = Logtext.Text & vbCrLf : Logtext.Refresh()
        End If

        My.Settings.lang_PTU = "eng"
        If File.Exists(SC_Installationpath_PTU & "\user.cfg") And SC_Installationpath_PTU <> "" Then
            'Einlesen der user.cfg
            Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_PTU & "\user.cfg").ToList()
            ' Suchen nach Einträgen bezüglich g_language
            For i As Integer = 0 To lines.Count - 1
                If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                    My.Settings.lang_PTU = "de"
                End If
            Next
        End If
        If SC_Installationpath_PTU = "" Then My.Settings.lang_PTU = ""
        If Verbose = True Then Logtext.Text = Logtext.Text & "PTU: " & My.Settings.lang_PTU & vbCrLf : Logtext.Refresh()

        My.Settings.lang_EPTU = "eng"
        If File.Exists(SC_Installationpath_EPTU & "\user.cfg") And SC_Installationpath_EPTU <> "" Then
            'Einlesen der user.cfg
            Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_EPTU & "\user.cfg").ToList()
            ' Suchen nach Einträgen bezüglich g_language
            For i As Integer = 0 To lines.Count - 1
                If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                    My.Settings.lang_EPTU = "de"
                End If
            Next
        End If
        If SC_Installationpath_EPTU = "" Then My.Settings.lang_EPTU = ""
        If Verbose = True Then Logtext.Text = Logtext.Text & "EPTU: " & My.Settings.lang_EPTU & vbCrLf : Logtext.Refresh()

        My.Settings.lang_TECH_PREVIEW = "eng"
        If File.Exists(SC_Installationpath_TECH_PREVIEW & "\user.cfg") And SC_Installationpath_TECH_PREVIEW <> "" Then
            'Einlesen der user.cfg
            Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_TECH_PREVIEW & "\user.cfg").ToList()
            ' Suchen nach Einträgen bezüglich g_language
            For i As Integer = 0 To lines.Count - 1
                If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                    My.Settings.lang_TECH_PREVIEW = "de"
                End If
            Next
        End If
        If SC_Installationpath_TECH_PREVIEW = "" Then My.Settings.lang_TECH_PREVIEW = ""
        If Verbose = True Then Logtext.Text = Logtext.Text & "TECH-PREVIEW: " & My.Settings.lang_TECH_PREVIEW & vbCrLf : Logtext.Refresh()


        My.Settings.lang_HOTFIX = "eng"
        If File.Exists(SC_Installationpath_HOTFIX & "\user.cfg") And SC_Installationpath_HOTFIX <> "" Then
            'Einlesen der user.cfg
            Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_HOTFIX & "\user.cfg").ToList()
            ' Suchen nach Einträgen bezüglich g_language
            For i As Integer = 0 To lines.Count - 1
                If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                    My.Settings.lang_HOTFIX = "de"
                End If
            Next
        End If
        If SC_Installationpath_HOTFIX = "" Then My.Settings.lang_HOTFIX = ""
        If Verbose = True Then Logtext.Text = Logtext.Text & "HOTFIX: " & My.Settings.lang_HOTFIX & vbCrLf : Logtext.Refresh()



        'Setzte die Sprache entsprechend der Daten im Resourcenspeicher - falls kein Eintrag auf english setzen
        '------------------------LIVE--------------------------------------------------------------
        If My.Settings.lang_LIVE = "eng" Or My.Settings.lang_LIVE = "" Then
            My.Settings.lang_LIVE = "eng"
            LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
            LIVE_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
            LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_deact.png")
        Else
            My.Settings.lang_LIVE = "de"
            LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png")
            If My.Settings.de_full_flag = False Then
                LIVE_de.Image = Image.FromFile(ImagePath & "DE_actv.png")
                LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_deact.png")
            Else
                LIVE_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_act.png")
            End If
        End If
        If SC_Installationpath_LIVE = "" Then LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : LIVE_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_deact.png") 'Falls keine Installation dann keine Auswahl
        '------------------------PTU--------------------------------------------------------------
        If My.Settings.lang_PTU = "eng" Or My.Settings.lang_PTU = "" Then
            My.Settings.lang_PTU = "eng"
            PTU_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
            PTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
        Else
            My.Settings.lang_PTU = "de"
            PTU_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png")
            PTU_de.Image = Image.FromFile(ImagePath & "DE_actv.png")
        End If
        If SC_Installationpath_PTU = "" Then PTU_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : PTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") 'Falls keine Installation dann keine Auswahl
        '------------------------EPTU--------------------------------------------------------------
        If My.Settings.lang_EPTU = "eng" Or My.Settings.lang_EPTU = "" Then
            My.Settings.lang_EPTU = "eng"
            EPTU_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
            EPTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
        Else
            My.Settings.lang_EPTU = "de"
            EPTU_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png")
            EPTU_de.Image = Image.FromFile(ImagePath & "DE_actv.png")
        End If
        If SC_Installationpath_EPTU = "" Then EPTU_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : EPTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") 'Falls keine Installation dann keine Auswahl
        '------------------------TECH_PREVIEW--------------------------------------------------------------
        If My.Settings.lang_TECH_PREVIEW = "eng" Or My.Settings.lang_TECH_PREVIEW = "" Then
            My.Settings.lang_TECH_PREVIEW = "eng"
            TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
            TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
            TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : TPLIVEini.Visible = False : Trenner0.Visible = False
            TPPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : TPPTUini.Visible = False : Trenner0.Visible = False
        Else
            My.Settings.lang_TECH_PREVIEW = "de"
            TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png")
            TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_actv.png")
            If My.Settings.TECH_PREVIEW_ini = "LIVE" Then
                TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_actv.png") : TPLIVEini.Visible = True : Trenner1.Visible = True
                TPPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : TPPTUini.Visible = True
            Else
                TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : TPLIVEini.Visible = True
                TPPTUini.Image = Image.FromFile(ImagePath & "xptu_actv.png") : TPPTUini.Visible = True
            End If
        End If
        If SC_Installationpath_TECH_PREVIEW = "" Then TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : TPLIVEini.Visible = False : TPPTUini.Visible = False : Trenner0.Visible = False  'Falls keine Installation dann keine Auswahl
        '------------------------HOTFIX--------------------------------------------------------------
        If My.Settings.lang_HOTFIX = "eng" Or My.Settings.lang_HOTFIX = "" Then
            My.Settings.lang_HOTFIX = "eng"
            HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
            HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
            HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : HOTFIXPTUini.Visible = False : Trenner1.Visible = False
            HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : HOTFIXLIVEini.Visible = False : Trenner1.Visible = False
        Else
            My.Settings.lang_HOTFIX = "de"
            HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png")
            HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_actv.png")
            If My.Settings.HOTFIX_ini = "LIVE" Then
                HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : HOTFIXPTUini.Visible = True : Trenner1.Visible = True
                HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_actv.png") : HOTFIXLIVEini.Visible = True : Trenner1.Visible = True
            Else
                HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_actv.png") : HOTFIXPTUini.Visible = True : Trenner1.Visible = True
                HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : HOTFIXLIVEini.Visible = True : Trenner1.Visible = True
            End If
        End If
        If SC_Installationpath_HOTFIX = "" Then HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : HOTFIXPTUini.Visible = False : HOTFIXLIVEini.Visible = False : Trenner1.Visible = False 'Falls keine Installation dann keine Auswahl

        My.Settings.Save()

        'wenn updateOnly true dann weiter - aber nur wenn wenigstens ein Installationspfad gefunden wurde und der Launcher pfad nicht leer ist
        If (SC_Installationpath_LIVE = "" And SC_Installationpath_PTU = "" And SC_Installationpath_EPTU = "" And SC_Installationpath_TECH_PREVIEW = "" And SC_Installationpath_HOTFIX = "") Or (LC_Installationpath = "") Then
            updateOnly = False
            Label11.Image = Image.FromFile(ImagePath & "Update_und_Launch.png")
            Label11.Refresh()
        Else
            If updateOnly = True Then
                Update()
            End If
        End If
    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    'Erweiterte automatische Suche
    '-----------------------------------------------------------------------------------------------------------------------------------------------------

    Function Outdated_ReturnInstallationPath(Installation As String) 'Installation is LIVE, EPTU, PTU, TECH-PREVIEW, HOTFIX
        'If Verbose = True Then Logtext.Text = Logtext.Text & "Suche Installationen in Laufwerken ..." & vbCrLf : Logtext.Refresh()
        Dim folderPath As String = "Program Files\Roberts Space Industries\StarCitizen\" & Installation
        Dim foundPath As String = ""
        ' Get all the available drives
        For Each drive As DriveInfo In DriveInfo.GetDrives()
            ' Check if the drive is ready
            If drive.IsReady Then
                ' Combine the drive root with the folder path
                Dim fullPath As String = Path.Combine(drive.RootDirectory.FullName, folderPath)

                ' Check if the directory exists
                If Directory.Exists(fullPath) Then
                    foundPath = fullPath
                End If
            End If
        Next
        If foundPath = "" Then
            folderPath = "Programme\Roberts Space Industries\StarCitizen\" & Installation
            For Each drive As DriveInfo In DriveInfo.GetDrives()
                ' Check if the drive is ready
                If drive.IsReady Then
                    ' Combine the drive root with the folder path
                    Dim fullPath As String = Path.Combine(drive.RootDirectory.FullName, folderPath)

                    ' Check if the directory exists
                    If Directory.Exists(fullPath) Then
                        foundPath = fullPath
                    End If
                End If
            Next
        End If
        If foundPath = "" Then
            folderPath = "Roberts Space Industries\StarCitizen\" & Installation
            For Each drive As DriveInfo In DriveInfo.GetDrives()
                ' Check if the drive is ready
                If drive.IsReady Then
                    ' Combine the drive root with the folder path
                    Dim fullPath As String = Path.Combine(drive.RootDirectory.FullName, folderPath)

                    ' Check if the directory exists
                    If Directory.Exists(fullPath) Then
                        foundPath = fullPath
                    End If
                End If
            Next
        End If
        If foundPath = "" Then
            folderPath = "StarCitizen\" & Installation
            For Each drive As DriveInfo In DriveInfo.GetDrives()
                ' Check if the drive is ready
                If drive.IsReady Then
                    ' Combine the drive root with the folder path
                    Dim fullPath As String = Path.Combine(drive.RootDirectory.FullName, folderPath)

                    ' Check if the directory exists
                    If Directory.Exists(fullPath) Then
                        foundPath = fullPath
                    End If
                End If
            Next
        End If
        Return foundPath
    End Function

    'Erweiterte Schleife für mehr Verzeichnisse

    Function ReturnInstallationPath(Installation As String) 'Installation is LIVE, EPTU, PTU, TECH-PREVIEW, HOTFIX
        Dim fullpath As String
        Dim folderpaths As New List(Of String)
        folderpaths.Add("Program Files\Roberts Space Industries\StarCitizen\" & Installation)
        folderpaths.Add("Roberts Space Industries\StarCitizen\" & Installation)
        folderpaths.Add("StarCitizen\" & Installation)
        folderpaths.Add("Spiele\Program Files\Roberts Space Industries\StarCitizen\" & Installation)
        folderpaths.Add("Spiele\Roberts Space Industries\StarCitizen\" & Installation)
        folderpaths.Add("Spiele\StarCitizen\" & Installation)
        folderpaths.Add("Games\Program Files\Roberts Space Industries\StarCitizen\" & Installation)
        folderpaths.Add("Games\Roberts Space Industries\StarCitizen\" & Installation)
        folderpaths.Add("Games\StarCitizen\" & Installation)

        Dim foundPath As String = ""

        ' Get all the available drives
        For Each drive As DriveInfo In DriveInfo.GetDrives()
            ' Check if the drive is ready
            If drive.IsReady Then
                ' Combine the drive root with the folder path

                For Each folderpath As String In folderpaths
                    fullpath = Path.Combine(drive.RootDirectory.FullName, folderpath)
                    ' Check if the directory exists
                    If Directory.Exists(fullpath) Then
                        foundPath = fullpath
                        Return foundPath
                    End If
                Next
            End If
        Next
        Return foundPath
    End Function

    'Searching for Launcher of Star citizen
    Function ReturnLCInstallationPath() 'Installation is LIVE, EPTU, PTU, TECH-PREVIEW, HOTFIX
        Dim fullpath As String
        Dim folderpaths As New List(Of String)
        folderpaths.Add("Program Files\Roberts Space Industries\RSI Launcher")
        folderpaths.Add("Roberts Space Industries\RSI Launcher")
        folderpaths.Add("RSI Launcher")
        folderpaths.Add("Spiele\Program Files\Roberts Space Industries\RSI Launcher")
        folderpaths.Add("Spiele\Roberts Space Industries\RSI Launcher")
        folderpaths.Add("Spiele\RSI Launcher")
        folderpaths.Add("Games\Program Files\Roberts Space Industries\RSI Launcher")
        folderpaths.Add("Games\Roberts Space Industries\RSI Launcher")
        folderpaths.Add("Games\RSI Launcher")

        Dim foundPath As String = ""

        ' Get all the available drives
        For Each drive As DriveInfo In DriveInfo.GetDrives()
            ' Check if the drive is ready
            If drive.IsReady Then
                ' Combine the drive root with the folder path

                For Each folderpath As String In folderpaths
                    fullpath = Path.Combine(drive.RootDirectory.FullName, folderpath)
                    ' Check if the directory exists
                    If Directory.Exists(fullpath) Then
                        foundPath = fullpath
                        Return foundPath
                    End If
                Next
            End If
        Next
        Return foundPath
    End Function






    '------------------------------------------------------------------------------------------------------------------------
    'Manuelle Suche des Installations-Verzeichnisses ------------------------------------------------------------------------
    Private Sub LIVE_Click(sender As Object, e As EventArgs) Handles LIVE.Click
        ' Create a new instance of FolderBrowserDialog
        Using folderBrowserDialog As New FolderBrowserDialog()
            ' Set the description (optional)
            folderBrowserDialog.Description = "Suchen und wählen Sie den Ordner 'LIVE' z.B. unter ..\Roberts Space Industries\StarCitizen aus"
            ' Show the dialog and check if the user clicked OK
            If folderBrowserDialog.ShowDialog() = DialogResult.OK Then
                'Fehlerroutine
                If File.Exists(folderBrowserDialog.SelectedPath & "\" & "StarCitizen_Launcher.exe") Then
                    ' Set the selected path to the TextBox
                    'TextBox1.Text = folderBrowserDialog.SelectedPath
                    SC_Installationpath_LIVE = folderBrowserDialog.SelectedPath
                    My.Settings.SC_Installationpath_LIVE = folderBrowserDialog.SelectedPath
                    LIVE.Image = Image.FromFile(ImagePath & "LIVE_actv.png")
                    LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
                    LIVE_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                    LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                Else
                    MsgBox("StarCitizen_Launcher.exe fehlt", vbOKOnly, "Fehler")
                    LIVE.Image = Image.FromFile(ImagePath & "LIVE_deactv.png")
                    SC_Installationpath_LIVE = ""
                    My.Settings.SC_Installationpath_LIVE = ""
                End If
            End If
            LIVE.Refresh()
            My.Settings.Save()
        End Using
    End Sub

    Private Sub PTU_Click(sender As Object, e As EventArgs) Handles PTU.Click
        ' Create a new instance of FolderBrowserDialog
        Using folderBrowserDialog As New FolderBrowserDialog()
            ' Set the description (optional)
            folderBrowserDialog.Description = "Suchen und wählen Sie den Ordner 'PTU' z.B. unter ..\Roberts Space Industries\StarCitizen aus"
            ' Show the dialog and check if the user clicked OK
            If folderBrowserDialog.ShowDialog() = DialogResult.OK Then
                'Fehlerroutine
                If File.Exists(folderBrowserDialog.SelectedPath & "\" & "StarCitizen_Launcher.exe") Then
                    ' Set the selected path to the TextBox
                    'TextBox1.Text = folderBrowserDialog.SelectedPath
                    SC_Installationpath_PTU = folderBrowserDialog.SelectedPath
                    My.Settings.SC_Installationpath_PTU = folderBrowserDialog.SelectedPath
                    PTU.Image = Image.FromFile(ImagePath & "PTU_actv.png")
                    PTU_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
                    PTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                Else
                    MsgBox("StarCitizen_Launcher.exe fehlt", vbOKOnly, "Fehler")
                    PTU.Image = Image.FromFile(ImagePath & "PTU_deactv.png")
                    SC_Installationpath_PTU = ""
                    My.Settings.SC_Installationpath_PTU = ""
                End If
            End If
            PTU.Refresh()
            My.Settings.Save()
        End Using
    End Sub

    Private Sub EPTU_Click(sender As Object, e As EventArgs) Handles EPTU.Click
        ' Create a new instance of FolderBrowserDialog
        Using folderBrowserDialog As New FolderBrowserDialog()
            ' Set the description (optional)
            folderBrowserDialog.Description = "Suchen und wählen Sie den Ordner 'EPTU' z.B. unter ..\Roberts Space Industries\StarCitizen aus"
            ' Show the dialog and check if the user clicked OK
            If folderBrowserDialog.ShowDialog() = DialogResult.OK Then
                'Fehlerroutine
                If File.Exists(folderBrowserDialog.SelectedPath & "\" & "StarCitizen_Launcher.exe") Then
                    ' Set the selected path to the TextBox
                    'TextBox1.Text = folderBrowserDialog.SelectedPath
                    SC_Installationpath_EPTU = folderBrowserDialog.SelectedPath
                    My.Settings.SC_Installationpath_EPTU = folderBrowserDialog.SelectedPath
                    EPTU.Image = Image.FromFile(ImagePath & "EPTU_actv.png")
                    EPTU_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
                    EPTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                Else
                    MsgBox("StarCitizen_Launcher.exe fehlt", vbOKOnly, "Fehler")
                    EPTU.Image = Image.FromFile(ImagePath & "EPTU_deactv.png")
                    SC_Installationpath_EPTU = ""
                    My.Settings.SC_Installationpath_EPTU = ""
                End If
            End If
            EPTU.Refresh()
            My.Settings.Save()
        End Using
    End Sub

    Private Sub TECH_PREVIEW_Click(sender As Object, e As EventArgs) Handles Tech_Preview.Click
        ' Create a new instance of FolderBrowserDialog
        Using folderBrowserDialog As New FolderBrowserDialog
            ' Set the description (optional)
            folderBrowserDialog.Description = "Suchen und wählen Sie den Ordner 'TECH-PREVIEW' z.B. unter ..\Roberts Space Industries\StarCitizen aus"
            ' Show the dialog and check if the user clicked OK
            If folderBrowserDialog.ShowDialog = DialogResult.OK Then
                'Fehlerroutine
                If File.Exists(folderBrowserDialog.SelectedPath & "\" & "StarCitizen_Launcher.exe") Then
                    ' Set the selected path to the TextBox
                    'TextBox1.Text = folderBrowserDialog.SelectedPath
                    SC_Installationpath_TECH_PREVIEW = folderBrowserDialog.SelectedPath
                    My.Settings.SC_Installationpath_TECH_PREVIEW = folderBrowserDialog.SelectedPath
                    Tech_Preview.Image = Image.FromFile(ImagePath & "TECH_PREVIEW_actv.png")
                    TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
                    TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                Else
                    MsgBox("StarCitizen_Launcher.exe fehlt", vbOKOnly, "Fehler")
                    Tech_Preview.Image = Image.FromFile(ImagePath & "TECH_PREVIEW_deactv.png")
                    SC_Installationpath_TECH_PREVIEW = ""
                    My.Settings.SC_Installationpath_TECH_PREVIEW = ""
                End If
            End If
            Tech_Preview.Refresh()
            My.Settings.Save()
        End Using
    End Sub

    Private Sub HOTFIX_Click(sender As Object, e As EventArgs) Handles HOTFIX.Click
        ' Create a new instance of FolderBrowserDialog
        Using folderBrowserDialog As New FolderBrowserDialog
            ' Set the description (optional)
            folderBrowserDialog.Description = "Suchen und wählen Sie den Ordner 'HOTFIX' z.B. unter ..\Roberts Space Industries\StarCitizen aus"
            ' Show the dialog and check if the user clicked OK
            If folderBrowserDialog.ShowDialog = DialogResult.OK Then
                'Fehlerroutine
                If File.Exists(folderBrowserDialog.SelectedPath & "\" & "StarCitizen_Launcher.exe") Then
                    ' Set the selected path to the TextBox
                    'TextBox1.Text = folderBrowserDialog.SelectedPath
                    SC_Installationpath_HOTFIX = folderBrowserDialog.SelectedPath
                    My.Settings.SC_Installationpath_HOTFIX = folderBrowserDialog.SelectedPath
                    HOTFIX.Image = Image.FromFile(ImagePath & "HOTFIX_actv.png")
                    HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png")
                    HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_deactv.png")
                Else
                    MsgBox("StarCitizen_Launcher.exe fehlt", vbOKOnly, "Fehler")
                    HOTFIX.Image = Image.FromFile(ImagePath & "HOTFIX_deactv.png")
                    SC_Installationpath_HOTFIX = ""
                    My.Settings.SC_Installationpath_HOTFIX = ""
                End If
            End If
            HOTFIX.Refresh()
            My.Settings.Save()
        End Using
    End Sub


    '________________________________________________________________________________________-
    '-------- Ändern der Sprache ------------------------------------------------------------
    Private Sub LIVE_eng_Click(sender As Object, e As EventArgs) Handles LIVE_eng.Click
        If SC_Installationpath_LIVE <> "" Then
            My.Settings.lang_LIVE = "eng"
            LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png") : LIVE_eng.Refresh()
            LIVE_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : LIVE_de.Refresh()
            LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_deact.png") : LIVE_de.Refresh()
            My.Settings.de_full_flag = False
            My.Settings.Save()
        End If
    End Sub

    Private Sub PTU_eng_Click(sender As Object, e As EventArgs) Handles PTU_eng.Click
        If SC_Installationpath_PTU <> "" Then
            My.Settings.lang_PTU = "eng"
            PTU_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png") : PTU_eng.Refresh()
            PTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : PTU_de.Refresh()
            My.Settings.Save()
        End If
    End Sub

    Private Sub EPTU_eng_Click(sender As Object, e As EventArgs) Handles EPTU_eng.Click
        If SC_Installationpath_EPTU <> "" Then
            My.Settings.lang_EPTU = "eng"
            EPTU_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png") : EPTU_eng.Refresh()
            EPTU_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : EPTU_de.Refresh()
            My.Settings.Save()
        End If
    End Sub

    Private Sub TECH_PREVIEW_eng_Click(sender As Object, e As EventArgs) Handles TECH_PREVIEW_eng.Click
        If SC_Installationpath_TECH_PREVIEW <> "" Then
            My.Settings.lang_TECH_PREVIEW = "eng"
            TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png") : TECH_PREVIEW_eng.Refresh()
            TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : TECH_PREVIEW_de.Refresh()
            TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : TPLIVEini.Visible = False : Trenner0.Visible = False : TPLIVEini.Refresh()
            TPPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : TPPTUini.Visible = False : Trenner0.Visible = False : TPPTUini.Refresh()
            My.Settings.Save()
        End If
    End Sub

    Private Sub HOTFIX_eng_Click(sender As Object, e As EventArgs) Handles HOTFIX_eng.Click
        If SC_Installationpath_HOTFIX <> "" Then
            My.Settings.lang_HOTFIX = "eng"
            HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_actv.png") : HOTFIX_eng.Refresh()
            HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : HOTFIX_de.Refresh()
            HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : HOTFIXPTUini.Visible = False : Trenner1.Visible = False : HOTFIXPTUini.Refresh()
            HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : HOTFIXLIVEini.Visible = False : Trenner1.Visible = False : HOTFIXLIVEini.Refresh()
            My.Settings.Save()
        End If
    End Sub


    Private Sub LIVE_de_Click(sender As Object, e As EventArgs) Handles LIVE_de.Click
        If SC_Installationpath_LIVE <> "" Then
            My.Settings.lang_LIVE = "de"
            LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : LIVE_eng.Refresh()
            LIVE_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : LIVE_de.Refresh()
            LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_deact.png") : LIVE_de.Refresh()
            My.Settings.de_full_flag = False
            My.Settings.Save()
        End If
    End Sub

    Private Sub LIVE_de_voll_Click(sender As Object, e As EventArgs) Handles LIVE_de_voll.Click
        If SC_Installationpath_LIVE <> "" Then
            My.Settings.lang_LIVE = "de"
            LIVE_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : LIVE_eng.Refresh()
            LIVE_de.Image = Image.FromFile(ImagePath & "DE_deactv.png") : LIVE_de.Refresh()
            LIVE_de_voll.Image = Image.FromFile(ImagePath & "DE_voll_act.png") : LIVE_de.Refresh()
            My.Settings.de_full_flag = True
            My.Settings.Save()
        End If
    End Sub

    Private Sub PTU_de_Click(sender As Object, e As EventArgs) Handles PTU_de.Click
        If SC_Installationpath_PTU <> "" Then
            My.Settings.lang_PTU = "de"
            PTU_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : PTU_eng.Refresh()
            PTU_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : PTU_de.Refresh()
            My.Settings.Save()
        End If
    End Sub

    Private Sub EPTU_de_Click(sender As Object, e As EventArgs) Handles EPTU_de.Click
        If SC_Installationpath_EPTU <> "" Then
            My.Settings.lang_EPTU = "de"
            EPTU_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : EPTU_eng.Refresh()
            EPTU_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : EPTU_de.Refresh()
            My.Settings.Save()
        End If
    End Sub

    Private Sub TECH_PREVIEW_de_Click(sender As Object, e As EventArgs) Handles TECH_PREVIEW_de.Click
        If SC_Installationpath_TECH_PREVIEW <> "" Then
            My.Settings.lang_TECH_PREVIEW = "de"
            TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : TECH_PREVIEW_eng.Refresh()
            TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : TECH_PREVIEW_de.Refresh()
            If My.Settings.TECH_PREVIEW_ini = "LIVE" Then
                TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_actv.png") : TPLIVEini.Visible = True : Trenner0.Visible = True : TPLIVEini.Refresh()
                TPPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : TPPTUini.Visible = True : Trenner0.Visible = True : TPPTUini.Refresh()
            Else
                TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : TPLIVEini.Visible = True : Trenner0.Visible = True : TPLIVEini.Refresh()
                TPPTUini.Image = Image.FromFile(ImagePath & "xptu_actv.png") : TPPTUini.Visible = True : Trenner0.Visible = True : TPPTUini.Refresh()
            End If
            My.Settings.Save()
        End If
    End Sub

    Private Sub TPLIVEini_Click(sender As Object, e As EventArgs) Handles TPLIVEini.Click
        If SC_Installationpath_TECH_PREVIEW <> "" Then
            My.Settings.lang_TECH_PREVIEW = "de"
            TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : TECH_PREVIEW_eng.Refresh()
            TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : TECH_PREVIEW_de.Refresh()
            TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_actv.png") : TPLIVEini.Refresh()
            TPPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : TPPTUini.Refresh()
            My.Settings.TECH_PREVIEW_ini = "LIVE"
            My.Settings.Save()
        End If
    End Sub

    Private Sub TPPTUini_Click(sender As Object, e As EventArgs) Handles TPPTUini.Click
        If SC_Installationpath_TECH_PREVIEW <> "" Then
            My.Settings.lang_TECH_PREVIEW = "de"
            TECH_PREVIEW_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : TECH_PREVIEW_eng.Refresh()
            TECH_PREVIEW_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : TECH_PREVIEW_de.Refresh()
            TPLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : TPLIVEini.Refresh()
            TPPTUini.Image = Image.FromFile(ImagePath & "xptu_actv.png") : TPPTUini.Refresh()
            My.Settings.TECH_PREVIEW_ini = "PTU"
            My.Settings.Save()
        End If
    End Sub


    Private Sub HOTFIX_de_Click(sender As Object, e As EventArgs) Handles HOTFIX_de.Click
        If SC_Installationpath_HOTFIX <> "" Then
            My.Settings.lang_HOTFIX = "de"
            HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : HOTFIX_eng.Refresh()
            HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : HOTFIX_de.Refresh()
            If My.Settings.HOTFIX_ini = "LIVE" Then
                HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : HOTFIXPTUini.Visible = True : Trenner1.Visible = True : HOTFIXPTUini.Refresh()
                HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_actv.png") : HOTFIXLIVEini.Visible = True : Trenner1.Visible = True : HOTFIXLIVEini.Refresh()
            Else
                HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_actv.png") : HOTFIXPTUini.Visible = True : Trenner1.Visible = True : HOTFIXPTUini.Refresh()
                HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : HOTFIXLIVEini.Visible = True : Trenner1.Visible = True : HOTFIXLIVEini.Refresh()
            End If
            My.Settings.Save()
        End If
    End Sub

    Private Sub HOTFIXLIVEini_Click(sender As Object, e As EventArgs) Handles HOTFIXLIVEini.Click
        If SC_Installationpath_HOTFIX <> "" Then
            My.Settings.lang_HOTFIX = "de"
            HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : HOTFIX_eng.Refresh()
            HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : HOTFIX_de.Refresh()
            HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_actv.png") : HOTFIXLIVEini.Refresh()
            HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_deactv.png") : HOTFIXPTUini.Refresh()
            My.Settings.HOTFIX_ini = "LIVE"
            My.Settings.Save()
        End If
    End Sub

    Private Sub HOTFIXPTUini_Click(sender As Object, e As EventArgs) Handles HOTFIXPTUini.Click
        If SC_Installationpath_HOTFIX <> "" Then
            My.Settings.lang_HOTFIX = "de"
            HOTFIX_eng.Image = Image.FromFile(ImagePath & "ENG_deactv.png") : HOTFIX_eng.Refresh()
            HOTFIX_de.Image = Image.FromFile(ImagePath & "DE_actv.png") : HOTFIX_de.Refresh()
            HOTFIXLIVEini.Image = Image.FromFile(ImagePath & "xlive_deactv.png") : HOTFIXLIVEini.Refresh()
            HOTFIXPTUini.Image = Image.FromFile(ImagePath & "xptu_actv.png") : HOTFIXPTUini.Refresh()
            My.Settings.HOTFIX_ini = "PTU"
            My.Settings.Save()
        End If
    End Sub






    Private Sub Label11_Click(sender As Object, e As EventArgs) Handles Label11.Click
        Update()
    End Sub





    Private Async Sub Update()
        'Update & Launch Proceedure
        '-- LIVE ----------------------------------
        '1.) Schreibe user.cfg um auf eng oder de oder schreibe user.cfg neu
        '2.) Check Installation folder falls deutsch
        '3.) Lade neue global.ini herunter
        '4.) wiederhole für alle installation die vorhanden sind
        '5.) Checke of RSI Launcher vorhanden ist - falls ja starten, falls nein - Directorybox und suchen lassen

        '------------------------------------------------------------------------------------------------------------    
        '1.) Check Installation folder falls deutsch
        '1a.) Check user.cfg exists
        Dim g_language_found As Boolean = False
        Dim g_languageAudio_found As Boolean = False

        'Ändern des Aussehens für den Button Update & Start nach click
        Label11.Image = Image.FromFile(ImagePath & "Autostart.png")
        Label11.Refresh()

        Logtext.Text = "" : Logtext.Refresh()
        If Verbose = True Then Logtext.Text = "Start Update - LIVE Environment ... " & vbCrLf : Logtext.Refresh()
        Try
            If File.Exists(SC_Installationpath_LIVE & "\user.cfg") And SC_Installationpath_LIVE <> "" Then
                'Einlesen der user.cfg
                Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_LIVE & "\user.cfg").ToList()

                ' Suchen ob der Eintrag g_language überhaupt vorhanden ist, falls nein die Einträge vornehmen und im nächsten Schritt die Sprache korrigieren
                For i As Integer = 0 To lines.Count - 1
                    If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                        g_language_found = True
                    End If
                    If lines(i).Trim().Equals("g_languageAudio = english", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("g_languageAudio=english", StringComparison.OrdinalIgnoreCase) Then
                        g_languageAudio_found = True
                    End If
                Next
                'Hinzufügen der Zeilen, falls nicht vorhanden
                If g_language_found = False Then lines.Add("g_language = german_(germany)")
                If g_languageAudio_found = False Then lines.Add("g_languageAudio = english")

                ' Suchen nach Einträgen bezüglich g_language
                For i As Integer = 0 To lines.Count - 1
                    'If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                    If lines(i).Contains("g_language=") Or lines(i).Contains("g_language =") Then
                        If My.Settings.lang_LIVE = "eng" Then
                            lines(i) = "#g_language = german_(germany)"
                            If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: eng -> #g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                        Else
                            lines(i) = "g_language = german_(germany)"
                            If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: de -> g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                        End If
                    End If
                Next
                ' Write all lines back to the file
                File.WriteAllLines(SC_Installationpath_LIVE & "\user.cfg", lines)
            Else
                'Create new user.cfg file
                If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: Creating new user.cfg and folder-tree" & vbCrLf : Logtext.Refresh()
                Using writer As New StreamWriter(SC_Installationpath_LIVE & "\user.cfg", False)
                    writer.WriteLine("g_languageAudio = english")
                    If My.Settings.lang_LIVE = "eng" Then
                        writer.WriteLine("#g_language = german_(germany)")
                        'writer.WriteLine("g_language = english")
                    Else
                        writer.WriteLine("g_language = german_(germany)")
                        'writer.WriteLine("#g_language = english")
                    End If
                End Using
            End If

            '2.) Untersuche ob der Foldertree vorhanden ist und falls nein erstelle ihn neu
            If Directory.Exists(SC_Installationpath_LIVE & "\data\Localization\german_(germany)") Then
                'Pfad existiert und global.ini kann geladen werden
            Else
                'Pfad existiert nicht und muß angelegt werden
                Directory.CreateDirectory(SC_Installationpath_LIVE & "\data\Localization\german_(germany)")
            End If

            'Error routines
        Catch ex As HttpRequestException
            ' Handle HTTP request errors (e.g., network errors, invalid URL)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                Logtext.Refresh()
            End If
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                Logtext.Refresh()
            End If
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                Logtext.Refresh()
            End If
        End Try


        '----------------------------------------------------------------------------------------------------
        '3.) Global.INI für LIVE laden

        ' new HTTP.Client -----------------------------------------------------------------------------------
        Try
            Using client As New HttpClient()
                Dim url As String
                Dim target As String = "LIVE"
                If My.Settings.de_full_flag = True Then
                    'full flag LIVE
                    If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: Start download new global.ini (full) ... " : Logtext.Refresh()
                    url = "https://www.fwkart.de/live-full-ini"
                Else
                    'regular LIVE
                    If Verbose = True Then Logtext.Text = Logtext.Text & "LIVE: Start download new global.ini (regular) ... " : Logtext.Refresh()
                    url = "https://www.fwkart.de/live-ini"
                End If

                'Prüfung darauf ob das File neue ist über HTTP.request - Sever unterstützt dies im Moment nicht  !
                'If Await IsFileNew(url, client, target) Then
                'Beep()
                'End If

                ' Download the file as a byte array
                Dim data As Byte() = Await client.GetByteArrayAsync(url)

                ' Save the downloaded data to a file
                Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), "global.ini")
                Await File.WriteAllBytesAsync(tempFilePath, data)

                ' Copy the file to the desired location
                Dim destinationPath As String = Path.Combine(SC_Installationpath_LIVE, "data\Localization\german_(germany)\global.ini")
                File.Copy(tempFilePath, destinationPath, True)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & "finished" & vbCrLf
                    Logtext.Refresh()
                End If

            End Using
            'Error routines
        Catch ex As HttpRequestException
            ' Handle HTTP request errors (e.g., network errors, invalid URL)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                Logtext.Refresh()
            End If
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                Logtext.Refresh()
            End If
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                Logtext.Refresh()
            End If
        End Try

        '-----------------------------------------------------------------------------------------------------
        '---------------------------------------------
        '4.) Wiederholung für alle anderen Installationen
        '---------------------------------------------
        '--- PTU -----------------------------------------------------------------------------

        'Check if PTU is available
        If SC_Installationpath_PTU <> "" Then
            Try
                '1a.) Check user.cfg exists
                If File.Exists(SC_Installationpath_PTU & "\user.cfg") Then
                    'Einlesen der user.cfg
                    Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_PTU & "\user.cfg").ToList()

                    ' Suchen ob der Eintrag g_language überhaupt vorhanden ist, falls nein die Einträge vornehmen und im nächsten Schritt die Sprache korrigieren
                    g_language_found = False
                    g_languageAudio_found = False
                    For i As Integer = 0 To lines.Count - 1
                        If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                            g_language_found = True
                        End If
                        If lines(i).Trim().Equals("g_languageAudio = english", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("g_languageAudio=english", StringComparison.OrdinalIgnoreCase) Then
                            g_languageAudio_found = True
                        End If
                    Next
                    'Hinzufügen der Zeilen, falls nicht vorhanden
                    If g_language_found = False Then lines.Add("g_language = german_(germany)")
                    If g_languageAudio_found = False Then lines.Add("g_languageAudio = english")

                    ' Suchen nach Einträgen bezüglich g_language
                    For i As Integer = 0 To lines.Count - 1
                        'If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                        If lines(i).Contains("g_language=") Or lines(i).Contains("g_language =") Then
                            If My.Settings.lang_PTU = "eng" Then
                                lines(i) = "#g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "PTU: eng -> #g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            Else
                                lines(i) = "g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "PTU: de -> g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            End If
                        End If
                    Next
                    ' Write all lines back to the file
                    File.WriteAllLines(SC_Installationpath_PTU & "\user.cfg", lines)
                Else
                    'Create new user.cfg file
                    If Verbose = True Then Logtext.Text = Logtext.Text & "PTU: Creating new user.cfg and folder-tree" & vbCrLf : Logtext.Refresh()
                    Using writer As New StreamWriter(SC_Installationpath_PTU & "\user.cfg", False)
                        writer.WriteLine("g_languageAudio = english")
                        If My.Settings.lang_PTU = "eng" Then
                            writer.WriteLine("#g_language = german_(germany)")
                            'writer.WriteLine("g_language = english")
                        Else
                            writer.WriteLine("g_language = german_(germany)")
                            'writer.WriteLine("#g_language = english")
                        End If
                    End Using
                End If


                '2.) Untersuche ob der Foldertree vorhanden ist und falls nein erstelle ihn neu
                If Directory.Exists(SC_Installationpath_PTU & "\data\Localization\german_(germany)") Then
                    'Pfad existiert und global.ini kann geladen werden
                Else
                    'Pfad existiert nicht und muß angelegt werden
                    Directory.CreateDirectory(SC_Installationpath_PTU & "\data\Localization\german_(germany)")
                End If
                'Error Routines
            Catch ex As HttpRequestException
                ' Handle HTTP request errors (e.g., network errors, invalid URL)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As IOException
                ' Handle IOException (e.g., file write errors)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As Exception
                ' Handle any other exceptions
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            End Try
        End If

        '--- EPTU -----------------------------------------------------------------------------
        If SC_Installationpath_EPTU <> "" Then
            Try
                '1a.) Check user.cfg exists
                If File.Exists(SC_Installationpath_EPTU & "\user.cfg") Then
                    'Einlesen der user.cfg
                    Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_EPTU & "\user.cfg").ToList()

                    ' Suchen ob der Eintrag g_language überhaupt vorhanden ist, falls nein die Einträge vornehmen und im nächsten Schritt die Sprache korrigieren
                    g_language_found = False
                    g_languageAudio_found = False
                    For i As Integer = 0 To lines.Count - 1
                        If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                            g_language_found = True
                        End If
                        If lines(i).Trim().Equals("g_languageAudio = english", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("g_languageAudio=english", StringComparison.OrdinalIgnoreCase) Then
                            g_languageAudio_found = True
                        End If
                    Next
                    'Hinzufügen der Zeilen, falls nicht vorhanden
                    If g_language_found = False Then lines.Add("g_language = german_(germany)")
                    If g_languageAudio_found = False Then lines.Add("g_languageAudio = english")

                    ' Suchen nach Einträgen bezüglich g_language
                    For i As Integer = 0 To lines.Count - 1
                        'If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                        If lines(i).Contains("g_language=") Or lines(i).Contains("g_language =") Then
                            If My.Settings.lang_EPTU = "eng" Then
                                lines(i) = "#g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "EPTU: eng -> #g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            Else
                                lines(i) = "g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "EPTU: de -> g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            End If
                        End If
                    Next
                    ' Write all lines back to the file
                    File.WriteAllLines(SC_Installationpath_EPTU & "\user.cfg", lines)
                Else
                    'Create new user.cfg file
                    If Verbose = True Then Logtext.Text = Logtext.Text & "EPTU: Creating new user.cfg and folder-tree" & vbCrLf : Logtext.Refresh()
                    Using writer As New StreamWriter(SC_Installationpath_EPTU & "\user.cfg", False)
                        writer.WriteLine("g_languageAudio = english")
                        If My.Settings.lang_EPTU = "eng" Then
                            writer.WriteLine("#g_language = german_(germany)")
                            'writer.WriteLine("g_language = english")
                        Else
                            writer.WriteLine("g_language = german_(germany)")
                            'writer.WriteLine("#g_language = english")
                        End If
                    End Using
                End If

                '2.) Untersuche ob der Foldertree vorhanden ist und falls nein erstelle ihn neu
                If Directory.Exists(SC_Installationpath_EPTU & "\data\Localization\german_(germany)") Then
                    'Pfad existiert und global.ini kann geladen werden
                Else
                    'Pfad existiert nicht und muß angelegt werden
                    Directory.CreateDirectory(SC_Installationpath_EPTU & "\data\Localization\german_(germany)")
                End If

                'Error Routines
            Catch ex As HttpRequestException
                ' Handle HTTP request errors (e.g., network errors, invalid URL)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As IOException
                ' Handle IOException (e.g., file write errors)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As Exception
                ' Handle any other exceptions
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            End Try
        End If

        '--- TECH-PREVIEW ------------------------------------------------------------------------------------
        If SC_Installationpath_TECH_PREVIEW <> "" Then

            Try
                '1a.) Check user.cfg exists
                If File.Exists(SC_Installationpath_TECH_PREVIEW & "\user.cfg") Then
                    'Einlesen der user.cfg
                    Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_TECH_PREVIEW & "\user.cfg").ToList()

                    ' Suchen ob der Eintrag g_language überhaupt vorhanden ist, falls nein die Einträge vornehmen und im nächsten Schritt die Sprache korrigieren
                    g_language_found = False
                    g_languageAudio_found = False
                    For i As Integer = 0 To lines.Count - 1
                        If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                            g_language_found = True
                        End If
                        If lines(i).Trim().Equals("g_languageAudio = english", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("g_languageAudio=english", StringComparison.OrdinalIgnoreCase) Then
                            g_languageAudio_found = True
                        End If
                    Next
                    'Hinzufügen der Zeilen, falls nicht vorhanden
                    If g_language_found = False Then lines.Add("g_language = german_(germany)")
                    If g_languageAudio_found = False Then lines.Add("g_languageAudio = english")

                    ' Suchen nach Einträgen bezüglich g_language
                    For i As Integer = 0 To lines.Count - 1
                        'If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                        If lines(i).Contains("g_language=") Or lines(i).Contains("g_language =") Then
                            If My.Settings.lang_TECH_PREVIEW = "eng" Then
                                lines(i) = "#g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "TECH-PREVIEW: eng -> #g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            Else
                                lines(i) = "g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "TECH-PREVIEW: de -> g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            End If
                        End If
                    Next
                    ' Write all lines back to the file
                    File.WriteAllLines(SC_Installationpath_TECH_PREVIEW & "\user.cfg", lines)
                Else
                    'Create new user.cfg file
                    If Verbose = True Then Logtext.Text = Logtext.Text & "TECH-PREVIEW: Creating new user.cfg and folder-tree" & vbCrLf : Logtext.Refresh()
                    Using writer As New StreamWriter(SC_Installationpath_TECH_PREVIEW & "\user.cfg", False)
                        writer.WriteLine("g_languageAudio = english")
                        If My.Settings.lang_TECH_PREVIEW = "eng" Then
                            writer.WriteLine("#g_language = german_(germany)")
                            'writer.WriteLine("g_language = english")
                        Else
                            writer.WriteLine("g_language = german_(germany)")
                            'writer.WriteLine("#g_language = english")
                        End If
                    End Using
                End If


                '2.) Untersuche ob der Foldertree vorhanden ist und falls nein erstelle ihn neu
                If Directory.Exists(SC_Installationpath_TECH_PREVIEW & "\data\Localization\german_(germany)") Then
                    'Pfad existiert und global.ini kann geladen werden
                Else
                    'Pfad existiert nicht und muß angelegt werden
                    Directory.CreateDirectory(SC_Installationpath_TECH_PREVIEW & "\data\Localization\german_(germany)")
                End If

                'Error Routines
            Catch ex As HttpRequestException
                ' Handle HTTP request errors (e.g., network errors, invalid URL)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As IOException
                ' Handle IOException (e.g., file write errors)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As Exception
                ' Handle any other exceptions
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            End Try
        End If

        '--- HOTFIX ------------------------------------------------------------------------------------
        If SC_Installationpath_HOTFIX <> "" Then

            Try
                '1a.) Check user.cfg exists
                If File.Exists(SC_Installationpath_HOTFIX & "\user.cfg") Then
                    'Einlesen der user.cfg
                    Dim lines As List(Of String) = File.ReadAllLines(SC_Installationpath_HOTFIX & "\user.cfg").ToList()

                    ' Suchen ob der Eintrag g_language überhaupt vorhanden ist, falls nein die Einträge vornehmen und im nächsten Schritt die Sprache korrigieren
                    g_language_found = False
                    g_languageAudio_found = False
                    For i As Integer = 0 To lines.Count - 1
                        If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                            g_language_found = True
                        End If
                        If lines(i).Trim().Equals("g_languageAudio = english", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("g_languageAudio=english", StringComparison.OrdinalIgnoreCase) Then
                            g_languageAudio_found = True
                        End If
                    Next
                    'Hinzufügen der Zeilen, falls nicht vorhanden
                    If g_language_found = False Then lines.Add("g_language = german_(germany)")
                    If g_languageAudio_found = False Then lines.Add("g_languageAudio = english")

                    ' Suchen nach Einträgen bezüglich g_language
                    For i As Integer = 0 To lines.Count - 1
                        'If lines(i).Trim().Equals("g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Or lines(i).Trim().Equals("#g_language = german_(germany)", StringComparison.OrdinalIgnoreCase) Then
                        If lines(i).Contains("g_language=") Or lines(i).Contains("g_language =") Then
                            If My.Settings.lang_HOTFIX = "eng" Then
                                lines(i) = "#g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "HOTFIX: eng -> #g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            Else
                                lines(i) = "g_language = german_(germany)"
                                If Verbose = True Then Logtext.Text = Logtext.Text & "HOTFIX: de -> g_language = german_(germany)" & vbCrLf : Logtext.Refresh()
                            End If
                        End If
                    Next
                    ' Write all lines back to the file
                    File.WriteAllLines(SC_Installationpath_HOTFIX & "\user.cfg", lines)
                Else
                    'Create new user.cfg file
                    If Verbose = True Then Logtext.Text = Logtext.Text & "HOTFIX: Creating new user.cfg and folder-tree" & vbCrLf : Logtext.Refresh()
                    Using writer As New StreamWriter(SC_Installationpath_HOTFIX & "\user.cfg", False)
                        writer.WriteLine("g_languageAudio = english")
                        If My.Settings.lang_HOTFIX = "eng" Then
                            writer.WriteLine("#g_language = german_(germany)")
                            'writer.WriteLine("g_language = english")
                        Else
                            writer.WriteLine("g_language = german_(germany)")
                            'writer.WriteLine("#g_language = english")
                        End If
                    End Using
                End If


                '2.) Untersuche ob der Foldertree vorhanden ist und falls nein erstelle ihn neu
                If Directory.Exists(SC_Installationpath_HOTFIX & "\data\Localization\german_(germany)") Then
                    'Pfad existiert und global.ini kann geladen werden
                Else
                    'Pfad existiert nicht und muß angelegt werden
                    Directory.CreateDirectory(SC_Installationpath_HOTFIX & "\data\Localization\german_(germany)")
                End If

                'Error Routines
            Catch ex As HttpRequestException
                ' Handle HTTP request errors (e.g., network errors, invalid URL)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As IOException
                ' Handle IOException (e.g., file write errors)
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            Catch ex As Exception
                ' Handle any other exceptions
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                    Logtext.Refresh()
                End If
            End Try
        End If


        ' ------- PTU - GLobal.ini laden falls erforderlich -------------------------------------------------------------
        Try
            If (SC_Installationpath_PTU <> "" And My.Settings.lang_PTU = "de") Then
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & "PTU: Start download new PTU global.ini ... "
                    Logtext.Refresh()
                End If

                ' Download the PTU global.ini file using HttpClient
                Using client As New HttpClient()
                    Dim url As String = "https://www.fwkart.de/ptu-ini"
                    Dim data As Byte() = Await client.GetByteArrayAsync(url)

                    ' Save the downloaded data to a temporary file
                    Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), "global.ini")
                    Await File.WriteAllBytesAsync(tempFilePath, data)

                    If Verbose = True Then
                        Logtext.Text = Logtext.Text & "finished" & vbCrLf
                        Logtext.Refresh()
                    End If

                    ' Copy the file to the desired PTU location
                    If SC_Installationpath_PTU <> "" Then
                        Dim destinationPath As String = Path.Combine(SC_Installationpath_PTU, "data\Localization\german_(germany)\global.ini")
                        File.Copy(tempFilePath, destinationPath, True)
                    End If
                End Using
            End If

        Catch ex As WebException
            ' Handle WebException (e.g., network errors, invalid URL)
            Console.WriteLine("Error downloading file: " & ex.Message)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()


        End Try

        ' ------- EPTU - GLobal.ini laden falls erforderlich -------------------------------------------------------------
        Try
            If (SC_Installationpath_EPTU <> "" And My.Settings.lang_EPTU = "de") Then
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & "EPTU: Start download new EPTU global.ini ..."
                    Logtext.Refresh()
                End If

                ' Download the EPTU global.ini file using HttpClient
                Using client As New HttpClient()
                    Dim url As String = "https://www.fwkart.de/eptu-ini"
                    Dim data As Byte() = Await client.GetByteArrayAsync(url)

                    ' Save the downloaded data to a temporary file
                    Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), "global.ini")
                    Await File.WriteAllBytesAsync(tempFilePath, data)

                    If Verbose = True Then
                        Logtext.Text = Logtext.Text & "finished" & vbCrLf
                        Logtext.Refresh()
                    End If

                    ' Copy the file to the desired EPTU location
                    If SC_Installationpath_EPTU <> "" Then
                        Dim destinationPath As String = Path.Combine(SC_Installationpath_EPTU, "data\Localization\german_(germany)\global.ini")
                        File.Copy(tempFilePath, destinationPath, True)
                    End If
                End Using
            End If

        Catch ex As WebException
            ' Handle WebException (e.g., network errors, invalid URL)
            Console.WriteLine("Error downloading file: " & ex.Message)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()


        End Try
        ' ------- Tech-Preview - GLobal.ini laden falls erforderlich -------------------------------------------------------------
        Try

            If (SC_Installationpath_TECH_PREVIEW <> "" And My.Settings.lang_TECH_PREVIEW = "de") Then
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & "TECH-P: Start download new Tech-Preview global.ini ...."
                    Logtext.Refresh()
                End If

                ' Download the Tech-Preview global.ini file using HttpClient
                Using client As New HttpClient()
                    Dim url As String
                    If My.Settings.TECH_PREVIEW_ini = "LIVE" Then 'Auswahl der richtigen ini zum Download LIVE oder PTU
                        url = "https://www.fwkart.de/tech-ptu-ini"
                    Else
                        url = "https://www.fwkart.de/tech-ini"
                    End If

                    Dim data As Byte() = Await client.GetByteArrayAsync(url)

                    ' Save the downloaded data to a temporary file
                    Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), "global.ini")
                    Await File.WriteAllBytesAsync(tempFilePath, data)

                    If Verbose = True Then
                        Logtext.Text = Logtext.Text & "(" & My.Settings.TECH_PREVIEW_ini & ") finished" & vbCrLf
                        Logtext.Refresh()
                    End If

                    ' Copy the file to the desired TECH_PREVIEW location
                    If SC_Installationpath_TECH_PREVIEW <> "" Then
                        Dim destinationPath As String = Path.Combine(SC_Installationpath_TECH_PREVIEW, "data\Localization\german_(germany)\global.ini")
                        File.Copy(tempFilePath, destinationPath, True)
                    End If
                End Using
            End If
        Catch ex As WebException
            ' Handle WebException (e.g., network errors, invalid URL)
            Console.WriteLine("Error downloading file: " & ex.Message)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()


        End Try

        ' ------- HOTFIX - GLobal.ini laden falls erforderlich -------------------------------------------------------------
        Try

            If (SC_Installationpath_HOTFIX <> "" And My.Settings.lang_HOTFIX = "de") Then
                If Verbose = True Then
                    Logtext.Text = Logtext.Text & "HOTFIX: Start download new HOTFIX global.ini ...."
                    Logtext.Refresh()
                End If

                ' Download the HOTFIX global.ini file using HttpClient
                Using client As New HttpClient()
                    Dim url As String
                    If My.Settings.HOTFIX_ini = "LIVE" Then 'Auswahl der richtigen ini zum Download LIVE oder PTU
                        url = "https://www.fwkart.de/hotfix-ini"
                    Else
                        url = "https://www.fwkart.de/hotfix-ptu-ini"
                    End If
                    Dim data As Byte() = Await client.GetByteArrayAsync(url)

                    ' Save the downloaded data to a temporary file
                    Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), "global.ini")
                    Await File.WriteAllBytesAsync(tempFilePath, data)

                    If Verbose = True Then
                        Logtext.Text = Logtext.Text & "(" & My.Settings.HOTFIX_ini & ") finished" & vbCrLf
                        Logtext.Refresh()
                    End If

                    ' Copy the file to the desired HOTFIX location
                    If SC_Installationpath_HOTFIX <> "" Then
                        Dim destinationPath As String = Path.Combine(SC_Installationpath_HOTFIX, "data\Localization\german_(germany)\global.ini")
                        File.Copy(tempFilePath, destinationPath, True)
                    End If
                End Using
            End If
        Catch ex As WebException
            ' Handle WebException (e.g., network errors, invalid URL)
            Console.WriteLine("Error downloading file: " & ex.Message)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf : Logtext.Refresh()


        End Try


        '----------------------------------------------------------------------------
        '5.) RSI Launcher aufrufen
        'Checke ob der Pfad vorhanden ist - falls nein rufe eine DirectoryBox auf

        'Suche nach dem Launcher Installationsordner 
        '---- LC-Installationspfad ----------------------------------------------------------------------------------------------------------------
        'Abfragen ob ein Star Citizen Installationspfad vorhanden ist
        If Verbose = True Then Logtext.Text = Logtext.Text & "Starte RSI-Launcher" & vbCrLf : Logtext.Refresh()

        If My.Settings.LC_Installationpath <> "" Then
            'Installationspfad gespeichert - checke Pfad ob noch stimmt und übernehme 
            If File.Exists(My.Settings.LC_Installationpath & "\" & "RSI Launcher.exe") Then
                LC_Installationpath = My.Settings.LC_Installationpath
            Else
                'Pfad stimmt nicht mehr - Lösche den Eintrag im Resourcenfile und stelle zurück auf ""
                My.Settings.LC_Installationpath = ""
                LC_Installationpath = ""
            End If
        End If
        'Falls LC-Installationspfad leer - dann suchen nach Standard-Installationspfad, ansonsten leer lassen und nach Initialisierung Settings page aufrufen
        If My.Settings.LC_Installationpath = "" Then
            'Regulärer Pfad
            If File.Exists("C:\Program Files\Roberts Space Industries\RSI Launcher" & "\" & "RSI Launcher.exe") Then
                My.Settings.LC_Installationpath = "C:\Program Files\Roberts Space Industries\RSI Launcher"
                LC_Installationpath = My.Settings.LC_Installationpath
            End If
            'Gängiger Pfad 1
            If File.Exists("C:\Roberts Space Industries\RSI Launcher" & "\" & "RSI Launcher.exe") Then
                My.Settings.LC_Installationpath = "C:\Roberts Space Industries\RSI Launcher"
                LC_Installationpath = My.Settings.LC_Installationpath
            End If
        End If
        'Falls der LC_Installationspfad immernoch "" ist, dann eine Directorybox aufmachen

        Dim a As Integer = 1

        If File.Exists(LC_Installationpath & "\" & "RSI Launcher.exe") Then
            Process.Start(LC_Installationpath & "\" & "RSI Launcher.exe")
            Label11.Image = Image.FromFile(ImagePath & "Update_und_Launch.png") : Label11.Refresh()
        Else
            If a = 1 Then
                'FileBrowseDialog Routine
                MsgBox("RSI Launcher.exe wurde nicht gefunden. Bitte suchen und öffnen Sie die 'RSI Launcher.exe' über das Open File Menü", vbOKOnly, "RSI Launcher nicht gefunden")
                Using openFileDialog As New OpenFileDialog()
                    openFileDialog.InitialDirectory = "C:\"
                    openFileDialog.Filter = "exe-files ('RSI Launcher.exe')|*.exe"
                    openFileDialog.FilterIndex = 1
                    openFileDialog.RestoreDirectory = True

                    If openFileDialog.ShowDialog() = DialogResult.OK Then
                        ' Get the path of specified file
                        Dim filePath As String = openFileDialog.FileName
                        ' Show the file path in the TextBox
                        LC_Installationpath = Path.GetDirectoryName(filePath)
                    End If
                End Using
                If File.Exists(LC_Installationpath & "\" & "RSI Launcher.exe") Then
                    ' Set the selected path to the TextBox
                    'TextBox1.Text = folderBrowserDialog.SelectedPath

                    My.Settings.LC_Installationpath = LC_Installationpath
                    Process.Start(LC_Installationpath & "\" & "RSI Launcher.exe")
                    Label11.Image = Image.FromFile(ImagePath & "Update_und_Launch.png") : Label11.Refresh()
                Else
                    MsgBox("RSI Launcher.exe fehlt", vbOKOnly, "Fehler")
                    LC_Installationpath = ""
                    My.Settings.LC_Installationpath = ""
                End If

            Else
                'FolderBrowse Dialog Routine ----------------------------------------------------
                Using folderBrowserDialog As New FolderBrowserDialog()
                    ' Set the description (optional)
                    folderBrowserDialog.Description = "Suchen und wählen Sie den Ordner 'RSI Launcher' z.B. unter ..\Roberts Space Industries aus"
                    ' Show the dialog and check if the user clicked OK
                    If folderBrowserDialog.ShowDialog() = DialogResult.OK Then
                        'Fehlerroutine
                        If File.Exists(folderBrowserDialog.SelectedPath & "\" & "RSI Launcher.exe") Then
                            ' Set the selected path to the TextBox
                            'TextBox1.Text = folderBrowserDialog.SelectedPath
                            LC_Installationpath = folderBrowserDialog.SelectedPath
                            My.Settings.LC_Installationpath = folderBrowserDialog.SelectedPath
                            Process.Start(LC_Installationpath & "\" & "RSI Launcher.exe")
                            Label11.Image = Image.FromFile(ImagePath & "Update_und_Launch.png") : Label11.Refresh()
                        Else
                            MsgBox("RSI Launcher.exe fehlt", vbOKOnly, "Fehler")
                            LC_Installationpath = ""
                            My.Settings.LC_Installationpath = ""
                        End If
                    End If
                End Using
            End If

        End If
        My.Settings.Save()
        SaveSettingsToXml()
        If Verbose = True Then Logtext.Text = Logtext.Text & "Launch sequence finished, Settings saved" & vbCrLf : Logtext.Refresh()
        'Wait for 5 seconds
        Await Task.Delay(10000)
        ' Close the application
        'Application.Exit()
        Me.WindowState = FormWindowState.Minimized
    End Sub




    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            My.Settings.Verbose = True : Verbose = True
        Else
            My.Settings.Verbose = False : Verbose = False
        End If
        My.Settings.Save()
    End Sub

    Private Sub Reset_Click(sender As Object, e As EventArgs) Handles Reset.Click
        My.Settings.Reset()
        My.Settings.Save()
        Application.Restart()
        Me.Close()
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        ' Handle the MouseEnter and MouseLeave events
        AddHandler LIVE.MouseEnter, AddressOf Label_MouseEnter
        AddHandler LIVE.MouseLeave, AddressOf Label_MouseLeave
        AddHandler PTU.MouseEnter, AddressOf Label_MouseEnter
        AddHandler PTU.MouseLeave, AddressOf Label_MouseLeave
        AddHandler EPTU.MouseEnter, AddressOf Label_MouseEnter
        AddHandler EPTU.MouseLeave, AddressOf Label_MouseLeave
        AddHandler Tech_Preview.MouseEnter, AddressOf Label_MouseEnter
        AddHandler Tech_Preview.MouseLeave, AddressOf Label_MouseLeave
        AddHandler HOTFIX.MouseEnter, AddressOf Label_MouseEnter
        AddHandler HOTFIX.MouseLeave, AddressOf Label_MouseLeave

        AddHandler LIVE_eng.MouseEnter, AddressOf Label_MouseEnter
        AddHandler LIVE_eng.MouseLeave, AddressOf Label_MouseLeave
        AddHandler PTU_eng.MouseEnter, AddressOf Label_MouseEnter
        AddHandler PTU_eng.MouseLeave, AddressOf Label_MouseLeave
        AddHandler EPTU_eng.MouseEnter, AddressOf Label_MouseEnter
        AddHandler EPTU_eng.MouseLeave, AddressOf Label_MouseLeave
        AddHandler TECH_PREVIEW_eng.MouseEnter, AddressOf Label_MouseEnter
        AddHandler TECH_PREVIEW_eng.MouseLeave, AddressOf Label_MouseLeave
        AddHandler HOTFIX_eng.MouseEnter, AddressOf Label_MouseEnter
        AddHandler HOTFIX_eng.MouseLeave, AddressOf Label_MouseLeave

        AddHandler LIVE_de.MouseEnter, AddressOf Label_MouseEnter
        AddHandler LIVE_de.MouseLeave, AddressOf Label_MouseLeave
        AddHandler LIVE_de_voll.MouseEnter, AddressOf Label_MouseEnter
        AddHandler LIVE_de_voll.MouseLeave, AddressOf Label_MouseLeave
        AddHandler PTU_de.MouseEnter, AddressOf Label_MouseEnter
        AddHandler PTU_de.MouseLeave, AddressOf Label_MouseLeave
        AddHandler EPTU_de.MouseEnter, AddressOf Label_MouseEnter
        AddHandler EPTU_de.MouseLeave, AddressOf Label_MouseLeave
        AddHandler TECH_PREVIEW_de.MouseEnter, AddressOf Label_MouseEnter
        AddHandler TECH_PREVIEW_de.MouseLeave, AddressOf Label_MouseLeave
        AddHandler HOTFIX_de.MouseEnter, AddressOf Label_MouseEnter
        AddHandler HOTFIX_de.MouseLeave, AddressOf Label_MouseLeave

        AddHandler TPLIVEini.MouseEnter, AddressOf Label_MouseEnter
        AddHandler TPLIVEini.MouseLeave, AddressOf Label_MouseLeave
        AddHandler TPPTUini.MouseEnter, AddressOf Label_MouseEnter
        AddHandler TPPTUini.MouseLeave, AddressOf Label_MouseLeave

        AddHandler HOTFIXPTUini.MouseEnter, AddressOf Label_MouseEnter
        AddHandler HOTFIXLIVEini.MouseLeave, AddressOf Label_MouseLeave
        AddHandler HOTFIXLIVEini.MouseEnter, AddressOf Label_MouseEnter
        AddHandler HOTFIXLIVEini.MouseLeave, AddressOf Label_MouseLeave


        AddHandler Label11.MouseEnter, AddressOf Label_MouseEnter
        AddHandler Label11.MouseLeave, AddressOf Label_MouseLeave

        AddHandler Update_available.MouseEnter, AddressOf Label_MouseEnter
        AddHandler Update_available.MouseLeave, AddressOf Label_MouseLeave


        AddHandler Reset.MouseEnter, AddressOf Label_MouseEnter
        AddHandler Reset.MouseLeave, AddressOf Label_MouseLeave

        AddHandler CheckBox1.MouseEnter, AddressOf Label_MouseEnter
        AddHandler CheckBox1.MouseLeave, AddressOf Label_MouseLeave

        AddHandler Discord.MouseEnter, AddressOf Label_MouseEnter
        AddHandler Discord.MouseLeave, AddressOf Label_MouseLeave


    End Sub


    Private Sub Label_MouseEnter(sender As Object, e As EventArgs)
        ' Change the cursor to a hand when the mouse enters the label
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub Label_MouseLeave(sender As Object, e As EventArgs)
        ' Revert the cursor to the default when the mouse leaves the label
        Me.Cursor = Cursors.Default
    End Sub



    Private Sub Discord_Click(sender As Object, e As EventArgs) Handles Discord.Click
        Process.Start(New ProcessStartInfo("https://discord.gg/5VZsTk3qjR") With {.UseShellExecute = True})
    End Sub


    '-------------------------------------------------------------------------------------------------------------------------------
    ' --- UPDATE Function 
    '-------------------------------------------------------------------------------------------------------------------------------

    ' 1.) Download https://fwkart.de/apps/scdl_release.txt
    ' Inhalt "release:1.3.0"
    ' 2.) Download https://fwkart.de/apps/sc_deutsch_launcher_update.zip

    Function CurrentVersionCheck(CurrentVersion As String) As Boolean

        ' new HTTP.Client -----------------------------------------------------------------------------------
        Try

            Using client As New HttpClient()
                If Verbose = True Then Logtext.Text = Logtext.Text & "Checking Version ... installed version " & CurrentVersion : Logtext.Refresh()
                Dim url As String = "https://fwkart.de/apps/scdl_release.txt"
                'Dim localFilePath As String = "C:\path\to\version.txt"
                Dim tempFilePath As String = Path.Combine(Path.GetTempPath(), "SCDeutschLauncher_version.txt")

                Dim response As HttpResponseMessage = client.GetAsync(url).Result
                If response.IsSuccessStatusCode Then
                    Dim fileContent As String = response.Content.ReadAsStringAsync().Result
                    ' Save the content to a local file
                    File.WriteAllText(tempFilePath, fileContent)
                Else
                    Console.WriteLine("Failed to download file. Status code: " & response.StatusCode)
                    If Verbose = True Then Logtext.Text = Logtext.Text & "Version check failed - Error scdl_release.txt download" : Logtext.Refresh()
                    Return True
                    Exit Function 'Funktion verlassen bei Fehler - kein Update
                End If

                'Auslesen der Versionsnummer
                If File.Exists(tempFilePath) Then
                    ' Read all lines from the file
                    Dim lines() As String = File.ReadAllLines(tempFilePath)

                    ' Assuming the version line is in the format "version:1.3.0.0"

                    For Each line As String In lines
                        If line.StartsWith("release:") Then
                            ' Extract the version number by splitting the string
                            Dim versionNumber As String = line.Split(":"c)(1).Trim()

                            ' Output the version number for verification
                            Console.WriteLine("Extracted Version Number: " & versionNumber)
                            'MsgBox("Update available Version installed: " & CurrentVersion & " Neue Version: " & versionNumber, vbOKOnly, "Update")
                            ' Step 4: Compare the version number
                            If versionNumber = CurrentVersion Then
                                If Verbose = True Then Logtext.Text = Logtext.Text & "Most current version installed" : Logtext.Refresh()
                                Return True
                            Else
                                'sub UpdateRoutine
                                Return False
                            End If
                        End If
                    Next
                Else
                    Console.WriteLine("File not found.")
                    If Verbose = True Then Logtext.Text = Logtext.Text & "Internal Error - local version.txt not found" : Logtext.Refresh()
                    MsgBox("Fehler im Update Vergleich", vbOKOnly, "Update")
                    Return True
                    Exit Function 'Funktion verlassen bei Fehler - kein Update
                End If
            End Using


            'Error routines
        Catch ex As HttpRequestException
            ' Handle HTTP request errors (e.g., network errors, invalid URL)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Return True
                Exit Function 'Funktion verlassen bei Fehler - kein Update
            End If
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Return True
                Exit Function 'Funktion verlassen bei Fehler - kein Update
            End If
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Return True
                Exit Function 'Funktion verlassen bei Fehler - kein Update
            End If
        End Try
    End Function


    Sub StartUpdateProcess()
        'Update über eigenen download von einer Website
        MsgBox("Ein Update ist verfügbar!" & vbCrLf & vbCrLf & "Wenn du auf 'OK' klickst, öffnet sich eine Website auf der du das Update herunterladen kannst.", vbOKOnly, "Update")
        Process.Start("explorer.exe", "https://www.fwkart.de/sc-deutsch-launcher-update")
        Environment.Exit(0)

    End Sub

    Sub obsolete_StartUpdateProcess()
        '1.) Download zip file
        ' new HTTP.Client -----------------------------------------------------------------------------------
        Try

            Using client As New HttpClient()
                If Verbose = True Then Logtext.Text = Logtext.Text & "Downloading new version ... " : Logtext.Refresh()
                Dim url As String = "https://fwkart.de/scdl-update"
                Dim tempDirectory As String = Path.Combine(Path.GetTempPath(), "sc_deutsch_launcher_update_temp") 'path for extraction
                Dim localZipPath As String = Path.Combine(Path.GetTempPath(), "sc_deutsch_launcher_update.zip") ' path for zip file

                If Directory.Exists(tempDirectory) Then
                    'Delete all files in directory if already exists from previous updates
                    Try
                        For Each d In Directory.GetDirectories(tempDirectory)
                            Directory.Delete(d, True)
                        Next
                        ' Finish removing also the files in the root folder
                        For Each f In Directory.GetFiles(tempDirectory)
                            File.Delete(f)
                        Next
                        Console.WriteLine("All files have been deleted successfully.")
                    Catch ex As Exception
                        Console.WriteLine("An error occurred: " & ex.Message)

                    End Try
                Else
                    'If no directory, then create one
                    Directory.CreateDirectory(tempDirectory)
                End If

                Dim response As HttpResponseMessage = client.GetAsync(url).Result
                If response.IsSuccessStatusCode Then
                    'zip file downloaded
                    File.WriteAllBytes(localZipPath, response.Content.ReadAsByteArrayAsync().Result)
                    'zipFile Extract
                    ZipFile.ExtractToDirectory(localZipPath, tempDirectory)
                Else
                    Console.WriteLine("Failed to download file. Status code: " & response.StatusCode)
                    If Verbose = True Then Logtext.Text = Logtext.Text & "zip file download error - Update cancelled" : Logtext.Refresh()
                    Exit Sub  'Funktion verlassen bei Fehler - kein Update
                End If
            End Using

            '2b.) Ausschreiben der My.Settings Parameter in ein XML File
            SaveSettingsToXml()

            '3.) Ausschreiben der BAT Routine um die Dateien in das Standardverzeichnis kopieren zu können.

            Dim batFilePath As String = Path.Combine(Path.GetTempPath(), "update.bat")
            Dim exePath As String = Application.ExecutablePath
            Dim batContent As String = $"
@echo off
:loop
tasklist /FI ""IMAGENAME eq {Path.GetFileName(exePath)}"" 2>NUL | find /I ""{Path.GetFileName(exePath)}"" >NUL
IF ""%ERRORLEVEL%"" == ""0"" (
timeout /T 1 /NOBREAK > NUL
goto loop
)
xcopy ""{Path.GetTempPath()}sc_deutsch_launcher_update_temp\*"" ""{AppDomain.CurrentDomain.BaseDirectory}"" /E /I /Y
start """" ""{exePath}""
del ""%~f0""
"

            File.WriteAllText(batFilePath, batContent)
            'MsgBox("Program will close for update", vbOKOnly, "Close")
            Process.Start(batFilePath)
            Environment.Exit(0)



            'Error routines
        Catch ex As HttpRequestException
            ' Handle HTTP request errors (e.g., network errors, invalid URL)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error downloading file: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Exit Sub  'Funktion verlassen bei Fehler - kein Update
            End If
        Catch ex As IOException
            ' Handle IOException (e.g., file write errors)
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "Error writing file: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Exit Sub  'Funktion verlassen bei Fehler - kein Update
            End If
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Exit Sub  'Funktion verlassen bei Fehler - kein Update
            End If
        End Try
    End Sub

    Sub SaveSettingsToXml()
        Dim exeDirectory As String = Path.GetTempPath()
        Dim xmlFileName As String = "settings.xml"
        Dim execPath As String = Path.Combine(exeDirectory, xmlFileName)
        Try
            If File.Exists(execPath) Then File.Delete(execPath)

            Dim settingsXml As New System.Xml.XmlTextWriter(execPath, System.Text.Encoding.UTF8)
            settingsXml.Formatting = System.Xml.Formatting.Indented
            settingsXml.WriteStartDocument()
            settingsXml.WriteStartElement("Settings")

            ' Loop through each setting and write it to the XML file
            For Each setting As SettingsProperty In My.Settings.Properties
                settingsXml.WriteStartElement(setting.Name)
                settingsXml.WriteString(My.Settings(setting.Name).ToString())
                settingsXml.WriteEndElement()
            Next

            settingsXml.WriteEndElement()
            settingsXml.WriteEndDocument()
            settingsXml.Close()
        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Exit Sub  'Funktion verlassen bei Fehler - kein Update
            End If
        End Try

    End Sub


    Sub LoadSettingsFromXml()
        Dim exeDirectory As String = Path.GetTempPath()
        Dim xmlFileName As String = "settings.xml"
        Dim execPath As String = Path.Combine(exeDirectory, xmlFileName)
        Try
            ' Check if the file exists
            If File.Exists(execPath) Then
                Dim settingsXml As New XmlDocument()
                settingsXml.Load(execPath)

                ' Loop through each setting in the XML file and update My.Settings
                For Each settingNode As XmlNode In settingsXml.DocumentElement.ChildNodes
                    Dim settingName As String = settingNode.Name
                    Dim settingValue As String = settingNode.InnerText

                    ' Check if the setting exists in My.Settings
                    If My.Settings.Properties.Cast(Of SettingsProperty).Any(Function(prop) prop.Name = settingName) Then
                        My.Settings(settingName) = Convert.ChangeType(settingValue, My.Settings(settingName).GetType())
                        My.Settings.Save()
                    End If
                Next

                ' Save the updated settings
                My.Settings.Save()
            Else
                Console.WriteLine("Settings file not found.")
            End If

        Catch ex As Exception
            ' Handle any other exceptions
            If Verbose = True Then
                Logtext.Text = Logtext.Text & " ERROR" & vbCrLf & "An error occurred: " & ex.Message & vbCrLf
                Logtext.Refresh()
                Exit Sub  'Funktion verlassen bei Fehler - kein Update
            End If
        End Try
    End Sub


    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.Save()
        SaveSettingsToXml()
    End Sub

    Private Sub Update_available_Click(sender As Object, e As EventArgs) Handles Update_available.Click
        StartUpdateProcess()
    End Sub
End Class
