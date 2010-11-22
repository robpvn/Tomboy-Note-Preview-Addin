CSOBJECTS = NotePreviewAddin.cs
CSC = gmcs

all: ${CSOBJECTS}
	$(CSC) ${CSOBJECTS} -debug -out:NotePreviewAddin.dll -target:library -pkg:tomboy-addins -r:Mono.Posix -resource:NotePreview.addin.xml

install: ${CSOBJECTS}
	$(CSC) ${CSOBJECTS} -out:/usr/lib/tomboy/addins/NotePreviewAddin.dll -target:library -pkg:tomboy-addins -r:Mono.Posix -resource:NotePreview.addin.xml
