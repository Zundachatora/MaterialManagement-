using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileFolderPathManagement : MonoBehaviour
{
    
    //GrantData��GD�Ɨ���
    //�u���[�U�[�t�@���_���p�X�v�Ƃ̓t�H���__userTopFolderName���ŏ�K�Ƃ��Ĉ������ۂ̃p�X
    //�t�^�f�[�^(GrantData)��().txt�ŊǗ�����B�� a��(a).txt�Aa.txt��(a.txt).txt�B

    public (string topFolderFullPath, string userTopFolderPath, string userGrantDataTopFolderPath) RootPaths { get; private set; }

    /// <summary>���[�U�[�t�H���_�ŏ�K�̖��O</summary>
    [SerializeField] private string _userTopFolderName = "���[�U�[�t�H���_";

    protected bool IsSafePath(string path,bool isFileName)
    {
        //�Q�l�ƃ��C�Z���X�\�L
        // LICENSE/SourceCode/IsSafePath
        //https://www.curict.com/item/0a/0a33f42.html

        if (string.IsNullOrEmpty(path)) return false;

        //�����ȕ������Ȃ���
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
    /// ���[�U�[�t�H���_���p�X���t���p�X�AGrantData�p�X�AGrantData�t���p�X�ɕϊ�����B
    /// </summary>
    /// <returns>�����ȃp�X��null��Ԃ��B</returns>
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
            {//���[�U�[�g�b�v�t�H���_

                // "���[�U�[�t�H���_"
                returnPaths.userPath = RootPaths.userTopFolderPath;
                // "���[�U�[�t�H���_GrantData"
                returnPaths.grantDataPath = RootPaths.userGrantDataTopFolderPath;
                // "(���[�U�[�t�H���_GrantData)"
                returnPaths.grantDataPath_Parentheses = "("+ RootPaths.userGrantDataTopFolderPath +")";
                // ""
                returnPaths.userPath_ExcludeBottom = "";
                // ""
                returnPaths.grantDataPath_ExcludeBottom = "";
                // "E:/.../ManagementFolderTop/���[�U�[�t�H���_"
                returnPaths.fullPath = RootPaths.topFolderFullPath + "\\" + RootPaths.userTopFolderPath;
                // "E:/.../ManagementFolderTop/���[�U�[�t�H���_GrantData"
                returnPaths.grantDataFullPath = RootPaths.topFolderFullPath + "\\" + RootPaths.userGrantDataTopFolderPath;
                // "E:/.../ManagementFolderTop/(���[�U�[�t�H���_GrantData)"
                returnPaths.grantDataFullPath_Parentheses = RootPaths.topFolderFullPath + "\\(" + RootPaths.userGrantDataTopFolderPath +")";
                // "���[�U�[�t�H���_"
                returnPaths.pathBottomLayer = RootPaths.userTopFolderPath;

                return returnPaths;
            }
            else if (userPath.StartsWith(RootPaths.userTopFolderPath))
            {//���[�U�[�g�b�v�t�H���_�̉��w

                // �t�@�C��"���[�U�[�t�H���_/�Z�Z/�Z�Z.�Z�Z"

                // �t�H���_"�Z�Z"
                // �t�@�C��"�Z�Z"
                string userFolderAndLastTruncationPath = userPath.Substring(userPath.IndexOf("\\")+1);
                userFolderAndLastTruncationPath = (userFolderAndLastTruncationPath.IndexOf("\\") == -1) ? "" : userFolderAndLastTruncationPath.Substring(0,userFolderAndLastTruncationPath.LastIndexOf("\\")+1);
                // �t�H���_"�Z�Z"
                // �t�@�C��"�Z�Z.�Z�Z"
                string lastPath = userPath.Substring(userPath.LastIndexOf("\\") + 1);

                // �t�H���_"���[�U�[�t�H���_/�Z�Z/�Z�Z"
                // �t�@�C��"���[�U�[�t�H���_/�Z�Z/�Z�Z.�Z�Z"
                returnPaths.userPath = userPath;
                // �t�H���_"���[�U�[�t�H���_GrantData/�Z�Z/�Z�Z"
                // �t�@�C��"���[�U�[�t�H���_GrantData/�Z�Z/�Z�Z.�Z�Z"
                returnPaths.grantDataPath = RootPaths.userGrantDataTopFolderPath + "\\" +((String.IsNullOrEmpty(userFolderAndLastTruncationPath))? lastPath: (userFolderAndLastTruncationPath + lastPath));
                // �t�H���_"���[�U�[�t�H���_GrantData/�Z�Z/(�Z�Z)"
                // �t�@�C��"���[�U�[�t�H���_GrantData/�Z�Z/(�Z�Z.�Z�Z)"
                returnPaths.grantDataPath_Parentheses = RootPaths.userGrantDataTopFolderPath + "\\" + userFolderAndLastTruncationPath + "(" + lastPath + ")";
                // �t�H���_"���[�U�[�t�H���_/�Z�Z"
                // �t�@�C��"���[�U�[�t�H���_/�Z�Z"
                returnPaths.userPath_ExcludeBottom = userPath.Substring(0, userPath.LastIndexOf("\\"));
                // �t�H���_"���[�U�[�t�H���_GrantData/�Z�Z"
                // �t�@�C��"���[�U�[�t�H���_GrantData/�Z�Z"
                returnPaths.grantDataPath_ExcludeBottom = returnPaths.grantDataPath.Substring(0, userPath.LastIndexOf("\\"));
                // �t�H���_"E:/.../ManagementFolderTop/���[�U�[�t�H���_/�Z�Z/�Z�Z"
                // �t�@�C��"E:/.../ManagementFolderTop/���[�U�[�t�H���_/�Z�Z/�Z�Z.�Z�Z"
                returnPaths.fullPath = RootPaths.topFolderFullPath + "\\" + userPath;
                // �t�H���_"E:/.../ManagementFolderTop/���[�U�[�t�H���_GrantData/�Z�Z/�Z�Z"
                // �t�@�C��"E:/.../ManagementFolderTop/���[�U�[�t�H���_GrantData/�Z�Z/�Z�Z.�Z�Z"
                returnPaths.grantDataFullPath = RootPaths.topFolderFullPath + "\\" + returnPaths.grantDataPath;
                // �t�H���_"E:/.../ManagementFolderTop/���[�U�[�t�H���_GrantData/�Z�Z/(�Z�Z)"
                // �t�@�C��"E:/.../ManagementFolderTop/���[�U�[�t�H���_GrantData/�Z�Z/(�Z�Z.�Z�Z)"
                returnPaths.grantDataFullPath_Parentheses = RootPaths.topFolderFullPath + "\\" + returnPaths.grantDataPath_Parentheses;
                // �t�H���_"�Z�Z"
                // �t�@�C��"�Z�Z.�Z�Z"
                returnPaths.pathBottomLayer = userPath.Substring(userPath.LastIndexOf("\\") + 1);

                return returnPaths;
            }
            else
            {//���[�U�[�t�H���_�O
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// base.Awake();�ŌĂяo���ĂˁB
    /// </summary>
    protected virtual void Awake()
    {
        //�t�H���_�����󔒂Ƃ����Ɩ��Ȃ̂�
        if (!IsSafePath(_userTopFolderName,true)) _userTopFolderName = "���[�U�[�t�H���_";
        
        (string topFolderFullPath,
         string userTopFolderPath,
         string userGrantDataTopFolderPath) rootPaths;

        //�X�N���v�g�ň����ŏ�K�t�H���_�̃p�X(���[�U�[�t�H���_�̈��)
        //�u/Assets�v�̕�����������ēK�؂ȃp�X��������
        rootPaths.topFolderFullPath = Application.dataPath;
        rootPaths.topFolderFullPath = rootPaths.topFolderFullPath.Remove(rootPaths.topFolderFullPath.Length - 7, 7);
        rootPaths.topFolderFullPath += "\\ManagementFolderTop";

        //���[�U�[�t�H���_�̍ŏ�K�p�X
        rootPaths.userTopFolderPath = _userTopFolderName;

        //���[�U�[�t�@�C���t�^�f�[�^�̍ŏ�K�p�X
        rootPaths.userGrantDataTopFolderPath = rootPaths.userTopFolderPath + "GrantData";

        RootPaths = rootPaths;

    }

}
