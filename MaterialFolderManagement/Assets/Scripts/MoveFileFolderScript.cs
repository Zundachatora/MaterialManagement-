using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoveFileFolderScript : ConditionalSearchScript
{
    public enum ReasonsForFailureEnum
    {
        FileFolderDoesNotExist,
        InvalidUserPath,
        InvalidDestinationFullPath,
        NameIsEmpty,
        AliasIsEmpty,
        DuplicateNameAtDestination,
        SubfileFoldersHaveDuplicateNames,
        NewNameIsInvalid,
        Success
    }
    /// <summary>
    /// セットのGrantDataテキストは既存のファイルフォルダがあると生成されない。
    /// </summary>
    /// <param name="grantData"></param>
    /// <param name="destinationFullPath"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public (bool move, ReasonsForFailureEnum reasonsForFailureEnum, string nameSuggestion) MoveFileFolder(FileFoldersGrantData grantData,string destinationFullPath,DateTime moveStartTime, MoveFileFolderOption option = null)
    {

        //移動させる元データのパスの確認
        var paths = PathConversion(grantData.UserPath);
        if (paths == null) return (false, ReasonsForFailureEnum.InvalidUserPath, null);

        //移動先のフォルダの確認
        if(!IsSafePath(destinationFullPath,false))return (false, ReasonsForFailureEnum.InvalidDestinationFullPath, null);
        if (!Directory.Exists(destinationFullPath)) return (false, ReasonsForFailureEnum.InvalidDestinationFullPath, null);

        //nullの場合、デフォルトのオプションを使用する
        if (option == null) option = new MoveFileFolderOption();

        //移動先のフルパスを作成。重複チェックも兼ねる。
        string moveFullPath = destinationFullPath.ToString();
        if (string.IsNullOrEmpty(option.NewFileFolderName))
        {
            if (option.Alias)
            {//フルパス\\別名
                if (string.IsNullOrEmpty(grantData.Alias)) return (false, ReasonsForFailureEnum.AliasIsEmpty, null);

                moveFullPath = Path.Combine(moveFullPath, grantData.Alias);

                if (!option.FileFolderOverwrite)
                {//重複チェック
                    //移動先に既に同じ名前があれば代理の名前を提案して終了する。
                    var duplicate = CheckForDuplicateName(moveFullPath);
                    if (duplicate.duplicates) return (false, ReasonsForFailureEnum.DuplicateNameAtDestination, duplicate.nameSuggestion);
                }
            }
            else
            {//フルパス\\名前
                if (string.IsNullOrEmpty(grantData.Name)) return (false, ReasonsForFailureEnum.NameIsEmpty, null);
                    
                moveFullPath = Path.Combine(moveFullPath, grantData.Name);
                
                if (!option.FileFolderOverwrite)
                {//重複チェック
                    //移動先に既に同じ名前があれば代理の名前を提案して終了する。
                    var duplicate = CheckForDuplicateName(moveFullPath);
                    if (duplicate.duplicates) return (false, ReasonsForFailureEnum.DuplicateNameAtDestination, duplicate.nameSuggestion);
                }
            } 
        }
        else
        {//フルパス\\newName

            if (!IsSafePath(option.NewFileFolderName, true)) return (false, ReasonsForFailureEnum.NewNameIsInvalid, null);

            moveFullPath = Path.Combine(moveFullPath, option.NewFileFolderName);

            if (!option.FileFolderOverwrite)
            {//重複チェック
                //移動先に既に同じ名前があれば代理の名前を提案して終了する。
                var duplicate = CheckForDuplicateName(moveFullPath);
                if (duplicate.duplicates) return (false, ReasonsForFailureEnum.DuplicateNameAtDestination, duplicate.nameSuggestion);
            }
        }

        if (grantData.Extension == FileFoldersGrantData.extension.folder)
        {
            return MoveFolder();
        }
        else
        {
            return MoveFile();
        }

        (bool duplicates, string nameSuggestion) CheckForDuplicateName(string checkFullPath)
        {
            string extension = Path.GetExtension(checkFullPath);
            string noExtensionFullPath = (string.IsNullOrEmpty(extension)?checkFullPath:checkFullPath.Substring(0,checkFullPath.LastIndexOf(extension)));
            string nameSuggestion = checkFullPath;

            //最初から重複しない場合
            if(!Directory.Exists(nameSuggestion) && !File.Exists(nameSuggestion)) return (false, nameSuggestion);

            //限界まで重複しない名前を探して返す。
            int addNum = 1;
            while (true)
            {
                if (addNum == int.MaxValue) return (true, noExtensionFullPath + "(" + addNum + ")+" + extension);

                if (Directory.Exists(nameSuggestion) || File.Exists(nameSuggestion))
                {
                    nameSuggestion = noExtensionFullPath + "(" + addNum + ")" + extension;

                    addNum++;
                }
                else
                {
                    return (true, Path.GetFileName(nameSuggestion));
                }
            }
        }

        (bool move, ReasonsForFailureEnum reasonsForFailureEnum, string nameSuggestion) MoveFolder()
        {
            DirectoryInfo moveFolderInfo = new DirectoryInfo(paths.Value.fullPath);
            
            if (!moveFolderInfo.Exists) return (false, ReasonsForFailureEnum.FileFolderDoesNotExist, null);

            if(FolderCopy(moveFolderInfo.FullName,moveFullPath, option.FileFolderOverwrite))
            {
                return (false, ReasonsForFailureEnum.SubfileFoldersHaveDuplicateNames, null);
            }
            
            if (!option.Copy) moveFolderInfo.Delete();

            return (true, ReasonsForFailureEnum.Success, null);

            //上書き(overwrite)出来なければ移動先に無いファイルだけコピーする。※フォルダは上書きする意味がないので使いまわす。
            //overwrite=falseで上書き出来ないファイルがあればfalseを返す。
            bool FolderCopy(string sourceFullPath, string destinationFullPath, bool overwrite)
            {//戻り値 true全てのファイルをコピー。false既にあるファイルを除いて全てのファイルをコピー。
                bool skipOnDuplicates = false;

                if (!Directory.Exists(destinationFullPath)) Directory.CreateDirectory(destinationFullPath);

                //コピー予定の全てのフォルダを探す。
                LinkedList<(string source, string destination)> folders = new LinkedList<(string source, string destination)>();
                LinkedList<(string source, string destination)> folders_SubChecked = new LinkedList<(string source, string destination)>();
                folders.AddLast((sourceFullPath, destinationFullPath));
                while (true)
                {
                    if (folders.Count == 0) break;

                    (string source, string destination) currentFolder = folders.Last.Value;
                    folders.RemoveLast();
                    foreach (var folderFullPath in Directory.GetDirectories(currentFolder.source))
                    {
                        //コピーの無限ループ防止
                        if (Directory.GetCreationTime(folderFullPath) >= moveStartTime) continue;

                        folders.AddLast((folderFullPath,
                            Path.Combine(currentFolder.destination, Path.GetFileName(folderFullPath)))
                            );
                    }
                    folders_SubChecked.AddLast(currentFolder);
                }

                foreach (var fullPaths in folders_SubChecked)
                {
                    if (!Directory.Exists(fullPaths.destination)) Directory.CreateDirectory(fullPaths.destination);
                    
                    SameLevelFileAllCopy(fullPaths.source, fullPaths.destination);
                }

                return skipOnDuplicates;

                void SameLevelFileAllCopy(string parentFolderFullPath_Source, string parentFolderFullPath_Destination)
                {
                    foreach (var fileFullPath in Directory.GetFiles(parentFolderFullPath_Source))
                    {
                        //コピーの無限ループ防止
                        if (File.GetCreationTime(fileFullPath) >= moveStartTime) continue;

                        string destinationFullPath = Path.Combine(parentFolderFullPath_Destination, Path.GetFileName(fileFullPath));

                        if (File.Exists(fileFullPath))
                        {
                            if (!overwrite && File.Exists(destinationFullPath))
                            {
                                skipOnDuplicates = true;
                                continue;
                            }

                            File.Copy(fileFullPath, destinationFullPath, true);
                        }

                    }
                }
                
            }
        }

        (bool move, ReasonsForFailureEnum reasonsForFailureEnum, string nameSuggestion) MoveFile()
        {
            FileInfo moveFileInfo = new FileInfo(paths.Value.fullPath);
            if (!moveFileInfo.Exists) return (false, ReasonsForFailureEnum.FileFolderDoesNotExist, null);
            
            moveFileInfo.CopyTo(moveFullPath,true);

            //テキストの追加
            string addTextFullPath = Path.ChangeExtension(moveFullPath, ".txt");
            if (!File.Exists(addTextFullPath) && !Directory.Exists(addTextFullPath))
            {
                string addTextString = "";
                if (option.AddNameText) addTextString += grantData.Name + "\n";
                if (option.AddAliasText) addTextString += grantData.Alias + "\n";
                if (option.AddMemorandumText) addTextString += grantData.Memorandum + "\n";
                if (option.AddHashtagText)
                {
                    addTextString += grantData.Hashtag.Length + "\n";
                    foreach (var tag in grantData.Hashtag)
                    {
                        addTextString += tag + "\n";
                    }
                }

                if (!string.IsNullOrEmpty(addTextString))
                {
                    using (var writer = new StreamWriter(addTextFullPath))
                    {
                        writer.Write(addTextString);
                    }
                }
            }

            if (!option.Copy)moveFileInfo.Delete();

            return (true, ReasonsForFailureEnum.Success, null);
        }
    }

    public class MoveFileFolderOption
    {
        public bool Copy = false;
        /// <summary>NewFileFolderNameがある場合はそちらが優先される</summary>
        public bool Alias = false;
        public bool FileFolderOverwrite = false;
        public string NewFileFolderName = null;
        /// <summary>移動させたファイルフォルダと同じ場所に同名(拡張子は.txt)のテキストファイルを作成する。既にファイルがある場合は作成しない(書き込みもしない)</summary>
        public bool AddNameText = false;
        /// <summary>移動させたファイルフォルダと同じ場所に同名(拡張子は.txt)のテキストファイルを作成する。既にファイルがある場合は作成しない(書き込みもしない)</summary>
        public bool AddAliasText = false;
        /// <summary>移動させたファイルフォルダと同じ場所に同名(拡張子は.txt)のテキストファイルを作成する。既にファイルがある場合は作成しない(書き込みもしない)</summary>
        public bool AddMemorandumText = false;
        /// <summary>移動させたファイルフォルダと同じ場所に同名(拡張子は.txt)のテキストファイルを作成する。既にファイルがある場合は作成しない(書き込みもしない)</summary>
        public bool AddHashtagText = false;

    }
}
