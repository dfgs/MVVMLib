using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ViewLib
{
	public static class RoutedCommands
	{
		private static readonly RoutedUICommand empty;
		public static RoutedUICommand Empty
		{
			get { return empty; }
		}

		private static readonly RoutedUICommand next;
		public static RoutedUICommand Next
		{
			get { return next; }
		}

		private static readonly RoutedUICommand previous;
		public static RoutedUICommand Previous
		{
			get { return previous; }
		}

		private static readonly RoutedUICommand restart	;
		public static RoutedUICommand Restart
		{
			get { return restart; }
		}

		private static readonly RoutedUICommand stop;
		public static RoutedUICommand Stop
		{
			get { return stop; }
		}



		private static readonly RoutedUICommand ok;
		public static RoutedUICommand OK
		{
			get { return ok; }
		}
		public static readonly RoutedUICommand cancel;
		public static RoutedUICommand Cancel
		{
			get { return cancel; }
		}
		public static readonly RoutedUICommand apply;
		public static RoutedUICommand Apply
		{
			get { return apply; }
		}

		public static readonly RoutedUICommand _new;
		public static RoutedUICommand New
		{
			get { return _new; }
		}
		public static readonly RoutedUICommand open;
		public static RoutedUICommand Open
		{
			get { return open; }
		}
		public static readonly RoutedUICommand save;
		public static RoutedUICommand Save
		{
			get { return save; }
		}
		public static readonly RoutedUICommand saveAs;
		public static RoutedUICommand SaveAs
		{
			get { return saveAs; }
		}

		public static readonly RoutedUICommand connect;
		public static RoutedUICommand Connect
		{
			get { return connect; }
		}
		public static readonly RoutedUICommand disconnect;
		public static RoutedUICommand Disconnect
		{
			get { return disconnect; }
		}

		static RoutedCommands()
		{
			InputGestureCollection gestures;

			empty= new RoutedUICommand("Empty", "EmptyCommand", typeof(RoutedCommands));

			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.Enter));
			next = new RoutedUICommand("Next", "NextCommand", typeof(RoutedCommands),gestures);
			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.Back));
			previous = new RoutedUICommand("Previous", "PreviousCommand", typeof(RoutedCommands), gestures);
			restart = new RoutedUICommand("Restart", "NextCommand", typeof(RoutedCommands));
			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.Escape));
			stop = new RoutedUICommand("Stop", "StopCommand", typeof(RoutedCommands), gestures);


			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.Enter));
			ok = new RoutedUICommand("OK", "OKCommand", typeof(RoutedCommands), gestures);
			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.Escape));
			cancel = new RoutedUICommand("Cancel", "CancelCommand", typeof(RoutedCommands), gestures);
			gestures = null;
			apply = new RoutedUICommand("Apply", "ApplyCommand", typeof(RoutedCommands), gestures);


			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
			_new = new RoutedUICommand("New", "NewCommand", typeof(RoutedCommands), gestures);
			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
			open = new RoutedUICommand("Open", "OpenCommand", typeof(RoutedCommands), gestures);
			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
			save = new RoutedUICommand("Save", "SaveCommand", typeof(RoutedCommands), gestures);

			saveAs = new RoutedUICommand("Save as", "SaveAsCommand", typeof(RoutedCommands));

			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.O,ModifierKeys.Control));
			connect = new RoutedUICommand("Connect", "ConnectCommand", typeof(RoutedCommands), gestures);
			gestures = new InputGestureCollection(); gestures.Add(new KeyGesture(Key.X, ModifierKeys.Control));
			disconnect = new RoutedUICommand("Disconnect", "DisconnectCommand", typeof(RoutedCommands), gestures);

		}


	}
}
