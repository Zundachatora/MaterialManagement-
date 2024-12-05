using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileFolderSavingScript : FileFolderPathManagement
{
    
    public bool Save(FileFoldersGrantData[] saveData)
    {
        if (saveData == null) return false;

        foreach (var sData in saveData)
        {
            if (!sData.IsUpdated) continue;

            var paths = PathConversion(sData.UserPath);
            if (paths == null) continue;

            //最上階のユーザーフォルダには付与データ(GrantData)を付けない。
            if (paths.Value.userPath.Equals(RootPaths.userTopFolderPath)) continue;

            string writeFullPath = paths.Value.grantDataFullPath_Parentheses + ".json";

            //ファイル置き場までのフォルダがなければ作成する。
            CreateFolderIfNotExists(Path.GetDirectoryName(writeFullPath));

            string json = JsonUtility.ToJson(sData);
            using (var writer = new StreamWriter(writeFullPath))
            {
                writer.Write(json);
            }
        }

        return true;
    }

    public bool Save(FileFoldersGrantData saveData)
    {
        if(saveData == null)return false;

        Save(new FileFoldersGrantData[] { saveData });
    
        return true;
    }

    /// <summary>
    /// フォルダーが存在しない場合は作成。
    /// </summary>
    /// <param name="path"></param>
    protected void CreateFolderIfNotExists(string fullPath)
    {
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }
    override protected void Awake()
    {
        base.Awake();

        //最低限のフォルダがなければ作成。
        CreateFolderIfNotExists(RootPaths.topFolderFullPath+"/"+RootPaths.userTopFolderPath);
        CreateFolderIfNotExists(RootPaths.topFolderFullPath + "/" + RootPaths.userGrantDataTopFolderPath);
    }
}
