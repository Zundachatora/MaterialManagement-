using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;



public class FileFolderLoadingScript : GrantDataSortsScript
{
    protected string _latestGetUserPath { get; private set; } = null;

    /// <returns>指定されたフォルダの配下のファイルフォルダのGrantDataを返す</returns>
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

        //パスになっていない文字列ははじく。
        var paths = PathConversion(folderUserPath);
        if (paths == null) return new FileFoldersGrantData[0];

        DirectoryInfo parentFolder = new DirectoryInfo(paths.Value.fullPath);
        DirectoryInfo grantDataParentFolder = new DirectoryInfo(paths.Value.grantDataFullPath);

        //フォルダが存在しなければ取得を拒否する
        if (!parentFolder.Exists) return new FileFoldersGrantData[0];

        FileFoldersGrantData[] grantDatas = LoadedGrantDatas(parentFolder, grantDataParentFolder);

        _latestGetUserPath = paths.Value.userPath;
        validPath = true;

        return grantDatas;

        FileFoldersGrantData[] LoadedGrantDatas(DirectoryInfo folderInfo, DirectoryInfo grantDataFolderInfo)
        {
            List<FileFoldersGrantData> folderGrantDatas = new List<FileFoldersGrantData>();

            //GrantDataの作成(フォルダ)。
            foreach (var folder in folderInfo.GetDirectories())
            {
                FileFoldersGrantData fileFoldersGrantData = new FileFoldersGrantData(paths.Value.userPath + "\\" + folder.Name, folder.Name);

                //拡張子の設定
                fileFoldersGrantData.Extension = FileFoldersGrantData.extension.folder;

                folderGrantDatas.Add(fileFoldersGrantData);
            }

            ReflectingExistingGrantData(folderGrantDatas);

            return folderGrantDatas.ToArray();

            void ReflectingExistingGrantData(List<FileFoldersGrantData> grantDatas)
            {
                //既存のGrantDataがあれば反映させる。
                foreach (var fGD in grantDatas)
                {
                    string loadPath = paths.Value.grantDataFullPath + "\\(" + fGD.Name + ").json";

                    if (!File.Exists(loadPath)) continue;

                    string json;
                    using (var reader = new StreamReader(loadPath))
                    {
                        json = reader.ReadToEnd();
                    }

                    //読み込みたい情報。
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

        //パスになっていない文字列ははじく。
        var paths = PathConversion(folderUserPath);
        if (paths.Value.userPath == null) return new FileFoldersGrantData[0];

        DirectoryInfo parentFolder = new DirectoryInfo(paths.Value.fullPath);
        DirectoryInfo grantDataParentFolder = new DirectoryInfo(paths.Value.grantDataFullPath);

        //フォルダが存在しなければ取得を拒否する
        if (!parentFolder.Exists) return new FileFoldersGrantData[0];

        FileFoldersGrantData[] grantDatas = LoadedGrantDatas(parentFolder, grantDataParentFolder);

        _latestGetUserPath = paths.Value.userPath;
        validPath = true;

        return grantDatas;

        FileFoldersGrantData[] LoadedGrantDatas(DirectoryInfo folderInfo, DirectoryInfo grantDataFolderInfo)
        {
            List<FileFoldersGrantData> fileGrantDatas = new List<FileFoldersGrantData>();

            //GrantDataの作成(ファイル)。
            foreach (var filse in folderInfo.GetFiles())
            {
                FileFoldersGrantData fileFoldersGrantData = new FileFoldersGrantData(paths.Value.userPath + "\\" + filse.Name, filse.Name);

                //拡張子の設定
                string extension = filse.Name.Substring(filse.Name.LastIndexOf(".") + 1);
                fileFoldersGrantData.Extension = FileFoldersGrantData.extension.others;//デフォルト
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
                //既存のGrantDataがあれば反映させる。
                foreach (var fGD in grantDatas)
                {
                    string loadPath = paths.Value.grantDataFullPath + "\\(" + fGD.Name + ").json";

                    if (!File.Exists(loadPath)) continue;

                    string json;
                    using (var reader = new StreamReader(loadPath))
                    {
                        json = reader.ReadToEnd();
                    }

                    //読み込みたい情報。
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
