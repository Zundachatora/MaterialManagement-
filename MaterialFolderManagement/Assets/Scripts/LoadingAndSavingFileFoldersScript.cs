using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;




public class LoadingAndSavingFileFoldersScript : MoveFileFolderScript
{
    public string SelectFolderUserPath { get; private set; } = null;
    /// <summary>SelectFolder�֐��őI�΂ꂽ�t�H���_���̃t�@�C���t�H���_�B��GrantData</summary>
    public FileFoldersGrantData[] SelectFileFolderGrantDatas { get; private set; } = new FileFoldersGrantData[0];
    
    public event OnFolderSelectedHandle OnFolderSelected = delegate { };

    public delegate void OnFolderSelectedHandle();

    /// <summary>
    /// ���������GrantData�ɂ����f�����B
    /// </summary>
    /// <param name="grantData"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    public bool FileFolderRenaming(ref FileFoldersGrantData grantData, string newName)
    {
        //���O�̕ύX(�t�@�C���t�H���_�̏ꏊ�̈ړ�)
        //�t�@�C���t�H���_���̂��̂�GrantData�𗼕��ړ�������B�����������GrantData�̖��O��usePath���ς���
        //�t�H���_�{�̂����݂��Ȃ���Ζ��O�͕ς��Ȃ�(�ς����Ȃ�)


        if (!IsSafePath(newName, true))return false;

        var paths = PathConversion(grantData.UserPath);
        var newPaths = PathConversion(paths.Value.userPath_ExcludeBottom + "\\" + newName);

        //�p�X�������Ȗ��O
        if ((paths==null) || (newPaths==null))return false;

        if (grantData.Extension == FileFoldersGrantData.extension.folder)
        {
            DirectoryInfo folder = new DirectoryInfo(paths.Value.fullPath);
            FileInfo folderGD = new FileInfo(paths.Value.grantDataFullPath_Parentheses);

            if (!folder.Exists) return false;

            //�ړ���ɐ�q������Έړ����Ȃ�(���O��ύX���Ȃ�)
            if (Directory.Exists(newPaths.Value.fullPath))return false;
            if (File.Exists(newPaths.Value.fullPath)) return false;

            folder.MoveTo(newPaths.Value.fullPath);

            //������GrantData�����݂���ꍇ�͂�����ړ�����
            if (folderGD.Exists)
            {
                folderGD.CopyTo(newPaths.Value.grantDataFullPath_Parentheses, true);
                folderGD.Delete();
            }

        }
        else
        {
            FileInfo fileInfo = new FileInfo(paths.Value.fullPath);
            FileInfo fileGD = new FileInfo(paths.Value.grantDataFullPath);

            if (!fileInfo.Exists) return false;

            //�ړ���ɐ�q������Έړ����Ȃ�(���O��ύX���Ȃ�)
            if (Directory.Exists(newPaths.Value.fullPath)) return false;
            if (File.Exists(newPaths.Value.fullPath)) return false;

            fileInfo.MoveTo(newPaths.Value.fullPath);

            //������GrantData�����݂���ꍇ�͂�����ړ�����
            if (fileGD.Exists)
            {
                fileGD.CopyTo(newPaths.Value.grantDataFullPath, true);
                fileGD.Delete();
            }
            
        }

        //���������GrantData���ύX
        grantData.UserPath = newPaths.Value.userPath;
        grantData.Name = newPaths.Value.pathBottomLayer; 

        return true;
    }

    /// <summary>
    /// �w�肳�ꂽ�t�H���_�����݂���ΊJ���B
    /// ���[�U�[�t�@���_���p�X��p�B
    /// </summary>
    /// <returns>�t�H���_�[�̑���</returns>
    public bool SelectFolder(string userPath, SearchConditionData searchConditionData = null)
    {
        bool get;
        FileFoldersGrantData[] getData = GetGrantDatas(userPath,out get);

        if (!get)return false;

        //���������ł���΍i�荞�݂�����
        if(searchConditionData == null)
        {
            SelectFileFolderGrantDatas = SortGrantDataWindowsStyle(getData);
        }
        else
        {
            //Debug.Log("���� ExcludeMatchingConditions = "+ searchConditionData.ExcludeMatchingConditions+ ";\r\nAscendingOrder = "+ searchConditionData .AscendingOrder+ ";\r\n\r\nSearchString = "+ searchConditionData .SearchString+ ";\r\nIncludeName = "+ searchConditionData .IncludeName+ ";\r\nIncludeFile = "+ searchConditionData .IncludeFile+ ";\r\nIncludeFolder = "+ searchConditionData .IncludeFolder+ ";\r\nIncludeAlias = "+ searchConditionData .IncludeAlias+ ";\r\nIncludeMemorandum = "+ searchConditionData .IncludeMemorandum+ ";\r\nIncludeHashtag = "+ searchConditionData.IncludeHashtag.Length+ ";");
            SelectFileFolderGrantDatas = SearchData(getData, searchConditionData);
        }


        SelectFolderUserPath = _latestGetUserPath;

        OnFolderSelected();

        return true;
    }



    override protected void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        SelectFolder(RootPaths.userTopFolderPath);
    }

}

