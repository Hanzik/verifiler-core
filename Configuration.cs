using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NLog;

namespace VerifilerCore {

	/// <summary>
	/// Configuration keeps library-wide settings such location of scanned files,
	/// verbosity of the output or other usefull data that most (if not all) verification
	/// steps need to know.
	/// 
	/// Configuration also prepares these variables at the start of each scan via Prepare()
	/// method and cleans up after itself via Cleanup() method.
	/// </summary>
	public class Configuration {

		private static Configuration instance;

		public int OutputType { get; set; } = Const.OutputBrief;
		/// <summary>
		/// Path to folder which user wants to scan.
		/// </summary>
		public string ScanPath { get; set; }
		/// <summary>
		/// Path to temporary folder used to extract archive (if one was provided instead
		/// of a normal folder).
		/// </summary>
		public string TempFolder { get; set; }
		/// <summary>
		/// List of paths to every file (contained within ScanPath folder) to undergo
		/// Verifile scan.
		/// </summary>
		public List<string> FileList { get; set; }

		public bool FormatSpecificEnabled { get; set; }

		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public Configuration() {
			FileList = new List<string>();
		}

		public static Configuration Instance {
			get {
				if (instance == null) {
					instance = new Configuration();
				}
				return instance;
			}
		}

		public int Prepare() {
			TempFolder = null;

			if (!IsPathValid()) {
				return Error.ScanPathInvalid;
			}

			try {
				PrepareFileList();
			} catch (FileNotFoundException ex) {
				logger.Error(ex.Message);
				return Error.FileNotFound;
			}

			return Result.Ok;
		}

		public void Cleanup() {
			if (TempFolder != null) {
				logger.Info("Deleting temporary folder {0}", TempFolder);
				Directory.Delete(TempFolder);
			}
			FileList.Clear();
		}

		private bool IsPathValid() {

			if (ScanPath == null) {
				logger.Error("Scan path is not set");
				return false;
			}

			try {
				var info = File.GetAttributes(ScanPath);
			} catch (Exception ex) {
				logger.Error(ex.Message);
				return false;
			}

			logger.Debug("Scan path seems to be valid");
			return true;
		}

		/// <summary>
		/// Fills up FileList variable with path to every file within ScanPath location.
		/// Should an archive be provided, it will be extracted to temporary folder from which
		/// the FileList will be filled.
		/// </summary>
		private void PrepareFileList() {

			FileAttributes attributes = File.GetAttributes(ScanPath);

			/** Path points to a directory, add list of contents recursively
				to list of files. */
			if ((attributes & FileAttributes.Directory) == FileAttributes.Directory) {
				logger.Info("Discovered a directory - Files inside will be recursively added to the list of scanned files");
				AddDirectoryContentsToFileList(ScanPath);
				return;
			}

			/** Path points to an archive, extract contents to temp folder and add
				them to list of files. */
			if (Path.GetExtension(ScanPath).Equals(".zip")) {
				logger.Info("Discovered an archive - Files will be extracted and recursively added to the list of scanned files");
				var tempFolder = ExtractArchive();
				AddDirectoryContentsToFileList(tempFolder);
				return;
			}

			/** Path points to a file, extract to temp folder. */
			FileList.Add(ScanPath);
		}

		private void AddDirectoryContentsToFileList(string dir) {
			var files = Directory.GetFiles(dir);
			var directories = Directory.GetDirectories(dir);
			foreach (var file in files) {
				FileList.Add(file);
			}
			foreach (var directory in directories) {
				AddDirectoryContentsToFileList(directory);
			}
		}

		private string ExtractArchive() {
			TempFolder = Path.GetTempPath() + "/" + Guid.NewGuid().ToString();
			logger.Debug("Creating a temporary directory {0}", TempFolder);
			Directory.CreateDirectory(TempFolder);
			ZipFile.ExtractToDirectory(ScanPath, TempFolder);
			AddDirectoryContentsToFileList(TempFolder);
			return TempFolder;
		}
	}
}