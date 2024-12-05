using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FileFoldersGrantData
{

    /// <summary>ユーザーフォルダ内パス※GrantDataのパスではない</summary>
    public string UserPath;
    /// <summary>ファイルフォルダ名※GrantDataのではない</summary>
    public string Name;
    public string Alias= "";
    public string Memorandum= "";
    /// <summary>変数内では先頭の#は付けないで管理する</summary>
    public string[] Hashtag= new string[0];
    public extension Extension { get; set; }
    public bool IsUpdated { get; set; } = false;
    public bool IsSelected { get; set; } = false;
    public bool IsLocked = false;
    public bool FileOrFolderNotFound { get;private set; }=false;

    public enum extension
    {
        others,
        folder,
        wav,
        txt,
        zip,
    }

    public FileFoldersGrantData(string userPath ,string name)
    {
        UserPath = userPath;
        Name = name;
    }



}
