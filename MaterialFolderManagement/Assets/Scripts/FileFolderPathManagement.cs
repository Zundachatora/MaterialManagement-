using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileFolderPathManagement : MonoBehaviour
{
    
    //GrantDataはGDと略す
    //「ユーザーファルダ内パス」とはフォルダ_userTopFolderNameを最上階として扱った際のパス
    //付与データ(GrantData)は().txtで管理する。例 aは(a).txt、a.txtは(a.txt).txt。

    public (string topFolderFullPath, string userTopFolderPath, string userGrantDataTopFolderPath) RootPaths { get; private set; }

    /// <summary>ユーザーフォルダ最上階の名前</summary>
    [SerializeField] private string _userTopFolderName = "ユーザーフォルダ";

    protected bool IsSafePath(string path,bool isFileName)
    {
        //参考とライセンス表記
        // LICENSE/SourceCode/IsSafePath
        //https://www.curict.com/item/0a/0a33f42.html

        if (string.IsNullOrEmpty(path)) return false;

        //無効な文字がないか
        char[] invalidChars = (isFileName) ? Path.GetInvalidFileNameChars() : Path.GetInvalidPathChars();
        if (path.IndexOfAny(invalidChars) >= 0)
        {
            return false;
        }

        if (System.Text.RegularExpressions.Regex.IsMatch(path
                                      , @"(^|\\|/)(CON|PRN|AUX|NUL|CLOCK\$|COM[0-9]|LPT[0-9])(\.|\\|/|$)"
                                      , System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {

            return false;
        }

        return true;
    }

    /// <summary>
    /// ユーザーフォルダ内パスをフルパス、GrantDataパス、GrantDataフルパスに変換する。
    /// </summary>
    /// <returns>無効なパスはnullを返す。</returns>
    protected (string userPath, string grantDataPath, string grantDataPath_Parentheses, string userPath_ExcludeBottom, string grantDataPath_ExcludeBottom, string fullPath, string grantDataFullPath, string grantDataFullPath_Parentheses, string pathBottomLayer)? PathConversion(string userPath)
    {
        if(IsSafePath(userPath, false))
        {
            (string userPath,
             string grantDataPath,
             string grantDataPath_Parentheses,
             string userPath_ExcludeBottom, 
             string grantDataPath_ExcludeBottom, 
             string fullPath, 
             string grantDataFullPath,
             string grantDataFullPath_Parentheses,
             string pathBottomLayer) returnPaths;

            if (userPath.Equals(RootPaths.userTopFolderPath))
            {//ユーザートップフォルダ

                // "ユーザーフォルダ"
                returnPaths.userPath = RootPaths.userTopFolderPath;
                // "ユーザーフォルダGrantData"
                returnPaths.grantDataPath = RootPaths.userGrantDataTopFolderPath;
                // "(ユーザーフォルダGrantData)"
                returnPaths.grantDataPath_Parentheses = "("+ RootPaths.userGrantDataTopFolderPath +")";
                // ""
                returnPaths.userPath_ExcludeBottom = "";
                // ""
                returnPaths.grantDataPath_ExcludeBottom = "";
                // "E:/.../ManagementFolderTop/ユーザーフォルダ"
                returnPaths.fullPath = RootPaths.topFolderFullPath + "\\" + RootPaths.userTopFolderPath;
                // "E:/.../ManagementFolderTop/ユーザーフォルダGrantData"
                returnPaths.grantDataFullPath = RootPaths.topFolderFullPath + "\\" + RootPaths.userGrantDataTopFolderPath;
                // "E:/.../ManagementFolderTop/(ユーザーフォルダGrantData)"
                returnPaths.grantDataFullPath_Parentheses = RootPaths.topFolderFullPath + "\\(" + RootPaths.userGrantDataTopFolderPath +")";
                // "ユーザーフォルダ"
                returnPaths.pathBottomLayer = RootPaths.userTopFolderPath;

                return returnPaths;
            }
            else if (userPath.StartsWith(RootPaths.userTopFolderPath))
            {//ユーザートップフォルダの下層

                // ファイル"ユーザーフォルダ/〇〇/〇〇.〇〇"

                // フォルダ"〇〇"
                // ファイル"〇〇"
                string userFolderAndLastTruncationPath = userPath.Substring(userPath.IndexOf("\\")+1);
                userFolderAndLastTruncationPath = (userFolderAndLastTruncationPath.IndexOf("\\") == -1) ? "" : userFolderAndLastTruncationPath.Substring(0,userFolderAndLastTruncationPath.LastIndexOf("\\")+1);
                // フォルダ"〇〇"
                // ファイル"〇〇.〇〇"
                string lastPath = userPath.Substring(userPath.LastIndexOf("\\") + 1);

                // フォルダ"ユーザーフォルダ/〇〇/〇〇"
                // ファイル"ユーザーフォルダ/〇〇/〇〇.〇〇"
                returnPaths.userPath = userPath;
                // フォルダ"ユーザーフォルダGrantData/〇〇/〇〇"
                // ファイル"ユーザーフォルダGrantData/〇〇/〇〇.〇〇"
                returnPaths.grantDataPath = RootPaths.userGrantDataTopFolderPath + "\\" +((String.IsNullOrEmpty(userFolderAndLastTruncationPath))? lastPath: (userFolderAndLastTruncationPath + lastPath));
                // フォルダ"ユーザーフォルダGrantData/〇〇/(〇〇)"
                // ファイル"ユーザーフォルダGrantData/〇〇/(〇〇.〇〇)"
                returnPaths.grantDataPath_Parentheses = RootPaths.userGrantDataTopFolderPath + "\\" + userFolderAndLastTruncationPath + "(" + lastPath + ")";
                // フォルダ"ユーザーフォルダ/〇〇"
                // ファイル"ユーザーフォルダ/〇〇"
                returnPaths.userPath_ExcludeBottom = userPath.Substring(0, userPath.LastIndexOf("\\"));
                // フォルダ"ユーザーフォルダGrantData/〇〇"
                // ファイル"ユーザーフォルダGrantData/〇〇"
                returnPaths.grantDataPath_ExcludeBottom = returnPaths.grantDataPath.Substring(0, userPath.LastIndexOf("\\"));
                // フォルダ"E:/.../ManagementFolderTop/ユーザーフォルダ/〇〇/〇〇"
                // ファイル"E:/.../ManagementFolderTop/ユーザーフォルダ/〇〇/〇〇.〇〇"
                returnPaths.fullPath = RootPaths.topFolderFullPath + "\\" + userPath;
                // フォルダ"E:/.../ManagementFolderTop/ユーザーフォルダGrantData/〇〇/〇〇"
                // ファイル"E:/.../ManagementFolderTop/ユーザーフォルダGrantData/〇〇/〇〇.〇〇"
                returnPaths.grantDataFullPath = RootPaths.topFolderFullPath + "\\" + returnPaths.grantDataPath;
                // フォルダ"E:/.../ManagementFolderTop/ユーザーフォルダGrantData/〇〇/(〇〇)"
                // ファイル"E:/.../ManagementFolderTop/ユーザーフォルダGrantData/〇〇/(〇〇.〇〇)"
                returnPaths.grantDataFullPath_Parentheses = RootPaths.topFolderFullPath + "\\" + returnPaths.grantDataPath_Parentheses;
                // フォルダ"〇〇"
                // ファイル"〇〇.〇〇"
                returnPaths.pathBottomLayer = userPath.Substring(userPath.LastIndexOf("\\") + 1);

                return returnPaths;
            }
            else
            {//ユーザーフォルダ外
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// base.Awake();で呼び出してね。
    /// </summary>
    protected virtual void Awake()
    {
        //フォルダ名が空白とかだと問題なので
        if (!IsSafePath(_userTopFolderName,true)) _userTopFolderName = "ユーザーフォルダ";
        
        (string topFolderFullPath,
         string userTopFolderPath,
         string userGrantDataTopFolderPath) rootPaths;

        //スクリプトで扱う最上階フォルダのパス(ユーザーフォルダの一つ上)
        //「/Assets」の文字列を消して適切なパスを代入する
        rootPaths.topFolderFullPath = Application.dataPath;
        rootPaths.topFolderFullPath = rootPaths.topFolderFullPath.Remove(rootPaths.topFolderFullPath.Length - 7, 7);
        rootPaths.topFolderFullPath += "\\ManagementFolderTop";

        //ユーザーフォルダの最上階パス
        rootPaths.userTopFolderPath = _userTopFolderName;

        //ユーザーファイル付与データの最上階パス
        rootPaths.userGrantDataTopFolderPath = rootPaths.userTopFolderPath + "GrantData";

        RootPaths = rootPaths;

    }

}
