namespace VerifilerCore {
	/// <summary>
	/// Constants for error reporting.
	/// </summary>
	public class Error {
		public const int Generic = 1;
		public const int Fatal = 2;
		public const int AvTriggered = 3;
		public const int Multiple = 4;
		public const int Extension = 101;
		public const int Signature = 102;
		public const int Size = 103;
		public const int Checksum = 104;
		public const int VirusTotal = 105;
		public const int FileNotFound = 106;
		public const int FilenameInvalid = 107;
		public const int ScanPathInvalid = 108;
		public const int Corrupted = 109;
		public const int Locked = 109;
	}
}