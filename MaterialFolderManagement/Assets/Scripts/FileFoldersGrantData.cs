using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FileFoldersGrantData
{

    /// <summary>���[�U�[�t�H���_���p�X��GrantData�̃p�X�ł͂Ȃ�</summary>
    public string UserPath;
    /// <summary>�t�@�C���t�H���_����GrantData�̂ł͂Ȃ�</summary>
    public string Name;
    public string Alias= "";
    public string Memorandum= "";
    /// <summary>�ϐ����ł͐擪��#�͕t���Ȃ��ŊǗ�����</summary>
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
