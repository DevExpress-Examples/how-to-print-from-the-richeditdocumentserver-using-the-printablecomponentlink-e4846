﻿Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Forms
Imports System.Drawing.Printing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native

Namespace RichEdit_PrintingSystem
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub
		#Region "#printfromserver"
		Private Sub btn_PrintFromServer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn_PrintFromServer.Click
			Dim srv As New RichEditDocumentServer()
			srv.LoadDocument("Grimm.docx", DocumentFormat.OpenXml)
			' Insert a field displaying the current date/time into the document header.
			srv.BeginUpdate()
			Dim _header As SubDocument = srv.Document.Sections(0).BeginUpdateHeader()
			_header.Fields.Add(_header.Range.Start,"DATE \@ ""dddd, MMMM dd, yyyy  HH:mm:ss"" \MERGEFORMAT")
			_header.Paragraphs(0).Alignment = ParagraphAlignment.Right
			srv.Document.Sections(0).EndUpdateHeader(_header)
			' Specify page margins, orientation, etc.
			SetPrintOptions(srv)
			srv.EndUpdate()
			' Display field values instead of code.
			srv.Options.MailMerge.ViewMergedData = True
			' Create a printable link to print a document.
			PrintViaLink(srv)
		End Sub
		#End Region ' #printfromserver
		#Region "#setprintoptions"
		Private Shared Sub SetPrintOptions(ByVal richedit As IRichEditDocumentServer)
			For Each _section As Section In richedit.Document.Sections
				_section.Page.PaperKind = System.Drawing.Printing.PaperKind.A4
				_section.Page.Landscape = False
				_section.Margins.Left = 150f
				_section.Margins.Right = 150f
				_section.Margins.Top = 50f
				_section.Margins.Bottom = 50f
				_section.PageNumbering.NumberingFormat = NumberingFormat.CardinalText
				_section.PageNumbering.Start = 0
			Next _section
		End Sub
		#End Region ' #setprintoptions
		#Region "#printvialink"
		Private Shared Sub PrintViaLink(ByVal srv As RichEditDocumentServer)
			If (Not srv.IsPrintingAvailable) Then
				Return
			End If
			Dim link As New PrintableComponentLink(New PrintingSystem())
			link.Component = srv
			' Disable warnings.
			link.PrintingSystem.ShowMarginsWarning = False
			link.PrintingSystem.ShowPrintStatusDialog = False
			' Find a printer containing 'Canon' in its name.
			Dim printerName As String = String.Empty
			For i As Integer = 0 To PrinterSettings.InstalledPrinters.Count - 1
				Dim pName As String = PrinterSettings.InstalledPrinters(i)
				If pName.Contains("Canon") Then
					printerName = pName
					Exit For
				End If
			Next i
			' Print to the specified printer.
			link.Print(printerName)
		End Sub
		#End Region ' #printvialink
	End Class
End Namespace
