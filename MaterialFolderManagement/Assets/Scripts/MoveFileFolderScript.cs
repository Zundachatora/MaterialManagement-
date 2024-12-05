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
    /// �Z�b�g��GrantData�e�L�X�g�͊����̃t�@�C���t�H���_������Ɛ�������Ȃ��B
    /// </summary>
    /// <param name="grantData"></param>
    /// <param name="destinationFullPath"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public (bool move, ReasonsForFailureEnum reasonsForFailureEnum, string nameSuggestion) MoveFileFolder(FileFoldersGrantData grantData,string destinationFullPath,DateTime moveStartTime, MoveFileFolderOption option = null)
    {

        //�ړ������錳�f�[�^�̃p�X�̊m�F
        var paths = PathConversion(grantData.UserPath);
        if (paths == null) return (false, ReasonsForFailureEnum.InvalidUserPath, null);

        //�ړ���̃t�H���_�̊m�F
        if(!IsSafePath(destinationFullPath,false))return (false, ReasonsForFailureEnum.InvalidDestinationFullPath, null);
        if (!Directory.Exists(destinationFullPath)) return (false, ReasonsForFailureEnum.InvalidDestinationFullPath, null);

        //null�̏ꍇ�A�f�t�H���g�̃I�v�V�������g�p����
        if (option == null) option = new MoveFileFolderOption();

        //�ړ���̃t���p�X���쐬�B�d���`�F�b�N�����˂�B
        string moveFullPath = destinationFullPath.ToString();
        if (string.IsNullOrEmpty(option.NewFileFolderName))
        {
            if (option.Alias)
            {//�t���p�X\\�ʖ�
                if (string.IsNullOrEmpty(grantData.Alias)) return (false, ReasonsForFailureEnum.AliasIsEmpty, null);

                moveFullPath = Path.Combine(moveFullPath, grantData.Alias);

                if (!option.FileFolderOverwrite)
                {//�d���`�F�b�N
                    //�ړ���Ɋ��ɓ������O������Α㗝�̖��O���Ă��ďI������B
                    var duplicate = CheckForDuplicateName(moveFullPath);
                    if (duplicate.duplicates) return (false, ReasonsForFailureEnum.DuplicateNameAtDestination, duplicate.nameSuggestion);
                }
            }
            else
            {//�t���p�X\\���O
                if (string.IsNullOrEmpty(grantData.Name)) return (false, ReasonsForFailureEnum.NameIsEmpty, null);
                    
                moveFullPath = Path.Combine(moveFullPath, grantData.Name);
                
                if (!option.FileFolderOverwrite)
                {//�d���`�F�b�N
                    //�ړ���Ɋ��ɓ������O������Α㗝�̖��O���Ă��ďI������B
                    var duplicate = CheckForDuplicateName(moveFullPath);
                    if (duplicate.duplicates) return (false, ReasonsForFailureEnum.DuplicateNameAtDestination, duplicate.nameSuggestion);
                }
            } 
        }
        else
        {//�t���p�X\\newName

            if (!IsSafePath(option.NewFileFolderName, true)) return (false, ReasonsForFailureEnum.NewNameIsInvalid, null);

            moveFullPath = Path.Combine(moveFullPath, option.NewFileFolderName);

            if (!option.FileFolderOverwrite)
            {//�d���`�F�b�N
                //�ړ���Ɋ��ɓ������O������Α㗝�̖��O���Ă��ďI������B
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

            //�ŏ�����d�����Ȃ��ꍇ
            if(!Directory.Exists(nameSuggestion) && !File.Exists(nameSuggestion)) return (false, nameSuggestion);

            //���E�܂ŏd�����Ȃ����O��T���ĕԂ��B
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

            //�㏑��(overwrite)�o���Ȃ���Έړ���ɖ����t�@�C�������R�s�[����B���t�H���_�͏㏑������Ӗ����Ȃ��̂Ŏg���܂킷�B
            //overwrite=false�ŏ㏑���o���Ȃ��t�@�C���������false��Ԃ��B
            bool FolderCopy(string sourceFullPath, string destinationFullPath, bool overwrite)
            {//�߂�l true�S�Ẵt�@�C�����R�s�[�Bfalse���ɂ���t�@�C���������đS�Ẵt�@�C�����R�s�[�B
                bool skipOnDuplicates = false;

                if (!Directory.Exists(destinationFullPath)) Directory.CreateDirectory(destinationFullPath);

                //�R�s�[�\��̑S�Ẵt�H���_��T���B
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
                        //�R�s�[�̖������[�v�h�~
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
                        //�R�s�[�̖������[�v�h�~
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

            //�e�L�X�g�̒ǉ�
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
        /// <summary>NewFileFolderName������ꍇ�͂����炪�D�悳���</summary>
        public bool Alias = false;
        public bool FileFolderOverwrite = false;
        public string NewFileFolderName = null;
        /// <summary>�ړ��������t�@�C���t�H���_�Ɠ����ꏊ�ɓ���(�g���q��.txt)�̃e�L�X�g�t�@�C�����쐬����B���Ƀt�@�C��������ꍇ�͍쐬���Ȃ�(�������݂����Ȃ�)</summary>
        public bool AddNameText = false;
        /// <summary>�ړ��������t�@�C���t�H���_�Ɠ����ꏊ�ɓ���(�g���q��.txt)�̃e�L�X�g�t�@�C�����쐬����B���Ƀt�@�C��������ꍇ�͍쐬���Ȃ�(�������݂����Ȃ�)</summary>
        public bool AddAliasText = false;
        /// <summary>�ړ��������t�@�C���t�H���_�Ɠ����ꏊ�ɓ���(�g���q��.txt)�̃e�L�X�g�t�@�C�����쐬����B���Ƀt�@�C��������ꍇ�͍쐬���Ȃ�(�������݂����Ȃ�)</summary>
        public bool AddMemorandumText = false;
        /// <summary>�ړ��������t�@�C���t�H���_�Ɠ����ꏊ�ɓ���(�g���q��.txt)�̃e�L�X�g�t�@�C�����쐬����B���Ƀt�@�C��������ꍇ�͍쐬���Ȃ�(�������݂����Ȃ�)</summary>
        public bool AddHashtagText = false;

    }
}
