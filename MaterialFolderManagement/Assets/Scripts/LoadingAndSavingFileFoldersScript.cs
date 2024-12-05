using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;




public class LoadingAndSavingFileFoldersScript : MoveFileFolderScript
{
    public string SelectFolderUserPath { get; private set; } = null;
    /// <summary>SelectFolder関数で選ばれたフォルダ内のファイルフォルダ達のGrantData</summary>
    public FileFoldersGrantData[] SelectFileFolderGrantDatas { get; private set; } = new FileFoldersGrantData[0];
    
    public event OnFolderSelectedHandle OnFolderSelected = delegate { };

    public delegate void OnFolderSelectedHandle();

    /// <summary>
    /// メモリ上のGrantDataにも反映される。
    /// </summary>
    /// <param name="grantData"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    public bool FileFolderRenaming(ref FileFoldersGrantData grantData, string newName)
    {
        //名前の変更(ファイルフォルダの場所の移動)
        //ファイルフォルダそのものとGrantDataを両方移動させる。※メモリ上のGrantDataの名前とusePathも変える
        //フォルダ本体が存在しなければ名前は変えない(変えられない)


        if (!IsSafePath(newName, true))return false;

        var paths = PathConversion(grantData.UserPath);
        var newPaths = PathConversion(paths.Value.userPath_ExcludeBottom + "\\" + newName);

        //パスが無効な名前
        if ((paths==null) || (newPaths==null))return false;

        if (grantData.Extension == FileFoldersGrantData.extension.folder)
        {
            DirectoryInfo folder = new DirectoryInfo(paths.Value.fullPath);
            FileInfo folderGD = new FileInfo(paths.Value.grantDataFullPath_Parentheses);

            if (!folder.Exists) return false;

            //移動先に先客がいれば移動しない(名前を変更しない)
            if (Directory.Exists(newPaths.Value.fullPath))return false;
            if (File.Exists(newPaths.Value.fullPath)) return false;

            folder.MoveTo(newPaths.Value.fullPath);

            //既存のGrantDataが存在する場合はそれも移動する
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

            //移動先に先客がいれば移動しない(名前を変更しない)
            if (Directory.Exists(newPaths.Value.fullPath)) return false;
            if (File.Exists(newPaths.Value.fullPath)) return false;

            fileInfo.MoveTo(newPaths.Value.fullPath);

            //既存のGrantDataが存在する場合はそれも移動する
            if (fileGD.Exists)
            {
                fileGD.CopyTo(newPaths.Value.grantDataFullPath, true);
                fileGD.Delete();
            }
            
        }

        //メモリ上のGrantDataも変更
        grantData.UserPath = newPaths.Value.userPath;
        grantData.Name = newPaths.Value.pathBottomLayer; 

        return true;
    }

    /// <summary>
    /// 指定されたフォルダが存在すれば開く。
    /// ユーザーファルダ内パス専用。
    /// </summary>
    /// <returns>フォルダーの存在</returns>
    public bool SelectFolder(string userPath, SearchConditionData searchConditionData = null)
    {
        bool get;
        FileFoldersGrantData[] getData = GetGrantDatas(userPath,out get);

        if (!get)return false;

        //条件検索であれば絞り込みをする
        if(searchConditionData == null)
        {
            SelectFileFolderGrantDatas = SortGrantDataWindowsStyle(getData);
        }
        else
        {
            //Debug.Log("条件 ExcludeMatchingConditions = "+ searchConditionData.ExcludeMatchingConditions+ ";\r\nAscendingOrder = "+ searchConditionData .AscendingOrder+ ";\r\n\r\nSearchString = "+ searchConditionData .SearchString+ ";\r\nIncludeName = "+ searchConditionData .IncludeName+ ";\r\nIncludeFile = "+ searchConditionData .IncludeFile+ ";\r\nIncludeFolder = "+ searchConditionData .IncludeFolder+ ";\r\nIncludeAlias = "+ searchConditionData .IncludeAlias+ ";\r\nIncludeMemorandum = "+ searchConditionData .IncludeMemorandum+ ";\r\nIncludeHashtag = "+ searchConditionData.IncludeHashtag.Length+ ";");
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

