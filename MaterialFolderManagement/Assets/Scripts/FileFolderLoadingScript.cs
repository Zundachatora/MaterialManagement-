using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;



public class FileFolderLoadingScript : GrantDataSortsScript
{
    protected string _latestGetUserPath { get; private set; } = null;

    /// <returns>�w�肳�ꂽ�t�H���_�̔z���̃t�@�C���t�H���_��GrantData��Ԃ�</returns>
    protected FileFoldersGrantData[] GetGrantDatas(string folderUserPath, out bool validPath)
    {

        FileFoldersGrantData[] returnGrantDatas;

        returnGrantDatas = GetGrantDatas_FolderOnly(folderUserPath,out validPath);

        if(!validPath)return new FileFoldersGrantData[0];

        returnGrantDatas = returnGrantDatas.Concat(GetGrantDatas_FileOnly(folderUserPath, out validPath)).ToArray();

        return returnGrantDatas;

    }

    protected FileFoldersGrantData[] GetGrantDatas_FolderOnly(string folderUserPath, out bool validPath)
    {
        validPath = false;

        //�p�X�ɂȂ��Ă��Ȃ�������͂͂����B
        var paths = PathConversion(folderUserPath);
        if (paths == null) return new FileFoldersGrantData[0];

        DirectoryInfo parentFolder = new DirectoryInfo(paths.Value.fullPath);
        DirectoryInfo grantDataParentFolder = new DirectoryInfo(paths.Value.grantDataFullPath);

        //�t�H���_�����݂��Ȃ���Ύ擾�����ۂ���
        if (!parentFolder.Exists) return new FileFoldersGrantData[0];

        FileFoldersGrantData[] grantDatas = LoadedGrantDatas(parentFolder, grantDataParentFolder);

        _latestGetUserPath = paths.Value.userPath;
        validPath = true;

        return grantDatas;

        FileFoldersGrantData[] LoadedGrantDatas(DirectoryInfo folderInfo, DirectoryInfo grantDataFolderInfo)
        {
            List<FileFoldersGrantData> folderGrantDatas = new List<FileFoldersGrantData>();

            //GrantData�̍쐬(�t�H���_)�B
            foreach (var folder in folderInfo.GetDirectories())
            {
                FileFoldersGrantData fileFoldersGrantData = new FileFoldersGrantData(paths.Value.userPath + "\\" + folder.Name, folder.Name);

                //�g���q�̐ݒ�
                fileFoldersGrantData.Extension = FileFoldersGrantData.extension.folder;

                folderGrantDatas.Add(fileFoldersGrantData);
            }

            ReflectingExistingGrantData(folderGrantDatas);

            return folderGrantDatas.ToArray();

            void ReflectingExistingGrantData(List<FileFoldersGrantData> grantDatas)
            {
                //������GrantData������Δ��f������B
                foreach (var fGD in grantDatas)
                {
                    string loadPath = paths.Value.grantDataFullPath + "\\(" + fGD.Name + ").json";

                    if (!File.Exists(loadPath)) continue;

                    string json;
                    using (var reader = new StreamReader(loadPath))
                    {
                        json = reader.ReadToEnd();
                    }

                    //�ǂݍ��݂������B
                    FileFoldersGrantData fFGDreader = JsonUtility.FromJson<FileFoldersGrantData>(json);
                    fGD.Alias = fFGDreader.Alias;
                    fGD.Memorandum = fFGDreader.Memorandum;
                    fGD.Hashtag = fFGDreader.Hashtag;
                    fGD.IsLocked = fFGDreader.IsLocked;

                }
            }
        }
    }

    protected FileFoldersGrantData[] GetGrantDatas_FileOnly(string folderUserPath, out bool validPath)
    {
        validPath = false;

        //�p�X�ɂȂ��Ă��Ȃ�������͂͂����B
        var paths = PathConversion(folderUserPath);
        if (paths.Value.userPath == null) return new FileFoldersGrantData[0];

        DirectoryInfo parentFolder = new DirectoryInfo(paths.Value.fullPath);
        DirectoryInfo grantDataParentFolder = new DirectoryInfo(paths.Value.grantDataFullPath);

        //�t�H���_�����݂��Ȃ���Ύ擾�����ۂ���
        if (!parentFolder.Exists) return new FileFoldersGrantData[0];

        FileFoldersGrantData[] grantDatas = LoadedGrantDatas(parentFolder, grantDataParentFolder);

        _latestGetUserPath = paths.Value.userPath;
        validPath = true;

        return grantDatas;

        FileFoldersGrantData[] LoadedGrantDatas(DirectoryInfo folderInfo, DirectoryInfo grantDataFolderInfo)
        {
            List<FileFoldersGrantData> fileGrantDatas = new List<FileFoldersGrantData>();

            //GrantData�̍쐬(�t�@�C��)�B
            foreach (var filse in folderInfo.GetFiles())
            {
                FileFoldersGrantData fileFoldersGrantData = new FileFoldersGrantData(paths.Value.userPath + "\\" + filse.Name, filse.Name);

                //�g���q�̐ݒ�
                string extension = filse.Name.Substring(filse.Name.LastIndexOf(".") + 1);
                fileFoldersGrantData.Extension = FileFoldersGrantData.extension.others;//�f�t�H���g
                foreach (var eName in Enum.GetValues(typeof(FileFoldersGrantData.extension)))
                {
                    if (extension.Equals(eName.ToString()))
                    {
                        fileFoldersGrantData.Extension = (FileFoldersGrantData.extension)Enum.Parse(typeof(FileFoldersGrantData.extension), eName.ToString());
                        break;
                    }
                }

                fileGrantDatas.Add(fileFoldersGrantData);
            }

            ReflectingExistingGrantData(fileGrantDatas);

            return fileGrantDatas.ToArray();

            void ReflectingExistingGrantData(List<FileFoldersGrantData> grantDatas)
            {
                //������GrantData������Δ��f������B
                foreach (var fGD in grantDatas)
                {
                    string loadPath = paths.Value.grantDataFullPath + "\\(" + fGD.Name + ").json";

                    if (!File.Exists(loadPath)) continue;

                    string json;
                    using (var reader = new StreamReader(loadPath))
                    {
                        json = reader.ReadToEnd();
                    }

                    //�ǂݍ��݂������B
                    FileFoldersGrantData fFGDreader = JsonUtility.FromJson<FileFoldersGrantData>(json);
                    fGD.Alias = fFGDreader.Alias;
                    fGD.Memorandum = fFGDreader.Memorandum;
                    fGD.Hashtag = fFGDreader.Hashtag;
                    fGD.IsLocked = fFGDreader.IsLocked;
                }
            }
        }
    }
}
