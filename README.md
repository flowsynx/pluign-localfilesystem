# FlowSynx Local FileSystem Plugin

The Local FileSystem Plugin is a pre-packaged, plug-and-play integration component for the FlowSynx engine. It enables interacting with local or mounted file systems, allowing workflows to read, write, list, and manage files and directories. Designed for FlowSynx's no-code/low-code automation workflows, this plugin simplifies local file system integration for data pipelines and automation tasks.

This plugin is automatically installed by the FlowSynx engine when selected within the platform. It is not intended for manual installation or standalone developer use outside the FlowSynx environment.

## Purpose

The Local FileSystem Plugin allows FlowSynx users to:

- Create files and directories on the local or mounted file system.
- Read data from local files.
- Write data to local files.
- List files and directories with filtering options.
- Delete files or directories.
- Check the existence of files or directories.
- Purge directories when needed.
- Rename files and directories.
- Integrate local file system operations into automation workflows without writing code.

## Supported Operations

- **create**: Creates a new directory at the specified path.
- **delete**: Deletes a file at the specified path.
- **exist**: Checks if a file or directory exists at the specified path.
- **list**: Lists files and directories under a specified path, with filtering and optional metadata.
- **purge**: Deletes all files and directories under the specified path, optionally forcing deletion.
- **read**: Reads and returns the contents of a file at the specified path.
- **rename**: Renames a file or directory from an old path to a new path.
- **write**: Writes data to a specified path, with support for overwrite.

## Plugin Specifications

The plugin does not require any connection configuration as it operates on the local file system where the FlowSynx engine is running.

### Example Configuration

```json
{}
```

## Input Parameters

Each operation accepts specific parameters:

### Create
| Parameter     | Type    | Required | Description                                          |
|---------------|---------|----------|------------------------------------------------------|
| `Path`        | string  | Yes      | The path where the new directory is created.         |
| `Hidden`      | bool    | No       | Whether to mark the file as hidden. Default: `false`.|

### Delete
| Parameter     | Type    | Required | Description                     |
|---------------|---------|----------|---------------------------------|
| `Path`        | string  | Yes      | The path of the file to delete. |

### Exist
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the file or directory to check. |

### List
| Parameter         | Type    | Required | Description                                         |
|--------------------|---------|----------|-----------------------------------------------------|
| `Path`             | string  | Yes      | The directory path to list files from.             |
| `Filter`           | string  | No       | A filter pattern for file/directory names.         |
| `Recurse`          | bool    | No       | Whether to list recursively. Default: `false`.     |
| `CaseSensitive`    | bool    | No       | Whether the filter is case-sensitive. Default: `false`. |
| `IncludeMetadata`  | bool    | No       | Whether to include file metadata. Default: `false`. |
| `MaxResults`       | int     | No       | Maximum number of items to list. |

### Purge
| Parameter     | Type    | Required | Description                                   |
|---------------|---------|----------|-----------------------------------------------|
| `Path`        | string  | Yes      | The directory path to purge.                  |
| `Force`       | bool    | No       | Whether to force deletion without confirmation. Default: `false`. |

### Read
| Parameter     | Type    | Required | Description                             |
|---------------|---------|----------|-----------------------------------------|
| `Path`        | string  | Yes      | The path of the file to read.           |

### Rename
| Parameter     | Type    | Required | Description                                        |
|---------------|---------|----------|----------------------------------------------------|
| `Path`		| string  | Yes      | The current path of the file or directory.         |
| `TargetPath`  | string  | Yes      | The new path for the file or directory.            |

### Write
| Parameter     | Type    | Required | Description                                                 |
|---------------|---------|----------|-------------------------------------------------------------|
| `Path`        | string  | Yes      | The path where data should be written.                      |
| `Data`        | object  | Yes      | The data to write to the file.                              |
| `Overwrite`   | bool    | No       | Whether to overwrite if the file already exists. Default: `false`. |

### Example input (Write)

```json
{
  "Operation": "write",
  "Path": "C:\\data\\documents\\report.json",
  "Data": "{This is the report content.}",
  "Overwrite": true
}
```

## Debugging Tips

- Ensure the path specified exists and is accessible from the FlowSynx engine.
- Verify the FlowSynx engine process has appropriate file system permissions for the target paths.
- Use the `Exist` operation to confirm file or directory presence before performing `Read` or `Delete`.
- For large directory listings, adjust `MaxResults` to limit returned data.
- Use absolute paths to avoid ambiguity, or ensure relative paths are resolved correctly based on the engine's working directory.

## Local FileSystem Considerations

- **Path Separators**: Use the appropriate path separator for your operating system (`\` for Windows, `/` for Linux/macOS). The plugin handles both formats.
- **Case Sensitivity**: File system case sensitivity depends on the underlying OS (case-insensitive on Windows, case-sensitive on Linux/macOS).
- **File Permissions**: The operation will fail if the FlowSynx engine process doesn't have sufficient permissions.
- **Overwrite Behavior**: The `Write` operation will fail if `Overwrite` is `false` and the file exists.
- **Hidden Files**: On Windows, the `Hidden` parameter in the `Create` operation sets the hidden file attribute.
- **Directory Operations**: The `Create` operation can create both files and directories. The `Delete` and `Purge` operations work on both as well.
- **Mounted File Systems**: This plugin works with any file system mounted and accessible to the operating system where FlowSynx is running.

## Security Notes

- File operations are performed with the permissions of the FlowSynx engine process.
- Access is controlled by FlowSynx platform user roles and permissions.
- Be cautious with `Purge` and `Delete` operations as they can permanently remove data.
- Always validate and sanitize paths to prevent unauthorized access to sensitive system directories.
- Consider using relative paths and configuring a working directory to limit file system access scope.

## License

© FlowSynx. All rights reserved.