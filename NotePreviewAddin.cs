/*
 *Note Preview Addin: Displays a tooltip with a preview of the note's 
 *content when you hover on a link.
 *
 *(Borrows heavily from the Tomboy MouseHandWatcher class in Watchers.cs)
 *
 * Author:
 * Robert Nordan (rpvn@robpvn.net)
 *
 * Copyright (C) 2010 Robert Nordan, licensed under the LGPL
*/
using System;
using Mono.Unix;
using Gtk;
using Gdk;
using Tomboy;

namespace Tomboy.NotePreview
{

	public class NotePreviewAddin : NoteAddin
	{
		bool hovering_on_link;
		Gtk.TextView editor;
		
		public override void Initialize ()
		{
		}
		
		public override void Shutdown ()
		{
		}
		
		public override void OnNoteOpened ()
		{
			editor = Window.Editor;
			editor.MotionNotifyEvent += OnEditorMotion;	
		}
		
		[GLib.ConnectBefore]
		void OnEditorMotion (object sender, Gtk.MotionNotifyEventArgs args)
		{
			string note_title = "";
			
			int pointer_x, pointer_y;
			Gdk.ModifierType pointer_mask;

			Window.Editor.GdkWindow.GetPointer (out pointer_x,
			                                    out pointer_y,
			                                    out pointer_mask);

			bool hovering = false;

			// Figure out if we're on a link by getting the text
			// iter at the mouse point, and checking for tags that
			// start with "link:"...

			int buffer_x, buffer_y;
			Window.Editor.WindowToBufferCoords (Gtk.TextWindowType.Widget,
			                                    pointer_x,
			                                    pointer_y,
			                                    out buffer_x,
			                                    out buffer_y);

			Gtk.TextIter iter = Window.Editor.GetIterAtLocation (buffer_x,
			                                                     buffer_y);
			
			foreach (Gtk.TextTag tag in iter.Tags) {
				if (tag.Name == "link:internal") {
					hovering = true;
					
					/* Check that we're not already at the first letter
					 * of the tagged section! */
					if (!iter.BeginsTag (tag)) {
						iter.BackwardToTagToggle (tag);
					}
					
					TextIter end = iter;
					end.ForwardToTagToggle (tag);
					
					note_title = iter.GetText (end);
					break;
				}
			}
	
			if (hovering != hovering_on_link) {
				hovering_on_link = hovering;
			
				if (hovering) { //Have moved over a note link
					editor.TooltipText = getNotePreview (note_title);
				} else {			//Have moved away from note link
					editor.TooltipText = "";
				}
			}
		}
		
		private string getNotePreview (string note_title)
		{
			Logger.Debug ("NotePreviewAddin: Looking up note with title \"" 
			              + note_title + "\"");
			
			Note linkedTo = Manager.Find (note_title);
			
			if (linkedTo == null) {
					return "couldn't find \"" + note_title + "\"";
			}
			
			string note_content = linkedTo.TextContent;
			
			/*If the note is longer than 400 chars we trim it down. 
			 * TODO: Can turn it into a preferences option?*/
			if (note_content.Length > 400) {
				return note_content.Remove (400) + " ...";
			} else {
				return note_content;
			}
		}	
	}
}