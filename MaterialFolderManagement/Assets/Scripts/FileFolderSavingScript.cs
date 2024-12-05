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

            //�ŏ�K�̃��[�U�[�t�H���_�ɂ͕t�^�f�[�^(GrantData)��t���Ȃ��B
            if (paths.Value.userPath.Equals(RootPaths.userTopFolderPath)) continue;

            string writeFullPath = paths.Value.grantDataFullPath_Parentheses + ".json";

            //�t�@�C���u����܂ł̃t�H���_���Ȃ���΍쐬����B
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
    /// �t�H���_�[�����݂��Ȃ��ꍇ�͍쐬�B
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

        //�Œ���̃t�H���_���Ȃ���΍쐬�B
        CreateFolderIfNotExists(RootPaths.topFolderFullPath+"/"+RootPaths.userTopFolderPath);
        CreateFolderIfNotExists(RootPaths.topFolderFullPath + "/" + RootPaths.userGrantDataTopFolderPath);
    }
}
