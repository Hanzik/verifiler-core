using System.Collections.Generic;
using System.IO;
using NLog;

namespace VerifilerCore {

	/// <summary>
	/// Base class for every format specific validators. RelevantExtensions
	/// decide, whether or not is the class going to be called for validating
	/// file of the given extension. This allows one validator to work with
	/// multiple extensions should this be needed.
	/// </summary>
	public class FormatSpecificValidator : Step {

		protected HashSet<string> RelevantExtensions = new HashSet<string>();
		protected int FilesVerified = 0;

		public override int ErrorCode { get; set; } = Error.Generic;

		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public override void Setup() { }

		public override void Run() {
			foreach (var file in Configuration.Instance.FileList) {
				var extension = Path.GetExtension(file);
				extension = extension?.ToLower();
				if (RelevantExtensions.Contains(extension)) {
					logger.Debug("Verifying integrity of {0}", file);
					ValidateFile(file);
				}
			}
		}

		public virtual void ValidateFile(string file) { }
	}
}
