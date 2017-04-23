using System.Collections.Generic;
using NLog;

namespace VerifilerCore {

	/// <summary>
	/// Result encapsulates scan result's data that might be relevant for the end user.
	/// </summary>
	public class Result {

		public const int Ok = 0;

		private int ResponseCode { get; set; }
		private readonly List<string> validFilesList;
		private readonly Dictionary<string, int> invalidFilesList;
		private readonly List<string> stepsExecuted;

		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public Result() {
			ResponseCode = Ok;
			validFilesList = new List<string>();
			invalidFilesList = new Dictionary<string, int>();
			stepsExecuted = new List<string>();
		}

		public void AddExecutedStep(string stepName) {
			stepsExecuted.Add(stepName);
		}

		public void SetFilesValid(List<string> filenames) {
			foreach (var filename in filenames) {
				invalidFilesList.Remove(filename);
				validFilesList.Add(filename);
			}
		}

		public void SetFilesInvalid(List<string> filenames, int code) {
			foreach (var filename in filenames) {
				validFilesList.Remove(filename);
				invalidFilesList.Add(filename, code);
			}
		}

		public List<string> GetValidFiles() {
			return validFilesList;
		}

		public Dictionary<string, int> GetInvalidFiles() {
			return invalidFilesList;
		}

		public List<string> GetStepsExecuted() {
			return stepsExecuted;
		}

		public int Code() {
			return ResponseCode;
		}

		public void SetResponseCode(int code) {
			logger.Debug("Setting response code to {0}", code);
			ResponseCode = code;
		}
	}
}