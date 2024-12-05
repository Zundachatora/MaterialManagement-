using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalSearchScript : PlayingWavFilesScript
{
    public FileFoldersGrantData[] SearchData(FileFoldersGrantData[] grantDatas , SearchConditionData searchConditionData)
    {
        List<FileFoldersGrantData> match = new List<FileFoldersGrantData>();
        List<FileFoldersGrantData> mismatch = new List<FileFoldersGrantData>();

        foreach (var gData in grantDatas)
        {
            if(ConditionMatch(gData))
            {
                match.Add(gData);
            }
            else
            {
                mismatch.Add(gData);
            }
        }
        

        if(searchConditionData.ExcludeMatchingConditions)
        {
            return SortGrantDataWindowsStyle(mismatch.ToArray(), searchConditionData.AscendingOrder);
        }
        else
        {
            return SortGrantDataWindowsStyle(match.ToArray(), searchConditionData.AscendingOrder);
        }

        bool ConditionMatch(FileFoldersGrantData grantData)
        {

            if(grantData.Extension == FileFoldersGrantData.extension.folder)
            {
                if (!searchConditionData.IncludeFolder) return false;
            }
            else
            {
                if (!searchConditionData.IncludeFile) return false;
            }

            if(stringMatch())
            {
                if(searchConditionData.IncludeHashtag.Length == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var searchTag in searchConditionData.IncludeHashtag)
                    {
                        foreach (var tag in grantData.Hashtag)
                        {
                            if (searchTag.Equals(tag)) return true;
                        }
                    }
                }
            }

            return false;

            //ãÛÇÃï∂éöóÒÇÕëSÇƒëŒè€Ç∆ÇµÇƒàµÇ§
            bool stringMatch()
            {
                if (string.IsNullOrEmpty(searchConditionData.SearchString))
                {
                    return true;
                }
                else
                {
                    if (searchConditionData.IncludeName && grantData.Name.Contains(searchConditionData.SearchString)) return true;
                    if (searchConditionData.IncludeAlias && grantData.Alias.Contains(searchConditionData.SearchString)) return true;
                    if (searchConditionData.IncludeMemorandum && grantData.Memorandum.Contains(searchConditionData.SearchString)) return true;
                }

                return false;
            }
        }
    }

    
}

public class SearchConditionData
{
    public bool ExcludeMatchingConditions = false;
    public bool AscendingOrder = false;

    public string SearchString = "";
    public bool IncludeName = false;
    public bool IncludeFile = false;
    public bool IncludeFolder = false;
    public bool IncludeAlias = false;
    public bool IncludeMemorandum = false;
    public string[] IncludeHashtag = new string[0];

}
