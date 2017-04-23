/****************************** Module Header ******************************\
Module Name:  Step.cs
Project:      VerifileCore

Parent class to every Verification step. Contains all methods the children
classes must support. 

Every scanned file is marked with one of response codes and the total occurence
of these codes is saved in summary dictionary. This allows calculating the final
response code of the step.

\***************************************************************************/

using System;
using System.Collections.Generic;
using NLog;

namespace VerifilerCore {

	/// <summary>
	/// Root class that every Step should inherit. Provides basic functionality,
	/// and handling of response codes.
	/// </summary>
	public class Step {

		public string Name { get; set; }
		public virtual int ErrorCode { get; set; } = Error.Generic;

		public bool StepAborted;
		public List<string> ValidFilesList { get; }
		public List<string> InvalidFilesList { get; }

		private int errorsEncountered;
		private bool enabled;
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public bool FatalErrorEncountered { get; set; }

		public Step() {
			ValidFilesList = new List<string>();
			InvalidFilesList = new List<string>();
		}

		public void Enable() {
			logger.Debug("Step {0} enabled", Name);
			enabled = true;
		}

		public void Disable() {
			logger.Debug("Step {0} disabled", Name);
			enabled = false;
		}

		public bool Enabled() {
			return enabled;
		}

		public virtual void Setup() {
			errorsEncountered = 0;
		}

		public virtual void Run() { }

		/// <summary>
		/// Prints the results of the step to user and returns the appropriate response code.
		/// </summary>
		/// <returns>
		///   <c>Result.Ok</c> if all files passed step's test
		///   <c>ErrorCode</c> if at least one file didn't pass this step's test
		/// </returns>
		public virtual int Summary() {
			logger.Debug("Retrieving the summary of step {0}", Name);

			var stepResponse = errorsEncountered > 0 ? ErrorCode : Result.Ok;
			string summaryMessage = "Step " + Name + " ended with result code: " + stepResponse;

			if (StepAborted) {
				summaryMessage += "Step was aborted due to an exception";
			} else if (errorsEncountered > 0) {
				summaryMessage += Environment.NewLine + errorsEncountered + " errors encountered";
			}

			logger.Info(summaryMessage);
			return stepResponse;
		}

		/// <summary>
		/// Returns the step and it's essential variables to their default state.
		/// </summary>
		public virtual void Cleanup() {
			logger.Debug("Cleaning up step {0}", Name);
			StepAborted = false;
			FatalErrorEncountered = false;
			errorsEncountered = 0;
			ValidFilesList.Clear();
			InvalidFilesList.Clear();
		}

		protected void ReportAsValid(string file = null, string msg = null) {
			logger.Debug("Valid file reported by step {0} for file {1} with message {2}", Name, file, msg);
			if (file != null) {
				ValidFilesList.Add(file);
			}
			logger.Info(msg);
		}

		protected void ReportAsError(string file = null, string msg = null) {
			logger.Error("Error reported by step {0} for file {1} with message {2}", Name, file, msg);
			if (file != null) {
				InvalidFilesList.Add(file);
			}
			logger.Error(msg);
			errorsEncountered++;
		}

		protected List<string> GetListOfFiles() {
			return Configuration.Instance.FileList;
		}
	}
}