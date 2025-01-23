using SystemPath = System.IO.Path;
using SystemDirectory = System.IO.Directory;
using System.Text;

namespace CsDocumentJsonGenerator.FileSystem;

public class DirectoryGroup {

	//======================================================================| Properties

	public string Path { get; init; }

	public DirectoryInfo ThisDirectory { get; init; }
	public IReadOnlyList<FileInfo> Files { get; init; }
	public IReadOnlyList<DirectoryGroup> Directories { get; init; }


	//======================================================================| Constructors

	public DirectoryGroup(string directoryPath) {

		bool pathNotFound = !SystemPath.Exists(directoryPath);
		
		if (pathNotFound)
			throw new DirectoryNotFoundException($"Given directory \"{directoryPath}\" was not found.");

		Path = directoryPath;
		ThisDirectory = new(directoryPath);
		Files = GetIncludedFiles();
		Directories = GetIncludedDirectories();

	}

	//======================================================================| Methods

	public override string ToString() {
		StringBuilder builder = new();
		BuildTree(builder, "", true, true);
		return builder.ToString();
	}

	//======================================================================| Private Methods

	private List<FileInfo> GetIncludedFiles() {

		string[] fileNames = SystemDirectory.GetFiles(Path);
		
		List<FileInfo> result = [];
		foreach (var fileName in fileNames) {
			result.Add(new(fileName));
		}

		return result;
	}

	private List<DirectoryGroup> GetIncludedDirectories() {

		string[] directoryNames = SystemDirectory.GetDirectories(Path);

		List<DirectoryGroup> result = [];
		foreach (var directoryName in directoryNames) {
			result.Add(new(directoryName));
		}

		return result;
	}

	private void BuildTree(StringBuilder builder, string indent, bool isLast, bool isRoot = false) {
		
		builder.Append(indent);

		if (!isRoot) 
			builder.Append(isLast ? "└─" : "├─");

		builder.AppendLine($"/{new DirectoryInfo(Path).Name}");

		string childIndent = indent + (isLast ? "   " : "│  ");

		for (int i = 0; i < Files.Count; i++) {
			
			bool isLastFile = 
				i == Files.Count - 1 &&
				!Directories.Any();

			builder
				.Append(childIndent)
				.Append(isLastFile ? "└─" : "├─")
				.AppendLine($" {Files[i].Name}");

		}

		for (int i = 0; i < Directories.Count; i++) {
			bool isLastDirectory = i == Directories.Count - 1;
			Directories[i].BuildTree(builder, childIndent, isLastDirectory);
		}

	}

}